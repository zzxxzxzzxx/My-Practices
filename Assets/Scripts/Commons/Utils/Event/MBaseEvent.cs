using System;
using System.Collections.Generic;

namespace RoachGame.Events {
    /// <summary>
    /// 通用事件类，可继承拓展，受保护的构造函数，必须使用静态方法ObtainEvent获取新事件
    /// </summary>
    public class MBaseEvent {
        public static Queue<MBaseEvent> recycleEventQueue = new Queue<MBaseEvent>();
        /// <summary>
        /// 事件ID
        /// </summary>
        protected long eventID;
        /// <summary>
        /// 事件类型
        /// </summary>
        protected int eventType;
        /// <summary>
        /// 获取事件类型
        /// </summary>
        public int Type {
            get {
                return eventType;
            }
        }
        /// <summary>
        /// 事件携带的基础信息
        /// </summary>
        public int what;
        /// <summary>
        /// 事件可携带的复合信息
        /// </summary>
        public object obj;
        /// <summary>
        /// 事件可携带的回调方法，仅以自身为参数，其它回调要通过额外回调列表进行
        /// </summary>
        public Action<MBaseEvent> callback;
        /// <summary>
        /// 是否需要唤醒接收方，可用于将非活动状态的物体唤起
        /// </summary>
        public bool needWakeUp = false;
        /// <summary>
        /// 是否广播，一般而言不建议和needWakeUp同时为True，广播事件将会向所有对象发送
        /// </summary>
        public bool isBroadcast = false;
        /// <summary>
        /// 事件源（来自对象TagName）
        /// </summary>
        public string eventSource;
        /// <summary>
        /// 事件目标（来自对象TagName）
        /// </summary>
        public string dispatchTarget;
        /// <summary>
        /// 事件携带的额外信息
        /// </summary>
        protected Dictionary<string, object> extra;
        /// <summary>
        /// 时间携带的额外回调
        /// </summary>
        protected Dictionary<string, Action> extraCallbacks;
        /// <summary>
        /// 是否已经被消费，用于防止重复消费事件
        /// </summary>
        protected bool isConsumed = false;

        /// <summary>
        /// 静态方法获取一个空事件，如果回收队列中有可用事件则重用，否则新建一个
        /// </summary>
        /// <param name="type">事件类型</param>
        /// <returns>事件对象</returns>
        public static MBaseEvent ObtainEvent(int type) {
            MBaseEvent ev = null;
            if (recycleEventQueue.Count > 0) {
                ev = recycleEventQueue.Dequeue();
                ev.eventID = DateTime.UtcNow.Ticks;
                ev.eventType = type;
                ev.isConsumed = false;
            } else {
                ev = new MBaseEvent(type);
            }
            return ev;
        }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="type">标明当前事件类型</param>
        protected MBaseEvent(int type) {
            eventID = DateTime.UtcNow.Ticks; // 打上时间戳
            eventType = type;
        }
        /// <summary>
        /// 放入额外信息
        /// </summary>
        /// <param name="key">额外信息关键字</param>
        /// <param name="value">额外信息内容</param>
        public void PutExtra(string key, Object value) {
            if (extra == null) {
                extra = new Dictionary<string, Object>();
            }
            extra[key] = value;
        }
        /// <summary>
        /// 获取额外信息
        /// </summary>
        /// <param name="key">额外信息关键字</param>
        /// <returns>额外信息内容</returns>
        public object GetExtra(string key) {
            object result = null;
            if (extra != null) {
                result = extra.TryGetElement(key);
            }
            return result;
        }
        /// <summary>
        /// 放入额外回调
        /// </summary>
        /// <param name="key">额外回调关键字</param>
        /// <param name="action">额外回调方法</param>
        public void PutCallback(string key, Action action) {
            if (extraCallbacks == null) {
                extraCallbacks = new Dictionary<string, Action>();
            }
            extraCallbacks[key] = action;
        }
        /// <summary>
        /// 获取额外回调
        /// </summary>
        /// <param name="key">额外回调关键字</param>
        /// <returns>额外回调方法</returns>
        public Action GetCallback(string key) {
            Action action = null;
            if (extraCallbacks != null) {
                action = extraCallbacks.TryGetElement(key);
            }
            return action;
        }
        /// <summary>
        /// 设置事件的来源和目的地
        /// </summary>
        /// <param name="src">来源</param>
        /// <param name="target">目的</param>
        public void SetupTarget(string src, string target) {
            eventSource = src;
            dispatchTarget = target;
        }
        /// <summary>
        /// 消费当前事件，调用一次回调并将该事件放入回收队列
        /// </summary>
        public void ConsumeEvent() {
            if (callback != null) {
                callback(this);
            }
            isConsumed = true;
            recycleEventQueue.Enqueue(this);
        }
    }
}
