// File create date:3/23/2018
using System;
using UnityEngine;
using UnityEngine.UI;
// Created By Yu.Liu
namespace RoachGame.Widgets {
    /// <summary>
    /// 内容切换组件，可以前后无限滚动切换内容
    /// </summary>
    public class CommonViewSwitcher : BaseView {
        // Code Here
        private const string COMMON_SWITCHER_CONTENT_PATH = "Content";

        private NormalObjectTranslator contentForward;
        private NormalObjectTranslator contentBackward;
        public Transform contentRoot;
        private Vector2 contentAreaSize;
        private float translateOffset;

        private GameObject prevView;
        private GameObject showView;
        private GameObject postView;

        private BaseAdapter baseAdapter;
        private int showIndex = 0;
        private bool initFlag = true;

        // --- 公共参数
        public bool isHorizontal = true;

        protected override void LoadViews() {
            base.LoadViews();
            Rect r = ((RectTransform)transform).rect;
            contentAreaSize = new Vector2(r.width, r.height);
            if (contentRoot == null) { // 自动生成内容展示区域
                GameObject root = new GameObject(COMMON_SWITCHER_CONTENT_PATH);
                contentRoot = root.transform;
                contentRoot.SetParent(transform);
                contentRoot.localPosition = Vector3.zero;
                contentRoot.localScale = Vector3.one;
                RectTransform rt = root.AddComponent<RectTransform>();
                rt.pivot = new Vector2(0.5f, 0.5f);
                rt.anchorMax = new Vector2(0.5f, 0.5f);
                rt.anchorMin = new Vector2(0.5f, 0.5f);
                rt.anchoredPosition = Vector2.zero;
                rt.sizeDelta = contentAreaSize;
            }
            // 垂直布局与水平布局
            if (isHorizontal) {
                HorizontalLayoutGroup grp = contentRoot.gameObject.AddComponent<HorizontalLayoutGroup>();
                grp.childControlHeight = true;
                grp.childControlWidth = true;
                grp.childForceExpandHeight = true;
                grp.childForceExpandWidth = true;
                translateOffset = contentAreaSize.x;
            } else {
                VerticalLayoutGroup grp = contentRoot.gameObject.AddComponent<VerticalLayoutGroup>();
                grp.childControlHeight = true;
                grp.childControlWidth = true;
                grp.childForceExpandHeight = true;
                grp.childForceExpandWidth = true;
                translateOffset = contentAreaSize.y;
            }
            // 自动适配尺寸
            ContentSizeFitter fitter = contentRoot.gameObject.AddComponent<ContentSizeFitter>();
            fitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        }

        protected override void LoadMembers() {
            base.LoadMembers();
            // 创建两个移动工具，分别对应向前移动和向后移动
            contentForward = new NormalObjectTranslator(contentRoot);
            contentBackward = new NormalObjectTranslator(contentRoot);
            Vector3 startPos = Vector3.zero;
            Vector3 forwardPos = startPos;
            Vector3 backwardPos = startPos;
            if (isHorizontal) {
                forwardPos.x -= translateOffset;
                backwardPos.x += translateOffset;
            } else {
                forwardPos.y += translateOffset;
                backwardPos.y -= translateOffset;
            }
            contentForward.startPosition = startPos;
            contentForward.endPosition = forwardPos;
            contentBackward.startPosition = startPos;
            contentBackward.endPosition = backwardPos;
            contentForward.beforeTransform = beforeForward;
            contentForward.afterTransform = afterForward;
            contentBackward.beforeTransform = beforeBackward;
            contentBackward.afterTransform = afterBackward;
            contentForward.isLocal = true;
            contentBackward.isLocal = true;
        }

        /// <summary>
        /// 刷新方法，重设当前展示的项目索引
        /// </summary>
        public override void UpdateViews() {
            base.UpdateViews();
            showIndex = 0;
            NotifyUpdate();
        }

        protected override void Execute() {
            if (initFlag) { // 进行初始化
                if (baseAdapter != null && baseAdapter.getCount() > 0) {
                    InitContent();
                    initFlag = false;
                }
            } else {
                if (UpdateNotifier) { // 刷新数据
                    LocalUpdate();
                    UpdateNotifier = false;
                }
            }
        }

        /// <summary>
        /// 初始化组件方法
        /// </summary>
        private void InitContent() {
            // 依次生成三个显示子项，采用循环制，只要项目总数不为零则可以生成
            int refIndex = GetCycleIndex(showIndex - 1);
            prevView = baseAdapter.getObject(null, refIndex);
            LayoutElement element = prevView.GetComponent<LayoutElement>();
            float width = element.preferredWidth;
            float height = element.preferredHeight;
            // 根据子项的尺寸来决定padding和spacing参数
            if (isHorizontal) {
                float spacing = (contentAreaSize.x - width);
                float vPadding = (contentAreaSize.y - height) / 2;
                float hPadding = (contentAreaSize.x - width) / 2;
                HorizontalLayoutGroup grp = contentRoot.gameObject.GetComponent<HorizontalLayoutGroup>();
                grp.spacing = spacing;
                grp.padding = new RectOffset((int)hPadding, (int)hPadding, (int)vPadding, (int)vPadding);
            } else {
                float spacing = (contentAreaSize.y - height);
                float vPadding = (contentAreaSize.y - height) / 2;
                float hPadding = (contentAreaSize.x - width) / 2;
                VerticalLayoutGroup grp = contentRoot.gameObject.GetComponent<VerticalLayoutGroup>();
                grp.spacing = spacing;
                grp.padding = new RectOffset((int)hPadding, (int)hPadding, (int)vPadding, (int)vPadding);
            }
            prevView.transform.SetParent(contentRoot);
            prevView.transform.localScale = Vector3.one;
            // --- 
            refIndex = GetCycleIndex(showIndex);
            showView = baseAdapter.getObject(null, refIndex);
            showView.transform.SetParent(contentRoot);
            showView.transform.localScale = Vector3.one;
            // ---
            refIndex = GetCycleIndex(showIndex + 1);
            postView = baseAdapter.getObject(null, refIndex);
            postView.transform.SetParent(contentRoot);
            postView.transform.localScale = Vector3.one;
        }

        /// <summary>
        /// 本地刷新方法
        /// </summary>
        private void LocalUpdate() {
            if (baseAdapter.getCount() > 0) { // 依次刷新三个位置的子项数据
                int refIndex = GetCycleIndex(showIndex - 1);
                prevView = baseAdapter.getObject(prevView, refIndex);
                prevView.transform.SetParent(contentRoot);
                prevView.transform.localScale = Vector3.one;
                // --- 
                refIndex = GetCycleIndex(showIndex);
                showView = baseAdapter.getObject(showView, refIndex);
                showView.transform.SetParent(contentRoot);
                showView.transform.localScale = Vector3.one;
                // ---
                refIndex = GetCycleIndex(showIndex + 1);
                postView = baseAdapter.getObject(postView, refIndex);
                postView.transform.SetParent(contentRoot);
                postView.transform.localScale = Vector3.one;
                // ---
                showIndex = GetCycleIndex(showIndex);
            }
        }

        /// <summary>
        /// 循环索引获取方法
        /// </summary>
        /// <param name="index">原索引</param>
        /// <returns>处理后索引</returns>
        private int GetCycleIndex(int index) {
            if (baseAdapter.getCount() > 0) {
                while (index <= 0) {
                    index += baseAdapter.getCount();
                }
                return index % baseAdapter.getCount();
            }
            throw new IndexOutOfRangeException("Adapter has no data!");
        }

        /// <summary>
        /// 向后移动一项的前置处理
        /// </summary>
        private void beforeForward() {
            //
        }

        /// <summary>
        /// 向后移动一项的后继处理
        /// </summary>
        private void afterForward() {
            prevView.transform.SetAsLastSibling();
            contentRoot.localPosition = Vector3.zero;
            GameObject temp = postView;
            postView = prevView;
            prevView = showView;
            showView = temp;
            NotifyUpdate();
        }

        /// <summary>
        /// 向前移动一项的前置处理
        /// </summary>
        private void beforeBackward() {
            //
        }

        /// <summary>
        /// 向前移动一项的后继处理
        /// </summary>
        private void afterBackward() {
            postView.transform.SetAsFirstSibling();
            contentRoot.localPosition = Vector3.zero;
            GameObject temp = prevView;
            prevView = postView;
            postView = showView;
            showView = temp;
            NotifyUpdate();
        }

        /// <summary>
        /// 下一个
        /// </summary>
        public void SwitchNext() {
            if (!contentForward.IsPlaying && !contentBackward.IsPlaying) {
                showIndex++;
                contentForward.Transform(true);
            }
        }

        /// <summary>
        /// 上一个
        /// </summary>
        public void SwitchPrev() {
            if (!contentForward.IsPlaying && !contentBackward.IsPlaying) {
                showIndex--;
                contentBackward.Transform(true);
            }
        }

        /// <summary>
        /// 设置数据Adapter
        /// </summary>
        /// <param name="adapter">Adapter实现</param>
        public void SetAdapter(BaseAdapter adapter) { // 设置适配器
            baseAdapter = adapter;
            baseAdapter.setupListReference(this); // 装载引用，观察者模式成立
            baseAdapter.setRelatedObject(TagName);
            UpdateViews();
        }

        /// <summary>
        /// 获取Adapter引用
        /// </summary>
        /// <returns>当前Adapter</returns>
        public BaseAdapter GetAdapter() {
            return baseAdapter;
        }
    }
}
