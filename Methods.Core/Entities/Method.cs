using System;
using System.Collections.Generic;
using Methods.Core.Common;

namespace Methods.Core.Entities
{
    public class Method : AggregateRoot
    {
        public string Creator { get; set; }

        public string Name { get; set; }

        public decimal ApplicationRate { get; set; }

        public DateTime CreationDate { get; set; }

        public List<long> ExperimentIds { get; set; }

        public Method(string creator, string name, decimal applicationRate)
        {
            Creator = creator;
            Name = name;
            ApplicationRate = applicationRate;
            CreationDate = DateTime.Now;
        }

        public void AddExperiment(long id)
        {
            if (ExperimentIds == null || id < 1 || ExperimentIds.Contains(id))
                return;

            ExperimentIds.Add(id);
        }

        public void RemoveExperimet(long id)
        {
            if (ExperimentIds == null || id < 1)
                return;

            ExperimentIds.Remove(id);
        }
    }
}
