using System;
using UnityEngine;
using UnityEngine.UI;

namespace RoachGame.Widgets {
    /// <summary>
    /// 场景淡出淡入切换工具，通用型，可自定义淡出淡入回调
    /// </summary>
    public class SceneFadeSwitcher : MonoBehaviour {

        private NormalSceneFade sceneFade; // 淡出淡入工具
        public Image sceneMask; // 场景遮罩
        public float fadeDuration = 1.0f; // 淡出淡入持续时间

        private Action onSwitchCallback; // 切换回调，切入时在动画前回调，切出时在动画后回调

        private void Start() {
            sceneFade = new NormalSceneFade(sceneMask);
            sceneFade.durationTime = fadeDuration;
            sceneFade.beforeTransit = beforeSwitch;
            sceneFade.afterTransit = afterSwitch;
            sceneMask.gameObject.SetActive(true);
        }

        public void SwitchIn(Action callback = null) {
            onSwitchCallback = callback;
            Switch(false);
        }

        public void SwitchOut(Action callback = null) {
            onSwitchCallback = callback;
            Switch(true);
        }

        private void Switch(bool isOut) {
            if (sceneFade != null) {
                sceneFade.Transit(isOut);
            }
        }

        private void beforeSwitch() {
            sceneMask.gameObject.SetActive(true);
            if (!sceneFade.IsTransitOut) {
                if (onSwitchCallback != null) {
                    onSwitchCallback();
                }
            }
        }

        private void afterSwitch() {
            if (sceneFade.IsTransitOut) {
                if (onSwitchCallback != null) {
                    onSwitchCallback();
                }
            }
            sceneMask.gameObject.SetActive(false);
        }
    }
}
