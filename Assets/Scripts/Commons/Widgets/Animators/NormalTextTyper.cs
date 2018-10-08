// File create date:4/22/2018
using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

// Created By Yu.Liu
namespace RoachGame.Widgets {
    /// <summary>
    /// 文字的打字机效果动画播放器
    /// </summary>
    public class NormalTextTyper : AbstractAnimation {
        // Code Here
        private Text contentText;
        public bool isRichText = false;
        private string contentStr;

        public NormalTextTyper(Text text) {
            contentText = text;
        }

        public void playText(string str) {
            contentStr = str;
            Animate();
        }

        protected override void InitiateAnimation() {
            if (contentText != null) {
                animTweener = contentText.DOText(contentStr, durationTime * contentStr.Length, isRichText).SetEase(Ease.Linear);
            }
        }

        public void stopPlay() {
            if (animTweener != null && isPlayingAnimate) {
                contentText.DOKill();
                animTweener.Kill();
                onAfterAnimate();
            }
        }
    }
}