using UnityEngine;
namespace RoachGame.Widgets {
    /// <summary>
    /// 适配器接口
    /// </summary>
    public interface IAdapter {
        int getCount(); // 获取列表项数目
        int getItemId(int index); // 获取项目ID
        object getItem(int index); // 获取项目数据
        GameObject getObject(GameObject prev, int index); // 生成项目对象
    }
}
