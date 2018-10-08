// File create date:6/6/2018
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
// Created By Yu.Liu
namespace RoachGame.Network {
    /// <summary>
    /// 拦截器处理线程
    /// </summary>
    public class InterceptorThread {

        private Thread _thread; // 拦截线程
        private Queue<byte[]> _interceptQueue; // 拦截到的消息队列
        private INetInterceptor _interceptor; // 拦截器接口
        private AutoResetEvent _interceptSignal; // 拦截消息触发器
        private bool isIntercepting = false; // 拦截器运行情况
        public bool isAvailable { // 是否可用
            get {
                return (_thread == null || !_thread.IsAlive);
            }
        }

        public InterceptorThread(INetInterceptor interceptor) {
            _interceptor = interceptor;
            _interceptQueue = new Queue<byte[]>();
            _interceptSignal = new AutoResetEvent(false);
        }
        /// <summary>
        /// 启动一次拦截
        /// </summary>
        /// <param name="data">被拦截的二进制消息数据</param>
        public void Run(byte[] data) {
            _interceptQueue.Enqueue(data);
            _interceptSignal.Set();
            if (isAvailable) {
                _thread = new Thread(new ThreadStart(onIntercept));
                _thread.Start();
            }
        }
        /// <summary>
        /// 停止拦截器线程
        /// </summary>
        public void Stop() {
            isIntercepting = false;
            _interceptQueue.Clear();
            _interceptSignal.Close();
        }
        /// <summary>
        /// 拦截器循环
        /// </summary>
        private void onIntercept() {
            while (isIntercepting) {
                try {
                    if (_interceptor != null) {
                        if (_interceptQueue.Count > 0) {
                            byte[] data = _interceptQueue.Dequeue();
                            _interceptor.Intercept(data);
                            Thread.Sleep(0);
                        } else {
                            _interceptSignal.WaitOne();
                        }
                    }
                } catch (Exception e) {
                    ProcessException(e);
                    isIntercepting = false;
                }
            }
        }
        /// <summary>
        /// 异常处理
        /// </summary>
        /// <param name="e"></param>
        protected virtual void ProcessException(Exception e) {
            Debug.LogError("Exception catched in Interceptor thread!");
            Debug.Log(e.Message + "\n" + e.StackTrace);
        }
    }
}
