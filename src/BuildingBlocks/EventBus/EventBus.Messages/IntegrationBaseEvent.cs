﻿namespace EventBus.Messages
{
    public record IntegrationBaseEvent : IIntegrationEvent
    {
        public DateTime CreationDate { get; set; } = DateTime.UtcNow;
        public Guid Id { get; set; }
    }
}
