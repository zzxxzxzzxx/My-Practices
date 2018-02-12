using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//snack移动方向
public enum HeadDir
{
    Up, //上
    Down, //下
    Left, //左
    Right, //右 
}
public class HeadCtrl : MonoBehaviour {
    //身子预设
    public GameObject bodyProfab;
    //食物预设
    public GameObject foodPrefab;
    // 移动速度 m/s
    public float speed;
    //计时器
    private float _Timer = 0f;
    //snack当前移动方向
    private HeadDir _CurrentDir = HeadDir.Up;
    //snack下一步移动方向
    private HeadDir _NextDir = HeadDir.Up;
    //游戏是否结束
    private bool _IsOver = false;
    //第一节身身子
    private Body _FirstBody;
    //最后一节身子
    private Body _LastBody;
    public void Creatfood()
    {
        //随机生成食物的位置
        float x = Random.Range(-9.5f, 9.5f);
        float z = Random.Range(-9.5f, 9.5f);
        //动态创建食物
        GameObject obj = Instantiate(foodPrefab, new Vector3(x, 0f, z), Quaternion.identity) as GameObject;
    }
    //触发事件
    public void OnTriggerEnter(Collider other)
    {
        //碰到边界
        if (other.tag.Equals("Bound") || other.tag.Equals("Body"))
        {
            //游戏结束
            _IsOver = true;
        }
        //碰到食物
        if (other.tag.Equals("Food"))
        {
            //删除原来的食物
            Destroy(other.gameObject);
            //snack成长
            Grow();
            //随机创建新的食物
            Creatfood();
        }

    }
    private void Grow()
    {
        //动态创建身子
        GameObject obj = Instantiate(bodyProfab, new Vector3(1000f, 1000f, 1000f), Quaternion.identity) as GameObject;
        //获取身子上的body脚本
        Body b = obj.GetComponent<Body>();
        //给第一个身子赋值
        if (_FirstBody == null)
        {
            //把已创建好的给第一节身子
            _FirstBody = b;
        }
        //判断最后一节身子是否定义
        if (_LastBody != null)
        {
            //给最后一节加身子
            _LastBody.next = b;
        }
        //更新最后一节身子
        _LastBody = b;
    }
    private void Start()
    {
        //游戏开始创建一个食物
        Creatfood();
    }
    private void Update()
    {
        //判断游戏是否结束
        if (!_IsOver)
        {
            //调用turn实现snack方向旋转
            Turn();
            //调用move实现snack移动
            Move();
        }
    }
    private void Turn()
    {
        //检测w按键
        if (Input.GetKey(KeyCode.W))
        {
            _NextDir = HeadDir.Up;
            //判断按键是否无效化
            if (_CurrentDir == HeadDir.Down)
            {
                _NextDir = _CurrentDir;
            }
        }
        //检测s按键
        if (Input.GetKey(KeyCode.S))
        {
            _NextDir = HeadDir.Down;
            //判断按键是否无效化
            if (_CurrentDir == HeadDir.Up)
            {
                _NextDir = _CurrentDir;
            }
        }
        //检测a按键
        if (Input.GetKey(KeyCode.A))
        {
            _NextDir = HeadDir.Left;
            //判断按键是否无效化
            if (_CurrentDir == HeadDir.Right)
            {
                _NextDir = _CurrentDir;
            }
        }
        //检测d按键
        if (Input.GetKey(KeyCode.D))
        {
            _NextDir = HeadDir.Right;
            //判断按键是否无效化
            if (_CurrentDir == HeadDir.Left)
            {
                _NextDir = _CurrentDir;
            }
        }
    }
    private void Move() 
    {
        //计时器计时
        _Timer += Time.deltaTime;
        //根据速度判断该帧是否该移动
        if (_Timer >= 1f / speed)
        {
            //旋转snack的头
            switch (_NextDir)
            {
                case HeadDir.Up:
                    transform.forward = Vector3.forward;
                    _CurrentDir = HeadDir.Up;
                    break;
                case HeadDir.Down:
                    transform.forward = Vector3.back;
                    _CurrentDir = HeadDir.Down;
                    break;
                case HeadDir.Left:
                    transform.forward = Vector3.left;
                    _CurrentDir = HeadDir.Left;
                    break;
                case HeadDir.Right:
                    transform.forward = Vector3.right;
                    _CurrentDir = HeadDir.Right;
                    break;
            }
            //记录头部之前移动的位置
            Vector3 nextPos = transform.position;
            //snack向前移动一格
            transform.Translate(Vector3.forward);
            //计时器清零
            _Timer = 0f;
            //如果有身子就让第一节身子移动
            if (_FirstBody != null)
            {
                _FirstBody.Move(nextPos);
            }
        }
    }

}
