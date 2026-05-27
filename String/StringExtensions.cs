namespace Allycs.Common
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Web;

    /// <summary>
    /// 字符串扩展方法类，提供丰富的字符串处理功能
    /// </summary>
    public static class StringExtensions
    {
        #region 空值检查

        /// <summary>
        /// 判断字符串是否为空字符串或Null
        /// </summary>
        /// <param name="s">要检查的字符串</param>
        /// <returns>如果为空或Null返回true，否则返回false</returns>
        public static bool IsNullOrEmpty(this string s)
        {
            return string.IsNullOrEmpty(s);
        }

        /// <summary>
        /// 判断字符串是否为空字符串、Null或全是空格的字符串
        /// </summary>
        /// <param name="s">要检查的字符串</param>
        /// <returns>如果为空、Null或全空格返回true，否则返回false</returns>
        public static bool IsNullOrWhiteSpace(this string s)
        {
            return string.IsNullOrWhiteSpace(s);
        }

        /// <summary>
        /// 修剪字符串并将空字符串转换为null
        /// </summary>
        /// <param name="value">要修剪的字符串</param>
        /// <returns>修剪后的字符串，如果修剪后为空则返回null</returns>
        public static string TrimToNull(this string value)
        {
            if (value == null)
                return null;

            var trimmed = value.Trim();
            return trimmed.Length == 0 ? null : trimmed;
        }

        #endregion

        #region 字符串过滤

        /// <summary>
        /// 过滤字符串中的非数字字符，只保留0-9的数字
        /// </summary>
        /// <param name="value">源字符串</param>
        /// <returns>仅由0-9的数字组成字符串</returns>
        public static string FilterToNumber(this string value)
        {
            return Regex.Replace(value, "[^0-9]", "", RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// 过滤字符串中的空白字符（空格、制表符、换行符等）
        /// </summary>
        /// <param name="value">源字符串</param>
        /// <returns>不包含空白符号的字符串</returns>
        public static string FilterTrim(this string value)
        {
            return Regex.Replace(value, @"\s", "");
        }

        #endregion

        #region 字符串验证

        /// <summary>
        /// 使用正则表达式验证字符串是否匹配指定模式
        /// </summary>
        /// <param name="input">输入字符串</param>
        /// <param name="pattern">正则表达式模式</param>
        /// <returns>如果匹配返回true，否则返回false。输入为空时返回false</returns>
        public static bool RegexIsMatch(this string input, string pattern)
        {
            if (string.IsNullOrEmpty(input))
                return false;

            return Regex.IsMatch(input, pattern);
        }

        /// <summary>
        /// 验证字符串是否为合法的IPv4格式
        /// </summary>
        /// <param name="value">要验证的字符串</param>
        /// <returns>如果是有效的IPv4地址返回true，否则返回false</returns>
        public static bool IsIPv4(this string value)
        {
            return RegexIsMatch(value,
                @"^(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$");
        }

        /// <summary>
        /// 验证字符串是否为合法的IPv6格式
        /// </summary>
        /// <param name="value">要验证的字符串</param>
        /// <returns>如果是有效的IPv6地址返回true，否则返回false</returns>
        public static bool IsIPv6(this string value)
        {
            if (string.IsNullOrEmpty(value))
                return false;

            return Regex.IsMatch(value, @"^([0-9a-fA-F]{1,4}:){7}[0-9a-fA-F]{1,4}$") ||
                   value == "::" ||
                   Regex.IsMatch(value, @"^(([0-9a-fA-F]{1,4}:){1,6})((:[0-9a-fA-F]{1,4}){1,6})$") ||
                   Regex.IsMatch(value, @"([0-9a-fA-F]{1,4}:){1,7}:$") ||
                   Regex.IsMatch(value, @":(:[0-9a-fA-F]{1,4}){1,7}$");
        }

        /// <summary>
        /// 验证字符串是否为合法的IP格式（IPv4或IPv6）
        /// </summary>
        /// <param name="value">要验证的字符串</param>
        /// <returns>如果是有效的IP地址返回true，否则返回false</returns>
        public static bool IsIP(this string value)
        {
            if (string.IsNullOrEmpty(value))
                return false;

            if (IsIPv4(value))
                return true;

            if (IsIPv6(value))
                return true;

            return Regex.IsMatch(value, @"^(([0-9a-fA-F]{1,4}:){1,5}:(([0-9]+.){3}[0-9]+))$") ||
                   Regex.IsMatch(value, @"::([0-9a-fA-F]{1,4}:){0,5}(([0-9]+.){3}[0-9]+)$") ||
                   Regex.IsMatch(value, @"([0-9a-fA-F]{1,4}:){1,4}(:[0-9a-fA-F]{1,4}){1,4}:(([0-9]+.){3}[0-9]+)$");
        }

        /// <summary>
        /// 验证字符串是否为合法的Email格式
        /// </summary>
        /// <param name="value">要验证的字符串</param>
        /// <returns>如果是有效的邮箱地址返回true，否则返回false</returns>
        public static bool IsEmail(this string value)
        {
            return RegexIsMatch(value,
                @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");
        }

        /// <summary>
        /// 验证字符串是否为非负整数（包含0、正整数）
        /// </summary>
        /// <param name="value">要验证的字符串</param>
        /// <returns>如果是非负整数返回true，否则返回false</returns>
        public static bool IsUnsignedInteger(this string value)
        {
            return RegexIsMatch(value, @"^\+?\d+$");
        }

        /// <summary>
        /// 验证字符串是否为整数（包含正整数、负整数、零）
        /// </summary>
        /// <param name="value">要验证的字符串</param>
        /// <returns>如果是整数返回true，否则返回false</returns>
        public static bool IsInteger(this string value)
        {
            return RegexIsMatch(value, @"^[\+\-]?\d+$");
        }

        /// <summary>
        /// 验证字符串是否为数字（支持整数和小数）
        /// </summary>
        /// <param name="value">要验证的字符串</param>
        /// <returns>如果是数字返回true，否则返回false</returns>
        public static bool IsNumber(this string value)
        {
            return double.TryParse(value, NumberStyles.Any, NumberFormatInfo.InvariantInfo, out _);
        }

        /// <summary>
        /// 验证字符串是否仅由单词字符组成（字母、数字、下划线）
        /// </summary>
        /// <param name="value">要验证的字符串</param>
        /// <returns>如果仅包含单词字符返回true，否则返回false</returns>
        public static bool IsNormalWord(this string value)
        {
            return RegexIsMatch(value, @"^\w+$");
        }

        /// <summary>
        /// 验证字符串是否包含特殊字符（非单词字符）
        /// </summary>
        /// <param name="value">要验证的字符串</param>
        /// <returns>如果包含特殊字符返回true，否则返回false</returns>
        public static bool HasSpecialChar(this string value)
        {
            return RegexIsMatch(value, @"\W");
        }

        /// <summary>
        /// 验证字符串是否为中文字符组成
        /// </summary>
        /// <param name="value">要验证的字符串</param>
        /// <returns>如果全为中文返回true，否则返回false</returns>
        public static bool IsChinese(this string value)
        {
            return RegexIsMatch(value, @"^[\u4e00-\u9fa5]+$");
        }

        /// <summary>
        /// 验证字符串是否包含双字节字符
        /// </summary>
        /// <param name="value">要验证的字符串</param>
        /// <returns>如果包含双字节字符返回true，否则返回false</returns>
        public static bool HasDoubleByteChar(this string value)
        {
            return Regex.IsMatch(value, @"[^\x00-\xff]");
        }

        /// <summary>
        /// 验证字符串是否为日期格式
        /// </summary>
        /// <param name="value">要验证的日期字符串</param>
        /// <returns>如果是有效日期返回true，否则返回false</returns>
        public static bool IsDate(this string value)
        {
            return DateTime.TryParse(value, out _);
        }

        /// <summary>
        /// 验证字符串是否为时间格式
        /// </summary>
        /// <param name="value">要验证的时间字符串（如：15:00:00）</param>
        /// <returns>如果是有效时间返回true，否则返回false</returns>
        public static bool IsTime(this string value)
        {
            return RegexIsMatch(value, @"^((20|21|22|23|[0-1]?\d):[0-5]?\d(:[0-5]?\d)?)$");
        }

        /// <summary>
        /// 验证字符串是否为URL格式
        /// </summary>
        /// <param name="value">要验证的字符串</param>
        /// <returns>如果是有效URL返回true，否则返回false</returns>
        public static bool IsUrl(this string value)
        {
            return Uri.TryCreate(value, UriKind.Absolute, out var uriResult) &&
                   (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
        }

        #endregion

        #region 字符串截取

        /// <summary>
        /// 获取字符串的字节长度（双字节字符长度为2）
        /// </summary>
        /// <param name="value">要度量的字符串</param>
        /// <returns>字节长度</returns>
        public static int GetByteLength(this string value)
        {
            return Encoding.Default.GetByteCount(value);
        }

        /// <summary>
        /// 从字符串左侧截取指定字节长度的子字符串
        /// </summary>
        /// <param name="value">要截取的字符串</param>
        /// <param name="byteLength">要截取的字节长度</param>
        /// <returns>截取后的字符串</returns>
        public static string Left(this string value, int byteLength)
        {
            if (byteLength < 1)
                throw new ArgumentOutOfRangeException(nameof(byteLength), "字节长度必须大于0");

            if (string.IsNullOrEmpty(value))
                return string.Empty;

            var sb = new StringBuilder();
            int currentLength = 0;

            foreach (char c in value)
            {
                int charBytes = c > 127 ? 2 : 1;
                if (currentLength + charBytes > byteLength)
                    break;

                sb.Append(c);
                currentLength += charBytes;
            }

            return sb.ToString();
        }

        /// <summary>
        /// 将字符串按空白字符分割为数组
        /// </summary>
        /// <param name="value">要被拆分的字符串</param>
        /// <returns>拆分后的字符串数组</returns>
        public static string[] SplitByWhitespace(this string value)
        {
            return Regex.Split(value, @"\s+", RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// 将字符串按正则表达式分割为数组
        /// </summary>
        /// <param name="value">要被拆分的字符串</param>
        /// <param name="pattern">分割使用的正则表达式</param>
        /// <returns>拆分后的字符串数组</returns>
        public static string[] SplitByRegex(this string value, string pattern)
        {
            return Regex.Split(value, pattern, RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// 将字符串按固定长度分割为列表
        /// </summary>
        /// <param name="value">要分割的字符串</param>
        /// <param name="chunkLength">每个片段的长度</param>
        /// <returns>分割后的字符串列表</returns>
        public static List<string> SplitByLength(this string value, int chunkLength)
        {
            if (chunkLength < 1)
                throw new ArgumentOutOfRangeException(nameof(chunkLength), "长度必须大于0");

            var result = new List<string>();
            if (string.IsNullOrEmpty(value))
                return result;

            int count = (value.Length + chunkLength - 1) / chunkLength;
            for (int i = 0; i < count; i++)
            {
                int start = i * chunkLength;
                int length = Math.Min(chunkLength, value.Length - start);
                result.Add(value.Substring(start, length));
            }

            return result;
        }

        /// <summary>
        /// 删除字符串的最后一个字符
        /// </summary>
        /// <param name="value">原字符串</param>
        /// <returns>删除最后一个字符后的字符串</returns>
        public static string RemoveLastChar(this string value)
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;

            return value.Substring(0, value.Length - 1);
        }

        /// <summary>
        /// 截取指定字符串之前的部分
        /// </summary>
        /// <param name="value">原字符串</param>
        /// <param name="separator">分隔字符串</param>
        /// <param name="comparisonType">字符串比较方式</param>
        /// <returns>分隔字符串之前的部分，如果不存在则返回原字符串</returns>
        public static string Before(this string value, string separator, StringComparison comparisonType = StringComparison.Ordinal)
        {
            if (string.IsNullOrEmpty(value))
                return value;

            int index = value.IndexOf(separator, comparisonType);
            return index >= 0 ? value.Substring(0, index) : value;
        }

        /// <summary>
        /// 截取指定字符串之后的部分
        /// </summary>
        /// <param name="value">原字符串</param>
        /// <param name="separator">分隔字符串</param>
        /// <param name="comparisonType">字符串比较方式</param>
        /// <returns>分隔字符串之后的部分，如果不存在则返回空字符串</returns>
        public static string After(this string value, string separator, StringComparison comparisonType = StringComparison.Ordinal)
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;

            int index = value.IndexOf(separator, comparisonType);
            return index >= 0 ? value.Substring(index + separator.Length) : string.Empty;
        }

        /// <summary>
        /// 去除字符串开头的指定字符串
        /// </summary>
        /// <param name="value">原字符串</param>
        /// <param name="prefix">要去除的前缀</param>
        /// <param name="comparisonType">字符串比较方式</param>
        /// <returns>去除前缀后的字符串</returns>
        public static string TrimStart(this string value, string prefix, StringComparison comparisonType = StringComparison.Ordinal)
        {
            if (string.IsNullOrEmpty(value) || string.IsNullOrEmpty(prefix))
                return value;

            while (value.StartsWith(prefix, comparisonType))
            {
                value = value.Substring(prefix.Length);
            }

            return value;
        }

        /// <summary>
        /// 去除字符串结尾的指定字符串
        /// </summary>
        /// <param name="value">原字符串</param>
        /// <param name="suffix">要去除的后缀</param>
        /// <param name="comparisonType">字符串比较方式</param>
        /// <returns>去除后缀后的字符串</returns>
        public static string TrimEnd(this string value, string suffix, StringComparison comparisonType = StringComparison.Ordinal)
        {
            if (string.IsNullOrEmpty(value) || string.IsNullOrEmpty(suffix))
                return value;

            while (value.EndsWith(suffix, comparisonType))
            {
                value = value.Substring(0, value.Length - suffix.Length);
            }

            return value;
        }

        #endregion

        #region 字符串格式化

        /// <summary>
        /// 完全反转字符串
        /// </summary>
        /// <param name="value">要被反转的字符串</param>
        /// <returns>反转后的字符串</returns>
        public static string Reverse(this string value)
        {
            if (string.IsNullOrEmpty(value))
                return value;

            char[] chars = value.ToCharArray();
            Array.Reverse(chars);
            return new string(chars);
        }

        /// <summary>
        /// 按单词反转字符串（以空格为分隔符）
        /// </summary>
        /// <param name="value">要被反转的字符串</param>
        /// <returns>单词反转后的字符串</returns>
        public static string ReverseWords(this string value)
        {
            if (string.IsNullOrEmpty(value))
                return value;

            var words = SplitByWhitespace(value);
            Array.Reverse(words);
            return string.Join(" ", words);
        }

        /// <summary>
        /// 将字符串首字母转换为大写
        /// </summary>
        /// <param name="value">要转换的字符串</param>
        /// <returns>首字母大写后的字符串</returns>
        public static string Capitalize(this string value)
        {
            if (string.IsNullOrEmpty(value))
                return value;

            return char.ToUpper(value[0]) + value.Substring(1);
        }

        /// <summary>
        /// 将字符串首字母转换为小写
        /// </summary>
        /// <param name="value">要转换的字符串</param>
        /// <returns>首字母小写后的字符串</returns>
        public static string Uncapitalize(this string value)
        {
            if (string.IsNullOrEmpty(value))
                return value;

            return char.ToLower(value[0]) + value.Substring(1);
        }

        #endregion

        #region 字符串转换

        /// <summary>
        /// 将字符串转换为ASCII码序列
        /// </summary>
        /// <param name="value">要转换的字符串</param>
        /// <returns>ASCII码连接字符串</returns>
        public static string ToASCII(this string value)
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;

            var sb = new StringBuilder(value.Length);
            foreach (char c in value)
            {
                sb.Append((int)c);
            }
            return sb.ToString();
        }

        /// <summary>
        /// 将字符串转换为Unicode转义序列（如：测试 -> \u6d4b\u8bd5）
        /// </summary>
        /// <param name="value">要被转换的字符串</param>
        /// <returns>Unicode转义序列字符串</returns>
        public static string ToUnicodeEscape(this string value)
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;

            var sb = new StringBuilder(value.Length * 6);
            foreach (char c in value)
            {
                sb.Append($@"\u{(int)c:X4}");
            }
            return sb.ToString();
        }

        /// <summary>
        /// 将字符串转换为HTML实体编码
        /// </summary>
        /// <param name="value">要被转换的字符串</param>
        /// <returns>HTML实体编码字符串</returns>
        public static string ToHtmlEntities(this string value)
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;

            var sb = new StringBuilder(value.Length * 6);
            foreach (char c in value)
            {
                sb.Append($"&#{(int)c};");
            }
            return sb.ToString();
        }

        /// <summary>
        /// 将字符串转换为十六进制表示
        /// </summary>
        /// <param name="value">要转换的字符串</param>
        /// <returns>十六进制字符串</returns>
        public static string ToHex(this string value)
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;

            var bytes = Encoding.BigEndianUnicode.GetBytes(value);
            return BitConverter.ToString(bytes).Replace("-", "");
        }

        /// <summary>
        /// 将十六进制字符串转换回普通字符串
        /// </summary>
        /// <param name="hex">十六进制字符串</param>
        /// <returns>普通字符串</returns>
        public static string FromHex(this string hex)
        {
            if (string.IsNullOrEmpty(hex) || hex.Length % 4 != 0)
                return string.Empty;

            var bytes = new byte[hex.Length / 4];
            for (int i = 0; i < bytes.Length; i++)
            {
                bytes[i] = Convert.ToByte(hex.Substring(i * 4, 4), 16);
            }
            return Encoding.BigEndianUnicode.GetString(bytes);
        }

        #endregion

        #region SQL安全

        /// <summary>
        /// 检查字符串是否包含SQL关键字
        /// </summary>
        /// <param name="value">要检查的字符串</param>
        /// <returns>如果包含SQL关键字返回true，否则返回false</returns>
        public static bool ContainsSqlKeyword(this string value)
        {
            if (string.IsNullOrEmpty(value))
                return false;

            string upper = value.ToUpperInvariant();
            return upper.Contains("'") || upper.Contains("\"") ||
                   upper.Contains(";") || upper.Contains("--") ||
                   upper.Contains("EXEC") || upper.Contains("EXECUTE") ||
                   upper.Contains("XP_") || upper.Contains("SP_") ||
                   upper.Contains("0X") || upper.Contains("DELETE") ||
                   upper.Contains("UPDATE") || upper.Contains("INSERT") ||
                   upper.Contains("DROP") || upper.Contains("AND") ||
                   upper.Contains("OR") || upper.Contains("SCRIPT") ||
                   upper.Contains("NET LOCALGROUP ADMINISTRATORS");
        }

        /// <summary>
        /// 过滤SQL危险字符
        /// </summary>
        /// <param name="value">要过滤的字符串</param>
        /// <returns>过滤后的字符串</returns>
        public static string FilterSqlChars(this string value)
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;

            return value.Replace("'", "").Replace("\"", "")
                        .Replace(";", "").Replace("@", "")
                        .Replace("=", "").Replace("+", "")
                        .Replace("*", "").Replace("#", "")
                        .Replace("%", "").Replace("$", "");
        }

        /// <summary>
        /// SQL注入过滤（单引号转义等）
        /// </summary>
        /// <param name="value">要过滤的字符串</param>
        /// <returns>过滤后的安全字符串</returns>
        public static string SqlFilter(this string value)
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;

            var result = value;
            result = result.Replace("'", "''");
            result = result.Replace(";", "；");
            result = result.Replace("(", "（");
            result = result.Replace(")", "）");
            result = Regex.Replace(result, "Exec", "", RegexOptions.IgnoreCase);
            result = Regex.Replace(result, "Execute", "", RegexOptions.IgnoreCase);
            result = Regex.Replace(result, "xp_", "x p_", RegexOptions.IgnoreCase);
            result = Regex.Replace(result, "sp_", "s p_", RegexOptions.IgnoreCase);
            result = Regex.Replace(result, "0x", "0 x", RegexOptions.IgnoreCase);

            return result;
        }

        #endregion

        #region HTML安全

        /// <summary>
        /// 去除HTML标签
        /// </summary>
        /// <param name="html">包含HTML的字符串</param>
        /// <returns>去除HTML标签后的纯文本</returns>
        public static string StripHtml(this string html)
        {
            if (string.IsNullOrEmpty(html))
                return string.Empty;

            var result = html;
            result = Regex.Replace(result, @"<script[^>]*?>.*?</script>", "", RegexOptions.IgnoreCase);
            result = Regex.Replace(result, @"<(.[^>]*)>", "", RegexOptions.IgnoreCase);
            result = Regex.Replace(result, @"([\r\n])[\s]+", "", RegexOptions.IgnoreCase);
            result = Regex.Replace(result, @"&(quot|#34);", "\"", RegexOptions.IgnoreCase);
            result = Regex.Replace(result, @"&(amp|#38);", "&", RegexOptions.IgnoreCase);
            result = Regex.Replace(result, @"&(lt|#60);", "<", RegexOptions.IgnoreCase);
            result = Regex.Replace(result, @"&(gt|#62);", ">", RegexOptions.IgnoreCase);
            result = Regex.Replace(result, @"&(nbsp|#160);", " ", RegexOptions.IgnoreCase);
            result = Regex.Replace(result, @"&#(\d+);", "", RegexOptions.IgnoreCase);
            result = HttpUtility.HtmlEncode(result).Trim();

            return result;
        }

        #endregion
    }
}
