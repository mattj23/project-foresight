using System.Security.Cryptography.X509Certificates;

namespace Foresight.Estimators
{
    public interface IEstimator
    {
        double RandomValue(Estimate estimate);

    }
}