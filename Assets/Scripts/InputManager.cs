using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class InputManager : MonoBehaviour
{
    #region variables

        [SerializeField]
        TMP_InputField _inputField;

    #endregion

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            GridManager.Instance.OnClick();
        }
        else if (Input.GetMouseButton(0))
        {
            GridManager.Instance.OnClickHold();
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            if (EventSystem.current.currentSelectedGameObject == _inputField.gameObject) { return; } //  Prevent action if typing text
            GridManager.Instance.PlayButton();
        }
    }
}
