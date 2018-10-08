// File create date:6/6/2018
// Created By Yu.Liu
namespace RoachGame.Network {
    /// <summary>
    /// 数据编译泛型接口
    /// </summary>
    /// <typeparam name="T">接受的数据类型</typeparam>
    public interface IDataCompiler<T> {
        /// <summary>
        /// 编译数据，用于将不同的对象编译为合规的二进制消息数据
        /// </summary>
        /// <param name="data">数据对象</param>
        /// <returns>二进制消息数据</returns>
        byte[] CompileData(T data);
        /// <summary>
        /// 反编译数据，用于将合规的二进制消息数据反编译为数据对象
        /// </summary>
        /// <param name="data">二进制消息数据</param>
        /// <returns>数据对象</returns>
        T DecompileData(byte[] data);
    }
}
