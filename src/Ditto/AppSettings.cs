using System;
using System.Collections.Generic;

namespace Ditto
{
    /// <summary>
    /// The application settings
    /// </summary>
    public class AppSettings
    {
        /// <summary>
        /// Gets or sets the connection string to the source Event Store
        /// </summary>
        public string SourceEventStoreConnectionString { get; set; }

        /// <summary>
        /// Gets or sets the connection string to the destination Event Store
        /// </summary>
        public string DestinationEventStoreConnectionString { get; set; }

        /// <summary>
        /// Gets or sets the time in milliseconds to wait between event handling
        /// </summary>
        public int? ReplicationThrottleInterval { get; set; }

        /// <summary>
        /// Gets the event store subscriptions to replicate
        /// </summary>
        public IEnumerable<Subscription> Subscriptions { get; set; }

        /// <summary>
        /// Gets the buffer size for subscriptions
        /// </summary>
        public int? PersistentSubscriptionBufferSize { get; set; }

        public class Subscription
        {
            public string StreamName { get; set; }
            public string GroupName { get; set; }
        }
    }
}