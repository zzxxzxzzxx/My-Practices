// File create date:3/29/2018
namespace RoachGame.Framework.VirtualMachine {
    /// <summary>
    /// 基础字节码枚举类
    /// </summary>
    public static class BasicByteCodes {
        public const int BC_BASIC_PUSH = 0x0000;
        public const int BC_BASIC_POP = 0x0001;
        public const int BC_BASIC_ADD = 0x0002;
        public const int BC_BASIC_MINUS = 0x0003;
        public const int BC_BASIC_MULTIPLY = 0x0004;
        public const int BC_BASIC_DIVIDE = 0x0005;
    }
}
