// File create date:5/22/2018
using System.Collections.Generic;
// Created By Yu.Liu
namespace RoachGame.Framework.Automata {
    /// <summary>
    /// 独立生命周期的下推自动机基类
    /// </summary>
    public abstract class AbstractExtPDA : AbstractExtFSM {
        // Code Here
        protected Stack<IState> stateStack = new Stack<IState>();

        public virtual void PushState(IState state) {
            stateStack.Push(state);
            CurrentState = state;
        }

        public virtual void PopState() {
            if (stateStack.Count > 0) {
                stateStack.Pop();
                if (stateStack.Count > 0) {
                    CurrentState = stateStack.Peek();
                } else {
                    CurrentState = null;
                }
            }
        }
    }
}
