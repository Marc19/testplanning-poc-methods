using System;
namespace Methods.Core.Messages.Commands
{
    public class CreateMethod : Command
    {
        public readonly string Creator;

        public readonly string Name;

        public readonly decimal ApplicationRate;

        public CreateMethod(string creator, string name, decimal applicationRate, long loggedInUserId, Guid sagaId = default)
            : base(loggedInUserId, sagaId)
        {
            Creator = creator;
            Name = name;
            ApplicationRate = applicationRate;
        }
    }
}
