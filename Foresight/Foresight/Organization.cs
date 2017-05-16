using System.Collections.Generic;
using System.Linq;

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

        public IResource GetResourceByName(string resourceName)
        {
            var employee = this.Employees.FirstOrDefault(x => x.Name == resourceName);
            if (employee != null)
                return employee;

            var resourceGroup = this.ResourceGroups.FirstOrDefault(x => x.Name == resourceName);
            return resourceGroup;
        }
    }
}