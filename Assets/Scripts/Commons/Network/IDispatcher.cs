// File create date:6/6/2018
// Created By Yu.Liu
namespace RoachGame.Network {
    /// <summary>
    /// 消息分发接口
    /// </summary>
    public interface IDispatcher {
        /// <summary>
        /// 消息分发
        /// </summary>
        /// <param name="msg">消息数据</param>
        void Dispatch(byte[] msg);
    }
}
