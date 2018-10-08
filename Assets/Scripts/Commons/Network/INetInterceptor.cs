// File create date:6/6/2018
// Created By Yu.Liu
namespace RoachGame.Network {
    /// <summary>
    /// 网络拦截器接口
    /// </summary>
    public interface INetInterceptor {
        /// <summary>
        /// 拦截消息
        /// </summary>
        /// <param name="data"></param>
        void Intercept(byte[] data);
    }
}
