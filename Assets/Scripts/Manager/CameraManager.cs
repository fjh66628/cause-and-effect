using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class CameraManager : SingletonMono<CameraManager>
{
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
        MapManager mapManager = FindObjectOfType<MapManager>();
        //相机跟随玩家
        MainCamera.transform.position = new Vector3(t*mapManager.GetPlayerWorldPosition().x, t*mapManager.GetPlayerWorldPosition().y, MainCamera.transform.position.z);
    }
}
