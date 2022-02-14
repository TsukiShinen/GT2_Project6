using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            GridManager.Instance.OnClick();
        }
        else if (Input.GetMouseButton(0))
        {
            GridManager.Instance.OnClickStay();
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            GridManager.Instance.StartGame();
        }
    }
}
