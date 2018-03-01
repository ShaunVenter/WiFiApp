using System;
using System.Xml.Serialization;

namespace AlwaysOn
{
	public class ServiceProvider
	{
		//properties

		[XmlElement(ElementName = "Id")]
		public string Id { get; set; }

		[XmlElement(ElementName = "Name")]
		public string Name { get; set; }


		//methods

		public ServiceProvider()
		{
		}

		public ServiceProvider (string id, string description)
		{
			if (string.IsNullOrWhiteSpace(id))
			{
				throw new ArgumentNullException("Empty service provider ID is not allowed");
			}

			if (string.IsNullOrWhiteSpace(description))
			{
				throw new ArgumentNullException("Empty service provider name is not allowed");
			}

			Id = id.Trim();
			Name = description.Trim();
		}
	}
}

