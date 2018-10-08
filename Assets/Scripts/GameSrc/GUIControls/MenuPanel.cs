/// File create date:10/8/2018
using RoachGame;
using RoachGame.Widgets;
// Created By Yu.Liu
public class MenuPanel : BaseView {

    public void onStartClick() {
        // 点击开始游戏
        ServiceUtils.GetEventService().RequestSceneSwitch(GameConfigs.ID_OBJ_SYNC_SCENE_LOADER, GameConfigs.NAME_SCENE_GAME);
    }

    public void onExitClick() {
        // 点击退出游戏
        ServiceUtils.GetEventService().RequestSceneSwitch(GameConfigs.ID_OBJ_SYNC_SCENE_LOADER, CommonConfigs.TARGET_SCENE_QUIT);
    }
}
