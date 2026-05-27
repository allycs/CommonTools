namespace Allycs.Common
{
    /// <summary>
    /// 密码格式类型枚举，定义支持的密码哈希算法
    /// </summary>
    public enum PasswordFormatType : int
    {
        /// <summary>
        /// 不加密（明文，不推荐）
        /// </summary>
        None = 0,

        /// <summary>
        /// SHA1哈希（已过时，不推荐）
        /// </summary>
        SHA1 = 1,

        /// <summary>
        /// MD5哈希（已过时，不推荐）
        /// </summary>
        MD5 = 2,

        /// <summary>
        /// SHA256哈希
        /// </summary>
        SHA256 = 3,

        /// <summary>
        /// SHA384哈希
        /// </summary>
        SHA384 = 4,

        /// <summary>
        /// SHA512哈希（推荐）
        /// </summary>
        SHA512 = 5,

        /// <summary>
        /// PBKDF2（推荐，支持迭代次数）
        /// </summary>
        PBKDF2 = 6,

        /// <summary>
        /// BCrypt（推荐，自适应哈希）
        /// </summary>
        BCrypt = 7
    }
}
