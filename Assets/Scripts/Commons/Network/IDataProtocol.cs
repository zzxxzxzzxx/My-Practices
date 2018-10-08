// File create date:6/6/2018
using System.Collections.Generic;
// Created By Yu.Liu
namespace RoachGame.Network {
    /// <summary>
    /// 数据协议接口
    /// </summary>
    public interface IDataProtocol {
        /// <summary>
        /// 提取符合协议的数据
        /// </summary>
        /// <param name="buffer">数据缓存</param>
        /// <returns></returns>
        byte[] ExtractData(List<byte> buffer);
    }
}
