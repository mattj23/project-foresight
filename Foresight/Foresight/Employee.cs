namespace Foresight
{
    public class Employee : IResource
    {
        public string Name { get; set; }
        public ResourceGroup Group { get; set; }
        public double Rate => Group.Rate;
        public int Available => 1;
    }
}