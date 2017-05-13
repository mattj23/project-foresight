using System.Diagnostics;
using System.Linq;
using Foresight.Serialization;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace Foresight.Tests
{
    [TestFixture]
    public class SerializationTests
    {
        private Project project;
        private PertTask task1;
        private PertTask task2;
        private PertTask task3;
        private PertTask task4;
        private PertTask task5;
        private PertTask task6;
        private PertTask task7;
        private PertTask task8;
        private PertTask task9;
        private PertTask task10;

        [SetUp]
        public void PrepareProject()
        {
            this.project = new Project {Name = "Test Project", Description = "Test project for serialization and deserialization"};
            task1 = new PertTask { Name = "Task 1" };
            task2 = new PertTask { Name = "Task 2" };
            task3 = new PertTask { Name = "Task 3" };
            task4 = new PertTask { Name = "Task 4" };
            task5 = new PertTask { Name = "Task 5" };
            task6 = new PertTask { Name = "Task 6" };
            task7 = new PertTask { Name = "Task 7" };
            task8 = new PertTask { Name = "Task 8" };
            task9 = new PertTask { Name = "Task 9" };
            task10 = new PertTask { Name = "Task 10" };

            task1.LinkToDescendant(task3);
            task2.LinkToDescendant(task3);
            task3.LinkToDescendant(task5);
            task4.LinkToDescendant(task5);
            task5.LinkToDescendant(task6);
            task5.LinkToDescendant(task7);
            task6.LinkToDescendant(task8);
            task6.LinkToDescendant(task9);
            task9.LinkToDescendant(task10);

            foreach (var pertTask in new PertTask[] { task1, task2, task3, task4, task5, task6, task7, task8, task9, task10 })
            {
                project.AddTask(pertTask);
            }


        }


        [Test]
        public void ProjectRoundTrip()
        {
            string data = SerializableProject.Serialize(project);
            var newProject = SerializableProject.Deserialize(data);

            Assert.AreEqual(project.Name, newProject.Name);
            Assert.AreEqual(project.Description, newProject.Description);

            foreach (var task in project.Tasks)
            {
                var newTask = newProject.GetTaskById(task.Id);
                Assert.IsNotNull(newTask);
                Assert.AreEqual(task.Name, newTask.Name);
                Assert.AreEqual(task.Description, newTask.Description);

                // Check ancestors
                foreach (var taskAncestor in task.Ancestors)
                {
                    var newTaskAncestor = newTask.Ancestors.First(x => x.Id == taskAncestor.Id);
                    Assert.IsNotNull(newTaskAncestor);
                }

                // Check descendants
                foreach (var taskDescendant in task.Descendants)
                {
                    var newTaskDescendant = newTask.Descendants.First(x => x.Id == taskDescendant.Id);
                    Assert.IsNotNull(newTaskDescendant);
                }
            }
        }
    }
}