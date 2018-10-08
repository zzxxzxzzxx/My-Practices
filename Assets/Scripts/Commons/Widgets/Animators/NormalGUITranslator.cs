/// File create date:9/27/2018
using DG.Tweening;
using UnityEngine;

namespace RoachGame.Widgets {
    /// <summary>
    /// 通用的GUI组件位移动画工具
    /// </summary>
    public class NormalGUITranslator : AbstractRectTransformation {

        public Vector2 startPosition;
        public Vector2 endPosition;

        public NormalGUITranslator(RectTransform transform) : base(transform) {
            //
        }

        protected override void InitiateAnimation(bool isForward) {
            transformTweener = objectTransform.DOAnchorPos(isForward ? endPosition : startPosition, durationTime).SetEase(Ease.InOutSine);
        }
    }
}
