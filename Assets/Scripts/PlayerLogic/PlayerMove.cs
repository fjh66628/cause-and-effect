using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.VisualScripting;
using UnityEngine;
[System.Serializable]
public class PlayerMove : MonoBehaviour
{
    [Header("鐜╁绉诲姩鍙傛暟")]
    [SerializeField] private float moveDuration = 0.4f;//绉诲姩鏃堕棿
    [SerializeField] private Animator animator;//绉诲姩鍔ㄧ敾缁勪欢
    [SerializeField] private GameObject direction_RU;//鎸囩ず涓婃柟鍚戠殑绮剧伒
    [SerializeField] private GameObject direction_RD;//鎸囩ず涓嬫柟鍚戠殑绮剧伒
    [SerializeField] private GameObject direction_LU;//鎸囩ず宸︽柟鍚戠殑绮剧伒
    [SerializeField] private GameObject direction_LD;//鎸囩ず鍙虫柟鍚戠殑绮剧伒
    private MapManager mapManager;
    private void Awake()
    {
        HideDirectionSprites();
    }

    private void OnEnable()
    {
        EventHandler.onMouseLeftClick += OnMouseLeftClick;//璁㈤槄榧犳爣鐐瑰嚮宸﹂敭浜嬩欢
        EventHandler.mapLoaded += OnLevelLoaded;//璁㈤槄鍏冲崱鍔犺浇浜嬩欢
        EventHandler.playerStateChange += playerStateChange;//璁㈤槄鐜╁鐘舵€佹敼鍙樹簨浠?
        EventHandler.playerStand += PlayerStand;//璁㈤槄鐜╁绔欑珛浜嬩欢
    }
    private void OnDisable()
    {
        EventHandler.onMouseLeftClick -= OnMouseLeftClick;//鍙栨秷璁㈤槄榧犳爣鐐瑰嚮宸﹂敭浜嬩欢
        EventHandler.mapLoaded -= OnLevelLoaded;//鍙栨秷璁㈤槄鍏冲崱鍔犺浇浜嬩欢
        EventHandler.playerStateChange -= playerStateChange;//鍙栨秷璁㈤槄鐜╁鐘舵€佹敼鍙樹簨浠?
        EventHandler.playerStand -= PlayerStand;//鍙栨秷璁㈤槄鐜╁绔欑珛浜嬩欢
    }

    public void OnLevelLoaded()
    {
        HideDirectionSprites();
        mapManager = FindObjectOfType<MapManager>();//鑾峰彇鍦板浘绠＄悊鍣ㄧ粍浠?
        if (mapManager == null)
        {
            return;
        }
        StartCoroutine(ChangePosition(mapManager.GetPlayerWorldPosition()));//鍒濆鍖栫帺瀹跺湪涓栫晫鍧愭爣涓殑浣嶇疆
    }

    IEnumerator ChangePosition(Vector3 targetPosition)
    {
        GameManager.Instance.SetIsPlayerMoving(true);//璁剧疆鐜╁鏄鍦ㄧЩ鍔?
        animator.SetBool("Moving", true);//璁剧疆绉诲姩鍔ㄧ敾瑙﹀彂
        yield return null;//绛夊緟涓€甯?
        float elapsedTime = 0f;//宸茬敤鏃堕棿
        Vector3 startPosition = transform.position;//璧峰浣嶇疆
        while (elapsedTime < moveDuration)
        {
            elapsedTime += Time.fixedDeltaTime;//鏇存柊宸茬敤鏃堕棿
            transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / moveDuration);//鎻掑€艰绠楁柊浣嶇疆
            yield return new WaitForFixedUpdate();//绛夊緟鍥哄畾鏃堕棿姝ラ暱
        }
        animator.SetBool("Moving", false);//璁剧疆绉诲姩鍔ㄧ敾瑙﹀彂
        yield return new WaitForSeconds(0.33f);//绛夊緟涓€甯?
        GameManager.Instance.SetIsPlayerMoving(false);//璁剧疆鐜╁涓嶆槸姝ｅ湪绉诲姩
    }

    private void OnMouseLeftClick(Vector2 mousePosition)
    {
        Vector2Int direction = mapManager.MoveDrection(mousePosition, transform.position);//鑾峰彇鐜╁绉诲姩鏂瑰悜
        if (direction != Vector2.zero)
        {
            mapManager.ChangePlayerPosition(direction);//鏀瑰彉鐜╁鍦ㄥ湴鍥句腑鐨勫潗鏍?
        }
        StartCoroutine(ChangePosition(mapManager.GetPlayerWorldPosition()));//鏀瑰彉鐜╁鍦ㄤ笘鐣屽潗鏍囦腑鐨勪綅缃?
    }

    private void FixedUpdate()//鍥哄畾鏃堕棿姝ラ暱鏇存柊
    {

        Vector2Int LU = new Vector2Int(1, 0);//鑾峰彇鐜╁绉诲姩鏂瑰悜
        Vector2Int RD = new Vector2Int(0, 1);//鑾峰彇鐜╁绉诲姩鏂瑰悜
        Vector2Int LD = new Vector2Int(-1, 0);//鑾峰彇鐜╁绉诲姩鏂瑰悜
        Vector2Int RU = new Vector2Int(0, -1);//鑾峰彇鐜╁绉诲姩鏂瑰悜
        Vector2Int input = mapManager.MoveDrection(InputManager.Instance.GetMoveDirection(), transform.position);
        if (!GameManager.Instance.IsPlayerMoving && GameManager.Instance.getGameState == GameState.Play)//濡傛灉鐜╁涓嶆槸姝ｅ湪绉诲姩
        {
            if (input == LU)
            {
                direction_RU.gameObject.SetActive(true);//璁剧疆鎸囩ず涓婃柟鍚戠殑绮剧伒
                direction_RD.gameObject.SetActive(false);//璁剧疆鎸囩ず涓嬫柟鍚戠殑绮剧伒
                direction_LU.gameObject.SetActive(false);//璁剧疆鎸囩ず宸︽柟鍚戠殑绮剧伒
                direction_LD.gameObject.SetActive(false);//璁剧疆鎸囩ず鍙虫柟鍚戠殑绮剧伒
            }
            else if (input == RD)
            {
                direction_RD.gameObject.SetActive(true);//璁剧疆鎸囩ず涓嬫柟鍚戠殑绮剧伒
                direction_RU.gameObject.SetActive(false);//璁剧疆鎸囩ず涓婃柟鍚戠殑绮剧伒
                direction_LU.gameObject.SetActive(false);//璁剧疆鎸囩ず宸︽柟鍚戠殑绮剧伒
                direction_LD.gameObject.SetActive(false);//璁剧疆鎸囩ず鍙虫柟鍚戠殑绮剧伒
            }
            else if (input == LD)
            {
                direction_LU.gameObject.SetActive(true);//璁剧疆鎸囩ず宸︽柟鍚戠殑绮剧伒
                direction_RD.gameObject.SetActive(false);//璁剧疆鎸囩ず涓嬫柟鍚戠殑绮剧伒
                direction_RU.gameObject.SetActive(false);//璁剧疆鎸囩ず涓婃柟鍚戠殑绮剧伒
                direction_LD.gameObject.SetActive(false);//璁剧疆鎸囩ず鍙虫柟鍚戠殑绮剧伒
                direction_RU.gameObject.SetActive(false);//璁剧疆鎸囩ず鍙虫柟鍚戠殑绮剧伒
            }
            else if (input == RU)
            {
                direction_LD.gameObject.SetActive(true);//璁剧疆鎸囩ず鍙虫柟鍚戠殑绮剧伒
                direction_RU.gameObject.SetActive(false);//璁剧疆鎸囩ず涓婃柟鍚戠殑绮剧伒
                direction_LU.gameObject.SetActive(false);//璁剧疆鎸囩ず宸︽柟鍚戠殑绮剧伒
                direction_RD.gameObject.SetActive(false);//璁剧疆鎸囩ず涓嬫柟鍚戠殑绮剧伒
            }
        }

        else if (GameManager.Instance.getGameState == GameState.Pause || GameManager.Instance.getGameState == GameState.SelectCard)
        {
            HideDirectionSprites();
        }
    }

    void HideDirectionSprites()
    {
        SetDirectionActive(direction_RU, false);
        SetDirectionActive(direction_RD, false);
        SetDirectionActive(direction_LU, false);
        SetDirectionActive(direction_LD, false);
    }

    void SetDirectionActive(GameObject direction, bool isActive)
    {
        if (direction != null)
        {
            direction.SetActive(isActive);
        }
    }
    void playerStateChange(PlayerState state)//鐜╁鐘舵€佹敼鍙樹簨浠?
    {
        switch (state)
        {
            case PlayerState.Fly:
                animator.SetTrigger("FlyUp");//璁剧疆绉诲姩鍔ㄧ敾瑙﹀彂
                break;
            case PlayerState.Stand:
                PlayerStand();
                break;
        }
    }
    void PlayerStand()//鐜╁绔欑珛浜嬩欢
    {
        animator.SetTrigger("FlyDown");//璁剧疆绉诲姩鍔ㄧ敾瑙﹀彂
    }
}


