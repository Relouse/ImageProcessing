using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class OriginalImageWrapper : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private bool onImage;
    private PointerEventData eventData;
    void Update()
    {
        if(onImage)
        {
            if (Input.GetAxis("Mouse X") == 0 || Input.GetAxis("Mouse Y") == 0) return;//Чтобы убрать вывод координат, когда мышка не шевелится, небольшая оптимизация
            PrintCoordinates(eventData);
        }
    }
    public void PrintCoordinates(PointerEventData dat)
    {
        Vector2 localCursor;
        var rect1 = GetComponent<RectTransform>();
        var pos1 = dat.position;
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(rect1, pos1,
            null, out localCursor))
            return;

        int xpos = (int)(localCursor.x);
        int ypos = (int)(localCursor.y);

        if (xpos < 0) xpos = xpos + (int)rect1.rect.width / 2;
        else xpos += (int)rect1.rect.width / 2;

        if (ypos > 0) ypos = ypos + (int)rect1.rect.height / 2;
        else ypos += (int)rect1.rect.height / 2;

        UIManager.Instance.PrintImageCoordinates(xpos, ypos);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        onImage = true;
        this.eventData = eventData;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        onImage = false;
    }
}
