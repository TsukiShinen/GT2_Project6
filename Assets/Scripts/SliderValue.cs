using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SliderValue : MonoBehaviour
{
    [SerializeField]
    private TMP_Text _text;

    void Awake()
    {
        _text = GetComponent<TMP_Text>();
    }

    public void TextUpdate(float value)
    {
        _text.text = value.ToString(); 
    }
}
