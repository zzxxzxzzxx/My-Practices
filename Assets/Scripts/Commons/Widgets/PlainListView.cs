using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RoachGame.Widgets {
    /// <summary>
    /// 简单的列表，仅支持统一的子项，使用Adapter机制
    /// </summary>
    public class PlainListView : AbstractListView {

        protected int overIndex; // 全局索引
        protected int prevCount; // 缓存上一次的项目数量

        protected Dictionary<int, GameObject> objectCache; // 列表项目缓存，改善效率
        protected float itemSize; // 子项尺寸，用于计算当前索引
        protected int curPosition = 0; // 当前列表浏览位置
        protected bool isFirstDone = false; // 第一部分是否更新完毕

        // 公用设置变量
        public bool isHorizontal = false; // 设置是否水平布局（默认垂直布局，即普通列表）
        public RectOffset paddingOffset; // 设置项目边距
        public int spacing; // 设置项目间距

        protected override void PreLoad() {
            overIndex = 0;
            prevCount = 0;
        }

        protected override void LoadViews() {
            base.LoadViews();
            if (isHorizontal) {
                HorizontalLayoutGroup grp = contentRoot.gameObject.AddComponent<HorizontalLayoutGroup>();
                grp.padding = paddingOffset;
                grp.spacing = spacing;
                grp.childForceExpandHeight = true;
                grp.childControlHeight = true;
                grp.childControlWidth = true;
                ContentSizeFitter fitter = contentRoot.gameObject.AddComponent<ContentSizeFitter>();
                fitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            } else {
                VerticalLayoutGroup grp = contentRoot.gameObject.AddComponent<VerticalLayoutGroup>();
                grp.padding = paddingOffset;
                grp.spacing = spacing;
                grp.childForceExpandWidth = true;
                grp.childControlHeight = true;
                grp.childControlWidth = true;
                ContentSizeFitter fitter = contentRoot.gameObject.AddComponent<ContentSizeFitter>();
                fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            }
        }

        protected override void LoadMembers() {
            objectCache = new Dictionary<int, GameObject>();
        }

        // 设置更新标志，重置索引，启动更新过程
        public override void UpdateViews() {
            overIndex = curPosition;
            isFirstDone = false;
            NotifyUpdate();
        }

        // 每帧执行，根据更新标志进行更新
        protected override void Execute() {
            if (itemSize > 0f) {
                curPosition = (int)(isHorizontal ? (Mathf.Abs(contentRoot.localPosition.x) / itemSize) : (Mathf.Abs(contentRoot.localPosition.y) / itemSize));
            }
            if (baseAdapter != null && UpdateNotifier) {
                if (baseAdapter.getCount() > 0) {
                    if (!isFirstDone) {
                        InnerUpdateFirst(innerItemLimit);
                    } else {
                        InnerUpdateSecond(innerItemLimit);
                    }
                } else {
                    foreach (GameObject obj in objectCache.Values) {
                        if (obj.activeSelf) {
                            obj.SetActive(false);
                        }
                    }
                    UpdateNotifier = false;
                }
            }
        }
        /// <summary>
        /// 内循环更新（第一部分）
        /// </summary>
        /// <param name="innerCount">内循环更新次数</param>
        private void InnerUpdateFirst(int innerCount) {
            if (overIndex < baseAdapter.getCount()) {
                for (int i = 0; i < innerCount; i++) { // 内循环
                    FromCache(overIndex);
                    overIndex++;
                    if (overIndex >= baseAdapter.getCount()) { // 第一部分项目更新完成
                        isFirstDone = true;
                        while (overIndex < prevCount) { // 更新项目少于已有的项目
                            ToCache(overIndex++);
                        }
                        prevCount = baseAdapter.getCount();
                        overIndex = 0;
                        break;
                    }
                }
            } else {
                overIndex = baseAdapter.getCount();
                isFirstDone = true;
                while (overIndex < prevCount) { // 更新项目少于已有的项目
                    ToCache(overIndex++);
                }
                prevCount = baseAdapter.getCount();
                overIndex = 0;
            }
        }
        /// <summary>
        /// 内循环更新（第二部分）
        /// </summary>
        /// <param name="innerCount">内循环更新次数</param>
        private void InnerUpdateSecond(int innerCount) {
            for (int i = 0; i < innerCount; i++) { // 内循环
                FromCache(overIndex);
                overIndex++;
                if (overIndex >= baseAdapter.getCount() || overIndex >= curPosition) { // 第二部分项目更新完成
                    UpdateNotifier = false;
                    break;
                }
            }
        }
        /// <summary>
        /// 从缓存中取出子项进行设置
        /// </summary>
        /// <param name="index">总体索引</param>
        private void FromCache(int index) {
            GameObject obj = objectCache.TryGetElement(baseAdapter.getItemId(index));
            bool isNewObj = (obj == null);
            obj = baseAdapter.getObject(obj, index);
            if (isNewObj) {
                obj.transform.SetParent(contentRoot);
                obj.transform.localScale = Vector3.one;
                obj.transform.localPosition = Vector3.zero;
                objectCache[baseAdapter.getItemId(overIndex)] = obj; // 计入缓存
            }
            obj.SetActive(true);
            LayoutElement e = obj.GetComponent<LayoutElement>() as LayoutElement;
            if (e != null) {
                e.ignoreLayout = false;
                itemSize = isHorizontal ? e.preferredWidth : e.preferredHeight;
            } else {
                throw new NotSupportedException("Item need LayoutElement Component!");
            }
        }
        /// <summary>
        /// 将子项放回缓存
        /// </summary>
        /// <param name="index">总体索引</param>
        private void ToCache(int index) {
            GameObject obj = objectCache.TryGetElement(baseAdapter.getItemId(index));
            if (obj != null) {
                LayoutElement e = obj.GetComponent<LayoutElement>() as LayoutElement;
                if (e != null) {
                    e.ignoreLayout = true;
                } else {
                    throw new NotSupportedException("Item need LayoutElement Component!");
                }
                obj.SetActive(false);
            }
        }
    }
}
