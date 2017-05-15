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

            /* If we go through an entire queue without making any progress, the algorithm 
             * is currently stuck.  This object reference is used to detect that condition.  
             *    -> If the algorithm finds a node that cannot be resolved because its descendants
             *       don't all exist in the results dictionary, and the firstNoprogressTask reference
             *       is null, then we take the current unresolvable task and store it in the fNT ref
             *    -> If the algorithm finds a node that ~can~ be resolved, it sets the fNT reference
             *       back to null, and the next task that can't be resolved will take its place
             *    -> If the algorithm gets to a node that can't be resolved and it's ~already~ stored
             *       in the fNT reference, then we've managed to go all the way through the queue and
             *       loop back around without resolving a single task.  That means we're stuck and we
             *       shoud throw an error. */
            PertTask firstNoprogressTask = null;

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
                    
                    if (firstNoprogressTask == task)
                        throw new ArgumentException("It seems some descendant activites were not included in the original list and the path length algorithm has stuck");

                    if (firstNoprogressTask == null)
                        firstNoprogressTask = task;
                }
                else
                {
                    firstNoprogressTask = null;
                    results.Add(task.Id, valueReader.GetValue(task.TimeEstimate) + maxDescendantWeight);
                    foreach (var taskAncestor in task.Ancestors)
                    {
                        if (!workingTasks.Contains(taskAncestor))
                            workingTasks.Enqueue(taskAncestor);
                    }
                }
            }

            return results;
        }

        public static Dictionary<Guid, double> NetworkModePathLengths(IEnumerable<PertTask> tasks)
        {
            return NetworkPathLengths(tasks, new EstimateModeReader());
        }
    }

}