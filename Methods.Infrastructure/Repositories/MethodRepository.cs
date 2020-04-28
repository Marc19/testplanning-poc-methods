using System;
using System.Collections.Generic;
using System.Linq;
using Methods.Core.Common;
using Methods.Core.Entities;
using Methods.Core.IRepositories;
using Methods.Core.Messages.Commands;
using Methods.Core.Validation;

namespace Methods.Infrastructure.Repositories
{
    public class MethodRepository : IMethodRepository
    {
        public static readonly List<Method> methodsMemoryDatabase = new List<Method>();
        private static long lastIdValue = 0;

        public MethodRepository()
        {
        }

        public Result<Method> CreateMethod(CreateMethod createMethod)
        {
            Method method = new Method(createMethod.Creator, createMethod.Name, createMethod.ApplicationRate);

            MethodValidator methodValidator = new MethodValidator();
            var validationResult = methodValidator.Validate(method);

            if (!validationResult.IsValid)
            {
                return Result.Fail<Method>(string.Join(" ", validationResult.Errors));
            }

            method.SetId(++lastIdValue);
            methodsMemoryDatabase.Add(method);

            return Result.Ok<Method>(method);
        }

        public Result<List<Method>> CreateMethods(CreateMethods createMethods)
        {
            List<Method> methods = createMethods.Methods.Select(m =>
                new Method(m.Creator, m.Name, m.ApplicationRate)).ToList();

            MethodValidator methodValidator = new MethodValidator();
            List<string> errors = new List<string>();
            foreach (var method in methods)
            {
                var validationResult = methodValidator.Validate(method);

                if (!validationResult.IsValid)
                {
                    errors.AddRange(validationResult.Errors.Select(e => e.ToString()));
                }
            }

            if(errors.Count() > 0)
            {
                return Result.Fail<List<Method>>(string.Join(" ", errors));
            }

            foreach (var method in methods)
            {
                method.SetId(++lastIdValue);
                methodsMemoryDatabase.Add(method);
            }

            return Result.Ok<List<Method>>(methods);
        }

        public Result DeleteMethod(DeleteMethod deleteMethod)
        {
            long idToDelete = deleteMethod.Id;

            int numberOfRemovedItems = methodsMemoryDatabase.RemoveAll(m => m.Id == idToDelete);

            if (numberOfRemovedItems == 0)
            {
                return Result.Fail("The method you are trying to delete does not exist");
            }

            return Result.Ok();
        }

        public Result DeleteMethods(DeleteMethods deleteMethods)
        {
            //Will fail if all provided methods do not exist
            List<long> idsToDelete = deleteMethods.Ids;

            int numberOfRemovedItems = methodsMemoryDatabase.RemoveAll(m => idsToDelete.Contains(m.Id));

            if (numberOfRemovedItems == 0)
            {
                return Result.Fail("All methods you are trying to delete do not exist");
            }

            return Result.Ok();
        }

        public Result<Method> UpdateMethod(UpdateMethod updateMethod)
        {
            Method methodToUpdate = methodsMemoryDatabase.Find(e => e.Id == updateMethod.Id);

            if (methodToUpdate == null)
            {
                return Result.Fail<Method>("The method you're trying to update does not exist");
            }

            methodToUpdate.Creator = updateMethod.Creator;
            methodToUpdate.Name = updateMethod.Name;

            return Result.Ok<Method>(methodToUpdate);
        }

        public Result AddMethodsToExperiment(AddMethodsToExperiment addMethodsToExperiment)
        {
            //Will fail if one of the provided methods does not exist
            long experimentId = addMethodsToExperiment.ExperimentId;
            List<long> methodsIds = addMethodsToExperiment.MethodsIds;

            List<long> allMethodsIds = methodsMemoryDatabase.Select(m => m.Id).ToList();

            if(!methodsIds.All(m => allMethodsIds.Contains(m)))
            {
                return Result.Fail("Some of the methods you provided do not exist");
            }

            methodsMemoryDatabase.ForEach(m =>
            {
                if (methodsIds.Contains(m.Id))
                {
                    m.AddExperiment(experimentId);
                }
            });

            return Result.Ok();
        }

        public Result RemoveMethodsFromExperiment(RemoveMethodsFromExperiment removeMethodsFromExperiment)
        {
            //Will fail if one of the provided methods does not exist
            long experimentId = removeMethodsFromExperiment.ExperimentId;
            List<long> methodsIds = removeMethodsFromExperiment.MethodsIds;

            List<long> allMethodsIds = methodsMemoryDatabase.Select(m => m.Id).ToList();

            if (!methodsIds.All(m => allMethodsIds.Contains(m)))
            {
                return Result.Fail("Some of the methods you provided do not exist");
            }

            methodsMemoryDatabase.ForEach(m =>
            {
                if (methodsIds.Contains(m.Id))
                {
                    m.RemoveExperimet(experimentId);
                }
            });

            return Result.Ok();
        }
    }
}
