using RoachGame.Events;
using RoachGame.Services;
using UnityEngine;

namespace RoachGame {
    /// <summary>
    /// 全局控制器，单例模式，静态初始化
    /// </summary>
    public class UniverseController {
        private static bool isInitialized = false;
        private readonly static UniverseController instance = new UniverseController();
        public static UniverseController Instance {
            get {
                return instance;
            }
        }
        /// <summary>
        /// 事件中心
        /// </summary>
        public static EventCenter eventCenter { get; private set; }

        public static void initialize() {
            if (!isInitialized) {
                // 生成所有的永久对象以及后台协程
                GameObject obj = new GameObject(CommonConfigs.NAME_EVENT_CENTER_OBJ);
                eventCenter = obj.AddComponent<EventCenter>();
                Object.DontDestroyOnLoad(obj);
                // 初始化所有游戏服务
                BaseEventService baseEventService = new BaseEventService(eventCenter);
                ServiceProvider.Instance.RegisterService(CommonConfigs.TAG_SVC_BASE_EVENT, baseEventService);
                EventService eventService = new EventService(eventCenter);
                ServiceProvider.Instance.RegisterService(GameConfigs.TAG_SVC_EVENT, eventService);
                // 初始化部分动态配置
                // 关于GUI比例的计算，必须按照CanvasScaler中指定的值进行，如果是按宽度缩放，则要用屏幕宽度计算，反之则反
                CommonConfigs.GUI_ANCHORED_RATIO = Screen.width / CommonConfigs.GUI_RESOLUTION_REF;
                isInitialized = true;
                // 任何涉及到BaseEventObject子类脚本的对象初始化都必须在更新标志后进行，避免无限递归
            }
        }
        /// <summary>
        /// 注册事件接受器
        /// </summary>
        /// <param name="tag">标签</param>
        /// <param name="obj">事件接收对象</param>
        public void registerEventObject(string tag, IEventObject obj) {
            if (eventCenter != null) {
                eventCenter.registerEventObject(tag, obj);
            }
        }
        /// <summary>
        /// 反注册事件接收器
        /// </summary>
        /// <param name="tag">标签</param>
        /// <param name="obj">事件接收对象</param>
        public void unregisterEventObject(string tag, IEventObject obj) {
            if (eventCenter != null) {
                eventCenter.unregisterEventObject(tag, obj);
            }
        }
        /// <summary>
        /// 分发事件
        /// </summary>
        /// <param name="e">事件对象</param>
        public void dispatchEvent(MBaseEvent e) {
            if (eventCenter != null) {
                eventCenter.dispatchEnqueue(e);
            }
        }
    }
}
