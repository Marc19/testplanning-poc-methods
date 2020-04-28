using System;
namespace Methods.Core.Messages.Events
{
    public abstract class FailedEvent : Event
    {
        public readonly string FailureReason;

        public FailedEvent(string failureReason, long loggedInUserId, Guid sagaId) : base(loggedInUserId, sagaId)
        {
            FailureReason = failureReason;
        }
    }
}
