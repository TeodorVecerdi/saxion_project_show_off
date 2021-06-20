using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Runtime;
using Runtime.Event;
using UnityCommons;
using UnityEngine;
using UnityEngine.TestTools;
using EventType = Runtime.Event.EventType;

namespace Tests {
    public class EventQueueTests {
        private GameObject utilityObject;
        private AssertionTestSubscriber assertionTestSubscriber;
        
        [OneTimeSetUp, UnitySetUp]
        public void Setup() {
            utilityObject = new GameObject("Utility GameObject", typeof(EventQueue));
            assertionTestSubscriber = new AssertionTestSubscriber();
        }
        
        [OneTimeTearDown, UnityTearDown]
        public void TearDown() {
            GameObject.DestroyImmediate(utilityObject);
            assertionTestSubscriber.Clean();
        }

        [UnityTest] public IEnumerator RaiseImmediatelyReceived() {
            assertionTestSubscriber.Clean();
            assertionTestSubscriber.Prepare(EventType.BeginBuild, EventType.CancelBuild, EventType.PerformBuild);
            
            EventQueue.RaiseEventImmediately(new EmptyEvent(this, EventType.BeginBuild));
            EventQueue.RaiseEventImmediately(new EmptyEvent(this, EventType.CancelBuild));
            EventQueue.RaiseEventImmediately(new EmptyEvent(this, EventType.PerformBuild));
            
            yield return AssertReceivedEvent(EventType.BeginBuild, 0);
            yield return AssertReceivedEvent(EventType.CancelBuild, 0);
            yield return AssertReceivedEvent(EventType.PerformBuild, 0);
        }
        
        [UnityTest] public IEnumerator RaiseImmediatelyNotReceived() {
            assertionTestSubscriber.Clean();
            assertionTestSubscriber.Prepare(EventType.BeginBuild, EventType.CancelBuild, EventType.PerformBuild);
            
            EventQueue.RaiseEventImmediately(new EmptyEvent(this, EventType.DepositInventoryUpdate));
            EventQueue.RaiseEventImmediately(new EmptyEvent(this, EventType.DepositInventoryRequest));
            EventQueue.RaiseEventImmediately(new EmptyEvent(this, EventType.DepositInventoryResponse));
            
            yield return AssertNotReceivedEvent(EventType.DepositInventoryUpdate, 0);
            yield return AssertNotReceivedEvent(EventType.DepositInventoryRequest, 0);
            yield return AssertNotReceivedEvent(EventType.DepositInventoryResponse, 0);
        }
        
        [UnityTest] public IEnumerator QueueReceived() {
            assertionTestSubscriber.Clean();
            assertionTestSubscriber.Prepare(EventType.BeginBuild, EventType.CancelBuild, EventType.PerformBuild);
            
            EventQueue.QueueEvent(new EmptyEvent(this, EventType.BeginBuild));
            EventQueue.QueueEvent(new EmptyEvent(this, EventType.CancelBuild));
            EventQueue.QueueEvent(new EmptyEvent(this, EventType.PerformBuild));
            
            yield return AssertReceivedEvent(EventType.BeginBuild);
            yield return AssertReceivedEvent(EventType.CancelBuild);
            yield return AssertReceivedEvent(EventType.PerformBuild);
        }
        
        [UnityTest] public IEnumerator QueueNotReceived() {
            assertionTestSubscriber.Clean();
            assertionTestSubscriber.Prepare(EventType.BeginBuild, EventType.CancelBuild, EventType.PerformBuild);
            
            EventQueue.QueueEvent(new EmptyEvent(this, EventType.DepositInventoryUpdate));
            EventQueue.QueueEvent(new EmptyEvent(this, EventType.DepositInventoryRequest));
            EventQueue.QueueEvent(new EmptyEvent(this, EventType.DepositInventoryResponse));
            
            yield return AssertNotReceivedEvent(EventType.DepositInventoryUpdate);
            yield return AssertNotReceivedEvent(EventType.DepositInventoryRequest);
            yield return AssertNotReceivedEvent(EventType.DepositInventoryResponse);
        }

        [UnityTest] public IEnumerator EventDataPreserved() {
            assertionTestSubscriber.Clean();
            assertionTestSubscriber.Prepare(EventType.ChangeMouseLock, EventType.GameModeChange, EventType.SettingsChanged);
            var event1 = new ChangeMouseLockEvent(this, Rand.Bool);
            var event2 = new EmptyEvent(this, EventType.GameModeChange);
            var event3 = new SettingsChangedEvent(this, Rand.Bool, Rand.Float, Rand.Float, Rand.Float);
            EventQueue.QueueEvent(event1);
            EventQueue.QueueEvent(event2);
            EventQueue.QueueEvent(event3);

            yield return AssertReceivedExactEvent(event1);
            yield return AssertReceivedExactEvent(event2);
            yield return AssertReceivedExactEvent(event3);
        }

        [UnityTest] public IEnumerator SubscribeWorks() {
            assertionTestSubscriber.Clean();
            EventQueue.RaiseEventImmediately(new EmptyEvent(this, EventType.BeginBuild));
            yield return AssertNotReceivedEvent(EventType.BeginBuild, 0);
            assertionTestSubscriber.ReceivedEvents.Clear();
            using (assertionTestSubscriber.Subscribe(EventType.BeginBuild)) {
                EventQueue.RaiseEventImmediately(new EmptyEvent(this, EventType.BeginBuild));
                yield return AssertReceivedEvent(EventType.BeginBuild, 0);
            }
        }
        
        [UnityTest] public IEnumerator UnsubscribeWorks() {
            assertionTestSubscriber.Clean();
            using (assertionTestSubscriber.Subscribe(EventType.BeginBuild)) {
                EventQueue.RaiseEventImmediately(new EmptyEvent(this, EventType.BeginBuild));
                yield return AssertReceivedEvent(EventType.BeginBuild, 0);
            }
            assertionTestSubscriber.ReceivedEvents.Clear();
            EventQueue.RaiseEventImmediately(new EmptyEvent(this, EventType.BeginBuild));
            yield return AssertNotReceivedEvent(EventType.BeginBuild, 0);
        }
        

        private IEnumerator AssertReceivedEvent(EventType type, int waitFrames = 1) {
            for (var i = 0; i < waitFrames; i++) {
                yield return null;
            }
            Assert.True(assertionTestSubscriber.ReceivedEvents.Any(data => data.Type == type), $"Expected to receive an event of type {type}, but did not receive");
        }
        
        private IEnumerator AssertNotReceivedEvent(EventType type, int waitFrames = 1) {
            for (var i = 0; i < waitFrames; i++) {
                yield return null;
            }
            Assert.False(assertionTestSubscriber.ReceivedEvents.Any(data => data.Type == type), $"Expected to not receive any event of type {type}, but received");
        }
        
        private IEnumerator AssertReceivedExactEvent(EventData @event, int waitFrames = 1) {
            for (var i = 0; i < waitFrames; i++) {
                yield return null;
            }
            Assert.True(assertionTestSubscriber.ReceivedEvents.Any(data => data == @event), $"Expected to receive an exact event (of type {@event.Type}), but did not.");
        }

        private class AssertionTestSubscriber : IEventSubscriber {
            public readonly List<EventData> ReceivedEvents = new List<EventData>();
            private readonly List<IDisposable> unsubscribeTokens = new List<IDisposable>();

            public void Prepare(params EventType[] eventTypes) {
                foreach (var eventType in eventTypes) {
                    unsubscribeTokens.Add(this.Subscribe(eventType));
                }
            }

            public void Clean() {
                ReceivedEvents.Clear();
                foreach (var unsubscribeToken in unsubscribeTokens) {
                    unsubscribeToken.Dispose();
                }
                unsubscribeTokens.Clear();
            }
            
            public bool OnEvent(EventData eventData) {
                ReceivedEvents.Add(eventData);
                return false;
            }
        }
    }
}