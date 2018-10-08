// File create date:4/22/2018
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
// Created By Yu.Liu
namespace RoachGame.Widgets {
    /// <summary>
    /// 通用打字机模块控制器，用于播放文字打字机动画
    /// </summary>
    public class CommonTypingController : BaseView {
        // Code Here
        public Text textArea; // 文字显示区域
        public bool isAutoPlay = false; // 是否自动播放
        public float autoPlayDelay = 1f; // 自动播放切换延迟

        private NormalTextTyper textTyper; // 打字机动画工具
        private List<string> textList = new List<string>(); // 所有需要显示的字符串
        private int textIndex = 0;
        private bool moveNext = false;
        private float counter = 0f;

        protected override void LoadMembers() {
            base.LoadMembers();
            textTyper = new NormalTextTyper(textArea);
            textTyper.afterAnim = onAfterPlay;
            textTyper.durationTime = 0.1f;
        }

        protected override void Execute() {
            if (isAutoPlay) {
                if (moveNext) {
                    if (textTyper.IsPlaying) {
                        textTyper.stopPlay();
                        textArea.text = textList[textIndex - 1];
                    } else {
                        textArea.text = "";
                        textTyper.playText(textList[textIndex]);
                    }
                    counter = 0f;
                    moveNext = false;
                } else {
                    if (!textTyper.IsPlaying) {
                        if (textIndex == 0) {
                            textArea.text = "";
                            textTyper.playText(textList[textIndex]);
                        } else {
                            if (counter < autoPlayDelay) {
                                counter += Time.deltaTime;
                            } else {
                                textArea.text = "";
                                textTyper.playText(textList[textIndex]);
                                counter = 0f;
                            }
                        }
                    }
                }
            } else {
                if (moveNext) {
                    if (textTyper.IsPlaying) {
                        textTyper.stopPlay();
                        textArea.text = textList[textIndex - 1];
                    } else {
                        textArea.text = "";
                        textTyper.playText(textList[textIndex]);
                    }
                    moveNext = false;
                }
            }
        }

        private void onAfterPlay() {
            textIndex++;
        }

        public void AppendText(string str) {
            textList.Add(str);
        }

        public void SetupText(List<string> list) {
            textTyper.stopPlay();
            textArea.text = "";
            textList = list;
            textIndex = 0;
            moveNext = false;
        }

        public void SetupText(string str) {
            textTyper.stopPlay();
            textArea.text = "";
            textList.Clear();
            AppendText(str);
            textIndex = 0;
            moveNext = false;
        }

        public void onMoveNext() {
            if (!moveNext) {
                moveNext = true;
            }
        }
    }
}
