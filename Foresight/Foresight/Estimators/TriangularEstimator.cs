using System;
using MathNet.Numerics.Distributions;

namespace Foresight.Estimators
{
    /// <summary>
    /// The Triangular estimator uses inverse transform sampling to pull the random 
    /// https://en.wikipedia.org/wiki/Triangular_distribution
    /// https://en.wikipedia.org/wiki/Inverse_transform_sampling
    /// number as if the probability density function was a triangluar shape with a starting node at
    /// Estimate.Min, an ending node at Estimate.Max, and the highest point at Estimate.Mode.
    /// </summary>
    public class TriangularEstimator : IEstimator
    {
        private Random _generator = new Random();

        public double RandomValue(Estimate estimate)
        {
            // All values are the same, return the value
            if (estimate.Max - estimate.Min <= 0)
                return estimate.Max;

            var triangle = new Triangular(estimate.Min, estimate.Max, estimate.Mode);
            return triangle.Sample();
        }
        
    }
}