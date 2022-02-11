using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionsMenu : MonoBehaviour
{

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
        PlayerPrefs.Save();
    }

}
