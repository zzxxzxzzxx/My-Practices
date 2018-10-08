using RoachGame.Events;
using UnityEngine;

namespace RoachGame.Widgets {
    /// <summary>
    /// 抽象列表基类，包含各种具体列表通用的方法
    /// </summary>
    public abstract class AbstractListView : BaseView {

        protected const string COMMON_LIST_CONTENT_PATH = "Viewport/Content"; // 通用列表项目集合的搜索路径
        protected BaseAdapter baseAdapter; // 列表适配器
        public RectTransform contentRoot; // 内容根对象
        protected int innerItemLimit = 60; // 每帧刷新项目数上限
        public int InnerItemLimit {
            set {
                innerItemLimit = value;
            }
        }

        protected override void LoadViews() {
            base.LoadViews();
            if (contentRoot == null) {
                contentRoot = (RectTransform)transform.Find(COMMON_LIST_CONTENT_PATH);
            }
        }

        /// <summary>
        /// 事件转发
        /// </summary>
        /// <param name="e">收到的消息</param>
        protected override void handleEvent(MBaseEvent e) {
            if (e != null) {
                e.dispatchTarget = RelatedObject;
                SendEvent(e);
            }
        }

        /// <summary>
        /// 设置适配器
        /// </summary>
        /// <param name="adapter">适配器</param>
        public void setAdapter(BaseAdapter adapter) {
            baseAdapter = adapter;
            baseAdapter.setupListReference(this); // 装载列表引用，观察者模式成立
            baseAdapter.setRelatedObject(TagName);
            UpdateViews();
        }

        public BaseAdapter getAdapter() {
            return baseAdapter;
        }
    }
}
