using System.Security.Cryptography.X509Certificates;

namespace Foresight.Estimators
{
    public interface IEstimator
    {
        double RandomValue(Estimate estimate);

        double InverseCdf(Estimate estimate, double value);

        double Cdf(Estimate estimate, double x);
    }
}