using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Foresight
{
    public class PertTask
    {
        /// <summary>
        /// Return a HashSet with all ancestors of a task located by recursively walking the graph
        /// </summary>
        /// <param name="task">The task to find all ancestors of</param>
        /// <returns></returns>
        public static HashSet<PertTask> AllAncestorsOf(PertTask task)
        {
            var tasks = new HashSet<PertTask>();
            var checkTasks = new Queue<PertTask>();
            checkTasks.Enqueue(task);
            while (checkTasks.Count != 0)
            {
                var working = checkTasks.Dequeue();
                foreach (var item in working.Ancestors)
                {
                    checkTasks.Enqueue(item);
                }
                tasks.Add(working);
            }
            tasks.Remove(task);
            return tasks;
        }

        /// <summary>
        /// Return as HashSet with all descendants of a task located by recursively walking the graph
        /// </summary>
        /// <param name="task">The task to find all descendants of</param>
        /// <returns></returns>
        public static HashSet<PertTask> AllDescendantsOf(PertTask task)
        {
            var tasks = new HashSet<PertTask>();
            var checkTasks = new Queue<PertTask>();
            checkTasks.Enqueue(task);
            while (checkTasks.Count != 0)
            {
                var working = checkTasks.Dequeue();
                foreach (var item in working.Descendants)
                {
                    checkTasks.Enqueue(item);
                }
                tasks.Add(working);
            }
            tasks.Remove(task);
            return tasks;
        }

        public Guid Id { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }

        public string Category { get; set; }

        public IReadOnlyCollection<PertTask> Ancestors => new ReadOnlyCollection<PertTask>(this._ancestors.ToList());
        public IReadOnlyCollection<PertTask> Descendants => new ReadOnlyCollection<PertTask>(this._descendants.ToList());

        public IReadOnlyCollection<PertTask> AllAncestors => new ReadOnlyCollection<PertTask>(AllAncestorsOf(this).ToList());

        public IReadOnlyCollection<PertTask> AllDescendants => new ReadOnlyCollection<PertTask>(AllDescendantsOf(this).ToList());

        public Estimate TimeEstimate { get; set; }
        public HashSet<Employee> Employees { get; set; }

        private readonly HashSet<PertTask> _ancestors;
        private readonly HashSet<PertTask> _descendants;

        public PertTask()
        {
            this.Id = Guid.NewGuid();
            this.Employees = new HashSet<Employee>();
            this._ancestors = new HashSet<PertTask>();
            this._descendants = new HashSet<PertTask>();
        }

        public void LinkToAncestor(PertTask ancestor)
        {
            // Check to make sure we're not creating a cyclic graph
            if (ancestor == this)
                throw new ArgumentException("Cannot link a task to itself");
            if (ancestor.AllAncestors.Contains(this))
                throw new ArgumentException($"Making task '{ancestor.Name}' an ancestor of task '{this.Name}' would create a cyclic project network.");
            if (this._ancestors.Contains(ancestor))
                throw new ArgumentException($"Task '{ancestor.Name}' is already an ancestor of task '{this.Name}'.");
            this._ancestors.Add(ancestor);
            ancestor._descendants.Add(this);
        }

        public void LinkToDescendant(PertTask descendant)
        {
            if (descendant == this)
                throw new ArgumentException("Cannot link a task to itself");
            if (descendant.AllDescendants.Contains(this))
                throw new ArgumentException($"Making task '{descendant.Name}' a descendant of task '{this.Name}' would create a cyclic project network.");
            if (this._descendants.Contains(descendant))
                throw new ArgumentException($"Task '{descendant.Name}' is already an ancestor of task '{this.Name}'.");
            this._descendants.Add(descendant);
            descendant._ancestors.Add(this);
        }

        public void UnlinkFromAncestor(PertTask ancestor)
        {
            if (this._ancestors.Contains(ancestor))
            {
                ancestor._descendants.Remove(this);
                this._ancestors.Remove(ancestor);
            }
            else
            {
                throw new ArgumentException($"Task '{this.Name}' does not have task '{ancestor.Name}' as an ancestor.");
            }
        }

        public void UnlinkFromDescendant(PertTask descendant)
        {
            if (this._descendants.Contains(descendant))
            {
                this._descendants.Remove(descendant);
                descendant._ancestors.Remove(this);
            }
            else
            {
                throw new ArgumentException($"Task '{this.Name}' does not have task '{descendant.Name}' as a descendant.");
            }
        }

        public void UnlinkAll()
        {
            foreach (var ancestor in this._ancestors.ToList())
            {
                this.UnlinkFromAncestor(ancestor);
            }

            foreach (var descendant in this._descendants.ToList())
            {
                this.UnlinkFromDescendant(descendant);
            }
        }

    }
}