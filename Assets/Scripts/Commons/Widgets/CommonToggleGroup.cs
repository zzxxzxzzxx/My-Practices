// File create date:3/27/2018
using RoachGame.Events;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
// Created By Yu.Liu
namespace RoachGame.Widgets {
    /// <summary>
    /// 通用选择组，支持单选和多选
    /// </summary>
    public class CommonToggleGroup : BaseView {
        // Code Here
        public const string TAG_COMMON_TOGGLE_GROUP = "CommonToggleGroup";

        private List<Toggle> toggleList; // 选项列表
        private int toggleCount; // 选项个数

        // 公共设置变量
        public bool singleSelect = true;

        protected override void LoadViews() {
            toggleList = new List<Toggle>();
            toggleCount = transform.childCount;
            for (int i = 0; i < transform.childCount; i++) {
                GameObject toggle = transform.GetChild(i).gameObject;
                toggleList.Add(toggle.GetComponent<Toggle>());
            }
            ToggleGroup grp = gameObject.GetComponent<ToggleGroup>();
            if (singleSelect && grp == null) {
                grp = gameObject.AddComponent<ToggleGroup>();
                for (int i = 0; i < toggleList.Count; i++) {
                    toggleList[i].group = grp;
                }
            }
        }

        protected override void SetupTriggers() {
            for (int i = 0; i < toggleList.Count; i++) {
                UnityAction<bool> action = onToggleChange;
                toggleList[i].onValueChanged.AddListener(action);
            }
        }

        protected override void Execute() {
            //
        }
        /// <summary>
        /// 设置一个选项被选中
        /// </summary>
        /// <param name="index">选项索引</param>
        public void SetSelected(int index) {
            if (index < toggleList.Count) {
                toggleList[index].isOn = true;
            }
        }
        /// <summary>
        /// 获取当前选中项（仅限单选）
        /// </summary>
        /// <returns>当前选中</returns>
        public int GetSelected() {
            if (singleSelect && toggleList != null) {
                for (int i = 0; i < toggleList.Count; i++) {
                    if (toggleList[i].isOn) {
                        return i;
                    }
                }
            }
            return -1;
        }
        /// <summary>
        /// 获取当前选中项（全部）
        /// </summary>
        /// <returns>当前选中</returns>
        public List<bool> GetToggleInfo() {
            List<bool> result = new List<bool>();
            if (toggleList != null) {
                foreach (Toggle t in toggleList) {
                    result.Add(t.isOn);
                }
            }
            return result;
        }
        /// <summary>
        /// 获取选项数量
        /// </summary>
        /// <returns></returns>
        public int GetToggleCount() {
            return toggleCount;
        }

        void onToggleChange(bool isOn) {
            EventUtils.EventBuilder builder = new EventUtils.EventBuilder(CommonConfigs.EVENT_COMMON_CALLBACK);
            builder.SetBasic(CommonConfigs.SUB_EVENT_CALLBACK_TOGGLE_CHANGE).SetTarget(RelatedObject, TagName);
            SendEvent(builder.BuildEvent());
        }
    }
}
