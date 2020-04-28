using System;
namespace Methods.Core.Messages.Events
{
    public class MethodDeleted : Event
    {
        public readonly long Id;

        public MethodDeleted(long id, long loggedInUserId, Guid sagaId = default)
            : base(loggedInUserId, sagaId)
        {
            Id = id;
        }
    }

    public class MethodDeletionFailed : FailedEvent
    {
        public MethodDeletionFailed(string failureReason, long loggedInUserId, Guid sagaId = default)
            : base(failureReason, loggedInUserId, sagaId) { }
    }
}
