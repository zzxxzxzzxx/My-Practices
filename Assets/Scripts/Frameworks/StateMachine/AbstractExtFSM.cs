// File create date:5/22/2018
using System;
using UnityEngine;
// Created By Yu.Liu
namespace RoachGame.Framework.Automata {
    /// <summary>
    /// 独立生命周期的抽象状态机基类
    /// </summary>
    public abstract class AbstractExtFSM : ExtraBehaviour {
        // Code Here
        private IState _current;
        public IState CurrentState {
            set {
                if (_current != null) {
                    _current.ExitState();
                }
                _current = value;
                if (_current != null) {
                    _current.EnterState();
                }
            }
            get {
                return _current;
            }
        }

        public override void ExtraStart() {
            base.ExtraStart();
            try {
                GlobalMachineStart();
            } catch (Exception e) {
                Debug.Log(e);
            }
        }

        public override void ExtraUpdate() {
            base.ExtraUpdate();
            try {
                GlobalMachineUpdate();
                if (CurrentState != null) {
                    CurrentState.UpdateState();
                }
            } catch (Exception e) {
                Debug.Log(e);
            }
        }

        public override void ExtraDestroy() {
            base.ExtraDestroy();
            GlobalMachineTerminate();
        }

        protected abstract void GlobalMachineStart();
        protected abstract void GlobalMachineUpdate();
        protected abstract void GlobalMachineTerminate();
    }
}
