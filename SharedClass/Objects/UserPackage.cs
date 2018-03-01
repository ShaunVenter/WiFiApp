using System;

namespace AlwaysOn.Objects
{
	public class UserPackage
	{
		public UserPackage ()
		{
		}

        public int dbPackageID { get; set; } = 0;
        public int Id {get; set;}
		public string LoginUserName {get; set;}
		public string LoginPassword {get; set;}
		public string Active {get; set;}
		public string ExpiryDate {get; set;}
		public string CredentialDesc {get; set;}
		public string AccountTypeDesc {get; set;}
		public string GroupDesc {get; set;}
		public string GroupName {get; set;}
		public string MacAddress {get; set;}
		public string ServiceProviderId {get; set;}
		public string CreateDate {get; set;}
		public string PackageID {get; set;}
		public string UsageLeftPercentage {get; set;}
		public string UsageLeftvalue {get; set;}
        public int UseRank { get; set; }
        public string UsageValue { get; set;}
		public string UsagePercentValue { get; set;}
		public string Unit { get; set;}
		public string GroupUnit { get; set;}
		public string GroupNumber { get; set;}
        public object toggle { get; set; }

        public void Sanitize()
		{
			UsageValue = string.Empty;
			Unit = string.Empty;

			try {

				if (!string.IsNullOrWhiteSpace(UsageLeftvalue)) 
				{
					string[] usage = UsageLeftvalue.Split(' ');

					if (usage.Length == 1) 
					{
						Unit = usage [0];
					} 
					else
					{
						Unit = usage [1];
						var strvalue = usage [0].Split(',');
						UsageValue = strvalue [0];
					}
				}

				if (!string.IsNullOrWhiteSpace(UsageLeftPercentage)) 
				{
					string[] usage = UsageLeftPercentage.Split(',');
					UsagePercentValue = usage [0];
				}

				SetGroupName();

			} 
			catch (Exception ex) 
			{
				throw new Exception (ex.Message);
			}
		}

		private void SetGroupName()
		{
			try 
			{
				if (string.IsNullOrWhiteSpace(GroupName))
				{
					GroupName = "Uncapped";
					return;
				}

				var groupNameCharacters = GroupName.ToCharArray();
				var number = string.Empty;
				var unit = string.Empty;
				int index = -1;

				foreach(var character in groupNameCharacters)
				{
					index += 1;
					int numberOut = 0;

					if (int.TryParse(character.ToString(), out numberOut))
					{
						number += character.ToString();
						GroupNumber = number;
					}
					else
					{
						if (character.ToString().Trim().ToLower() == "g")
						{
							unit = "GB";
							GroupUnit = unit;
						}

						if (character.ToString().Trim().ToLower() == "m")
						{
							if (index == groupNameCharacters.Length - 1)
							{
								break;
							}

							if (groupNameCharacters[index + 1].ToString().Trim().ToLower() == "i")
							{
								unit = "Minutes";
								GroupUnit = unit;
							}

							if (groupNameCharacters[index + 1].ToString().Trim().ToLower() == "b")
							{
								unit = "MB";
								GroupUnit = unit;
							}
						}

						break;
					}
				}

				FormatGroupName (GroupUnit, GroupNumber);
			} 


			catch (Exception ex) 
			{
				throw new Exception ("Could not set group name. More details: " + ex.Message);
			}
		}

		private void FormatGroupName (string number, string unit)
		{
			GroupName = string.Format ("{0} {1}", unit ,number );
		}
	}
}

