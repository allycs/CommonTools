namespace Allycs.Common
{
    using System.Collections.Generic;

    /// <summary>
    /// 命令执行结果类，用于封装操作的成功/失败状态和错误信息
    /// </summary>
    public class CommandResult
    {
        private readonly List<CommandError> _errors = new List<CommandError>();

        /// <summary>
        /// 创建一个空的命令结果对象
        /// </summary>
        public CommandResult() { }

        /// <summary>
        /// 创建带有错误消息的命令结果对象
        /// </summary>
        /// <param name="errorMessage">错误消息</param>
        public CommandResult(string errorMessage)
        {
            AddError(errorMessage);
        }

        /// <summary>
        /// 获取成功结果实例
        /// </summary>
        public static CommandResult SuccessResult => new CommandResult();

        /// <summary>
        /// 判断命令是否执行成功
        /// </summary>
        public bool IsSuccess => _errors.Count == 0;

        /// <summary>
        /// 判断命令是否执行失败
        /// </summary>
        public bool IsFailure => _errors.Count > 0;

        /// <summary>
        /// 添加错误消息
        /// </summary>
        /// <param name="error">错误消息</param>
        public void AddError(string error)
        {
            _errors.Add(new CommandError(error));
        }

        /// <summary>
        /// 获取错误消息列表
        /// </summary>
        public IReadOnlyList<CommandError> Errors => _errors.AsReadOnly();

        /// <summary>
        /// 获取第一个错误消息，如果没有错误则返回null
        /// </summary>
        public string ErrorMessage => _errors.Count > 0 ? _errors[0].Message : null;

        /// <summary>
        /// 获取所有错误消息的合并字符串
        /// </summary>
        public string ErrorsMessage => _errors.Count > 0 ? string.Join("; ", _errors) : null;
    }

    /// <summary>
    /// 带泛型数据的命令执行结果类
    /// </summary>
    /// <typeparam name="TData">返回数据的类型</typeparam>
    public class CommandResult<TData> : CommandResult
    {
        /// <summary>
        /// 创建一个空的命令结果对象
        /// </summary>
        public CommandResult() : base() { }

        /// <summary>
        /// 创建带有错误消息的命令结果对象
        /// </summary>
        /// <param name="errorMessage">错误消息</param>
        public CommandResult(string errorMessage) : base(errorMessage) { }

        /// <summary>
        /// 返回成功结果并附带数据
        /// </summary>
        /// <param name="data">返回的数据</param>
        /// <returns>包含数据的成功结果</returns>
        public static CommandResult<TData> Ok(TData data) => new CommandResult<TData> { Data = data };

        /// <summary>
        /// 返回失败结果
        /// </summary>
        /// <param name="errorMessage">错误消息</param>
        /// <returns>失败结果</returns>
        public static CommandResult<TData> Fail(string errorMessage) => new CommandResult<TData>(errorMessage);

        /// <summary>
        /// 获取或设置返回结果的数据
        /// </summary>
        public TData Data { get; set; }

        /// <summary>
        /// 获取数据，如果失败则返回默认值
        /// </summary>
        public TData DataOrDefault => Data;
    }

    /// <summary>
    /// 命令错误信息类
    /// </summary>
    public class CommandError
    {
        /// <summary>
        /// 获取错误消息
        /// </summary>
        public virtual string Message { get; }

        /// <summary>
        /// 创建错误信息实例
        /// </summary>
        /// <param name="message">错误消息</param>
        public CommandError(string message)
        {
            Message = message;
        }

        /// <summary>
        /// 获取错误消息字符串
        /// </summary>
        public override string ToString()
        {
            return Message;
        }
    }
}
