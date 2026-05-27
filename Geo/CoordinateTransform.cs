namespace Allycs.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// 坐标系转换工具类，提供WGS84、GCJ02、BD09之间的坐标转换功能
    /// </summary>
    /// <remarks>
    /// WGS84: GPS原始坐标系
    /// GCJ02: 国测局坐标系，又称火星坐标系
    /// BD09: 百度坐标系
    /// </remarks>
    public static class CoordinateTransform
    {
        private static readonly double X_PI = Math.PI * 3000.0 / 180.0;
        private static readonly double PI = Math.PI;
        private static readonly double A = 6378245.0;
        private static readonly double EE = 0.00669342162296594323;

        /// <summary>
        /// 将百度坐标系(BD-09)转换为火星坐标系(GCJ-02)
        /// </summary>
        /// <param name="bdLon">百度坐标系的经度</param>
        /// <param name="bdLat">百度坐标系的纬度</param>
        /// <returns>火星坐标系坐标 [经度, 纬度]</returns>
        public static double[] BD09ToGCJ02(double bdLon, double bdLat)
        {
            double x = bdLon - 0.0065;
            double y = bdLat - 0.006;
            double z = Math.Sqrt(x * x + y * y) - 0.00002 * Math.Sin(y * X_PI);
            double theta = Math.Atan2(y, x) - 0.000003 * Math.Cos(x * X_PI);
            return new double[] { z * Math.Cos(theta), z * Math.Sin(theta) };
        }

        /// <summary>
        /// 将火星坐标系(GCJ-02)转换为百度坐标系(BD-09)
        /// </summary>
        /// <param name="lng">火星坐标系的经度</param>
        /// <param name="lat">火星坐标系的纬度</param>
        /// <returns>百度坐标系坐标字符串 "经度,纬度"</returns>
        public static string GCJ02ToBD09(double lng, double lat)
        {
            double z = Math.Sqrt(lng * lng + lat * lat) + 0.00002 * Math.Sin(lat * X_PI);
            double theta = Math.Atan2(lat, lng) + 0.000003 * Math.Cos(lng * X_PI);
            double bdLon = z * Math.Cos(theta) + 0.0065;
            double bdLat = z * Math.Sin(theta) + 0.006;
            return $"{bdLon},{bdLat}";
        }

        /// <summary>
        /// 将WGS84坐标系转换为GCJ-02坐标系
        /// </summary>
        /// <param name="lng">WGS84经度</param>
        /// <param name="lat">WGS84纬度</param>
        /// <returns>GCJ-02坐标字符串 "经度,纬度"</returns>
        public static string WGS84ToGCJ02(double lng, double lat)
        {
            var result = WGS84ToGCJ02Arr(lng, lat);
            return $"{result[0]},{result[1]}";
        }

        /// <summary>
        /// 将WGS84坐标系转换为GCJ-02坐标系
        /// </summary>
        /// <param name="lng">WGS84经度</param>
        /// <param name="lat">WGS84纬度</param>
        /// <returns>GCJ-02坐标数组 [经度, 纬度]</returns>
        public static double[] WGS84ToGCJ02Arr(double lng, double lat)
        {
            double dlat = TransformLat(lng - 105.0, lat - 35.0);
            double dlon = TransformLon(lng - 105.0, lat - 35.0);
            double radlat = lat / 180.0 * PI;
            double magic = 1 - EE * Math.Sin(radlat) * Math.Sin(radlat);
            double sqrtmagic = Math.Sqrt(magic);
            dlat = (dlat * 180.0) / ((A * (1 - EE)) / (magic * sqrtmagic) * PI);
            dlon = (dlon * 180.0) / (A / sqrtmagic * Math.Cos(radlat) * PI);
            return new double[] { lng + dlon, lat + dlat };
        }

        /// <summary>
        /// 将WGS84坐标系转换为GCJ-02坐标系（返回纬度-经度顺序）
        /// </summary>
        /// <param name="lng">WGS84经度</param>
        /// <param name="lat">WGS84纬度</param>
        /// <returns>GCJ-02坐标数组 [纬度, 经度]</returns>
        public static double[] WGS84ToGCJ02ByLatLon(double lng, double lat)
        {
            var result = WGS84ToGCJ02Arr(lng, lat);
            return new double[] { result[1], result[0] };
        }

        /// <summary>
        /// 将GCJ-02坐标系转换为WGS84坐标系
        /// </summary>
        /// <param name="lon">GCJ-02经度</param>
        /// <param name="lat">GCJ-02纬度</param>
        /// <returns>WGS84坐标字符串 "纬度,经度"</returns>
        public static string GCJ02ToWGS84(double lon, double lat)
        {
            var result = GCJ02ToWGS84Arr(lon, lat);
            return $"{result[1]},{result[0]}";
        }

        /// <summary>
        /// 将GCJ-02坐标系转换为WGS84坐标系
        /// </summary>
        /// <param name="lon">GCJ-02经度</param>
        /// <param name="lat">GCJ-02纬度</param>
        /// <returns>WGS84坐标数组 [经度, 纬度]</returns>
        public static double[] GCJ02ToWGS84Arr(double lon, double lat)
        {
            double dlat = TransformLat(lon - 105.0, lat - 35.0);
            double dlon = TransformLon(lon - 105.0, lat - 35.0);
            double radlat = lat / 180.0 * PI;
            double magic = 1 - EE * Math.Sin(radlat) * Math.Sin(radlat);
            double sqrtmagic = Math.Sqrt(magic);
            dlat = (dlat * 180.0) / ((A * (1 - EE)) / (magic * sqrtmagic) * PI);
            dlon = (dlon * 180.0) / (A / sqrtmagic * Math.Cos(radlat) * PI);
            return new double[] { lon + dlon, lat + dlat };
        }

        /// <summary>
        /// 将WGS84坐标系转换为百度坐标系(BD-09)
        /// </summary>
        /// <param name="lng">WGS84经度</param>
        /// <param name="lat">WGS84纬度</param>
        /// <returns>百度坐标系坐标 [经度, 纬度]</returns>
        public static double[] WGS84ToBD09(double lng, double lat)
        {
            var gcj02 = WGS84ToGCJ02Arr(lng, lat);
            double z = Math.Sqrt(gcj02[0] * gcj02[0] + gcj02[1] * gcj02[1]) + 0.00002 * Math.Sin(gcj02[1] * X_PI);
            double theta = Math.Atan2(gcj02[1], gcj02[0]) + 0.000003 * Math.Cos(gcj02[0] * X_PI);
            return new double[] { z * Math.Cos(theta) + 0.0065, z * Math.Sin(theta) + 0.006 };
        }

        /// <summary>
        /// 纬度转换辅助函数
        /// </summary>
        private static double TransformLat(double lon, double lat)
        {
            double ret = -100.0 + 2.0 * lon + 3.0 * lat + 0.2 * lat * lat + 0.1 * lon * lat + 0.2 * Math.Sqrt(Math.Abs(lon));
            ret += (20.0 * Math.Sin(6.0 * lon * PI) + 20.0 * Math.Sin(2.0 * lon * PI)) * 2.0 / 3.0;
            ret += (20.0 * Math.Sin(lat * PI) + 40.0 * Math.Sin(lat / 3.0 * PI)) * 2.0 / 3.0;
            ret += (160.0 * Math.Sin(lat / 12.0 * PI) + 320.0 * Math.Sin(lat * PI / 30.0)) * 2.0 / 3.0;
            return ret;
        }

        /// <summary>
        /// 经度转换辅助函数
        /// </summary>
        private static double TransformLon(double lon, double lat)
        {
            double ret = 300.0 + lon + 2.0 * lat + 0.1 * lon * lon + 0.1 * lon * lat + 0.1 * Math.Sqrt(Math.Abs(lon));
            ret += (20.0 * Math.Sin(6.0 * lon * PI) + 20.0 * Math.Sin(2.0 * lon * PI)) * 2.0 / 3.0;
            ret += (20.0 * Math.Sin(lon * PI) + 40.0 * Math.Sin(lon / 3.0 * PI)) * 2.0 / 3.0;
            ret += (150.0 * Math.Sin(lon / 12.0 * PI) + 300.0 * Math.Sin(lon / 30.0 * PI)) * 2.0 / 3.0;
            return ret;
        }

        /// <summary>
        /// 计算多边形面积（平方米）
        /// </summary>
        /// <param name="coordinates">多边形顶点坐标列表，每个坐标为 [经度, 纬度]</param>
        /// <returns>多边形面积（平方米）</returns>
        public static double CalculatePolygonArea(IList<double[]> coordinates)
        {
            if (coordinates == null || coordinates.Count < 3)
                return 0;

            double area = 0;
            int count = coordinates.Count;
            for (int i = 0; i < count - 1; i++)
            {
                double lon1 = coordinates[i][0];
                double lat1 = coordinates[i][1];
                double lon2 = coordinates[i + 1][0];
                double lat2 = coordinates[i + 1][1];

                area += (ToRadian(lon2) - ToRadian(lon1)) * (2 + Math.Sin(ToRadian(lat1)) + Math.Sin(ToRadian(lat2)));
            }
            area = area * A * A / 2;

            return Math.Abs(area);
        }

        /// <summary>
        /// 将角度转换为弧度
        /// </summary>
        private static double ToRadian(double degree)
        {
            return degree * PI / 180.0;
        }

        /// <summary>
        /// 使用Haversine公式计算两个经纬度点之间的球面距离（单位：米）
        /// </summary>
        /// <param name="lat1">第一个点的纬度</param>
        /// <param name="lon1">第一个点的经度</param>
        /// <param name="lat2">第二个点的纬度</param>
        /// <param name="lon2">第二个点的经度</param>
        /// <returns>两点之间的球面距离（米）</returns>
        /// <remarks>
        /// Haversine公式假设地球是一个完美的球体，半径约为6371公里。
        /// 对于大多数应用场景，这个精度已经足够。
        /// </remarks>
        /// <example>
        /// <code>
        /// // 计算北京到上海的距离（约1069公里）
        /// double distance = CoordinateTransform.CalculateDistance(39.9042, 116.4074, 31.2304, 121.4737);
        /// // distance ≈ 1069000 米
        /// </code>
        /// </example>
        public static double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            double dLat = ToRadian(lat2 - lat1);
            double dLon = ToRadian(lon2 - lon1);

            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                       Math.Cos(ToRadian(lat1)) * Math.Cos(ToRadian(lat2)) *
                       Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            return A * c;
        }

        /// <summary>
        /// 使用Haversine公式计算两个经纬度点之间的球面距离（单位：米）
        /// </summary>
        /// <param name="point1">第一个点的坐标 [纬度, 经度]</param>
        /// <param name="point2">第二个点的坐标 [纬度, 经度]</param>
        /// <returns>两点之间的球面距离（米）</returns>
        public static double CalculateDistance(double[] point1, double[] point2)
        {
            Guard.NotNull(nameof(point1), point1);
            Guard.NotNull(nameof(point2), point2);

            if (point1.Length < 2 || point2.Length < 2)
                throw new ArgumentException("坐标数组必须至少包含纬度和经度");

            return CalculateDistance(point1[0], point1[1], point2[0], point2[1]);
        }

        /// <summary>
        /// 使用Haversine公式计算两个经纬度点之间的球面距离（单位：指定单位）
        /// </summary>
        /// <param name="lat1">第一个点的纬度</param>
        /// <param name="lon1">第一个点的经度</param>
        /// <param name="lat2">第二个点的纬度</param>
        /// <param name="lon2">第二个点的经度</param>
        /// <param name="unit">距离单位</param>
        /// <returns>两点之间的距离（指定单位）</returns>
        public static double CalculateDistance(double lat1, double lon1, double lat2, double lon2, DistanceUnit unit)
        {
            double meters = CalculateDistance(lat1, lon1, lat2, lon2);
            return ConvertDistance(meters, unit);
        }

        /// <summary>
        /// 将距离从米转换为指定单位
        /// </summary>
        /// <param name="meters">距离（米）</param>
        /// <param name="unit">目标单位</param>
        /// <returns>转换后的距离</returns>
        public static double ConvertDistance(double meters, DistanceUnit unit)
        {
            switch (unit)
            {
                case DistanceUnit.Meters:
                    return meters;
                case DistanceUnit.Kilometers:
                    return meters / 1000.0;
                case DistanceUnit.Miles:
                    return meters / 1609.344;
                case DistanceUnit.NauticalMiles:
                    return meters / 1852.0;
                default:
                    throw new ArgumentOutOfRangeException(nameof(unit), unit, null);
            }
        }
    }

    /// <summary>
    /// 距离单位枚举
    /// </summary>
    public enum DistanceUnit
    {
        /// <summary>
        /// 米
        /// </summary>
        Meters = 0,

        /// <summary>
        /// 千米/公里
        /// </summary>
        Kilometers = 1,

        /// <summary>
        /// 英里
        /// </summary>
        Miles = 2,

        /// <summary>
        /// 海里
        /// </summary>
        NauticalMiles = 3
    }
}
