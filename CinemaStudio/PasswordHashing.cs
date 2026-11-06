using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CinemaStudio
{
    internal static class PasswordHashing
    {
        private const int Salt = 16;
        private const int Hash = 20;
        private const int Iteration = 10000;

        public static string HashPassword(string password)
        {
            RNGCryptoServiceProvider rng =
                new RNGCryptoServiceProvider();

            byte[] saltBytes = new byte[Salt];

            rng.GetBytes(saltBytes);

            Rfc2898DeriveBytes keys =
                new Rfc2898DeriveBytes(
                    password,
                    saltBytes,
                    Iteration);

            byte[] hashBytes = keys.GetBytes(Hash);

            byte[] pwdHash = new byte[Salt + Hash];

            Array.Copy(saltBytes, 0, pwdHash, 0, Salt);
            Array.Copy(hashBytes, 0, pwdHash, Salt, Hash);

            rng.Dispose();
            keys.Dispose();

            return Convert.ToBase64String(pwdHash);
        }

        public static bool PasswordChecking(
            string password,
            string hashPassword)
        {
            if (string.IsNullOrWhiteSpace(password)
                || string.IsNullOrWhiteSpace(hashPassword))
                return false;

            try
            {
                byte[] pwdHash = Convert.FromBase64String(hashPassword);

                byte[] saltBytes = new byte[Salt];
                Array.Copy(pwdHash, 0, saltBytes, 0, Salt);

                Rfc2898DeriveBytes keys =
                    new Rfc2898DeriveBytes(
                        password,
                        saltBytes,
                        Iteration);

                byte[] hashBytes = keys.GetBytes(Hash);

                bool check = true;
                for (int i = 0; i < Hash; i++)
                {
                    if (pwdHash[i + Salt] != hashBytes[i])
                    {
                        check = false;
                    }
                }

                keys.Dispose();
                return check;
            }
            catch (FormatException)
            {
                return false;
            }
            catch (ArgumentException)
            {
                return false;
            }
            catch (CryptographicException)
            {
                return false;
            }
        }
    }
}
