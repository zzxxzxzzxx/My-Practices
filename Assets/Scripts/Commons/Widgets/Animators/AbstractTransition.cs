// File create date:4/22/2018
using DG.Tweening;
using System;

// Created By Yu.Liu
namespace RoachGame.Widgets {
    /// <summary>
    /// 切换动画工具抽象基类，用于派生各种切换动画工具类
    /// </summary>
    public abstract class AbstractTransition {
        protected Tweener transitTweener;
        public Action beforeTransit;
        public Action afterTransit;
        protected bool isPlayingAnimate = false; // 是否正在播放动画
        public bool IsPlaying {
            get {
                return isPlayingAnimate;
            }
        }
        protected bool isTransitOut = false;
        public bool IsTransitOut {
            get {
                return isTransitOut;
            }
        }
        private string targetScene;
        public string TargetScene {
            get {
                return targetScene;
            }
        }
        public float durationTime = 1f;

        public void Transit(bool isTransitOut, string target = null) {
            this.isTransitOut = isTransitOut;
            if (target != null) {
                targetScene = target;
            }
            InitiateAnimation(isTransitOut);
            transitTweener.OnStart(onBeforeAnimate);
            transitTweener.OnComplete(onAfterAnimate);
            transitTweener.PlayForward();
        }

        protected abstract void InitiateAnimation(bool isTransitOut);

        protected virtual void onBeforeAnimate() {
            isPlayingAnimate = true;
            if (beforeTransit != null) {
                beforeTransit();
            }
        }

        protected virtual void onAfterAnimate() {
            if (afterTransit != null) {
                afterTransit();
            }
            isPlayingAnimate = false;
        }
    }
}
