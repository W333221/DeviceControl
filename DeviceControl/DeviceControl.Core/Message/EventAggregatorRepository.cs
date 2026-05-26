using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceControl.Core.Message
{
    public class EventAggregatorRepository
    {
        public EventAggregatorRepository()
        {
            eventAggregator = new CustomEventAggregator();
        }

        public IEventAggregator eventAggregator;
        public static EventAggregatorRepository eventRepository = null;

        //单例，保持内存唯一实例
        public static EventAggregatorRepository GetInstance()
        {
            if (eventRepository == null)
            {
                eventRepository = new EventAggregatorRepository();
            }
            return eventRepository;
        }
        /// <summary>
        /// 自定义事件集合器
        /// </summary>
        public class CustomEventAggregator : IEventAggregator
        {
            private readonly Dictionary<Type, EventBase> events = new Dictionary<Type, EventBase>();
            // Captures the sync context for the UI thread when constructed on the UI thread 
            // in a platform agnostic way so it can be used for UI thread dispatching
            private readonly SynchronizationContext syncContext = new SynchronizationContext();

            /// <summary>
            /// Gets the single instance of the event managed by this EventAggregator. Multiple calls to this method with the same <typeparamref name="TEventType"/> returns the same event instance.
            /// </summary>
            /// <typeparam name="TEventType">The type of event to get. This must inherit from <see cref="EventBase"/>.</typeparam>
            /// <returns>A singleton instance of an event object of type <typeparamref name="TEventType"/>.</returns>
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
            public TEventType GetEvent<TEventType>() where TEventType : EventBase, new()
            {
                lock (events)
                {
                    EventBase existingEvent = null;

                    if (!events.TryGetValue(typeof(TEventType), out existingEvent))
                    {
                        TEventType newEvent = new TEventType();
                        newEvent.SynchronizationContext = syncContext;
                        events[typeof(TEventType)] = newEvent;

                        return newEvent;
                    }
                    else
                    {
                        return (TEventType)existingEvent;
                    }
                }
            }
        }
    }
}