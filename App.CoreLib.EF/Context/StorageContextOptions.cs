namespace App.CoreLib.EF.Context
{
    public class StorageContextOptions
    {
        public string ConnectionString { get; set; }

        public string MigrationsAssembly { get; set; }
    }
}