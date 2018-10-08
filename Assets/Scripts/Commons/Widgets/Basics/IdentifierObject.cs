using RoachGame.Utils;
using System;
using UnityEngine;
namespace RoachGame {
    /// <summary>
    /// 带全局标识的游戏对象，将对象加入全局标识中，作为整体框架的基础
    /// </summary>
    public class IdentifierObject : MonoBehaviour {
        [Tooltip("组件唯一标识，如果置空则与对象名称相同")]
        public string ObjectID = "";
        [Tooltip("组件作用域，可区分不同域下的同一ID")]
        public string ObjectDomain = "Global";
        [Tooltip("是否将该对象注册到全局标识")]
        public bool UseGlobalIdentity = true;
        protected bool exceptionFlag = false; // 异常标识，用于避免异常情况下依然进行错误更新
        private string _tagName = ""; // 对象标签，区别于GameObject的tag属性
        /// <summary>
        /// 对象标签，用于识别该对象
        /// </summary>
        public string TagName {
            get {
                if (!TextUtils.hasData(_tagName)) {

                    _tagName = String.Format("({0}@{1})&[0x{2:X8}]", ObjectID, ObjectDomain, GetInstanceID());
                }
                return _tagName;
            }
        }

        protected virtual void PreInit() { }

        private void Awake() {
            UniverseController.initialize();
            try {
                PreInit();
                InitIdentity();
            } catch (Exception e) {
                LogUtils.logNotice("ERROR in - " + gameObject.name + " Awake");
                LogUtils.logError(e.Message + "\n" + e.StackTrace);
                exceptionFlag = true;
            }
        }

        /// <summary>
        /// 全局标识符初始化
        /// </summary>
        private void InitIdentity() {
            ObjectID = TextUtils.hasData(ObjectID) ? ObjectID : name;
            if (UseGlobalIdentity) {
                string ID_Domain = ObjectID + "@" + ObjectDomain;
                if (IdentifierUtils.IDMap.ContainsKey(ID_Domain)) {
                    LogUtils.logWarning("Duplicated ObjectID:" + ID_Domain + " detected, please advice!");
                } else {
                    IdentifierUtils.IDMap[ID_Domain] = GetInstanceID();
                    IdentifierUtils.ObjectMap[GetInstanceID()] = gameObject;
                }
            }
        }

        protected virtual void Release() { }

        private void OnDestroy() {
            Release();
            if (UseGlobalIdentity) { // 反注册全局标识
                string ID_Domain = ObjectID + "@" + ObjectDomain;
                int id = IdentifierUtils.IDMap.TryGetElement(ID_Domain);
                if (id == GetInstanceID()) {
                    IdentifierUtils.IDMap.Remove(ID_Domain);
                    IdentifierUtils.ObjectMap.Remove(id);
                }
            }
        }
    }
}
