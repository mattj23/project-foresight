using System;
using Foresight.Tools;
using NUnit.Framework;
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
    }
}