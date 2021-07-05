namespace Catalog.Api.Settings
{
    public class MongoDBSettings
    {
        //These properties filled by .NET on runtime from secrets and appsettings.json
        public string Host { get; set; }
        public int Port { get; set; }

        public string User {get;set;}
        public string Password {get;set;}
        public string ConnectionString {get{
            return $"mongodb://{User}:{Password}@{Host}:{Port}";
        }}
    }
}