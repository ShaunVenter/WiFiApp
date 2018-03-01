namespace AlwaysOn_iOS
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
                        var libraryPath = System.IO.Path.Combine(documentsPath, "..", "Library");
                        var path = System.IO.Path.Combine(libraryPath, sqliteFilename);
                        
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
