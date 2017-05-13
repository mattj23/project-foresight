using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Foresight
{
    /// <summary>
    /// Represents a PERT Network
    /// </summary>
    public class Project
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public ReadOnlyCollection<PertTask> Tasks => new ReadOnlyCollection<PertTask>(this._tasks.ToList());

        private HashSet<PertTask> _tasks;



        public Project()
        {
            this._tasks = new HashSet<PertTask>();
        }

        public void AddTask(PertTask task)
        {
            this._tasks.Add(task);
        }

        public void RemoveTask(PertTask task)
        {
            this._tasks.Remove(task);
            task.UnlinkAll();
        }

        public PertTask GetTaskById(Guid id)
        {
            return this._tasks.FirstOrDefault(x => x.Id == id);
        }

    }
}