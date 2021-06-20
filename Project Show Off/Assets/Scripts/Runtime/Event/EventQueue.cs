using System;
using System.Collections.Generic;
using DG.Tweening.Core;
using JetBrains.Annotations;
using UnityCommons;
using UnityEngine;

namespace Runtime.Event {
    public sealed class EventQueue : MonoSingleton<EventQueue> {
        private static readonly bool debug = false;
        private readonly Dictionary<EventType, List<IEventSubscriber>> subscribers = new Dictionary<EventType, List<IEventSubscriber>>();
        private readonly Queue<EventData> eventQueue = new Queue<EventData>();
        
        /// <inheritdoc cref="SubscribeWithPriority_Impl"/>
        [MustUseReturnValue] public static IDisposable SubscribeWithPriority(IEventSubscriber subscriber, EventType eventType, int priority) => Instance.SubscribeWithPriority_Impl(subscriber, eventType, priority);

        /// <summary>
        /// Subscribes <paramref name="subscriber"/> to receive events of type <paramref name="eventType"/>.
        /// </summary>
        /// <param name="subscriber">The subscriber which wants to receive events</param>
        /// <param name="eventType">The type of event that the subscriber wants to receive</param>
        /// <returns>An IDisposable that allows the subscriber to unsubscribe from the Event Queue by calling <c>Dispose()</c></returns>
        [MustUseReturnValue] public static IDisposable Subscribe(IEventSubscriber subscriber, EventType eventType) => Instance.SubscribeWithPriority_Impl(subscriber, eventType, int.MaxValue);

        
        /// <summary>
        /// Queues an event for raising during the next update loop.
        /// </summary>
        /// <param name="eventData">Event data that will be raised</param>
        public static void QueueEvent(EventData eventData) {
            if (debug) {
                Debug.Log($"Queued event {eventData} from {eventData.Sender}");
            }
            Instance.eventQueue.Enqueue(eventData);
        }

        /// <summary>
        /// Raises an event immediately
        /// </summary>
        /// <param name="eventData">Event data that is raised</param>
        public static void RaiseEventImmediately(EventData eventData) => Instance.EmitEvent(eventData);

        /// <summary>
        /// Subscribes <paramref name="subscriber"/> to receive events of type <paramref name="eventType"/> with a priority.
        /// </summary>
        /// <param name="eventType">The type of event that the subscriber wants to receive</param>
        /// <param name="subscriber">The subscriber which wants to receive events</param>
        /// <param name="priority">Priority to receive events. The lower this value is, the sooner the subscriber will receive the event</param>
        /// <returns>An IDisposable that allows the subscriber to unsubscribe from the Event Queue by calling <c>Dispose()</c></returns>
        private IDisposable SubscribeWithPriority_Impl(IEventSubscriber subscriber, EventType eventType, int priority) {
            if(!subscribers.ContainsKey(eventType)) subscribers.Add(eventType, new List<IEventSubscriber>());
            if (subscribers[eventType].Contains(subscriber)) {
                Debug.LogWarning($"Attempting to subscribe {subscriber.GetType()} multiple times to the same event [{eventType}]. Ignoring subsequent subscriptions");
                return new DummyDisposable();
            }

            if (priority < 0) priority = 0;
            if (priority > subscribers[eventType].Count) priority = subscribers[eventType].Count;
            subscribers[eventType].Insert(priority, subscriber);
            return new UnsubscribeToken(this, eventType, subscriber);
        }

        /// <summary>
        /// Unsubscribes <paramref name="subscriber"/> from receiving events of type <paramref name="eventType"/>.
        /// </summary>
        /// <param name="subscriber">The subscriber which wants to stop receiving events</param>
        /// <param name="eventType">The type of event the subscriber wants to stop receiving</param>
        private void Unsubscribe(IEventSubscriber subscriber, EventType eventType) {
            if (!subscribers.ContainsKey(eventType) || !subscribers[eventType].Contains(subscriber)) {
                Debug.LogWarning($"Attempting to unsubscribe {subscriber.GetType()} from receiving {eventType} events when not subscribed. Ignoring.");
                return;
            }

            subscribers[eventType].Remove(subscriber);
        }
        
        /// <summary>
        /// Broadcasts an event to all subscribers
        /// </summary>
        /// <param name="eventData">Event data that is raised</param>
        private void EmitEvent(EventData eventData) {
            if (!subscribers.ContainsKey(eventData.Type) || subscribers[eventData.Type].Count == 0) {
                if (debug) {
                    Debug.LogWarning($"An event of type {eventData.Type} was raised but there is no subscriber for that event type");
                }
                return;
            }
            
            foreach (var subscriber in subscribers[eventData.Type]) {
                if (debug) {
                    Debug.Log($"Sent event {eventData}[Type={eventData.Type}] from {eventData.Sender} to {subscriber}");
                }
                if (subscriber.OnEvent(eventData)) {
                    // stop propagation if event was consumed
                    break;
                }
            }
        }

        private void Update() {
            while (eventQueue.Count > 0) {
                var @event = eventQueue.Dequeue();
                EmitEvent(@event);
            }
        }
        
        /// <summary>
        /// Used for invalid subscriptions that fail (mostly) silently.
        /// </summary>
        private class DummyDisposable : IDisposable {
            public void Dispose() { /* empty */ }
        }

        private class UnsubscribeToken : IDisposable {
            private readonly EventQueue source;
            private readonly EventType eventType;
            private readonly IEventSubscriber owner;
            
            private bool disposed;

            public UnsubscribeToken(EventQueue source, EventType eventType, IEventSubscriber owner) {
                this.source = source;
                this.eventType = eventType;
                this.owner = owner;
            }
            
            public void Dispose() {
                if (disposed) {
                    return;
                }

                if (debug) {
                    Debug.Log($"Unsubscribed {owner} from receiving events of type {eventType}");
                }

                source.Unsubscribe(owner, eventType);

                disposed = true;
            }
        }
    }
}