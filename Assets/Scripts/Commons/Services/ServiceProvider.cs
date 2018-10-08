/// File create date:8/3/2018
using RoachGame.Utils;
using System.Collections.Generic;
/// Created By Yu.Liu
namespace RoachGame.Services {
    /// <summary>
    /// 服务提供器
    /// </summary>
    public class ServiceProvider {
        private readonly static ServiceProvider instance = new ServiceProvider();
        /// <summary>
        /// 服务映射表
        /// </summary>
        private Dictionary<string, IGameService> services = new Dictionary<string, IGameService>();
        public static ServiceProvider Instance {
            get {
                return instance;
            }
        }
        /// <summary>
        /// 注册服务
        /// </summary>
        /// <param name="tag">服务标签</param>
        /// <param name="service">服务具体类</param>
        public void RegisterService(string tag, IGameService service) {
            if (services.ContainsKey(tag)) {
                LogUtils.logWarning("Duplicated Service Tag detected, please advice!");
            } else {
                services[tag] = service;
            }
        }
        /// <summary>
        /// 反注册服务
        /// </summary>
        /// <param name="tag">服务标签</param>
        public void UnregisterService(string tag) {
            if (services.ContainsKey(tag)) {
                services.Remove(tag);
            }
        }
        /// <summary>
        /// 提供服务
        /// </summary>
        /// <param name="tag">服务标签</param>
        /// <returns>服务接口</returns>
        public IGameService ProvideService(string tag) {
            IGameService result = null;
            if (services.ContainsKey(tag)) {
                result = services[tag];
            }
            return result;
        }
    }
}
