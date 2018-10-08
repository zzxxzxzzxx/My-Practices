/// File create date:8/29/2018
using System;
using System.Reflection;
/// Created By Yu.Liu
namespace RoachGame.Framework.Automata {
    /// <summary>
    /// 状态机的基本状态类，反射获取指定对象的方法
    /// </summary>
    public class MachineState<Machine> : IState {

        protected Machine stateMachineObj;

        protected string stateName;
        protected MethodInfo enterMethod;
        protected MethodInfo updateMethod;
        protected MethodInfo exitMethod;

        public MachineState(Machine machine, string state) {
            stateMachineObj = machine;
            stateName = state;
            ExtractMachineMethods();
        }

        private void ExtractMachineMethods() {
            Type t = stateMachineObj.GetType();
            enterMethod = t.GetMethod(stateName + "_Enter");
            updateMethod = t.GetMethod(stateName + "_Update");
            exitMethod = t.GetMethod(stateName + "_Exit");
        }

        public void EnterState() {
            if (enterMethod != null) {
                enterMethod.InvokeEmpty(stateMachineObj);
            }
        }

        public void ExitState() {
            if (exitMethod != null) {
                exitMethod.InvokeEmpty(stateMachineObj);
            }
        }

        public void UpdateState() {
            if (updateMethod != null) {
                updateMethod.InvokeEmpty(stateMachineObj);
            }
        }
    }
}
