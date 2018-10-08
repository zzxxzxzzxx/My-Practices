// File create date:5/11/2018
using RoachGame.Events;
// Created By Yu.Liu
namespace RoachGame {
    /// <summary>
    /// 可订阅对象接口
    /// </summary>
    public interface IEventSubject {
        void RegisterObject(IEventObject obj);
        void UnRegisterObject(IEventObject obj);
        void Announce(MBaseEvent e);
    }
}
