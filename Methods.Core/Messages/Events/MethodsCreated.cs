using System;
using System.Collections.Generic;

namespace Methods.Core.Messages.Events
{
    public class MethodsCreated : Event
    {
        public readonly List<MethodCreated> CreatedMethods;

        public MethodsCreated(List<MethodCreated> createdMethods, long loggedInUserId, Guid sagaId = default)
            : base(loggedInUserId, sagaId)
        {
            CreatedMethods = new List<MethodCreated>();
            foreach (var createdMethod in createdMethods)
            {
                MethodCreated createMethod =
                    new MethodCreated(
                        createdMethod.Id,
                        createdMethod.Creator,
                        createdMethod.Name,
                        createdMethod.ApplicationRate,
                        createdMethod.CreationDate,
                        createdMethod.LoggedInUserId
                    );
                CreatedMethods.Add(createMethod);
            }
        }
    }

    public class MethodsCreationFailed : FailedEvent
    {
        public MethodsCreationFailed(string failureReason, long loggedInUserId, Guid sagaId = default)
            : base(failureReason, loggedInUserId, sagaId) { }
    }
}
