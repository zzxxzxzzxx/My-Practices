// File create date:6/25/2018
using System.Collections.Generic;
// Created By Yu.Liu
namespace RoachGame.Network {
    /// <summary>
    /// 一个简单的数据协议示例
    /// </summary>
    public class SimpleDataProtocol : IDataProtocol {
        // Code Here
        public byte[] ExtractData(List<byte> buffer) {
            byte[] result = buffer.ToArray();
            buffer.Clear();
            return result;
        }
    }
}
