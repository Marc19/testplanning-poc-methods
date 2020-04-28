using System;
using System.Collections.Generic;

namespace Methods.Core.Messages.Commands
{
    public class CreateMethods : Command
    {
        public readonly List<CreateMethod> Methods;

        public CreateMethods(List<CreateMethod> methods, long loggedInUserId, Guid sagaId = default)
            : base(loggedInUserId, sagaId)
        {
            Methods = new List<CreateMethod>();
            foreach (var method in methods)
            {
                CreateMethod createMethod = new CreateMethod(method.Creator, method.Name, method.ApplicationRate, method.LoggedInUserId);
                Methods.Add(createMethod);
            }
        }
    }
}
