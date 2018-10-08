/// File create date:10/8/2018
using RoachGame;
using RoachGame.Events;
using RoachGame.Services;
// Created By Yu.Liu
public class EventService : BaseEventService {
    // Script Code
    public EventService(EventCenter center) : base(center) {
    }

    /// <summary>
    /// 请求场景切换，加载器必须在SceneCtrl作用域下
    /// </summary>
    /// <param name="loader">加载器ID</param>
    /// <param name="sceneName">目标场景名称</param>
    /// <param name="src">事件源</param>
    public void RequestSceneSwitch(string loader, string sceneName, string src = null) {
        string target = IdentifierUtils.CompileTagNameByID(loader, GameConfigs.DOMAIN_OBJ_SCENE_CTRL);
        EventUtils.EventBuilder builder = GenerateCommonBuilder(CommonConfigs.EVENT_COMMON_MAIN_GAME, CommonConfigs.SUB_EVENT_MAIN_GAME_SCENE_LOAD, target, src);
        builder.SetObject(sceneName);
        SendEvent(builder.BuildEvent());
    }
}
