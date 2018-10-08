// File create date:3/26/2018
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
// Created By Yu.Liu
namespace RoachGame.Widgets {
    /// <summary>
    /// 复杂的可滑动网格布局，使用Adapter机制，支持多种子项，要求重写Adapter中关于子项类型的方法
    /// </summary>
    public class CommonGridView : AbstractListView {
        // Code Here
        private Transform cacheRoot; // 缓存对象挂载点

        private int overIndex; // 全局索引
        private float prevPos; // 缓存上一次的列表位置
        private float viewSize; // 保存列表视口尺寸
        private float curOffset; // 计算当前列表的高度

        private Dictionary<int, Queue<GameObject>> backgroundCache; // 列表项后台缓冲
        private Dictionary<int, Queue<GameObject>> foregroundCache; // 列表项前景缓存

        // ------ 公共成员
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
        }

        protected override void LoadViews() {
            base.LoadViews();
            GameObject cacheRootObj = new GameObject("ObjectCacheRoot"); // 新建缓存对象挂载点
            cacheRoot = cacheRootObj.transform;
            cacheRoot.SetParent(contentRoot.parent);
            cacheRoot.localPosition = Vector3.zero;
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
            backgroundCache = new Dictionary<int, Queue<GameObject>>();
            foregroundCache = new Dictionary<int, Queue<GameObject>>();
            if (isHorizontal) {
                viewSize = ((RectTransform)transform).rect.width;
            } else {
                viewSize = ((RectTransform)transform).rect.height;
            }
        }

        /// <summary>
        /// 设置更新标志，重置索引，启动更新过程
        /// </summary>
        public override void UpdateViews() {
            overIndex = 0;
            PrepareCaches();
            NotifyUpdate();
        }

        /// <summary>
        /// 每帧执行，根据更新标志进行更新
        /// </summary>
        protected override void Execute() {
            if (baseAdapter != null && UpdateNotifier) {
                if (baseAdapter.getCount() > 0) {
                    int innerCount = isHorizontal ? rowCount : colCount; // 计算每帧需要更新的项目数
                    InnerUpdate(innerCount);
                }
            }
        }
        /// <summary>
        /// 内循环更新
        /// </summary>
        /// <param name="innerCount">内循环更新次数</param>
        private void InnerUpdate(int innerCount) {
            curOffset += isHorizontal ? itemSize.x : itemSize.y;
            for (int i = 0; i < innerCount; i++) { // 内循环
                int viewType = baseAdapter.getObjectType(overIndex);
                GameObject obj = FromBackground(viewType); // 查询缓存
                obj = baseAdapter.getObject(obj, overIndex); // getObject方法将会判断是否需要新建对象
                obj.transform.SetParent(contentRoot);
                obj.transform.localScale = Vector3.one;
                obj.transform.localPosition = Vector3.zero;
                obj.SetActive(true);
                ToForeground(viewType, obj);
                overIndex++;
                if (overIndex >= baseAdapter.getCount()) { // 全部项目更新完成
                    UpdateNotifier = false;
                    Vector3 newPos = contentRoot.localPosition; // 重设列表位置
                    if (curOffset >= prevPos + viewSize) {
                        if (isHorizontal) {
                            newPos.x = prevPos;
                        } else {
                            newPos.y = prevPos;
                        }
                    } else {
                        if (isHorizontal) {
                            newPos.x = curOffset - viewSize;
                        } else {
                            newPos.y = curOffset - viewSize;
                        }
                    }
                    contentRoot.localPosition = newPos;
                    break;
                }
            }
        }
        /// <summary>
        /// 从缓存中取得所需的对象
        /// </summary>
        /// <param name="viewType">子项类型</param>
        /// <returns>取得的对象</returns>
        private GameObject FromBackground(int viewType) {
            GameObject obj = null;
            Queue<GameObject> backgroundQueue = backgroundCache.TryGetElement(viewType);
            if (backgroundQueue == null) {
                backgroundQueue = new Queue<GameObject>();
                backgroundCache[viewType] = backgroundQueue;
            }
            if (backgroundQueue.Count > 0) {
                obj = backgroundQueue.Dequeue();
            }
            return obj;
        }
        /// <summary>
        /// 将对象放入前端缓存
        /// </summary>
        /// <param name="viewType">子项类型</param>
        /// <param name="obj">对象</param>
        private void ToForeground(int viewType, GameObject obj) {
            Queue<GameObject> foregroundQueue = foregroundCache.TryGetElement(viewType);
            if (foregroundQueue == null) {
                foregroundQueue = new Queue<GameObject>();
                foregroundCache[viewType] = foregroundQueue;
            }
            foregroundQueue.Enqueue(obj);
        }
        /// <summary>
        /// 准备缓存空间以及相关参数
        /// </summary>
        private void PrepareCaches() {
            prevPos = isHorizontal ? contentRoot.localPosition.x : contentRoot.localPosition.y;
            curOffset = 0;
            if (foregroundCache != null) { // 将所有在前台展示的对象清理掉
                foreach (int viewType in foregroundCache.Keys) {
                    Queue<GameObject> foregroundQueue = foregroundCache[viewType];
                    Queue<GameObject> backgroundQueue = backgroundCache.TryGetElement(viewType);
                    if (backgroundQueue == null) {
                        backgroundQueue = new Queue<GameObject>();
                        backgroundCache[viewType] = backgroundQueue;
                    }
                    while (foregroundQueue.Count > 0) {
                        GameObject obj = foregroundQueue.Dequeue();
                        obj.SetActive(false);
                        obj.transform.SetParent(cacheRoot);
                        backgroundQueue.Enqueue(obj);
                    }
                }
            }
        }
    }
}
