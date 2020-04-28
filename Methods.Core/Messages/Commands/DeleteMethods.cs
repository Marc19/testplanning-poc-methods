using System;
using System.Collections.Generic;

namespace Methods.Core.Messages.Commands
{
    public class DeleteMethods : Command
    {
        public readonly List<long> Ids;

        public DeleteMethods(List<long> ids, long loggedInUserId, Guid sagaId = default)
            : base(loggedInUserId, sagaId)
        {
            Ids = ids;
        }
    }
}
