// File create date:3/22/2018
using UnityEngine;

namespace RoachGame.Widgets {
    /// <summary>
    /// 基础滑动UI面板，派生自BaseView
    /// </summary>
    public abstract class BaseMobilePanel : BaseView {

        /// <summary>
        /// 滑动方向
        /// </summary>
        public enum SlideDirection {
            UP,
            DOWN,
            LEFT,
            RIGHT
        }

        public SlideDirection slideDirection;
        [Tooltip("滑动距离，如果为0则会自动测量宽度作为距离")]
        public float slideDistance = 0f;

        protected NormalGUITranslator guiTranslate; // 窗体滑动工具
        protected RectTransform rectTransform;

        protected Vector2 startPosition;
        protected Vector2 endPosition;

        /// <summary>
        /// 是否正在滑动
        /// </summary>
        public bool IsTranslating {
            get {
                return guiTranslate.IsPlaying;
            }
        }

        /// <summary>
        /// 是否已经展开，展开状态特指已经移动到终点的状态
        /// </summary>
        public bool IsExpended {
            get {
                return guiTranslate.IsForward;
            }
        }

        protected override void PreLoad() {
            rectTransform = GetComponent<RectTransform>();
            startPosition = rectTransform.anchoredPosition;
            endPosition = startPosition;
            Vector2 rectSize = new Vector2(rectTransform.rect.width, rectTransform.rect.height);
            switch (slideDirection) {
                case SlideDirection.UP:
                    if (slideDistance == 0f) {
                        slideDistance = rectSize.y / 2f;
                    }
                    endPosition.y += slideDistance;
                    break;
                case SlideDirection.DOWN:
                    if (slideDistance == 0f) {
                        slideDistance = rectSize.y / 2f;
                    }
                    endPosition.y -= slideDistance;
                    break;
                case SlideDirection.LEFT:
                    if (slideDistance == 0f) {
                        slideDistance = rectSize.x / 2f;
                    }
                    endPosition.x -= slideDistance;
                    break;
                case SlideDirection.RIGHT:
                    if (slideDistance == 0f) {
                        slideDistance = rectSize.x / 2f;
                    }
                    endPosition.x += slideDistance;
                    break;
            }
        }

        protected override void LoadMembers() {
            base.LoadMembers();
            guiTranslate = new NormalGUITranslator(rectTransform);
            guiTranslate.beforeTransform = BeforeSlide;
            guiTranslate.afterTransform = AfterSlide;
            guiTranslate.startPosition = startPosition;
            guiTranslate.endPosition = endPosition;
        }

        /// <summary>
        /// 滑动面板
        /// </summary>
        /// <param name="isForward">滑动方向是否为正向</param>
        public void SlidePanel(bool isForward) {
            if (guiTranslate != null) {
                guiTranslate.Transform(isForward);
            }
        }
        /// <summary>
        /// 滑动动画播放前回调
        /// </summary>
        protected virtual void BeforeSlide() {
            //
        }

        /// <summary>
        /// 滑动动画播放后回调
        /// </summary>
        protected virtual void AfterSlide() {
            //
        }
    }
}
