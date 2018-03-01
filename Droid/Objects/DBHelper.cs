namespace AlwaysOn_Droid
{
    public class DBHelper
    {
        static AlwaysOn.DatabaseHelper database;
        public static AlwaysOn.DatabaseHelper Database
        {
            get
            {
                try
                {
                    if (database == null)
                    {
                        var sqliteFilename = "alwayson.db";
                        var documentsPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);

                        var path = System.IO.Path.Combine(documentsPath, sqliteFilename);
                        
                        var conn = new SQLite.SQLiteConnection(path);

                        conn.CreateTable<AlwaysOn.dbUser>();
                        conn.CreateTable<AlwaysOn.dbUserSettings>();
                        conn.CreateTable<AlwaysOn.dbPackage>();
                        conn.CreateTable<AlwaysOn.dbServiceProvider>();
                        conn.CreateTable<AlwaysOn.dbPackageRankingUpdated>();
                        conn.CreateTable<AlwaysOn.dbHotspotSSID>();
                        conn.CreateTable<AlwaysOn.dbHotspotHelper>();

                        database = new AlwaysOn.DatabaseHelper(conn);
                    }
                    return database;
                }
                catch //(System.Exception ex)
                {
                    return null;
                }
            }
        }
    }
}
