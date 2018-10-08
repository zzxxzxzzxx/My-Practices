using RoachGame.Events;
using UnityEngine.UI;

namespace RoachGame.Widgets {
    /// <summary>
    /// 列表项基类，由BaseView类派生而来
    /// </summary>
    public abstract class BaseItem : BaseView {

        protected Button objectButton;
        protected int index;

        protected override void LoadViews() {
            objectButton = gameObject.GetComponent<Button>();
            if (objectButton == null) {
                objectButton = gameObject.AddComponent<Button>();
            }
        }

        protected override void SetupTriggers() {
            if (objectButton != null) {
                SetupClickListener(objectButton, onItemClick);
            }
        }

        /// <summary>
        /// 设置当前项目索引
        /// </summary>
        /// <param name="i">索引值</param>
        public void setIndex(int i) {
            index = i;
        }

        protected void onItemClick() {
            SendEvent(EventUtils.GetBuilder(CommonConfigs.EVENT_COMMON_ITEM).SetTarget(RelatedObject, TagName).SetBasic(index).BuildEvent());
        }
    }
}
