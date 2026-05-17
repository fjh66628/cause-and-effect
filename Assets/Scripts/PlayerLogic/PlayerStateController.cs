using UnityEngine;

/// <summary>
/// 玩家状态控制器 - 解耦状态相关逻辑
/// 处理玩家状态改变时的各种检查和触发
/// </summary>
public class PlayerStateController : MonoBehaviour
{
    private GameManager gameManager;
    private MapManager mapManager;
    
    void Awake()
    {
        gameManager = GameManager.Instance;
        mapManager = FindObjectOfType<MapManager>();
    }
    
    void OnEnable()
    {
        EventHandler.playerStateChange += OnPlayerStateChange;
    }
    
    void OnDisable()
    {
        EventHandler.playerStateChange -= OnPlayerStateChange;
    }
    
    // 玩家状态改变时的处理
    void OnPlayerStateChange(PlayerState newState)
    {
        if (gameManager == null || mapManager == null) return;
        
        // 如果玩家从飞行状态切换到其他状态，触发退出飞行动画
        PlayerState oldState = gameManager.getPlayerState;
        
        if (oldState == PlayerState.Fly && newState != PlayerState.Fly)
        {
            // 触发FlyDown动画（通过EventHandler.playerStand事件）
            EventHandler.CallPlayerStand();
        }
        
        // 检查新状态下玩家是否在特殊格子中需要GameOver
        CheckGameOverForState(newState);
    }
    
    // 根据状态检查是否需要触发GameOver
    void CheckGameOverForState(PlayerState state)
    {
        if (mapManager == null) return;
        
        // 如果是飞行状态，不需要检查（飞行状态可以待在任何格子上）
        if (state == PlayerState.Fly) return;
        
        // 检查玩家当前位置
        mapManager.CheckPlayerPosition();
    }
    
    // 公开方法：手动改变玩家状态（供其他类调用）
    public void ChangePlayerState(PlayerState newState)
    {
        if (gameManager != null)
        {
            gameManager.ChangePlayerState(newState);
        }
    }
}
