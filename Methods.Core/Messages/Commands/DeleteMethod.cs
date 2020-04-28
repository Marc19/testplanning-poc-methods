using System;
namespace Methods.Core.Messages.Commands
{
    public class DeleteMethod : Command
    {
        public readonly long Id;

        public DeleteMethod(long id, long loggedInUserId, Guid sagaId = default)
            : base(loggedInUserId, sagaId)
        {
            Id = id;
        }
    }
}
