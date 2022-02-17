using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

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
            Init();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion

    #region variables

    [SerializeField]
    private TMP_InputField InputField;

    public float camSpeed = 0;

    #endregion
    private void Init()
    {
        camSpeed = (PlayerPrefs.GetInt("nbrLines") + PlayerPrefs.GetInt("nbrColumns"))*2;
    }
    public void CameraInit()
    {
        Vector3 camPos;
        camPos.x = PlayerPrefs.GetInt("nbrColumns")/2;
        camPos.y = PlayerPrefs.GetInt("nbrLines")/2;
        camPos.z = -5;

        int value = PlayerPrefs.GetInt("nbrLines") > PlayerPrefs.GetInt("nbrColumns") ? PlayerPrefs.GetInt("nbrLines") : PlayerPrefs.GetInt("nbrColumns");
        GetComponent<Camera>().orthographicSize = value / 2;

        transform.position = camPos;

    }
    void Update()
    {
        if (EventSystem.current.currentSelectedGameObject == InputField.gameObject) { return; }
        
        float InputX = Input.GetAxisRaw("Horizontal");
        float InputY = Input.GetAxisRaw("Vertical");
        float InputZ = Input.mouseScrollDelta.y;

        transform.Translate(new Vector3(InputX * GetComponent<Camera>().orthographicSize, InputY * GetComponent<Camera>().orthographicSize, 0) * Time.deltaTime);
        GetComponent<Camera>().orthographicSize -= InputZ * camSpeed * Time.deltaTime;
    }
}
