using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using DG.Tweening;
using Runtime.Data;
using Runtime.Event;
using UnityCommons;
using UnityEngine;
using UnityEngine.UI;
using EventType = Runtime.Event.EventType;

namespace Runtime.Bars {
    public class BarController : MonoBehaviour, IEventSubscriber {
        [SerializeField] private Image peopleFill;
        [SerializeField] private Image biodiversityFill;
        [SerializeField] private float fillAnimationDuration = 0.25f;
        [Space]
        [SerializeField] private float TotalBuildingRequiredPeople = 15.0f;
        [SerializeField] private float TotalBuildingRequiredBiodiversity = 15.0f;

        private float peopleFillAmount;
        private float biodiversityFillAmount;
        private float pollution;
        
        private float totalBuiltPeople;
        private float totalBuiltBiodiversity;
        private List<IDisposable> eventUnsubscribeTokens;

        // means that 100% clean will only account to this much of a bar being full
        private const float maxPollutionContribution = 0.5f;

        private void Awake() {
            eventUnsubscribeTokens = new List<IDisposable> {
                this.Subscribe(EventType.PollutionUpdate),
                this.Subscribe(EventType.PerformBuild)
            };

            totalBuiltPeople = 0.0f;
            totalBuiltBiodiversity = 0.0f;
            UpdateFillValue();
            UpdateFill(peopleFill, peopleFillAmount);
            UpdateFill(biodiversityFill, biodiversityFillAmount);
        }

        private void OnDestroy() {
            foreach (var eventUnsubscribeToken in eventUnsubscribeTokens) {
                eventUnsubscribeToken.Dispose();
            }

            eventUnsubscribeTokens.Clear();
        }

        private void UpdateFill(Image image, float fillAmount) {
            image.DOKill();
            image.DOFillAmount(fillAmount, fillAnimationDuration);
            Debug.Log($"Updating {image} fillAmount to [{fillAmount}] from [{image.fillAmount}]");
        }

        /// <summary>
        /// <para>Receives an event from the Event Queue</para>
        /// </summary>
        /// <param name="eventData">Event data raised</param>
        /// <returns><c>true</c> if event propagation should be stopped, <c>false</c> otherwise.</returns>
        public bool OnEvent(EventData eventData) {
            switch (eventData) {
                case PollutionUpdateEvent pollutionUpdateEvent: {
                    UpdateBasedOnPollution(pollutionUpdateEvent.PollutionRatio);
                    return false;
                }
                case PerformBuildEvent performBuildEvent: {
                    UpdateBasedOnBuild(performBuildEvent.BuildableObject);
                    return false;
                }
                default: return false;
            }
        }

        private void UpdateBasedOnBuild(BuildableObject buildableObject) {
            totalBuiltPeople += buildableObject.PeopleHappinessAmount;
            totalBuiltBiodiversity += buildableObject.BiodiversityHappinessAmount;
            
            UpdateFillValue();
            UpdateFill(peopleFill, peopleFillAmount);
            UpdateFill(biodiversityFill, biodiversityFillAmount);
        }

        private void UpdateBasedOnPollution(float pollutionRatio) {
            pollution = CalcPollution(pollutionRatio);
            
            UpdateFillValue();
            UpdateFill(peopleFill, peopleFillAmount);
            UpdateFill(biodiversityFill, biodiversityFillAmount);
        }

        private void UpdateFillValue() {
            peopleFillAmount = pollution + CalcHalfFull_BuildingValue(totalBuiltPeople, TotalBuildingRequiredPeople);
            biodiversityFillAmount = pollution + CalcHalfFull_BuildingValue(totalBuiltBiodiversity, TotalBuildingRequiredBiodiversity);
            EventQueue.QueueEvent(new BarUpdateEvent(this, peopleFillAmount, biodiversityFillAmount));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private float CalcHalfFull_BuildingValue(float actual, float required)
            => 0.5f * (actual / required);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private float CalcPollution(float pollutionRatio)
                // invert                                             
            => ((1.0f - pollutionRatio)
                // reduce
              - maxPollutionContribution)
                // clamp to maxContribution
                .Clamped(0.0f, maxPollutionContribution.Clamped01());
    }
}