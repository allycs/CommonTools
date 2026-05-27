namespace Allycs.Common
{
    using NetTopologySuite.Geometries;

    /// <summary>
    /// 表示一个地理矩形的类，通过两个对角点定义
    /// </summary>
    public class AllycsGeoRectangle
    {
        /// <summary>
        /// 矩形的右上角坐标 (X=经度, Y=纬度)
        /// </summary>
        public Coordinate RightTop { get; set; }

        /// <summary>
        /// 矩形的左下角坐标 (X=经度, Y=纬度)
        /// </summary>
        public Coordinate LeftBottom { get; set; }

        /// <summary>
        /// 构造函数，初始化矩形的左上角和右下角坐标
        /// </summary>
        /// <param name="leftBottom">左下角坐标点</param>
        /// <param name="rightTop">右上角坐标点</param>
        public AllycsGeoRectangle(WGS84Point leftBottom, WGS84Point rightTop)
        {
            RightTop = new Coordinate(rightTop.Longitude.Value, rightTop.Latitude.Value);
            LeftBottom = new Coordinate(leftBottom.Longitude.Value, leftBottom.Latitude.Value);
        }

        /// <summary>
        /// 将矩形转换为 Envelope 对象
        /// </summary>
        /// <returns>表示矩形的 Envelope 对象</returns>
        public Envelope ToEnvelope()
        {
            return new Envelope(LeftBottom.X, RightTop.X, LeftBottom.Y, RightTop.Y);
        }

        /// <summary>
        /// 将矩形转换为几何对象
        /// </summary>
        /// <returns>表示矩形的几何对象</returns>
        public Geometry ToGeometry()
        {
            var geometryFactory = new GeometryFactory(new PrecisionModel(), 4326);
            return geometryFactory.ToGeometry(ToEnvelope());
        }
    }

    /// <summary>
    /// WGS84坐标点，用于表示经纬度坐标
    /// </summary>
    public class WGS84Point
    {
        /// <summary>
        /// 纬度
        /// </summary>
        public double? Latitude { get; set; }

        /// <summary>
        /// 经度
        /// </summary>
        public double? Longitude { get; set; }

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public WGS84Point() { }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="latitude">纬度</param>
        /// <param name="longitude">经度</param>
        public WGS84Point(double latitude, double longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }
    }
}
