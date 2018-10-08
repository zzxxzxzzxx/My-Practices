using RoachGame.Data;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RoachGame.Widgets {
    /// <summary>
    /// 复杂的重用列表，支持非统一的子项，使用Adapter机制，要求覆盖Adapter中与子项尺寸相关的方法
    /// </summary>
    public class CommonRecycleListView : AbstractListView {

        private const float ITEM_SHIFT_LIMIT = 20f;

        private Transform cacheRoot; // 缓存区域根对象
        private RectTransform sliderRoot; // 滑动区根对象

        private Deque<TriTuple<int, int, GameObject>> sliderContentQueue; // 滑动区域双端队列缓存，三元组（索引，类型，对象）
        private Dictionary<int, float> itemTypeSize; // 子项类型和尺寸映射表
        private List<TriTuple<int, float, float>> listItemSizeList; // 列表全体子项尺寸列表，三元组（类型，当前项目尺寸，所处位置）
        private Dictionary<int, List<GameObject>> backgroundCache; // 列表项后台缓冲
        private Dictionary<int, List<GameObject>> foregroundCache; // 列表项前景缓存

        private float prevPos; // 缓存上一次的列表位置
        private float viewSize; // 视口尺寸
        private float listSize; // 列表尺寸
        private float leftPadding; // 左间隔
        private float rightPadding; // 右间隔
        private float topPadding; // 上间隔
        private float bottomPadding; // 下间隔

        // 公用变量
        public bool isHorizontal = false; // 是否水平方向的列表
        public RectOffset paddingOffset; // 设置项目边距，在水平列表中仅有top和bot生效，在垂直列表中则仅有left和right生效
        public int spacing; // 间距

        /// <summary>
        /// 前置加载
        /// </summary>
        //protected override void preLoad() {
        //    prevStartIndex = 0;
        //    currStartIndex = 0;
        //}

        /// <summary>
        /// 组件加载
        /// </summary>
        protected override void LoadViews() {
            base.LoadViews();
            GameObject cacheRootObj = new GameObject("ObjectCacheRoot");
            cacheRoot = cacheRootObj.transform;
            cacheRoot.SetParent(contentRoot.parent);
            cacheRoot.localPosition = Vector3.zero;
            cacheRoot.localScale = Vector3.one;
            GameObject sliderRootObj = new GameObject("SliderRootObject");
            sliderRoot = sliderRootObj.AddComponent<RectTransform>();
            sliderRoot.SetParent(contentRoot);
            Vector2 posRef = new Vector2(0, 1);
            sliderRoot.anchorMax = posRef;
            sliderRoot.anchorMin = posRef;
            sliderRoot.pivot = posRef;
            sliderRoot.localPosition = Vector3.zero;
            sliderRoot.localScale = Vector3.one;
            leftPadding = paddingOffset.left;
            rightPadding = paddingOffset.right;
            topPadding = paddingOffset.top;
            bottomPadding = paddingOffset.bottom;
            if (isHorizontal) {
                HorizontalLayoutGroup grp = sliderRootObj.AddComponent<HorizontalLayoutGroup>();
                paddingOffset.left = 0;
                paddingOffset.right = 0;
                grp.padding = paddingOffset;
                grp.spacing = spacing;
                grp.childForceExpandHeight = true;
                grp.childControlHeight = true;
                grp.childControlWidth = true;
                ContentSizeFitter fitter = sliderRootObj.AddComponent<ContentSizeFitter>();
                fitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            } else {
                VerticalLayoutGroup grp = sliderRootObj.AddComponent<VerticalLayoutGroup>();
                paddingOffset.top = 0;
                paddingOffset.bottom = 0;
                grp.padding = paddingOffset;
                grp.spacing = spacing;
                grp.childForceExpandWidth = true;
                grp.childControlHeight = true;
                grp.childControlWidth = true;
                ContentSizeFitter fitter = sliderRootObj.AddComponent<ContentSizeFitter>();
                fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            }
        }

        /// <summary>
        /// 成员加载
        /// </summary>
        protected override void LoadMembers() {
            sliderContentQueue = new Deque<TriTuple<int, int, GameObject>>();
            itemTypeSize = new Dictionary<int, float>();
            listItemSizeList = new List<TriTuple<int, float, float>>();
            if (isHorizontal) {
                viewSize = ((RectTransform)transform).rect.width;
            } else {
                viewSize = ((RectTransform)transform).rect.height;
            }
            backgroundCache = new Dictionary<int, List<GameObject>>();
            foregroundCache = new Dictionary<int, List<GameObject>>();
        }

        /// <summary>
        /// 后处理
        /// </summary>
        protected override void PostLoad() {
            if (isHorizontal) {
                sliderRoot.sizeDelta = new Vector2(0, contentRoot.rect.height);
            } else {
                sliderRoot.sizeDelta = new Vector2(contentRoot.rect.width, 0);
            }
        }

        /// <summary>
        /// 刷新方法
        /// </summary>
        public override void UpdateViews() {
            NotifyUpdate();
        }

        /// <summary>
        /// 每帧执行方法
        /// </summary>
        protected override void Execute() {
            if (baseAdapter != null) {
                float offsetPos = isHorizontal ? -contentRoot.localPosition.x : contentRoot.localPosition.y;
                if (UpdateNotifier) {
                    LocalUpdate(offsetPos);
                } else {
                    CheckRecycle(offsetPos);
                }
            }
        }

        /// <summary>
        /// 本地刷新方法，用于数据列表变化时进行刷新
        /// </summary>
        /// <param name="offsetPos">当前列表的偏移量</param>
        private void LocalUpdate(float offsetPos) {
            // 就地刷新
            Vector2 size = contentRoot.sizeDelta;
            float sizeSum = 0f;
            listItemSizeList.Clear();
            sizeSum += (isHorizontal ? leftPadding : topPadding);
            for (int i = 0; i < baseAdapter.getCount(); i++) {
                int viewType = baseAdapter.getObjectType(i);
                float itemSize = CheckSizeByType(viewType);
                TriTuple<int, float, float> itemTuple = new TriTuple<int, float, float>();
                if (i < baseAdapter.getCount() - 1) {
                    itemSize += spacing;
                }
                itemTuple.E1 = viewType;
                itemTuple.E2 = itemSize;
                itemTuple.E3 = sizeSum;
                listItemSizeList.Add(itemTuple);
                sizeSum += itemSize;
            }
            listSize = sizeSum;
            if (isHorizontal) {
                size.x = listSize + rightPadding;
            } else {
                size.y = listSize + bottomPadding;
            }
            contentRoot.sizeDelta = size;
            while (sliderContentQueue.Count > 0) {
                TriTuple<int, int, GameObject> objTuple = sliderContentQueue.DequeueHead();
                ToBackground(objTuple.E2, objTuple.E3);
            }
            if (baseAdapter.getCount() > 0) {
                int offsetIndex = FindIndexByOffset(offsetPos);
                int initIndex = 0;
                if (offsetPos > listSize - viewSize) {
                    // 超过底端，要调整位置
                    int avaIndex = FindIndexByOffset(listSize - viewSize);
                    initIndex = avaIndex;
                    ChangeSliderPos(listItemSizeList[avaIndex].E3);
                    prevPos = listSize - viewSize;
                } else {
                    initIndex = offsetIndex;
                    ChangeSliderPos(listItemSizeList[0].E3);
                    prevPos = offsetPos;
                }
                while (initIndex < baseAdapter.getCount() && listItemSizeList[initIndex].E3 <= listSize) {
                    WakeupItemToPosition(initIndex);
                    TriTuple<int, float, float> checkTuple = listItemSizeList[initIndex];
                    initIndex++;
                    if (checkTuple.E3 + checkTuple.E2 >= prevPos + viewSize) {
                        break;
                    }
                }
            }
            UpdateNotifier = false;
        }

        /// <summary>
        /// 每帧检查重用状态
        /// </summary>
        /// <param name="offsetPos">当前列表偏移量</param>
        private void CheckRecycle(float offsetPos) {
            if (listSize > viewSize && baseAdapter.getCount() > 0) {
                TriTuple<int, int, GameObject> headTuple = sliderContentQueue.PeekHead();
                TriTuple<int, int, GameObject> tailTuple = sliderContentQueue.PeekTail();
                TriTuple<int, float, float> headSize = listItemSizeList[headTuple.E1];
                TriTuple<int, float, float> tailSize = listItemSizeList[tailTuple.E1];
                if (offsetPos > prevPos && offsetPos <= listSize - viewSize) { // 列表被向上推，顶端推出，底端推入；限制在触底后不进行处理
                    if (offsetPos - prevPos <= headSize.E2) { // 单帧滑动距离在一个子项范围内时
                        if (offsetPos > headSize.E3 + headSize.E2 + ITEM_SHIFT_LIMIT) { // 顶部最上的元素的底已经被推出视口之外且多移动了预定的距离
                            sliderContentQueue.DequeueHead();
                            ToBackground(headTuple.E2, headTuple.E3);
                            ChangeSliderPos(listItemSizeList[headTuple.E1 + 1].E3);
                        }
                        float botPos = offsetPos + viewSize;
                        if (botPos > tailSize.E3 + ITEM_SHIFT_LIMIT) { // 底部最底元素的顶露出预定距离后
                            int newIndex = tailTuple.E1 + 1;
                            if (newIndex < baseAdapter.getCount()) {
                                WakeupItemToPosition(newIndex);
                            }
                        }
                    } else { // 否则找到推出项目数，重排队列和重设滑动区域位置
                        int curIndex = FindIndexByOffset(offsetPos);
                        for (int i = headTuple.E1; i <= curIndex; i++) {
                            if (sliderContentQueue.Count > 0) {
                                headTuple = sliderContentQueue.DequeueHead();
                                ToBackground(headTuple.E2, headTuple.E3);
                            }
                        }
                        if (sliderContentQueue.Count > 0) { // 是否已经滑过整个滑动区域的距离
                            int headIndex = sliderContentQueue.PeekHead().E1;
                            int cusorIndex = tailTuple.E1 + 1;
                            headSize = listItemSizeList[headIndex];
                            ChangeSliderPos(listItemSizeList[headIndex].E3);
                            while (cusorIndex < baseAdapter.getCount() && listItemSizeList[cusorIndex].E3 <= listSize) {
                                WakeupItemToPosition(cusorIndex);
                                TriTuple<int, float, float> checkTuple = listItemSizeList[cusorIndex];
                                cusorIndex++;
                                if (checkTuple.E3 + checkTuple.E2 >= offsetPos + viewSize) {
                                    break;
                                }
                            }
                        } else { // 单帧内就滑过了整个滑动区域的距离，需要重新排队
                            int headIndex = Mathf.Max(curIndex - 1, 0);
                            int cusorIndex = headIndex;
                            headSize = listItemSizeList[headIndex];
                            ChangeSliderPos(listItemSizeList[headIndex].E3);
                            while (cusorIndex < baseAdapter.getCount() && listItemSizeList[cusorIndex].E3 <= listSize) {
                                WakeupItemToPosition(cusorIndex);
                                TriTuple<int, float, float> checkTuple = listItemSizeList[cusorIndex];
                                cusorIndex++;
                                if (checkTuple.E3 + checkTuple.E2 >= offsetPos + viewSize) {
                                    break;
                                }
                            }
                        }
                    }
                } else if (offsetPos < prevPos && offsetPos >= 0) { // 列表被向下推，顶端推入，底端推出；限制在触顶后不进行处理
                    if (prevPos - offsetPos <= tailSize.E2) { // 单帧滑动距离在一个子项范围内时
                        float botPos = offsetPos + viewSize;
                        if (botPos < tailSize.E3 - ITEM_SHIFT_LIMIT) { // 底部最下元素的顶已经推出视口之外且多移动了预定的距离
                            sliderContentQueue.DequeueTail();
                            ToBackground(tailTuple.E2, tailTuple.E3);
                        }
                        if (offsetPos < headSize.E3 + ITEM_SHIFT_LIMIT) { // 顶部最上的元素的顶接近视口达到预定距离
                            int newIndex = headTuple.E1 - 1;
                            if (newIndex >= 0) {
                                WakeupItemToPosition(newIndex, true);
                                ChangeSliderPos(listItemSizeList[newIndex].E3);
                            }
                        }
                    } else { // 否则找到推出项目数，重排队列和重设滑动区域位置
                        int curIndex = FindIndexByOffset(offsetPos + viewSize) + 1;
                        for (int i = curIndex; i <= tailTuple.E1; i++) {
                            if (sliderContentQueue.Count > 0) {
                                tailTuple = sliderContentQueue.DequeueTail();
                                ToBackground(tailTuple.E2, tailTuple.E3);
                            }
                        }
                        if (sliderContentQueue.Count > 0) { // 是否已经滑过整个滑动区域的距离
                            int headIndex = FindIndexByOffset(offsetPos) - 1;
                            headIndex = Mathf.Max(headIndex, 0);
                            int cusorIndex = headTuple.E1 - 1;
                            headSize = listItemSizeList[headIndex];
                            ChangeSliderPos(listItemSizeList[headIndex].E3);
                            while (cusorIndex >= headIndex) {
                                WakeupItemToPosition(cusorIndex, true);
                                cusorIndex--;
                            }
                        } else { // 单帧内就滑过了整个滑动区域的距离，需要重新排队
                            int headIndex = FindIndexByOffset(offsetPos) - 1;
                            headIndex = Mathf.Max(headIndex, 0);
                            int cusorIndex = headIndex;
                            headSize = listItemSizeList[headIndex];
                            ChangeSliderPos(listItemSizeList[headIndex].E3);
                            while (cusorIndex < baseAdapter.getCount() && listItemSizeList[cusorIndex].E3 <= listSize) {
                                WakeupItemToPosition(cusorIndex);
                                TriTuple<int, float, float> checkTuple = listItemSizeList[cusorIndex];
                                cusorIndex++;
                                if (checkTuple.E3 + checkTuple.E2 >= offsetPos + viewSize) {
                                    break;
                                }
                            }
                        }
                    }
                }
                prevPos = offsetPos;
            }
        }

        /// <summary>
        /// 根据子项类型获取尺寸，有缓存
        /// </summary>
        /// <param name="viewType">子项类型</param>
        /// <returns>尺寸</returns>
        private float CheckSizeByType(int viewType) {
            if (itemTypeSize.ContainsKey(viewType)) {
                return itemTypeSize[viewType];
            } else {
                float result = baseAdapter.getSizeByType(viewType);
                itemTypeSize[viewType] = result;
                return result;
            }
        }

        /// <summary>
        /// 将滑动区域的坐标改变到合适的位置
        /// </summary>
        /// <param name="targetPos">目标位置偏移量</param>
        private void ChangeSliderPos(float targetPos) {
            Vector3 pos = sliderRoot.localPosition;
            if (isHorizontal) {
                pos.x = targetPos;
            } else {
                pos.y = -targetPos;
            }
            sliderRoot.localPosition = pos;
        }

        /// <summary>
        /// 尝试从后台缓冲中唤醒一个可用的子项，如果无法找到则返回null
        /// </summary>
        /// <param name="position">位置索引</param>
        /// <param name="toHead">是否插入滑动区域头部</param>
        private void WakeupItemToPosition(int position, bool toHead = false) {
            GameObject obj = FromBackground(listItemSizeList[position].E1);
            obj = baseAdapter.getObject(obj, position);
            ToForeground(listItemSizeList[position].E1, obj, toHead);
            TriTuple<int, int, GameObject> newTuple = new TriTuple<int, int, GameObject>();
            newTuple.E1 = position;
            newTuple.E2 = listItemSizeList[position].E1;
            newTuple.E3 = obj;
            if (toHead) {
                sliderContentQueue.EnqueueHead(newTuple);
            } else {
                sliderContentQueue.EnqueueTail(newTuple);
            }
        }

        /// <summary>
        /// 通过偏移量寻找对应的位置索引，二分法查找
        /// </summary>
        /// <param name="offset">偏移量</param>
        /// <returns>最接近的位置索引</returns>
        private int FindIndexByOffset(float offset) {
            int l = 0;
            int r = listItemSizeList.Count - 1;
            int mid = (l + r) / 2;
            while (l < r - 1 && listItemSizeList[mid].E3 != offset) {
                if (listItemSizeList[mid].E3 > offset) {
                    r = mid;
                } else {
                    l = mid;
                }
                mid = (l + r) / 2;
            }
            if (listItemSizeList[mid].E3 == offset) {
                return mid;
            } else {
                return l;
            }
        }

        /// <summary>
        /// 辅助方法，从后台缓冲中取得子项
        /// </summary>
        /// <param name="viewType">子项类型</param>
        /// <returns>子项对象</returns>
        private GameObject FromBackground(int viewType) {
            GameObject obj = null;
            List<GameObject> backgroundList = backgroundCache.TryGetElement(viewType);
            if (backgroundList == null) {
                backgroundList = new List<GameObject>();
                backgroundCache[viewType] = backgroundList;
            }
            if (backgroundList.Count > 0) {
                obj = backgroundList[0];
                backgroundList.RemoveAt(0);
            }
            return obj;
        }

        /// <summary>
        /// 辅助方法，将子项放入后台缓冲
        /// </summary>
        /// <param name="viewType">子项类型</param>
        /// <param name="obj">子项对象</param>
        private void ToBackground(int viewType, GameObject obj) {
            List<GameObject> backgroundList = backgroundCache.TryGetElement(viewType);
            if (backgroundList == null) {
                backgroundList = new List<GameObject>();
                backgroundCache[viewType] = backgroundList;
            }
            List<GameObject> foregroundList = foregroundCache.TryGetElement(viewType);
            if (foregroundList != null && foregroundList.Contains(obj)) {
                foregroundList.Remove(obj);
            }
            obj.SetActive(false);
            obj.transform.SetParent(cacheRoot);
            backgroundList.Add(obj);
        }

        /// <summary>
        /// 辅助方法，将子项从前台展示中取出
        /// </summary>
        /// <param name="viewType">子项类型</param>
        /// <returns>子项对象</returns>
        private GameObject FromForeground(int viewType) {
            GameObject obj = null;
            List<GameObject> foregroundList = foregroundCache.TryGetElement(viewType);
            if (foregroundList == null) {
                foregroundList = new List<GameObject>();
                backgroundCache[viewType] = foregroundList;
            }
            if (foregroundList.Count > 0) {
                obj = foregroundList[0];
                foregroundList.RemoveAt(0);
            }
            return obj;
        }

        /// <summary>
        /// 辅助方法，将子项放入前台展示中
        /// </summary>
        /// <param name="viewType">子项类型</param>
        /// <param name="obj">子项对象</param>
        /// <param name="isHead">是否设置为第一个</param>
        private void ToForeground(int viewType, GameObject obj, bool isHead = false) {
            List<GameObject> foregroundList = foregroundCache.TryGetElement(viewType);
            if (foregroundList == null) {
                foregroundList = new List<GameObject>();
                foregroundCache[viewType] = foregroundList;
            }
            List<GameObject> backgroundList = backgroundCache.TryGetElement(viewType);
            if (backgroundList != null && backgroundList.Contains(obj)) {
                backgroundList.Remove(obj);
            }
            obj.transform.SetParent(sliderRoot);
            obj.transform.localScale = Vector3.one;
            obj.transform.localPosition = Vector3.zero;
            if (isHead) {
                obj.transform.SetAsFirstSibling();
            }
            obj.SetActive(true);
            foregroundList.Add(obj);
        }
    }
}
