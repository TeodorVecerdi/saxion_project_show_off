﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Runtime;
using Runtime.Event;
using UnityEngine;
using UnityEngine.TestTools;
using EventType = Runtime.Event.EventType;

namespace Tests {
    public class EventQueueTests {
        private GameObject utilityObject;
        private AssertionTestSubscriber assertionTestSubscriber;
        
        [OneTimeSetUp]
        public void Setup() {
            utilityObject = new GameObject("Utility GameObject", typeof(EventQueue));
            assertionTestSubscriber = new AssertionTestSubscriber();
        }

        [UnityTest] public IEnumerator EventQueue_RaiseImmediatelyReceived() {
            assertionTestSubscriber.Clean();
            assertionTestSubscriber.Prepare(EventType.BeginBuild, EventType.CancelBuild, EventType.PerformBuild);
            
            EventQueue.RaiseEventImmediately(new EmptyEvent(this, EventType.BeginBuild));
            EventQueue.RaiseEventImmediately(new EmptyEvent(this, EventType.CancelBuild));
            EventQueue.RaiseEventImmediately(new EmptyEvent(this, EventType.PerformBuild));
            
            yield return AssertReceivedEvent(EventType.BeginBuild, 0);
            yield return AssertReceivedEvent(EventType.CancelBuild, 0);
            yield return AssertReceivedEvent(EventType.PerformBuild, 0);
        }
        
        [UnityTest] public IEnumerator EventQueue_RaiseImmediatelyNotReceived() {
            assertionTestSubscriber.Clean();
            assertionTestSubscriber.Prepare(EventType.BeginBuild, EventType.CancelBuild, EventType.PerformBuild);
            
            EventQueue.RaiseEventImmediately(new EmptyEvent(this, EventType.DepositInventoryUpdate));
            EventQueue.RaiseEventImmediately(new EmptyEvent(this, EventType.DepositInventoryRequest));
            EventQueue.RaiseEventImmediately(new EmptyEvent(this, EventType.DepositInventoryResponse));
            
            yield return AssertNotReceivedEvent(EventType.DepositInventoryUpdate, 0);
            yield return AssertNotReceivedEvent(EventType.DepositInventoryRequest, 0);
            yield return AssertNotReceivedEvent(EventType.DepositInventoryResponse, 0);
        }
        
        [UnityTest] public IEnumerator EventQueue_QueueReceived() {
            assertionTestSubscriber.Clean();
            assertionTestSubscriber.Prepare(EventType.BeginBuild, EventType.CancelBuild, EventType.PerformBuild);
            
            EventQueue.QueueEvent(new EmptyEvent(this, EventType.BeginBuild));
            EventQueue.QueueEvent(new EmptyEvent(this, EventType.CancelBuild));
            EventQueue.QueueEvent(new EmptyEvent(this, EventType.PerformBuild));
            
            yield return AssertReceivedEvent(EventType.BeginBuild);
            yield return AssertReceivedEvent(EventType.CancelBuild);
            yield return AssertReceivedEvent(EventType.PerformBuild);
        }
        
        [UnityTest] public IEnumerator EventQueue_QueueNotReceived() {
            assertionTestSubscriber.Clean();
            assertionTestSubscriber.Prepare(EventType.BeginBuild, EventType.CancelBuild, EventType.PerformBuild);
            
            EventQueue.QueueEvent(new EmptyEvent(this, EventType.DepositInventoryUpdate));
            EventQueue.QueueEvent(new EmptyEvent(this, EventType.DepositInventoryRequest));
            EventQueue.QueueEvent(new EmptyEvent(this, EventType.DepositInventoryResponse));
            
            yield return AssertNotReceivedEvent(EventType.DepositInventoryUpdate);
            yield return AssertNotReceivedEvent(EventType.DepositInventoryRequest);
            yield return AssertNotReceivedEvent(EventType.DepositInventoryResponse);
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