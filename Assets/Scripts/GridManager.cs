using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using UnityEngine;
using System.Text;

public struct Cell
{
    public int x;
    public int y;

    public MeshRenderer mesh;

    public bool isAlive;

    public Cell(int pX, int pY, MeshRenderer pMesh, bool pIsAlive)
    {
        x = pX;
        y = pY;
        mesh = pMesh;
        isAlive = pIsAlive;
    }

    public string ToStringCell()
    {
        return "Position : " + y + "/" + x + " /// Is Alive : " + isAlive;
    }

    public JsonCell toJsonCell()
    {
        JsonCell cell = new JsonCell();
        cell.x = x;
        cell.y = y;
        cell.isAlive = isAlive;
        return cell;
    }
}

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
    private int _stepPerSeconds = 2;

    [Header("Prefabs")]

    [SerializeField]
    private GameObject _cellPrefab;
    [SerializeField]
    private Material _aliveMaterial;
    [SerializeField]
    private Material _deadMaterial;
    #endregion

    #region Private
    private float counter = 0;

    private Cell[] _lstCells;

    private bool _running = false;
    #endregion

    #region Init
    private void Start()
    {
        Init();
    }

    private void Init()
    {
        LoadSettings();
        CreateGrid();
        CameraManager.Instance.CameraInit();

        SaveToJson("OUI");
    }

    private void LoadSettings()
    {
        if (PlayerPrefs.HasKey("nbrLines")) { _nbrLines = PlayerPrefs.GetInt("nbrLines"); }
        if (PlayerPrefs.HasKey("nbrColumns")) { _nbrColumns = PlayerPrefs.GetInt("nbrColumns"); }
        if (PlayerPrefs.HasKey("speed")) { _stepPerSeconds = 1/PlayerPrefs.GetInt("speed"); }
    }

    void CreateGrid()
    {
        _lstCells = new Cell[_nbrLines * _nbrColumns];

        for (int y = 0; y < _nbrLines; y++)
        {
            for (int x = 0; x < _nbrColumns; x++)
            {
                GameObject gameObject = Instantiate(_cellPrefab, new Vector3(x, y, 0), new Quaternion(), transform);
                Cell cell = new Cell(x, y, gameObject.GetComponentInChildren<MeshRenderer>(), false);
                _lstCells[y * _nbrColumns + x] = cell;
            }
        }
    }
    #endregion

    private void Update()
    {
        if (!_running) { return; }

        counter += Time.deltaTime;

        if (!(counter >= _stepPerSeconds)) { return; }
        counter = 0;

        SimulationStep();
    }

    #region Public functions
    public void StartGame()
    {
        _running = true;
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

    public void SaveButton()
    {
        SaveToJson("Last");
    }
    #endregion

    #region Cells
    private void changeTile(Vector2 tileCoord)
    {
        int index =  Mathf.FloorToInt(tileCoord.y) * _nbrColumns + Mathf.FloorToInt(tileCoord.x);

        _lstCells[index].mesh.sharedMaterial = (_lstCells[index].isAlive) ? _deadMaterial : _aliveMaterial;
        _lstCells[index].isAlive = !_lstCells[index].isAlive;
    }
    private void changeTile(int index)
    {
        _lstCells[index].mesh.sharedMaterial = (_lstCells[index].isAlive) ? _deadMaterial : _aliveMaterial;
        _lstCells[index].isAlive = !_lstCells[index].isAlive;
    }

    private async void SimulationStep()
    {
        List<Task<int>> tasks = new List<Task<int>>();
        for (int i = 0; i < _lstCells.Length; i++)
        {
            var index = i;
            tasks.Add(Task.Run(() => UpdateCell(index)));
        }
        List<int> lstIndexToModify = new List<int>();
        while (tasks.Count > 0)
        {
            var finishedTask = await Task.WhenAny(tasks);

            if (finishedTask.Result != -1)
            {
                lstIndexToModify.Add(finishedTask.Result);
            }

            tasks.Remove(finishedTask);
        }

        foreach (int index in lstIndexToModify)
        {
            changeTile(index);
        }
    }

    private int UpdateCell(int index)
    {
        int neighborsAlive = GetNbrNeighborsAlive(index);
        if (!_lstCells[index].isAlive && neighborsAlive == 3)
        {
            return index;
        } 
        else if (_lstCells[index].isAlive && (neighborsAlive < 2 || neighborsAlive > 3))
        {
            return index;
        }

        return -1;
    }

    private int GetNbrNeighborsAlive(int index)
    {
        int neighborsAlive = 0;
        for (int i = _lstCells[index].y - 1; i <= _lstCells[index].y + 1; i++)
        {
            for (int j = _lstCells[index].x - 1; j <= _lstCells[index].x + 1; j++)
            {
                if (!(_lstCells[index].y == i && _lstCells[index].x == j))
                {
                    if (!(i < 0 || i >= _nbrLines || j < 0 || j >= _nbrColumns))
                    {
                        if (_lstCells[i * _nbrColumns + j].isAlive)
                        {
                            neighborsAlive++;
                        }
                    }
                }
            }
        }
        return neighborsAlive;
    }
    #endregion

    #region Json
    private string getJsonMap(string name)
    {
        JsonMap map = new JsonMap();
        map.name = name;
        map.nbrColumns = _nbrColumns;
        map.nbrLines = _nbrLines;
        map.lstCell = new JsonCell[_nbrColumns * _nbrColumns];
        for (int i = 0; i < _lstCells.Length; i++)
        {
            map.lstCell[i] = _lstCells[i].toJsonCell();
        }

        return JsonUtility.ToJson(map);
    }

    private async Task SaveToJson(string name)
    {
        string json = getJsonMap(name);
        string filePath = Application.persistentDataPath + "/Maps";

        byte[] encodedText = Encoding.Unicode.GetBytes(json);

        DirectoryInfo info = new DirectoryInfo(filePath);
        if (!info.Exists)
        {
            info.Create();
        }

        string path = Path.Combine(filePath, $"{name}.json");

        using (FileStream sourceStream = new FileStream(path,
            FileMode.Append, FileAccess.Write, FileShare.None,
            bufferSize: 4096, useAsync: true))
        {
            await sourceStream.WriteAsync(encodedText, 0, encodedText.Length);
        };
    }
    #endregion
}
