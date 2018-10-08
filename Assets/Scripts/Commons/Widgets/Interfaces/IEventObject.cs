using RoachGame.Events;
using UnityEngine;
namespace RoachGame {
    /// <summary>
    /// 事件对象接口
    /// </summary>
    public interface IEventObject {
        void EnqueueEvent(MBaseEvent e);
        GameObject GetGameObject();
    }
}
