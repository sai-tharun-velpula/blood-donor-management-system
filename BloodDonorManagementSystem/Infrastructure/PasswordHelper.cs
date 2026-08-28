using System;
using System.Security.Cryptography;
using System.Text;

namespace BloodDonorManagementSystem.Infrastructure
{
    public static class PasswordHelper
    {
        // =========================================================
        // PASSWORD SECURITY SETTINGS
        // =========================================================

        private const int SaltSize = 32;
        private const int HashSize = 32;

        // PBKDF2-SHA256
        private const int Iterations = 120000;

        // =========================================================
        // GENERATE SALT
        // =========================================================

        public static string GenerateSalt()
        {
            byte[] salt = new byte[SaltSize];

            using (RandomNumberGenerator random =
                   RandomNumberGenerator.Create())
            {
                random.GetBytes(salt);
            }

            return Convert.ToBase64String(salt);
        }

        // =========================================================
        // GENERATE TEMPORARY PASSWORD
        // =========================================================

        public static string GenerateTemporaryPassword(
            int length = 12)
        {
            const string characters =
                "ABCDEFGHJKLMNPQRSTUVWXYZ" +
                "abcdefghijkmnopqrstuvwxyz" +
                "23456789" +
                "@#$%";

            if (length < 8)
            {
                length = 8;
            }

            StringBuilder result =
                new StringBuilder(length);

            byte[] randomBytes =
                new byte[length];

            using (RandomNumberGenerator random =
                   RandomNumberGenerator.Create())
            {
                random.GetBytes(randomBytes);
            }

            for (int i = 0; i < length; i++)
            {
                int index =
                    randomBytes[i] % characters.Length;

                result.Append(characters[index]);
            }

            return result.ToString();
        }

        // =========================================================
        // HASH PASSWORD
        // =========================================================

        public static string Hash(
            string password,
            string salt)
        {
            if (string.IsNullOrEmpty(password))
            {
                throw new ArgumentException(
                    "Password is required.",
                    nameof(password));
            }

            if (string.IsNullOrEmpty(salt))
            {
                throw new ArgumentException(
                    "Salt is required.",
                    nameof(salt));
            }

            byte[] saltBytes =
                Convert.FromBase64String(salt);

            using (Rfc2898DeriveBytes pbkdf2 =
                   new Rfc2898DeriveBytes(
                       password,
                       saltBytes,
                       Iterations,
                       HashAlgorithmName.SHA256))
            {
                byte[] hash =
                    pbkdf2.GetBytes(HashSize);

                return Convert.ToBase64String(hash);
            }
        }

        // =========================================================
        // VERIFY PASSWORD
        // =========================================================

        public static bool Verify(
            string password,
            string salt,
            string expectedHash)
        {
            if (string.IsNullOrEmpty(password))
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(expectedHash))
            {
                return false;
            }

            // -----------------------------------------------------
            // EXISTING / LEGACY PASSWORD
            // -----------------------------------------------------

            if (IsLegacyHash(expectedHash))
            {
                return VerifyLegacySha256(
                    password,
                    salt,
                    expectedHash);
            }

            // -----------------------------------------------------
            // NEW PBKDF2 PASSWORD
            // -----------------------------------------------------

            if (string.IsNullOrWhiteSpace(salt))
            {
                return false;
            }

            try
            {
                byte[] saltBytes =
                    Convert.FromBase64String(salt);

                byte[] expectedBytes =
                    Convert.FromBase64String(expectedHash);

                // PBKDF2 hashes created by this helper
                // must contain exactly 32 bytes.
                if (expectedBytes.Length != HashSize)
                {
                    return false;
                }

                using (Rfc2898DeriveBytes pbkdf2 =
                       new Rfc2898DeriveBytes(
                           password,
                           saltBytes,
                           Iterations,
                           HashAlgorithmName.SHA256))
                {
                    byte[] actualHash =
                        pbkdf2.GetBytes(HashSize);

                    return FixedTimeEquals(
                        actualHash,
                        expectedBytes);
                }
            }
            catch
            {
                return false;
            }
        }

        // =========================================================
        // DETECT LEGACY SHA-256 HASH
        // =========================================================

        public static bool IsLegacyHash(
            string hash)
        {
            if (string.IsNullOrWhiteSpace(hash))
            {
                return false;
            }

            hash = hash.Trim();

            // SHA-256 hexadecimal string = 64 characters
            if (hash.Length != 64)
            {
                return false;
            }

            foreach (char character in hash)
            {
                bool hexadecimal =
                    (character >= '0' &&
                     character <= '9') ||
                    (character >= 'a' &&
                     character <= 'f') ||
                    (character >= 'A' &&
                     character <= 'F');

                if (!hexadecimal)
                {
                    return false;
                }
            }

            return true;
        }

        // =========================================================
        // VERIFY LEGACY SHA-256
        // =========================================================

        private static bool VerifyLegacySha256(
            string password,
            string salt,
            string expectedHash)
        {
            try
            {
                using (SHA256 sha =
                       SHA256.Create())
                {
                    string combined =
                        (password ?? string.Empty) +
                        (salt ?? string.Empty);

                    byte[] bytes =
                        Encoding.UTF8.GetBytes(
                            combined);

                    byte[] hash =
                        sha.ComputeHash(bytes);

                    StringBuilder result =
                        new StringBuilder(64);

                    foreach (byte value in hash)
                    {
                        result.Append(
                            value.ToString("x2"));
                    }

                    return string.Equals(
                        result.ToString(),
                        expectedHash.Trim(),
                        StringComparison.OrdinalIgnoreCase);
                }
            }
            catch
            {
                return false;
            }
        }

        // =========================================================
        // CONSTANT-TIME COMPARISON
        // =========================================================

        private static bool FixedTimeEquals(
            byte[] first,
            byte[] second)
        {
            if (first == null ||
                second == null ||
                first.Length != second.Length)
            {
                return false;
            }

            int difference = 0;

            for (int i = 0; i < first.Length; i++)
            {
                difference |=
                    first[i] ^ second[i];
            }

            return difference == 0;
        }
    }
}































/*using System;
using System.Security.Cryptography;
using System.Text;

namespace BloodDonorManagementSystem.Infrastructure
{
    public static class PasswordHelper
    {
        // =========================================================
        // PASSWORD SECURITY SETTINGS
        // =========================================================

        private const int SaltSize = 32;
        private const int HashSize = 32;

        // PBKDF2 iteration count.
        // Suitable for a .NET Framework 4.8 application.
        private const int Iterations = 120000;

        // =========================================================
        // GENERATE SALT
        // =========================================================

        public static string GenerateSalt()
        {
            byte[] salt = new byte[SaltSize];

            using (RandomNumberGenerator random =
                   RandomNumberGenerator.Create())
            {
                random.GetBytes(salt);
            }

            return Convert.ToBase64String(salt);
        }

        // =========================================================
        // GENERATE TEMPORARY PASSWORD
        // =========================================================

        public static string GenerateTemporaryPassword(
            int length = 12)
        {
            const string characters =
                "ABCDEFGHJKLMNPQRSTUVWXYZ" +
                "abcdefghijkmnopqrstuvwxyz" +
                "23456789" +
                "@#$%";

            if (length < 8)
            {
                length = 8;
            }

            StringBuilder result =
                new StringBuilder(length);

            byte[] randomBytes =
                new byte[length];

            using (RandomNumberGenerator random =
                   RandomNumberGenerator.Create())
            {
                random.GetBytes(randomBytes);
            }

            for (int i = 0; i < length; i++)
            {
                int index =
                    randomBytes[i] % characters.Length;

                result.Append(
                    characters[index]);
            }

            return result.ToString();
        }

        // =========================================================
        // HASH PASSWORD
        // =========================================================

        public static string Hash(
            string password,
            string salt)
        {
            if (string.IsNullOrEmpty(password))
            {
                throw new ArgumentException(
                    "Password is required.",
                    nameof(password));
            }

            if (string.IsNullOrEmpty(salt))
            {
                throw new ArgumentException(
                    "Salt is required.",
                    nameof(salt));
            }

            byte[] saltBytes =
                Convert.FromBase64String(salt);

            using (Rfc2898DeriveBytes pbkdf2 =
                   new Rfc2898DeriveBytes(
                       password,
                       saltBytes,
                       Iterations,
                       HashAlgorithmName.SHA256))
            {
                byte[] hash =
                    pbkdf2.GetBytes(HashSize);

                return Convert.ToBase64String(hash);
            }
        }

        // =========================================================
        // VERIFY PASSWORD
        // =========================================================

        public static bool Verify(
            string password,
            string salt,
            string expectedHash)
        {
            if (string.IsNullOrEmpty(password) ||
            string.IsNullOrEmpty(expectedHash))
            {
                return false;
            }

            // -----------------------------------------------------
            // LEGACY SHA-256 HASH
            // -----------------------------------------------------

            if (IsLegacyHash(expectedHash))
            {
                return VerifyLegacySha256(
                    password,
                    salt,
                    expectedHash);
            }

            if (string.IsNullOrEmpty(salt))
            {
                return false;
            }

            // -----------------------------------------------------
            // NEW PBKDF2 HASH
            // -----------------------------------------------------

            try
            {
                byte[] saltBytes =
                    Convert.FromBase64String(salt);

                using (Rfc2898DeriveBytes pbkdf2 =
                       new Rfc2898DeriveBytes(
                           password,
                           saltBytes,
                           Iterations,
                           HashAlgorithmName.SHA256))
                {
                    byte[] actualHash =
                        pbkdf2.GetBytes(HashSize);

                    byte[] expectedBytes =
                        Convert.FromBase64String(
                            expectedHash);

                    return FixedTimeEquals(
                        actualHash,
                        expectedBytes);
                }
            }
            catch
            {
                // -------------------------------------------------
                // FALL BACK TO LEGACY SHA-256
                // -------------------------------------------------

                *//*return VerifyLegacySha256(
                    password,
                    salt,
                    expectedHash);*//*
                    return false;
            }
        }

        // =========================================================
        // DETECT LEGACY HASH
        // =========================================================

        public static bool IsLegacyHash(
            string hash)
        {
            if (string.IsNullOrWhiteSpace(hash))
            {
                return false;
            }

            if (hash.Length != 64)
            {
                return false;
            }

            foreach (char character in hash)
            {
                bool hexadecimal =
                    (character >= '0' &&
                     character <= '9') ||
                    (character >= 'a' &&
                     character <= 'f') ||
                    (character >= 'A' &&
                     character <= 'F');

                if (!hexadecimal)
                {
                    return false;
                }
            }

            return true;
        }

        // =========================================================
        // VERIFY LEGACY SHA-256
        // =========================================================

        private static bool VerifyLegacySha256(
            string password,
            string salt,
            string expectedHash)
        {
            try
            {
                using (SHA256 sha =
                       SHA256.Create())
                {
                    string combined =
                        (password ?? string.Empty) +
                        (salt ?? string.Empty);

                    byte[] bytes =
                        Encoding.UTF8.GetBytes(
                            combined);

                    byte[] hash =
                        sha.ComputeHash(bytes);

                    StringBuilder result =
                        new StringBuilder();

                    foreach (byte value in hash)
                    {
                        result.Append(
                            value.ToString("x2"));
                    }

                    return string.Equals(
                        result.ToString(),
                        expectedHash,
                        StringComparison.OrdinalIgnoreCase);
                }
            }
            catch
            {
                return false;
            }
        }

        // =========================================================
        // CONSTANT-TIME COMPARISON
        // =========================================================

        private static bool FixedTimeEquals(
            byte[] first,
            byte[] second)
        {
            if (first == null ||
                second == null ||
                first.Length != second.Length)
            {
                return false;
            }

            int difference = 0;

            for (int i = 0; i < first.Length; i++)
            {
                difference |=
                    first[i] ^ second[i];
            }

            return difference == 0;
        }
    }
}*/