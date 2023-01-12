using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

//用于能够拖拽的序列化
//[System.Serializable]
//public class EventVector3 : UnityEvent<Vector3> { }

public class MouseManager : Singleton<MouseManager>
{
    RaycastHit hitInfo;

    //添加Action事件，在MouseManager中触发
    public event Action<Vector3> OnMouseClicked;
    public event Action<GameObject> OnEnemyClicked; 

    //光标图片素材
    public Texture2D point, doorway, attack, target, arrow;

    protected override void Awake()
    {
        base.Awake();
        //DontDestroyOnLoad(this);
    }

    void Update(){
        SetCursorTexture();
        MouseControl();  
    }
     
    //设置材质贴图
    void SetCursorTexture(){

        //创建射线，方向为鼠标点击方向
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if(Physics.Raycast(ray, out hitInfo))
        {

            //切换贴图
            switch (hitInfo.collider.gameObject.tag)
            {
                case "Ground":
                    Cursor.SetCursor(target, new Vector2(16, 16),CursorMode.Auto);
                    break;
                case "Enemy":
                    Cursor.SetCursor(attack, new Vector2(16, 16), CursorMode.Auto);
                    break;
            }
        }
    }

    void MouseControl(){

        //按下鼠标左键且碰撞体不为空
        if(Input.GetMouseButtonDown(0) && hitInfo.collider != null)
        {
            if(hitInfo.collider.gameObject.CompareTag("Ground"))
            {
                //？判断OnMouseClicked是否为空，空则传入参数唤醒事件，且因为是Action event，所以会执行所有添加进去的事件
                OnMouseClicked?.Invoke(hitInfo.point);
            }

            if (hitInfo.collider.gameObject.CompareTag("Enemy"))
            {
                //？判断OnEnemyClicked是否为空，空则传入参数唤醒事件，且因为是Action event，所以会执行所有添加进去的事件
                OnEnemyClicked?.Invoke(hitInfo.collider.gameObject);
            }

            if (hitInfo.collider.gameObject.CompareTag("Attackable"))
            {
                //？判断OnEnemyClicked是否为空，空则传入参数唤醒事件，且因为是Action event，所以会执行所有添加进去的事件
                OnEnemyClicked?.Invoke(hitInfo.collider.gameObject);
            }
        }
    }
}
