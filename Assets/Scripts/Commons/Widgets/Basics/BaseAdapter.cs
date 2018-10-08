using System;
using UnityEngine;

namespace RoachGame.Widgets {
    /// <summary>
    /// 适配器基类
    /// </summary>
    public abstract class BaseAdapter : IAdapter {

        /// <summary>
        /// 列表刷新回调接口引用
        /// </summary>
        protected IUpdateViews listReference;
        /// <summary>
        /// 关联对象的标签字符串
        /// </summary>
        protected string relatedObject;
        /// <summary>
        /// 关联的列表对象名字
        /// </summary>
        protected string listObjectName;
        /// <summary>
        /// 返回需要显示的数据项目个数
        /// </summary>
        /// <returns>项目个数</returns>
        public abstract int getCount();
        /// <summary>
        /// 通过索引值获取对应的数据项
        /// </summary>
        /// <param name="index">索引值</param>
        /// <returns>数据项</returns>
        public abstract object getItem(int index);
        /// <summary>
        /// 通过索引值获取项目唯一ID
        /// </summary>
        /// <param name="index">索引值</param>
        /// <returns>唯一ID</returns>
        public abstract int getItemId(int index);
        /// <summary>
        /// 通过索引值获取对应的游戏展示对象，存在重用机制
        /// </summary>
        /// <param name="prev">重用对象</param>
        /// <param name="index">索引值</param>
        /// <returns>更新后的对象</returns>
        public abstract GameObject getObject(GameObject prev, int index);
        /// <summary>
        /// 通过索引值获取项目类型
        /// </summary>
        /// <param name="index">索引值</param>
        /// <returns>项目类型</returns>
        public virtual int getObjectType(int index) {
            throw new ObjectTypeMismatchException("No Object Type can be used!");
        }
        /// <summary>
        /// 通过项目类型获取尺寸
        /// </summary>
        /// <param name="index">项目类型</param>
        /// <returns>尺寸数值</returns>
        public virtual float getSizeByType(int viewType) {
            throw new NotImplementedException("Cannot get size by item type, no such implementation!");
        }
        /// <summary>
        /// 设置关联对象标签
        /// </summary>
        /// <param name="obj">标签字符串</param>
        public void setRelatedObject(string obj) {
            relatedObject = obj;
        }
        /// <summary>
        /// 设置列表引用接口和名称
        /// </summary>
        /// <param name="list">列表对象</param>
        public void setupListReference(BaseView list) { // 设置列表引用
            listReference = list;
            listObjectName = list.name;
        }
        /// <summary>
        /// 通知列表更新
        /// </summary>
        public void notifyDataChanged() {
            if (listReference != null) {
                listReference.UpdateViews();
            }
        }
        /// <summary>
        /// 多种子项类型的列表中必须知道索引对应的子项的尺寸，如果未重写getObjectType则抛出该异常
        /// </summary>
        public class ObjectTypeMismatchException : ApplicationException {
            private string error;
            private Exception innerException;
            //无参数构造函数
            public ObjectTypeMismatchException() {
                error = "Object Type Mismatch! Please check the adapter implementation!";
            }
            //带一个字符串参数的构造函数，作用：当程序员用Exception类获取异常信息而非ObjectTypeMismatchException时把自定义异常信息传递过去
            public ObjectTypeMismatchException(string msg) : base(msg) {
                error = msg;
            }
            //带有一个字符串参数和一个内部异常信息参数的构造函数
            public ObjectTypeMismatchException(string msg, Exception innerException) : base(msg) {
                this.innerException = innerException;
                error = msg;
            }
            public string GetError() {
                return error;
            }
            public string GetInnerError() {
                if (innerException != null) {
                    return innerException.Message;
                }
                return "No Inner Exception Catched!";
            }
        }
    }
}
