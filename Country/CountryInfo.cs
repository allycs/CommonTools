namespace Allycs.Common.Country
{
    /// <summary>
    /// 国家信息类，包含国家的基本信息
    /// </summary>
    public class CountryInfo
    {
        /// <summary>
        /// 国家的MID代码（Maritime Identification Digits）
        /// </summary>
        public int Mid { get; set; }

        /// <summary>
        /// 国家英文名称
        /// </summary>
        public string NameEn { get; set; } = string.Empty;

        /// <summary>
        /// 国家中文名称
        /// </summary>
        public string NameZh { get; set; } = string.Empty;

        /// <summary>
        /// ISO 3166-1 alpha-2 国家代码（两位字母）
        /// </summary>
        public string IsoCode { get; set; } = string.Empty;

        /// <summary>
        /// 返回国家信息的字符串表示
        /// </summary>
        /// <returns>国家信息字符串</returns>
        public override string ToString()
        {
            return $"{NameZh} ({NameEn}) - ISO: {IsoCode}, MID: {Mid}";
        }
    }

    /// <summary>
    /// ICAO国家信息类，扩展自CountryInfo，包含ICAO相关信息
    /// </summary>
    public class IcaoCountryInfo : CountryInfo
    {
        /// <summary>
        /// ICAO 24位地址码（六字符十六进制）
        /// </summary>
        public string Icao24 { get; set; } = string.Empty;

        /// <summary>
        /// 返回ICAO国家信息的字符串表示
        /// </summary>
        /// <returns>ICAO国家信息字符串</returns>
        public override string ToString()
        {
            return $"{base.ToString()}, ICAO24: {Icao24}";
        }
    }
}
