using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionsMenu : MonoBehaviour
{

    public void setLines(float nbrLines)
    {
        PlayerPrefs.SetInt("nbrLines", (int)nbrLines);
        PlayerPrefs.Save();

    }

    public void setColumns(float nbrColumns)
    {
        PlayerPrefs.SetInt("nbrColumns", (int)nbrColumns);
        PlayerPrefs.Save();
    }

}
