namespace Foresight.Serialization
{
    public class SerializableEmployee
    {
        public string Name { get; set; }
        public string ResourceGroupName { get; set; }

        public static SerializableEmployee FromEmployee(Employee item)
        {
            return new SerializableEmployee
            {
                Name = item.Name,
                ResourceGroupName = item.Group?.Name
            };
        }

        
    }
}