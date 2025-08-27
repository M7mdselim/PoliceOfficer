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
        //string configPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MyApp", "config.txt");

        //if (File.Exists(configPath))
        //{
        //    var lines = File.ReadAllLines(configPath);
        //    if (lines.Length >= 3)
        //    {
        //        string serverName = lines[0];
        //        string username = lines[1];
        //        string password = lines[2];
        //     //connectionString = $"Data Source=DESKTOP-64N23O5;Initial Catalog=MixedGymDB;Integrated Security=True;Encrypt=False";

        //     connectionString = $"Data Source={serverName};Initial Catalog=MixedGymDB;User Id={username};Password={password};Encrypt=False";
        //    }
        //    else
        //    {
        //        throw new InvalidOperationException("The configuration file is invalid.");
        //    }
        //}
        //else
        //{
        //    throw new FileNotFoundException("The configuration file was not found.");
        //}


       // connectionString = $"Data Source=Mohammed-selim;Initial Catalog=MixedGymDB;Integrated Security=True;Encrypt=False";
        connectionString = $"Data Source=Desktop-Q5l24on;Initial Catalog=MixedGymDB;Integrated Security=True;Encrypt=False";

        //  connectionString = $"Data Source=192.168.50.5;Initial Catalog=MixedGymDB;User Id=sa;Password=comsys@123;Encrypt=False";
    }
}
