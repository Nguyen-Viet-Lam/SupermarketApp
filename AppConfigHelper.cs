using System.Configuration;

public static class AppConfigHelper
{
    public static string GetConnectionString(string name = "SupermarketDB")
        => ConfigurationManager.ConnectionStrings[name]?.ConnectionString;
    
    public static void UpdateConnectionString(string name, string connectionString)
    {
        var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
        var connectionStrings = config.ConnectionStrings.ConnectionStrings;
        
        if (connectionStrings[name] != null)
        {
            connectionStrings[name].ConnectionString = connectionString;
        }
        else
        {
            connectionStrings.Add(new ConnectionStringSettings(name, connectionString));
        }
        
        config.Save(ConfigurationSaveMode.Modified);
        ConfigurationManager.RefreshSection("connectionStrings");
    }
}
