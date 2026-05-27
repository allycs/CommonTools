namespace Allycs.Common
{
    using System;

    /// <summary>
    /// 数学计算工具类，提供常用的数学运算辅助方法
    /// </summary>
    public static class MathHelper
    {
        /// <summary>
        /// 计算两个角度之间的最小差值
        /// </summary>
        /// <param name="a">第一个角度（度）</param>
        /// <param name="b">第二个角度（度）</param>
        /// <returns>两个角度之间的最小差值（度），范围为0-180</returns>
        /// <remarks>
        /// 例如：AngleDiff(10, 350) 返回 20，因为两者相差20度
        /// </remarks>
        public static double AngleDiff(double a, double b)
        {
            double diff = Math.Abs(a - b) % 360;
            return diff > 180 ? 360 - diff : diff;
        }

        /// <summary>
        /// 将角度转换为弧度
        /// </summary>
        /// <param name="degrees">角度值</param>
        /// <returns>对应的弧度值</returns>
        public static double DegreesToRadians(double degrees)
        {
            return degrees * Math.PI / 180.0;
        }

        /// <summary>
        /// 将弧度转换为角度
        /// </summary>
        /// <param name="radians">弧度值</param>
        /// <returns>对应的角度值</returns>
        public static double RadiansToDegrees(double radians)
        {
            return radians * 180.0 / Math.PI;
        }

        /// <summary>
        /// 将值限制在指定范围内
        /// </summary>
        /// <typeparam name="T">可比较的类型</typeparam>
        /// <param name="value">要限制的值</param>
        /// <param name="min">最小值</param>
        /// <param name="max">最大值</param>
        /// <returns>限制后的值</returns>
        public static T Clamp<T>(T value, T min, T max) where T : IComparable<T>
        {
            if (value.CompareTo(min) < 0)
                return min;
            if (value.CompareTo(max) > 0)
                return max;
            return value;
        }

        /// <summary>
        /// 判断数值是否在指定范围内（包含边界）
        /// </summary>
        /// <param name="value">要检查的值</param>
        /// <param name="min">最小值</param>
        /// <param name="max">最大值</param>
        /// <returns>如果在范围内返回true，否则返回false</returns>
        public static bool InRange(double value, double min, double max)
        {
            return value >= min && value <= max;
        }

        /// <summary>
        /// 计算两点之间的欧几里得距离
        /// </summary>
        /// <param name="x1">第一个点的X坐标</param>
        /// <param name="y1">第一个点的Y坐标</param>
        /// <param name="x2">第二个点的X坐标</param>
        /// <param name="y2">第二个点的Y坐标</param>
        /// <returns>两点之间的欧几里得距离</returns>
        public static double Distance(double x1, double y1, double x2, double y2)
        {
            double dx = x2 - x1;
            double dy = y2 - y1;
            return Math.Sqrt(dx * dx + dy * dy);
        }

        /// <summary>
        /// 四舍五入到指定小数位数
        /// </summary>
        /// <param name="value">要四舍五入的值</param>
        /// <param name="decimals">小数位数</param>
        /// <returns>四舍五入后的值</returns>
        public static double Round(double value, int decimals)
        {
            return Math.Round(value, decimals);
        }
    }
}
