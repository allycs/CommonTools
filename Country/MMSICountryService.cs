namespace Allycs.Common.Country
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// MMSI国家服务类，提供基于MMSI和MID代码的国家信息查询功能
    /// </summary>
    /// <remarks>
    /// MID (Maritime Identification Digits) 是国际海事组织(IMO)分配给各国的三位数字代码，
    /// 用于船舶识别号(MMSI)的前三位。
    /// 
    /// MMSI编码规则：
    /// - 前三位：MID国家代码
    /// - 剩余部分：船舶识别码
    /// 
    /// 例如：中国的MID代码为412-414，所以中国船舶的MMSI以412、413或414开头。
    /// </remarks>
    public static class MMSICountryService
    {
        private static readonly Dictionary<int, CountryInfo> _midToCountryMap;

        /// <summary>
        /// 静态构造函数，初始化MID到国家的映射关系
        /// </summary>
        static MMSICountryService()
        {
            _midToCountryMap = InitializeMidMapping();
        }

        /// <summary>
        /// 根据MMSI号码获取国家信息
        /// </summary>
        /// <param name="mmsi">船舶MMSI号码（9-15位数字字符串）</param>
        /// <returns>对应的国家信息，如果无法识别则返回null</returns>
        /// <example>
        /// <code>
        /// string mmsi = "412123456";
        /// CountryInfo country = MMSICountryService.GetCountryByMmsi(mmsi);
        /// // 返回中国国家信息
        /// </code>
        /// </example>
        public static CountryInfo GetCountryByMmsi(string mmsi)
        {
            if (string.IsNullOrWhiteSpace(mmsi) || mmsi.Length < 3)
                return null;

            if (!int.TryParse(mmsi.Substring(0, 3), out int mid))
                return null;

            return GetCountryByMid(mid);
        }

        /// <summary>
        /// 根据MID代码获取国家信息
        /// </summary>
        /// <param name="mid">MID国家代码（三位数字）</param>
        /// <returns>对应的国家信息，如果无法识别则返回null</returns>
        /// <example>
        /// <code>
        /// int mid = 412;
        /// CountryInfo country = MMSICountryService.GetCountryByMid(mid);
        /// // 返回中国国家信息
        /// </code>
        /// </example>
        public static CountryInfo GetCountryByMid(int mid)
        {
            return _midToCountryMap.TryGetValue(mid, out CountryInfo country) ? country : null;
        }

        /// <summary>
        /// 获取所有MID到国家的映射关系
        /// </summary>
        /// <returns>包含所有MID映射的字典副本</returns>
        public static Dictionary<int, CountryInfo> GetAllMappings()
        {
            return new Dictionary<int, CountryInfo>(_midToCountryMap);
        }

        /// <summary>
        /// 获取所有国家信息列表
        /// </summary>
        /// <returns>去重后的国家信息列表</returns>
        public static List<CountryInfo> GetAllCountries()
        {
            var seen = new HashSet<string>();
            var result = new List<CountryInfo>();
            foreach (var country in _midToCountryMap.Values)
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
        /// <returns>对应的国家信息列表（可能有多个MID对应同一国家）</returns>
        public static List<CountryInfo> GetCountriesByIsoCode(string isoCode)
        {
            if (string.IsNullOrWhiteSpace(isoCode))
                return new List<CountryInfo>();

            return _midToCountryMap.Values
                .Where(c => string.Equals(c.IsoCode, isoCode, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        /// <summary>
        /// 检查MID代码是否有效
        /// </summary>
        /// <param name="mid">MID代码</param>
        /// <returns>如果MID存在则返回true，否则返回false</returns>
        public static bool IsValidMid(int mid)
        {
            return _midToCountryMap.ContainsKey(mid);
        }

        /// <summary>
        /// 检查MMSI号码格式是否有效
        /// </summary>
        /// <param name="mmsi">MMSI号码</param>
        /// <returns>如果格式有效则返回true，否则返回false</returns>
        public static bool IsValidMmsi(string mmsi)
        {
            if (string.IsNullOrWhiteSpace(mmsi))
                return false;

            if (mmsi.Length < 9 || mmsi.Length > 15)
                return false;

            if (!long.TryParse(mmsi, out _))
                return false;

            return true;
        }

        /// <summary>
        /// 初始化MID到国家的映射关系
        /// </summary>
        /// <returns>MID到国家信息的字典</returns>
        private static Dictionary<int, CountryInfo> InitializeMidMapping()
        {
            return new Dictionary<int, CountryInfo>
            {
                // ===== 欧洲 (2xx) =====
                { 201, new CountryInfo { Mid = 201, NameEn = "Albania", NameZh = "阿尔巴尼亚", IsoCode = "AL" } },
                { 202, new CountryInfo { Mid = 202, NameEn = "Andorra", NameZh = "安道尔", IsoCode = "AD" } },
                { 203, new CountryInfo { Mid = 203, NameEn = "Austria", NameZh = "奥地利", IsoCode = "AT" } },
                { 204, new CountryInfo { Mid = 204, NameEn = "Azores", NameZh = "亚速尔群岛", IsoCode = "PT" } },
                { 205, new CountryInfo { Mid = 205, NameEn = "Belgium", NameZh = "比利时", IsoCode = "BE" } },
                { 206, new CountryInfo { Mid = 206, NameEn = "Belarus", NameZh = "白俄罗斯", IsoCode = "BY" } },
                { 207, new CountryInfo { Mid = 207, NameEn = "Bulgaria", NameZh = "保加利亚", IsoCode = "BG" } },
                { 208, new CountryInfo { Mid = 208, NameEn = "Vatican City State", NameZh = "梵蒂冈城国", IsoCode = "VA" } },
                { 209, new CountryInfo { Mid = 209, NameEn = "Cyprus", NameZh = "塞浦路斯", IsoCode = "CY" } },
                { 210, new CountryInfo { Mid = 210, NameEn = "Cyprus", NameZh = "塞浦路斯", IsoCode = "CY" } },
                { 211, new CountryInfo { Mid = 211, NameEn = "Germany", NameZh = "德国", IsoCode = "DE" } },
                { 212, new CountryInfo { Mid = 212, NameEn = "Cyprus", NameZh = "塞浦路斯", IsoCode = "CY" } },
                { 213, new CountryInfo { Mid = 213, NameEn = "Georgia", NameZh = "格鲁吉亚", IsoCode = "GE" } },
                { 214, new CountryInfo { Mid = 214, NameEn = "Moldova", NameZh = "摩尔多瓦", IsoCode = "MD" } },
                { 215, new CountryInfo { Mid = 215, NameEn = "Malta", NameZh = "马耳他", IsoCode = "MT" } },
                { 216, new CountryInfo { Mid = 216, NameEn = "Armenia", NameZh = "亚美尼亚", IsoCode = "AM" } },
                { 218, new CountryInfo { Mid = 218, NameEn = "Germany", NameZh = "德国", IsoCode = "DE" } },
                { 219, new CountryInfo { Mid = 219, NameEn = "Denmark", NameZh = "丹麦", IsoCode = "DK" } },
                { 220, new CountryInfo { Mid = 220, NameEn = "Denmark", NameZh = "丹麦", IsoCode = "DK" } },
                { 224, new CountryInfo { Mid = 224, NameEn = "Spain", NameZh = "西班牙", IsoCode = "ES" } },
                { 225, new CountryInfo { Mid = 225, NameEn = "Spain", NameZh = "西班牙", IsoCode = "ES" } },
                { 226, new CountryInfo { Mid = 226, NameEn = "France", NameZh = "法国", IsoCode = "FR" } },
                { 227, new CountryInfo { Mid = 227, NameEn = "France", NameZh = "法国", IsoCode = "FR" } },
                { 228, new CountryInfo { Mid = 228, NameEn = "France", NameZh = "法国", IsoCode = "FR" } },
                { 229, new CountryInfo { Mid = 229, NameEn = "Malta", NameZh = "马耳他", IsoCode = "MT" } },
                { 230, new CountryInfo { Mid = 230, NameEn = "Finland", NameZh = "芬兰", IsoCode = "FI" } },
                { 231, new CountryInfo { Mid = 231, NameEn = "Faroe Islands", NameZh = "法罗群岛", IsoCode = "FO" } },
                { 232, new CountryInfo { Mid = 232, NameEn = "United Kingdom", NameZh = "英国", IsoCode = "GB" } },
                { 233, new CountryInfo { Mid = 233, NameEn = "United Kingdom", NameZh = "英国", IsoCode = "GB" } },
                { 234, new CountryInfo { Mid = 234, NameEn = "United Kingdom", NameZh = "英国", IsoCode = "GB" } },
                { 235, new CountryInfo { Mid = 235, NameEn = "United Kingdom", NameZh = "英国", IsoCode = "GB" } },
                { 236, new CountryInfo { Mid = 236, NameEn = "Gibraltar", NameZh = "直布罗陀", IsoCode = "GI" } },
                { 237, new CountryInfo { Mid = 237, NameEn = "Greece", NameZh = "希腊", IsoCode = "GR" } },
                { 238, new CountryInfo { Mid = 238, NameEn = "Croatia", NameZh = "克罗地亚", IsoCode = "HR" } },
                { 239, new CountryInfo { Mid = 239, NameEn = "Greece", NameZh = "希腊", IsoCode = "GR" } },
                { 240, new CountryInfo { Mid = 240, NameEn = "Greece", NameZh = "希腊", IsoCode = "GR" } },
                { 241, new CountryInfo { Mid = 241, NameEn = "Greece", NameZh = "希腊", IsoCode = "GR" } },
                { 242, new CountryInfo { Mid = 242, NameEn = "Morocco", NameZh = "摩洛哥", IsoCode = "MA" } },
                { 243, new CountryInfo { Mid = 243, NameEn = "Hungary", NameZh = "匈牙利", IsoCode = "HU" } },
                { 244, new CountryInfo { Mid = 244, NameEn = "Netherlands", NameZh = "荷兰", IsoCode = "NL" } },
                { 245, new CountryInfo { Mid = 245, NameEn = "Netherlands", NameZh = "荷兰", IsoCode = "NL" } },
                { 246, new CountryInfo { Mid = 246, NameEn = "Netherlands", NameZh = "荷兰", IsoCode = "NL" } },
                { 247, new CountryInfo { Mid = 247, NameEn = "Italy", NameZh = "意大利", IsoCode = "IT" } },
                { 248, new CountryInfo { Mid = 248, NameEn = "Malta", NameZh = "马耳他", IsoCode = "MT" } },
                { 249, new CountryInfo { Mid = 249, NameEn = "Malta", NameZh = "马耳他", IsoCode = "MT" } },
                { 250, new CountryInfo { Mid = 250, NameEn = "Ireland", NameZh = "爱尔兰", IsoCode = "IE" } },
                { 251, new CountryInfo { Mid = 251, NameEn = "Iceland", NameZh = "冰岛", IsoCode = "IS" } },
                { 252, new CountryInfo { Mid = 252, NameEn = "Liechtenstein", NameZh = "列支敦士登", IsoCode = "LI" } },
                { 253, new CountryInfo { Mid = 253, NameEn = "Luxembourg", NameZh = "卢森堡", IsoCode = "LU" } },
                { 254, new CountryInfo { Mid = 254, NameEn = "Monaco", NameZh = "摩纳哥", IsoCode = "MC" } },
                { 255, new CountryInfo { Mid = 255, NameEn = "Madeira", NameZh = "马德拉", IsoCode = "PT" } },
                { 256, new CountryInfo { Mid = 256, NameEn = "Malta", NameZh = "马耳他", IsoCode = "MT" } },
                { 257, new CountryInfo { Mid = 257, NameEn = "Norway", NameZh = "挪威", IsoCode = "NO" } },
                { 258, new CountryInfo { Mid = 258, NameEn = "Norway", NameZh = "挪威", IsoCode = "NO" } },
                { 259, new CountryInfo { Mid = 259, NameEn = "Norway", NameZh = "挪威", IsoCode = "NO" } },
                { 261, new CountryInfo { Mid = 261, NameEn = "Poland", NameZh = "波兰", IsoCode = "PL" } },
                { 262, new CountryInfo { Mid = 262, NameEn = "Montenegro", NameZh = "黑山", IsoCode = "ME" } },
                { 263, new CountryInfo { Mid = 263, NameEn = "Portugal", NameZh = "葡萄牙", IsoCode = "PT" } },
                { 264, new CountryInfo { Mid = 264, NameEn = "Romania", NameZh = "罗马尼亚", IsoCode = "RO" } },
                { 265, new CountryInfo { Mid = 265, NameEn = "Sweden", NameZh = "瑞典", IsoCode = "SE" } },
                { 266, new CountryInfo { Mid = 266, NameEn = "Sweden", NameZh = "瑞典", IsoCode = "SE" } },
                { 267, new CountryInfo { Mid = 267, NameEn = "Slovakia", NameZh = "斯洛伐克", IsoCode = "SK" } },
                { 268, new CountryInfo { Mid = 268, NameEn = "San Marino", NameZh = "圣马力诺", IsoCode = "SM" } },
                { 269, new CountryInfo { Mid = 269, NameEn = "Switzerland", NameZh = "瑞士", IsoCode = "CH" } },
                { 270, new CountryInfo { Mid = 270, NameEn = "Czech Republic", NameZh = "捷克共和国", IsoCode = "CZ" } },
                { 271, new CountryInfo { Mid = 271, NameEn = "Turkey", NameZh = "土耳其", IsoCode = "TR" } },
                { 272, new CountryInfo { Mid = 272, NameEn = "Ukraine", NameZh = "乌克兰", IsoCode = "UA" } },
                { 273, new CountryInfo { Mid = 273, NameEn = "Russia", NameZh = "俄罗斯联邦", IsoCode = "RU" } },
                { 274, new CountryInfo { Mid = 274, NameEn = "North Macedonia", NameZh = "北马其顿共和国", IsoCode = "MK" } },
                { 275, new CountryInfo { Mid = 275, NameEn = "Latvia", NameZh = "拉脱维亚", IsoCode = "LV" } },
                { 276, new CountryInfo { Mid = 276, NameEn = "Estonia", NameZh = "爱沙尼亚", IsoCode = "EE" } },
                { 277, new CountryInfo { Mid = 277, NameEn = "Lithuania", NameZh = "立陶宛", IsoCode = "LT" } },
                { 278, new CountryInfo { Mid = 278, NameEn = "Slovenia", NameZh = "斯洛文尼亚", IsoCode = "SI" } },
                { 279, new CountryInfo { Mid = 279, NameEn = "Serbia", NameZh = "塞尔维亚", IsoCode = "RS" } },

                // ===== 北美洲 (3xx) =====
                { 301, new CountryInfo { Mid = 301, NameEn = "Anguilla", NameZh = "安圭拉", IsoCode = "AI" } },
                { 303, new CountryInfo { Mid = 303, NameEn = "Alaska (USA)", NameZh = "美国阿拉斯加州", IsoCode = "US" } },
                { 304, new CountryInfo { Mid = 304, NameEn = "Antigua and Barbuda", NameZh = "安提瓜和巴布达", IsoCode = "AG" } },
                { 305, new CountryInfo { Mid = 305, NameEn = "Antigua and Barbuda", NameZh = "安提瓜和巴布达", IsoCode = "AG" } },
                { 306, new CountryInfo { Mid = 306, NameEn = "Sint Maarten (Dutch part)", NameZh = "圣马丁（荷兰部分）", IsoCode = "SX" } },
                { 307, new CountryInfo { Mid = 307, NameEn = "Bonaire, Sint Eustatius and Saba", NameZh = "博内尔、圣尤斯特歇斯和萨巴", IsoCode = "BQ" } },
                { 308, new CountryInfo { Mid = 308, NameEn = "Curacao", NameZh = "库拉索", IsoCode = "CW" } },
                { 309, new CountryInfo { Mid = 309, NameEn = "Aruba", NameZh = "阿鲁巴", IsoCode = "AW" } },
                { 310, new CountryInfo { Mid = 310, NameEn = "Bahamas", NameZh = "巴哈马", IsoCode = "BS" } },
                { 311, new CountryInfo { Mid = 311, NameEn = "Bahamas", NameZh = "巴哈马", IsoCode = "BS" } },
                { 312, new CountryInfo { Mid = 312, NameEn = "Belize", NameZh = "伯利兹", IsoCode = "BZ" } },
                { 314, new CountryInfo { Mid = 314, NameEn = "Barbados", NameZh = "巴巴多斯", IsoCode = "BB" } },
                { 316, new CountryInfo { Mid = 316, NameEn = "Canada", NameZh = "加拿大", IsoCode = "CA" } },
                { 319, new CountryInfo { Mid = 319, NameEn = "Cayman Islands", NameZh = "开曼群岛", IsoCode = "KY" } },
                { 321, new CountryInfo { Mid = 321, NameEn = "Costa Rica", NameZh = "哥斯达黎加", IsoCode = "CR" } },
                { 323, new CountryInfo { Mid = 323, NameEn = "Cuba", NameZh = "古巴", IsoCode = "CU" } },
                { 325, new CountryInfo { Mid = 325, NameEn = "Dominica", NameZh = "多米尼加", IsoCode = "DM" } },
                { 327, new CountryInfo { Mid = 327, NameEn = "Dominican Republic", NameZh = "多米尼加共和国", IsoCode = "DO" } },
                { 329, new CountryInfo { Mid = 329, NameEn = "Guadeloupe (French Department)", NameZh = "法属瓜德罗普", IsoCode = "GP" } },
                { 330, new CountryInfo { Mid = 330, NameEn = "Grenada", NameZh = "格林纳达", IsoCode = "GD" } },
                { 331, new CountryInfo { Mid = 331, NameEn = "Greenland", NameZh = "格陵兰", IsoCode = "GL" } },
                { 332, new CountryInfo { Mid = 332, NameEn = "Guatemala", NameZh = "危地马拉", IsoCode = "GT" } },
                { 334, new CountryInfo { Mid = 334, NameEn = "Honduras", NameZh = "洪都拉斯", IsoCode = "HN" } },
                { 336, new CountryInfo { Mid = 336, NameEn = "Haiti", NameZh = "海地", IsoCode = "HT" } },
                { 338, new CountryInfo { Mid = 338, NameEn = "United States of America", NameZh = "美国", IsoCode = "US" } },
                { 339, new CountryInfo { Mid = 339, NameEn = "Jamaica", NameZh = "牙买加", IsoCode = "JM" } },
                { 341, new CountryInfo { Mid = 341, NameEn = "Saint Kitts and Nevis", NameZh = "圣基茨和尼维斯", IsoCode = "KN" } },
                { 343, new CountryInfo { Mid = 343, NameEn = "Saint Lucia", NameZh = "圣卢西亚", IsoCode = "LC" } },
                { 345, new CountryInfo { Mid = 345, NameEn = "Mexico", NameZh = "墨西哥", IsoCode = "MX" } },
                { 347, new CountryInfo { Mid = 347, NameEn = "Martinique", NameZh = "法属马提尼克", IsoCode = "MQ" } },
                { 348, new CountryInfo { Mid = 348, NameEn = "Montserrat", NameZh = "蒙特塞拉特", IsoCode = "MS" } },
                { 350, new CountryInfo { Mid = 350, NameEn = "Nicaragua", NameZh = "尼加拉瓜", IsoCode = "NI" } },
                { 351, new CountryInfo { Mid = 351, NameEn = "Panama", NameZh = "巴拿马", IsoCode = "PA" } },
                { 352, new CountryInfo { Mid = 352, NameEn = "Panama", NameZh = "巴拿马", IsoCode = "PA" } },
                { 353, new CountryInfo { Mid = 353, NameEn = "Panama", NameZh = "巴拿马", IsoCode = "PA" } },
                { 354, new CountryInfo { Mid = 354, NameEn = "Panama", NameZh = "巴拿马", IsoCode = "PA" } },
                { 358, new CountryInfo { Mid = 358, NameEn = "Puerto Rico", NameZh = "波多黎各", IsoCode = "PR" } },
                { 359, new CountryInfo { Mid = 359, NameEn = "El Salvador", NameZh = "萨尔瓦多", IsoCode = "SV" } },
                { 361, new CountryInfo { Mid = 361, NameEn = "Saint Pierre and Miquelon", NameZh = "法属圣皮埃尔和密克隆", IsoCode = "PM" } },
                { 362, new CountryInfo { Mid = 362, NameEn = "Trinidad and Tobago", NameZh = "特立尼达和多巴哥", IsoCode = "TT" } },
                { 364, new CountryInfo { Mid = 364, NameEn = "Turks and Caicos Islands", NameZh = "特克斯和凯科斯群岛", IsoCode = "TC" } },
                { 366, new CountryInfo { Mid = 366, NameEn = "United States of America", NameZh = "美国", IsoCode = "US" } },
                { 367, new CountryInfo { Mid = 367, NameEn = "United States of America", NameZh = "美国", IsoCode = "US" } },
                { 368, new CountryInfo { Mid = 368, NameEn = "United States of America", NameZh = "美国", IsoCode = "US" } },
                { 369, new CountryInfo { Mid = 369, NameEn = "United States of America", NameZh = "美国", IsoCode = "US" } },
                { 370, new CountryInfo { Mid = 370, NameEn = "Panama", NameZh = "巴拿马", IsoCode = "PA" } },
                { 371, new CountryInfo { Mid = 371, NameEn = "Panama", NameZh = "巴拿马", IsoCode = "PA" } },
                { 372, new CountryInfo { Mid = 372, NameEn = "Panama", NameZh = "巴拿马", IsoCode = "PA" } },
                { 373, new CountryInfo { Mid = 373, NameEn = "Panama", NameZh = "巴拿马", IsoCode = "PA" } },
                { 375, new CountryInfo { Mid = 375, NameEn = "Saint Vincent and the Grenadines", NameZh = "圣文森特和格林纳丁斯", IsoCode = "VC" } },
                { 376, new CountryInfo { Mid = 376, NameEn = "Saint Vincent and the Grenadines", NameZh = "圣文森特和格林纳丁斯", IsoCode = "VC" } },
                { 377, new CountryInfo { Mid = 377, NameEn = "Saint Vincent and the Grenadines", NameZh = "圣文森特和格林纳丁斯", IsoCode = "VC" } },
                { 378, new CountryInfo { Mid = 378, NameEn = "British Virgin Islands", NameZh = "英属维尔京群岛", IsoCode = "VG" } },
                { 379, new CountryInfo { Mid = 379, NameEn = "US Virgin Islands", NameZh = "美属维尔京群岛", IsoCode = "VI" } },

                // ===== 亚洲 (4xx) =====
                { 401, new CountryInfo { Mid = 401, NameEn = "Afghanistan", NameZh = "阿富汗", IsoCode = "AF" } },
                { 403, new CountryInfo { Mid = 403, NameEn = "Saudi Arabia", NameZh = "沙特阿拉伯", IsoCode = "SA" } },
                { 405, new CountryInfo { Mid = 405, NameEn = "Bangladesh", NameZh = "孟加拉国", IsoCode = "BD" } },
                { 408, new CountryInfo { Mid = 408, NameEn = "Bahrain", NameZh = "巴林", IsoCode = "BH" } },
                { 410, new CountryInfo { Mid = 410, NameEn = "Bhutan", NameZh = "不丹", IsoCode = "BT" } },
                { 412, new CountryInfo { Mid = 412, NameEn = "China", NameZh = "中国", IsoCode = "CN" } },
                { 413, new CountryInfo { Mid = 413, NameEn = "China", NameZh = "中国", IsoCode = "CN" } },
                { 414, new CountryInfo { Mid = 414, NameEn = "China", NameZh = "中国", IsoCode = "CN" } },
                { 416, new CountryInfo { Mid = 416, NameEn = "Taiwan", NameZh = "中国台湾", IsoCode = "TW" } },
                { 417, new CountryInfo { Mid = 417, NameEn = "Sri Lanka", NameZh = "斯里兰卡", IsoCode = "LK" } },
                { 419, new CountryInfo { Mid = 419, NameEn = "India", NameZh = "印度", IsoCode = "IN" } },
                { 422, new CountryInfo { Mid = 422, NameEn = "Iran", NameZh = "伊朗", IsoCode = "IR" } },
                { 423, new CountryInfo { Mid = 423, NameEn = "Azerbaijan", NameZh = "阿塞拜疆", IsoCode = "AZ" } },
                { 425, new CountryInfo { Mid = 425, NameEn = "Iraq", NameZh = "伊拉克", IsoCode = "IQ" } },
                { 428, new CountryInfo { Mid = 428, NameEn = "Israel", NameZh = "以色列", IsoCode = "IL" } },
                { 431, new CountryInfo { Mid = 431, NameEn = "Japan", NameZh = "日本", IsoCode = "JP" } },
                { 432, new CountryInfo { Mid = 432, NameEn = "Japan", NameZh = "日本", IsoCode = "JP" } },
                { 434, new CountryInfo { Mid = 434, NameEn = "Turkmenistan", NameZh = "土库曼斯坦", IsoCode = "TM" } },
                { 436, new CountryInfo { Mid = 436, NameEn = "Kazakhstan", NameZh = "哈萨克斯坦", IsoCode = "KZ" } },
                { 437, new CountryInfo { Mid = 437, NameEn = "Uzbekistan", NameZh = "乌兹别克斯坦", IsoCode = "UZ" } },
                { 438, new CountryInfo { Mid = 438, NameEn = "Jordan", NameZh = "约旦", IsoCode = "JO" } },
                { 440, new CountryInfo { Mid = 440, NameEn = "South Korea", NameZh = "韩国", IsoCode = "KR" } },
                { 441, new CountryInfo { Mid = 441, NameEn = "South Korea", NameZh = "韩国", IsoCode = "KR" } },
                { 443, new CountryInfo { Mid = 443, NameEn = "Palestine", NameZh = "巴勒斯坦国", IsoCode = "PS" } },
                { 445, new CountryInfo { Mid = 445, NameEn = "North Korea", NameZh = "朝鲜", IsoCode = "KP" } },
                { 447, new CountryInfo { Mid = 447, NameEn = "Kuwait", NameZh = "科威特", IsoCode = "KW" } },
                { 450, new CountryInfo { Mid = 450, NameEn = "Lebanon", NameZh = "黎巴嫩", IsoCode = "LB" } },
                { 451, new CountryInfo { Mid = 451, NameEn = "Kyrgyzstan", NameZh = "吉尔吉斯斯坦", IsoCode = "KG" } },
                { 453, new CountryInfo { Mid = 453, NameEn = "Macao", NameZh = "中国澳门", IsoCode = "MO" } },
                { 455, new CountryInfo { Mid = 455, NameEn = "Maldives", NameZh = "马尔代夫", IsoCode = "MV" } },
                { 457, new CountryInfo { Mid = 457, NameEn = "Mongolia", NameZh = "蒙古", IsoCode = "MN" } },
                { 459, new CountryInfo { Mid = 459, NameEn = "Nepal", NameZh = "尼泊尔", IsoCode = "NP" } },
                { 461, new CountryInfo { Mid = 461, NameEn = "Oman", NameZh = "阿曼", IsoCode = "OM" } },
                { 463, new CountryInfo { Mid = 463, NameEn = "Pakistan", NameZh = "巴基斯坦", IsoCode = "PK" } },
                { 466, new CountryInfo { Mid = 466, NameEn = "Qatar", NameZh = "卡塔尔", IsoCode = "QA" } },
                { 468, new CountryInfo { Mid = 468, NameEn = "Syria", NameZh = "叙利亚", IsoCode = "SY" } },
                { 470, new CountryInfo { Mid = 470, NameEn = "United Arab Emirates", NameZh = "阿拉伯联合酋长国", IsoCode = "AE" } },
                { 472, new CountryInfo { Mid = 472, NameEn = "Tajikistan", NameZh = "塔吉克斯坦", IsoCode = "TJ" } },
                { 473, new CountryInfo { Mid = 473, NameEn = "Yemen", NameZh = "也门", IsoCode = "YE" } },
                { 475, new CountryInfo { Mid = 475, NameEn = "Yemen", NameZh = "也门", IsoCode = "YE" } },
                { 477, new CountryInfo { Mid = 477, NameEn = "Hong Kong", NameZh = "中国香港", IsoCode = "HK" } },
                { 478, new CountryInfo { Mid = 478, NameEn = "Bosnia and Herzegovina", NameZh = "波斯尼亚和黑塞哥维那", IsoCode = "BA" } },

                // ===== 大洋洲 (5xx) =====
                { 501, new CountryInfo { Mid = 501, NameEn = "Adélie Land", NameZh = "阿德莱德地", IsoCode = "FR" } },
                { 503, new CountryInfo { Mid = 503, NameEn = "Australia", NameZh = "澳大利亚", IsoCode = "AU" } },
                { 506, new CountryInfo { Mid = 506, NameEn = "Myanmar", NameZh = "缅甸", IsoCode = "MM" } },
                { 508, new CountryInfo { Mid = 508, NameEn = "Brunei Darussalam", NameZh = "文莱达鲁萨兰国", IsoCode = "BN" } },
                { 510, new CountryInfo { Mid = 510, NameEn = "Micronesia", NameZh = "密克罗尼西亚", IsoCode = "FM" } },
                { 511, new CountryInfo { Mid = 511, NameEn = "Palau", NameZh = "帕劳", IsoCode = "PW" } },
                { 512, new CountryInfo { Mid = 512, NameEn = "New Zealand", NameZh = "新西兰", IsoCode = "NZ" } },
                { 514, new CountryInfo { Mid = 514, NameEn = "Cambodia", NameZh = "柬埔寨", IsoCode = "KH" } },
                { 515, new CountryInfo { Mid = 515, NameEn = "Cambodia", NameZh = "柬埔寨", IsoCode = "KH" } },
                { 516, new CountryInfo { Mid = 516, NameEn = "Christmas Island (Indian Ocean)", NameZh = "印度洋的圣诞岛", IsoCode = "CX" } },
                { 518, new CountryInfo { Mid = 518, NameEn = "Cook Islands", NameZh = "库克群岛", IsoCode = "CK" } },
                { 520, new CountryInfo { Mid = 520, NameEn = "Fiji", NameZh = "斐济", IsoCode = "FJ" } },
                { 523, new CountryInfo { Mid = 523, NameEn = "Cocos (Keeling) Islands", NameZh = "科科斯（基林）群岛", IsoCode = "CC" } },
                { 525, new CountryInfo { Mid = 525, NameEn = "Indonesia", NameZh = "印度尼西亚", IsoCode = "ID" } },
                { 529, new CountryInfo { Mid = 529, NameEn = "Kiribati", NameZh = "基里巴斯", IsoCode = "KI" } },
                { 531, new CountryInfo { Mid = 531, NameEn = "Lao People's Democratic Republic", NameZh = "老挝人民民主共和国", IsoCode = "LA" } },
                { 533, new CountryInfo { Mid = 533, NameEn = "Malaysia", NameZh = "马来西亚", IsoCode = "MY" } },
                { 536, new CountryInfo { Mid = 536, NameEn = "Northern Mariana Islands", NameZh = "北马里亚纳群岛", IsoCode = "MP" } },
                { 538, new CountryInfo { Mid = 538, NameEn = "Marshall Islands", NameZh = "马绍尔群岛", IsoCode = "MH" } },
                { 540, new CountryInfo { Mid = 540, NameEn = "New Caledonia", NameZh = "新喀里多尼亚", IsoCode = "NC" } },
                { 542, new CountryInfo { Mid = 542, NameEn = "Niue", NameZh = "纽埃", IsoCode = "NU" } },
                { 544, new CountryInfo { Mid = 544, NameEn = "Nauru", NameZh = "瑙鲁", IsoCode = "NR" } },
                { 546, new CountryInfo { Mid = 546, NameEn = "French Polynesia", NameZh = "法属波利尼西亚", IsoCode = "PF" } },
                { 548, new CountryInfo { Mid = 548, NameEn = "Philippines", NameZh = "菲律宾", IsoCode = "PH" } },
                { 553, new CountryInfo { Mid = 553, NameEn = "Papua New Guinea", NameZh = "巴布亚新几内亚", IsoCode = "PG" } },
                { 555, new CountryInfo { Mid = 555, NameEn = "Pitcairn Island", NameZh = "皮特凯恩群岛", IsoCode = "PN" } },
                { 557, new CountryInfo { Mid = 557, NameEn = "Solomon Islands", NameZh = "所罗门群岛", IsoCode = "SB" } },
                { 559, new CountryInfo { Mid = 559, NameEn = "American Samoa", NameZh = "美属萨摩亚", IsoCode = "AS" } },
                { 561, new CountryInfo { Mid = 561, NameEn = "Samoa", NameZh = "萨摩亚", IsoCode = "WS" } },
                { 563, new CountryInfo { Mid = 563, NameEn = "Singapore", NameZh = "新加坡", IsoCode = "SG" } },
                { 564, new CountryInfo { Mid = 564, NameEn = "Singapore", NameZh = "新加坡", IsoCode = "SG" } },
                { 565, new CountryInfo { Mid = 565, NameEn = "Singapore", NameZh = "新加坡", IsoCode = "SG" } },
                { 566, new CountryInfo { Mid = 566, NameEn = "Singapore", NameZh = "新加坡", IsoCode = "SG" } },
                { 567, new CountryInfo { Mid = 567, NameEn = "Thailand", NameZh = "泰国", IsoCode = "TH" } },
                { 570, new CountryInfo { Mid = 570, NameEn = "Tonga", NameZh = "汤加", IsoCode = "TO" } },
                { 572, new CountryInfo { Mid = 572, NameEn = "Tuvalu", NameZh = "图瓦卢", IsoCode = "TV" } },
                { 574, new CountryInfo { Mid = 574, NameEn = "Viet Nam", NameZh = "越南", IsoCode = "VN" } },
                { 576, new CountryInfo { Mid = 576, NameEn = "Vanuatu", NameZh = "瓦努阿图", IsoCode = "VU" } },
                { 577, new CountryInfo { Mid = 577, NameEn = "Vanuatu", NameZh = "瓦努阿图", IsoCode = "VU" } },
                { 578, new CountryInfo { Mid = 578, NameEn = "Wallis and Futuna Islands", NameZh = "瓦利斯和富图纳群岛", IsoCode = "WF" } },

                // ===== 非洲 (6xx) =====
                { 601, new CountryInfo { Mid = 601, NameEn = "South Africa", NameZh = "南非", IsoCode = "ZA" } },
                { 603, new CountryInfo { Mid = 603, NameEn = "Angola", NameZh = "安哥拉", IsoCode = "AO" } },
                { 605, new CountryInfo { Mid = 605, NameEn = "Algeria", NameZh = "阿尔及利亚", IsoCode = "DZ" } },
                { 607, new CountryInfo { Mid = 607, NameEn = "Saint Paul and Amsterdam Islands", NameZh = "圣保罗和阿姆斯特丹群岛", IsoCode = "TF" } },
                { 608, new CountryInfo { Mid = 608, NameEn = "Ascension Island", NameZh = "阿森松岛", IsoCode = "SH" } },
                { 609, new CountryInfo { Mid = 609, NameEn = "Burundi", NameZh = "布隆迪", IsoCode = "BI" } },
                { 610, new CountryInfo { Mid = 610, NameEn = "Benin", NameZh = "贝宁", IsoCode = "BJ" } },
                { 611, new CountryInfo { Mid = 611, NameEn = "Botswana", NameZh = "博茨瓦纳", IsoCode = "BW" } },
                { 612, new CountryInfo { Mid = 612, NameEn = "Central African Republic", NameZh = "中非共和国", IsoCode = "CF" } },
                { 613, new CountryInfo { Mid = 613, NameEn = "Cameroon", NameZh = "喀麦隆", IsoCode = "CM" } },
                { 615, new CountryInfo { Mid = 615, NameEn = "Congo", NameZh = "刚果共和国", IsoCode = "CG" } },
                { 616, new CountryInfo { Mid = 616, NameEn = "Comoros", NameZh = "科摩罗", IsoCode = "KM" } },
                { 617, new CountryInfo { Mid = 617, NameEn = "Cabo Verde", NameZh = "佛得角", IsoCode = "CV" } },
                { 618, new CountryInfo { Mid = 618, NameEn = "Crozet Archipelago", NameZh = "克罗泽群岛", IsoCode = "TF" } },
                { 619, new CountryInfo { Mid = 619, NameEn = "Côte d'Ivoire", NameZh = "科特迪瓦", IsoCode = "CI" } },
                { 620, new CountryInfo { Mid = 620, NameEn = "Comoros", NameZh = "科摩罗", IsoCode = "KM" } },
                { 621, new CountryInfo { Mid = 621, NameEn = "Djibouti", NameZh = "吉布提", IsoCode = "DJ" } },
                { 622, new CountryInfo { Mid = 622, NameEn = "Egypt", NameZh = "埃及", IsoCode = "EG" } },
                { 624, new CountryInfo { Mid = 624, NameEn = "Ethiopia", NameZh = "埃塞俄比亚", IsoCode = "ET" } },
                { 625, new CountryInfo { Mid = 625, NameEn = "Eritrea", NameZh = "厄立特里亚", IsoCode = "ER" } },
                { 626, new CountryInfo { Mid = 626, NameEn = "Gabonese Republic", NameZh = "加蓬共和国", IsoCode = "GA" } },
                { 627, new CountryInfo { Mid = 627, NameEn = "Ghana", NameZh = "加纳", IsoCode = "GH" } },
                { 629, new CountryInfo { Mid = 629, NameEn = "Gambia", NameZh = "冈比亚", IsoCode = "GM" } },
                { 630, new CountryInfo { Mid = 630, NameEn = "Guinea-Bissau", NameZh = "几内亚比绍", IsoCode = "GW" } },
                { 631, new CountryInfo { Mid = 631, NameEn = "Equatorial Guinea", NameZh = "赤道几内亚", IsoCode = "GQ" } },
                { 632, new CountryInfo { Mid = 632, NameEn = "Guinea", NameZh = "几内亚", IsoCode = "GN" } },
                { 633, new CountryInfo { Mid = 633, NameEn = "Burkina Faso", NameZh = "布基纳法索", IsoCode = "BF" } },
                { 634, new CountryInfo { Mid = 634, NameEn = "Kenya", NameZh = "肯尼亚", IsoCode = "KE" } },
                { 635, new CountryInfo { Mid = 635, NameEn = "Kerguelen Islands", NameZh = "克尔格伦群岛", IsoCode = "TF" } },
                { 636, new CountryInfo { Mid = 636, NameEn = "Liberia", NameZh = "利比里亚", IsoCode = "LR" } },
                { 637, new CountryInfo { Mid = 637, NameEn = "Liberia", NameZh = "利比里亚", IsoCode = "LR" } },
                { 638, new CountryInfo { Mid = 638, NameEn = "South Sudan", NameZh = "南苏丹", IsoCode = "SS" } },
                { 642, new CountryInfo { Mid = 642, NameEn = "Libya", NameZh = "利比亚", IsoCode = "LY" } },
                { 644, new CountryInfo { Mid = 644, NameEn = "Lesotho", NameZh = "莱索托", IsoCode = "LS" } },
                { 645, new CountryInfo { Mid = 645, NameEn = "Mauritius", NameZh = "毛里求斯", IsoCode = "MU" } },
                { 647, new CountryInfo { Mid = 647, NameEn = "Madagascar", NameZh = "马达加斯加", IsoCode = "MG" } },
                { 649, new CountryInfo { Mid = 649, NameEn = "Mali", NameZh = "马里", IsoCode = "ML" } },
                { 650, new CountryInfo { Mid = 650, NameEn = "Mozambique", NameZh = "莫桑比克", IsoCode = "MZ" } },
                { 654, new CountryInfo { Mid = 654, NameEn = "Mauritania", NameZh = "毛里塔尼亚", IsoCode = "MR" } },
                { 655, new CountryInfo { Mid = 655, NameEn = "Malawi", NameZh = "马拉维", IsoCode = "MW" } },
                { 656, new CountryInfo { Mid = 656, NameEn = "Niger", NameZh = "尼日尔", IsoCode = "NE" } },
                { 657, new CountryInfo { Mid = 657, NameEn = "Nigeria", NameZh = "尼日利亚", IsoCode = "NG" } },
                { 659, new CountryInfo { Mid = 659, NameEn = "Namibia", NameZh = "纳米比亚", IsoCode = "NA" } },
                { 660, new CountryInfo { Mid = 660, NameEn = "Reunion (French Department)", NameZh = "留尼汪（法国海外省）", IsoCode = "RE" } },
                { 661, new CountryInfo { Mid = 661, NameEn = "Rwanda", NameZh = "卢旺达", IsoCode = "RW" } },
                { 662, new CountryInfo { Mid = 662, NameEn = "Sudan", NameZh = "苏丹", IsoCode = "SD" } },
                { 663, new CountryInfo { Mid = 663, NameEn = "Senegal", NameZh = "塞内加尔", IsoCode = "SN" } },
                { 664, new CountryInfo { Mid = 664, NameEn = "Seychelles", NameZh = "塞舌尔", IsoCode = "SC" } },
                { 665, new CountryInfo { Mid = 665, NameEn = "Saint Helena", NameZh = "圣赫勒拿", IsoCode = "SH" } },
                { 666, new CountryInfo { Mid = 666, NameEn = "Somalia", NameZh = "索马里", IsoCode = "SO" } },
                { 667, new CountryInfo { Mid = 667, NameEn = "Sierra Leone", NameZh = "塞拉利昂", IsoCode = "SL" } },
                { 668, new CountryInfo { Mid = 668, NameEn = "Sao Tome and Principe", NameZh = "圣多美和普林西比", IsoCode = "ST" } },
                { 669, new CountryInfo { Mid = 669, NameEn = "Swaziland", NameZh = "斯威士兰/埃斯瓦蒂尼王国", IsoCode = "SZ" } },
                { 670, new CountryInfo { Mid = 670, NameEn = "Chad", NameZh = "乍得", IsoCode = "TD" } },
                { 671, new CountryInfo { Mid = 671, NameEn = "Togolese Republic", NameZh = "多哥", IsoCode = "TG" } },
                { 672, new CountryInfo { Mid = 672, NameEn = "Tunisia", NameZh = "突尼斯", IsoCode = "TN" } },
                { 674, new CountryInfo { Mid = 674, NameEn = "Tanzania", NameZh = "坦桑尼亚", IsoCode = "TZ" } },
                { 675, new CountryInfo { Mid = 675, NameEn = "Uganda", NameZh = "乌干达", IsoCode = "UG" } },
                { 676, new CountryInfo { Mid = 676, NameEn = "Democratic Republic of the Congo", NameZh = "刚果民主共和国", IsoCode = "CD" } },
                { 677, new CountryInfo { Mid = 677, NameEn = "Tanzania", NameZh = "坦桑尼亚", IsoCode = "TZ" } },
                { 678, new CountryInfo { Mid = 678, NameEn = "Zambia", NameZh = "赞比亚", IsoCode = "ZM" } },
                { 679, new CountryInfo { Mid = 679, NameEn = "Zimbabwe", NameZh = "津巴布韦", IsoCode = "ZW" } },

                // ===== 南美洲 (7xx) =====
                { 701, new CountryInfo { Mid = 701, NameEn = "Argentina", NameZh = "阿根廷", IsoCode = "AR" } },
                { 710, new CountryInfo { Mid = 710, NameEn = "Brazil", NameZh = "巴西", IsoCode = "BR" } },
                { 720, new CountryInfo { Mid = 720, NameEn = "Bolivia", NameZh = "玻利维亚", IsoCode = "BO" } },
                { 725, new CountryInfo { Mid = 725, NameEn = "Chile", NameZh = "智利", IsoCode = "CL" } },
                { 730, new CountryInfo { Mid = 730, NameEn = "Colombia", NameZh = "哥伦比亚", IsoCode = "CO" } },
                { 735, new CountryInfo { Mid = 735, NameEn = "Ecuador", NameZh = "厄瓜多尔", IsoCode = "EC" } },
                { 740, new CountryInfo { Mid = 740, NameEn = "Falkland Islands (Malvinas)", NameZh = "福克兰群岛（马尔维纳斯）", IsoCode = "FK" } },
                { 745, new CountryInfo { Mid = 745, NameEn = "Guiana (French Department)", NameZh = "法属圭亚那", IsoCode = "GF" } },
                { 750, new CountryInfo { Mid = 750, NameEn = "Guyana", NameZh = "圭亚那", IsoCode = "GY" } },
                { 755, new CountryInfo { Mid = 755, NameEn = "Paraguay", NameZh = "巴拉圭", IsoCode = "PY" } },
                { 760, new CountryInfo { Mid = 760, NameEn = "Peru", NameZh = "秘鲁", IsoCode = "PE" } },
                { 765, new CountryInfo { Mid = 765, NameEn = "Suriname", NameZh = "苏里南", IsoCode = "SR" } },
                { 770, new CountryInfo { Mid = 770, NameEn = "Uruguay", NameZh = "乌拉圭", IsoCode = "UY" } },
                { 775, new CountryInfo { Mid = 775, NameEn = "Venezuela", NameZh = "委内瑞拉", IsoCode = "VE" } }
            };
        }
    }
}
