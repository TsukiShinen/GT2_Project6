using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    #region Variables
    [Header("Grid Parameters")]

    [Range(1,50)]
    public int nbrColumns = 10;
    [Range(1, 50)]
    public int nbrLines = 10;
    [Range(0, 0.5f)]
    public float offset = 0;

    [Header("Prefabs")]

    [SerializeField]
    private GameObject alivePrefab;
    [SerializeField]
    private GameObject deadPrefab;
    #endregion

    private void Awake()
    {
        for (int i=0; i<nbrColumns;i++)
        {
            for(int j=0; j<nbrLines;j++)
            {
                Instantiate(deadPrefab,new Vector3(i+i*offset,j+j*offset,0),new Quaternion());
            }
        }
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
