using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using UnityEngine;
using System.Text;
using System.Threading;
using System;
using TMPro;

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

public enum GridMode
{
    None,
    Continue
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

    [Header("Mode")]

    [SerializeField]
    private GridMode _gridMode;

    [Header("Mode")]

    [SerializeField]
    TMP_InputField inputField;

    #endregion

    #region Private
    private float counter = 0;

    private Cell[] _lstCells;

    private bool _running = false;
    private bool _lastIsAlive = false;
    #endregion

    #region Init

    public void Init()
    {
        _running = false;
        LoadSettings();
        CreateGrid();
        CameraManager.Instance.CameraInit();
    }

    public void Init(JsonMap loadedMap)
    {
        _nbrLines = loadedMap.nbrLines;
        _nbrColumns = loadedMap.nbrColumns;
        PlayerPrefs.SetInt("nbrLines", _nbrLines);
        PlayerPrefs.SetInt("nbrColumns", _nbrColumns);
        PlayerPrefs.Save();
        CreateGrid(loadedMap.lstCell);
        CameraManager.Instance.CameraInit();
    }

    private void LoadSettings()
    {
        if (PlayerPrefs.HasKey("nbrLines")) { _nbrLines = PlayerPrefs.GetInt("nbrLines"); }
        if (PlayerPrefs.HasKey("nbrColumns")) { _nbrColumns = PlayerPrefs.GetInt("nbrColumns"); }
        if (PlayerPrefs.HasKey("speed")) { _stepPerSeconds = PlayerPrefs.GetInt("speed"); }
    }

    void CreateGrid()
    {
        UnloadGrid();
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

    void CreateGrid(JsonCell[] lstCell)
    {
        UnloadGrid();
        _lstCells = new Cell[_nbrLines * _nbrColumns];

        for (int y = 0; y < _nbrLines; y++)
        {
            for (int x = 0; x < _nbrColumns; x++)
            {
                GameObject gameObject = Instantiate(_cellPrefab, new Vector3(x, y, 0), new Quaternion(), transform);
                Debug.Log(lstCell[y * _nbrColumns + x].isAlive);
                Cell cell = new Cell(x, y, gameObject.GetComponentInChildren<MeshRenderer>(), lstCell[y * _nbrColumns + x].isAlive);

                if (cell.isAlive)
                {
                    cell.mesh.sharedMaterial = _aliveMaterial;
                }

                _lstCells[y * _nbrColumns + x] = cell;
            }
        }
    }

    void UnloadGrid()
    {
        if (_lstCells == null) { return; }

        for (int i = 0; i < _lstCells.Length; i++)
        {
            Destroy(_lstCells[i].mesh.gameObject);
        }
    }
    #endregion

    private void Update()
    {
        if (!_running) { return; }

        counter += Time.deltaTime;

        if (!(counter >= 1 / (float)_stepPerSeconds)) { return; }
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

        _lastIsAlive =  changeTile(mouseCoords);
    }
    public void OnClickStay()
    {
        if (_running) { return; }

        Vector2 mouseCoords = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // Check if click is inside
        if (!(mouseCoords.x >= 0 && mouseCoords.x < _nbrColumns &&
              mouseCoords.y >= 0 && mouseCoords.y < _nbrLines)) { return; }

        setTile(mouseCoords, _lastIsAlive);
    }

    public async void SaveButton()
    {
        await SaveToJson(inputField.text);
    }

    public async void LoadButton()
    {
        var data = await LoadJson(MainMenu.Instance.selectedFilePath);
        JsonMap loadedMap = JsonUtility.FromJson<JsonMap>(data);
        Init(loadedMap);
    }
    #endregion

    #region Cells
    private bool changeTile(Vector2 tileCoord)
    {
        int index =  Mathf.FloorToInt(tileCoord.y) * _nbrColumns + Mathf.FloorToInt(tileCoord.x);

        _lstCells[index].mesh.sharedMaterial = (_lstCells[index].isAlive) ? _deadMaterial : _aliveMaterial;
        _lstCells[index].isAlive = !_lstCells[index].isAlive;
        return _lstCells[index].isAlive;
    }
    private void changeTile(int index)
    {
        _lstCells[index].mesh.sharedMaterial = (_lstCells[index].isAlive) ? _deadMaterial : _aliveMaterial;
        _lstCells[index].isAlive = !_lstCells[index].isAlive;
    }
    private void setTile(Vector2 tileCoord, bool isAlive)
    {
        int index = Mathf.FloorToInt(tileCoord.y) * _nbrColumns + Mathf.FloorToInt(tileCoord.x);

        if (_lstCells[index].isAlive == isAlive) { return; }

        _lstCells[index].mesh.sharedMaterial = isAlive ? _aliveMaterial : _deadMaterial;
        _lstCells[index].isAlive = isAlive;
    }

    private async void SimulationStep()
    {
        List<Task<List<int>>> tasks = new List<Task<List<int>>>();
        List<int> lstIndexToModify = new List<int>();
        for (int i = 0; i < Environment.ProcessorCount; i++)
        {
            var index = i;
            tasks.Add(Task.Run(() => getTaskCell(_lstCells.Length * index / Environment.ProcessorCount, _lstCells.Length * (index + 1) / Environment.ProcessorCount)));
        }
        while (tasks.Count > 0)
        {
            var finishedTask = await Task.WhenAny(tasks);

            lstIndexToModify.AddRange(finishedTask.Result);
            
            tasks.Remove(finishedTask);
        }

        foreach (int index in lstIndexToModify)
        {
            changeTile(index);
        }
    }

    private List<int> getTaskCell(int from, int to)
    {
        Debug.Log($"{from} / {to}");
        List<int> lstIndexToModify = new List<int>();
        for (int i = from; i < to; i++)
        {
            int index = UpdateCell(i);
            if (index != -1)
                lstIndexToModify.Add(index);
        }

        return lstIndexToModify;
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
                    neighborsAlive += isCellAlive(j, i) ? 1 : 0;
                }
            }
        }
        return neighborsAlive;
    }

    private bool isCellAlive(int x, int y)
    {
        switch (_gridMode)
        {
            case GridMode.None:
                return isCellAliveNoneMode(x, y);
            case GridMode.Continue:
                return isCellAliveContinueMode(x, y);
            default:
                break;
        }


        return false;
    }

    private bool isCellAliveNoneMode(int x, int y)
    {
        if (y < 0 || y >= _nbrLines || x < 0 || x >= _nbrColumns) { return false; }
        if (!_lstCells[y * _nbrColumns + x].isAlive) { return false; }
        return true;
    }

    private bool isCellAliveContinueMode(int x, int y)
    {
        if (y < 0) { y += _nbrLines; }
        if (y >= _nbrLines) { y -= _nbrLines; }
        if (x < 0) { x += _nbrColumns; }
        if (x >= _nbrLines) { x -= _nbrColumns; }

        if (!_lstCells[y * _nbrColumns + x].isAlive) { return false; }
        return true;
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

        byte[] encodedText = Encoding.UTF8.GetBytes(json);

        DirectoryInfo info = new DirectoryInfo(filePath);
        if (!info.Exists)
        {
            info.Create();
        }

        string path = Path.Combine(filePath, $"{name}.json");

        //CHECK IF FILE ALREADY EXIST

        using (FileStream sourceStream = new FileStream(path,
            FileMode.Create, FileAccess.Write, FileShare.Write,
            bufferSize: 4096, useAsync: true))
        {
            await sourceStream.WriteAsync(encodedText, 0, encodedText.Length);
        };
    }

    public async Task<string> LoadJson(string name)
    {
        string filePath = Application.persistentDataPath + "/Maps";
        string path = Path.Combine(filePath, name);
        using var sourceStream = new FileStream(
            path,
            FileMode.Open, FileAccess.Read, FileShare.Read,
            bufferSize: 4096, useAsync: true);
        var sb = new StringBuilder();

        byte[] buffer = new byte[0x1000];
        int numRead;
        while ((numRead = await sourceStream.ReadAsync(buffer, 0, buffer.Length)) != 0)
        {
            string text = Encoding.UTF8.GetString(buffer, 0, numRead);
            sb.Append(text);
        }

        return sb.ToString();
    }
    #endregion
}
