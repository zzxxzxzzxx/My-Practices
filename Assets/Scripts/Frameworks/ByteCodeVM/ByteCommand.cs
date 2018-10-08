// File create date:3/29/2018
namespace RoachGame.Framework.VirtualMachine {
    /// <summary>
    /// 字节码指令结构
    /// </summary>
    public class ByteCommand {
        public bool withReturn = false; // 是否有返回值需要压栈
        public int commandCode; // 指令字节码
        public bool isNumberArg = true; // 指示需要压栈的字面值是数字还是字符串引用
        public int argument; // 或者是单个的字节码字面值，或者是从栈中取的数据个数
        public string strArg; // 字符串字面值
    }
}
