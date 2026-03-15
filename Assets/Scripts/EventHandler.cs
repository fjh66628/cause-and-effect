using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public static class EventHandler
{
    public static Action<Vector2> onMouseLeftClick;//鼠标点击左键事件

    public static void CallMouseLeftClick(Vector2 position)//调用鼠标点击左键事件
    {
        onMouseLeftClick?.Invoke(position);
    }
}
