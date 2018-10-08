// File create date:3/13/2018
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Created By Yu.Liu
namespace RoachGame.Widgets {
    /// <summary>
    /// 复杂的列表，支持非统一的子项，使用Adapter机制，要求覆盖Adapter中与子项尺寸相关的方法
    /// </summary>
    public class CommonListView : AbstractListView {

        private Transform cacheRoot; // 缓存对象挂载点

        private int overIndex; // 全局索引
        private float prevPos; // 缓存上一次的列表位置
        private float viewSize; // 保存列表视口尺寸
        private float curOffset; // 计算当前列表的高度

        private Dictionary<int, Queue<GameObject>> backgroundCache; // 列表项后台缓冲
        private Dictionary<int, Queue<GameObject>> foregroundCache; // 列表项前景缓存

        // 公用设置变量
        public bool isHorizontal = false; // 设置是否水平布局（默认垂直布局，即普通列表）
        public RectOffset paddingOffset; // 设置项目边距
        public int spacing; // 设置项目间距

        protected override void PreLoad() {
            overIndex = 0;
        }

        protected override void LoadViews() {
            base.LoadViews();
            GameObject cacheRootObj = new GameObject("ObjectCacheRoot"); // 新建缓存对象挂载点
            cacheRoot = cacheRootObj.transform;
            cacheRoot.SetParent(contentRoot.parent);
            cacheRoot.localPosition = Vector3.zero;
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
                    int innerCount = baseAdapter.getCount() / 2;
                    while (innerCount > innerItemLimit) {
                        innerCount /= 2;
                    }
                    innerCount += (innerCount == 0 ? 1 : 0);
                    InnerUpdate(innerCount);
                }
            }
        }
        /// <summary>
        /// 内循环更新
        /// </summary>
        /// <param name="innerCount">内循环更新次数</param>
        private void InnerUpdate(int innerCount) {
            for (int i = 0; i < innerCount; i++) { // 内循环
                int viewType = baseAdapter.getObjectType(overIndex);
                GameObject obj = FromBackground(viewType); // 查询缓存
                obj = baseAdapter.getObject(obj, overIndex); // getObject方法将会判断是否需要新建对象
                obj.transform.SetParent(contentRoot);
                obj.transform.localScale = Vector3.one;
                obj.transform.localPosition = Vector3.zero;
                obj.SetActive(true);
                curOffset += isHorizontal ? ((RectTransform)obj.transform).sizeDelta.x : ((RectTransform)obj.transform).sizeDelta.y;
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
