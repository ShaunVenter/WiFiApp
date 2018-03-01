using System;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;
using AlwaysOn.Objects;

namespace AlwaysOn
{
    public class UserResponse
    {
        public string Message { get; }
        public bool Success { get; }
        public bool IsPackageCredentials { get; }
        public bool IsPackageLinked { get; }
        public string PackageLinkedAccount { get; }
        public string UserId { get; set; }
        public string AccountsStatus { get; set; }
        public string CountryID { get; set; }
        public string DateCreated { get; set; }
        public string EmailEnc { get; set; }
        public string LoginCredential { get; set; }
        public string MobileNumber { get; set; }
        public string Title { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }

        public UserResponse()
        {
        }

        public UserResponse(string json)
        {
            try
            {
                var jsonObject = JObject.Parse(json);
                var userProfile = (object)jsonObject["UserProfile"];

                var obj = JsonConvert.SerializeObject((object)jsonObject["UserProfile"]);
                var userObject = JsonConvert.DeserializeObject<User>(obj);

                if (obj == "null")
                {
                    Success = (bool)jsonObject["Success"];
                    Message = (string)jsonObject["Message"];
                    IsPackageCredentials = (bool)jsonObject["IsPackageCredentials"];
                    IsPackageLinked = (bool)jsonObject["IsPackageLinked"];
                    PackageLinkedAccount = (string)jsonObject["PackageLinkedAccount"];
                }
                else
                {
                    Success = (bool)jsonObject["Success"];
                    Message = (string)jsonObject["Message"];

                    Name = string.IsNullOrWhiteSpace(userObject.name) ? string.Empty : userObject.name;
                    Surname = string.IsNullOrWhiteSpace(userObject.surname) ? string.Empty : userObject.surname;
                    UserId = string.IsNullOrWhiteSpace(userObject.user_id) ? string.Empty : userObject.user_id;
                    AccountsStatus = string.IsNullOrWhiteSpace(userObject.acountstatus_id) ? string.Empty : userObject.acountstatus_id;
                    CountryID = string.IsNullOrWhiteSpace(userObject.country_id) ? string.Empty : userObject.country_id;
                    DateCreated = string.IsNullOrWhiteSpace(userObject.date_created) ? string.Empty : userObject.date_created;
                    EmailEnc = string.IsNullOrWhiteSpace(userObject.email_enc) ? string.Empty : userObject.email_enc;
                    LoginCredential = string.IsNullOrWhiteSpace(userObject.login_credential) ? string.Empty : userObject.login_credential;
                    MobileNumber = string.IsNullOrWhiteSpace(userObject.mobile_number) ? string.Empty : userObject.mobile_number;
                    Title = string.IsNullOrWhiteSpace(userObject.title) ? string.Empty : userObject.title;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Could not translate user response. More details: " + ex.Message);
            }
        }
    }
}

