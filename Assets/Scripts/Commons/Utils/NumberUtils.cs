// File create date:1/6/2018
using UnityEngine;

// Created By Yu.Liu
namespace RoachGame.Utils {
    /// <summary>
    /// 数字相关工具类
    /// </summary>
    public class NumberUtils {
        /// <summary>
        /// 通用随机整数获取工具
        /// </summary>
        /// <param name="max">随机最大值</param>
        /// <param name="min">随机最小值</param>
        /// <returns>整数值</returns>
        public static int RandomInteger(int max, int min = 0) {
            return Random.Range(min, max + 1);
        }
        /// <summary>
        /// 通用随机浮点数获取工具
        /// </summary>
        /// <param name="max">随机最大值</param>
        /// <param name="min">随机最小值</param>
        /// <returns>浮点数值</returns>
        public static float RandomFloat(float max, float min = 0f) {
            return Random.Range(min, max);
        }
    }
}