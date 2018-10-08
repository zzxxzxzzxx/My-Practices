using System;
using UnityEngine;

namespace RoachGame.Utils {
    // ------ 通用日志工具
    public class LogUtils {
        public const string LOG_PREFIX_NOTICE = "NOTICE:";
        public const string LOG_PREFIX_WARNING = "WARNING:";
        public const string LOG_PREFIX_ERROR = "ERROR:";
        public static bool isNoticeLogEnable = true;
        public static bool isWarningLogEnable = true;
        public static bool isErrorLogEnable = true;
        // 打印通知日志
        public static void logNotice(string msg) {
            if (isNoticeLogEnable) {
                if (TextUtils.hasData(msg)) {
                    Debug.Log(LOG_PREFIX_NOTICE + msg);
                } else {
                    Debug.Log(LOG_PREFIX_NOTICE + "No Message!");
                }
            }
        }
        // 打印警告日志
        public static void logWarning(string msg) {
            if (isWarningLogEnable) {
                if (TextUtils.hasData(msg)) {
                    Debug.LogWarning(LOG_PREFIX_WARNING + msg);
                } else {
                    Debug.LogWarning(LOG_PREFIX_WARNING + "No Message!");
                }
            }
        }
        // 打印错误日志
        public static void logError(string msg) {
            if (isErrorLogEnable) {
                if (TextUtils.hasData(msg)) {
                    Debug.LogError(LOG_PREFIX_ERROR + msg);
                } else {
                    Debug.LogError(LOG_PREFIX_ERROR + "No Message!");
                }
            }
        }
    }
}