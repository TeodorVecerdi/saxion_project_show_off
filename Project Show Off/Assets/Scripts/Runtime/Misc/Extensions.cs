using System;
using Runtime.Event;

namespace Runtime {
    public static class Extensions {
        public static IDisposable Subscribe(this IEventSubscriber eventSubscriber, EventType eventType) {
            return EventQueue.Subscribe(eventSubscriber, eventType);
        }
    }
}