using System;
using System.Xml.Serialization;

namespace AlwaysOn.Objects
{
	[XmlRoot("userObject")]
	public class UserObject
	{
		
		[XmlElement(ElementName = "name")]
		public string Name { get; set; }

		[XmlElement(ElementName = "surname")]
		public string Surname { get; set; }

		[XmlElement(ElementName = "user_id")]
		public string UserId { get; set; }

		[XmlElement(ElementName = "accountstatus_id")]
		public string AccountStatusID { get; set; }

		[XmlElement(ElementName = "country_id")]
		public string CountryId { get; set; }

		[XmlElement(ElementName = "date_created")]
		public string DateCreated { get; set; }

		[XmlElement(ElementName = "email_enc")]
		public string EmailEnc { get; set; }

		[XmlElement(ElementName = "login_credential")]
		public string LoginCredential { get; set; }

		[XmlElement(ElementName = "mobile_number")]
		public string MobileNumber { get; set; }

		[XmlElement(ElementName = "title")]
		public string Title { get; set; }

		[XmlElement(ElementName = "remember_me")]
		public bool RememberMe { get; set; }

		[XmlElement(ElementName = "password")]
		public string Password { get; set; }
	}
}

