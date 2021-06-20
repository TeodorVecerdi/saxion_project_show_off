using System;
using JetBrains.Annotations;
using Runtime.Event;

namespace Runtime {
    public static class Extensions {
        [MustUseReturnValue] 
        public static IDisposable Subscribe(this IEventSubscriber eventSubscriber, EventType eventType) {
            return EventQueue.Subscribe(eventSubscriber, eventType);
        }
    }
}