namespace Foresight.Tools
{
    public class EstimateModeReader : IEstimateValueReader
    {
        public double GetValue(Estimate e)
        {
            return e.Mode;
        }
    }

    public class EstimateMinReader : IEstimateValueReader
    {
        public double GetValue(Estimate e)
        {
            return e.Min;
        }
    }

    public class EstimateMaxReader : IEstimateValueReader
    {
        public double GetValue(Estimate e)
        {
            return e.Max;
        }
    }
}