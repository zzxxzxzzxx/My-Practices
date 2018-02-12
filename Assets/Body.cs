using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Body : MonoBehaviour {
    //下一个身子的引用
    public Body next;
    public void Move(Vector3 pos)
    {
        //记录当前身子的位置
        Vector3 nextPos = transform.position;
        //当前身子移动
        transform.position = pos;
        if (next != null)
        {
            //后面身子移动
            next.Move(nextPos);
        }
    }
}
