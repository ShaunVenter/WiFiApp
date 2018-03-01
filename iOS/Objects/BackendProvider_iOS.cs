using AlwaysOn;
using AlwaysOn.Objects;
using AlwaysOn_iOS.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AlwaysOn_iOS
{
    public class BackendProvider_iOS
    {
        public static void ClearSettings()
        {
            DBHelper.Database.UserDelete();
            DBHelper.Database.PackageDeleteAll();
            DBHelper.Database.UserSettingsDelete();
        }

        public static void SetUser(UserResponse userObj, bool RememberUser, string Password)
        {
            var u = DBHelper.Database.UserGet() ?? new dbUser();

            u.Name = userObj.Name;
            u.Surname = userObj.Surname;
            u.AccountStatusID = userObj.AccountsStatus;
            u.RememberMe = RememberUser;
            u.CountryId = userObj.CountryID;
            u.DateCreated = userObj.DateCreated;
            u.EmailEnc = userObj.EmailEnc;
            u.LoginCredential = userObj.LoginCredential;
            u.MobileNumber = userObj.MobileNumber;
            u.Title = userObj.Title;
            u.UserId = userObj.UserId;
            u.Password = Password;

            DBHelper.Database.UserUpdate(u);
        }

        public static UserObject GetUser
        {
            get
            {
                var u = DBHelper.Database.UserGet();
                if (u != null)
                {
                    return new UserObject()
                    {
                        Name = u.Name,
                        Surname = u.Surname,
                        AccountStatusID = u.AccountStatusID,
                        RememberMe = u.RememberMe,
                        CountryId = u.CountryId,
                        DateCreated = u.DateCreated,
                        EmailEnc = u.EmailEnc,
                        LoginCredential = u.LoginCredential,
                        MobileNumber = u.MobileNumber,
                        Title = u.Title,
                        UserId = u.UserId,
                        Password = u.Password
                    };
                }

                return null;
            }
        }

        public static void SetUserSettings(UserSettings settingsObj)
        {
            var u = DBHelper.Database.UserSettingsGet() ?? new dbUserSettings();

            u.Notifications = settingsObj.Notifications;
            u.NotificationText = settingsObj.NotificationText;
            u.NotificationSound = settingsObj.NotificationSound;
            u.NotificationAvailable = settingsObj.NotificationAvailable;

            u.LastConnectionNotification = settingsObj.LastConnectionNotification;
            u.LastAvailableNotification = settingsObj.LastAvailableNotification;

            DBHelper.Database.UserSettingsUpdate(u);
        }

        public static UserSettings GetUserSettings
        {
            get
            {
                var u = DBHelper.Database.UserSettingsGet() ?? new dbUserSettings();

                return new UserSettings()
                {
                    Notifications = u.Notifications,
                    NotificationText = u.NotificationText,
                    NotificationSound = u.NotificationSound,
                    NotificationAvailable = u.NotificationAvailable,

                    LastConnectionNotification = u.LastConnectionNotification,
                    LastAvailableNotification = u.LastAvailableNotification
                };
            }
        }

        public static void SetPackages(List<UserPackage> packages)
        {
            SetPackages(packages, true);
        }

        private static async void SetPackages(List<UserPackage> packages, bool setServer)
        {
            DBHelper.Database.PackageUpdate(packages.Select(n => new dbPackage()
            {
                Id = n.Id,
                LoginUserName = n.LoginUserName,
                LoginPassword = n.LoginPassword,
                Active = n.Active,
                ExpiryDate = n.ExpiryDate,
                CredentialDesc = n.CredentialDesc,
                AccountTypeDesc = n.AccountTypeDesc,
                GroupDesc = n.GroupDesc,
                GroupName = n.GroupName,
                MacAddress = n.MacAddress,
                ServiceProviderId = n.ServiceProviderId,
                CreateDate = n.CreateDate,
                PackageID = n.PackageID,
                UsageLeftPercentage = n.UsageLeftPercentage,
                UsageLeftvalue = n.UsageLeftvalue,
                UseRank = n.UseRank,
                UsageValue = n.UsageValue,
                UsagePercentValue = n.UsagePercentValue,
                Unit = n.Unit,
                GroupUnit = n.GroupUnit,
                GroupNumber = n.GroupNumber
            }).ToList());

            if (setServer)
            {
                SetPackagesToServer(packages);
            }
        }

        private static async void SetPackagesToServer(List<UserPackage> packages)
        {
            var packageIds = string.Join(",", packages.Select(n => n.Id));
            var packageRanks = string.Join(",", packages.Select((n, i) => i + 1));
            //Do Server Ranking Update
            var updatePackageRanking = await BackendProvider.UpdatePackageRanking(AppDelegate.ApiKey, GetUser.UserId, packageIds, packageRanks);

            var packageRanking = DBHelper.Database.PackageRankingUpdatedGet() ?? new dbPackageRankingUpdated();
            packageRanking.Updated = updatePackageRanking.Result == OperationResult.Success;

            DBHelper.Database.PackageRankingUpdatedUpdate(packageRanking);
        }

        public static List<UserPackage> GetStoredPackages()
        {
            return DBHelper.Database.PackageGetAll().Select(n => new UserPackage()
            {
                dbPackageID = n.dbPackageID,
                Id = n.Id,
                LoginUserName = n.LoginUserName,
                LoginPassword = n.LoginPassword,
                Active = n.Active,
                ExpiryDate = n.ExpiryDate,
                CredentialDesc = n.CredentialDesc,
                AccountTypeDesc = n.AccountTypeDesc,
                GroupDesc = n.GroupDesc,
                GroupName = n.GroupName,
                MacAddress = n.MacAddress,
                ServiceProviderId = n.ServiceProviderId,
                CreateDate = n.CreateDate,
                PackageID = n.PackageID,
                UsageLeftPercentage = n.UsageLeftPercentage,
                UsageLeftvalue = n.UsageLeftvalue,
                UseRank = n.UseRank,
                UsageValue = n.UsageValue,
                UsagePercentValue = n.UsagePercentValue,
                Unit = n.Unit,
                GroupUnit = n.GroupUnit,
                GroupNumber = n.GroupNumber
            }).ToList();
        }
        
        public static void UpdatePackagesFromServer()
        {
            Task.Factory.StartNew(() =>
            {
                try
                {
                    var PackageRankUpdated = DBHelper.Database.PackageRankingUpdatedGet();
                    if (PackageRankUpdated != null && !PackageRankUpdated.Updated)
                    {
                        SetPackagesToServer(GetStoredPackages());
                    }

                    //Read from service
                    var getServicePackages = BackendProvider.GetUserPackagesSync(AppDelegate.ApiKey, GetUser.UserId, DBHelper.Database.HotspotHelperGet().SessionID ?? "");
                    if (getServicePackages != null)
                    {
                        //Set Stored Packages
                        SetPackages(getServicePackages, false);
                    }
                }
                catch (Exception ex)
                {
                }
            }, TaskCreationOptions.LongRunning);
        }

        public static List<UserPackage> GetPackagesFromServerSync()
        {
            try
            {
                //Read from service
                var getServicePackages = BackendProvider.GetUserPackagesSync(AppDelegate.ApiKey, GetUser.UserId, DBHelper.Database.HotspotHelperGet().SessionID ?? "");
                if (getServicePackages != null)
                {
                    //Set Stored Packages
                    SetPackages(getServicePackages, false);

                    return getServicePackages;
                }
            }
            catch (Exception ex)
            {
            }

            return new List<UserPackage>();
        }

        public static UserPackage FirstAvailablePackage
        {
            get
            {
                try
                {
                    UpdatePackagesFromServer();

                    return GetStoredPackages().OrderBy(n => n.UseRank).Where(n => Convert.ToInt32(n.UsageLeftPercentage) > 0).FirstOrDefault();
                }
                catch { }

                return null;
            }
        }

        public static async Task<InfoButton> GetInfoButton()
        {
            InfoButton _infoButton = new InfoButton();
            Operation getInfoButton = await BackendProvider.GetInfoButton(AppDelegate.ApiKey);
            if (getInfoButton.Result == OperationResult.Success)
            {
                var inforesp = (InfoButtonResponse)getInfoButton.Response;
                _infoButton = new InfoButton() { Title = inforesp.Title, Url = inforesp.URL };
            }
            return _infoButton;
        }

        private static Operation SetServiceProviders(ServiceProviders serviceProviders)
        {
            var operation = new Operation("Store service providers");

            if (serviceProviders?.Items?.Count > 0)
            {
                DBHelper.Database.ServiceProviderUpdate(serviceProviders.Items.Select(n => new dbServiceProvider()
                {
                    Id = n.Id,
                    Name = n.Name
                }).ToList());
            }

            operation.CreateSuccessfulResult(null);

            return operation;
        }

        public static Operation GetStoredServiceProviders()
        {
            var operation = new Operation("Read service providers");

            var ServiceProviders = DBHelper.Database.ServiceProviderGetAll();
            if (ServiceProviders.Count() > 0)
            {
                var sp = new ServiceProviders()
                {
                    Items = ServiceProviders.Select(n => new ServiceProvider()
                    {
                        Id = n.Id,
                        Name = n.Name
                    }).ToList()
                };

                operation.CreateSuccessfulResult(sp);
            }
            else
            {
                operation.CreateFailingResult("No Service Providers");
            }

            return operation;
        }

        public static void GetServiceProviders()
        {
            Task.Factory.StartNew(async () =>
            {
                try
                {
                    //attempt to read from sandbox
                    var operation = GetStoredServiceProviders();
                    if (operation.Result == OperationResult.Success)
                    {
                        return;
                    }

                    //attempt to request from service
                    operation = await BackendProvider.GetServiceProviders(AppDelegate.ApiKey);
                    if (operation.Result == OperationResult.Failure)
                    {
                        return;
                    }

                    //save newly acquired service providers
                    var serviceProvidersResponse = (ServiceProvidersResponse)operation.Response;
                    var serviceProviders = new ServiceProviders(serviceProvidersResponse.ServiceProviders);
                    SetServiceProviders(serviceProviders);
                }
                catch (Exception ex)
                {
                }
            }, TaskCreationOptions.LongRunning);
        }

        public static string GetServiceProviderName(string id)
        {
            if (int.TryParse(id, out int Id))
            {
                var ServiceProviderName = DBHelper.Database.ServiceProviderGetByID(Id.ToString())?.Name;

                return !string.IsNullOrEmpty(ServiceProviderName) ? ServiceProviderName : "Unknown";
            }

            return "Unknown";
        }

        public static bool IsValidEmail(string strIn)
        {
            if (string.IsNullOrEmpty(strIn))
                return false;

            // Return true if strIn is in valid e-mail format.
            try
            {
                return Regex.IsMatch(strIn,
                    @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                    @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$",
                    RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
        }

        public static List<string> LookupSSIDList(List<string> SSIDList)
        {
            var updateList = SSIDList
                            .Distinct()
                            .Select(n => new dbHotspotSSID() { SSID = n, Valid = false, Verified = false })
                            .ToList();

            var staticSpots = new string[] { "AlwaysOn", "@VAST" };

            foreach (var s in staticSpots)
            {
                if (!SSIDList.Contains(s))
                {
                    updateList.Add(new dbHotspotSSID() { SSID = s, Valid = true, Verified = true });
                }
            }

            DBHelper.Database.HotspotSSIDUpdate(updateList);

            //Async Server SSID Update
            GetServerSSIDs();

            //Return Existing SSIDs in the meantime
            return DBHelper.Database.HotspotSSIDGetAllValidAndVerified(SSIDList).Select(n => n.SSID).ToList();
        }

        static bool ServerSSIDRunning = false;
        public static void GetServerSSIDs()
        {
            try
            {
                if (ServerSSIDRunning) return;

                ServerSSIDRunning = true;
                Task.Factory.StartNew(() =>
                {
                    var SSIDList = DBHelper.Database.HotspotSSIDGetAllInvalid().Select(n => n.SSID).ToList();
                    if (SSIDList != null && SSIDList.Count > 0)
                    {
                        using (var client = new HotspotHelperWebClient())
                        {
                            while (SSIDList.Count > 0)
                            {
                                var tempSSIDs = SSIDList.Take(15).ToList();
                                SSIDList.RemoveAll(n => tempSSIDs.Contains(n));

                                try
                                {
                                    client.Timeout = 5000;

                                    var ListToUpdate = new List<dbHotspotSSID>();

                                    var parameters = "?api_key=" + AppDelegate.ApiKey + "&ssids=" + WebUtility.UrlEncode(string.Join(",", tempSSIDs));
                                    string json = client.UploadString(ConfigurationProvider.GetSSIDs + parameters, "");

                                    var response = new SSIDResponse(json);
                                    if (response.Success)
                                    {
                                        //Verified & Valid SSIDs
                                        ListToUpdate.AddRange(response.SSIDList.Select(n => new dbHotspotSSID() { SSID = n, Valid = true, Verified = true }));
                                        //Verified & Invalid SSIDs
                                        ListToUpdate.AddRange(tempSSIDs.Where(n => !response.SSIDList.Contains(n)).Select(n => new dbHotspotSSID() { SSID = n, Valid = false, Verified = true }));

                                        DBHelper.Database.HotspotSSIDUpdate(ListToUpdate);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    //Timeout
                                    //Save SSID as invalid and unverified
                                }
                            }
                        }
                    }

                    ServerSSIDRunning = false;
                }, TaskCreationOptions.LongRunning);
            }
            catch (Exception ex)
            {
            }
        }
    }

    public class UIViewPosition
    {
        public UIKit.UIView uiView { get; set; }
        public float originalY { get; set; }
        public float newY { get; set; }
    }

    public class AlertTextfieldFeedback
    {
        public nint ButtonIndex { get; set; }
        public string TextFieldText { get; set; }
    }
}

