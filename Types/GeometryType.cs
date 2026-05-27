namespace Allycs.Common
{
    /// <summary>
    /// 几何类型枚举，定义支持的几何对象类型
    /// </summary>
    public enum GeometryType : int
    {
        /// <summary>
        /// 点
        /// </summary>
        Point = 0,

        /// <summary>
        /// 多个点
        /// </summary>
        MultiPoint = 1,

        /// <summary>
        /// 线
        /// </summary>
        LineString = 2,

        /// <summary>
        /// 环形线
        /// </summary>
        LinearRing = 3,

        /// <summary>
        /// 多线
        /// </summary>
        MultiLineString = 4,

        /// <summary>
        /// 多边形
        /// </summary>
        Polygon = 5,

        /// <summary>
        /// 多多边形
        /// </summary>
        MultiPolygon = 6,

        /// <summary>
        /// 多边形集合
        /// </summary>
        GeometryCollection = 7,

        /// <summary>
        /// 圆
        /// </summary>
        Circle = 8
    }
}
