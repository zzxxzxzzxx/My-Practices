using System.Collections.Generic;
using UnityEngine;

namespace RoachGame.Events {
    /// <summary>
    /// 事件分发中心
    /// </summary>
    public class EventCenter : MonoBehaviour {
        /// <summary>
        /// 事件分发队列
        /// </summary>
        private Queue<MBaseEvent> dispatchQueue = new Queue<MBaseEvent>();
        /// <summary>
        /// 事件分发表
        /// </summary>
        private Dictionary<string, List<IEventObject>> dispatchTable = new Dictionary<string, List<IEventObject>>();
        /// <summary>
        /// 是否正在进行分发
        /// </summary>
        private bool isDispatching = true;
        public bool IsDispatching {
            get {
                return isDispatching;
            }
        }

        void Update() {
            EventCycle();
        }
        /// <summary>
        /// 消息主循环
        /// </summary>
        private void EventCycle() {
            if (isDispatching) {
                MBaseEvent dispatchEvent = null;
                lock (dispatchQueue) {
                    dispatchEvent = dispatchDequeue();
                }
                if (dispatchEvent != null) { // 队列为空时不分发
                    if (dispatchEvent.isBroadcast) { // 广播事件不需要考虑目标
                        foreach (List<IEventObject> dispatch in dispatchTable.Values) {
                            foreach (IEventObject obj in dispatch) {
                                if (obj.GetGameObject().activeSelf) {
                                    obj.EnqueueEvent(dispatchEvent);
                                }
                            }
                        }
                    } else { // 非广播事件需要取到目标对象表
                        List<IEventObject> dispatch = dispatchTable.TryGetElement(dispatchEvent.dispatchTarget);
                        if (dispatch != null) {
                            foreach (IEventObject obj in dispatch) {
                                obj.EnqueueEvent(dispatchEvent);
                            }
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 注册分发入口
        /// </summary>
        /// <param name="tag">分发入口标签</param>
        /// <param name="obj">分发入口对象</param>
        public void registerEventObject(string tag, IEventObject obj) {
            List<IEventObject> tempList;
            if (dispatchTable.ContainsKey(tag)) {
                tempList = dispatchTable[tag];
            } else {
                tempList = new List<IEventObject>();
            }
            tempList.Add(obj);
            dispatchTable[tag] = tempList;
        }
        /// <summary>
        /// 反注册分发入口
        /// </summary>
        /// <param name="tag">分发入口标签</param>
        /// <param name="obj">分发入口对象</param>
        public void unregisterEventObject(string tag, IEventObject obj) {
            List<IEventObject> tempList;
            if (dispatchTable.ContainsKey(tag)) {
                tempList = dispatchTable[tag];
                tempList.Remove(obj);
                dispatchTable[tag] = tempList;
            }
        }
        /// <summary>
        /// 分发事件入队
        /// </summary>
        /// <param name="e">事件对象</param>
        public void dispatchEnqueue(MBaseEvent e) {
            lock (dispatchQueue) {
                dispatchQueue.Enqueue(e);
            }
        }
        /// <summary>
        /// 分发事件出队
        /// </summary>
        /// <returns>事件对象</returns>
        private MBaseEvent dispatchDequeue() {
            MBaseEvent result = null;
            lock (dispatchQueue) {
                if (dispatchQueue.Count > 0) {
                    result = dispatchQueue.Dequeue();
                }
            }
            return result;
        }
        /// <summary>
        /// 分发开关
        /// </summary>
        /// <param name="isOn">是否启用分发</param>
        public void dispatchSwitch(bool isOn) {
            isDispatching = isOn;
        }
    }
}
