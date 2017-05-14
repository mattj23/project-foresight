namespace Foresight
{
    public class ResourceGroup : IResource
    {
        public string Name { get; set; }
        public double Rate { get; set; }
        public int Available { get; set; }
    }
}