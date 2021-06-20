using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Runtime;
using Runtime.Data;
using Runtime.Event;
using static Runtime.Data.TrashMaterial.Types;
using EventType = Runtime.Event.EventType;

namespace Tests {
    public class PlayerInventory {
        private GameObject utilityObject;
        private GameObject testObject;

        private Runtime.PlayerInventory playerInventory;

        private AssertionTestSubscriber assertionTestSubscriber;
        private List<TrashPickup> pickups;
        private Dictionary<TrashMaterial.Types, TrashMaterial> trash;

        [OneTimeSetUp, UnitySetUp]
        public void Setup() {
            utilityObject = new GameObject("Utility Scripts", typeof(Runtime.Event.EventQueue));

            LogAssert.ignoreFailingMessages = true;
            testObject = new GameObject("GameObject", typeof(Runtime.PlayerInventory));

            playerInventory = testObject.GetComponent<Runtime.PlayerInventory>();

            assertionTestSubscriber = new AssertionTestSubscriber();

            trash = Resources.LoadAll<TrashMaterial>("Trash Materials").ToDictionary(material => material.Type, material => material);
            pickups = Resources.LoadAll<TrashPickup>("Trash Pickups").ToList();
        }

        [OneTimeTearDown, UnityTearDown]
        public void TearDown() {
            GameObject.DestroyImmediate(utilityObject);
            GameObject.DestroyImmediate(testObject);
            assertionTestSubscriber.Clean();
        }

        [UnityTest]
        public IEnumerator InventorySpaceRequestSuccess() {
            var remainingSpace = playerInventory.MaximumCarryMass - playerInventory.MaterialInventory.TotalMass;
            var spaceRequestEvent0 = new TrashPickupSpaceRequest(this, 0);
            var spaceRequestEvent1 = new TrashPickupSpaceRequest(this, remainingSpace);

            assertionTestSubscriber.Clean();
            assertionTestSubscriber.Prepare(EventType.TrashPickupSpaceResponse);

            Runtime.Event.EventQueue.QueueEvent(spaceRequestEvent0);
            yield return AssertReceivedAndMatches(eventData => eventData is TrashPickupSpaceResponse {CanPickUp: true}, 2);

            Runtime.Event.EventQueue.QueueEvent(spaceRequestEvent1);
            yield return AssertReceivedAndMatches(eventData => eventData is TrashPickupSpaceResponse {CanPickUp: true}, 2);

            assertionTestSubscriber.Clean();
        }

        [UnityTest]
        public IEnumerator InventorySpaceRequestFail() {
            var remainingSpace = playerInventory.MaximumCarryMass - playerInventory.MaterialInventory.TotalMass;
            var spaceRequestEvent = new TrashPickupSpaceRequest(this, remainingSpace + 1);

            assertionTestSubscriber.Clean();
            assertionTestSubscriber.Prepare(EventType.TrashPickupSpaceResponse);

            Runtime.Event.EventQueue.QueueEvent(spaceRequestEvent);
            yield return AssertReceivedAndMatches(eventData => eventData is TrashPickupSpaceResponse {CanPickUp: false}, 2);

            assertionTestSubscriber.Clean();
        }

        [UnityTest]
        public IEnumerator DepositMaterialCorrect() {
            assertionTestSubscriber.Clean();
            assertionTestSubscriber.Prepare(EventType.InventoryUpdate, EventType.DepositInventoryUpdate);

            playerInventory.MaterialInventory.Add(trash[Plastic], 20.0f);
            var depositInventory = new MaterialInventory {
                {trash[Plastic], 100.0f}, 
                {trash[Metal], 100.0f}, 
                {trash[Paper], 100.0f}
            };

            var oldPlayerMass = playerInventory.MaterialInventory.TotalMass;
            var oldDepositMass = depositInventory.TotalMass;

            Runtime.Event.EventQueue.QueueEvent(new DepositMaterialsRequestEvent(this, depositInventory));

            yield return AssertReceivedAndMatches(data => data is MaterialInventoryUpdateEvent updateEvent && updateEvent.Inventory == playerInventory.MaterialInventory, 2);
            yield return AssertReceivedAndMatches(data => data is DepositInventoryUpdateEvent updateEvent && updateEvent.Inventory == depositInventory, 0);

            Assert.Zero(playerInventory.MaterialInventory.TotalMass, "Player Inventory Mass should be zero after a deposit, but was not");
            Assert.AreEqual(oldPlayerMass + oldDepositMass, depositInventory.TotalMass, "materialDeposit.Inventory.TotalMass != oldPlayerMass + oldDepositMass");

            assertionTestSubscriber.Clean();
        }

        [UnityTest]
        public IEnumerator TrashPickupFailsWhenMassExceedsInventorySize() {
            var remainingSpace = playerInventory.MaximumCarryMass - playerInventory.MaterialInventory.TotalMass;
            var trashPickup = GameObject.Instantiate(pickups[0].Prefab);
            trashPickup.Load(pickups[0]);

            // Set mass manually
            var massProp = trashPickup.GetType().GetProperty("Mass", BindingFlags.Instance | BindingFlags.Public);
            massProp.SetValue(trashPickup, remainingSpace + 1.0f);

            assertionTestSubscriber.Clean();
            assertionTestSubscriber.Prepare(EventType.TrashPickupSuccess, EventType.InventoryUpdate);
            var trashEvent = new TrashEvent(this, EventType.TrashPickupRequest, trashPickup);
            Runtime.Event.EventQueue.QueueEvent(trashEvent);

            yield return AssertNotReceivedEvent(EventType.TrashPickupSuccess, 2);
            yield return AssertNotReceivedEvent(EventType.InventoryUpdate, 0);

            assertionTestSubscriber.Clean();
        }

        [UnityTest]
        public IEnumerator TrashPickupSucceedsWhenMassLessThanInventorySize() {
            var remainingSpace = playerInventory.MaximumCarryMass - playerInventory.MaterialInventory.TotalMass;
            var trashPickup = GameObject.Instantiate(pickups[0].Prefab);
            trashPickup.Load(pickups[0]);

            // Set mass manually
            var massProp = trashPickup.GetType().GetProperty("Mass", BindingFlags.Instance | BindingFlags.Public);
            massProp.SetValue(trashPickup, remainingSpace);

            assertionTestSubscriber.Clean();
            assertionTestSubscriber.Prepare(EventType.TrashPickupSuccess, EventType.InventoryUpdate);

            var trashEvent = new TrashEvent(this, EventType.TrashPickupRequest, trashPickup);
            Runtime.Event.EventQueue.QueueEvent(trashEvent);

            yield return AssertReceivedEvent(EventType.TrashPickupSuccess, 2);
            yield return AssertReceivedEvent(EventType.InventoryUpdate, 0);

            assertionTestSubscriber.Clean();
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

        private IEnumerator AssertReceivedAndMatches(Func<EventData, bool> match, int waitFrames = 1) {
            for (var i = 0; i < waitFrames; i++) {
                yield return null;
            }

            Assert.True(assertionTestSubscriber.ReceivedEvents.Any(match), $"Expected to receive an event that matches condition, but did not.");
        }

        private class EventTester : IEventSubscriber {
            private readonly List<(Func<EventData, bool> match, Action<EventData> action)> performEvents = new List<(Func<EventData, bool> match, Action<EventData> action)>();
            private readonly List<IDisposable> unsubscribeTokens = new List<IDisposable>();

            public void Subscribe(EventType eventType, Func<EventData, bool> match, Action<EventData> action) {
                unsubscribeTokens.Add(this.Subscribe(eventType));
                performEvents.Add((match, action));
            }

            public bool OnEvent(EventData eventData) {
                foreach (var (match, action) in performEvents) {
                    if (match(eventData)) action(eventData);
                }

                return false;
            }
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