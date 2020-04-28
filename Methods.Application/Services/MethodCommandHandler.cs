using System;
using System.Collections.Generic;
using System.Linq;
using Methods.Application.IServices;
using Methods.Core.Common;
using Methods.Core.Entities;
using Methods.Core.IKafka;
using Methods.Core.IRepositories;
using Methods.Core.Messages.Commands;
using Methods.Core.Messages.Events;
using Microsoft.Extensions.Configuration;

namespace Methods.Application.Services
{
    public class MethodCommandHandlers : IMethodCommandHandlers
    {
        private readonly IMethodRepository _repository;
        private readonly IKafkaProducer _kafkaProducer;
        private readonly IConfiguration Configuration;
        private readonly string METHODS_TOPIC;

        public MethodCommandHandlers(IMethodRepository repository, IKafkaProducer kafkaProducer, IConfiguration configuration)
        {
            _repository = repository;
            _kafkaProducer = kafkaProducer;
            Configuration = configuration;
            METHODS_TOPIC = Configuration["MethodsTopic"];
        }

        public void Handle(Command command)
        {
            switch (command)
            {
                case CreateMethod c: HandleCreateMethod(c); break;
                case CreateMethods cs: HandleCreateMethods(cs); break;
                case DeleteMethod d: HandleDeleteMethod(d); break;
                case DeleteMethods ds: HandleDeleteMethods(ds); break;
                case UpdateMethod u: HandleUpdateMethod(u); break;
                case AddMethodsToExperiment a: HandleAddMethodsToExperiment(a); break;
                case RemoveMethodsFromExperiment r: HandleRemoveMethodsFromExperiment(r); break;
            }
        }

        private void HandleCreateMethod(CreateMethod createMethod)
        {
            Result<Method> methodCreationResult = _repository.CreateMethod(createMethod);

            if (methodCreationResult.IsFailure)
            {
                MethodCreationFailed failedMethod =
                    new MethodCreationFailed(
                        methodCreationResult.Error,
                        createMethod.LoggedInUserId,
                        createMethod.SagaId
                    );
                _kafkaProducer.Produce(failedMethod, METHODS_TOPIC);
                return;
            }

            MethodCreated createdMethod = new MethodCreated(
                methodCreationResult.Value.Id,
                methodCreationResult.Value.Creator,
                methodCreationResult.Value.Name,
                methodCreationResult.Value.ApplicationRate,
                methodCreationResult.Value.CreationDate,
                createMethod.LoggedInUserId,
                createMethod.SagaId
            );

            _kafkaProducer.Produce(createdMethod, METHODS_TOPIC);

        }

        private void HandleCreateMethods(CreateMethods createMethods)
        {
            Result<List<Method>> methodsCreationResult = _repository.CreateMethods(createMethods);

            if (methodsCreationResult.IsFailure)
            {
                MethodsCreationFailed failedMethod =
                    new MethodsCreationFailed(
                        methodsCreationResult.Error,
                        createMethods.LoggedInUserId,
                        createMethods.SagaId
                    );
                _kafkaProducer.Produce(failedMethod, METHODS_TOPIC);
                return;
            }

            List<MethodCreated> createdMethods = new List<MethodCreated>();
            foreach (var methodCreationResult in methodsCreationResult.Value)
            {
                MethodCreated createdMethod = new MethodCreated(
                    methodCreationResult.Id,
                    methodCreationResult.Creator,
                    methodCreationResult.Name,
                    methodCreationResult.ApplicationRate,
                    methodCreationResult.CreationDate,
                    createMethods.LoggedInUserId,
                    createMethods.SagaId
                );
                createdMethods.Add(createdMethod);
            }

            MethodsCreated createdMethodsEvent = new MethodsCreated(createdMethods, createMethods.LoggedInUserId, createMethods.SagaId);
            _kafkaProducer.Produce(createdMethodsEvent, METHODS_TOPIC);
        }

        private void HandleDeleteMethod(DeleteMethod deleteMethod)
        {
            Result deletionResult = _repository.DeleteMethod(deleteMethod);

            if (deletionResult.IsFailure)
            {
                MethodDeletionFailed failedMethodDeletion =
                    new MethodDeletionFailed(
                        deletionResult.Error,
                        deleteMethod.LoggedInUserId,
                        deleteMethod.SagaId
                    );
                _kafkaProducer.Produce(failedMethodDeletion, METHODS_TOPIC);
                return;
            }

            MethodDeleted deletedMethod =
                new MethodDeleted(deleteMethod.Id, deleteMethod.LoggedInUserId, deleteMethod.SagaId);
            _kafkaProducer.Produce(deletedMethod, METHODS_TOPIC);
        }

        private void HandleDeleteMethods(DeleteMethods deleteMethods)
        {
            Result deletionResult = _repository.DeleteMethods(deleteMethods);

            if (deletionResult.IsFailure)
            {
                MethodsDeletionFailed failedCreateMethodsDeletion =
                    new MethodsDeletionFailed(
                        deletionResult.Error,
                        deleteMethods.LoggedInUserId,
                        deleteMethods.SagaId
                    );
                _kafkaProducer.Produce(failedCreateMethodsDeletion, METHODS_TOPIC);
                return;
            }

            List<long> deletedIds = deleteMethods.Ids.Select(id => id).ToList();
            MethodsDeleted deletedMethod =
                new MethodsDeleted(deletedIds, deleteMethods.LoggedInUserId, deleteMethods.SagaId);
            _kafkaProducer.Produce(deletedMethod, METHODS_TOPIC);
        }

        private void HandleUpdateMethod(UpdateMethod updateMethod)
        {
            Result<Method> updatedMethodResult = _repository.UpdateMethod(updateMethod);

            if (updatedMethodResult.IsFailure)
            {
                MethodUpdateFailed failedMethodUpdate =
                    new MethodUpdateFailed(
                        updatedMethodResult.Error,
                        updateMethod.LoggedInUserId,
                        updateMethod.SagaId
                    );
                _kafkaProducer.Produce(failedMethodUpdate, METHODS_TOPIC);
                return;
            }

            MethodUpdated updatedMethod = new MethodUpdated(
                updatedMethodResult.Value.Id,
                updatedMethodResult.Value.Creator,
                updatedMethodResult.Value.Name,
                updatedMethodResult.Value.ApplicationRate,
                updatedMethodResult.Value.CreationDate,
                updateMethod.LoggedInUserId,
                updateMethod.SagaId
            );

            _kafkaProducer.Produce(updatedMethod, METHODS_TOPIC);
        }

        private void HandleAddMethodsToExperiment(AddMethodsToExperiment addMethodsToExperiment)
        {
            Result additionResult = _repository.AddMethodsToExperiment(addMethodsToExperiment);

            if (additionResult.IsFailure)
            {
                ExperimentAdditionToMethodsFailed failedMethodsAdditionToExperiment =
                    new ExperimentAdditionToMethodsFailed(
                        additionResult.Error,
                        addMethodsToExperiment.LoggedInUserId,
                        addMethodsToExperiment.SagaId
                    );
                _kafkaProducer.Produce(failedMethodsAdditionToExperiment, METHODS_TOPIC);
                return;
            }

            ExperimentAddedToMethods methodsAddedToExperiment = new ExperimentAddedToMethods(
                addMethodsToExperiment.ExperimentId,
                addMethodsToExperiment.MethodsIds.Select(mId => mId).ToList(),
                addMethodsToExperiment.LoggedInUserId,
                addMethodsToExperiment.SagaId
            );

            _kafkaProducer.Produce(methodsAddedToExperiment, METHODS_TOPIC);
        }

        private void HandleRemoveMethodsFromExperiment(RemoveMethodsFromExperiment removeMethodsFromExperiment)
        {
            Result removalResult = _repository.RemoveMethodsFromExperiment(removeMethodsFromExperiment);

            if (removalResult.IsFailure)
            {
                ExperimentRemovalFromMethodsFailed failedMethodsRemovalFromExperiment =
                    new ExperimentRemovalFromMethodsFailed(
                        removalResult.Error,
                        removeMethodsFromExperiment.LoggedInUserId,
                        removeMethodsFromExperiment.SagaId
                    );
                _kafkaProducer.Produce(failedMethodsRemovalFromExperiment, METHODS_TOPIC);
                return;
            }

            ExperimentRemovedFromMethods methodsRemovedFromExperiment = new ExperimentRemovedFromMethods(
                removeMethodsFromExperiment.ExperimentId,
                removeMethodsFromExperiment.MethodsIds.Select(mId => mId).ToList(),
                removeMethodsFromExperiment.LoggedInUserId,
                removeMethodsFromExperiment.SagaId
            );

            _kafkaProducer.Produce(methodsRemovedFromExperiment, METHODS_TOPIC);
        }
    }
}
