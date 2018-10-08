// File create date:3/29/2018
namespace RoachGame.Framework {
    /// <summary>
    /// 独立生命周期的运行基类
    /// </summary>
    public abstract class ExtraBehaviour {

        public abstract string Tag { get; }
        public string Name { set; get; }

        public virtual void ExtraAwake() { }
        public virtual void ExtraStart() { }
        public virtual void ExtraUpdate() { }
        public virtual void ExtraPause() { }
        public virtual void ExtraDestroy() { }
    }
}
