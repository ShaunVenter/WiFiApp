using System;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;
using AlwaysOn.Objects;
using System.Linq;


namespace AlwaysOn
{
    public class UserPackageResponse
    {

        private List<UserPackage> packageList;

        private string _message;
        private bool _success;

        public List<UserPackage> PackageList { get { return packageList; } }
        public string Message { get { return _message; } }
        public bool Success { get { return _success; } }
        
        public UserPackageResponse(string json)
        {
            try
            {
                var jsonObject = JObject.Parse(json);
                JToken token = JToken.Parse(json);

                if ((bool)jsonObject["Success"] != false)
                {
                    JArray packages = (JArray)token.SelectToken("UserPackages");
                    var package = packages.Select(item => new UserPackage()
                    {
                        Id = int.Parse((item["id"] ?? "0").ToString()),
                        LoginUserName = (item["login_username"] ?? "").ToString(),
                        LoginPassword = (item["login_password"] ?? "").ToString(),
                        Active = (item["active"] ?? "").ToString(),
                        ExpiryDate = (item["expiry_date"] ?? "").ToString(),
                        CredentialDesc = (item["credential_desc"] ?? "").ToString(),
                        AccountTypeDesc = (item["accounttype_desc"] ?? "").ToString(),
                        GroupDesc = (item["group_desc"] ?? "").ToString(),
                        GroupName = (item["group_name"] ?? "").ToString(),
                        MacAddress = (item["mac_address"] ?? "").ToString(),
                        ServiceProviderId = (item["service_provider_id"] ?? "").ToString(),
                        CreateDate = (item["create_date"] ?? "").ToString(),
                        PackageID = (item["package_id"] ?? "").ToString(),
                        UsageLeftPercentage = (item["usageleft_percentage"] ?? "").ToString().Split(',')[0],
                        UsageLeftvalue = (item["usageleft_value"] ?? "").ToString(),
                        UseRank = int.Parse((item["use_rank"] ?? "0").ToString())
                    })
                    .ToList();
                    
                    //var EnabledPackage = package.Where(x => x.isEnabled).ToList();
                    //var OtherServiceProviders = package.Where(x => x.ServiceProviderId.Trim() != "1" && !x.isEnabled).ToList();
                    //var CombineList = EnabledPackage.Concat(OtherServiceProviders).ToList();

                    //var AlwaysOnList = package.Where(p => !CombineList.Any(p2 => p2.Id == p.Id)).ToList().OrderBy(n => n.UseRank).ToList();
                    var AlwaysOnList = package.OrderBy(n => n.UseRank).ToList();

                    //AlwaysOnList.Sort((x, y) => int.Parse(y.UsageLeftPercentage).CompareTo(int.Parse(x.UsageLeftPercentage)));

                    packageList = new List<UserPackage>();

                    //packageList.AddRange(EnabledPackage);
                    //packageList.AddRange(OtherServiceProviders);
                    packageList.AddRange(AlwaysOnList);

                    //packageList =  EnabledPackage.Concat(OtherServiceProviders).Concat(AlwaysOnList).ToList();
                }


                _success = (bool)jsonObject["Success"];
                _message = (string)jsonObject["Message"];


            }
            catch (Exception ex)
            {
                throw new Exception("Could not translate user package response. More details: " + ex.Message);
            }
        }

        bool IsString(object value)
        {
            return value is string;
        }
    }
}

