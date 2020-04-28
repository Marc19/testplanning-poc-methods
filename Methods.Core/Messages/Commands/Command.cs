using System;
namespace Methods.Core.Messages.Commands
{
    public abstract class Command : Message
    {
        public Command(long loggedInUserId, Guid sagaId) : base(loggedInUserId, sagaId) { }
    }
}
