using System;
using System.Collections.Generic;
using System.Linq;

namespace Foresight.Tools
{
    /// <summary>
    /// Tools to analyze the PERT network tree structure and path lengths
    /// </summary>
    public static class PathTools
    {
        public static Dictionary<Guid, double> NetworkPathLengths(IEnumerable<PertTask> tasks,
            IEstimateValueReader valueReader)
        {
            var results = new Dictionary<Guid, double>();
            var workingTasks = new Queue<PertTask>();

            // Prepare the starting (ending) tasks as the ones with no descendants
            foreach (var pertTask in tasks)
            {
                if (!pertTask.Descendants.Any())
                    workingTasks.Enqueue(pertTask);
            }

            while (workingTasks.Any())
            {
                // For each cycle we check each task to see if it has no unresolved descendants.  Each 
                // task located can be assigned a value based on the largest weight of its descendants
                // added to its own weight, at which time it's stored in the results dictionary
                var task = workingTasks.Dequeue();


                double maxDescendantWeight = 0;
                bool hasUnresolvedDescendant = false;
                foreach (var taskDescendant in task.Descendants)
                {
                    // If we find any descendant that does not have an entry already in the results list,
                    // we abort working on this task put it back at the end of the queue
                    if (!results.ContainsKey(taskDescendant.Id))
                    {
                        hasUnresolvedDescendant = true;
                        break;
                    }
                    
                    // Otherwise, we check and see if this descendant has a longer weight than the 
                    // currently recorded maximum
                    if (results[taskDescendant.Id] > maxDescendantWeight)
                        maxDescendantWeight = results[taskDescendant.Id];

                }

                if (hasUnresolvedDescendant)
                {
                    workingTasks.Enqueue(task);
                }
                else
                {
                    results.Add(task.Id, valueReader.GetValue(task.TimeEstimate) + maxDescendantWeight);
                    foreach (var taskAncestor in task.Ancestors)
                    {
                        workingTasks.Enqueue(taskAncestor);
                    }
                }
            }

            return results;
        }
    }

}