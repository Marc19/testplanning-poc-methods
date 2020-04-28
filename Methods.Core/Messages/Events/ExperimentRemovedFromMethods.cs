using System;
using System.Collections.Generic;

namespace Methods.Core.Messages.Events
{
    public class ExperimentRemovedFromMethods : Event
    {
        public readonly long ExperimentId;

        public readonly List<long> MethodsIds;

        public ExperimentRemovedFromMethods(long experimentId, List<long> methodsIds,
            long loggedInUserId, Guid sagaId = default)
            : base(loggedInUserId, sagaId)
        {
            ExperimentId = experimentId;
            MethodsIds = methodsIds;
        }
    }

    public class ExperimentRemovalFromMethodsFailed : FailedEvent
    {
        public ExperimentRemovalFromMethodsFailed(string failureReason, long loggedInUserId, Guid sagaId = default)
            : base(failureReason, loggedInUserId, sagaId) { }
    }
}
