namespace Allycs.Common.Net
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.NetworkInformation;
    using System.Net.Sockets;
    using System.Text.RegularExpressions;

    /// <summary>
    /// 网络工具类，提供IP地址、URL、MAC地址等相关功能
    /// </summary>
    public static class NetHelper
    {
        private static readonly Regex IpV4Regex = new Regex(
            @"^(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$",
            RegexOptions.Compiled);

        private static readonly Regex IpV6Regex = new Regex(
            @"^(?:[0-9a-fA-F]{1,4}:){7}[0-9a-fA-F]{1,4}$|^::$|^::1$|^(?:[0-9a-fA-F]{1,4}:){1,7}:$|^(?:[0-9a-fA-F]{1,4}:){1,6}:[0-9a-fA-F]{1,4}$|^(?:[0-9a-fA-F]{1,4}:){1,5}(?::[0-9a-fA-F]{1,4}){1,2}$|^(?:[0-9a-fA-F]{1,4}:){1,4}(?::[0-9a-fA-F]{1,4}){1,3}$|^(?:[0-9a-fA-F]{1,4}:){1,3}(?::[0-9a-fA-F]{1,4}){1,4}$|^(?:[0-9a-fA-F]{1,4}:){1,2}(?::[0-9a-fA-F]{1,4}){1,5}$|^[0-9a-fA-F]{1,4}:(?::[0-9a-fA-F]{1,4}){1,6}$|^:(?::[0-9a-fA-F]{1,4}){1,7}$",
            RegexOptions.Compiled);

        private static readonly Regex MacAddressRegex = new Regex(
            @"^([0-9A-Fa-f]{2}[:-]){5}([0-9A-Fa-f]{2})$",
            RegexOptions.Compiled);

        /// <summary>
        /// 验证IP地址是否为有效的IPv4地址
        /// </summary>
        /// <param name="ip">要验证的IP地址</param>
        /// <returns>如果是有效的IPv4地址返回true</returns>
        public static bool IsValidIPv4(string ip)
        {
            if (string.IsNullOrWhiteSpace(ip))
                return false;

            return IpV4Regex.IsMatch(ip);
        }

        /// <summary>
        /// 验证IP地址是否为有效的IPv6地址
        /// </summary>
        /// <param name="ip">要验证的IP地址</param>
        /// <returns>如果是有效的IPv6地址返回true</returns>
        public static bool IsValidIPv6(string ip)
        {
            if (string.IsNullOrWhiteSpace(ip))
                return false;

            return IpV6Regex.IsMatch(ip);
        }

        /// <summary>
        /// 验证IP地址是否为有效的IPv4或IPv6地址
        /// </summary>
        /// <param name="ip">要验证的IP地址</param>
        /// <returns>如果是有效的IP地址返回true</returns>
        public static bool IsValidIP(string ip)
        {
            return IsValidIPv4(ip) || IsValidIPv6(ip);
        }

        /// <summary>
        /// 检查IP地址是否在指定的CIDR范围内
        /// </summary>
        /// <param name="ip">要检查的IP地址</param>
        /// <param name="cidr">CIDR格式的网络（如：192.168.1.0/24）</param>
        /// <returns>如果IP在范围内返回true</returns>
        public static bool IsIPInRange(string ip, string cidr)
        {
            if (!IsValidIP(ip) || string.IsNullOrWhiteSpace(cidr))
                return false;

            try
            {
                var parts = cidr.Split('/');
                if (parts.Length != 2)
                    return false;

                var networkAddress = IPAddress.Parse(parts[0]);
                var prefixLength = int.Parse(parts[1]);
                var ipAddress = IPAddress.Parse(ip);

                if (networkAddress.AddressFamily != ipAddress.AddressFamily)
                    return false;

                var networkBytes = networkAddress.GetAddressBytes();
                var ipBytes = ipAddress.GetAddressBytes();

                var maskBytes = GetNetworkMaskBytes(networkBytes.Length, prefixLength);

                for (int i = 0; i < networkBytes.Length; i++)
                {
                    if ((networkBytes[i] & maskBytes[i]) != (ipBytes[i] & maskBytes[i]))
                        return false;
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        private static byte[] GetNetworkMaskBytes(int addressLength, int prefixLength)
        {
            var maskBytes = new byte[addressLength];
            var bitsRemaining = prefixLength;

            for (int i = 0; i < maskBytes.Length && bitsRemaining > 0; i++)
            {
                var bitsToSet = Math.Min(bitsRemaining, 8);
                maskBytes[i] = (byte)(0xFF << (8 - bitsToSet));
                bitsRemaining -= bitsToSet;
            }

            return maskBytes;
        }

        /// <summary>
        /// 检查IP地址是否为内网地址
        /// </summary>
        /// <param name="ip">要检查的IP地址</param>
        /// <returns>如果是内网地址返回true</returns>
        public static bool IsPrivateIP(string ip)
        {
            if (!IsValidIPv4(ip))
                return false;

            var bytes = IPAddress.Parse(ip).GetAddressBytes();

            // 10.0.0.0/8
            if (bytes[0] == 10)
                return true;

            // 172.16.0.0/12
            if (bytes[0] == 172 && bytes[1] >= 16 && bytes[1] <= 31)
                return true;

            // 192.168.0.0/16
            if (bytes[0] == 192 && bytes[1] == 168)
                return true;

            // 127.0.0.0/8 (localhost)
            if (bytes[0] == 127)
                return true;

            return false;
        }

        /// <summary>
        /// 验证MAC地址格式是否正确
        /// </summary>
        /// <param name="mac">要验证的MAC地址</param>
        /// <returns>如果MAC地址格式正确返回true</returns>
        public static bool IsValidMACAddress(string mac)
        {
            if (string.IsNullOrWhiteSpace(mac))
                return false;

            return MacAddressRegex.IsMatch(mac);
        }

        /// <summary>
        /// 格式化MAC地址为标准格式（统一使用冒号分隔）
        /// </summary>
        /// <param name="mac">MAC地址</param>
        /// <returns>格式化后的MAC地址</returns>
        public static string FormatMACAddress(string mac)
        {
            if (string.IsNullOrWhiteSpace(mac))
                return mac;

            // 移除所有非十六进制字符
            var cleanMac = Regex.Replace(mac, "[^0-9A-Fa-f]", "");

            if (cleanMac.Length != 12)
                return mac;

            // 插入冒号
            return string.Join(":",
                cleanMac.Select((c, i) => new { c, i })
                    .GroupBy(x => x.i / 2)
                    .Select(g => new string(g.Select(x => x.c).ToArray())));
        }

        /// <summary>
        /// 获取本机所有网卡的IP地址
        /// </summary>
        /// <returns>IP地址列表</returns>
        public static List<string> GetLocalIPAddresses()
        {
            var addresses = new List<string>();

            try
            {
                foreach (var ni in NetworkInterface.GetAllNetworkInterfaces())
                {
                    if (ni.OperationalStatus == OperationalStatus.Up &&
                        (ni.NetworkInterfaceType == NetworkInterfaceType.Ethernet ||
                         ni.NetworkInterfaceType == NetworkInterfaceType.Wireless80211))
                    {
                        foreach (var ip in ni.GetIPProperties().UnicastAddresses)
                        {
                            if (ip.Address.AddressFamily == AddressFamily.InterNetwork ||
                                ip.Address.AddressFamily == AddressFamily.InterNetworkV6)
                            {
                                addresses.Add(ip.Address.ToString());
                            }
                        }
                    }
                }
            }
            catch
            {
                // 出错时返回空列表
            }

            return addresses;
        }

        /// <summary>
        /// 获取本机所有MAC地址
        /// </summary>
        /// <returns>MAC地址列表</returns>
        public static List<string> GetLocalMACAddresses()
        {
            var addresses = new List<string>();

            try
            {
                foreach (var ni in NetworkInterface.GetAllNetworkInterfaces())
                {
                    if (ni.OperationalStatus == OperationalStatus.Up &&
                        (ni.NetworkInterfaceType == NetworkInterfaceType.Ethernet ||
                         ni.NetworkInterfaceType == NetworkInterfaceType.Wireless80211))
                    {
                        var mac = BitConverter.ToString(ni.GetPhysicalAddress().GetAddressBytes());
                        addresses.Add(mac.Replace("-", ":"));
                    }
                }
            }
            catch
            {
                // 出错时返回空列表
            }

            return addresses;
        }

        /// <summary>
        /// 验证URL格式是否正确
        /// </summary>
        /// <param name="url">要验证的URL</param>
        /// <returns>如果URL格式正确返回true</returns>
        public static bool IsValidUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return false;

            return Uri.TryCreate(url, UriKind.Absolute, out var uriResult) &&
                   (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
        }

        /// <summary>
        /// 安全地构建URL，处理查询参数
        /// </summary>
        /// <param name="baseUrl">基础URL</param>
        /// <param name="parameters">查询参数字典</param>
        /// <returns>构建好的完整URL</returns>
        public static string BuildUrl(string baseUrl, Dictionary<string, string> parameters)
        {
            if (string.IsNullOrWhiteSpace(baseUrl))
                throw new ArgumentException("基础URL不能为空", nameof(baseUrl));

            var uriBuilder = new UriBuilder(baseUrl);

            if (parameters != null && parameters.Count > 0)
            {
                var query = new List<string>();
                foreach (var param in parameters)
                {
                    query.Add($"{Uri.EscapeDataString(param.Key)}={Uri.EscapeDataString(param.Value)}");
                }
                uriBuilder.Query = string.Join("&", query);
            }

            return uriBuilder.ToString();
        }

        /// <summary>
        /// 解析URL中的查询参数
        /// </summary>
        /// <param name="url">URL</param>
        /// <returns>查询参数字典</returns>
        public static Dictionary<string, string> ParseQueryString(string url)
        {
            var parameters = new Dictionary<string, string>();

            if (string.IsNullOrWhiteSpace(url))
                return parameters;

            try
            {
                var uri = new Uri(url);
                if (!string.IsNullOrWhiteSpace(uri.Query))
                {
                    var queryString = uri.Query.TrimStart('?');
                    var pairs = queryString.Split('&');

                    foreach (var pair in pairs)
                    {
                        var parts = pair.Split('=');
                        if (parts.Length == 2)
                        {
                            var key = Uri.UnescapeDataString(parts[0]);
                            var value = Uri.UnescapeDataString(parts[1]);
                            parameters[key] = value;
                        }
                    }
                }
            }
            catch
            {
                // 解析失败返回空字典
            }

            return parameters;
        }

        /// <summary>
        /// 检查端口是否在有效范围内
        /// </summary>
        /// <param name="port">端口号</param>
        /// <returns>如果端口有效返回true</returns>
        public static bool IsValidPort(int port)
        {
            return port >= 0 && port <= 65535;
        }

        /// <summary>
        /// 检查端口是否为知名端口（0-1023）
        /// </summary>
        /// <param name="port">端口号</param>
        /// <returns>如果是知名端口返回true</returns>
        public static bool IsWellKnownPort(int port)
        {
            return port >= 0 && port <= 1023;
        }

        /// <summary>
        /// 检查端口是否为注册端口（1024-49151）
        /// </summary>
        /// <param name="port">端口号</param>
        /// <returns>如果是注册端口返回true</returns>
        public static bool IsRegisteredPort(int port)
        {
            return port >= 1024 && port <= 49151;
        }

        /// <summary>
        /// 检查端口是否为动态/私有端口（49152-65535）
        /// </summary>
        /// <param name="port">端口号</param>
        /// <returns>如果是动态/私有端口返回true</returns>
        public static bool IsDynamicPort(int port)
        {
            return port >= 49152 && port <= 65535;
        }
    }
}
