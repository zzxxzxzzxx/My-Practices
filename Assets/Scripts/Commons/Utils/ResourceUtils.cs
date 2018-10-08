using System;
using System.Collections.Generic;
using UnityEngine;

namespace RoachGame.Utils {
    /// <summary>
    /// 资源加载工具
    /// </summary>
    public class ResourceUtils {
        public const int CACHE_LIMIT = 64; // 缓存数量
        /// <summary>
        /// 资源缓存
        /// </summary>
        private static Dictionary<string, UnityEngine.Object> resourceCache = new Dictionary<string, UnityEngine.Object>();
        private static Dictionary<string, UnityEngine.Object> ResourceCache {
            get {
                if (resourceCache.Count >= CACHE_LIMIT) {
                    List<string> keyList = new List<string>(resourceCache.Keys);
                    for (int i = 0; i < 10; i++) {
                        resourceCache.Remove(keyList[i]);
                    }
                }
                return resourceCache;
            }
        }
        private static Dictionary<string, Sprite> staticSpriteCache = new Dictionary<string, Sprite>();
        /// <summary>
        /// 从Resource文件中加载Sprite
        /// </summary>
        /// <param name="uri">Sprite的路径</param>
        /// <param name="index">分割型Sprite的索引</param>
        /// <returns>Sprite对象</returns>
        public static Sprite loadSpriteByUri(string uri, int index = -1) {
            Sprite result = null;
            if (TextUtils.hasData(uri)) {
                if (index == -1) { // 加载单个Sprite
                    result = ResourceCache.TryGetElement(uri) as Sprite;
                    if (result == null) {
                        result = Resources.Load<Sprite>(uri);
                        ResourceCache[uri] = result;
                    }
                } else { // 加载多个Sprite中的一个
                    result = ResourceCache.TryGetElement(uri + "_" + index) as Sprite;
                    if (result == null) {
                        Sprite[] spSet = Resources.LoadAll<Sprite>(uri);
                        result = spSet[index];
                        for (int i = 0; i < spSet.Length; i++) {
                            string key = uri + "_" + i;
                            if (!ResourceCache.ContainsKey(key)) {
                                ResourceCache[key] = spSet[i];
                            }
                        }
                    }
                }
            }
            return result;
        }
        /// <summary>
        /// 从Resource文件中加载Sprite，静态缓存，禁止加载分割型Sprite
        /// </summary>
        /// <param name="uri">Sprite的路径</param>
        /// <returns>Sprite对象</returns>
        public static Sprite loadSpriteWithCache(string uri) {
            Sprite result = null;
            if (TextUtils.hasData(uri)) {
                result = staticSpriteCache.TryGetElement(uri);
                if (result == null) {
                    result = Resources.Load<Sprite>(uri);
                    staticSpriteCache[uri] = result;
                }
            }
            return result;
        }
        /// <summary>
        /// 从预制对象创建新物体
        /// </summary>
        /// <typeparam name="T">预制对象的类型</typeparam>
        /// <param name="uri">预制对象的URI</param>
        /// <returns>新建对象</returns>
        public static T createFromPrefab<T>(string uri, Transform parent = null) where T : UnityEngine.Object {
            if (TextUtils.hasData(uri)) {
                T prefab = ResourceCache.TryGetElement(uri) as T;
                if (prefab == null) {
                    prefab = Resources.Load<T>(uri);
                    ResourceCache[uri] = prefab;
                }
                if (parent != null) {
                    return UnityEngine.Object.Instantiate(prefab, parent);
                } else {
                    return UnityEngine.Object.Instantiate(prefab);
                }
            }
            throw new NoSuchUriException();
        }
        /// <summary>
        /// 从Resource目录加载指定资源
        /// </summary>
        /// <typeparam name="T">资源类型</typeparam>
        /// <param name="uri">资源URI</param>
        /// <returns>资源对象</returns>
        public static T loadResource<T>(string uri) where T : UnityEngine.Object {
            if (TextUtils.hasData(uri)) {
                T res = ResourceCache.TryGetElement(uri) as T;
                if (res == null) {
                    res = Resources.Load<T>(uri);
                    ResourceCache[uri] = res;
                }
                return res;
            }
            throw new NoSuchUriException();
        }
        /// <summary>
        /// 找不到指定URI对应资源的异常
        /// </summary>
        public class NoSuchUriException : ApplicationException {
            private string error;
            //无参数构造函数
            public NoSuchUriException() {
                error = "There is no such target, please check the uri string!";
            }
            //带一个字符串参数的构造函数，作用：当程序员用Exception类获取异常信息而非NoSuchUriException时把自定义异常信息传递过去
            public NoSuchUriException(string msg) : base(msg) {
                error = msg;
            }
            public string GetError() {
                return error;
            }
        }
    }
}
