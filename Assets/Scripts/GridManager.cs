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
                GameObject clone = Instantiate(_deadPrefab, new Vector3(j, i, 0), new Quaternion(), transform);
                _grid[i, j] = clone;
            }
        }
    }

    public void OnClick()
    {
        Vector2 mouseCoords = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 tileCoord = convertToGrid(mouseCoords);

        // Verify Coord
        if (!(tileCoord.x >= 0 && tileCoord.x < _nbrColumns &&
              tileCoord.y >= 0 && tileCoord.y < _nbrLines)) { return; }

        changeTile(tileCoord);
    }


    private Vector2 convertToGrid(Vector2 mousePos)
    {
        return new Vector2(mousePos.x / GridManager.Instance._nbrLines * 10,
                           mousePos.y / GridManager.Instance._nbrColumns * 10);
    }
    private void changeTile(Vector2 tileCoord)
    {
        GameObject tile = _grid[(int)tileCoord.y, (int)tileCoord.x];

        if (tile.tag.ToLower() == "dead")
        {
            tile.GetComponentInChildren<MeshRenderer>().sharedMaterial = _alivePrefab.GetComponentInChildren<MeshRenderer>().sharedMaterial;
            tile.tag = "Alive";
        } 
        else
        {
            tile.GetComponentInChildren<MeshRenderer>().sharedMaterial = _deadPrefab.GetComponentInChildren<MeshRenderer>().sharedMaterial;
            tile.tag = "Dead";
        }
    }
 }
