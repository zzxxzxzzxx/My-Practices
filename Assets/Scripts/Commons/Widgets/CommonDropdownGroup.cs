// File create date:3/27/2018
using RoachGame.Events;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
// Created By Yu.Liu
namespace RoachGame.Widgets {
    /// <summary>
    /// 通用下拉组
    /// </summary>
    public class CommonDropdownGroup : BaseView {
        private List<Dropdown> dropList;
        private List<List<string>> dropOptions;
        private List<int> selectList;
        private bool initFlag;
        private bool isInited = false;
        private bool updateSelect;
        private int dropdownCount;

        protected override void LoadViews() {
            dropList = new List<Dropdown>();
            selectList = new List<int>();
            dropdownCount = transform.childCount;
            for (int i = 0; i < transform.childCount; i++) {
                GameObject drop = transform.GetChild(i).gameObject;
                dropList.Add(drop.GetComponent<Dropdown>());
                selectList.Add(0);
            }
        }

        protected override void LoadMembers() {
            initFlag = false;
            updateSelect = false;
        }

        protected override void SetupTriggers() {
            for (int i = 0; i < dropList.Count; i++) {
                UnityAction<int> action = onSelectChange;
                dropList[i].onValueChanged.AddListener(action);
            }
        }

        protected override void Execute() {
            if (initFlag) {
                UpdateDropdown();
            }
            if (updateSelect) {
                UpdateSelects();
            }
        }
        /// <summary>
        /// 刷新所有下拉
        /// </summary>
        private void UpdateDropdown() {
            for (int i = 0; i < dropList.Count; i++) {
                Dropdown drop = dropList[i];
                drop.options.Clear();
                if (i < dropOptions.Count) {
                    List<string> opts = dropOptions[i];
                    foreach (string str in opts) {
                        Dropdown.OptionData op = new Dropdown.OptionData();
                        op.text = str;
                        drop.options.Add(op);
                    }
                }
                if (drop.options.Count > 1) {
                    drop.value = drop.options.Count - 1;
                    drop.value = 0;
                } else if (drop.options.Count > 0) {
                    drop.gameObject.FindComponent<Text>("Label").text = drop.options[0].text;
                } else {
                    drop.gameObject.FindComponent<Text>("Label").text = "--";
                }
            }
            initFlag = false;
            isInited = true;
        }
        /// <summary>
        /// 收集下拉选项信息
        /// </summary>
        private void GatherSelectInfo() {
            for (int i = 0; i < dropList.Count; i++) {
                Dropdown d = dropList[i];
                selectList[i] = d.value;
            }
        }
        /// <summary>
        /// 更新选中选项
        /// </summary>
        private void UpdateSelects() {
            for (int i = 0; i < dropList.Count; i++) {
                dropList[i].value = selectList[i];
            }
            updateSelect = false;
        }
        /// <summary>
        /// 设置每个下拉列表的选项
        /// </summary>
        /// <param name="lists">所有选项</param>
        public void SetDropdownLists(List<List<string>> lists) {
            dropOptions = lists;
            initFlag = true;
        }
        /// <summary>
        /// 根据索引设置下拉列表选项
        /// </summary>
        /// <param name="list">选项表</param>
        /// <param name="index">索引</param>
        public void SetDropdownListByIndex(List<string> list, int index) {
            dropOptions[index] = list;
            Dropdown drop = dropList[index];
            drop.options.Clear();
            for (int i = 0; i < list.Count; i++) {
                Dropdown.OptionData opt = new Dropdown.OptionData();
                opt.text = list[i];
                drop.options.Add(opt);
            }
            if (drop.options.Count > 1) {
                drop.value = drop.options.Count - 1;
                drop.value = 0;
            } else if (drop.options.Count > 0) {
                drop.gameObject.FindComponent<Text>("Label").text = drop.options[0].text;
            } else {
                drop.gameObject.FindComponent<Text>("Label").text = "--";
            }
        }
        /// <summary>
        /// 是否已经初始化完成
        /// </summary>
        /// <returns></returns>
        public bool isInitiated() {
            return isInited;
        }
        /// <summary>
        /// 根据索引设置指定下拉的选项
        /// </summary>
        /// <param name="index">索引</param>
        /// <param name="sub">选项</param>
        public void SetSelected(int index, int sub) {
            if (index < dropList.Count) {
                selectList[index] = sub;
                updateSelect = true;
            }
        }
        /// <summary>
        /// 重置所有选项到初始值
        /// </summary>
        public void ResetSelected() {
            for (int i = 0; i < selectList.Count; i++) {
                selectList[i] = 0;
            }
            updateSelect = true;
        }
        /// <summary>
        /// 获取所有下拉选择数据
        /// </summary>
        /// <returns>选择数据</returns>
        public List<int> GetDropdownInfo() {
            List<int> result = new List<int>();
            GatherSelectInfo();
            result.AddRange(selectList);
            return result;
        }
        /// <summary>
        /// 获取下拉列表数量
        /// </summary>
        /// <returns>列表数量</returns>
        public int GetDropdownCount() {
            return dropdownCount;
        }

        void onSelectChange(int select) {
            EventUtils.EventBuilder builder = new EventUtils.EventBuilder(CommonConfigs.EVENT_COMMON_CALLBACK);
            builder.SetBasic(CommonConfigs.SUB_EVENT_CALLBACK_DROPDOWN_CHANGE).SetTarget(RelatedObject, TagName);
            SendEvent(builder.BuildEvent());
            GatherSelectInfo();
        }
    }
}
