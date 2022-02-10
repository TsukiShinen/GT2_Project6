using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    private Vector2 _position;
    private bool _alive = false;

    public Cell(Vector2 pPosition)
    {
        _position = pPosition;
    }


}
