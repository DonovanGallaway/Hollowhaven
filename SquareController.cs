using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquareController : MonoBehaviour
{
    public Vector2Int gridPosition;
    public GridManager gridManager;

    void OnMouseDown()
    {
        if (Input.GetMouseButtonDown(0)) // Left click
        {
            if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) // Ctrl + Left click
            {
                Debug.Log("Ctrl + click detected");
                gridManager.OnSquareRightClicked(gridPosition.x, gridPosition.y);
            }
            else // Regular left click
            {
                gridManager.OnSquareClicked(gridPosition.x, gridPosition.y);
            }
        }
    }
}
