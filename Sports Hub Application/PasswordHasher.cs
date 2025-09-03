using System;
using System.Security.Cryptography;
using System.Text;
using System.Data.SqlClient;

namespace Mixed_Gym_Application
{
    public static class PasswordHasher
    {
        public static string HashPassword(string password)
        {
            // Use the same approach as SQL Server's HASHBYTES
            using (var sha256 = SHA256.Create())
            {
                // Use UTF-16 encoding (Unicode) to match SQL Server's default
                byte[] bytes = Encoding.Unicode.GetBytes(password);
                byte[] hash = sha256.ComputeHash(bytes);

                // Convert to hexadecimal string (lowercase)
                return BitConverter.ToString(hash).Replace("-", "").ToLower();
            }
        }

        public static bool VerifyPassword(string password, string hashedPassword)
        {
            try
            {
                string enteredHash = HashPassword(password);
                return enteredHash == hashedPassword;
            }
            catch
            {
                return false;
            }
        }

        // Alternative: Use SQL Server to verify the password (ensures exact match)
        public static bool VerifyPasswordUsingSQL(string username, string password, string connectionString)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT dbo.HashPassword(@Password) AS HashedPassword";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Password", password);

                    try
                    {
                        connection.Open();
                        string sqlHash = command.ExecuteScalar() as string;

                        // Now get the stored hash for this user
                        string getHashQuery = "SELECT PasswordHash FROM CashierDetails WHERE Username = @Username";
                        using (SqlCommand getHashCommand = new SqlCommand(getHashQuery, connection))
                        {
                            getHashCommand.Parameters.AddWithValue("@Username", username);
                            string storedHash = getHashCommand.ExecuteScalar() as string;

                            return sqlHash == storedHash;
                        }
                    }
                    catch
                    {
                        return false;
                    }
                }
            }
        }
    }
}