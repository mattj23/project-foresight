using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Foresight;
using Foresight.Serialization;
using Project_Foresight.ViewModels;

namespace Project_Foresight.Serialization
{
    public class SerializableOrganizationViewModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public List<ResourceGroup> ResourceGroups { get; set; }
        public List<SerializableEmployee> Employees { get; set; }
        public List<CategoryViewModel> Categories { get; set; }


        public static SerializableOrganizationViewModel FromOrganizationViewModel(OrganizationViewModel item)
        {
            return new SerializableOrganizationViewModel
            {
                Name = item.Name,
                Description = item.Description,
                ResourceGroups = item.ResourceGroups.Select(x => x.Model).ToList(),
                Employees = item.Employees.Select(x => SerializableEmployee.FromEmployee(x.Model)).ToList(),
                Categories = new List<CategoryViewModel>(item.Categories)
            };
        }

        public static OrganizationViewModel ToOrganizationViewModel(SerializableOrganizationViewModel item)
        {
            var working = new OrganizationViewModel()
            {
                Name = item.Name,
                Description = item.Description,
               
            };

            foreach (var itemResourceGroup in item.ResourceGroups)
            {
                working.ResourceGroups.Add(new ResourceGroupViewModel(itemResourceGroup));
            }

            foreach (SerializableEmployee serializableEmployee in item.Employees)
            {
                working.Employees.Add(new EmployeeViewModel()
                {
                    Name = serializableEmployee.Name,
                    Group = working.ResourceGroups.FirstOrDefault(x => x.Name == serializableEmployee.ResourceGroupName),
                    ResourceGroupName = serializableEmployee.ResourceGroupName
                });
            }

            if (item.Categories == null)
                item.Categories = new List<CategoryViewModel>();
            foreach (var categoryViewModel in item.Categories)
            {
                working.Categories.Add(categoryViewModel);
            }

            return working;
        }

    }
}