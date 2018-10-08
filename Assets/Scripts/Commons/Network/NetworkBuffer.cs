// File create date:6/6/2018
using System.Collections.Generic;
// Created By Yu.Liu
namespace RoachGame.Network {
    /// <summary>
    /// 网络数据缓冲区
    /// </summary>
    public class NetworkBuffer {

        private List<byte> byteBuffer; // 数据缓冲列表
        private IDataProtocol dataProtocol; // 数据协议，根据协议才能解析出合适的数据

        public NetworkBuffer() {
            byteBuffer = new List<byte>();
        }
        /// <summary>
        /// 设置数据协议
        /// </summary>
        /// <param name="protocol">数据协议接口</param>
        public void setProtocol(IDataProtocol protocol) {
            dataProtocol = protocol;
        }
        /// <summary>
        /// 添加接收到的原始二进制数据
        /// </summary>
        /// <param name="data">接收数据</param>
        public virtual void AddData(byte[] data) {
            lock (byteBuffer) {
                byteBuffer.AddRange(data);
            }
        }
        /// <summary>
        /// 添加接收到的原始二进制数据
        /// </summary>
        /// <param name="data">接收数据</param>
        /// <param name="length">指定长度</param>
        public virtual void AddData(byte[] data, int length) {
            lock (byteBuffer) {
                for (int i = 0; i < length; i++) {
                    byteBuffer.Add(data[i]);
                }
            }
        }
        /// <summary>
        /// 解析出一个符合协议的数据对象并返回，如果不存在这样的数据对象则返回null
        /// </summary>
        /// <returns>符合协议的数据对象二进制表示</returns>
        public virtual byte[] GetData() {
            if (dataProtocol != null) {
                return dataProtocol.ExtractData(byteBuffer);
            }
            return null;
        }
    }
}
