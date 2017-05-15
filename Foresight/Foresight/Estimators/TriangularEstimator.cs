using System;

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


            return _generator.NextDouble();

        }

        /// <summary>
        /// Calculates the value of the Probability Density Function at the mode value of the
        /// estimate.
        /// </summary>
        /// <param name="estimate"></param>
        /// <returns></returns>
        public double ModeProbability(Estimate estimate)
        {
            /* The probability density function for the triangle distribution is zero at the Min and the
             * Max, and has its highest value at the Mode.  The area under the PDF's curve must be unity,
             * so the probability at the mode is whatever value makes the area of the triangle == 1.  Since
             * a triangle's area is 1/2 base * height, height is solvable. */
            double triangleBase = estimate.Max - estimate.Min;
            if (triangleBase <= 0)
                return double.PositiveInfinity;
            return 2.0 / triangleBase;
        }


        public double InverseCdf(Estimate estimate, double value)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Apply the Cumulative Density Function for a value 
        /// </summary>
        /// <param name="estimate"></param>
        /// <param name="x"></param>
        /// <returns></returns>
        public double Cdf(Estimate estimate, double x)
        {
            if (x < estimate.Min)
                return 0;
            if (x > estimate.Max)
                return 1;

            if (x <= estimate.Mode)
            {
                // Between the min and the mode, the value should return the area under the first triangle
                return Math.Pow(x - estimate.Min, 2) / ((estimate.Max - estimate.Min) * (estimate.Mode - estimate.Min));
            }

            // The value is above the mode but less than or equal to the max
            return 2 * (estimate.Max - x) / ((estimate.Max - estimate.Min) * (estimate.Mode - estimate.Min));

        }
    }
}