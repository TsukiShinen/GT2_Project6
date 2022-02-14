using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class JsonMap
{
    public string name;
    public int nbrLines;
    public int nbrColumns;
    public JsonCell[] lstCell;
}

[Serializable]
public class JsonCell
{
    public int x;
    public int y;
    public bool isAlive;
}
