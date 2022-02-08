using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{

    void Update()
    {
        if (!Input.GetMouseButtonDown(0)) return;

        Vector3 mouseCoords = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        Vector2 selectedQuad;
        selectedQuad.x = mouseCoords.x / GridManager.Instance.nbrLines;
        selectedQuad.y = mouseCoords.y / GridManager.Instance.nbrColumns;
        Debug.Log(selectedQuad);

        //GridManager.Instance.grid



    }
}
