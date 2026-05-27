namespace Allycs.Common.Collections
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// 集合工具类，提供常用的集合操作功能
    /// </summary>
    public static class CollectionHelper
    {
        /// <summary>
        /// 判断集合是否为null或空
        /// </summary>
        /// <typeparam name="T">集合元素类型</typeparam>
        /// <param name="source">源集合</param>
        /// <returns>如果集合为null或空返回true</returns>
        public static bool IsNullOrEmpty<T>(IEnumerable<T> source)
        {
            return source == null || !source.Any();
        }

        /// <summary>
        /// 判断集合不为null且不为空
        /// </summary>
        /// <typeparam name="T">集合元素类型</typeparam>
        /// <param name="source">源集合</param>
        /// <returns>如果集合不为null且不为空返回true</returns>
        public static bool IsNotNullOrEmpty<T>(IEnumerable<T> source)
        {
            return source != null && source.Any();
        }

        /// <summary>
        /// 对集合进行分页
        /// </summary>
        /// <typeparam name="T">集合元素类型</typeparam>
        /// <param name="source">源集合</param>
        /// <param name="pageIndex">页码，从1开始</param>
        /// <param name="pageSize">每页大小</param>
        /// <returns>分页后的集合</returns>
        public static IEnumerable<T> Page<T>(this IEnumerable<T> source, int pageIndex, int pageSize)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            if (pageIndex < 1)
                throw new ArgumentOutOfRangeException(nameof(pageIndex), "页码必须大于等于1");

            if (pageSize < 1)
                throw new ArgumentOutOfRangeException(nameof(pageSize), "每页大小必须大于等于1");

            return source.Skip((pageIndex - 1) * pageSize).Take(pageSize);
        }

        /// <summary>
        /// 将集合转换为HashSet
        /// </summary>
        /// <typeparam name="T">集合元素类型</typeparam>
        /// <param name="source">源集合</param>
        /// <returns>HashSet</returns>
        public static HashSet<T> ToHashSet<T>(IEnumerable<T> source)
        {
            return new HashSet<T>(source);
        }

        /// <summary>
        /// 批量添加元素到集合
        /// </summary>
        /// <typeparam name="T">集合元素类型</typeparam>
        /// <param name="collection">目标集合</param>
        /// <param name="items">要添加的元素</param>
        public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> items)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));

            if (items == null)
                return;

            foreach (var item in items)
            {
                collection.Add(item);
            }
        }

        /// <summary>
        /// 获取集合的所有组合
        /// </summary>
        /// <typeparam name="T">集合元素类型</typeparam>
        /// <param name="source">源集合</param>
        /// <returns>所有组合</returns>
        public static IEnumerable<IEnumerable<T>> Combinations<T>(this IEnumerable<T> source)
        {
            var list = source.ToList();
            int count = list.Count;

            for (int mask = 1; mask < (1 << count); mask++)
            {
                var combination = new List<T>();
                for (int i = 0; i < count; i++)
                {
                    if ((mask & (1 << i)) != 0)
                    {
                        combination.Add(list[i]);
                    }
                }
                yield return combination;
            }
        }

        /// <summary>
        /// 查找集合中重复的元素
        /// </summary>
        /// <typeparam name="T">集合元素类型</typeparam>
        /// <param name="source">源集合</param>
        /// <returns>重复的元素列表</returns>
        public static IEnumerable<T> FindDuplicates<T>(this IEnumerable<T> source)
        {
            return source.GroupBy(x => x)
                        .Where(g => g.Count() > 1)
                        .Select(g => g.Key);
        }

        /// <summary>
        /// 查找集合中重复的元素（带选择器）
        /// </summary>
        /// <typeparam name="T">集合元素类型</typeparam>
        /// <typeparam name="TKey">键类型</typeparam>
        /// <param name="source">源集合</param>
        /// <param name="keySelector">键选择器</param>
        /// <returns>重复的元素列表</returns>
        public static IEnumerable<T> FindDuplicates<T, TKey>(this IEnumerable<T> source, Func<T, TKey> keySelector)
        {
            return source.GroupBy(keySelector)
                        .Where(g => g.Count() > 1)
                        .SelectMany(g => g);
        }

        /// <summary>
        /// 打乱集合顺序
        /// </summary>
        /// <typeparam name="T">集合元素类型</typeparam>
        /// <param name="source">源集合</param>
        /// <returns>打乱后的集合</returns>
        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source)
        {
            var list = source.ToList();
            var random = new Random();

            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = random.Next(i + 1);
                var temp = list[i];
                list[i] = list[j];
                list[j] = temp;
            }

            return list;
        }

        /// <summary>
        /// 获取集合的随机元素
        /// </summary>
        /// <typeparam name="T">集合元素类型</typeparam>
        /// <param name="source">源集合</param>
        /// <returns>随机元素</returns>
        public static T Random<T>(this IEnumerable<T> source)
        {
            var list = source.ToList();
            if (list.Count == 0)
                throw new InvalidOperationException("集合不能为空");

            var random = new Random();
            return list[random.Next(list.Count)];
        }

        /// <summary>
        /// 获取集合的随机元素（如果为空返回默认值）
        /// </summary>
        /// <typeparam name="T">集合元素类型</typeparam>
        /// <param name="source">源集合</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>随机元素或默认值</returns>
        public static T RandomOrDefault<T>(this IEnumerable<T> source, T defaultValue = default(T))
        {
            var list = source.ToList();
            if (list.Count == 0)
                return defaultValue;

            var random = new Random();
            return list[random.Next(list.Count)];
        }

        /// <summary>
        /// 分割集合为指定大小的多个子集合
        /// </summary>
        /// <typeparam name="T">集合元素类型</typeparam>
        /// <param name="source">源集合</param>
        /// <param name="size">每个子集合的大小</param>
        /// <returns>子集合列表</returns>
        public static IEnumerable<IEnumerable<T>> Chunk<T>(this IEnumerable<T> source, int size)
        {
            if (size < 1)
                throw new ArgumentOutOfRangeException(nameof(size));

            var list = new List<T>();

            foreach (var item in source)
            {
                list.Add(item);
                if (list.Count == size)
                {
                    yield return list;
                    list = new List<T>();
                }
            }

            if (list.Count > 0)
            {
                yield return list;
            }
        }

        /// <summary>
        /// 判断两个集合是否相等（不考虑顺序）
        /// </summary>
        /// <typeparam name="T">集合元素类型</typeparam>
        /// <param name="first">第一个集合</param>
        /// <param name="second">第二个集合</param>
        /// <returns>如果两个集合相等返回true</returns>
        public static bool ScrambledEquals<T>(this IEnumerable<T> first, IEnumerable<T> second)
        {
            var firstList = first.ToList();
            var secondList = second.ToList();

            if (firstList.Count != secondList.Count)
                return false;

            var counts = new Dictionary<T, int>();

            foreach (var item in firstList)
            {
                if (counts.ContainsKey(item))
                    counts[item]++;
                else
                    counts[item] = 1;
            }

            foreach (var item in secondList)
            {
                if (!counts.ContainsKey(item) || counts[item] == 0)
                    return false;

                counts[item]--;
            }

            return true;
        }

        /// <summary>
        /// 根据指定属性去重
        /// </summary>
        /// <typeparam name="T">集合元素类型</typeparam>
        /// <typeparam name="TKey">键类型</typeparam>
        /// <param name="source">源集合</param>
        /// <param name="keySelector">键选择器</param>
        /// <returns>去重后的集合</returns>
        public static IEnumerable<T> DistinctBy<T, TKey>(this IEnumerable<T> source, Func<T, TKey> keySelector)
        {
            var seenKeys = new HashSet<TKey>();

            foreach (var item in source)
            {
                var key = keySelector(item);
                if (seenKeys.Add(key))
                {
                    yield return item;
                }
            }
        }

        /// <summary>
        /// 交替合并两个集合
        /// </summary>
        /// <typeparam name="T">集合元素类型</typeparam>
        /// <param name="first">第一个集合</param>
        /// <param name="second">第二个集合</param>
        /// <returns>交替合并后的集合</returns>
        public static IEnumerable<T> Interleave<T>(this IEnumerable<T> first, IEnumerable<T> second)
        {
            using (var e1 = first.GetEnumerator())
            using (var e2 = second.GetEnumerator())
            {
                bool e1HasNext, e2HasNext;

                do
                {
                    e1HasNext = e1.MoveNext();
                    e2HasNext = e2.MoveNext();

                    if (e1HasNext)
                        yield return e1.Current;

                    if (e2HasNext)
                        yield return e2.Current;
                }
                while (e1HasNext || e2HasNext);
            }
        }
    }
}
