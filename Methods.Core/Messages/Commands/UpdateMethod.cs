using System;
namespace Methods.Core.Messages.Commands
{
    public class UpdateMethod : Command
    {
        public readonly long Id;

        public readonly string Creator;

        public readonly string Name;

        public readonly decimal ApplicationRate;

        public UpdateMethod(long id, string creator, string name, decimal applicationRate, long loggedInUserId, Guid sagaId = default)
            : base(loggedInUserId, sagaId)
        {
            Id = id;
            Creator = creator;
            Name = name;
            ApplicationRate = applicationRate;
        }
    }
}
