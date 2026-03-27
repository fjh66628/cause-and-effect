using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.VisualScripting;
using UnityEngine;
[System.Serializable]
public class PlayerMove : MonoBehaviour
{
    [Header("玩家移动参数")]
    [SerializeField] private float moveDuration = 0.4f;//移动时间
    [SerializeField] private Animator animator;//移动动画组件
    [SerializeField] private GameObject direction_RU;//指示上方向的精灵
    [SerializeField] private GameObject direction_RD;//指示下方向的精灵
    [SerializeField] private GameObject direction_LU;//指示左方向的精灵
    [SerializeField] private GameObject direction_LD;//指示右方向的精灵
    private MapManager mapManager;
    private void OnEnable()
    {
        EventHandler.onMouseLeftClick += OnMouseLeftClick;//订阅鼠标点击左键事件
        EventHandler.mapLoaded += OnLevelLoaded;//订阅关卡加载事件
        EventHandler.playerStateChange += playerStateChange;//订阅玩家状态改变事件
        EventHandler.playerStand += PlayerStand;//订阅玩家站立事件
    }
    private void OnDisable()
    {
        EventHandler.onMouseLeftClick -= OnMouseLeftClick;//取消订阅鼠标点击左键事件
        EventHandler.mapLoaded -= OnLevelLoaded;//取消订阅关卡加载事件
        EventHandler.playerStateChange -= playerStateChange;//取消订阅玩家状态改变事件
        EventHandler.playerStand -= PlayerStand;//取消订阅玩家站立事件
    }

    public void OnLevelLoaded()
    {
        mapManager = FindObjectOfType<MapManager>();//获取地图管理器组件
        if (mapManager == null)
        {
            return;
        }
        StartCoroutine(ChangePosition(mapManager.GetPlayerWorldPosition()));//初始化玩家在世界坐标中的位置
    }

    IEnumerator ChangePosition(Vector3 targetPosition)
    {
        GameManager.Instance.SetIsPlayerMoving(true);//设置玩家是正在移动
        animator.SetBool("Moving", true);//设置移动动画触发
        yield return null;//等待一帧
        float elapsedTime = 0f;//已用时间
        Vector3 startPosition = transform.position;//起始位置
        while (elapsedTime < moveDuration)
        {
            elapsedTime += Time.fixedDeltaTime;//更新已用时间
            transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / moveDuration);//插值计算新位置
            yield return new WaitForFixedUpdate();//等待固定时间步长
        }
        animator.SetBool("Moving", false);//设置移动动画触发
        yield return new WaitForSeconds(0.33f);//等待一帧
        GameManager.Instance.SetIsPlayerMoving(false);//设置玩家不是正在移动
    }

    private void OnMouseLeftClick(Vector2 mousePosition)
    {
        Vector2Int direction = mapManager.MoveDrection(mousePosition, transform.position);//获取玩家移动方向
        if (direction != Vector2.zero)
        {
            mapManager.ChangePlayerPosition(direction);//改变玩家在地图中的坐标
        }
        StartCoroutine(ChangePosition(mapManager.GetPlayerWorldPosition()));//改变玩家在世界坐标中的位置
    }

    private void FixedUpdate()//固定时间步长更新
    {

        Vector2Int LU = new Vector2Int(1, 0);//获取玩家移动方向
        Vector2Int RD = new Vector2Int(0, 1);//获取玩家移动方向
        Vector2Int LD = new Vector2Int(-1, 0);//获取玩家移动方向
        Vector2Int RU = new Vector2Int(0, -1);//获取玩家移动方向
        Vector2Int input = mapManager.MoveDrection(InputManager.Instance.GetMoveDirection(), transform.position);
        if (!GameManager.Instance.IsPlayerMoving && GameManager.Instance.getGameState == GameState.Play)//如果玩家不是正在移动
        {
            if (input == LU)
            {
                direction_RU.gameObject.SetActive(true);//设置指示上方向的精灵
                direction_RD.gameObject.SetActive(false);//设置指示下方向的精灵
                direction_LU.gameObject.SetActive(false);//设置指示左方向的精灵
                direction_LD.gameObject.SetActive(false);//设置指示右方向的精灵
            }
            else if (input == RD)
            {
                direction_RD.gameObject.SetActive(true);//设置指示下方向的精灵
                direction_RU.gameObject.SetActive(false);//设置指示上方向的精灵
                direction_LU.gameObject.SetActive(false);//设置指示左方向的精灵
                direction_LD.gameObject.SetActive(false);//设置指示右方向的精灵
            }
            else if (input == LD)
            {
                direction_LU.gameObject.SetActive(true);//设置指示左方向的精灵
                direction_RD.gameObject.SetActive(false);//设置指示下方向的精灵
                direction_RU.gameObject.SetActive(false);//设置指示上方向的精灵
                direction_LD.gameObject.SetActive(false);//设置指示右方向的精灵
                direction_RU.gameObject.SetActive(false);//设置指示右方向的精灵
            }
            else if (input == RU)
            {
                direction_LD.gameObject.SetActive(true);//设置指示右方向的精灵
                direction_RU.gameObject.SetActive(false);//设置指示上方向的精灵
                direction_LU.gameObject.SetActive(false);//设置指示左方向的精灵
                direction_RD.gameObject.SetActive(false);//设置指示下方向的精灵
            }
        }

        else if (GameManager.Instance.getGameState == GameState.Pause)
        {
            direction_RU.gameObject.SetActive(false);//设置指示上方向的精灵
            direction_RD.gameObject.SetActive(false);//设置指示下方向的精灵
            direction_LU.gameObject.SetActive(false);//设置指示左方向的精灵
            direction_LD.gameObject.SetActive(false);//设置指示右方向的精灵
        }
    }

    void playerStateChange(PlayerState state)//玩家状态改变事件
    {
        switch (state)
        {
            case PlayerState.Fly:
                animator.SetTrigger("FlyUp");//设置移动动画触发
                break;
        }
    }
    void PlayerStand()//玩家站立事件
    {
        animator.SetTrigger("FlyDown");//设置移动动画触发
    }
}
