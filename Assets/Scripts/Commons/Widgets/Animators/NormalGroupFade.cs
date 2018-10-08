// File create date:1/6/2018
using UnityEngine;
using DG.Tweening;
using System;

// Created By Yu.Liu
namespace RoachGame.Widgets {
    /// <summary>
    /// 通用UI组淡出淡入工具
    /// </summary>
    public class NormalGroupFade : AbstractTransition {
        // Code Here
        private CanvasGroup maskGroup;

        public NormalGroupFade(CanvasGroup grp) {
            maskGroup = grp;
        }

        protected override void InitiateAnimation(bool isFadeOut) {
            if (maskGroup != null) {
                transitTweener = maskGroup.DOFade(isFadeOut ? 1f : 0f, durationTime).SetEase(Ease.Linear);
            }
        }
    }
}