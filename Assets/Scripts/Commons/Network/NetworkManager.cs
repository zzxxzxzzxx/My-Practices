// File create date:6/6/2018
using System.Collections.Generic;
using System.Text;
// Created By Yu.Liu
namespace RoachGame.Network {
    /// <summary>
    /// 网络连接管理器
    /// </summary>
    public class NetworkManager {

        private static volatile NetworkManager instance; // 线程同步的单例模式
        private static readonly object locker = new object(); //用于确保线程同步的标志变量
        private NetworkManager() {
            sockThreadPool = new Dictionary<string, NetworkThread>();
        }
        public static NetworkManager Instance {
            get {
                if (instance == null) { //此处为双重锁定实现，既可以做到线程同步又不会因为每次访问都要请求加锁而造成性能损失
                    lock (locker) {
                        if (instance == null) {
                            instance = new NetworkManager();
                        }
                    }
                }
                return instance;
            }
        }
        /// <summary>
        /// Socket连接线程池
        /// </summary>
        private Dictionary<string, NetworkThread> sockThreadPool;
        /// <summary>
        /// 设置服务器地址和端口，此处允许使用多个线程连接不同的服务器和端口
        /// </summary>
        /// <param name="serverIP">服务器IP</param>
        /// <param name="port">服务器端口号</param>
        public void SetupServer(string serverIP, int port) {
            string host = serverIP + " - " + port;
            if (sockThreadPool.ContainsKey(host)) {
                if (!sockThreadPool[host].IsAlive) {
                    sockThreadPool.Remove(host);
                } else {
                    // 已经有连接了，直接返回
                    return;
                }
            }
            NetworkThread sth = NetworkThread.GetThread();
            sth.SetupAddressAndPort(serverIP, port);
            sockThreadPool[host] = sth;
        }
        /// <summary>
        /// 设置数据协议，从远端接收消息后需要按照协议解析为合适的对象，如果客户端要接收服务端的消息，则数据协议是必须的
        /// </summary>
        /// <param name="serverIP">服务器IP</param>
        /// <param name="port">服务器端口号</param>
        /// <param name="dataProtocol">数据协议接口</param>
        public void SetupProtocol(string serverIP, int port, IDataProtocol dataProtocol) {
            string host = serverIP + " - " + port;
            if (sockThreadPool.ContainsKey(host)) {
                sockThreadPool[host].SetupDataProtocol(dataProtocol);
            }
        }
        /// <summary>
        /// 设置数据分发器，这是接收远端消息后进行消息分发的主要部件，如果要将网络消息传输到游戏系统中，则分发器是必须的
        /// </summary>
        /// <param name="serverIP">服务器IP</param>
        /// <param name="port">服务器端口号</param>
        /// <param name="dispatcher">数据分发器</param>
        public void SetupDispatcher(string serverIP, int port, IDispatcher dispatcher) {
            string host = serverIP + " - " + port;
            if (sockThreadPool.ContainsKey(host)) {
                sockThreadPool[host].SetupDispatcher(dispatcher);
            }
        }
        /// <summary>
        /// 注册数据拦截器，这是拦截接收到的远端原始数据并进行额外处理的部件，其运作不影响主要接收循环
        /// </summary>
        /// <param name="serverIP">服务器IP</param>
        /// <param name="port">服务器端口号</param>
        /// <param name="interceptor">拦截器接口</param>
        public void RegisterInterceptor(string serverIP, int port, INetInterceptor interceptor) {
            string host = serverIP + " - " + port;
            if (sockThreadPool.ContainsKey(host)) {
                sockThreadPool[host].RegisterdInterceptor(interceptor);
            }
        }
        /// <summary>
        /// 反注册数据拦截器
        /// </summary>
        /// <param name="serverIP">服务器IP</param>
        /// <param name="port">服务器端口号</param>
        /// <param name="interceptor">拦截器接口</param>
        public void UnregisterInterceptor(string serverIP, int port, INetInterceptor interceptor) {
            string host = serverIP + " - " + port;
            sockThreadPool.Remove(host);
        }
        /// <summary>
        /// 启动套接字线程
        /// </summary>
        /// <param name="serverIP">服务器IP</param>
        /// <param name="port">服务器端口号</param>
        public void StartThread(string serverIP, int port) {
            string host = serverIP + " - " + port;
            if (sockThreadPool.ContainsKey(host)) {
                if (!sockThreadPool[host].IsAlive) {
                    sockThreadPool[host].Connect();
                }
            }
        }
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="msg">消息字符串</param>
        /// <param name="serverIP">服务器IP</param>
        /// <param name="port">服务器端口号</param>
        public void SendMessage(string msg, string serverIP, int port) {
            string host = serverIP + " - " + port;
            if (sockThreadPool.ContainsKey(host)) {
                if (sockThreadPool[host].IsAlive) {
                    sockThreadPool[host].SendMessage(Encoding.UTF8.GetBytes(msg));
                }
            }
        }
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="msg">二进制消息内容</param>
        /// <param name="serverIP">服务器IP</param>
        /// <param name="port">服务器端口号</param>
        public void SendMessage(byte[] msg, string serverIP, int port) {
            string host = serverIP + " - " + port;
            if (sockThreadPool.ContainsKey(host)) {
                if (sockThreadPool[host].IsAlive) {
                    sockThreadPool[host].SendMessage(msg);
                }
            }
        }
        /// <summary>
        /// 断开指定连接
        /// </summary>
        /// <param name="serverIP">服务器IP</param>
        /// <param name="port">服务器端口号</param>
        public void Disconnect(string serverIP, int port) {
            string host = serverIP + " - " + port;
            if (sockThreadPool.ContainsKey(host)) {
                if (sockThreadPool[host].IsAlive) {
                    sockThreadPool[host].Disconnect();
                }
            }
        }
        /// <summary>
        /// 释放所有Socket资源
        /// </summary>
        public void ReleaseAll() {
            foreach (NetworkThread sth in sockThreadPool.Values) {
                sth.Disconnect();
            }
        }
    }
}
