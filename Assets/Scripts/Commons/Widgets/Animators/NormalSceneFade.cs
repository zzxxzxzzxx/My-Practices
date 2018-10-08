using System;
using UnityEngine.UI;
using DG.Tweening;

namespace RoachGame.Widgets {
    /// <summary>
    /// 通用场景淡出淡入工具
    /// </summary>
    public class NormalSceneFade : AbstractTransition {

        private Image sceneMask;

        public NormalSceneFade(Image mask) {
            sceneMask = mask;
        }

        protected override void InitiateAnimation(bool isFadeOut) {
            if (sceneMask != null) {
                transitTweener = sceneMask.DOFade(isFadeOut ? 1f : 0f, durationTime).SetEase(Ease.Linear);
            }
        }
    }
}