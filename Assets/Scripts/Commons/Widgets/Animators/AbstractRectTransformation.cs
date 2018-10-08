/// File create date:9/27/2018
using DG.Tweening;
using System;
using UnityEngine;

namespace RoachGame.Widgets {
    /// <summary>
    /// 专用于UGUI的动画工具
    /// </summary>
    public abstract class AbstractRectTransformation {

        protected RectTransform objectTransform;
        protected Tweener transformTweener;
        public Action beforeTransform;
        public Action afterTransform;
        protected bool isPlayingAnimate = false; // 是否正在播放动画
        public bool IsPlaying {
            get {
                return isPlayingAnimate;
            }
        }
        protected bool isForwardFlag = false;
        public bool IsForward {
            get {
                return isForwardFlag;
            }
        }
        private bool isForwardCache;
        public float durationTime = 1f;

        public AbstractRectTransformation(RectTransform transform) {
            objectTransform = transform;
        }

        public void Transform(bool isForward) {
            isForwardCache = isForward;
            InitiateAnimation(isForward);
            transformTweener.OnStart(onBeforeAnimate);
            transformTweener.OnComplete(onAfterAnimate);
            transformTweener.PlayForward();
        }

        protected abstract void InitiateAnimation(bool isForward);

        protected virtual void onBeforeAnimate() {
            isPlayingAnimate = true;
            if (beforeTransform != null) {
                beforeTransform();
            }
        }

        protected virtual void onAfterAnimate() {
            if (afterTransform != null) {
                afterTransform();
            }
            isPlayingAnimate = false;
            isForwardFlag = isForwardCache;
        }
    }
}
