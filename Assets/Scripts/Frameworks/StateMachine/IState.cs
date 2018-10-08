// Created By Yu.Liu
namespace RoachGame.Framework.Automata {
    /// <summary>
    /// 自动机状态接口
    /// </summary>
    public interface IState {
        void EnterState();
        void UpdateState();
        void ExitState();
    }
}
