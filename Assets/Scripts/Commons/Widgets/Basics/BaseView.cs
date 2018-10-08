using RoachGame.Utils;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace RoachGame.Widgets {
    /// <summary>
    /// UI组件基类，可重载PreLoad，LoadViews，LoadMembers，SetupTriggers，PostLoad，UpdateViews等方法
    /// </summary>
    public abstract class BaseView : BaseEventObject, IUpdateViews {

        protected string RelatedObject; // 关联上级组件tag
        protected bool UpdateNotifier = false; // 数据刷新标志位
        /// <summary>
        /// 实现预初始化
        /// </summary>
        protected override void PreInit() {
            try {
                PreLoad();
            } catch (Exception e) {
                LogUtils.logNotice("ERROR in - " + gameObject.name);
                LogUtils.logError(e.Message + "\n" + e.StackTrace);
            }
        }
        /// <summary>
        /// 实现初始化
        /// </summary>
        protected override void Init() {
            try {
                LoadViews();
                LoadMembers();
                SetupTriggers();
                PostLoad();
            } catch (Exception e) {
                LogUtils.logNotice("ERROR in - " + gameObject.name);
                LogUtils.logError(e.Message + "\n" + e.StackTrace);
            }
        }
        /// <summary>
        /// 预加载
        /// </summary>
        protected virtual void PreLoad() { }
        /// <summary>
        /// 加载成员数据
        /// </summary>
        protected virtual void LoadMembers() { }
        /// <summary>
        /// 加载视图组件
        /// </summary>
        protected virtual void LoadViews() { }
        /// <summary>
        /// 设定事件监听
        /// </summary>
        protected virtual void SetupTriggers() { }
        /// <summary>
        /// 后处理
        /// </summary>
        protected virtual void PostLoad() { }
        /// <summary>
        /// 手动刷新
        /// </summary>
        public virtual void UpdateViews() { }
        /// <summary>
        /// 通过完整路径寻找子物体组件的快捷方法
        /// </summary>
        /// <typeparam name="T">目标组件类型</typeparam>
        /// <param name="path">完整路径</param>
        /// <returns>目标组件</returns>
        protected T FindComponentWithPath<T>(string path) where T : Component {
            return gameObject.FindComponent<T>(path);
        }
        /// <summary>
        /// 通过完整路径寻找子物体的快捷方法
        /// </summary>
        /// <param name="path">完整路径</param>
        /// <returns>目标物体对象</returns>
        protected GameObject FindGameObjectWithPath(string path) {
            return gameObject.FindObject(path);
        }
        /// <summary>
        /// 通过ObjectID寻找子物体组件的快捷方法
        /// </summary>
        /// <typeparam name="T">目标组件类型</typeparam>
        /// <param name="oid">ObjectID字符串</param>
        /// <returns>对应组件</returns>
        protected T FindComponentByID<T>(string oid) {
            return IdentifierUtils.FindComponentByID<T>(oid, ObjectDomain);
        }
        /// <summary>
        /// 通过ObjectID寻找子物体组件的快捷方法
        /// </summary>
        /// <typeparam name="T">目标组件类型</typeparam>
        /// <param name="oid">ObjectID字符串</param>
        /// <param name="domain">Object所在域</param>
        /// <returns>对应组件</returns>
        protected T FindComponentByID<T>(string oid, string domain) {
            return IdentifierUtils.FindComponentByID<T>(oid, domain);
        }
        /// <summary>
        /// 通过ObjectID寻找子物体的快捷方法
        /// </summary>
        /// <param name="oid">ObjectID字符串</param>
        /// <returns>游戏对象</returns>
        protected GameObject FindGameObjectByID(string oid) {
            return IdentifierUtils.FindGameObjectByID(oid, ObjectDomain);
        }
        /// <summary>
        /// 通过ObjectID寻找子物体的快捷方法
        /// </summary>
        /// <param name="oid">ObjectID字符串</param>
        /// <param name="domain">Object所在域</param>
        /// <returns>游戏对象</returns>
        protected GameObject FindGameObjectByID(string oid, string domain) {
            return IdentifierUtils.FindGameObjectByID(oid, domain);
        }
        /// <summary>
        /// 设置按钮的点击事件监听回调
        /// </summary>
        /// <param name="btn">按钮组件</param>
        /// <param name="listener">监听回调</param>
        /// <param name="isAppend">是否为附加监听（默认为替代监听）</param>
        protected void SetupClickListener(Button btn, UnityAction listener, bool isAppend = false) {
            if (btn != null) {
                if (!isAppend) {
                    btn.onClick.RemoveAllListeners();
                }
                btn.onClick.AddListener(listener);
            } else {
                LogUtils.logError("ERROR - Button listener cannot be set to null! - in " + gameObject.name);
            }
        }
        /// <summary>
        /// 设置关联的上级组件TagName
        /// </summary>
        /// <param name="objTag">关联上级物体的TagName</param>
        public void SetRelatedObject(string objTag) {
            RelatedObject = objTag;
        }
        /// <summary>
        /// 通知界面进行刷新
        /// </summary>
        public virtual void NotifyUpdate() {
            if (!UpdateNotifier) {
                UpdateNotifier = true;
            }
        }
    }
}
