namespace RoachGame.Utils {
    /// <summary>
    /// 字符串工具
    /// </summary>
    public class TextUtils {
        // 封装string对象数据判定
        public static bool hasData(string str) {
            return str != null && str.Length > 0;
        }

        // 封装数据交换操作，使用引用类型保证交换成功，支持泛型
        public static void swapData<T>(ref T left, ref T right) {
            T temp = left;
            left = right;
            right = temp;
        }
    }
}
