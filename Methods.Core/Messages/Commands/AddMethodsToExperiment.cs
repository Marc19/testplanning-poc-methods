using System;
using System.Collections.Generic;

namespace Methods.Core.Messages.Commands
{
    public class AddMethodsToExperiment : Command
    {
        public readonly long ExperimentId;

        public readonly List<long> MethodsIds;

        public AddMethodsToExperiment(long experimentId, List<long> methodsIds,
            long loggedInUserId, Guid sagaId = default)
            : base(loggedInUserId, sagaId)
        {
            ExperimentId = experimentId;
            MethodsIds = methodsIds;
        }
    }
}
