using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    public float speed = 10.0f;

    void Update()
    {
        float InputX = Input.GetAxisRaw("Horizontal");
        float InputY = Input.GetAxisRaw("Vertical");
        float InputZ = Input.mouseScrollDelta.y;

        transform.Translate(new Vector3(InputX, InputY, 0) * speed * Time.deltaTime);
        GetComponent<Camera>().orthographicSize += InputZ * speed * Time.deltaTime;
    }
}
