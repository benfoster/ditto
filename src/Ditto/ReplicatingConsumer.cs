using System;
using System.Threading;
using EventStore.ClientAPI;
using SerilogTimings.Extensions;

namespace Ditto
{
    /// <summary>
    /// Stream consumer that replicates to the destination event store
    /// </summary>
    public class ReplicatingConsumer : ICompetingConsumer
    {
        private readonly IEventStoreConnection _connection;
        private readonly Serilog.ILogger _logger;
        private readonly AppSettings _settings;

        public ReplicatingConsumer(
            IEventStoreConnection connection, Serilog.ILogger logger, AppSettings settings, string streamName, string groupName)
        {
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            StreamName = streamName ?? throw new ArgumentNullException(nameof(streamName));
            GroupName = groupName ?? throw new ArgumentNullException(nameof(groupName));
        }

        public string StreamName { get; }
        public string GroupName { get; }
        public bool CanConsume(string eventType) => true;

        public void Consume(string eventType, ResolvedEvent resolvedEvent)
        {
            if (string.IsNullOrWhiteSpace(eventType)) throw new ArgumentException("Event type required", nameof(eventType));
            
            var eventData = new EventData(
                resolvedEvent.Event.EventId,
                resolvedEvent.Event.EventType,
                true,
                resolvedEvent.Event.Data,
                resolvedEvent.Event.Metadata
            );

            using (_logger.ForContext("OriginalEventNumber", resolvedEvent.OriginalEventNumber).TimeOperation("Replicating {EventType} #{EventNumber} from {StreamName}",
                resolvedEvent.Event.EventType,
                resolvedEvent.Event.EventNumber,
                resolvedEvent.Event.EventStreamId))
            {
                _connection.AppendToStreamAsync(resolvedEvent.Event.EventStreamId, resolvedEvent.Event.EventNumber - 1, eventData).GetAwaiter().GetResult();
            }

            if (_settings.ReplicationThrottleInterval.GetValueOrDefault() > 0)
                Thread.Sleep(_settings.ReplicationThrottleInterval.Value);
        }
    }
}