// File create date:3/29/2018
namespace RoachGame.Framework.VirtualMachine {
    /// <summary>
    /// 字节码虚拟机所用的栈值结构
    /// </summary>
    public class StackValue {
        public bool isNumber = true; // 标明栈值是否为数字
        public int code; // 字面值，如果栈值为数字则本身即为数据，如果不是数字则表示虚拟内存索引
    }
}
