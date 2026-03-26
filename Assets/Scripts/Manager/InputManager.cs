using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager : SingletonMono<InputManager>
{
    private Vector2 moveDirection;

    // 输入缓冲相关变量
    private Vector2 inputBuffer = Vector2.zero; // 输入缓冲队列
    private float bufferTime = 0.2f; // 缓冲时间（秒）
    private float lastInputTime = 0f; // 最后输入时间

    [Header("输入缓冲设置")]
    [SerializeField] private float bufferDuration = 0.2f; // 缓冲持续时间

    public Vector2 GetMoveDirection()
    {
        return moveDirection;
    }

    void Start()
    {
        // 初始化缓冲时间
        bufferTime = bufferDuration;
    }

    void Update()
    {
        // 处理鼠标移动方向
        moveDirection = CameraManager.Instance.MainCamera.ScreenToWorldPoint(Input.mousePosition);

        // 处理鼠标点击事件
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
        {
            EventHandler.CallMouseClick();//调用鼠标点击事件
        }

        // 处理左键点击输入（带缓冲）
        ProcessMouseLeftClickWithBuffer();

        // 处理缓冲队列
        ProcessInputBuffer();
    }

    /// <summary>
    /// 处理鼠标左键点击（带输入缓冲）
    /// </summary>
    void ProcessMouseLeftClickWithBuffer()
    {
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            Vector2 mouseWorldPos = CameraManager.Instance.MainCamera.ScreenToWorldPoint(Input.mousePosition);


            // 启用输入缓冲：将输入加入缓冲队列
            AddToInputBuffer(mouseWorldPos);
            lastInputTime = Time.time;
        }
    }

    /// <summary>
    /// 添加输入到缓冲队列
    /// </summary>
    void AddToInputBuffer(Vector2 inputPosition)
    {


        inputBuffer = inputPosition;
    }

    /// <summary>
    /// 处理输入缓冲队列
    /// </summary>
    void ProcessInputBuffer()
    {


        // 检查是否可以处理缓冲的输入
        if (CanProcessBufferedInput())
        {
            // 处理队列中最旧的输入
            Vector2 bufferedInput = inputBuffer;
            EventHandler.CallMouseLeftClick(bufferedInput);

            // 重置最后输入时间
            lastInputTime = Time.time;
        }
        else if (Time.time - lastInputTime > bufferTime)
        {
            // 缓冲超时，清空队列
            inputBuffer = Vector2.zero;
        }
    }

    /// <summary>
    /// 检查是否可以立即处理输入
    /// </summary>
    bool CanProcessInputImmediately()
    {
        return GameManager.Instance.getGameState == GameState.Play &&
               !GameManager.Instance.IsPlayerMoving;
    }

    /// <summary>
    /// 检查是否可以处理缓冲的输入
    /// </summary>
    bool CanProcessBufferedInput()
    {
        return GameManager.Instance.getGameState == GameState.Play &&
               !GameManager.Instance.IsPlayerMoving &&
               inputBuffer != Vector2.zero;
    }

}