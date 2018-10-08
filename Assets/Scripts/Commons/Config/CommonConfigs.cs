namespace RoachGame {
    public static class CommonConfigs {
        #region "Number Configs"
        public const int EVENT_COMMON_UI_FUNCTION = 0x0000; // 通用界面功能事件
        public const int SUB_EVENT_COMMON_FUNC_OPEN = 0x0001;
        public const int EVENT_COMMON_CALLBACK = 0x0001; // 通用回调事件
        public const int SUB_EVENT_CALLBACK_TOGGLE_CHANGE = 0x0000; // Toggle选择变化回调
        public const int SUB_EVENT_CALLBACK_DROPDOWN_CHANGE = 0x0001; // Dropdown选择变化回调
        public const int EVENT_COMMON_ITEM = 0x0002; // 通用列表项功能事件
        public const int EVENT_COMMON_MAIN_GAME = 0x0003; // 主游戏功能事件
        public const int SUB_EVENT_MAIN_GAME_SCENE_LOAD = 0x0000; // 场景加载功能
        #endregion
        #region "String Configs"
        public const string TARGET_SCENE_QUIT = "EXIT_GAME";
        public const string NAME_EVENT_CENTER_OBJ = "EventCenter";
        public const string TAG_NULL_OBJ = "NULL_OBJECT";
        public const string TAG_SYSTEM_OBJ = "GameSystem";
        public const string TAG_SVC_BASE_EVENT = "BaseEventService";
        public const string EV_EXTRA_KEY_SCENE_LOAD_CALLBACK = "SceneLoadCallback";
        #endregion
        #region "Dynamic Configs"
        public const float GUI_RESOLUTION_REF = 1920f; // 这是UGUI的参考分辨率，也就是CanvasScaler上的缩放参考，可以是宽或者高
        public static float GUI_ANCHORED_RATIO = 1f; // 这是UGUI的比例参数，用于将屏幕位置映射到GUI坐标
        #endregion
    }
}
