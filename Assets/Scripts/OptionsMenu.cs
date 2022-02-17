using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class OptionsMenu : MonoBehaviour
{

    [SerializeField]
    private Slider _linesSlider;
    [SerializeField]
    private Slider _columnsSlider;
    [SerializeField]
    private Slider _speedSlider;

    void Start()
    {
        if (PlayerPrefs.HasKey("nbrLines")) { _linesSlider.value = PlayerPrefs.GetInt("nbrLines"); }
        if (PlayerPrefs.HasKey("nbrColumns")) { _columnsSlider.value = PlayerPrefs.GetInt("nbrColumns"); }
        if (PlayerPrefs.HasKey("speed")) { _speedSlider.value = PlayerPrefs.GetInt("speed"); }
    }

    public void SetLines(float nbrLines)
    {
        PlayerPrefs.SetInt("nbrLines", (int)nbrLines);
        PlayerPrefs.Save();
    }

    public void SetColumns(float nbrColumns)
    {
        PlayerPrefs.SetInt("nbrColumns", (int)nbrColumns);
        PlayerPrefs.Save();
    }

    public void SetSpeed(float speed)
    {
        PlayerPrefs.SetInt("speed", (int)speed);
        GridManager.Instance.SetSpeed(speed);
        PlayerPrefs.Save();
    }

}
