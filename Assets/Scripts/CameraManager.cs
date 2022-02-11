using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{

    #region Singleton
    public static CameraManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion

    public float camSpeed = 20.0f;

    public void CameraInit()
    {
        Vector3 camPos;
        camPos.x = PlayerPrefs.GetInt("nbrColumns")/2;
        camPos.y = PlayerPrefs.GetInt("nbrLines")/2;
        camPos.z = -5;

        GetComponent<Camera>().orthographicSize = PlayerPrefs.GetInt("nbrLines")/2;

        transform.position = camPos;

    }

    void Update()
    {
        float InputX = Input.GetAxisRaw("Horizontal");
        float InputY = Input.GetAxisRaw("Vertical");
        float InputZ = Input.mouseScrollDelta.y;

        transform.Translate(new Vector3(InputX, InputY, 0) * camSpeed * Time.deltaTime);
        GetComponent<Camera>().orthographicSize -= InputZ * camSpeed * Time.deltaTime;
    }
}
