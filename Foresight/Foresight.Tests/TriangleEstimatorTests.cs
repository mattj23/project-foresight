using System;
using Foresight.Estimators;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace Foresight.Tests
{
    [TestFixture]
    public class TriangleEstimatorTests
    {
        /* Things that must get tested:
         * When Min, Mode, and Max are all equal, mode must be returned and nothing crashes
         */

        [TestCase(Double.PositiveInfinity, 1, 1, 1)]
        [TestCase(2, 0, 1, 1)]
        [TestCase(2, 0, 0, 1)]
        [TestCase(1, 0, 1, 2)]
        public void TriangleModeProbability(double expected, double min, double mode, double max)
        {
            var estimator = new TriangularEstimator();
            var estimate = new Estimate(min, mode, max);
            Assert.AreEqual(expected, estimator.ModeProbability(estimate));
        }

        [TestCase(0, -1, 0, 1, 2)]
        [TestCase(1, 4, 0, 1, 2)]
        [TestCase(0.5, 1, 0, 1, 2)]
        public void TriangleCumulativeDensityFunction(double expected, double x, double min, double mode, double max)
        {
            var estimator = new TriangularEstimator();
            var estimate = new Estimate(min, mode, max);
            Assert.AreEqual(expected, estimator.Cdf(estimate, x));
        }


    }
}