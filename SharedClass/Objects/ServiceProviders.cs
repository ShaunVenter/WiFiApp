using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Linq;

namespace AlwaysOn
{
	[XmlRoot("ServiceProviders")]
	public class ServiceProviders
	{
		[XmlElement("ServiceProvider")]
		public List<ServiceProvider> Items { get; set; }


		public ServiceProviders()
		{
		}

		public ServiceProviders (List<ServiceProvider> serviceProviders)
		{
			if (serviceProviders == null)
			{
				Items = new List<ServiceProvider>();
			}

			Items = SortItems(serviceProviders);
		}

		private List<ServiceProvider> SortItems(List<ServiceProvider> items)
		{
			try
			{
				//sort by name asc
				items = items.OrderBy(x => x.Name).ToList();

				//put Always On item on top
				var index = items.FindIndex(x => x.Id == "1"); //the Always On item in the list

				if (index <= 0)
				{
					return items;
				}

				var item = items[index];		//swap Always On item to front 
				items.RemoveAt(index);
				items.Insert(0, item);

				return items;
			} 			
			catch (Exception ex) 
			{
				throw new Exception("Could not sort service providers. More details: " + ex.Message);
			}
			finally
			{

			}
		}
	}
}

