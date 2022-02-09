using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    #region Singleton
    public static GridManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
            Init();
        }
        else
        {
            Destroy(gameObject);
        }

    }
    #endregion

    #region Show in Editor
    [Header("Grid Parameters")]
    [SerializeField]
    [Range(1, 50)]
    private int _nbrColumns = 10;
    [SerializeField]
    [Range(1, 50)]
    private int _nbrLines = 10;

    [Header("Prefabs")]

    [SerializeField]
    private GameObject _alivePrefab;
    [SerializeField]
    private GameObject _deadPrefab;
    #endregion

    #region Private
    private GameObject[,] _grid;
    #endregion

    private void Init()
    {
        CreateGrid();
    }

    void CreateGrid()
    {
        _grid = new GameObject[_nbrLines, _nbrColumns];

        for (int i = 0; i < _nbrLines; i++)
        {
            for (int j = 0; j < _nbrColumns; j++)
            {
                _grid[i, j] = Instantiate(_deadPrefab, new Vector3(j, i, 0), new Quaternion(), transform);
            }
        }
    }

    public void OnClick()
    {
        Vector2 mouseCoords = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // Check if click is inside
        if (!(mouseCoords.x >= 0 && mouseCoords.x < _nbrColumns &&
              mouseCoords.y >= 0 && mouseCoords.y < _nbrLines)) { return; }

        changeTile(mouseCoords);
    }

    private void changeTile(Vector2 tileCoord)
    {
        GameObject tile = _grid[Mathf.FloorToInt(tileCoord.y), Mathf.FloorToInt(tileCoord.x)];
        GameObject prefabUsed = (tile.tag == _deadPrefab.tag) ? _alivePrefab : _deadPrefab;

        tile.GetComponentInChildren<MeshRenderer>().sharedMaterial = prefabUsed.GetComponentInChildren<MeshRenderer>().sharedMaterial;
        tile.tag = prefabUsed.tag;
    }
 }
