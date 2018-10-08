// File create date:5/11/2018
using RoachGame.Events;
using System.Collections.Generic;

namespace RoachGame {
    /// <summary>
    /// 基础可订阅对象，派生自BaseEventObject类，包含一个订阅者列表，可将事件通知到列表中的所有订阅者
    /// </summary>
    public abstract class BaseEventSubject : BaseEventObject, IEventSubject {
        // 订阅者列表，可一次性将事件通知到列表中的全部订阅者
        protected List<IEventObject> observers = new List<IEventObject>();

        /// <summary>
        /// 通知订阅者们
        /// </summary>
        /// <param name="e">事件</param>
        public void Announce(MBaseEvent e) {
            for (int i = 0; i < observers.Count; i++) {
                observers[i].EnqueueEvent(e);
            }
        }

        /// <summary>
        /// 订阅者注册
        /// </summary>
        /// <param name="obj">订阅者</param>
        public void RegisterObject(IEventObject obj) {
            observers.Add(obj);
        }

        /// <summary>
        /// 订阅者反注册
        /// </summary>
        /// <param name="obj">订阅者</param>
        public void UnRegisterObject(IEventObject obj) {
            if (observers.Contains(obj)) {
                observers.Remove(obj);
            }
        }

        protected override void Release() {
            base.Release();
            observers.Clear();
        }
    }
}
