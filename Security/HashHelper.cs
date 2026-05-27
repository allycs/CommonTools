namespace Allycs.Common
{
    using System;
    using System.IO;
    using System.Security.Cryptography;
    using System.Text;
    using Allycs.Common.Security;

    /// <summary>
    /// 哈希生成工具类（已过时，请使用PasswordHelper）
    /// </summary>
    [Obsolete("HashHelper已过时，请使用PasswordHelper替代")]
    public static class HashHelper
    {
        /// <summary>
        /// 生成高强度随机密码（排除易混淆字符如0、O、l、1等）
        /// </summary>
        /// <param name="length">密码长度</param>
        /// <returns>随机生成的密码</returns>
        public static string GeneratePassword(int length)
        {
            return PasswordHelper.GeneratePassword(length);
        }

        /// <summary>
        /// 生成安全的盐值
        /// </summary>
        /// <returns>Base64编码的盐值字符串</returns>
        public static string GenerateSalt()
        {
            return PasswordHelper.GenerateSalt();
        }

        /// <summary>
        /// 对密码进行哈希编码
        /// </summary>
        /// <param name="password">原始密码</param>
        /// <param name="format">密码格式类型</param>
        /// <param name="salt">密码盐值</param>
        /// <returns>编码后的密码哈希值</returns>
        public static string EncodePassword(string password, PasswordFormatType format, string salt)
        {
            return PasswordHelper.HashPassword(password, format, salt);
        }

        /// <summary>
        /// 计算文件的MD5哈希值
        /// </summary>
        /// <param name="stream">文件流</param>
        /// <returns>32位小写十六进制MD5字符串</returns>
        public static string ComputeFileMD5(Stream stream)
        {
            return PasswordHelper.ComputeFileMD5(stream);
        }

        /// <summary>
        /// 计算字符串的MD5哈希值
        /// </summary>
        /// <param name="input">输入字符串</param>
        /// <returns>32位小写十六进制MD5字符串</returns>
        public static string ComputeStringMD5(string input)
        {
            return PasswordHelper.ComputeMD5(input);
        }

        /// <summary>
        /// 计算字符串的SHA1哈希值
        /// </summary>
        /// <param name="input">输入字符串</param>
        /// <returns>40位小写十六进制SHA1字符串</returns>
        public static string ComputeStringSHA1(string input)
        {
            return PasswordHelper.ComputeSHA1(input);
        }

        /// <summary>
        /// 计算字符串的SHA256哈希值
        /// </summary>
        /// <param name="input">输入字符串</param>
        /// <returns>64位小写十六进制SHA256字符串</returns>
        public static string ComputeStringSHA256(string input)
        {
            return PasswordHelper.ComputeSHA256(input);
        }
    }
}
