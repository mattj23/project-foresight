namespace Foresight
{
    /// <summary>
    /// Describes a resource that can be assigned to a particular task
    /// </summary>
    public interface IResource
    {
        string Name { get; }
        
        double Rate { get; }

        int Available { get; }
    }
}