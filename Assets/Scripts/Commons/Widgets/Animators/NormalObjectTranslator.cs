using UnityEngine;
using DG.Tweening;

namespace RoachGame.Widgets {
    /// <summary>
    /// 通用对象位移动画工具
    /// </summary>
    public class NormalObjectTranslator : AbstractTransformation {

        public Vector3 startPosition;
        public Vector3 endPosition;

        public NormalObjectTranslator(Transform transform) : base(transform) {
            //
        }

        protected override void InitiateAnimation(bool isForward) {
            if (!isLocal) {
                transformTweener = objectTransform.DOMove(isForward ? endPosition : startPosition, durationTime).SetEase(Ease.InOutSine);
            } else {
                transformTweener = objectTransform.DOLocalMove(isForward ? endPosition : startPosition, durationTime).SetEase(Ease.InOutSine);
            }
        }
    }
}
