/// File create date:8/3/2018
using RoachGame.Events;
using RoachGame.Utils;
using System;
using System.Collections.Generic;
/// Created By Yu.Liu
namespace RoachGame.Services {
    /// <summary>
    /// 基础事件服务，可继承拓展
    /// </summary>
    public class BaseEventService : EmptyService {
        private EventCenter _center;
        public BaseEventService(EventCenter center) {
            _center = center;
        }
        /// <summary>
        /// 发送事件
        /// </summary>
        /// <param name="e">事件对象</param>
        public void SendEvent(MBaseEvent e) {
            if (_center != null) {
                _center.dispatchEnqueue(e);
            }
        }
        /// <summary>
        /// 获取一个通用事件构造器
        /// </summary>
        /// <param name="type">事件类型</param>
        /// <param name="what">事件标签</param>
        /// <param name="target">事件目标</param>
        /// <param name="src">事件源</param>
        /// <returns>构造器</returns>
        protected EventUtils.EventBuilder GenerateCommonBuilder(int type, int what, string target, string src) {
            EventUtils.EventBuilder builder = EventUtils.GetBuilder(type);
            builder.SetBasic(what);
            if (TextUtils.hasData(src)) {
                builder.SetTarget(target, src);
            } else {
                builder.SetTarget(target);
            }
            return builder;
        }
        /// <summary>
        /// 发送一个通用事件
        /// </summary>
        /// <param name="type">事件类型</param>
        /// <param name="what">事件标签</param>
        /// <param name="target">事件目标</param>
        /// <param name="src">事件源</param>
        /// <param name="args">事件参数</param>
        public void SubmitCommonEvent(int type, int what, string target, string src = null, Dictionary<string, object> args = null) {
            EventUtils.EventBuilder builder = GenerateCommonBuilder(type, what, target, src);
            if (args != null) {
                foreach (string key in args.Keys) {
                    builder.PutExtra(key, args[key]);
                }
            }
            SendEvent(builder.BuildEvent());
        }
        /// <summary>
        /// 发送一个回调事件
        /// </summary>
        /// <param name="type">事件类型</param>
        /// <param name="what">事件标签</param>
        /// <param name="target">事件目标</param>
        /// <param name="callback">事件回调</param>
        /// <param name="src">事件源</param>
        /// <param name="args">事件参数</param>
        public void SubmitCommonCallbackEvent(int type, int what, string target, Action<MBaseEvent> callback, string src = null, Dictionary<string, object> args = null) {
            EventUtils.EventBuilder builder = GenerateCommonBuilder(type, what, target, src);
            builder.SetCallback(callback);
            if (args != null) {
                foreach (string key in args.Keys) {
                    builder.PutExtra(key, args[key]);
                }
            }
            SendEvent(builder.BuildEvent());
        }
    }
}
