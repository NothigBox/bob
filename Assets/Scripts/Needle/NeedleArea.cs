using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeedleArea : MonoBehaviour
{
    [SerializeField] private NeedleManager needle;

    public bool canMoveNeedle;

    private void Awake()
    {
        canMoveNeedle = false;
    }

    private void OnMouseOver()
    {
        if (!canMoveNeedle) 
        {
            Cursor.visible = true;
            return; 
        }

        Cursor.visible = false;

        Vector2 mousePosition = Input.mousePosition;

        var worldPoint = Camera.main.ScreenToWorldPoint(mousePosition);

        needle.SetPosition(worldPoint);
    }

    private void OnMouseExit()
    {
        Cursor.visible = true;
    }
}
