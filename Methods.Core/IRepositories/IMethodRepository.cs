using System;
using System.Collections.Generic;
using Methods.Core.Common;
using Methods.Core.Entities;
using Methods.Core.Messages.Commands;

namespace Methods.Core.IRepositories
{
    public interface IMethodRepository
    {
        Result<Method> CreateMethod(CreateMethod createMethod);

        Result<List<Method>> CreateMethods(CreateMethods createMethods);

        Result DeleteMethod(DeleteMethod deleteMethod);

        Result<Method> UpdateMethod(UpdateMethod updateMethod);

        Result DeleteMethods(DeleteMethods deleteMethods);

        Result AddMethodsToExperiment(AddMethodsToExperiment addMethodsToExperiment);

        Result RemoveMethodsFromExperiment(RemoveMethodsFromExperiment removeMethodsFromExperiment);
    }
}
