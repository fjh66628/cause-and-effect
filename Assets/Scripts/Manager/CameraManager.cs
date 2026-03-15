using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : SingletonMono<CameraManager>
{
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
}
