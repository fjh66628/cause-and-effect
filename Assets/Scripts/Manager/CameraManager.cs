using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class CameraManager : SingletonMono<CameraManager>
{
    void OnEnable()
    {
        EventHandler.levelLoaded += RefreshCamera;//添加关卡加载事件监听
    }
    void OnDisable()
    {
        EventHandler.levelLoaded -= RefreshCamera;//移除关卡加载事件监听
    }
    [SerializeField]float t = 0.5f;//相机跟随玩家的速度
    private Camera mainCamera;//相机
    public Camera MainCamera
    {
        get
        {
            if(mainCamera == null)
            {
                mainCamera = Camera.main;
                if(mainCamera == null)
                {
                    mainCamera = FindObjectOfType<Camera>();
                    if(mainCamera == null)
                    {
                        Debug.LogError("没有找到相机");
                    }
                }

            }
            return mainCamera;
        }
    }
    protected override void InitializeSingleton()
    {
        _=MainCamera;
    }
    public void RefreshCamera()
    {
        mainCamera=null;
        _=MainCamera;
    }

    void FixedUpdate()
    {
        if(!mainCamera)
        {
            return;
        }
        PlayerMove playerMove = FindObjectOfType<PlayerMove>();
        if(playerMove == null)
        {

            return;
        }
        //相机跟随玩家
        MainCamera.transform.position = new Vector3(t*playerMove.transform.position.x, t*playerMove.transform.position.y, MainCamera.transform.position.z);
    }
}
