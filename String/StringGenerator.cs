namespace Allycs.Common
{
    using System;
    using System.Text;

    /// <summary>
    /// 字符串生成工具类，提供各种字符串随机生成功能
    /// </summary>
    public static class StringGenerator
    {
        private const string Digits = "0123456789";
        private const string LowercaseLetters = "abcdefghijklmnopqrstuvwxyz";
        private const string UppercaseLetters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private const string SpecialChars = "@$%*?&.!";
        private static readonly Random GlobalRandom = new Random();

        /// <summary>
        /// 生成包含大写字母、小写字母、数字和特殊字符的随机字符串
        /// </summary>
        /// <param name="length">生成字符串的长度</param>
        /// <returns>随机生成的字符串</returns>
        public static string GenerateMixed(int length)
        {
            if (length < 1)
                throw new ArgumentException("长度必须大于0", nameof(length));

            var allChars = Digits + LowercaseLetters + UppercaseLetters + SpecialChars;
            return GenerateFromCharset(length, allChars);
        }

        /// <summary>
        /// 生成仅包含大写字母的随机字符串
        /// </summary>
        /// <param name="length">生成字符串的长度</param>
        /// <returns>随机生成的字符串</returns>
        public static string GenerateUppercase(int length)
        {
            if (length < 1)
                throw new ArgumentException("长度必须大于0", nameof(length));

            return GenerateFromCharset(length, UppercaseLetters);
        }

        /// <summary>
        /// 生成仅包含小写字母的随机字符串
        /// </summary>
        /// <param name="length">生成字符串的长度</param>
        /// <returns>随机生成的字符串</returns>
        public static string GenerateLowercase(int length)
        {
            if (length < 1)
                throw new ArgumentException("长度必须大于0", nameof(length));

            return GenerateFromCharset(length, LowercaseLetters);
        }

        /// <summary>
        /// 生成仅包含数字的随机字符串
        /// </summary>
        /// <param name="length">生成字符串的长度</param>
        /// <returns>随机生成的字符串</returns>
        public static string GenerateDigits(int length)
        {
            if (length < 1)
                throw new ArgumentException("长度必须大于0", nameof(length));

            return GenerateFromCharset(length, Digits);
        }

        /// <summary>
        /// 生成指定字符集范围内的随机字符串
        /// </summary>
        /// <param name="length">生成字符串的长度</param>
        /// <param name="charset">字符集</param>
        /// <returns>随机生成的字符串</returns>
        private static string GenerateFromCharset(int length, string charset)
        {
            var result = new StringBuilder(length);
            lock (GlobalRandom)
            {
                for (int i = 0; i < length; i++)
                {
                    result.Append(charset[GlobalRandom.Next(charset.Length)]);
                }
            }
            return result.ToString();
        }

        /// <summary>
        /// 生成高强度随机密码（排除易混淆字符）
        /// </summary>
        /// <param name="length">密码长度</param>
        /// <returns>随机生成的密码</returns>
        public static string GenerateStrongPassword(int length)
        {
            if (length < 4)
                throw new ArgumentException("密码长度至少为4位", nameof(length));

            const string chars = "abcdefghjkmnprstuvwxyACDEFGHJKMNPQRSTUVWXY34567_-^";
            var password = new StringBuilder(length);
            lock (GlobalRandom)
            {
                for (int i = 0; i < length; i++)
                {
                    password.Append(chars[GlobalRandom.Next(chars.Length)]);
                }
            }
            return password.ToString();
        }

        /// <summary>
        /// 生成GUID字符串（无连字符）
        /// </summary>
        /// <returns>32位的GUID字符串</returns>
        public static string GenerateGuid()
        {
            return Guid.NewGuid().ToString("N");
        }

        /// <summary>
        /// 生成指定数量的随机字节
        /// </summary>
        /// <param name="count">字节数量</param>
        /// <returns>随机字节数组</returns>
        public static byte[] GenerateRandomBytes(int count)
        {
            if (count < 1)
                throw new ArgumentException("数量必须大于0", nameof(count));

            var bytes = new byte[count];
            lock (GlobalRandom)
            {
                GlobalRandom.NextBytes(bytes);
            }
            return bytes;
        }

        /// <summary>
        /// 生成盐值（Base64编码）
        /// </summary>
        /// <returns>Base64编码的盐值字符串</returns>
        public static string GenerateSalt()
        {
            using (var rng = System.Security.Cryptography.RandomNumberGenerator.Create())
            {
                var data = new byte[16];
                rng.GetBytes(data);
                return Convert.ToBase64String(data);
            }
        }
    }
}
