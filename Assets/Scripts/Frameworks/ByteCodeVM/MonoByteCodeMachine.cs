using System;
using System.Collections.Generic;
using UnityEngine;

namespace RoachGame.Framework.VirtualMachine {
    /// <summary>
    /// 字节码虚拟机抽象基类，与MonoBehaviour关联，执行效率为每帧固定行数，可以自行调节
    /// 基于栈的执行方式，所有栈值一律采用字节码，仅支持两种栈值，字符串与数字，结构为{标识符，字节码}
    /// 指令结构为{是否有返回值，指令字节码，参数个数}
    /// 数字直接使用字节码转换回整型得到，字符串需要通过虚拟内存池获取
    /// </summary>
    public abstract class MonoByteCodeMachine : MonoBehaviour {

        public int BCM_CODE_EXE_PER_UPDATE = 2; // 字节码虚拟机执行效率，单帧执行的指令数
        public int BCM_MEM_POOL_LIMIT = 65535; // 字节码虚拟机的内存池大小，默认最多储存65535个字符串字面值

        protected bool isMachineActive = false;

        protected Stack<StackValue> valueStack;
        protected Queue<ByteCommand> commandQueue;
        protected Dictionary<int, string> memoryPool;
        protected Queue<int> memorySpace;

        private void Awake() {
            valueStack = new Stack<StackValue>();
            commandQueue = new Queue<ByteCommand>();
            memoryPool = new Dictionary<int, string>();
            memorySpace = new Queue<int>();
        }

        // Use this for initialization
        private void Start() {
            try {
                InitMachine();
            } catch (Exception e) {
                Debug.LogError(e.Message + "\n" + e.StackTrace);
                isMachineActive = false;
            }
        }

        // Update is called once per frame
        private void Update() {
            if (isMachineActive) {
                try {
                    if (commandQueue.Count > 0) {
                        for (int i = 0; i < BCM_CODE_EXE_PER_UPDATE; i++) {
                            if (commandQueue.Count > 0) {
                                ByteCommand cmd = commandQueue.Dequeue();
                                ExecuteCommand(cmd);
                            }
                        }
                    }
                } catch (Exception e) {
                    Debug.LogError(e.Message + "\n" + e.StackTrace);
                    isMachineActive = false;
                }
            }
        }
        /// <summary>
        /// 数据成员初始化方法，子类可覆盖
        /// </summary>
        protected virtual void InitMembers() { }
        /// <summary>
        /// 虚拟机初始化方法，子类重写
        /// </summary>
        protected abstract void InitMachine();

        /// <summary>
        /// 指令执行
        /// </summary>
        /// <param name="cmd">待执行指令</param>
        protected virtual void ExecuteCommand(ByteCommand cmd) {
            if (cmd.commandCode == BasicByteCodes.BC_BASIC_PUSH) { // 基本操作：数据压栈
                StackValue val = new StackValue();
                if (cmd.isNumberArg) {
                    val.isNumber = true;
                    val.code = cmd.argument;
                } else {
                    val.isNumber = false;
                    val.code = AllocateMemory(cmd.strArg);
                }
                valueStack.Push(val);
            } else if (cmd.commandCode == BasicByteCodes.BC_BASIC_POP) { // 基本操作：数据出栈
                StackValue val = valueStack.Pop();
                if (!val.isNumber) {
                    ReleaseMemory(val.code);
                }
            } else if (cmd.commandCode == BasicByteCodes.BC_BASIC_ADD) { // 基本操作：数值相加
                StackValue aVal = valueStack.Pop();
                StackValue bVal = valueStack.Pop();
                if (aVal.isNumber && bVal.isNumber) {
                    StackValue val = new StackValue();
                    val.isNumber = true;
                    val.code = aVal.code + bVal.code;
                    valueStack.Push(val);
                } else {
                    throw new ArgumentException("Can only add two number values!");
                }
            } else if (cmd.commandCode == BasicByteCodes.BC_BASIC_MINUS) { // 基本操作：数值相减
                StackValue aVal = valueStack.Pop();
                StackValue bVal = valueStack.Pop();
                if (aVal.isNumber && bVal.isNumber) {
                    StackValue val = new StackValue();
                    val.isNumber = true;
                    val.code = aVal.code - bVal.code;
                    valueStack.Push(val);
                } else {
                    throw new ArgumentException("Can only minus two number values!");
                }
            } else if (cmd.commandCode == BasicByteCodes.BC_BASIC_MINUS) { // 基本操作：数值相乘
                StackValue aVal = valueStack.Pop();
                StackValue bVal = valueStack.Pop();
                if (aVal.isNumber && bVal.isNumber) {
                    StackValue val = new StackValue();
                    val.isNumber = true;
                    val.code = aVal.code * bVal.code;
                    valueStack.Push(val);
                } else {
                    throw new ArgumentException("Can only multiply two number values!");
                }
            } else if (cmd.commandCode == BasicByteCodes.BC_BASIC_MINUS) { // 基本操作：数值相除
                StackValue aVal = valueStack.Pop();
                StackValue bVal = valueStack.Pop();
                if (aVal.isNumber && bVal.isNumber) {
                    StackValue val = new StackValue();
                    val.isNumber = true;
                    val.code = aVal.code / bVal.code;
                    valueStack.Push(val);
                } else {
                    throw new ArgumentException("Can only divide two number values!");
                }
            } else {
                ExtendedCommand(cmd);
            }
        }
        /// <summary>
        /// 拓展指令集处理方法，子类重写
        /// </summary>
        /// <param name="cmd"></param>
        protected abstract void ExtendedCommand(ByteCommand cmd);

        /// <summary>
        /// 分配内存空间给字符串字面值
        /// </summary>
        /// <param name="value">字符串字面值</param>
        /// <returns>分配的字节码</returns>
        private int AllocateMemory(string value) {
            if (memoryPool.Count < BCM_MEM_POOL_LIMIT) {
                int valueCode = 0;
                if (memorySpace.Count > 0) {
                    valueCode = memorySpace.Dequeue();
                } else {
                    System.Random rand = new System.Random(DateTime.Now.Millisecond);
                    while (memoryPool.ContainsKey(valueCode)) {
                        valueCode = rand.Next(int.MaxValue);
                    }
                }
                memoryPool[valueCode] = value;
                return valueCode;
            }
            throw new OutOfMemoryException("Byte Code VM run out of memory!");
        }
        /// <summary>
        /// 释放内存空间
        /// </summary>
        /// <param name="code">分配的字节码</param>
        private void ReleaseMemory(int code) {
            if (memoryPool.ContainsKey(code)) {
                memoryPool.Remove(code);
                memorySpace.Enqueue(code);
            }
        }

        private void OnDestroy() {
            isMachineActive = false;
            valueStack.Clear();
            commandQueue.Clear();
            memoryPool.Clear();
            memorySpace.Clear();
        }
        /// <summary>
        /// 暂停虚拟机运行
        /// </summary>
        public void PauseMachine() {
            isMachineActive = false;
        }
    }
}
