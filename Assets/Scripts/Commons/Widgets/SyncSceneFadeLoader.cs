/// File create date:9/18/2018
using RoachGame.Events;
using RoachGame.Utils;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
// Created By Yu.Liu
namespace RoachGame.Widgets {
    /// <summary>
    /// 同步场景加载工具，包含淡出淡入切换功能
    /// </summary>
    public class SyncSceneFadeLoader : BaseEventObject {

        protected NormalSceneFade sceneFade; // 淡出淡入工具
        private Action onSwitchCallback; // 场景淡入后回调
        public Image sceneMask; // 场景遮罩
        public bool needFadeSwitch = false; // 是否需要淡出淡入切换标志
        public bool isInitFade = false; // 初始化时是否需要淡入
        public float fadeDuration = 1.0f; // 淡出淡入持续时间

        protected override void PreInit() {
            if (needFadeSwitch) {
                if (sceneMask != null) {
                    sceneFade = new NormalSceneFade(sceneMask);
                    sceneFade.durationTime = fadeDuration;
                } else {
                    needFadeSwitch = false;
                    sceneFade = null;
                }
            }
        }

        protected override void Init() {
            if (sceneFade != null) {
                sceneFade.beforeTransit = onBeforeLoad;
                sceneFade.afterTransit = onAfterLoad;
                if (isInitFade) {
                    sceneFade.Transit(false);
                }
            }
        }

        protected override void handleEvent(MBaseEvent e) {
            if (e.Type == CommonConfigs.EVENT_COMMON_MAIN_GAME) {
                if (e.what == CommonConfigs.SUB_EVENT_MAIN_GAME_SCENE_LOAD) {
                    LoadScene((string)e.obj, e.GetCallback(CommonConfigs.EV_EXTRA_KEY_SCENE_LOAD_CALLBACK));
                }
            }
        }

        private void onBeforeLoad() {
            sceneMask.gameObject.SetActive(true);
            if (!sceneFade.IsTransitOut) {
                if (onSwitchCallback != null) {
                    onSwitchCallback();
                }
            }
        }

        private void onAfterLoad() {
            if (sceneFade.IsTransitOut) {
                if (onSwitchCallback != null) {
                    onSwitchCallback();
                }
                if (TextUtils.hasData(sceneFade.TargetScene)) {
                    if (sceneFade.TargetScene.Equals(CommonConfigs.TARGET_SCENE_QUIT)) {
                        Application.Quit();
                    } else {
                        SceneManager.LoadScene(sceneFade.TargetScene);
                    }
                } else {
                    sceneMask.gameObject.SetActive(false);
                }
            } else {
                sceneMask.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// 加载指定场景，如果场景名为空则仅淡出
        /// </summary>
        /// <param name="target">目标场景名</param>
        public void LoadScene(string target = null, Action callback = null) {
            onSwitchCallback = callback;
            if (sceneFade != null) {
                sceneFade.Transit(true, target);
            }
        }
    }
}
