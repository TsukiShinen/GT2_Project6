using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SliderValue : MonoBehaviour
{

    TMP_Text text;

    void Awake()
    {
        text = GetComponent<TMP_Text>();
    }

    public void TextUpdate(float value)
    {
        text.text = value.ToString(); 
    }
}
