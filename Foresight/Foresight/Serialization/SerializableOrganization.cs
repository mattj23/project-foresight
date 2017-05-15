using System.Collections.Generic;
using System.Linq;

namespace Foresight.Serialization
{
    public class SerializableOrganization
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public List<ResourceGroup> ResourceGroups { get; set; }
        public List<SerializableEmployee> Employees { get; set; }

        public static SerializableOrganization FromOrganization(Organization item)
        {
            return new SerializableOrganization
            {
                Name = item.Name,
                Description = item.Description,
                ResourceGroups = item.ResourceGroups,
                Employees = item.Employees.Select(SerializableEmployee.FromEmployee).ToList()
            };
        }

        public static Organization ToOrganization(SerializableOrganization item)
        {
            var working = new Organization
            {
                Name = item.Name,
                Description = item.Description,
                ResourceGroups = item.ResourceGroups,
                Employees = new List<Employee>()
            };

            foreach (var serializableEmployee in item.Employees)
            {
                working.Employees.Add(new Employee
                {
                    Name = serializableEmployee.Name,
                    Group = working.ResourceGroups.FirstOrDefault(x => x.Name == serializableEmployee.ResourceGroupName)
                });
            }

            return working;
        }
    }
}