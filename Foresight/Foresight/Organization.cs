using System.Collections.Generic;

namespace Foresight
{
    public class Organization
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public List<ResourceGroup> ResourceGroups { get; set; }

        public List<Employee> Employees { get; set; }

        public Organization()
        {
            this.ResourceGroups = new List<ResourceGroup>();
            this.Employees = new List<Employee>();
        }
    }
}