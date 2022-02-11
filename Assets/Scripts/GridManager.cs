using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
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
    [SerializeField]
    private int _speed = 2;

    [Header("Prefabs")]

    [SerializeField]
    private GameObject _cellPrefab;
    [SerializeField]
    private Material _aliveMaterial;
    [SerializeField]
    private Material _deadMaterial;
    #endregion

    #region Private
    private GameObject[,] _grid;

    private bool _running = false;
    #endregion

    private void Start()
    {
        if (PlayerPrefs.HasKey("nbrLines")) { _nbrLines = PlayerPrefs.GetInt("nbrLines"); }
        if (PlayerPrefs.HasKey("nbrColumns")) { _nbrColumns = PlayerPrefs.GetInt("nbrColumns"); }
        if (PlayerPrefs.HasKey("speed")) { _speed = PlayerPrefs.GetInt("speed"); }
        Init();
    }

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
                _grid[i, j] = Instantiate(_cellPrefab, new Vector3(j, i, 0), new Quaternion(), transform);
            }
        }
    }

    public void OnClick()
    {
        if (_running) { return; }

        Vector2 mouseCoords = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // Check if click is inside
        if (!(mouseCoords.x >= 0 && mouseCoords.x < _nbrColumns &&
              mouseCoords.y >= 0 && mouseCoords.y < _nbrLines)) { return; }

        changeTile(mouseCoords);
    }

    private void changeTile(Vector2 tileCoord)
    {
        GameObject cell = _grid[Mathf.FloorToInt(tileCoord.y), Mathf.FloorToInt(tileCoord.x)];
        Material material = (cell.tag == "Dead") ? _aliveMaterial : _deadMaterial;
        string tag = (cell.tag == "Dead") ? "Alive" : "Dead";

        cell.GetComponentInChildren<MeshRenderer>().sharedMaterial = material;
        cell.tag = tag;
    }

    public void StartGame()
    {
        _running = true;

        Invoke("UpdateGame", 1/_speed);
    }

    private void UpdateGame()
    {
        if (!_running) { return; }

        int[,] changeGrid = new int[_nbrLines, _nbrColumns];
        for (int i = 0; i < _nbrLines; i++)
        {
            for (int j = 0; j < _nbrColumns; j++)
            {
                changeGrid[i, j] = UpdateCell(new Vector2(j,i));
            }
        }

        for (int i = 0; i < _nbrLines; i++)
        {
            for (int j = 0; j < _nbrColumns; j++)
            {
                GameObject cell = _grid[Mathf.FloorToInt(i), Mathf.FloorToInt(j)];

                if (changeGrid[i,j] == 0)
                {
                    cell.GetComponentInChildren<MeshRenderer>().sharedMaterial = _deadMaterial;
                    cell.tag = "Dead";
                }

                if (changeGrid[i, j] == 1)
                {
                    cell.GetComponentInChildren<MeshRenderer>().sharedMaterial = _aliveMaterial;
                    cell.tag = "Alive";
                }
            }
        }

        Invoke("UpdateGame", 1/_speed);
    }

    private int UpdateCell(Vector2 pPosition)
    {
        int neighborsAlive = GetNbrNeighborsAlive(pPosition);
        GameObject cell = _grid[Mathf.FloorToInt(pPosition.y), Mathf.FloorToInt(pPosition.x)];

        if (cell.tag == "Dead" && neighborsAlive == 3)
        {
            return 1;
        } 
        else if (cell.tag == "Alive" && (neighborsAlive < 2 || neighborsAlive > 3))
        {
            return 0;
        }

        return -1;
    }

    private int GetNbrNeighborsAlive(Vector2 pPosition)
    {
        int neighborsAlive = 0;

        for (int i = (int)(pPosition.y - 1); i <= (int)(pPosition.y + 1); i++)
        {
            for (int j = (int)(pPosition.x - 1); j <= (int)(pPosition.x + 1); j++)
            {
                if (!(pPosition.y == i && pPosition.x == j))
                {
                    if (!(i < 0 || i >= _nbrLines || j < 0 || j >= _nbrColumns)) 
                    {
                        if (_grid[i, j].tag == "Alive") 
                        {
                            neighborsAlive++;
                        }
                    }
                }
            }
        }
        return neighborsAlive;
    }
}
