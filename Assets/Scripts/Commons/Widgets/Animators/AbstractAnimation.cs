// File create date:4/22/2018
using DG.Tweening;
using System;
// Created By Yu.Liu
namespace RoachGame.Widgets {
    /// <summary>
    /// 一般动画工具抽象基类，用于派生一般动画工具
    /// </summary>
    public abstract class AbstractAnimation {
        protected Tweener animTweener;
        public Action beforeAnim;
        public Action afterAnim;
        protected bool isPlayingAnimate = false; // 是否正在播放动画
        public bool IsPlaying {
            get {
                return isPlayingAnimate;
            }
        }
        public float durationTime = 1f;

        public void Animate() {
            InitiateAnimation();
            animTweener.OnStart(onBeforeAnimate);
            animTweener.OnComplete(onAfterAnimate);
            animTweener.PlayForward();
        }

        protected abstract void InitiateAnimation();

        protected virtual void onBeforeAnimate() {
            isPlayingAnimate = true;
            if (beforeAnim != null) {
                beforeAnim();
            }
        }

        protected virtual void onAfterAnimate() {
            if (afterAnim != null) {
                afterAnim();
            }
            isPlayingAnimate = false;
        }
    }
}
