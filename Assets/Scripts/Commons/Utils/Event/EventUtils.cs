using System;

namespace RoachGame.Events {
    /// <summary>
    /// 事件相关工具集
    /// </summary>
    public class EventUtils {

        public static EventBuilder GetBuilder(int type) {
            return new EventBuilder(type);
        }
        /// <summary>
        /// 事件对象创建工具
        /// </summary>
        public class EventBuilder {

            private MBaseEvent baseEvent;

            public EventBuilder(int type) {
                baseEvent = MBaseEvent.ObtainEvent(type);
            }

            public EventBuilder SetTarget(string target, string src = CommonConfigs.TAG_SYSTEM_OBJ) {
                baseEvent.SetupTarget(src, target);
                return this;
            }

            public EventBuilder SetTarget(string targetID, string targetDomain, string src = CommonConfigs.TAG_SYSTEM_OBJ) {
                baseEvent.SetupTarget(src, IdentifierUtils.CompileTagNameByID(targetID, targetDomain));
                return this;
            }

            public EventBuilder SetBasic(int what) {
                baseEvent.what = what;
                return this;
            }

            public EventBuilder SetObject(object obj) {
                baseEvent.obj = obj;
                return this;
            }

            public EventBuilder SetCallback(Action<MBaseEvent> callback) {
                baseEvent.callback = callback;
                return this;
            }

            public EventBuilder PutExtra(string key, object value) {
                baseEvent.PutExtra(key, value);
                return this;
            }

            public EventBuilder PutCallback(string key, Action action) {
                baseEvent.PutCallback(key, action);
                return this;
            }

            public MBaseEvent BuildEvent() {
                return baseEvent;
            }
        }
    }
}
