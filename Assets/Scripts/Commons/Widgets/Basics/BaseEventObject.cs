using RoachGame.Events;
using RoachGame.Services;
using RoachGame.Utils;
using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace RoachGame {
    /// <summary>
    /// 事件系统中游戏对象的公共基类，提供PreInit，Init，Execute和Release等方法可供重载
    /// </summary>
    public abstract class BaseEventObject : IdentifierObject, IEventObject {
        [Tooltip("是否将对象注册到事件系统")]
        public bool UseEventSystem = true;
        private IDisposable updateSubscriber; // UniRx代理主循环的IDisposable接口返回对象，用于在对象销毁时关闭主循环
        protected Queue<MBaseEvent> eventQueue = new Queue<MBaseEvent>(); // 事件队列
        private BaseEventService baseEventService; // 基础事件系统服务

        /// <summary>
        /// 事件入队操作
        /// </summary>
        /// <param name="e">事件对象</param>
        public void EnqueueEvent(MBaseEvent e) {
            if (eventQueue != null) {
                if (gameObject.activeSelf) {
                    eventQueue.Enqueue(e);
                } else if (e.needWakeUp) {
                    SetActive(true);
                    eventQueue.Enqueue(e);
                }
            }
        }

        /// <summary>
        /// 获取当前接口涉及的GameObject
        /// </summary>
        /// <returns>游戏对象</returns>
        public GameObject GetGameObject() {
            return gameObject;
        }

        /// <summary>
        /// 事件出队操作
        /// </summary>
        /// <returns>事件对象</returns>
        protected MBaseEvent DequeueEvent() {
            MBaseEvent result = null;
            if (eventQueue != null && eventQueue.Count > 0) {
                result = eventQueue.Dequeue();
            }
            return result;
        }

        protected void SendEvent(MBaseEvent e) {
            if (baseEventService != null) {
                baseEventService.SendEvent(e);
            }
        }
        /// <summary>
        /// 初始化虚方法，子类可覆盖
        /// </summary>
        protected virtual void Init() { }
        /// <summary>
        /// 帧更新虚方法，子类可覆盖
        /// </summary>
        protected virtual void Execute() { }

        /// <summary>
        /// 事件处理虚方法，子类可以覆盖
        /// </summary>
        /// <param name="e">待处理事件</param>
        protected virtual void handleEvent(MBaseEvent e) {
            LogUtils.logWarning("WARNING - in " + gameObject.name + " there is no implementation of handleEvent function!");
        }

        public void SetActive(bool active) {
            gameObject.SetActive(active);
        }

        private void Start() {
            try {
                // 自动注册本类实例到事件分发表
                if (UseEventSystem) {
                    UniverseController.Instance.registerEventObject(TagName, this);
                }
                Init(); // 初始化
                // UniRx将Update更新循环转化为观察者模式，通过订阅进行
                updateSubscriber = Observable.EveryGameObjectUpdate().Subscribe(DoUpdate);
            } catch (Exception e) {
                LogUtils.logNotice("ERROR in - " + gameObject.name + " Start");
                LogUtils.logError(e.Message + "\n" + e.StackTrace);
                exceptionFlag = true;
            }
        }

        /// <summary>
        /// 订阅更新方法
        /// </summary>
        private void DoUpdate(long ut) {
            try {
                if (!exceptionFlag) {
                    Execute(); // 子类实现更新调用
                    if (UseEventSystem) {
                        MBaseEvent e = DequeueEvent(); // 取出一个待处理事件
                        if (e != null) {
                            handleEvent(e);
                            e.ConsumeEvent();
                        }
                    }
                }
            } catch (Exception e) {
                LogUtils.logNotice("ERROR in - " + gameObject.name + " Update");
                LogUtils.logError(e.Message + "\n" + e.StackTrace);
                exceptionFlag = true;
            }
        }

        /// <summary>
        /// 对象回收时释放资源
        /// </summary>
        protected override void Release() {
            // 清理事件队列
            if (eventQueue != null) {
                eventQueue.Clear();
            }
            updateSubscriber.Dispose(); // 注销掉UniRx订阅代理的主更新循环
            if (UseEventSystem) { // 反注册事件系统
                UniverseController.Instance.unregisterEventObject(TagName, this);
            }
        }
    }
}
