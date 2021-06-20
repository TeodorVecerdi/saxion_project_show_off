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
using UnityCommons;
using static Runtime.Data.TrashMaterial.Types;
using EventType = Runtime.Event.EventType;

namespace Tests {
    public class DepositTests {
        private GameObject utilityObject;
        private GameObject testObject;

        private MaterialDeposit materialDeposit;

        private AssertionTestSubscriber assertionTestSubscriber;
        private List<BuildableObject> buildableObjects;
        private Dictionary<TrashMaterial.Types, TrashMaterial> trash;
        private List<TrashPickup> pickups;

        [OneTimeSetUp, UnitySetUp]
        public void Setup() {
            utilityObject = new GameObject("Utility Scripts", typeof(EventQueue));

            LogAssert.ignoreFailingMessages = true;
            testObject = new GameObject("GameObject");
            try {
                testObject.AddComponent<MaterialDeposit>();
            } catch {
                // oof
            }

            materialDeposit = testObject.GetComponent<MaterialDeposit>();

            assertionTestSubscriber = new AssertionTestSubscriber();

            trash = Resources.LoadAll<TrashMaterial>("Trash Materials").ToDictionary(material => material.Type, material => material);
            buildableObjects = Resources.LoadAll<BuildableObject>("Buildable Objects").ToList();
            pickups = Resources.LoadAll<TrashPickup>("Trash Pickups").ToList();
        }

        [OneTimeTearDown, UnityTearDown]
        public void TearDown() {
            GameObject.DestroyImmediate(utilityObject);
            GameObject.DestroyImmediate(testObject);
            assertionTestSubscriber.Clean();
        }

        [UnityTest]
        public IEnumerator Deposit_PerformBuildEventCorrect() {
            materialDeposit.Inventory.Clear();
            materialDeposit.Inventory.Add(trash[Glass], Mathf.Floor(Rand.Float * 1000));
            materialDeposit.Inventory.Add(trash[Metal], Mathf.Floor(Rand.Float * 1000));
            materialDeposit.Inventory.Add(trash[Paper], Mathf.Floor(Rand.Float * 1000));
            materialDeposit.Inventory.Add(trash[Plastic], Mathf.Floor(Rand.Float * 1000));

            var buildable = Rand.ListItem(buildableObjects);
            var expectedMaterials = new MaterialInventory {materialDeposit.Inventory};
            expectedMaterials.Remove(buildable.ConstructionRequirements);
            
            assertionTestSubscriber.Clean();
            assertionTestSubscriber.Prepare(EventType.DepositInventoryUpdate);
            
            EventQueue.QueueEvent(new PerformBuildEvent(this, buildable));
            
            yield return AssertReceivedEvent(EventType.DepositInventoryUpdate, 2);
            Assert.IsTrue(expectedMaterials.Equals(materialDeposit.Inventory), "expectedMaterials.Equals(materialDeposit.Inventory)");
            
            assertionTestSubscriber.Clean();
        }

        [UnityTest]
        public IEnumerator Deposit_TrashPickupBinUpdatesInventory() {
            materialDeposit.Inventory.Clear();
            materialDeposit.Inventory.Add(trash[Glass], Mathf.Floor(Rand.Float * 1000));
            materialDeposit.Inventory.Add(trash[Metal], Mathf.Floor(Rand.Float * 1000));
            materialDeposit.Inventory.Add(trash[Paper], Mathf.Floor(Rand.Float * 1000));
            materialDeposit.Inventory.Add(trash[Plastic], Mathf.Floor(Rand.Float * 1000));

            var pickup = Rand.ListItem(pickups);
            var mass = Mathf.Floor(Rand.Float * 1000);
            var expectedMaterials = new MaterialInventory {materialDeposit.Inventory, {pickup.TrashMaterial, mass}};

            assertionTestSubscriber.Clean();
            assertionTestSubscriber.Prepare(EventType.DepositInventoryUpdate);

            EventQueue.QueueEvent(new TrashPickupBinEvent(this, pickup, mass));
            
            yield return AssertReceivedEvent(EventType.DepositInventoryUpdate, 2);
            Assert.IsTrue(expectedMaterials.Equals(materialDeposit.Inventory), "expectedMaterials.Equals(materialDeposit.Inventory)");
        }

        private IEnumerator AssertReceivedEvent(EventType type, int waitFrames = 1) {
            for (var i = 0; i < waitFrames; i++) {
                yield return null;
            }

            Assert.IsTrue(assertionTestSubscriber.ReceivedEvents.Any(data => data.Type == type), $"Expected to receive an event of type {type}, but did not receive");
        }

        private IEnumerator AssertNotReceivedEvent(EventType type, int waitFrames = 1) {
            for (var i = 0; i < waitFrames; i++) {
                yield return null;
            }

            Assert.IsFalse(assertionTestSubscriber.ReceivedEvents.Any(data => data.Type == type), $"Expected to not receive any event of type {type}, but received");
        }

        private IEnumerator AssertReceivedExactEvent(EventData @event, int waitFrames = 1) {
            for (var i = 0; i < waitFrames; i++) {
                yield return null;
            }

            Assert.IsTrue(assertionTestSubscriber.ReceivedEvents.Any(data => data == @event), $"Expected to receive an exact event (of type {@event.Type}), but did not.");
        }

        private IEnumerator AssertReceivedAndMatches(Func<EventData, bool> match, int waitFrames = 1) {
            for (var i = 0; i < waitFrames; i++) {
                yield return null;
            }

            Assert.IsTrue(assertionTestSubscriber.ReceivedEvents.Any(match), $"Expected to receive an event that matches condition, but did not.");
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