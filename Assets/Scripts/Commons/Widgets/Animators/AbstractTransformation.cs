// File create date:4/14/2018
using System;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

// Created By Yu.Liu
namespace RoachGame.Widgets {
    /// <summary>
    /// 形变位移动画工具抽象基类，用于派生可往复的形变和位移动画工具
    /// </summary>
    public abstract class AbstractTransformation {
        protected Transform objectTransform;
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
        public bool isLocal = false; // 是否根据localPosition进行动画

        public AbstractTransformation(Transform transform) {
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
            if(beforeTransform != null) {
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