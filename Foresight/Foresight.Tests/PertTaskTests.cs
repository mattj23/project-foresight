using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;


namespace Foresight.Tests
{

    [TestFixture]
    public class PertTaskTests
    {
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
        public void CreateNetworks()
        {
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
        }

        [Test]
        public void LinkAncestor()
        {
            var t1 = new PertTask { Name = "Task 1" };
            var t2 = new PertTask { Name = "Task 2" };

            t2.LinkToAncestor(t1);

            Assert.IsTrue(t1.Descendants.Contains(t2));
            Assert.IsTrue(t2.Ancestors.Contains(t1));
        }

        [Test]
        public void LinkDescendant()
        {
            var t1 = new PertTask { Name = "Task 1" };
            var t2 = new PertTask { Name = "Task 2" };

            t1.LinkToDescendant(t2);

            Assert.IsTrue(t1.Descendants.Contains(t2));
            Assert.IsTrue(t2.Ancestors.Contains(t1));
        }

        [Test]
        public void UnlinkAncestor()
        {
            task5.UnlinkFromAncestor(task3);
            CollectionAssert.DoesNotContain(task3.Descendants, task5);
            CollectionAssert.DoesNotContain(task5.Ancestors, task3);
            Assert.Throws<ArgumentException>(() => { task8.UnlinkFromAncestor(task1); });
        }

        [Test]
        public void UnlinkDescendant()
        {
            task5.UnlinkFromDescendant(task6);
            CollectionAssert.DoesNotContain(task6.Ancestors, task5);
            CollectionAssert.DoesNotContain(task5.Descendants, task6);
            Assert.Throws<ArgumentException>(()=>{task8.UnlinkFromDescendant(task1);});
        }

        [Test]
        public void UnlinkAll()
        {
            task5.UnlinkAll();
            CollectionAssert.DoesNotContain(task3.Descendants, task5);
            CollectionAssert.DoesNotContain(task4.Descendants, task5);
            CollectionAssert.DoesNotContain(task6.Ancestors, task5);
            CollectionAssert.DoesNotContain(task7.Ancestors, task5);
        }

        [Test]
        public void AllAncestors()
        {
            var ancestors = task5.AllAncestors.ToList();
            var expected = new List<PertTask> { task1, task2, task3, task4 };

            CollectionAssert.AreEquivalent(expected, ancestors);
        }


        [Test]
        public void AllDescendants()
        {
            var descendants = task5.AllDescendants.ToList();
            var expected = new List<PertTask> {task6, task7, task8, task9, task10};
            
            CollectionAssert.AreEquivalent(expected, descendants);
        }

        [Test]
        public void NoAncestorOfSelf()
        {
            Assert.Throws<ArgumentException>(() => { task5.LinkToAncestor(task5); });
            Assert.Throws<ArgumentException>(() => { task5.LinkToAncestor(task10); });
        }

        [Test]
        public void NoDescendentOfSelf()
        {
            Assert.Throws<ArgumentException>(() => { task5.LinkToDescendant(task5); });
            Assert.Throws<ArgumentException>(() => { task5.LinkToDescendant(task1); });
        }

        [Test]
        public void NoDuplicateAncestorLinks()
        {
            Assert.Throws<ArgumentException>(() => { task5.LinkToAncestor(task4); });
        }

        [Test]
        public void NoDuplicateDescendantLinks()
        {
            Assert.Throws<ArgumentException>(() => { task5.LinkToDescendant(task6); });
        }
    }
}