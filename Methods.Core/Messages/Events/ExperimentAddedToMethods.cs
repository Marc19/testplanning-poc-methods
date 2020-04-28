using System;
using System.Collections.Generic;

namespace Methods.Core.Messages.Events
{
    public class ExperimentAddedToMethods : Event
    {
        public readonly long ExperimentId;

        public readonly List<long> MethodsIds;

        public ExperimentAddedToMethods(long experimentId, List<long> methodsIds,
            long loggedInUserId, Guid sagaId = default)
            : base(loggedInUserId, sagaId)
        {
            ExperimentId = experimentId;
            MethodsIds = methodsIds;
        }
    }

    public class ExperimentAdditionToMethodsFailed : FailedEvent
    {
        public ExperimentAdditionToMethodsFailed(string failureReason, long loggedInUserId, Guid sagaId = default)
            : base(failureReason, loggedInUserId, sagaId) { }
    }
}
