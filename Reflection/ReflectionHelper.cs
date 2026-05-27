namespace Allycs.Common.Reflection
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// 反射工具类，提供常用的反射操作功能
    /// </summary>
    public static class ReflectionHelper
    {
        /// <summary>
        /// 获取对象的属性值
        /// </summary>
        /// <param name="obj">对象</param>
        /// <param name="propertyName">属性名</param>
        /// <returns>属性值</returns>
        public static object GetPropertyValue(object obj, string propertyName)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));

            var property = obj.GetType().GetProperty(propertyName);
            if (property == null)
                throw new ArgumentException($"属性 {propertyName} 不存在", nameof(propertyName));

            return property.GetValue(obj, null);
        }

        /// <summary>
        /// 设置对象的属性值
        /// </summary>
        /// <param name="obj">对象</param>
        /// <param name="propertyName">属性名</param>
        /// <param name="value">属性值</param>
        public static void SetPropertyValue(object obj, string propertyName, object value)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));

            var property = obj.GetType().GetProperty(propertyName);
            if (property == null)
                throw new ArgumentException($"属性 {propertyName} 不存在", nameof(propertyName));

            if (!property.CanWrite)
                throw new ArgumentException($"属性 {propertyName} 不可写", nameof(propertyName));

            property.SetValue(obj, value, null);
        }

        /// <summary>
        /// 获取对象的所有公共属性
        /// </summary>
        /// <param name="obj">对象</param>
        /// <returns>属性信息列表</returns>
        public static PropertyInfo[] GetPublicProperties(object obj)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));

            return obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
        }

        /// <summary>
        /// 获取类型的所有公共属性
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns>属性信息列表</returns>
        public static PropertyInfo[] GetPublicProperties(Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            return type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
        }

        /// <summary>
        /// 检查类型是否具有指定属性
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="propertyName">属性名</param>
        /// <returns>如果具有该属性返回true</returns>
        public static bool HasProperty(Type type, string propertyName)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            return type.GetProperty(propertyName) != null;
        }

        /// <summary>
        /// 调用对象的方法
        /// </summary>
        /// <param name="obj">对象</param>
        /// <param name="methodName">方法名</param>
        /// <param name="parameters">方法参数</param>
        /// <returns>方法返回值</returns>
        public static object InvokeMethod(object obj, string methodName, params object[] parameters)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));

            var method = obj.GetType().GetMethod(methodName);
            if (method == null)
                throw new ArgumentException($"方法 {methodName} 不存在", nameof(methodName));

            return method.Invoke(obj, parameters);
        }

        /// <summary>
        /// 创建类型的实例
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="args">构造函数参数</param>
        /// <returns>实例</returns>
        public static T CreateInstance<T>(params object[] args)
        {
            return (T)Activator.CreateInstance(typeof(T), args);
        }

        /// <summary>
        /// 创建类型的实例
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="args">构造函数参数</param>
        /// <returns>实例</returns>
        public static object CreateInstance(Type type, params object[] args)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            return Activator.CreateInstance(type, args);
        }

        /// <summary>
        /// 检查对象是否具有指定的特性
        /// </summary>
        /// <typeparam name="TAttribute">特性类型</typeparam>
        /// <param name="obj">对象</param>
        /// <returns>如果具有该特性返回true</returns>
        public static bool HasAttribute<TAttribute>(object obj) where TAttribute : Attribute
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));

            return obj.GetType().GetCustomAttributes(typeof(TAttribute), false).Any();
        }

        /// <summary>
        /// 检查类型是否具有指定的特性
        /// </summary>
        /// <typeparam name="TAttribute">特性类型</typeparam>
        /// <param name="type">类型</param>
        /// <returns>如果具有该特性返回true</returns>
        public static bool HasAttribute<TAttribute>(Type type) where TAttribute : Attribute
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            return type.GetCustomAttributes(typeof(TAttribute), false).Any();
        }

        /// <summary>
        /// 获取对象的特性
        /// </summary>
        /// <typeparam name="TAttribute">特性类型</typeparam>
        /// <param name="obj">对象</param>
        /// <returns>特性实例</returns>
        public static TAttribute GetAttribute<TAttribute>(object obj) where TAttribute : Attribute
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));

            return obj.GetType().GetCustomAttributes(typeof(TAttribute), false)
                      .OfType<TAttribute>()
                      .FirstOrDefault();
        }

        /// <summary>
        /// 获取类型的特性
        /// </summary>
        /// <typeparam name="TAttribute">特性类型</typeparam>
        /// <param name="type">类型</param>
        /// <returns>特性实例</returns>
        public static TAttribute GetAttribute<TAttribute>(Type type) where TAttribute : Attribute
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            return type.GetCustomAttributes(typeof(TAttribute), false)
                      .OfType<TAttribute>()
                      .FirstOrDefault();
        }

        /// <summary>
        /// 获取对象的所有特性
        /// </summary>
        /// <param name="obj">对象</param>
        /// <returns>特性列表</returns>
        public static IEnumerable<Attribute> GetAttributes(object obj)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));

            return obj.GetType().GetCustomAttributes(false).OfType<Attribute>();
        }

        /// <summary>
        /// 获取类型的所有特性
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns>特性列表</returns>
        public static IEnumerable<Attribute> GetAttributes(Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            return type.GetCustomAttributes(false).OfType<Attribute>();
        }

        /// <summary>
        /// 检查类型是否实现了指定的接口
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="interfaceType">接口类型</param>
        /// <returns>如果实现了该接口返回true</returns>
        public static bool ImplementsInterface(Type type, Type interfaceType)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            if (interfaceType == null)
                throw new ArgumentNullException(nameof(interfaceType));

            return type.GetInterfaces().Any(i => i == interfaceType);
        }

        /// <summary>
        /// 检查类型是否继承自指定的基类
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="baseType">基类类型</param>
        /// <returns>如果继承自该基类返回true</returns>
        public static bool InheritsFrom(Type type, Type baseType)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            if (baseType == null)
                throw new ArgumentNullException(nameof(baseType));

            return baseType.IsAssignableFrom(type) && type != baseType;
        }

        /// <summary>
        /// 获取类型的所有公共字段
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns>字段信息列表</returns>
        public static FieldInfo[] GetPublicFields(Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            return type.GetFields(BindingFlags.Public | BindingFlags.Instance);
        }

        /// <summary>
        /// 深度复制对象（浅拷贝）
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="obj">源对象</param>
        /// <returns>拷贝对象</returns>
        public static T ShallowCopy<T>(T obj) where T : class, new()
        {
            if (obj == null)
                return null;

            var copy = new T();
            var properties = GetPublicProperties(typeof(T));

            foreach (var property in properties)
            {
                if (property.CanRead && property.CanWrite)
                {
                    var value = property.GetValue(obj, null);
                    property.SetValue(copy, value, null);
                }
            }

            return copy;
        }

        /// <summary>
        /// 将对象转换为字典
        /// </summary>
        /// <param name="obj">对象</param>
        /// <returns>属性字典</returns>
        public static Dictionary<string, object> ToDictionary(object obj)
        {
            var dict = new Dictionary<string, object>();

            if (obj == null)
                return dict;

            var properties = GetPublicProperties(obj);

            foreach (var property in properties)
            {
                if (property.CanRead)
                {
                    var value = property.GetValue(obj, null);
                    dict[property.Name] = value;
                }
            }

            return dict;
        }

        /// <summary>
        /// 从字典创建对象
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="dict">字典</param>
        /// <returns>对象实例</returns>
        public static T FromDictionary<T>(Dictionary<string, object> dict) where T : class, new()
        {
            var obj = new T();
            var properties = GetPublicProperties(typeof(T));

            foreach (var property in properties)
            {
                if (property.CanWrite && dict.ContainsKey(property.Name))
                {
                    try
                    {
                        var value = dict[property.Name];
                        if (value != null && value.GetType() != property.PropertyType)
                        {
                            value = Convert.ChangeType(value, property.PropertyType);
                        }
                        property.SetValue(obj, value, null);
                    }
                    catch
                    {
                        // 转换失败时跳过
                    }
                }
            }

            return obj;
        }
    }
}
