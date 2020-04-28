using System;
using Methods.Core.Messages.Commands;

namespace Methods.Application.IServices
{
    public interface IMethodCommandHandlers
    {
        void Handle(Command command);
    }
}
