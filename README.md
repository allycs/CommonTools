# Allycs.Common 通用工具库

![.NET Standard 2.0](https://img.shields.io/badge/.NET%20Standard-2.0-blue.svg)
![License: MIT](https://img.shields.io/badge/License-MIT-green.svg)
![Build Status](https://img.shields.io/badge/Build-Passing-brightgreen.svg)

Allycs.Common 是一个功能丰富的 **.NET Standard 2.0** 通用工具库，专为企业级应用开发设计。它提供了一套全面的工具类，涵盖地理坐标处理、字符串操作、安全加密、日期时间处理、网络工具、集合操作等多个领域，帮助开发者快速构建高质量的.NET应用程序。

## ✨ 核心特性

- **跨平台兼容**：支持 .NET Framework 4.6.1+、.NET Core 2.0+、.NET 5+
- **工业级标准**：完善的参数验证、异常处理、线程安全设计
- **性能优化**：高效的算法实现，预编译正则表达式，O(1) 查找复杂度
- **易于使用**：丰富的扩展方法，直观的 API 设计
- **向后兼容**：保留旧版本接口，平滑升级

## 🌟 功能模块

| 模块 | 功能 | 适用场景 |
|------|------|----------|
| **Geo** | 坐标系转换、距离计算、地理几何 | 地图应用、位置服务 |
| **Country** | MMSI/ICAO国家查询 | 船舶/飞机识别系统 |
| **String** | 验证、生成、过滤、加密 | 表单验证、数据清洗 |
| **Security** | 密码哈希、AES加密、HMAC签名 | 用户认证、数据安全 |
| **Time** | Unix时间戳、日期计算 | 日志记录、时间处理 |
| **Net** | IP/MAC/URL处理 | 网络编程、安全检查 |
| **IO** | 文件/目录操作 | 文件管理、配置读写 |
| **Collections** | 分页、去重、分批 | 数据处理、批量操作 |
| **Reflection** | 属性读写、类型检查 | 框架开发、动态编程 |

## 📁 项目结构

```
Allycs.Common/
├── Geo/              # 地理/坐标工具
│   ├── AllycsGeoCircle.cs      # 地理圆类
│   ├── AllycsGeoRectangle.cs   # 地理矩形类
│   ├── CoordinateTransform.cs  # 坐标系转换与距离计算
│   └── GeoTypeHelper.cs        # GeoJSON处理
├── Math/             # 数学计算工具
│   └── MathHelper.cs           # 数学辅助方法
├── String/           # 字符串工具
│   ├── StringExtensions.cs     # 字符串扩展方法
│   └── StringGenerator.cs      # 字符串生成工具
├── Security/         # 安全/哈希工具
│   ├── HashHelper.cs           # 哈希生成工具（兼容旧版本）
│   └── PasswordHelper.cs       # 现代密码处理工具
├── Time/             # 时间工具
│   └── DateTimeExtensions.cs   # 日期时间扩展
├── Validation/       # 验证工具
│   └── Guard.cs                # 参数验证工具
├── Result/           # 结果封装工具
│   └── CommandResult.cs        # 命令执行结果
├── Id/               # ID生成工具
│   └── ObjectId.cs             # MongoDB风格ObjectId
├── Country/          # 国家服务工具
│   ├── CountryInfo.cs          # 国家信息类
│   ├── MMSICountryService.cs   # MMSI国家服务（船舶识别）
│   └── IcaoCountryService.cs   # ICAO国家服务（飞机识别）
├── Net/              # 网络工具
│   └── NetHelper.cs            # IP/MAC/URL处理
├── IO/               # IO工具
│   └── IOHelper.cs             # 文件/目录操作
├── Collections/      # 集合工具
│   └── CollectionHelper.cs     # 集合操作扩展
├── Reflection/       # 反射工具
│   └── ReflectionHelper.cs     # 反射操作辅助
└── Types/            # 类型定义
    ├── GeometryType.cs         # 几何类型枚举
    └── PasswordFormatType.cs   # 密码格式枚举
```

## 🚀 快速开始

### 安装

通过NuGet安装：

```bash
Install-Package Allycs.Common
```

或者直接引用项目文件。

### 命名空间引用

```csharp
using Allycs.Common;
using Allycs.Common.Geo;
using Allycs.Common.Country;
using Allycs.Common.Security;
using Allycs.Common.Net;
using Allycs.Common.IO;
// ... 其他命名空间
```

## 📖 使用手册

### 1. 地理工具

#### 1.1 坐标系转换

```csharp
// WGS84转GCJ02
double[] gcj02 = CoordinateTransform.WGS84ToGCJ02Arr(116.4074, 39.9042);

// GCJ02转WGS84
double[] wgs84 = CoordinateTransform.GCJ02ToWGS84Arr(gcj02[0], gcj02[1]);

// WGS84转BD09（百度坐标系）
double[] bd09 = CoordinateTransform.WGS84ToBD09(116.4074, 39.9042);
```

#### 1.2 两点距离计算

```csharp
// 计算北京到上海的距离（约1069公里）
double distance = CoordinateTransform.CalculateDistance(39.9042, 116.4074, 31.2304, 121.4737);

// 使用指定单位
double kmDistance = CoordinateTransform.CalculateDistance(39.9042, 116.4074, 31.2304, 121.4737, DistanceUnit.Kilometers);
```

#### 1.3 地理圆与矩形

```csharp
// 创建一个以北京天安门为圆心，半径1000米的圆
var circle = new AllycsGeoCircle(39.9042, 116.4074, 1000);
Polygon polygon = circle.ToPolygon();

// 创建矩形区域
var bottomLeft = new WGS84Point(39.9, 116.3);
var topRight = new WGS84Point(40.0, 116.5);
var rectangle = new AllycsGeoRectangle(bottomLeft, topRight);
```

### 2. 国家服务

#### 2.1 MMSI国家服务（船舶识别）

```csharp
// 根据MMSI号码获取国家
string mmsi = "412123456";
CountryInfo country = MMSICountryService.GetCountryByMmsi(mmsi);
Console.WriteLine(country.NameZh); // 输出: 中国

// 根据MID代码获取国家
CountryInfo china = MMSICountryService.GetCountryByMid(412);

// 验证MMSI格式
bool isValid = MMSICountryService.IsValidMmsi(mmsi);
```

#### 2.2 ICAO国家服务（飞机识别）

```csharp
// 根据ICAO24码获取国家
string icao24 = "7C0000";
IcaoCountryInfo country = IcaoCountryService.GetCountryByIcao24(icao24);
Console.WriteLine(country.NameZh); // 输出: 中国

// 验证ICAO24格式
bool isValid = IcaoCountryService.IsValidIcao24(icao24);
```

### 3. 字符串工具

```csharp
// 验证邮箱
bool isEmail = "test@example.com".IsEmail();

// 验证IP地址
bool isIp = "192.168.1.1".IsIP();

// SQL注入检测
bool hasSqlKeyword = input.ContainsSqlKeyword();

// 修剪字符串并将空字符串转为null
string trimmed = "  hello  ".TrimToNull(); // "hello"
string nullResult = "   ".TrimToNull();    // null

// 生成随机密码
string password = StringGenerator.GenerateStrongPassword(16);

// 生成GUID
string guid = StringGenerator.GenerateGuid();
```

### 4. 安全工具

#### 4.1 密码哈希

```csharp
// 生成盐值
string salt = PasswordHelper.GenerateSalt();

// 使用PBKDF2哈希密码
string hash = PasswordHelper.HashPasswordWithPBKDF2("password", salt);

// 验证密码
bool isValid = PasswordHelper.VerifyPassword("password", hash, salt, PasswordFormatType.PBKDF2);
```

#### 4.2 加密解密

```csharp
// AES加密
string key = PasswordHelper.GenerateAESKey();
string iv = PasswordHelper.GenerateAESIV();
string encrypted = PasswordHelper.AESEncrypt("secret", key, iv);
string decrypted = PasswordHelper.AESDecrypt(encrypted, key, iv);

// HMAC签名
string signature = PasswordHelper.ComputeHMACSHA256("data", "secretKey");
```

### 5. 日期时间工具

```csharp
// 获取Unix时间戳（秒）
int timestamp = DateTime.Now.ToUnixTimestamp();

// 获取Unix时间戳（毫秒）
long timestampMillis = DateTime.Now.ToUnixTimestampMillis();

// 从时间戳转换
DateTime date = DateTimeExtensions.FromUnixTimestamp(timestamp);

// 判断是否是周末
bool isWeekend = DateTime.Now.IsWeekend();

// 获取中文星期
string weekDay = DateTime.Now.WeekDayOfChinese();
```

### 6. 参数验证

```csharp
// 检查非空
Guard.NotNull(nameof(param), param);
Guard.NotNullOrEmpty(nameof(name), name);

// 检查条件
Guard.IsTrue(value > 0, nameof(value), "值必须大于0");

// 检查范围
Guard.InRange(age, nameof(age), 18, 65);
```

### 7. 网络工具

```csharp
// IP地址验证
bool isIPv4 = NetHelper.IsValidIPv4("192.168.1.1");
bool isPrivateIP = NetHelper.IsPrivateIP("10.0.0.1");

// CIDR范围检查
bool isInRange = NetHelper.IsIPInRange("192.168.1.100", "192.168.1.0/24");

// MAC地址处理
bool isValidMAC = NetHelper.IsValidMACAddress("AA:BB:CC:DD:EE:FF");
string formattedMAC = NetHelper.FormatMACAddress("AABBCCDDEEFF");

// URL处理
bool isValidUrl = NetHelper.IsValidUrl("https://example.com");
Dictionary<string, string> params = NetHelper.ParseQueryString("https://example.com?a=1&b=2");
```

### 8. IO工具

```csharp
// 确保目录存在
IOHelper.EnsureDirectoryExists(@"C:\temp");

// 读写文件
string content = IOHelper.ReadAllText("file.txt");
IOHelper.WriteAllText("file.txt", "content");

// 安全删除
bool deleted = IOHelper.SafeDeleteFile("temp.txt");

// 文件大小格式化
string size = IOHelper.FormatFileSize("largefile.zip");
```

### 9. 集合工具

```csharp
// 分页
var page = list.Page(1, 10);

// 去重
var distinct = list.DistinctBy(x => x.Id);

// 打乱顺序
var shuffled = list.Shuffle();

// 分批处理
var chunks = list.Chunk(100);

// 查找重复项
var duplicates = list.FindDuplicates();
```

### 10. 反射工具

```csharp
// 获取属性值
object value = ReflectionHelper.GetPropertyValue(obj, "PropertyName");

// 设置属性值
ReflectionHelper.SetPropertyValue(obj, "PropertyName", "newValue");

// 创建实例
var instance = ReflectionHelper.CreateInstance<MyClass>();

// 对象转字典
Dictionary<string, object> dict = ReflectionHelper.ToDictionary(obj);
```

### 11. ObjectId生成

```csharp
// 生成新的ObjectId
ObjectId id = ObjectId.NewId();

// 转换为字符串
string idStr = id.ToString();

// 从字符串解析
ObjectId parsedId = ObjectId.Parse(idStr);
```

## 🎯 功能特点

| 模块 | 功能 | 特点 |
|------|------|------|
| **Geo** | 坐标系转换、地理几何、距离计算 | 支持WGS84/GCJ02/BD09互转 |
| **Country** | MMSI/ICAO国家查询 | 支持船舶和飞机识别 |
| **String** | 验证、生成、加密、SQL过滤 | 丰富的扩展方法 |
| **Security** | 密码、哈希、加密 | PBKDF2、AES、HMAC支持 |
| **Time** | 日期时间处理 | 便捷的扩展方法 |
| **Validation** | 参数验证 | 简洁易用 |
| **Id** | ObjectId生成 | MongoDB兼容，性能优化 |
| **Net** | IP、MAC、URL验证 | 全面的网络工具 |
| **IO** | 文件、目录操作 | 安全可靠的IO工具 |
| **Collections** | 集合操作扩展 | 分页、去重、随机等 |
| **Reflection** | 反射工具 | 简化反射操作 |

## 🔧 技术要求

- .NET Standard 2.0
- .NET Framework 4.6.1+
- .NET Core 2.0+

## 📄 许可证

MIT License

## 🤝 贡献

欢迎提交Issue和Pull Request！

---

## 📚 API参考

### MMSICountryService 方法

| 方法 | 说明 | 返回值 |
|------|------|--------|
| `GetCountryByMmsi(string mmsi)` | 根据MMSI获取国家 | `CountryInfo` |
| `GetCountryByMid(int mid)` | 根据MID获取国家 | `CountryInfo` |
| `GetAllCountries()` | 获取所有国家 | `List<CountryInfo>` |
| `IsValidMmsi(string mmsi)` | 验证MMSI格式 | `bool` |
| `IsValidMid(int mid)` | 验证MID是否存在 | `bool` |

### IcaoCountryService 方法

| 方法 | 说明 | 返回值 |
|------|------|--------|
| `GetCountryByIcao24(string icao24)` | 根据ICAO24获取国家 | `IcaoCountryInfo` |
| `GetAllCountries()` | 获取所有国家 | `List<IcaoCountryInfo>` |
| `IsValidIcao24(string icao24)` | 验证ICAO24格式 | `bool` |

### PasswordHelper 方法

| 方法 | 说明 | 返回值 |
|------|------|--------|
| `GeneratePassword(int length)` | 生成随机密码 | `string` |
| `GenerateSalt(int size)` | 生成盐值 | `string` |
| `HashPassword(string, PasswordFormatType, string)` | 哈希密码 | `string` |
| `VerifyPassword(string, string, string, PasswordFormatType)` | 验证密码 | `bool` |
| `AESEncrypt/Decrypt` | AES加密解密 | `string` |
| `ComputeHMACSHA256/512` | HMAC签名 | `string` |

### NetHelper 方法

| 方法 | 说明 | 返回值 |
|------|------|--------|
| `IsValidIPv4(string)` | 验证IPv4 | `bool` |
| `IsValidIPv6(string)` | 验证IPv6 | `bool` |
| `IsPrivateIP(string)` | 检查内网IP | `bool` |
| `IsIPInRange(string, string)` | CIDR范围检查 | `bool` |
| `IsValidMACAddress(string)` | 验证MAC地址 | `bool` |
| `IsValidUrl(string)` | 验证URL | `bool` |

### CollectionHelper 方法

| 方法 | 说明 | 返回值 |
|------|------|--------|
| `Page(IEnumerable, int, int)` | 分页 | `IEnumerable<T>` |
| `DistinctBy(IEnumerable, Func)` | 按属性去重 | `IEnumerable<T>` |
| `Chunk(IEnumerable, int)` | 分批 | `IEnumerable<IEnumerable<T>>` |
| `Shuffle(IEnumerable)` | 打乱顺序 | `IEnumerable<T>` |
| `FindDuplicates(IEnumerable)` | 查找重复 | `IEnumerable<T>` |

### CountryInfo 属性

| 属性 | 类型 | 说明 |
|------|------|------|
| `Mid` | `int` | MID国家代码 |
| `NameEn` | `string` | 英文名称 |
| `NameZh` | `string` | 中文名称 |
| `IsoCode` | `string` | ISO 3166-1代码 |

### PasswordFormatType 枚举

| 值 | 说明 |
|----|------|
| `None` | 无加密 |
| `MD5` | MD5哈希 |
| `SHA1` | SHA1哈希 |
| `SHA256` | SHA256哈希 |
| `SHA384` | SHA384哈希 |
| `SHA512` | SHA512哈希 |
| `PBKDF2` | PBKDF2哈希（推荐） |
| `BCrypt` | BCrypt哈希（回退到PBKDF2） |

### DistanceUnit 枚举

| 值 | 说明 |
|----|------|
| `Meters` | 米 |
| `Kilometers` | 千米/公里 |
| `Miles` | 英里 |
| `NauticalMiles` | 海里 |