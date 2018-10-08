using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RoachGame {
    /// <summary>
    /// 拓展方法类
    /// </summary>
    public static class ExtFunctions {
        public const float COLOR_NORMALIZER = 255f; // 颜色通道归一化常量
        /// <summary>
        /// 安全获取Dictionary元素的拓展方法，在不存在对应Key的时候会返回默认值
        /// </summary>
        /// <typeparam name="K">Key的类型</typeparam>
        /// <typeparam name="V">Value的类型</typeparam>
        /// <param name="dict">拓展方法对应的Dictionary对象</param>
        /// <param name="key">键</param>
        /// <returns>值</returns>
        public static V TryGetElement<K, V>(this Dictionary<K, V> dict, K key) {
            V result = default(V);
            dict.TryGetValue(key, out result);
            return result;
        }

        /// <summary>
        /// 直接在GameObject之下按路径寻找子物体组件的拓展方法
        /// </summary>
        /// <typeparam name="T">所寻找组件类型</typeparam>
        /// <param name="obj">拓展方法对应的GameObject</param>
        /// <param name="path">路径</param>
        /// <returns>组件</returns>
        public static T FindComponent<T>(this GameObject obj, string path) where T : Component {
            T result = null;
            if (obj != null) {
                Transform target = obj.transform.Find(path);
                if (target != null) {
                    result = target.gameObject.GetComponent<T>();
                }
            }
            return result;
        }

        /// <summary>
        /// 直接在GameObject之下按路径寻找子物体的拓展方法
        /// </summary>
        /// <param name="obj">拓展方法对应的GameObject</param>
        /// <param name="path">路径</param>
        /// <returns>子物体</returns>
        public static GameObject FindObject(this GameObject obj, string path) {
            GameObject result = null;
            if (obj != null) {
                Transform target = obj.transform.Find(path);
                if (target != null) {
                    result = target.gameObject;
                }
            }
            return result;
        }

        /// <summary>
        /// 组件获取的合并方法，第一步检查传入组件是否为空（默认为空）
        /// 如果第一步得到为空则从GameObject获取组件，否则返回传入组件
        /// 如果获取结果依然为空则添加组件
        /// </summary>
        /// <typeparam name="T">组件类型</typeparam>
        /// <param name="obj">拓展方法对应的GameObject</param>
        /// <param name="origin">传入组件</param>
        /// <returns>组件</returns>
        public static T CheckAndGetComponent<T>(this GameObject obj, T origin = null) where T : Component {
            T component = null;
            if (origin == null) {
                component = obj.GetComponent<T>();
                if (component == null) {
                    component = obj.AddComponent<T>();
                }
            } else {
                component = origin;
            }
            return component;
        }

        /// <summary>
        /// 为GameObject设置触发事件回调
        /// </summary>
        /// <param name="target">触发器所在对象</param>
        /// <param name="listener">回调方法</param>
        /// <param name="type">事件类型</param>
        public static void SetupEventTrigger(this GameObject target, UnityAction<BaseEventData> listener, EventTriggerType type) {
            EventTrigger trigger = target.GetComponent<EventTrigger>() as EventTrigger;
            if (trigger == null) {
                trigger = target.AddComponent<EventTrigger>();
            }
            if (trigger.triggers == null) {
                trigger.triggers = new List<EventTrigger.Entry>();
            }
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = type;
            entry.callback = new EventTrigger.TriggerEvent();
            entry.callback.AddListener(listener);
            trigger.triggers.Add(entry);
        }

        /// <summary>
        /// 对Image的Color属性进行赋值
        /// </summary>
        /// <param name="img">拓展方法对应的Image</param>
        /// <param name="colorStr">颜色字符串，要求符合"#FFFFFF"或者"#FFFFFFFF"格式</param>
        public static void SetColor(this Image img, string colorStr) {
            if (colorStr.Length == 7 || colorStr.Length == 9) {
                if (colorStr[0] == '#') {
                    if (colorStr.Length == 7) {
                        // RGB
                        string rStr = colorStr.Substring(1, 2);
                        string gStr = colorStr.Substring(3, 2);
                        string bStr = colorStr.Substring(5, 2);
                        int r = Int32.Parse(rStr, System.Globalization.NumberStyles.HexNumber);
                        int g = Int32.Parse(gStr, System.Globalization.NumberStyles.HexNumber);
                        int b = Int32.Parse(bStr, System.Globalization.NumberStyles.HexNumber);
                        img.color = new Color(r / COLOR_NORMALIZER, g / COLOR_NORMALIZER, b / COLOR_NORMALIZER);
                    }
                    if (colorStr.Length == 9) {
                        // ARGB
                        string aStr = colorStr.Substring(1, 2);
                        string rStr = colorStr.Substring(3, 2);
                        string gStr = colorStr.Substring(5, 2);
                        string bStr = colorStr.Substring(7, 2);
                        int a = Int32.Parse(aStr, System.Globalization.NumberStyles.HexNumber);
                        int r = Int32.Parse(rStr, System.Globalization.NumberStyles.HexNumber);
                        int g = Int32.Parse(gStr, System.Globalization.NumberStyles.HexNumber);
                        int b = Int32.Parse(bStr, System.Globalization.NumberStyles.HexNumber);
                        img.color = new Color(r / COLOR_NORMALIZER, g / COLOR_NORMALIZER, b / COLOR_NORMALIZER, a / COLOR_NORMALIZER);
                    }
                }
            }
        }

        /// <summary>
        /// 对Text的Color属性进行赋值
        /// </summary>
        /// <param name="txt">拓展方法对应的Text</param>
        /// <param name="colorStr">颜色字符串，要求符合"#FFFFFF"或者"#FFFFFFFF"格式</param>
        public static void SetColor(this Text txt, string colorStr) {
            if (colorStr.Length == 7 || colorStr.Length == 9) {
                if (colorStr[0] == '#') {
                    if (colorStr.Length == 7) {
                        // RGB
                        string rStr = colorStr.Substring(1, 2);
                        string gStr = colorStr.Substring(3, 2);
                        string bStr = colorStr.Substring(5, 2);
                        int r = Int32.Parse(rStr, System.Globalization.NumberStyles.HexNumber);
                        int g = Int32.Parse(gStr, System.Globalization.NumberStyles.HexNumber);
                        int b = Int32.Parse(bStr, System.Globalization.NumberStyles.HexNumber);
                        txt.color = new Color(r / COLOR_NORMALIZER, g / COLOR_NORMALIZER, b / COLOR_NORMALIZER);
                    }
                    if (colorStr.Length == 9) {
                        // ARGB
                        string aStr = colorStr.Substring(1, 2);
                        string rStr = colorStr.Substring(3, 2);
                        string gStr = colorStr.Substring(5, 2);
                        string bStr = colorStr.Substring(7, 2);
                        int a = Int32.Parse(aStr, System.Globalization.NumberStyles.HexNumber);
                        int r = Int32.Parse(rStr, System.Globalization.NumberStyles.HexNumber);
                        int g = Int32.Parse(gStr, System.Globalization.NumberStyles.HexNumber);
                        int b = Int32.Parse(bStr, System.Globalization.NumberStyles.HexNumber);
                        txt.color = new Color(r / COLOR_NORMALIZER, g / COLOR_NORMALIZER, b / COLOR_NORMALIZER, a / COLOR_NORMALIZER);
                    }
                }
            }
        }

        /// <summary>
        /// 对Image的Color属性进行赋值
        /// </summary>
        /// <param name="img">拓展方法对应的Image</param>
        /// <param name="color">颜色值，以无符号整数类型给出，建议使用0xFFFFFFFF格式</param>
        public static void SetColor(this Image img, uint color) {
            uint tmpColor = color;
            uint b = tmpColor & 0xFF;
            tmpColor = tmpColor >> 8;
            uint g = tmpColor & 0xFF;
            tmpColor = tmpColor >> 8;
            uint r = tmpColor & 0xFF;
            tmpColor = tmpColor >> 8;
            uint a = tmpColor & 0xFF;
            img.color = new Color(r / COLOR_NORMALIZER, g / COLOR_NORMALIZER, b / COLOR_NORMALIZER, a / COLOR_NORMALIZER);
        }

        /// <summary>
        /// 对Text的Color属性进行赋值
        /// </summary>
        /// <param name="txt">拓展方法对应的Text</param>
        /// <param name="color">颜色值，以无符号整数类型给出，建议使用0xFFFFFFFF格式</param>
        public static void SetColor(this Text txt, uint color) {
            uint tmpColor = color;
            uint b = tmpColor & 0xFF;
            tmpColor = tmpColor >> 8;
            uint g = tmpColor & 0xFF;
            tmpColor = tmpColor >> 8;
            uint r = tmpColor & 0xFF;
            tmpColor = tmpColor >> 8;
            uint a = tmpColor & 0xFF;
            txt.color = new Color(r / COLOR_NORMALIZER, g / COLOR_NORMALIZER, b / COLOR_NORMALIZER, a / COLOR_NORMALIZER);
        }
        /// <summary>
        /// 按指定长度裁剪字符串
        /// </summary>
        /// <param name="str">拓展方法对应的字符串</param>
        /// <param name="length">限定长度</param>
        /// <returns>裁剪后的字符串</returns>
        public static string ShrinkString(this string str, int length, string suffix) {
            string result = str;
            if (str.Length >= length) {
                result = str.Substring(0, length - 1);
                result += suffix;
            }
            return result;
        }
        /// <summary>
        /// 运行指定方法，空参数表
        /// </summary>
        /// <param name="method">对应的方法信息</param>
        /// <param name="source">方法源</param>
        public static void InvokeEmpty(this MethodInfo method, object source) {
            method.Invoke(source, new object[0]);
        }
    }
}
