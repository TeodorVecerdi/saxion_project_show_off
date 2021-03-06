using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using DG.Tweening;
using Runtime.Data;
using Runtime.Event;
using UnityCommons;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using EventType = Runtime.Event.EventType;

namespace Runtime.Bars {
    public class BarController : MonoBehaviour, IEventSubscriber {
        [SerializeField] private ScoreUI scoreUI;
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

            if (peopleFillAmount <= 0.01f || biodiversityFillAmount <= 0.01f) {
                PlayerPrefs.SetInt("Score", scoreUI.Score);
                PlayerPrefs.Save();
                Time.timeScale = 1.0f;
                EventQueue.RaiseEventImmediately(new ChangeMouseLockEvent(this, false));
                SceneManager.LoadScene(3);
            } else if (peopleFillAmount >= 0.99f && biodiversityFillAmount >= 0.99f) {
                PlayerPrefs.SetInt("Score", scoreUI.Score);
                PlayerPrefs.Save();
                EventQueue.RaiseEventImmediately(new ChangeMouseLockEvent(this, false));
                Time.timeScale = 1.0f;
                SceneManager.LoadScene(4);
            }
            
            EventQueue.QueueEvent(new BarUpdateEvent(this, peopleFillAmount, biodiversityFillAmount));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private float CalcHalfFull_BuildingValue(float actual, float required)
            => 0.5f * (actual / required);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private float CalcPollution(float pollutionRatio)
            => (1.0f - pollutionRatio - maxPollutionContribution).Clamped(0.0f, maxPollutionContribution);
    }
}