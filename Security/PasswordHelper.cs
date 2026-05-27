namespace Allycs.Common.Security
{
    using System;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Security.Cryptography;
    using System.Text;

    /// <summary>
    /// 密码处理工具类，提供安全的密码哈希、验证和随机字符串生成功能
    /// </summary>
    public static class PasswordHelper
    {
        // 默认的PBKDF2迭代次数（OWASP推荐100000+）
        private const int DefaultIterations = 100000;
        private static readonly char[] HexDigits = "0123456789abcdef".ToCharArray();

        /// <summary>
        /// 生成高强度随机密码（排除易混淆字符如0、O、l、1等）
        /// </summary>
        /// <param name="length">密码长度</param>
        /// <returns>随机生成的密码</returns>
        public static string GeneratePassword(int length)
        {
            if (length < 1)
                throw new ArgumentException("密码长度必须大于0", nameof(length));

            const string validChars = "abcdefghjkmnprstuvwxyACDEFGHJKMNPQRSTUVWXY3456789_-^";
            var password = new char[length];
            
            using (var rng = RandomNumberGenerator.Create())
            {
                var data = new byte[length];
                rng.GetBytes(data);

                for (int i = 0; i < length; i++)
                {
                    password[i] = validChars[data[i] % validChars.Length];
                }
            }

            return new string(password);
        }

        /// <summary>
        /// 生成安全的盐值
        /// </summary>
        /// <param name="size">盐值字节长度（默认16字节）</param>
        /// <returns>Base64编码的盐值字符串</returns>
        public static string GenerateSalt(int size = 16)
        {
            var data = new byte[size];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(data);
            }
            return Convert.ToBase64String(data);
        }

        /// <summary>
        /// 使用SHA256加密密码（带盐值）
        /// </summary>
        /// <param name="password">原始密码</param>
        /// <param name="salt">密码盐值</param>
        /// <returns>编码后的密码哈希值</returns>
        public static string HashPasswordWithSHA256(string password, string salt)
        {
            Guard.NotNullOrEmpty(nameof(password), password);
            Guard.NotNullOrEmpty(nameof(salt), salt);

            var passwordBytes = Encoding.UTF8.GetBytes(password);
            var saltBytes = Convert.FromBase64String(salt);
            var combinedBytes = new byte[saltBytes.Length + passwordBytes.Length];

            Buffer.BlockCopy(saltBytes, 0, combinedBytes, 0, saltBytes.Length);
            Buffer.BlockCopy(passwordBytes, 0, combinedBytes, saltBytes.Length, passwordBytes.Length);

            using (var sha256 = SHA256.Create())
            {
                var hashBytes = sha256.ComputeHash(combinedBytes);
                return Convert.ToBase64String(hashBytes);
            }
        }

        /// <summary>
        /// 使用SHA512加密密码（带盐值）
        /// </summary>
        /// <param name="password">原始密码</param>
        /// <param name="salt">密码盐值</param>
        /// <returns>编码后的密码哈希值</returns>
        public static string HashPasswordWithSHA512(string password, string salt)
        {
            Guard.NotNullOrEmpty(nameof(password), password);
            Guard.NotNullOrEmpty(nameof(salt), salt);

            var passwordBytes = Encoding.UTF8.GetBytes(password);
            var saltBytes = Convert.FromBase64String(salt);
            var combinedBytes = new byte[saltBytes.Length + passwordBytes.Length];

            Buffer.BlockCopy(saltBytes, 0, combinedBytes, 0, saltBytes.Length);
            Buffer.BlockCopy(passwordBytes, 0, combinedBytes, saltBytes.Length, passwordBytes.Length);

            using (var sha512 = SHA512.Create())
            {
                var hashBytes = sha512.ComputeHash(combinedBytes);
                return Convert.ToBase64String(hashBytes);
            }
        }

        /// <summary>
        /// 使用PBKDF2加密密码
        /// </summary>
        /// <param name="password">原始密码</param>
        /// <param name="salt">密码盐值</param>
        /// <param name="iterations">迭代次数（默认100000）</param>
        /// <returns>编码后的密码哈希值</returns>
        public static string HashPasswordWithPBKDF2(string password, string salt, int iterations = -1)
        {
            Guard.NotNullOrEmpty(nameof(password), password);
            Guard.NotNullOrEmpty(nameof(salt), salt);

            if (iterations < 1)
                iterations = DefaultIterations;

            var passwordBytes = Encoding.UTF8.GetBytes(password);
            var saltBytes = Convert.FromBase64String(salt);

            // .NET Standard 2.0兼容版本，使用SHA1（兼容性更好）
            // 在更高版本中可切换为SHA256
            using (var pbkdf2 = new Rfc2898DeriveBytes(passwordBytes, saltBytes, iterations))
            {
                var hashBytes = pbkdf2.GetBytes(32);
                return Convert.ToBase64String(hashBytes);
            }
        }

        /// <summary>
        /// 对密码进行哈希编码
        /// </summary>
        /// <param name="password">原始密码</param>
        /// <param name="format">密码格式类型</param>
        /// <param name="salt">密码盐值</param>
        /// <returns>编码后的密码哈希值</returns>
        public static string HashPassword(string password, PasswordFormatType format, string salt)
        {
            Guard.NotNullOrEmpty(nameof(password), password);
            Guard.NotNull(nameof(salt), salt);

            switch (format)
            {
                case PasswordFormatType.None:
                    return password;

                case PasswordFormatType.SHA1:
                    return HashPasswordLegacy(password, salt, SHA1.Create());

                case PasswordFormatType.MD5:
                    return HashPasswordLegacy(password, salt, MD5.Create());

                case PasswordFormatType.SHA256:
                    return HashPasswordWithSHA256(password, salt);

                case PasswordFormatType.SHA384:
                    return HashPasswordLegacy(password, salt, SHA384.Create());

                case PasswordFormatType.SHA512:
                    return HashPasswordWithSHA512(password, salt);

                case PasswordFormatType.PBKDF2:
                    return HashPasswordWithPBKDF2(password, salt);

                case PasswordFormatType.BCrypt:
                    // BCrypt not supported in this version, use PBKDF2 as fallback
                    return HashPasswordWithPBKDF2(password, salt);

                default:
                    throw new ArgumentException($"不支持的密码格式: {format}", nameof(format));
            }
        }

        private static string HashPasswordLegacy(string password, string salt, HashAlgorithm algorithm)
        {
            var passwordBytes = Encoding.Unicode.GetBytes(password);
            var saltBytes = Convert.FromBase64String(salt);
            var combinedBytes = new byte[saltBytes.Length + passwordBytes.Length];

            Buffer.BlockCopy(saltBytes, 0, combinedBytes, 0, saltBytes.Length);
            Buffer.BlockCopy(passwordBytes, 0, combinedBytes, saltBytes.Length, passwordBytes.Length);

            using (algorithm)
            {
                var hashBytes = algorithm.ComputeHash(combinedBytes);
                return Convert.ToBase64String(hashBytes);
            }
        }

        /// <summary>
        /// 计算文件的MD5哈希值
        /// </summary>
        /// <param name="stream">文件流</param>
        /// <returns>32位小写十六进制MD5字符串</returns>
        public static string ComputeFileMD5(Stream stream)
        {
            Guard.NotNull(nameof(stream), stream);

            using (var md5 = MD5.Create())
            {
                var hashBytes = md5.ComputeHash(stream);
                stream.Position = 0;
                return BytesToHexString(hashBytes);
            }
        }

        /// <summary>
        /// 计算字符串的MD5哈希值
        /// </summary>
        /// <param name="input">输入字符串</param>
        /// <returns>32位小写十六进制MD5字符串</returns>
        public static string ComputeMD5(string input)
        {
            Guard.NotNull(nameof(input), input);

            var inputBytes = Encoding.UTF8.GetBytes(input);
            using (var md5 = MD5.Create())
            {
                var hashBytes = md5.ComputeHash(inputBytes);
                return BytesToHexString(hashBytes);
            }
        }

        /// <summary>
        /// 计算字符串的SHA1哈希值
        /// </summary>
        /// <param name="input">输入字符串</param>
        /// <returns>40位小写十六进制SHA1字符串</returns>
        public static string ComputeSHA1(string input)
        {
            Guard.NotNull(nameof(input), input);

            var inputBytes = Encoding.UTF8.GetBytes(input);
            using (var sha1 = SHA1.Create())
            {
                var hashBytes = sha1.ComputeHash(inputBytes);
                return BytesToHexString(hashBytes);
            }
        }

        /// <summary>
        /// 计算字符串的SHA256哈希值
        /// </summary>
        /// <param name="input">输入字符串</param>
        /// <returns>64位小写十六进制SHA256字符串</returns>
        public static string ComputeSHA256(string input)
        {
            Guard.NotNull(nameof(input), input);

            var inputBytes = Encoding.UTF8.GetBytes(input);
            using (var sha256 = SHA256.Create())
            {
                var hashBytes = sha256.ComputeHash(inputBytes);
                return BytesToHexString(hashBytes);
            }
        }

        /// <summary>
        /// 计算字符串的SHA512哈希值
        /// </summary>
        /// <param name="input">输入字符串</param>
        /// <returns>128位小写十六进制SHA512字符串</returns>
        public static string ComputeSHA512(string input)
        {
            Guard.NotNull(nameof(input), input);

            var inputBytes = Encoding.UTF8.GetBytes(input);
            using (var sha512 = SHA512.Create())
            {
                var hashBytes = sha512.ComputeHash(inputBytes);
                return BytesToHexString(hashBytes);
            }
        }

        /// <summary>
        /// 计算HMAC-SHA256
        /// </summary>
        /// <param name="input">输入字符串</param>
        /// <param name="key">密钥</param>
        /// <returns>64位小写十六进制HMAC-SHA256字符串</returns>
        public static string ComputeHMACSHA256(string input, string key)
        {
            Guard.NotNull(nameof(input), input);
            Guard.NotNull(nameof(key), key);

            var inputBytes = Encoding.UTF8.GetBytes(input);
            var keyBytes = Encoding.UTF8.GetBytes(key);

            using (var hmac = new HMACSHA256(keyBytes))
            {
                var hashBytes = hmac.ComputeHash(inputBytes);
                return BytesToHexString(hashBytes);
            }
        }

        /// <summary>
        /// 计算HMAC-SHA512
        /// </summary>
        /// <param name="input">输入字符串</param>
        /// <param name="key">密钥</param>
        /// <returns>128位小写十六进制HMAC-SHA512字符串</returns>
        public static string ComputeHMACSHA512(string input, string key)
        {
            Guard.NotNull(nameof(input), input);
            Guard.NotNull(nameof(key), key);

            var inputBytes = Encoding.UTF8.GetBytes(input);
            var keyBytes = Encoding.UTF8.GetBytes(key);

            using (var hmac = new HMACSHA512(keyBytes))
            {
                var hashBytes = hmac.ComputeHash(inputBytes);
                return BytesToHexString(hashBytes);
            }
        }

        /// <summary>
        /// 将字节数组转换为十六进制字符串（优化版本）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static string BytesToHexString(byte[] bytes)
        {
            var chars = new char[bytes.Length * 2];
            for (int i = 0; i < bytes.Length; i++)
            {
                int b = bytes[i];
                chars[i * 2] = HexDigits[b >> 4];
                chars[i * 2 + 1] = HexDigits[b & 0x0F];
            }
            return new string(chars);
        }

        /// <summary>
        /// 验证密码是否匹配
        /// </summary>
        /// <param name="password">待验证密码</param>
        /// <param name="hash">已存储的密码Hash</param>
        /// <param name="salt">已存储的盐值</param>
        /// <param name="format">密码格式类型</param>
        /// <returns>如果密码匹配则返回true，否则返回false</returns>
        public static bool VerifyPassword(string password, string hash, string salt, PasswordFormatType format)
        {
            Guard.NotNullOrEmpty(nameof(password), password);
            Guard.NotNullOrEmpty(nameof(hash), hash);
            Guard.NotNull(nameof(salt), salt);

            var computedHash = HashPassword(password, format, salt);
            return string.Equals(computedHash, hash, StringComparison.Ordinal);
        }

        /// <summary>
        /// AES加密（无盐值）
        /// </summary>
        /// <param name="plainText">明文</param>
        /// <param name="key">密钥（32字节）</param>
        /// <param name="iv">初始化向量（16字节）</param>
        /// <returns>Base64编码的加密文本</returns>
        public static string AESEncrypt(string plainText, string key, string iv)
        {
            Guard.NotNullOrEmpty(nameof(plainText), plainText);
            Guard.NotNullOrEmpty(nameof(key), key);
            Guard.NotNullOrEmpty(nameof(iv), iv);

            using (var aes = Aes.Create())
            {
                aes.Key = Convert.FromBase64String(key);
                aes.IV = Convert.FromBase64String(iv);

                var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (var ms = new MemoryStream())
                {
                    using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    using (var sw = new StreamWriter(cs))
                    {
                        sw.Write(plainText);
                    }

                    return Convert.ToBase64String(ms.ToArray());
                }
            }
        }

        /// <summary>
        /// AES解密
        /// </summary>
        /// <param name="cipherText">密文</param>
        /// <param name="key">密钥</param>
        /// <param name="iv">初始化向量</param>
        /// <returns>解密后的明文</returns>
        public static string AESDecrypt(string cipherText, string key, string iv)
        {
            Guard.NotNullOrEmpty(nameof(cipherText), cipherText);
            Guard.NotNullOrEmpty(nameof(key), key);
            Guard.NotNullOrEmpty(nameof(iv), iv);

            using (var aes = Aes.Create())
            {
                aes.Key = Convert.FromBase64String(key);
                aes.IV = Convert.FromBase64String(iv);

                var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (var ms = new MemoryStream(Convert.FromBase64String(cipherText)))
                using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                using (var sr = new StreamReader(cs))
                {
                    return sr.ReadToEnd();
                }
            }
        }

        /// <summary>
        /// 生成AES密钥
        /// </summary>
        /// <returns>Base64编码的密钥（32字节）</returns>
        public static string GenerateAESKey()
        {
            using (var aes = Aes.Create())
            {
                aes.GenerateKey();
                return Convert.ToBase64String(aes.Key);
            }
        }

        /// <summary>
        /// 生成AES IV
        /// </summary>
        /// <returns>Base64编码的IV（16字节）</returns>
        public static string GenerateAESIV()
        {
            using (var aes = Aes.Create())
            {
                aes.GenerateIV();
                return Convert.ToBase64String(aes.IV);
            }
        }
    }
}
