namespace Allycs.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// 参数验证工具类，提供各种参数检查功能
    /// </summary>
    public static class Guard
    {
        /// <summary>
        /// 检查对象是否为null，如果为null则抛出ArgumentNullException异常
        /// </summary>
        /// <param name="argumentName">参数名称</param>
        /// <param name="value">要检查的值</param>
        /// <exception cref="ArgumentNullException">当value为null时抛出</exception>
        public static void NotNull(string argumentName, object value)
        {
            if (value == null)
                throw new ArgumentNullException(argumentName);
        }

        /// <summary>
        /// 检查字符串是否为null或空字符串，如果是则抛出相应异常
        /// </summary>
        /// <param name="argumentName">参数名称</param>
        /// <param name="value">要检查的字符串</param>
        /// <exception cref="ArgumentNullException">当value为null时抛出</exception>
        /// <exception cref="ArgumentException">当value为空字符串时抛出</exception>
        public static void NotNullOrEmpty(string argumentName, string value)
        {
            if (value == null)
                throw new ArgumentNullException(argumentName);

            if (value.Length == 0)
                throw new ArgumentException("值不能为空字符串", argumentName);
        }

        /// <summary>
        /// 检查集合是否为null或空集合，如果是则抛出相应异常
        /// </summary>
        /// <typeparam name="T">集合元素类型</typeparam>
        /// <param name="argumentName">参数名称</param>
        /// <param name="collection">要检查的集合</param>
        /// <exception cref="ArgumentNullException">当collection为null时抛出</exception>
        /// <exception cref="ArgumentException">当collection为空集合时抛出</exception>
        public static void NotNullOrEmpty<T>(string argumentName, IReadOnlyCollection<T> collection)
        {
            if (collection == null)
                throw new ArgumentNullException(argumentName);

            if (collection.Count == 0)
                throw new ArgumentException("集合必须至少包含一个元素", argumentName);
        }

        /// <summary>
        /// 检查条件是否满足，如果不满足则抛出ArgumentException异常
        /// </summary>
        /// <param name="condition">要检查的条件</param>
        /// <param name="argumentName">参数名称</param>
        /// <param name="message">异常消息</param>
        /// <exception cref="ArgumentException">当condition为false时抛出</exception>
        public static void IsTrue(bool condition, string argumentName, string message)
        {
            if (!condition)
                throw new ArgumentException(message, argumentName);
        }

        /// <summary>
        /// 检查条件是否满足，如果不满足则抛出InvalidOperationException异常
        /// </summary>
        /// <param name="condition">要检查的条件</param>
        /// <param name="message">异常消息</param>
        /// <exception cref="InvalidOperationException">当condition为false时抛出</exception>
        public static void IsValid(bool condition, string message)
        {
            if (!condition)
                throw new InvalidOperationException(message);
        }

        /// <summary>
        /// 检查值是否在指定范围内
        /// </summary>
        /// <param name="value">要检查的值</param>
        /// <param name="argumentName">参数名称</param>
        /// <param name="min">最小值（包含）</param>
        /// <param name="max">最大值（包含）</param>
        /// <exception cref="ArgumentException">当值不在范围内时抛出</exception>
        public static void InRange(int value, string argumentName, int min, int max)
        {
            if (value < min || value > max)
                throw new ArgumentException($"值必须在{min}和{max}之间", argumentName);
        }

        /// <summary>
        /// 检查字符串是否满足指定长度约束
        /// </summary>
        /// <param name="value">要检查的字符串</param>
        /// <param name="argumentName">参数名称</param>
        /// <param name="maxLength">最大长度</param>
        /// <exception cref="ArgumentException">当字符串长度超过最大值时抛出</exception>
        public static void MaxLength(string value, string argumentName, int maxLength)
        {
            if (value != null && value.Length > maxLength)
                throw new ArgumentException($"字符串长度不能超过{maxLength}个字符", argumentName);
        }

        /// <summary>
        /// 检查类型是否匹配
        /// </summary>
        /// <typeparam name="T">期望的类型</typeparam>
        /// <param name="value">要检查的值</param>
        /// <param name="argumentName">参数名称</param>
        /// <exception cref="ArgumentException">当类型不匹配时抛出</exception>
        public static void TypeOf<T>(object value, string argumentName)
        {
            if (!(value is T))
                throw new ArgumentException($"值必须是{typeof(T).Name}类型", argumentName);
        }
    }
}
