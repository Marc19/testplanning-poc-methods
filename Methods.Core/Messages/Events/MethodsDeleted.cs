using System;
using System.Collections.Generic;

namespace Methods.Core.Messages.Events
{
    public class MethodsDeleted : Event
    {
        public readonly List<long> Ids;

        public MethodsDeleted(List<long> ids, long loggedInUserId, Guid sagaId = default)
            : base(loggedInUserId, sagaId)
        {
            Ids = ids;
        }
    }

    public class MethodsDeletionFailed : FailedEvent
    {
        public MethodsDeletionFailed(string failureReason, long loggedInUserId, Guid sagaId = default)
            : base(failureReason, loggedInUserId, sagaId) { }
    }
}
