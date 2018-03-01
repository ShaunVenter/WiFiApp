using AlwaysOn.Objects;
using ModernHttpClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;


namespace AlwaysOn
{
    public class BackendProvider
    {
        public static async Task<Operation> RegisterUser(string ApiKey, string Name, string Surname, string UserName, string MobileNumber, string Password)
        {
            var operationResult = new Operation("User Registration");


            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(ConfigurationProvider.RegistrationUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(ConfigurationProvider.BackendEncodingType));

                    var parameters = "?api_key=" + ApiKey 
                                   + "&name=" + WebUtility.UrlEncode(Name)
                                   + "&surname=" + WebUtility.UrlEncode(Surname)
                                   + "&username=" + WebUtility.UrlEncode(UserName)
                                   + "&password=" + WebUtility.UrlEncode(Password)
                                   + "&mobile=" + WebUtility.UrlEncode(MobileNumber);

                    HttpResponseMessage response = await client.PostAsync(client.BaseAddress + parameters, new StringContent(parameters));

                    response.EnsureSuccessStatusCode();


                    string json = await response.Content.ReadAsStringAsync();

                    var connectionResponse = (new UserResponse(json));

                    if (connectionResponse.Success == false)
                    {
                        operationResult.CreateFailingResult(connectionResponse.Message);
                    }
                    else
                    {
                        operationResult.CreateSuccessfulResult(connectionResponse);
                    }
                }
            }
            catch (Exception ex)
            {
                operationResult.CreateFailingResult(string.Empty);
            }

            return operationResult;
        }

        public static async Task<Operation> ResetPassword(string ApiKey, string Email, string PhoneNumber)
        {
            var operationResult = new Operation("Reset Password");


            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(ConfigurationProvider.ResetPasswordUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(ConfigurationProvider.BackendEncodingType));


                    var parameters = "?api_key=" + ApiKey 
                                   + "&email=" + WebUtility.UrlEncode(Email)
                                   + "&mobile=" + WebUtility.UrlEncode(PhoneNumber);

                    HttpResponseMessage response = await client.PostAsync(client.BaseAddress + parameters, new StringContent(parameters));

                    response.EnsureSuccessStatusCode();


                    string json = await response.Content.ReadAsStringAsync();

                    var resetResponse = (new ResetPasswordResponse(json));

                    if (resetResponse.Success == false)
                    {
                        operationResult.CreateFailingResult("Unable to reset Password");

                    }
                    else
                    {
                        operationResult.CreateSuccessfulResult(resetResponse);
                    }
                }
            }
            catch (Exception ex)
            {
                operationResult.CreateFailingResult(string.Empty);
            }

            return operationResult;

        }

        public static async Task<Operation> UpdateProfile(string ApiKey, string UserId, string Title, string Name, string Surname, string Email, string MobileNumber)
        {

            //string user_id, string title, string name, string surname, string email, string mobile
            var operationResult = new Operation("User Registration");


            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(ConfigurationProvider.UpdateProfileUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(ConfigurationProvider.BackendEncodingType));

                    var parameters = "?api_key=" + ApiKey 
                                   + "&user_id=" + UserId 
                                   + "&title=" + WebUtility.UrlEncode(Title)
                                   + "&name=" + WebUtility.UrlEncode(Name)
                                   + "&surname=" + WebUtility.UrlEncode(Surname)
                                   + "&email=" + WebUtility.UrlEncode(Email)
                                   + "&mobile=" + WebUtility.UrlEncode(MobileNumber);
                    HttpResponseMessage response = await client.PostAsync(client.BaseAddress + parameters, new StringContent(parameters));

                    response.EnsureSuccessStatusCode();


                    string json = await response.Content.ReadAsStringAsync();

                    var UpdateProfileResponse = (new ProfileResponse(json));

                    if (UpdateProfileResponse.Success == false)
                    {
                        operationResult.CreateFailingResult(UpdateProfileResponse.Message);

                    }
                    else
                    {
                        //blaf
                        operationResult.CreateSuccessfulResult(UpdateProfileResponse);

                    }


                }

            }
            catch (Exception ex)
            {
                operationResult.CreateFailingResult(string.Empty);
            }

            return operationResult;
        }

        public static async Task<Operation> UserLogin(string ApiKey, string Email, string Password, bool RememberMe)
        {
            var operationResult = new Operation("User Login");


            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(ConfigurationProvider.LoginUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(ConfigurationProvider.BackendEncodingType));


                    var parameters = "?api_key=" + ApiKey 
                                   + "&username=" + WebUtility.UrlEncode(Email)
                                   + "&password=" + WebUtility.UrlEncode(Password);

                    HttpResponseMessage response = await client.PostAsync(client.BaseAddress + parameters, new StringContent(parameters));

                    response.EnsureSuccessStatusCode();


                    string json = await response.Content.ReadAsStringAsync();

                    var userResponse = new UserResponse(json);
                    if (userResponse.Success == false)
                    {
                        operationResult.CreateFailingResult("Invalid email or password");
                    }
                    else
                    {
                        operationResult.CreateSuccessfulResult(userResponse);
                    }
                }
            }
            catch (Exception ex)
            {
                var exception = ex.Message + ex.StackTrace;
                operationResult.CreateFailingResult("Unable to sign in. The AlwaysOn server is unreachable.");
            }

            return operationResult;
        }

        //public static async Task<Operation> GetUserPackages(string UserID)
        //{
        //    var operationResult = new Operation("Get User Packages");

        //    try
        //    {
        //        using (var client = new HttpClient())
        //        {
        //            client.BaseAddress = new Uri(ConfigurationProvider.GetUserPackagesUrl);
        //            client.DefaultRequestHeaders.Accept.Clear();
        //            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(ConfigurationProvider.BackendEncodingType));
        //            client.Timeout = new TimeSpan(0, 0, 0, 3, 0);

        //            var parameters = "?api_key=" + ConfigurationProvider.ApiKey + "&user_id=" + UserID;
        //            HttpResponseMessage response = await client.PostAsync(client.BaseAddress + parameters, new StringContent(parameters));

        //            response.EnsureSuccessStatusCode();

        //            string json = await response.Content.ReadAsStringAsync();
        //            var userPackageResponse = (new UserPackageResponse(json));
        //            operationResult.CreateSuccessfulResult(userPackageResponse.PackageList);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        operationResult.CreateFailingResult(string.Empty);
        //    }

        //    return operationResult;
        //}

        public static List<UserPackage> GetUserPackagesSync(string ApiKey, string UserID, string SessionID)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(ConfigurationProvider.GetUserPackagesUrlV2);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(ConfigurationProvider.BackendEncodingType));

                try
                {
                    var parameters = "?api_key=" + ApiKey + "&user_id=" + UserID + "&sessionid=" + SessionID;
                    var response = client.PostAsync(client.BaseAddress + parameters, new StringContent(parameters)).Result;
                    response.EnsureSuccessStatusCode();
                    string json = response.Content.ReadAsStringAsync().Result;
                    return new UserPackageResponse(json).PackageList;
                }
                catch (Exception ex)
                {

                }

                return null;
            }
        }

        public static async Task<Operation> LinkPurchasedPackage(string ApiKey, string userId, string packageUsername, string packagePassword, string serviceProvider = "1")
        {
            var operationResult = new Operation("Link Purchased Package");

            serviceProvider = (serviceProvider ?? "1").ToString().Trim();

            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(ConfigurationProvider.LinkExistingCredentialsUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(ConfigurationProvider.BackendEncodingType));

                    var parameters = "?api_key=" + ApiKey
                                   + "&user_id=" + userId
                                   + "&username=" + WebUtility.UrlEncode(packageUsername)
                                   + "&password=" + WebUtility.UrlEncode(packagePassword)
                                   + "&service_provider=" + serviceProvider;
                    
                    HttpResponseMessage response = await client.PostAsync(client.BaseAddress + parameters, new StringContent(parameters));
                    response.EnsureSuccessStatusCode();

                    string json = await response.Content.ReadAsStringAsync();
                    var linkPurchasedPackageResponse = (new LinkPurchasedPackageResponse(json));

                    if (linkPurchasedPackageResponse.Success)
                    {
                        operationResult.CreateSuccessfulResult(linkPurchasedPackageResponse);
                    }
                    else
                    {
                        operationResult.CreateFailingResult(linkPurchasedPackageResponse.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                operationResult.CreateFailingResult(string.Empty);
            }

            return operationResult;
        }

        public static async Task<Operation> LinkAccessCode(string ApiKey, string userId, string code)
        {
            var operationResult = new Operation("Link Access Code");

            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(ConfigurationProvider.LinkSingleCode);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(ConfigurationProvider.BackendEncodingType));

                    var parameters = "?api_key=" + ApiKey
                                   + "&user_id=" + userId
                                   + "&code=" + WebUtility.UrlEncode(code);

                    HttpResponseMessage response = await client.PostAsync(client.BaseAddress + parameters, new StringContent(parameters));
                    response.EnsureSuccessStatusCode();

                    string json = await response.Content.ReadAsStringAsync();
                    var linkPurchasedPackageResponse = (new LinkPurchasedPackageResponse(json));

                    if (linkPurchasedPackageResponse.Success)
                    {
                        operationResult.CreateSuccessfulResult(linkPurchasedPackageResponse);
                    }
                    else
                    {
                        operationResult.CreateFailingResult(linkPurchasedPackageResponse.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                operationResult.CreateFailingResult(string.Empty);
            }

            return operationResult;
        }

        public static UnlinkPackageResponse UnlinkPackageSync(string ApiKey, int PackageId)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(ConfigurationProvider.UnlinkExistingCredentialsUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(ConfigurationProvider.BackendEncodingType));

                try
                {
                    var parameters = "?api_key=" + ApiKey + "&user_package_id=" + PackageId;
                    var response = client.PostAsync(client.BaseAddress + parameters, new StringContent(parameters)).Result;
                    response.EnsureSuccessStatusCode();
                    string json = response.Content.ReadAsStringAsync().Result;
                    return new UnlinkPackageResponse(json);
                }
                catch (Exception ex)
                {

                }

                return null;
            }
        }

        #region Hotspot Finder

        public static event EventHandler<HotspotResponse> HotspotsReceived;

        protected static void OnHotspotsReceived(object sender, HotspotResponse e)
        {
            if (HotspotsReceived != null)
                HotspotsReceived(sender, e);
        }

        /// <summary>
        /// You have to hook up the event HotspotsReceived in order to receive the hotspot list
        /// </summary>
        public static void GetHotspotsInternational(string ApiKey, RadiusCoordinates Coordinates)
        {
            Coordinates.SetRadiusPoints(0.1);

            var maxHotspots = 100;

            GetHotspotsByBoundsInternational(ApiKey,
                                             Coordinates.pointALat.ToString(),
                                             Coordinates.pointBLat.ToString(),
                                             Coordinates.pointALong.ToString(),
                                             Coordinates.pointBLong.ToString(),
                                             Coordinates.CurrentLat.ToString(),
                                             Coordinates.CurrentLong.ToString(),
                                             Coordinates.CurrentLat.ToString(),
                                             Coordinates.CurrentLong.ToString(),
                                             maxHotspots);
        }

        static object locker = new object();
        /// <summary>
        /// You have to hook up the event HotspotsReceived in order to receive the hotspot list
        /// </summary>
        public static void GetHotspotsByBoundsInternational(string ApiKey, string latA, string latB, string lngA, string lngB, string userLat, string userLng, string centerLat, string centerLng, int limit)
        {
            Task.Factory.StartNew(() =>
            {
                lock (locker)
                {
                    try
                    {
                        using (var client = new HttpClient())
                        {
                            client.Timeout = new TimeSpan(0, 0, 45);
                            client.BaseAddress = new Uri(ConfigurationProvider.GetHotspotLocationsInternational);
                            client.DefaultRequestHeaders.Accept.Clear();
                            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(ConfigurationProvider.BackendEncodingType));

                            var parameters = "?api_key=" + ApiKey
                                        + "&lata=" + latA.Replace('.', ',')
                                        + "&latb=" + latB.Replace('.', ',')
                                        + "&lnga=" + lngA.Replace('.', ',')
                                        + "&lngb=" + lngB.Replace('.', ',')
                                        + "&userlat=" + userLat.Replace('.', ',')
                                        + "&userlng=" + userLng.Replace('.', ',')
                                        + "&centerlat=" + centerLat.Replace('.', ',')
                                        + "&centerlng=" + centerLng.Replace('.', ',')
                                        + "&limit=" + limit;

                            HttpResponseMessage response = client.PostAsync(client.BaseAddress + parameters, new StringContent(parameters)).Result;
                            response.EnsureSuccessStatusCode();

                            string json = response.Content.ReadAsStringAsync().Result;

                            OnHotspotsReceived(new { }, new HotspotResponse(json));
                        }
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }, TaskCreationOptions.LongRunning);
        }
        
        #endregion Hotspot Finder

        public static async Task<Operation> GetServiceProviders(string ApiKey)
        {
            var operation = new Operation("Request service providers");

            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(ConfigurationProvider.GetServiceProvidersUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(ConfigurationProvider.BackendEncodingType));
                    client.Timeout = new TimeSpan(0, 0, 0, 3, 0);

                    StringBuilder parameters = new StringBuilder("?api_key=" + ApiKey);
                    HttpResponseMessage response = await client.PostAsync(client.BaseAddress + parameters.ToString(), new StringContent(parameters.ToString()));
                    response.EnsureSuccessStatusCode();

                    string json = await response.Content.ReadAsStringAsync();
                    var serviceProvidersResponse = (new ServiceProvidersResponse(json));

                    if (!serviceProvidersResponse.Success)
                    {
                        operation.CreateFailingResult(serviceProvidersResponse.Message);
                    }
                    else
                    {
                        operation.CreateSuccessfulResult(serviceProvidersResponse);
                    }
                }
            }
            catch (Exception ex)
            {
                var exception = ex.Message + ex.StackTrace;
                operation.CreateFailingResult(string.Empty);
            }

            return operation;
        }

        public static async Task<Operation> GetInfoButton(string ApiKey)
        {
            var operationResult = new Operation("Get Info Button");

            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(ConfigurationProvider.GetInfoButton);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(ConfigurationProvider.BackendEncodingType));
                    client.Timeout = new TimeSpan(0, 0, 0, 3, 0);

                    var parameters = "?api_key=" + ApiKey;
                    HttpResponseMessage response = await client.PostAsync(client.BaseAddress + parameters, new StringContent(parameters));

                    response.EnsureSuccessStatusCode();

                    string json = await response.Content.ReadAsStringAsync();
                    var infoButtonResponse = (new InfoButtonResponse(json));
                    operationResult.CreateSuccessfulResult(infoButtonResponse);
                }
            }
            catch (Exception ex)
            {
                operationResult.CreateFailingResult(string.Empty);
            }

            return operationResult;
        }

        public static async Task<Operation> UpdatePackageRanking(string ApiKey, string userId, string arrPackage_ids, string arrPackage_ranks)
        {
            var operationResult = new Operation("Update Package Ranking");

            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(ConfigurationProvider.UpdatePackageRanking);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(ConfigurationProvider.BackendEncodingType));

                    StringBuilder parameters = new StringBuilder("?api_key=" + ApiKey);
                    parameters.Append("&user_id=" + userId);
                    parameters.Append("&package_ids=" + arrPackage_ids);
                    parameters.Append("&package_ranks=" + arrPackage_ranks);

                    HttpResponseMessage response = await client.PostAsync(client.BaseAddress + parameters.ToString(), new StringContent(parameters.ToString()));
                    response.EnsureSuccessStatusCode();

                    response.EnsureSuccessStatusCode();

                    string json = await response.Content.ReadAsStringAsync();
                    var updatePackageRankingResponse = (new UpdatePackageRankingResponse(json));

                    if (updatePackageRankingResponse.Success)
                    {
                        operationResult.CreateSuccessfulResult(updatePackageRankingResponse);
                    }
                    else
                    {
                        operationResult.CreateFailingResult(updatePackageRankingResponse.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                operationResult.CreateFailingResult(string.Empty);
            }

            return operationResult;
        }

        public static DefaultResponse AccurisLogin(string ApiKey, string username, string password, string sessionid)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(ConfigurationProvider.AccurisLogin);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(ConfigurationProvider.BackendEncodingType));
                    client.Timeout = new TimeSpan(0, 0, 4);

                    StringBuilder parameters = new StringBuilder("?api_key=" + ApiKey);
                    parameters.Append("&username=" + username);
                    parameters.Append("&password=" + password);
                    parameters.Append("&sessionid=" + sessionid);

                    HttpResponseMessage response = client.PostAsync(client.BaseAddress + parameters.ToString(), new StringContent(parameters.ToString())).Result;
                    
                    string json = response.Content.ReadAsStringAsync().Result;

                    return new DefaultResponse(json);
                }
            }
            catch (Exception ex)
            {
                return new DefaultResponse(false, "Could not login with the package. " + ex.Message);
            }
        }

        public static DefaultResponse AccurisLogout(string ApiKey, string sessionid)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(ConfigurationProvider.AccurisLogout);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(ConfigurationProvider.BackendEncodingType));
                    client.Timeout = new TimeSpan(0,0,4);

                    StringBuilder parameters = new StringBuilder("?api_key=" + ApiKey);
                    parameters.Append("&sessionid=" + sessionid);

                    HttpResponseMessage response = client.PostAsync(client.BaseAddress + parameters.ToString(), new StringContent(parameters.ToString())).Result;

                    string json = response.Content.ReadAsStringAsync().Result;

                    return new DefaultResponse(json);
                }
            }
            catch (Exception ex)
            {
                return new DefaultResponse(false, "Could not disconnect the package. " + ex.Message);
            }
        }

        public static string HTTPClientModernHTTPPost(string httpUrl, string Parameters, int Timeout = 1500)
        {
            try
            {
                
                using (var client = new HttpClient(new NativeMessageHandler(throwOnCaptiveNetwork: false, customSSLVerification: true)) { Timeout = new TimeSpan(0, 0, 0, 0, Timeout) })
                {
                    client.BaseAddress = new System.Uri(httpUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(ConfigurationProvider.BackendEncodingType));
                    
                    HttpResponseMessage response = client.PostAsync(client.BaseAddress + Parameters, new StringContent(Parameters)).Result;
                    response.EnsureSuccessStatusCode();
                    
                    string json = response.Content.ReadAsStringAsync().Result;

                    return json;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}

