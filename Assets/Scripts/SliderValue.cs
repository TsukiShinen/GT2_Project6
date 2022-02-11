using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SliderValue : MonoBehaviour
{

    TMP_Text percentageText;

    void Start()
    {
        percentageText = GetComponent<TMP_Text>();
    }

    public void TextUpdate(float value)
    {
        percentageText.text = value+""; 
    }
}
