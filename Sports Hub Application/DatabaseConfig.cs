using System;
using System.IO;

public static class DatabaseConfig
{
    public static string connectionString { get; private set; }

    static DatabaseConfig()
    {
        LoadSqlConfiguration();
    }

    private static void LoadSqlConfiguration()
    {
        string configPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MyApp", "config.txt");

        if (!File.Exists(configPath))
            throw new FileNotFoundException("The configuration file was not found.");

        var lines = File.ReadAllLines(configPath);

        if (lines.Length < 3)
            throw new InvalidOperationException("The configuration file is invalid.");

        string serverName = lines[0];
        string modeOrUsername = lines[1]; // Can be "WindowsAuth" or a username
        string passwordOrConnection = lines[2]; // Password or full connection string (for WindowsAuth)

        if (modeOrUsername.Equals("WindowsAuth", StringComparison.OrdinalIgnoreCase))
        {
            // Windows Authentication
            connectionString = $"Data Source={serverName};Initial Catalog=PoliceOfficerDB;Integrated Security=True;Encrypt=False";
        }
        else
        {
            // SQL Server Authentication
            string username = modeOrUsername;
            string password = passwordOrConnection;

            connectionString = $"Data Source={serverName};Initial Catalog=PoliceOfficerDB;User Id={username};Password={password};Encrypt=False";
        }
    }
}
