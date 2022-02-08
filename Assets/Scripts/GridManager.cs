using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance;

    #region Variables
    [Header("Grid Parameters")]

    private GameObject[,] grid;
    [Range(1, 50)]
    public int nbrColumns = 10;
    [Range(1, 50)]
    public int nbrLines = 10;
    //[Range(0, 0.5f)]
    //private float offset = 0;

    [Header("Prefabs")]

    [SerializeField]
    private GameObject alivePrefab;
    [SerializeField]
    private GameObject deadPrefab;

    #endregion

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            CreateGrid();
        }
        else { Destroy(gameObject); }

    }

    void CreateGrid()
    {
        grid = new GameObject[nbrLines, nbrColumns];

        for (int i = 0; i < nbrLines; i++)
        {
            for (int j = 0; j < nbrColumns; j++)
            {
                //GameObject clone = Instantiate(deadPrefab, new Vector3(j + j * offset, i + i * offset, 0), new Quaternion());
                GameObject clone = Instantiate(deadPrefab, new Vector3(j, i, 0), new Quaternion());
                grid[i, j] = clone;
            }
        }
    }
}
