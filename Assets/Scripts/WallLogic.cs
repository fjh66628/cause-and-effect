using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallLogic : MonoBehaviour
{
    void FixedUpdate()
    {
        if (GameManager.Instance.getPlayerState == PlayerState.PassWall)
        {
            foreach (var item in transform.GetComponentsInChildren<SpriteRenderer>())
            {
                item.color = new Color(1, 1, 1, 0.5f);
            }
        }
        else
        {
            foreach (var item in transform.GetComponentsInChildren<SpriteRenderer>())
            {
                item.color = Color.white;
            }
        }
    }
}
