using RoachGame.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RoachGame.Widgets {
    /// <summary>
    /// 自由滑动容器，可以放入任何物体滑动查看
    /// </summary>
    public class FreeContentScroller : BaseView {

        private const string COMMON_CONTENT_PATH = "Viewport/Content"; // 内容根对象搜索路径
        private const int CONTENT_CACHE_LIMIT = 20; // 缓存池大小

        private string currContentUri; // 当前内容的URI，也就是Prefab的URI

        public Transform contentRoot; // 主要容器
        private Transform cacheRoot; // 缓存根对象
        private GameObject contentObj; // 内容对象

        private Dictionary<string, GameObject> contentCache; // 缓存对象池

        /// <summary>
        /// 加载组件
        /// </summary>
        protected override void LoadViews() {
            if (contentRoot == null) {
                contentRoot = transform.Find(COMMON_CONTENT_PATH);
            }
            GameObject cacheRootObj = new GameObject("ObjectCacheRoot");
            cacheRoot = cacheRootObj.transform;
            cacheRoot.SetParent(contentRoot.parent);
            cacheRoot.localPosition = Vector3.zero;
            HorizontalLayoutGroup grp = contentRoot.gameObject.AddComponent<HorizontalLayoutGroup>();
            grp.childForceExpandWidth = true;
            grp.childForceExpandHeight = true;
            grp.childControlHeight = true;
            grp.childControlWidth = true;
            ContentSizeFitter fitter = contentRoot.gameObject.AddComponent<ContentSizeFitter>();
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            fitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
        }
        /// <summary>
        /// 加载和创建数据成员
        /// </summary>
        protected override void LoadMembers() {
            contentCache = new Dictionary<string, GameObject>();
        }

        protected override void Execute() {
            //
        }
        /// <summary>
        /// 更换滑动内容的协程
        /// </summary>
        /// <returns></returns>
        private IEnumerator doChangeContent() {
            yield return null;
            contentObj = ApplyFromCache(currContentUri);
        }
        /// <summary>
        /// 更换滑动内容的协程（直接对象）
        /// </summary>
        /// <returns></returns>
        private IEnumerator doChangeContentObject() {
            yield return null;
            contentObj.transform.SetParent(contentRoot);
            contentObj.transform.localScale = Vector3.one;
            contentObj.transform.localPosition = Vector3.zero;
            contentObj.SetActive(true);
            contentCache[currContentUri] = contentObj;
        }
        /// <summary>
        /// 更换滑动内容到URI指定对象，原来的对象缓存
        /// </summary>
        /// <param name="uri">指定Prefab的URI</param>
        public void ChangeContent(string uri) {
            PushToCache(currContentUri, contentObj);
            currContentUri = uri;
            StartCoroutine(doChangeContent());
        }
        /// <summary>
        /// 更换滑动内容到指定对象，原来的对象缓存
        /// </summary>
        /// <param name="tag">指定对象的标识符</param>
        /// <param name="obj">指定对象本身</param>
        public void ChangeContent(string tag, GameObject obj = null) {
            PushToCache(currContentUri, contentObj);
            currContentUri = tag;
            contentObj = obj;
            if (obj == null) {
                contentObj = contentCache.TryGetElement(tag);
            }
            if (contentObj != null) {
                StartCoroutine(doChangeContentObject());
            } else {
                throw new System.NotSupportedException("Cannot find Map content!");
            }
        }
        /// <summary>
        /// 从缓存中申请对象
        /// </summary>
        /// <param name="uri">URI作为标识符</param>
        /// <returns>取到的对象</returns>
        private GameObject ApplyFromCache(string uri) {
            GameObject obj = null;
            if (contentCache != null) {
                obj = contentCache.TryGetElement(uri);
                if (obj == null) {
                    obj = ResourceUtils.createFromPrefab<GameObject>(uri);
                } else {
                    contentCache.Remove(uri);
                }
            }
            obj.transform.SetParent(contentRoot);
            obj.transform.localScale = Vector3.one;
            obj.transform.localPosition = Vector3.zero;
            obj.SetActive(true);
            return obj;
        }
        /// <summary>
        /// 用完的对象压入缓存
        /// </summary>
        /// <param name="uri">URI作为标识符</param>
        /// <param name="obj">压入的对象</param>
        private void PushToCache(string uri, GameObject obj) {
            if (obj != null && contentCache != null && !contentCache.ContainsKey(uri)) {
                if (contentCache.Count >= 20) {
                    int i = 0;
                    List<string> keys = new List<string>();
                    foreach (string key in contentCache.Keys) {
                        keys.Add(key);
                        i++;
                        if (i >= (CONTENT_CACHE_LIMIT / 2 - 1)) {
                            break;
                        }
                    }
                    foreach (string key in keys) {
                        contentCache.Remove(key);
                    }
                }
                obj.SetActive(false);
                obj.transform.SetParent(cacheRoot);
                contentCache[uri] = obj;
            }
        }
    }
}
