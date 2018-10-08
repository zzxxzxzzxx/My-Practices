using UnityEngine;
using DG.Tweening;

namespace RoachGame.Widgets {
    /// <summary>
    /// 通用窗体缩放工具
    /// </summary>
    public class NormalWindowScaler : AbstractTransformation {

        public Vector3 shrinkScale = new Vector3(0.1f, 0.1f, 0.1f);
        public Vector3 expandScale = new Vector3(1f, 1f, 1f);

        public NormalWindowScaler(Transform transform) : base(transform) {
            //
        }

        protected override void InitiateAnimation(bool isForward) {
            transformTweener = objectTransform.DOScale(isForward ? expandScale : shrinkScale, durationTime).SetEase(Ease.Linear);
        }
    }
}
