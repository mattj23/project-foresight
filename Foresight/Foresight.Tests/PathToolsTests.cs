using System;
using System.Linq;
using Foresight.Tools;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;

namespace Foresight.Tests
{
    [TestFixture]
    public class PathToolsTests
    {

        [Test]
        public void SimpleNetworkLengths()
        {
            var t1 = new PertTask {TimeEstimate = new Estimate(1)};
            var t2 = new PertTask {TimeEstimate = new Estimate(1)};
            var t3 = new PertTask {TimeEstimate = new Estimate(1)};
            t1.LinkToDescendant(t2);
            t2.LinkToDescendant(t3);
            var tasks = new PertTask[] {t1, t2, t3};

            var result = PathTools.NetworkPathLengths(tasks, new EstimateModeReader());

            Assert.AreEqual(1.0, result[t3.Id]);
            Assert.AreEqual(2.0, result[t2.Id]);
            Assert.AreEqual(3.0, result[t1.Id]);
        }

        [Test]
        public void ForkedNetworkLengths()
        {
            var t1 = new PertTask { TimeEstimate = new Estimate(1) };
            var t2 = new PertTask { TimeEstimate = new Estimate(1) };
            var t3 = new PertTask { TimeEstimate = new Estimate(1) };
            var t4 = new PertTask { TimeEstimate = new Estimate(2) };
            t1.LinkToDescendant(t2);
            t2.LinkToDescendant(t3);
            t2.LinkToDescendant(t4);
            var tasks = new PertTask[] { t1, t2, t3, t4};

            var result = PathTools.NetworkPathLengths(tasks, new EstimateModeReader());

            Assert.AreEqual(1.0, result[t3.Id]);
            Assert.AreEqual(3.0, result[t2.Id]);
            Assert.AreEqual(4.0, result[t1.Id]);
            Assert.AreEqual(2.0, result[t4.Id]);
        }

        [Test]
        public void StuckNetworkLengthsThrowsError()
        {
            var t1 = new PertTask { TimeEstimate = new Estimate(1) };
            var t2 = new PertTask { TimeEstimate = new Estimate(1) };
            var t3 = new PertTask { TimeEstimate = new Estimate(1) };
            var t4 = new PertTask { TimeEstimate = new Estimate(2) };
            t1.LinkToDescendant(t2);
            t2.LinkToDescendant(t3);
            t2.LinkToDescendant(t4);
            var tasks = new PertTask[] { t1, t2, t3 };

            Assert.Throws<ArgumentException>(() =>
            {
                PathTools.NetworkPathLengths(tasks, new EstimateModeReader());
            });
        }

        [TestCase("1", 18)]
        [TestCase("2", 17)]
        [TestCase("3", 15)]
        [TestCase("4", 14)]
        [TestCase("5", 10)]
        [TestCase("6", 7)]
        [TestCase("7", 1)]
        [TestCase("8", 4)]
        [TestCase("9", 5)]
        [TestCase("10",2)]
        public void ComplexNetwork(string node, double expected)
        {
            var task1 = new PertTask { Name = "1", TimeEstimate = new Estimate(3) };
            var task2 = new PertTask { Name = "2", TimeEstimate = new Estimate(2) };
            var task3 = new PertTask { Name = "3", TimeEstimate = new Estimate(5) };
            var task4 = new PertTask { Name = "4", TimeEstimate = new Estimate(4) };
            var task5 = new PertTask { Name = "5", TimeEstimate = new Estimate(3) };
            var task6 = new PertTask { Name = "6", TimeEstimate = new Estimate(2) };
            var task7 = new PertTask { Name = "7", TimeEstimate = new Estimate(1) };
            var task8 = new PertTask { Name = "8", TimeEstimate = new Estimate(4) };
            var task9 = new PertTask { Name = "9", TimeEstimate = new Estimate(3) };
            var task10 = new PertTask { Name = "10", TimeEstimate = new Estimate(2) };

            task1.LinkToDescendant(task3);
            task2.LinkToDescendant(task3);
            task3.LinkToDescendant(task5);
            task4.LinkToDescendant(task5);
            task5.LinkToDescendant(task6);
            task5.LinkToDescendant(task7);
            task6.LinkToDescendant(task8);
            task6.LinkToDescendant(task9);
            task9.LinkToDescendant(task10);
            var tasks = new PertTask[] { task1, task2, task3, task4, task5, task6, task7, task8, task9, task10 };

            var result = PathTools.NetworkPathLengths(tasks, new EstimateModeReader());

            var checkTaskId = tasks.First(x => x.Name == node).Id;

            Assert.AreEqual(expected, result[checkTaskId]);
        }
    }
}