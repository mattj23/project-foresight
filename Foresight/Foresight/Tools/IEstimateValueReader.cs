namespace Foresight.Tools
{
    /// <summary>
    /// Defines objects that grab a value from an estimate
    /// </summary>
    public interface IEstimateValueReader
    {
        double GetValue(Estimate e);
    }
}
