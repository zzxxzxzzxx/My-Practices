using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RoachGame.Widgets {
    /// <summary>
    /// 简单的可滑动网格布局，使用Adapter机制
    /// </summary>
    public class PlainGridView : AbstractListView {

        protected int overIndex; // 全局索引
        protected int prevCount; // 缓存上一次的项目数量
        protected Dictionary<int, GameObject> objectCache; // 列表项目缓存，改善效率

        // 公共成员
        public bool isHorizontal = false;
        public int rowCount = 4;
        public int colCount = 4;
        public float scrollBarOffset;
        public Vector2 itemSize; // 子项大小尺寸，仅有一个维度受约束，另一个维度随着列表尺寸变化而变化
        public RectOffset itemPadding;
        public Vector2 itemSpacing;

        protected override void PreLoad() {
            base.PreLoad();
            overIndex = 0;
            prevCount = 0;
        }

        protected override void LoadViews() {
            base.LoadViews();
            if (isHorizontal) {
                float realSize = ((RectTransform)transform).rect.height - scrollBarOffset - itemPadding.top - itemPadding.bottom - (itemSpacing.y * (rowCount - 1));
                itemSize.y = realSize / rowCount;
                GridLayoutGroup grp = contentRoot.gameObject.AddComponent<GridLayoutGroup>();
                grp.cellSize = itemSize;
                grp.padding = itemPadding;
                grp.spacing = itemSpacing;
                grp.startAxis = GridLayoutGroup.Axis.Vertical;
                grp.constraint = GridLayoutGroup.Constraint.FixedRowCount;
                grp.constraintCount = rowCount;
                ContentSizeFitter fitter = contentRoot.gameObject.AddComponent<ContentSizeFitter>();
                fitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            } else {
                float realSize = ((RectTransform)transform).rect.width - scrollBarOffset - itemPadding.left - itemPadding.right - (itemSpacing.x * (colCount - 1));
                itemSize.x = realSize / colCount;
                GridLayoutGroup grp = contentRoot.gameObject.AddComponent<GridLayoutGroup>();
                grp.cellSize = itemSize;
                grp.padding = itemPadding;
                grp.spacing = itemSpacing;
                grp.startAxis = GridLayoutGroup.Axis.Horizontal;
                grp.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
                grp.constraintCount = colCount;
                ContentSizeFitter fitter = contentRoot.gameObject.AddComponent<ContentSizeFitter>();
                fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            }
        }

        protected override void LoadMembers() {
            objectCache = new Dictionary<int, GameObject>();
        }

        // 设置更新标志，重置索引，启动更新过程
        public override void UpdateViews() {
            overIndex = 0;
            NotifyUpdate();
        }

        // 每帧执行，根据更新标志进行更新
        protected override void Execute() {
            if (baseAdapter != null && UpdateNotifier) {
                if (baseAdapter.getCount() > 0) {
                    int innerCount = isHorizontal ? rowCount : colCount; // 计算每帧需要更新的项目数
                    InnerUpdate(innerCount);
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
        /// 内循环更新
        /// </summary>
        /// <param name="innerCount">内循环更新次数</param>
        private void InnerUpdate(int innerCount) {
            for (int i = 0; i < innerCount; i++) { // 内循环
                FromCache(overIndex);
                overIndex++;
                if (overIndex >= baseAdapter.getCount()) { // 全部项目更新完成
                    UpdateNotifier = false;
                    while (overIndex < prevCount) { // 更新项目少于已有的项目
                        ToCache(overIndex++);
                    }
                    prevCount = baseAdapter.getCount();
                    break;
                }
            }
        }
        /// <summary>
        /// 从缓存中取出子项进行设置
        /// </summary>
        /// <param name="index">总体索引</param>
        /// <returns>子项</returns>
        private GameObject FromCache(int index) {
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
            } else {
                throw new NotSupportedException("Item need LayoutElement Component!");
            }
            return obj;
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
