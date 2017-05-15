using System.Collections.Generic;
using Foresight.Simulation;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace Foresight.Tests
{
    [TestFixture]
    public class SimulationTests
    {
        private Project _simpleProject;
        private Organization _org;
        PertTask t1;
        PertTask t2;
        PertTask t3;
        PertTask t4;
        PertTask t5;
        PertTask t6;
        PertTask t7;
        PertTask t8;
        PertTask t9;

        private Employee pete;
        private Employee joe;

        [SetUp]
        public void SetupProjectNetwork()
        {
            _org = new Organization();
            var engineer = new ResourceGroup {Name = "Engineer", Rate = 100};
            var fabricator = new ResourceGroup {Name = "Fabricator", Rate = 50};
            _org.ResourceGroups.Add(engineer);
            _org.ResourceGroups.Add(fabricator);
            pete = new Employee {Name = "Pete", Group = engineer};
            joe = new Employee {Name = "Joe", Group = fabricator};
            _org.Employees.Add(pete);
            _org.Employees.Add(joe);

            t1 = new PertTask { Name = "t1", TimeEstimate = new Estimate(2) };
            t2 = new PertTask { Name = "t2", TimeEstimate = new Estimate(3) };
            t3 = new PertTask { Name = "t3", TimeEstimate = new Estimate(4) };
            t4 = new PertTask { Name = "t4", TimeEstimate = new Estimate(2) };
            t5 = new PertTask { Name = "t5", TimeEstimate = new Estimate(3) };
            t6 = new PertTask { Name = "t6", TimeEstimate = new Estimate(4) };
            t7 = new PertTask { Name = "t7", TimeEstimate = new Estimate(5) };
            t8 = new PertTask { Name = "t8", TimeEstimate = new Estimate(2) };
            t9 = new PertTask { Name = "t9", TimeEstimate = new Estimate(3) };
            
            _simpleProject = new Project();
            _simpleProject.AddTask(t1);
            _simpleProject.AddTask(t2);
            _simpleProject.AddTask(t3);
            _simpleProject.AddTask(t4);
            _simpleProject.AddTask(t5);
            _simpleProject.AddTask(t6);
            _simpleProject.AddTask(t7);
            _simpleProject.AddTask(t8);
            _simpleProject.AddTask(t9);

            t1.LinkToDescendant(t2);
            t2.LinkToDescendant(t3);
            t2.LinkToDescendant(t4);
            t2.LinkToDescendant(t5);
            t3.LinkToDescendant(t6);
            t4.LinkToDescendant(t7);
            t5.LinkToDescendant(t9);
            t6.LinkToDescendant(t8);
            t7.LinkToDescendant(t8);
        }

        [Test]
        public void SimpleSimulationNoResources()
        {
            var simulator = new ProjectSimulator(_simpleProject);
            var result = simulator.Simulate();

            Assert.AreEqual(15, result.TotalCompletionDays);
        }

        [Test]
        public void SimpleSimulationNoResources2()
        {
            t3.LinkToDescendant(t7);
            var simulator = new ProjectSimulator(_simpleProject);
            var result = simulator.Simulate();

            Assert.AreEqual(16, result.TotalCompletionDays);
        }

        [Test]
        public void SimpleSimulationWithResources()
        {
            _simpleProject.Organization = _org;

            foreach (var pertTask in new PertTask[]{t1, t2, t3, t5, t6})
                pertTask.Resources.Add(pete);

            foreach (var pertTask in new PertTask[] { t1, t4, t7, t8 })
                pertTask.Resources.Add(joe);

            var simulator = new ProjectSimulator(_simpleProject);
            var result = simulator.Simulate();

            Assert.AreEqual(19, result.TotalCompletionDays);
        }
    }
}