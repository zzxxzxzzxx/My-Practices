// File create date:6/6/2018
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;
// Created By Yu.Liu
namespace RoachGame.Network {
    /// <summary>
    /// 网络连接管理线程
    /// </summary>
    public class NetworkThread {
        public static int TEMP_BUFFER_SIZE = 1024; // 临时缓冲区大小
        // 工作线程
        protected Thread _connect; // 连接线程
        protected Thread _sender; // 消息发送线程
        protected Thread _receiver; // 消息接收线程
        protected Dictionary<INetInterceptor, InterceptorThread> _interceptThreads; // 拦截器线程
        // 套接字相关
        protected Socket _socket; // 套接字对象
        protected string _ip; // 连接IP地址
        protected int _port; // 连接端口号
        // 数据与缓冲区
        protected byte[] tempBuffer; // 临时数据缓冲区
        protected NetworkBuffer receiverBuffer; // 数据缓存区
        protected IDataProtocol dataProtocol; // 数据协议接口
        // 消息相关
        protected Queue<byte[]> messageQueue; // 消息队列
        protected int receivedBytes = 0; // 接收到的字节数
        protected IDispatcher messageDispatcher; // 消息分发器接口
        // 标志位
        protected bool isConnected = false; // 是否连接上
        protected bool senderRunning = false; // 发送线程是否工作
        protected bool receiverRunning = false; // 接收线程是否工作
        protected AutoResetEvent sendEvent; // 发送线程触发器
        public bool IsAlive { // 当前线程是否存活
            get {
                return isConnected && senderRunning && receiverRunning;
            }
        }

        protected NetworkThread() {
            _interceptThreads = new Dictionary<INetInterceptor, InterceptorThread>();
            tempBuffer = new byte[TEMP_BUFFER_SIZE];
            sendEvent = new AutoResetEvent(false);
            messageQueue = new Queue<byte[]>();
            receiverBuffer = new NetworkBuffer();
            receiverBuffer.setProtocol(new SimpleDataProtocol());
        }
        /// <summary>
        /// 静态方法获取一个套接字线程
        /// </summary>
        /// <returns>套接字线程</returns>
        public static NetworkThread GetThread() {
            return new NetworkThread();
        }
        /// <summary>
        /// 设置消息分发器
        /// </summary>
        /// <param name="dispatcher">分发器接口</param>
        /// <returns></returns>
        public NetworkThread SetupDispatcher(IDispatcher dispatcher) {
            messageDispatcher = dispatcher;
            return this;
        }
        /// <summary>
        /// 设置服务器地址和端口号
        /// </summary>
        /// <param name="ip">服务器IP地址</param>
        /// <param name="port">服务器端口号</param>
        /// <returns></returns>
        public NetworkThread SetupAddressAndPort(string ip, int port) {
            _ip = ip;
            _port = port;
            return this;
        }
        /// <summary>
        /// 设置数据协议
        /// </summary>
        /// <param name="protocol">数据协议接口</param>
        /// <returns></returns>
        public NetworkThread SetupDataProtocol(IDataProtocol protocol) {
            dataProtocol = protocol;
            receiverBuffer.setProtocol(dataProtocol);
            return this;
        }
        /// <summary>
        /// 注册数据拦截器
        /// </summary>
        /// <param name="interceptor">拦截器接口</param>
        public void RegisterdInterceptor(INetInterceptor interceptor) {
            _interceptThreads.Add(interceptor, new InterceptorThread(interceptor));
        }
        /// <summary>
        /// 反注册数据拦截器
        /// </summary>
        /// <param name="interceptor">拦截器接口</param>
        public void UnregisterInterceptor(INetInterceptor interceptor) {
            _interceptThreads.Remove(interceptor);
        }
        /// <summary>
        /// 开始连接
        /// </summary>
        public void Connect() {
            if (!isConnected) {
                if (_socket == null || !_socket.Connected) {
                    if (_connect == null || !_connect.IsAlive) {
                        _connect = new Thread(new ThreadStart(onConnect));
                        _connect.Start();
                    }
                }
            }
        }
        /// <summary>
        /// 断开连接
        /// </summary>
        public void Disconnect() {
            if (isConnected) {
                isConnected = false;
                senderRunning = false;
                receiverRunning = false;
                if (_socket != null && _socket.Connected) {
                    ReleaseSocket();
                    _socket = null;
                }
            }
        }
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="msg">二进制消息数据</param>
        public void SendMessage(byte[] msg) {
            messageQueue.Enqueue(msg);
            sendEvent.Set();
        }
        /// <summary>
        /// 消息发送循环
        /// </summary>
        protected virtual void onSender() {
            while (senderRunning) {
                try {
                    if (_socket.Connected) {
                        if (messageQueue.Count > 0) {
                            byte[] sendBuffer = messageQueue.Dequeue();
                            _socket.BeginSend(sendBuffer, 0, sendBuffer.Length, SocketFlags.None, onSendComplete, _socket);
                        } else {
                            sendEvent.WaitOne();
                        }
                        Thread.Sleep(0);
                    }
                } catch (Exception e) {
                    ProcessException(e);
                    senderRunning = false;
                }
            }
        }
        /// <summary>
        /// 消息接收循环
        /// </summary>
        protected virtual void onReceiver() {
            while (receiverRunning) {
                try {
                    if (_socket.Connected) {
                        int receiveLength = _socket.Receive(tempBuffer);
                        if (receiveLength > 0) {
                            receiverBuffer.AddData(tempBuffer, receiveLength);
                            byte[] dataArr = receiverBuffer.GetData();
                            if (dataArr != null) {
                                foreach (InterceptorThread ith in _interceptThreads.Values) {
                                    ith.Run(dataArr);
                                }
                                if (messageDispatcher != null) {
                                    messageDispatcher.Dispatch(dataArr);
                                }
                            }
                        }
                        Thread.Sleep(0);
                    }
                } catch (Exception e) {
                    ProcessException(e);
                    receiverRunning = false;
                }
            }
        }
        /// <summary>
        /// 连接过程
        /// </summary>
        protected virtual void onConnect() {
            if (_socket == null) {
                try {
                    _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    IPAddress addr = IPAddress.Parse(_ip);
                    IPEndPoint endPoint = new IPEndPoint(addr, _port);
                    IAsyncResult result = _socket.BeginConnect(endPoint, onConnectResult, _socket);
                    bool success = result.AsyncWaitHandle.WaitOne(5000, true);
                    if (!success) {
                        // Time out
                        onConnectTimeout();
                    }
                } catch (Exception e) {
                    onConnectFail();
                    Debug.Log(e.Message + "\n" + e.StackTrace);
                }
            }
        }
        /// <summary>
        /// 连接异步回调结果
        /// </summary>
        /// <param name="ar"></param>
        protected virtual void onConnectResult(IAsyncResult ar) {
            Debug.Log("Connection established!");
            isConnected = true;
            senderRunning = true;
            receiverRunning = true;
            _sender = new Thread(new ThreadStart(onSender));
            _receiver = new Thread(new ThreadStart(onReceiver));
            _sender.Start();
            _receiver.Start();
        }
        /// <summary>
        /// 连接超时回调
        /// </summary>
        protected virtual void onConnectTimeout() {
            Debug.LogError("Connection time out!");
        }
        /// <summary>
        /// 连接失败回调
        /// </summary>
        protected virtual void onConnectFail() {
            Debug.LogError("Connection failed!");
        }
        /// <summary>
        /// 发送完成回调
        /// </summary>
        /// <param name="ar"></param>
        protected virtual void onSendComplete(IAsyncResult ar) {
            Debug.Log("Message Send!");
        }
        /// <summary>
        /// 异常处理
        /// </summary>
        /// <param name="e"></param>
        protected virtual void ProcessException(Exception e) {
            Debug.LogError("Exception catched in Socket thread!");
            Debug.Log(e.Message + "\n" + e.StackTrace);
            ReleaseSocket();
        }
        /// <summary>
        /// 释放套接字对象
        /// </summary>
        protected virtual void ReleaseSocket() {
            if (_socket != null) {
                _socket.Disconnect(false);
                _socket.Close();
            }
        }
    }
}
