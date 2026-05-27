namespace Allycs.Common.Country
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// ICAO国家服务类，提供基于ICAO 24位地址码的国家信息查询功能
    /// </summary>
    /// <remarks>
    /// ICAO 24位地址码是国际民航组织(ICAO)为飞机分配的唯一标识代码，
    /// 由6个十六进制字符组成（共24位二进制）。前三位字符通常代表国家或地区。
    /// 
    /// 例如：英国的ICAO代码前缀为400-405，所以英国注册的飞机ICAO24码以这些前缀开头。
    /// </remarks>
    public static class IcaoCountryService
    {
        private static readonly Dictionary<string, IcaoCountryInfo> _icaoToCountryMap;

        /// <summary>
        /// 静态构造函数，初始化ICAO到国家的映射关系
        /// </summary>
        static IcaoCountryService()
        {
            _icaoToCountryMap = InitializeIcaoMapping();
        }

        /// <summary>
        /// 根据ICAO 24位地址码获取国家信息
        /// </summary>
        /// <param name="icao24">ICAO 24位地址码（6位十六进制字符串，如"400000"）</param>
        /// <returns>对应的国家信息，如果无法识别则返回null</returns>
        /// <example>
        /// <code>
        /// string icao24 = "400000";
        /// IcaoCountryInfo country = IcaoCountryService.GetCountryByIcao24(icao24);
        /// // 返回英国国家信息
        /// </code>
        /// </example>
        public static IcaoCountryInfo GetCountryByIcao24(string icao24)
        {
            if (string.IsNullOrWhiteSpace(icao24))
                return null;

            string key = icao24.ToUpperInvariant();
            
            // 精确匹配
            if (_icaoToCountryMap.TryGetValue(key, out var country))
                return country;

            // 前缀匹配（检查前3位）
            if (key.Length >= 3)
            {
                string prefix = key.Substring(0, 3);
                var matchingCountry = _icaoToCountryMap.Values
                    .FirstOrDefault(c => c.Icao24.StartsWith(prefix));
                if (matchingCountry != null)
                {
                    return new IcaoCountryInfo
                    {
                        Icao24 = key,
                        Mid = matchingCountry.Mid,
                        NameEn = matchingCountry.NameEn,
                        NameZh = matchingCountry.NameZh,
                        IsoCode = matchingCountry.IsoCode
                    };
                }
            }

            return null;
        }

        /// <summary>
        /// 获取所有ICAO到国家的映射关系
        /// </summary>
        /// <returns>包含所有ICAO映射的字典副本</returns>
        public static Dictionary<string, IcaoCountryInfo> GetAllMappings()
        {
            return new Dictionary<string, IcaoCountryInfo>(_icaoToCountryMap);
        }

        /// <summary>
        /// 获取所有国家信息列表
        /// </summary>
        /// <returns>去重后的国家信息列表</returns>
        public static List<IcaoCountryInfo> GetAllCountries()
        {
            var seen = new HashSet<string>();
            var result = new List<IcaoCountryInfo>();
            foreach (var country in _icaoToCountryMap.Values)
            {
                if (seen.Add(country.IsoCode))
                {
                    result.Add(country);
                }
            }
            return result;
        }

        /// <summary>
        /// 根据ISO代码获取国家信息
        /// </summary>
        /// <param name="isoCode">ISO 3166-1 alpha-2 国家代码</param>
        /// <returns>对应的国家信息列表</returns>
        public static List<IcaoCountryInfo> GetCountriesByIsoCode(string isoCode)
        {
            if (string.IsNullOrWhiteSpace(isoCode))
                return new List<IcaoCountryInfo>();

            return _icaoToCountryMap.Values
                .Where(c => string.Equals(c.IsoCode, isoCode, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        /// <summary>
        /// 检查ICAO 24位地址码格式是否有效
        /// </summary>
        /// <param name="icao24">ICAO 24位地址码</param>
        /// <returns>如果格式有效则返回true，否则返回false</returns>
        public static bool IsValidIcao24(string icao24)
        {
            if (string.IsNullOrWhiteSpace(icao24))
                return false;

            if (icao24.Length != 6)
                return false;

            return icao24.All(c => 
                (c >= '0' && c <= '9') || 
                (c >= 'A' && c <= 'F') || 
                (c >= 'a' && c <= 'f'));
        }

        /// <summary>
        /// 初始化ICAO到国家的映射关系
        /// </summary>
        /// <returns>ICAO到国家信息的字典</returns>
        private static Dictionary<string, IcaoCountryInfo> InitializeIcaoMapping()
        {
            var map = new Dictionary<string, IcaoCountryInfo>(StringComparer.OrdinalIgnoreCase);

            // ===== 欧洲国家 =====
            AddCountryRange(map, "400", "405", "United Kingdom", "英国", "GB");
            AddCountryRange(map, "424", "424", "United Kingdom", "英国", "GB");
            AddCountryRange(map, "481", "481", "United Kingdom", "英国", "GB");

            // 其他国家可以继续添加...

            // 示例数据 - 完整数据应包含所有ICAO国家代码
            map["E007"] = new IcaoCountryInfo { Icao24 = "E007", NameEn = "United Kingdom", NameZh = "英国", IsoCode = "GB" };
            map["E008"] = new IcaoCountryInfo { Icao24 = "E008", NameEn = "Denmark", NameZh = "丹麦", IsoCode = "DK" };

            // 添加更多示例数据
            AddCountryRange(map, "390", "39F", "Germany", "德国", "DE");
            AddCountryRange(map, "3C0", "3CF", "France", "法国", "FR");
            AddCountryRange(map, "3B0", "3BF", "Spain", "西班牙", "ES");
            AddCountryRange(map, "3A0", "3AF", "Italy", "意大利", "IT");
            AddCountryRange(map, "4B0", "4BF", "Netherlands", "荷兰", "NL");
            AddCountryRange(map, "500", "50F", "Sweden", "瑞典", "SE");
            AddCountryRange(map, "4D0", "4DF", "Norway", "挪威", "NO");
            AddCountryRange(map, "4E0", "4EF", "Finland", "芬兰", "FI");
            AddCountryRange(map, "450", "45F", "Poland", "波兰", "PL");
            AddCountryRange(map, "530", "53F", "Austria", "奥地利", "AT");
            AddCountryRange(map, "540", "54F", "Switzerland", "瑞士", "CH");
            AddCountryRange(map, "510", "51F", "Czech Republic", "捷克共和国", "CZ");
            AddCountryRange(map, "560", "56F", "Hungary", "匈牙利", "HU");
            AddCountryRange(map, "570", "57F", "Romania", "罗马尼亚", "RO");
            AddCountryRange(map, "580", "58F", "Greece", "希腊", "GR");
            AddCountryRange(map, "620", "62F", "Russia", "俄罗斯联邦", "RU");
            AddCountryRange(map, "600", "60F", "Turkey", "土耳其", "TR");

            // ===== 北美洲 =====
            AddCountryRange(map, "A00", "AFF", "United States", "美国", "US");
            AddCountryRange(map, "C00", "CFF", "Canada", "加拿大", "CA");
            AddCountryRange(map, "E80", "E8F", "Mexico", "墨西哥", "MX");

            // ===== 亚洲 =====
            AddCountryRange(map, "7C0", "7CF", "China", "中国", "CN");
            AddCountryRange(map, "780", "78F", "Japan", "日本", "JP");
            AddCountryRange(map, "7A0", "7AF", "South Korea", "韩国", "KR");
            AddCountryRange(map, "890", "89F", "India", "印度", "IN");
            AddCountryRange(map, "8A0", "8AF", "Singapore", "新加坡", "SG");
            AddCountryRange(map, "8B0", "8BF", "Malaysia", "马来西亚", "MY");
            AddCountryRange(map, "8C0", "8CF", "Indonesia", "印度尼西亚", "ID");
            AddCountryRange(map, "8D0", "8DF", "Thailand", "泰国", "TH");
            AddCountryRange(map, "8E0", "8EF", "Vietnam", "越南", "VN");

            // ===== 大洋洲 =====
            AddCountryRange(map, "940", "94F", "Australia", "澳大利亚", "AU");
            AddCountryRange(map, "900", "90F", "New Zealand", "新西兰", "NZ");

            // ===== 南美洲 =====
            AddCountryRange(map, "E00", "E7F", "Brazil", "巴西", "BR");
            AddCountryRange(map, "F00", "F3F", "Argentina", "阿根廷", "AR");
            AddCountryRange(map, "F40", "F5F", "Chile", "智利", "CL");

            // ===== 非洲 =====
            AddCountryRange(map, "590", "5AF", "South Africa", "南非", "ZA");
            AddCountryRange(map, "5B0", "5CF", "Nigeria", "尼日利亚", "NG");
            AddCountryRange(map, "5D0", "5DF", "Egypt", "埃及", "EG");

            return map;
        }

        /// <summary>
        /// 添加一系列ICAO代码范围到映射表
        /// </summary>
        /// <param name="map">映射字典</param>
        /// <param name="startPrefix">起始前缀（3位十六进制）</param>
        /// <param name="endPrefix">结束前缀（3位十六进制）</param>
        /// <param name="nameEn">英文名称</param>
        /// <param name="nameZh">中文名称</param>
        /// <param name="isoCode">ISO代码</param>
        private static void AddCountryRange(
            Dictionary<string, IcaoCountryInfo> map,
            string startPrefix,
            string endPrefix,
            string nameEn,
            string nameZh,
            string isoCode)
        {
            int start = int.Parse(startPrefix, System.Globalization.NumberStyles.HexNumber);
            int end = int.Parse(endPrefix, System.Globalization.NumberStyles.HexNumber);

            for (int i = start; i <= end; i++)
            {
                string prefix = i.ToString("X3");
                // 添加前缀代表的国家信息（使用前缀作为键）
                map[prefix + "00"] = new IcaoCountryInfo
                {
                    Icao24 = prefix + "00",
                    NameEn = nameEn,
                    NameZh = nameZh,
                    IsoCode = isoCode
                };
            }
        }
    }
}
