namespace Allycs.Common
{
    using NetTopologySuite.Geometries;

    /// <summary>
    /// 表示一个地理圆形的类，通过圆心和半径定义
    /// </summary>
    public class AllycsGeoCircle
    {
        /// <summary>
        /// 圆心的经纬度坐标 (X=经度, Y=纬度)
        /// </summary>
        public Coordinate Center { get; set; }

        /// <summary>
        /// 圆的半径（以米为单位）
        /// </summary>
        public double RadiusInMeters { get; set; }

        /// <summary>
        /// 构造函数，初始化圆的中心点和半径
        /// </summary>
        /// <param name="latitude">圆心的纬度</param>
        /// <param name="longitude">圆心的经度</param>
        /// <param name="radiusInMeters">圆的半径（以米为单位）</param>
        public AllycsGeoCircle(double latitude, double longitude, double radiusInMeters)
        {
            Center = new Coordinate(longitude, latitude);
            RadiusInMeters = radiusInMeters;
        }

        /// <summary>
        /// 将圆转换为多边形（近似表示）
        /// </summary>
        /// <param name="segments">圆的细分段数，值越大越精确，默认64</param>
        /// <returns>表示圆的多边形</returns>
        public Polygon ToPolygon(int segments = 64)
        {
            var geometryFactory = new GeometryFactory(new PrecisionModel(), 4326);
            var point = geometryFactory.CreatePoint(Center);
            var distanceDegrees = RadiusInMeters / 111320.0;
            return (Polygon)point.Buffer(distanceDegrees, segments);
        }
    }
}
