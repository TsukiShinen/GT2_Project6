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

    #region Variables

        [SerializeField]
        private TMP_InputField _inputField;

        [SerializeField]
        private float _speed = 2;

        private float _camSpeed = 0;

    #endregion

    private void Init()
    {
        _camSpeed = (PlayerPrefs.GetInt("nbrLines") + PlayerPrefs.GetInt("nbrColumns"))*_speed;
    }

    public void CameraInit() // Place the camera accordingly to the grid size
    {
        Vector3 camPos;
        camPos.x = PlayerPrefs.GetInt("nbrColumns")/3;
        camPos.y = PlayerPrefs.GetInt("nbrLines")/2;
        camPos.z = -5; // z axis doesnt matter as long as it is < 0

        int longestSide = PlayerPrefs.GetInt("nbrLines") > PlayerPrefs.GetInt("nbrColumns") ? PlayerPrefs.GetInt("nbrLines") : PlayerPrefs.GetInt("nbrColumns");
        GetComponent<Camera>().orthographicSize = longestSide / 2;

        transform.position = camPos;
    }

    void Update()
    {
        if (EventSystem.current.currentSelectedGameObject == _inputField.gameObject) { return; } // Prevent movement if typing text
        
        float InputX = Input.GetAxisRaw("Horizontal");
        float InputY = Input.GetAxisRaw("Vertical");
        float InputZ = Input.mouseScrollDelta.y;

        transform.Translate(new Vector3(InputX * GetComponent<Camera>().orthographicSize, InputY * GetComponent<Camera>().orthographicSize, 0) * Time.deltaTime);
        GetComponent<Camera>().orthographicSize -= InputZ * _camSpeed * Time.deltaTime;
    }
}
