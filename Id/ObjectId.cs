namespace Allycs.Common
{
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Security;
    using System.Text;
    using System.Threading;

    /// <summary>
    /// MongoDB风格的ObjectId结构，提供高性能的分布式唯一ID生成
    /// </summary>
    /// <remarks>
    /// ObjectId是一个12字节的ID，由以下部分组成：
    /// - 4字节：时间戳
    /// - 3字节：机器标识
    /// - 2字节：进程ID
    /// - 3字节：计数器
    /// </remarks>
    public struct ObjectId : IComparable<ObjectId>, IEquatable<ObjectId>
    {
        private static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        private static readonly int StaticMachine;
        private static readonly short StaticPid;
        private static int StaticIncrement;
        private static readonly char[] HexDigits = "0123456789abcdef".ToCharArray();

        private readonly int _timestamp;
        private readonly int _machine;
        private readonly short _pid;
        private readonly int _increment;

        static ObjectId()
        {
            StaticMachine = GetMachineHash();
            StaticIncrement = new Random().Next();
            try
            {
                StaticPid = (short)GetCurrentProcessId();
            }
            catch (SecurityException)
            {
                StaticPid = 0;
            }
        }

        /// <summary>
        /// 创建一个空的ObjectId
        /// </summary>
        public static ObjectId Empty => default;

        /// <summary>
        /// 使用字节数组创建ObjectId
        /// </summary>
        /// <param name="bytes">12字节的数组</param>
        /// <exception cref="ArgumentNullException">当bytes为null时抛出</exception>
        /// <exception cref="ArgumentException">当bytes长度不为12时抛出</exception>
        public ObjectId(byte[] bytes)
        {
            Guard.NotNull(nameof(bytes), bytes);
            if (bytes.Length != 12)
                throw new ArgumentException("字节数组长度必须为12", nameof(bytes));

            Unpack(bytes, out _timestamp, out _machine, out _pid, out _increment);
        }

        /// <summary>
        /// 使用指定的时间戳、机器标识、进程ID和计数器创建ObjectId
        /// </summary>
        public ObjectId(int timestamp, int machine, short pid, int increment)
        {
            if ((machine & 0xff000000) != 0)
                throw new ArgumentOutOfRangeException(nameof(machine), "机器标识必须在0-16777215之间");

            if ((increment & 0xff000000) != 0)
                throw new ArgumentOutOfRangeException(nameof(increment), "计数器必须在0-16777215之间");

            _timestamp = timestamp;
            _machine = machine;
            _pid = pid;
            _increment = increment;
        }

        /// <summary>
        /// 使用十六进制字符串创建ObjectId
        /// </summary>
        /// <param name="value">24位十六进制字符串</param>
        /// <exception cref="ArgumentNullException">当value为null时抛出</exception>
        /// <exception cref="FormatException">当格式不正确时抛出</exception>
        public ObjectId(string value)
        {
            Guard.NotNull(nameof(value), value);
            if (value.Length != 24)
                throw new ArgumentException("字符串长度必须为24", nameof(value));
            Unpack(ParseHexString(value), out _timestamp, out _machine, out _pid, out _increment);
        }

        /// <summary>
        /// 获取ObjectId的时间戳部分
        /// </summary>
        public int Timestamp => _timestamp;

        /// <summary>
        /// 获取ObjectId的机器标识部分
        /// </summary>
        public int Machine => _machine;

        /// <summary>
        /// 获取ObjectId的进程ID部分
        /// </summary>
        public short Pid => _pid;

        /// <summary>
        /// 获取ObjectId的计数器部分
        /// </summary>
        public int Increment => _increment;

        /// <summary>
        /// 获取ObjectId的创建时间
        /// </summary>
        public DateTime CreationTime => UnixEpoch.AddSeconds(_timestamp);

        /// <summary>
        /// 生成一个新的ObjectId
        /// </summary>
        /// <returns>新的ObjectId</returns>
        public static ObjectId NewId()
        {
            return NewId(GetTimestampFromDateTime(DateTime.UtcNow));
        }

        /// <summary>
        /// 使用指定时间生成ObjectId
        /// </summary>
        /// <param name="timestamp">时间戳</param>
        /// <returns>新的ObjectId</returns>
        public static ObjectId NewId(int timestamp)
        {
            int increment = Interlocked.Increment(ref StaticIncrement) & 0x00ffffff;
            return new ObjectId(timestamp, StaticMachine, StaticPid, increment);
        }

        /// <summary>
        /// 解析十六进制字符串为ObjectId
        /// </summary>
        /// <param name="s">24位十六进制字符串</param>
        /// <returns>ObjectId实例</returns>
        public static ObjectId Parse(string s)
        {
            Guard.NotNull(nameof(s), s);

            if (TryParse(s, out var objectId))
                return objectId;

            throw new FormatException($"'{s}' 不是有效的24位十六进制字符串");
        }

        /// <summary>
        /// 尝试解析十六进制字符串为ObjectId
        /// </summary>
        /// <param name="s">十六进制字符串</param>
        /// <param name="objectId">输出ObjectId</param>
        /// <returns>如果解析成功返回true，否则返回false</returns>
        public static bool TryParse(string s, out ObjectId objectId)
        {
            if (s != null && s.Length == 24 && TryParseHexString(s, out var bytes))
            {
                objectId = new ObjectId(bytes);
                return true;
            }

            objectId = default;
            return false;
        }

        /// <summary>
        /// 将ObjectId转换为字节数组
        /// </summary>
        /// <returns>12字节的数组</returns>
        public byte[] ToByteArray()
        {
            return Pack(_timestamp, _machine, _pid, _increment);
        }

        /// <summary>
        /// 获取ObjectId的字符串表示
        /// </summary>
        /// <returns>24位十六进制字符串</returns>
        public override string ToString()
        {
            return ToHexString(Pack(_timestamp, _machine, _pid, _increment));
        }

        /// <summary>
        /// 比较两个ObjectId是否相等
        /// </summary>
        /// <param name="left">左侧ObjectId</param>
        /// <param name="right">右侧ObjectId</param>
        /// <returns>如果相等返回true，否则返回false</returns>
        public static bool operator ==(ObjectId left, ObjectId right) => left.Equals(right);

        /// <summary>
        /// 比较两个ObjectId是否不相等
        /// </summary>
        /// <param name="left">左侧ObjectId</param>
        /// <param name="right">右侧ObjectId</param>
        /// <returns>如果不相等返回true，否则返回false</returns>
        public static bool operator !=(ObjectId left, ObjectId right) => !left.Equals(right);

        /// <summary>
        /// 判断左侧ObjectId是否小于右侧ObjectId
        /// </summary>
        /// <param name="left">左侧ObjectId</param>
        /// <param name="right">右侧ObjectId</param>
        /// <returns>如果左侧小于右侧返回true</returns>
        public static bool operator <(ObjectId left, ObjectId right) => left.CompareTo(right) < 0;

        /// <summary>
        /// 判断左侧ObjectId是否大于右侧ObjectId
        /// </summary>
        /// <param name="left">左侧ObjectId</param>
        /// <param name="right">右侧ObjectId</param>
        /// <returns>如果左侧大于右侧返回true</returns>
        public static bool operator >(ObjectId left, ObjectId right) => left.CompareTo(right) > 0;

        /// <summary>
        /// 判断左侧ObjectId是否小于或等于右侧ObjectId
        /// </summary>
        /// <param name="left">左侧ObjectId</param>
        /// <param name="right">右侧ObjectId</param>
        /// <returns>如果左侧小于或等于右侧返回true</returns>
        public static bool operator <=(ObjectId left, ObjectId right) => left.CompareTo(right) <= 0;

        /// <summary>
        /// 判断左侧ObjectId是否大于或等于右侧ObjectId
        /// </summary>
        /// <param name="left">左侧ObjectId</param>
        /// <param name="right">右侧ObjectId</param>
        /// <returns>如果左侧大于或等于右侧返回true</returns>
        public static bool operator >=(ObjectId left, ObjectId right) => left.CompareTo(right) >= 0;

        /// <summary>
        /// 比较当前ObjectId与另一个ObjectId
        /// </summary>
        /// <param name="other">要比较的ObjectId</param>
        /// <returns>负数表示当前小于other，零表示相等，正数表示当前大于other</returns>
        public int CompareTo(ObjectId other)
        {
            int result = _timestamp.CompareTo(other._timestamp);
            if (result != 0) return result;

            result = _machine.CompareTo(other._machine);
            if (result != 0) return result;

            result = _pid.CompareTo(other._pid);
            if (result != 0) return result;

            return _increment.CompareTo(other._increment);
        }

        /// <summary>
        /// 判断当前ObjectId是否等于另一个ObjectId
        /// </summary>
        /// <param name="other">要比较的ObjectId</param>
        /// <returns>如果相等返回true，否则返回false</returns>
        public bool Equals(ObjectId other)
        {
            return _timestamp == other._timestamp &&
                   _machine == other._machine &&
                   _pid == other._pid &&
                   _increment == other._increment;
        }

        /// <summary>
        /// 判断当前ObjectId是否等于指定对象
        /// </summary>
        /// <param name="obj">要比较的对象</param>
        /// <returns>如果相等返回true，否则返回false</returns>
        public override bool Equals(object obj)
        {
            return obj is ObjectId other && Equals(other);
        }

        /// <summary>
        /// 获取ObjectId的哈希码
        /// </summary>
        /// <returns>哈希码值</returns>
        public override int GetHashCode()
        {
            int hash = 17;
            hash = hash * 37 + _timestamp.GetHashCode();
            hash = hash * 37 + _machine.GetHashCode();
            hash = hash * 37 + _pid.GetHashCode();
            hash = hash * 37 + _increment.GetHashCode();
            return hash;
        }

        /// <summary>
        /// 将ObjectId隐式转换为字符串
        /// </summary>
        /// <param name="objectId">要转换的ObjectId</param>
        /// <returns>24位十六进制字符串</returns>
        public static implicit operator string(ObjectId objectId) => objectId.ToString();

        /// <summary>
        /// 将字符串隐式转换为ObjectId
        /// </summary>
        /// <param name="value">24位十六进制字符串</param>
        /// <returns>对应的ObjectId，如果字符串为空则返回Empty</returns>
        public static implicit operator ObjectId(string value) => string.IsNullOrEmpty(value) ? Empty : new ObjectId(value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static byte[] Pack(int timestamp, int machine, short pid, int increment)
        {
            var bytes = new byte[12];
            bytes[0] = (byte)(timestamp >> 24);
            bytes[1] = (byte)(timestamp >> 16);
            bytes[2] = (byte)(timestamp >> 8);
            bytes[3] = (byte)timestamp;
            bytes[4] = (byte)(machine >> 16);
            bytes[5] = (byte)(machine >> 8);
            bytes[6] = (byte)machine;
            bytes[7] = (byte)(pid >> 8);
            bytes[8] = (byte)pid;
            bytes[9] = (byte)(increment >> 16);
            bytes[10] = (byte)(increment >> 8);
            bytes[11] = (byte)increment;
            return bytes;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Unpack(byte[] bytes, out int timestamp, out int machine, out short pid, out int increment)
        {
            timestamp = (bytes[0] << 24) | (bytes[1] << 16) | (bytes[2] << 8) | bytes[3];
            machine = (bytes[4] << 16) | (bytes[5] << 8) | bytes[6];
            pid = (short)((bytes[7] << 8) | bytes[8]);
            increment = (bytes[9] << 16) | (bytes[10] << 8) | bytes[11];
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static int GetCurrentProcessId() => Process.GetCurrentProcess().Id;

        private static int GetMachineHash()
        {
            var hostName = Environment.MachineName;
            return 0x00ffffff & hostName.GetHashCode();
        }

        private static int GetTimestampFromDateTime(DateTime timestamp)
        {
            var secondsSinceEpoch = (long)(ToUniversalTime(timestamp) - UnixEpoch).TotalSeconds;
            if (secondsSinceEpoch < int.MinValue || secondsSinceEpoch > int.MaxValue)
                throw new ArgumentOutOfRangeException(nameof(timestamp));
            return (int)secondsSinceEpoch;
        }

        private static DateTime ToUniversalTime(DateTime dateTime)
        {
            if (dateTime == DateTime.MinValue)
                return DateTime.SpecifyKind(DateTime.MinValue, DateTimeKind.Utc);
            if (dateTime == DateTime.MaxValue)
                return DateTime.SpecifyKind(DateTime.MaxValue, DateTimeKind.Utc);
            return dateTime.ToUniversalTime();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int HexValue(char c)
        {
            if (c >= '0' && c <= '9')
                return c - '0';
            if (c >= 'a' && c <= 'f')
                return c - 'a' + 10;
            if (c >= 'A' && c <= 'F')
                return c - 'A' + 10;
            throw new ArgumentException("Invalid hex character");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool TryHexValue(char c, out int value)
        {
            if (c >= '0' && c <= '9')
            {
                value = c - '0';
                return true;
            }
            if (c >= 'a' && c <= 'f')
            {
                value = c - 'a' + 10;
                return true;
            }
            if (c >= 'A' && c <= 'F')
            {
                value = c - 'A' + 10;
                return true;
            }
            value = 0;
            return false;
        }

        private static byte[] ParseHexString(string s)
        {
            Guard.NotNull(nameof(s), s);

            if (s.Length != 24)
                throw new ArgumentException("字符串长度必须为24", nameof(s));

            var bytes = new byte[12];
            for (int i = 0; i < 12; i++)
            {
                int hi = HexValue(s[i * 2]);
                int lo = HexValue(s[i * 2 + 1]);
                bytes[i] = (byte)((hi << 4) | lo);
            }
            return bytes;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static string ToHexString(byte[] bytes)
        {
            Guard.NotNull(nameof(bytes), bytes);
            var chars = new char[bytes.Length * 2];
            for (int i = 0; i < bytes.Length; i++)
            {
                int b = bytes[i];
                chars[i * 2] = HexDigits[b >> 4];
                chars[i * 2 + 1] = HexDigits[b & 0x0F];
            }
            return new string(chars);
        }

        private static bool TryParseHexString(string s, out byte[] bytes)
        {
            if (s.Length != 24)
            {
                bytes = null;
                return false;
            }

            var result = new byte[12];
            for (int i = 0; i < 12; i++)
            {
                if (!TryHexValue(s[i * 2], out int hi) ||
                    !TryHexValue(s[i * 2 + 1], out int lo))
                {
                    bytes = null;
                    return false;
                }
                result[i] = (byte)((hi << 4) | lo);
            }

            bytes = result;
            return true;
        }
    }
}
