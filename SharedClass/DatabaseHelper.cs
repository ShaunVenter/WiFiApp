using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AlwaysOn
{
    public class DatabaseHelper
    {
        private SQLiteConnection database;
        static object locker = new object();

        public DatabaseHelper(SQLiteConnection conn)
        {
            database = conn;
        }

        #region User
        public dbUser UserGet()
        {
            lock (locker)
            {
                return database.Table<dbUser>().FirstOrDefault();
            }
        }
        public int UserUpdate(dbUser user)
        {
            lock (locker)
            {
                if (user.ID > 0)
                {
                    database.Update(user);
                    return user.ID;
                }
                else
                {
                    return database.Insert(user);
                }
            }
        }
        public int UserDelete()
        {
            lock (locker)
            {
                return database.DeleteAll<dbUser>();
            }
        }
        #endregion User

        #region UserSettings
        public dbUserSettings UserSettingsGet()
        {
            lock (locker)
            {
                var settings = database.Table<dbUserSettings>().FirstOrDefault();
                if(settings == null)
                {
                    UserSettingsUpdate(new dbUserSettings());

                    return database.Table<dbUserSettings>().FirstOrDefault();
                }

                return settings;
            }
        }
        public int UserSettingsUpdate(dbUserSettings userSettings)
        {
            lock (locker)
            {
                if (userSettings.ID > 0)
                {
                    database.Update(userSettings);
                    return userSettings.ID;
                }
                else
                {
                    return database.Insert(userSettings);
                }
            }
        }
        public int UserSettingsDelete()
        {
            lock (locker)
            {
                return database.DeleteAll<dbUserSettings>();
            }
        }
        #endregion UserSettings

        #region Package
        public IEnumerable<dbPackage> PackageGetAll()
        {
            lock (locker)
            {
                return database.Table<dbPackage>().ToList();
            }
        }
        public dbPackage PackageGetByID(int id)
        {
            lock (locker)
            {
                return database.Table<dbPackage>().Where(n => n.Id == id).FirstOrDefault();
            }
        }
        public int PackageUpdate(dbPackage package)
        {
            lock (locker)
            {
                var current = database.Table<dbPackage>().Where(n => n.Id == package.Id).FirstOrDefault();
                if (current != null)
                {
                    package.dbPackageID = current.dbPackageID;

                    database.Update(package);
                    return package.dbPackageID;
                }
                else
                {
                    return database.Insert(package);
                }
            }
        }
        public void PackageUpdate(List<dbPackage> packages)
        {
            lock (locker)
            {
                database.DeleteAll<dbPackage>();
                database.InsertAll(packages);
            }
        }
        public int PackageDeleteAll()
        {
            lock (locker)
            {
                return database.DeleteAll<dbPackage>();
            }
        }
        #endregion Package

        #region ServiceProvider
        public IEnumerable<dbServiceProvider> ServiceProviderGetAll()
        {
            lock (locker)
            {
                return database.Table<dbServiceProvider>().ToList();
            }
        }
        public dbServiceProvider ServiceProviderGetByID(string Id)
        {
            lock (locker)
            {
                return database.Table<dbServiceProvider>().Where(n => n.Id == Id).FirstOrDefault();
            }
        }
        public void ServiceProviderUpdate(List<dbServiceProvider> serviceProviders)
        {
            lock (locker)
            {
                foreach (var serviceProvider in serviceProviders)
                {
                    var current = database.Table<dbServiceProvider>().Where(n => n.Id == serviceProvider.Id).FirstOrDefault();
                    if (current != null)
                    {
                        serviceProvider.dbServiceProviderID = current.dbServiceProviderID;
                        database.Update(serviceProvider);
                    }
                    else
                    {
                        database.Insert(serviceProvider);
                    }
                }
            }
        }
        #endregion ServiceProvider

        #region PackageRankingUpdated
        public dbPackageRankingUpdated PackageRankingUpdatedGet()
        {
            lock (locker)
            {
                return database.Table<dbPackageRankingUpdated>().FirstOrDefault();
            }
        }
        public int PackageRankingUpdatedUpdate(dbPackageRankingUpdated packageRankingUpdated)
        {
            lock (locker)
            {
                if (packageRankingUpdated.ID > 0)
                {
                    database.Update(packageRankingUpdated);
                    return packageRankingUpdated.ID;
                }
                else
                {
                    return database.Insert(packageRankingUpdated);
                }
            }
        }
        #endregion PackageRankingUpdated

        #region HotspotSSID
        public string HotspotSSIDGet(string SSID)
        {
            lock (locker)
            {
                return database.Table<dbHotspotSSID>().Where(n => n.SSID == SSID).ToList().Select(n => n.SSID).FirstOrDefault();
            }
        }
        public IEnumerable<dbHotspotSSID> HotspotSSIDGetAll(List<string> SSIDList)
        {
            lock (locker)
            {
                return database.Table<dbHotspotSSID>().Where(n => SSIDList.Contains(n.SSID)).ToList();
            }
        }
        public IEnumerable<dbHotspotSSID> HotspotSSIDGetAllInvalid()
        {
            lock (locker)
            {
                return database.Table<dbHotspotSSID>().Where(n => !n.Verified).ToList();
            }
        }
        public IEnumerable<dbHotspotSSID> HotspotSSIDGetAllValidAndVerified(List<string> SSIDList)
        {
            lock (locker)
            {
                var SSIDs = database.Table<dbHotspotSSID>().Where(n => SSIDList.Contains(n.SSID)).ToList();

                var validVerified = SSIDs.Where(n => n.Valid && n.Verified).ToList();

                return validVerified;
            }
        }
        public void HotspotSSIDUpdate(List<dbHotspotSSID> HotspotSSIDList)
        {
            lock (locker)
            {
                foreach (var HotspotSSID in HotspotSSIDList)
                {
                    var SSID = database.Table<dbHotspotSSID>().Where(n => n.SSID == HotspotSSID.SSID).FirstOrDefault();
                    if (SSID == null)
                    {
                        //Insert
                        database.Insert(HotspotSSID);
                    }
                    else if(SSID.Valid && SSID.Verified)
                    {
                        //Do nothing
                    }
                    else
                    {
                        //Update
                        if (SSID.Valid != HotspotSSID.Valid || SSID.Verified != HotspotSSID.Verified)
                        {
                            SSID.Valid = HotspotSSID.Valid;
                            SSID.Verified = HotspotSSID.Verified;
                            
                            database.Update(SSID);
                        }
                    }
                }
            }
        }
        #endregion HotspotSSID

        #region HotspotHelper
        public dbHotspotHelper HotspotHelperGet()
        {
            lock (locker)
            {
                var settings = database.Table<dbHotspotHelper>().FirstOrDefault();
                if (settings == null)
                {
                    HotspotHelperSet(new dbHotspotHelper());
                }
                return database.Table<dbHotspotHelper>().FirstOrDefault();
            }
        }
        public bool HotspotHelperSet(dbHotspotHelper helper)
        {
            lock (locker)
            {
                database.DeleteAll<dbHotspotHelper>();
                return database.Insert(helper) > 0;
            }
        }
        public bool HotspotHelperSetAuthenticatedUsername(string Username)
        {
            lock (locker)
            {
                var hotspothelper = HotspotHelperGet();
                hotspothelper.AuthenticatedUsername = Username;
                return database.Update(hotspothelper) > 0;
            }
        }
        public bool HotspotHelperSetSessionID(string SessionID)
        {
            lock (locker)
            {
                var hotspothelper = HotspotHelperGet();
                hotspothelper.SessionID = SessionID;
                hotspothelper.IsAccuris = !string.IsNullOrEmpty(SessionID);
                return database.Update(hotspothelper) > 0;
            }
        }
        #endregion HotspotHelper
    }

    public class dbUser
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; } = 0;
        public string Name { get; set; } = "";
        public string Surname { get; set; } = "";
        public string UserId { get; set; } = "";
        public string AccountStatusID { get; set; } = "";
        public string CountryId { get; set; } = "";
        public string DateCreated { get; set; } = "";
        public string EmailEnc { get; set; } = "";
        public string LoginCredential { get; set; } = "";
        public string MobileNumber { get; set; } = "";
        public string Title { get; set; } = "";
        public bool RememberMe { get; set; } = false;
        public string Password { get; set; } = "";
    }

    public class dbUserSettings
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; } = 0;
        public bool Notifications { get; set; } = true;
        public bool NotificationText { get; set; } = true;
        public bool NotificationSound { get; set; } = true;
        public bool NotificationAvailable { get; set; } = true;
        public bool DisableConnectionManager { get; set; } = false;
        public DateTime LastConnectionNotification { get; set; } = DateTime.Now.AddMinutes(-60);
        public DateTime LastAvailableNotification { get; set; } = DateTime.Now.AddMinutes(-60);
    }

    public class dbPackage
    {
        [PrimaryKey, AutoIncrement]
        public int dbPackageID { get; set; } = 0;
        public int Id { get; set; } = 0;
        public string LoginUserName { get; set; } = "";
        public string LoginPassword { get; set; } = "";
        public string Active { get; set; } = "";
        public string ExpiryDate { get; set; } = "";
        public string CredentialDesc { get; set; } = "";
        public string AccountTypeDesc { get; set; } = "";
        public string GroupDesc { get; set; } = "";
        public string GroupName { get; set; } = "";
        public string MacAddress { get; set; } = "";
        public string ServiceProviderId { get; set; } = "";
        public string CreateDate { get; set; } = "";
        public string PackageID { get; set; } = "";
        public string UsageLeftPercentage { get; set; } = "";
        public string UsageLeftvalue { get; set; } = "";
        public int UseRank { get; set; } = 0;
        public string UsageValue { get; set; } = "";
        public string UsagePercentValue { get; set; } = "";
        public string Unit { get; set; } = "";
        public string GroupUnit { get; set; } = "";
        public string GroupNumber { get; set; } = "";
    }

    public class dbServiceProvider
    {
        [PrimaryKey, AutoIncrement]
        public int dbServiceProviderID { get; set; } = 0;
        public string Id { get; set; } = "";
        public string Name { get; set; } = "";
    }

    public class dbPackageRankingUpdated
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; } = 0;
        public bool Updated { get; set; } = false;
    }

    public class dbHotspotSSID
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; } = 0;
        public string SSID { get; set; } = "";
        public bool Verified { get; set; } = false;
        public bool Valid { get; set; } = false;
    }

    public class dbHotspotHelper
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; } = 0;
        public string SessionID { get; set; } = "";
        public bool IsAccuris { get; set; } = false;
        public string AuthenticatedUsername { get; set; } = "";
    }
}