using RoachGame.Events;

namespace RoachGame.Widgets {
    /// <summary>
    /// 基础弹窗，派生自BaseView类，提供BeforeScale，AfterScale以及BeforeShowPopupWindow方法可供重载
    /// </summary>
    public abstract class BasePopupWindow : BaseView {

        private NormalWindowScaler windowScaler; // 弹窗缩放工具

        public bool isShowing {
            get {
                if (windowScaler != null) {
                    return windowScaler.IsForward;
                }
                return false;
            }
        }

        protected override void LoadMembers() {
            base.LoadMembers();
            windowScaler = new NormalWindowScaler(transform);
            windowScaler.durationTime = 0.2f;
            windowScaler.beforeTransform = BeforeScale;
            windowScaler.afterTransform = AfterScale;
        }

        protected override void PostLoad() {
            base.PostLoad();
            hidePop();
        }

        protected override void handleEvent(MBaseEvent e) {
            if (e.Type == CommonConfigs.EVENT_COMMON_UI_FUNCTION) {
                if (e.what == CommonConfigs.SUB_EVENT_COMMON_FUNC_OPEN) {
                    BeforeShowPopupWindow(e);
                    showPop();
                }
            }
        }

        /// <summary>
        /// 显示弹窗
        /// </summary>
        public void showPop() {
            if (windowScaler != null && !windowScaler.IsForward && !windowScaler.IsPlaying) {
                transform.SetAsLastSibling();
                windowScaler.Transform(true);
            }
        }

        /// <summary>
        /// 关闭弹窗
        /// </summary>
        public void dismissPop() {
            if (windowScaler != null && windowScaler.IsForward && !windowScaler.IsPlaying) {
                windowScaler.Transform(false);
            }
        }

        /// <summary>
        /// 初始化隐藏弹窗
        /// </summary>
        protected void hidePop() {
            transform.localScale = windowScaler.shrinkScale;
            gameObject.SetActive(false);
        }

        /// <summary>
        /// 缩放动画开始前处理，可重写
        /// </summary>
        protected virtual void BeforeScale() {

        }

        /// <summary>
        /// 缩放动画结束后处理，可重写
        /// </summary>
        protected virtual void AfterScale() {
            if (windowScaler.IsForward) {
                gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// 处理打开弹窗时的额外参数
        /// </summary>
        protected virtual void BeforeShowPopupWindow(MBaseEvent e) { }
    }
}
