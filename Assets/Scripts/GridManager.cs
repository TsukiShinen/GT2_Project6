using System;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public struct Cell
{
    public int x;
    public int y;

    public bool isAlive;

    public Cell(int pX, int pY, bool pIsAlive)
    {
        x = pX;
        y = pY;
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
        [Range(1, 400)]
        private int _nbrColumns = 10;
        [SerializeField]
        [Range(1, 400)]
        private int _nbrLines = 10;
        [SerializeField]
        private int _stepPerSeconds = 2;

        [Header("Prefabs")]

        [SerializeField]
        private MeshFilter _meshFilter;
        [SerializeField]
        private MeshRenderer _meshRenderer;

        [Header("Mode")]

        [SerializeField]
        private GridMode _gridMode;

        [Header("InputField")]

        [SerializeField]
        private TMP_InputField _inputField;

    #endregion

    #region Private

        private float _counter = 0;

        private Cell[] _lstCells;
        private Color[] _lstCellsColor;

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
            _running = false;
            LoadSettings(loadedMap);
            CreateGrid(loadedMap.lstCell);
            CameraManager.Instance.CameraInit();
        }
        public void Init(Texture2D texture)
        {
            _running = false;
            LoadSettings(texture);
            CreateGrid(texture);
            CameraManager.Instance.CameraInit();
        }

        private void LoadSettings()
        {
            // Check if PlayersPrefs already exist, if not they are created

            if (PlayerPrefs.HasKey("nbrLines")) { _nbrLines = PlayerPrefs.GetInt("nbrLines"); }
            else { PlayerPrefs.SetInt("nbrLines", 10); }

            if (PlayerPrefs.HasKey("nbrColumns")) { _nbrColumns = PlayerPrefs.GetInt("nbrColumns"); }
            else { PlayerPrefs.SetInt("nbrColumns", 10); }

            if (PlayerPrefs.HasKey("speed")) { _stepPerSeconds = PlayerPrefs.GetInt("speed"); }
            else { PlayerPrefs.SetInt("speed", 2); }

            PlayerPrefs.Save();
        }
        private void LoadSettings(JsonMap loadedMap)
        {
            _nbrLines = loadedMap.nbrLines;
            _nbrColumns = loadedMap.nbrColumns;
            PlayerPrefs.SetInt("nbrLines", _nbrLines);
            PlayerPrefs.SetInt("nbrColumns", _nbrColumns);
            PlayerPrefs.Save();
        }
        private void LoadSettings(Texture2D texture)
        {
            _nbrLines = texture.height;
            _nbrColumns = texture.width;
            PlayerPrefs.SetInt("nbrLines", _nbrLines);
            PlayerPrefs.SetInt("nbrColumns", _nbrColumns);
            PlayerPrefs.Save();
        }

        void CreateGrid()
        {
            UnloadGrid();

            DrawMesh();
            _lstCells = new Cell[_nbrLines * _nbrColumns];

            _lstCellsColor = new Color[_nbrLines * _nbrColumns];
            for (int y = 0; y < _nbrLines; y++)
            {
                for (int x = 0; x < _nbrColumns; x++)
                {
                    Cell cell = new Cell(x, y, false);
                    _lstCells[y * _nbrColumns + x] = cell;
                    _lstCellsColor[y * _nbrColumns + x] = Color.black;
                }
            }

            RechargeTexture();
        }
        void CreateGrid(JsonCell[] lstCell)
        {
            UnloadGrid();

            DrawMesh();
            _lstCells = new Cell[_nbrLines * _nbrColumns];

            _lstCellsColor = new Color[_nbrLines * _nbrColumns];
            for (int y = 0; y < _nbrLines; y++)
            {
                for (int x = 0; x < _nbrColumns; x++)
                {
                    Cell cell = new Cell(x, y, lstCell[y * _nbrColumns + x].isAlive);
                    _lstCells[y * _nbrColumns + x] = cell;
                    _lstCellsColor[y * _nbrColumns + x] = lstCell[y * _nbrColumns + x].isAlive ? Color.white : Color.black;
                }
            }

            RechargeTexture();
        }
        void CreateGrid(Texture2D texture)
        {
            DrawMesh();
            _lstCells = new Cell[_nbrLines * _nbrColumns];
            _lstCellsColor = new Color[_nbrLines * _nbrColumns];
            _lstCellsColor = texture.GetPixels();
            for (int y = 0; y < _nbrLines; y++)
            {
                for (int x = 0; x < _nbrColumns; x++)
                {
                    bool alive = _lstCellsColor[y * _nbrColumns + x] == Color.white;
                    Cell cell = new Cell(x, y, alive);
                    _lstCells[y * _nbrColumns + x] = cell;
                }
            }
            texture.filterMode = FilterMode.Point;
            texture.wrapMode = TextureWrapMode.Clamp;
            _meshRenderer.sharedMaterial.mainTexture = texture;
        }

        void RechargeTexture()
        {
            Texture2D texture = new Texture2D(_nbrColumns, _nbrLines);
            texture.filterMode = FilterMode.Point;
            texture.wrapMode = TextureWrapMode.Clamp;
            texture.SetPixels(_lstCellsColor);
            texture.Apply();
            Destroy(_meshRenderer.sharedMaterial.mainTexture);
            _meshRenderer.sharedMaterial.mainTexture = texture;
        }

        void UnloadGrid()
        {
            // NEW DESTROY TO DO
        }

    #endregion

    private void Update()
    {
        if (!_running) { return; }

        _counter += Time.deltaTime;

        if (!(_counter >= 1 / (float)_stepPerSeconds)) { return; }
        _counter = 0;

        SimulationStep();
    }

    private void DrawMesh()
    {
        _meshFilter.sharedMesh = MeshGenerator.GenerateMesh(_nbrColumns + 1, _nbrLines + 1).CreateMesh();
    }

    #region Public functions

        public void PlayButton()
        {
            _running = true;
        }

        public void PauseButton()
        {
            _running = false;
        }

        public void StepByStep()
        {
            SimulationStep();
        }

        public void OnClick()
        {
            if (_running) { return; }

            Vector2 mouseCoords = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // Check if click is inside
            if (!(mouseCoords.x >= 0 && mouseCoords.x < _nbrColumns &&
                  mouseCoords.y >= 0 && mouseCoords.y < _nbrLines)) { return; }

            _lastIsAlive =  ChangeCellAt(mouseCoords);
            RechargeTexture();
        }

        public void OnClickHold()
        {
            if (_running) { return; }

            Vector2 mouseCoords = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // Check if click is inside
            if (!(mouseCoords.x >= 0 && mouseCoords.x < _nbrColumns &&
                  mouseCoords.y >= 0 && mouseCoords.y < _nbrLines)) { return; }

            SetCellAt(mouseCoords, _lastIsAlive);
            RechargeTexture();
        }

        public async void SaveButtonToJson()
        {
            await JsonLoader.SaveMapToJson(_inputField.text);
        }

        public async void SaveButtonToPNG()
        {
            await PNGLoader.SaveMapToPNG(_inputField.text);
        }

        public async void LoadButton()
        {
            switch(Path.GetExtension(MainMenu.Instance.m_selectedFilePath))
            {
                case ".json":
                    var data = await JsonLoader.LoadMapFromJson(MainMenu.Instance.m_selectedFilePath);
                    JsonMap loadedMap = JsonUtility.FromJson<JsonMap>(data);
                    Init(loadedMap);
                    break;
                case ".png":
                    Texture2D texture = await PNGLoader.LoadMapFromPNG(MainMenu.Instance.m_selectedFilePath);
                    Init(texture);
                    break;
                default:
                    Init();
                    break;
            }
        }

        public Texture2D GetTexture()
        {
            Texture2D texture = new Texture2D(_nbrColumns, _nbrLines);
            texture.filterMode = FilterMode.Point;
            texture.wrapMode = TextureWrapMode.Clamp;
            texture.SetPixels(_lstCellsColor);
            texture.Apply();
            return texture;
        }

        public void SetSpeed(float speed)
        {
            _stepPerSeconds = (int)speed;
        }

    #endregion

    #region Cells

        private async void SimulationStep()
        {
            // Start the check to update on cells (distributed on each cores)
            List<Task<List<int>>> cellUpdate = new List<Task<List<int>>>();
            for (int i = 0; i < Environment.ProcessorCount; i++)
            {
                var index = i;
                cellUpdate.Add(Task.Run(() => GetTaskCell(_lstCells.Length * index / Environment.ProcessorCount, _lstCells.Length * (index + 1) / Environment.ProcessorCount)));
            }

            // Get the cell who need update
            List<int> lstCellToUpdate = new List<int>();
            while (cellUpdate.Count > 0)
            {
                var finishedTask = await Task.WhenAny(cellUpdate);

                lstCellToUpdate.AddRange(finishedTask.Result);
            
                cellUpdate.Remove(finishedTask);
            }

            // Change the cells
            foreach (int index in lstCellToUpdate)
            {
                ChangeCellAt(index);
            }
            RechargeTexture();
        }

        private List<int> GetTaskCell(int from, int to)
        {
            List<int> lstIndexToUpdate = new List<int>();
            for (int index = from; index < to; index++)
            {
            
                if (NeedUpdate(index))
                    lstIndexToUpdate.Add(index);
            }

            return lstIndexToUpdate;
        }

        private bool NeedUpdate(int index) // Return true if a cell needs to be updated
    {
            // Get Neightbor
            int neighborsAlive = GetNbrNeighborsAlive(index);

            // Check the rule if need an update
            if ((!_lstCells[index].isAlive && neighborsAlive == 3) ||
                (_lstCells[index].isAlive && (neighborsAlive < 2 || neighborsAlive > 3)))
            {
                return true;
            }

            // Return none
            return false;
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
                        neighborsAlive += IsCellAlive(j, i) ? 1 : 0;
                    }
                }
            }
            return neighborsAlive;
        }

        private bool IsCellAlive(int x, int y)
        {
            switch (_gridMode)
            {
                case GridMode.None:
                    return IsCellAliveNoneMode(x, y);
                case GridMode.Continue:
                    return IsCellAliveContinueMode(x, y);
                default:
                    break;
            }


            return false;
        }

        private bool IsCellAliveNoneMode(int x, int y)
        {
            if (y < 0 || y >= _nbrLines || x < 0 || x >= _nbrColumns) { return false; }
            if (!_lstCells[y * _nbrColumns + x].isAlive) { return false; }
            return true;
        }

        private bool IsCellAliveContinueMode(int x, int y)
        {
            if (y < 0) { y += _nbrLines; }
            if (y >= _nbrLines) { y -= _nbrLines; }
            if (x < 0) { x += _nbrColumns; }
            if (x >= _nbrLines) { x -= _nbrColumns; }

            if (!_lstCells[y * _nbrColumns + x].isAlive) { return false; }
            return true;
        }

        private bool ChangeCellAt(Vector2 tileCoord)
        {
            int index = Mathf.FloorToInt(tileCoord.y) * _nbrColumns + Mathf.FloorToInt(tileCoord.x);

            _lstCells[index].isAlive = !_lstCells[index].isAlive;
            _lstCellsColor[index] = _lstCells[index].isAlive ? Color.white : Color.black;

            return _lstCells[index].isAlive;
        }
        private void ChangeCellAt(int index)
        {
            _lstCells[index].isAlive = !_lstCells[index].isAlive;
            _lstCellsColor[index] = _lstCells[index].isAlive ? Color.white : Color.black;
        }

        private void SetCellAt(Vector2 tileCoord, bool isAlive)
        {
            int index = Mathf.FloorToInt(tileCoord.y) * _nbrColumns + Mathf.FloorToInt(tileCoord.x);

            if (_lstCells[index].isAlive == isAlive) { return; }

            _lstCellsColor[index] = isAlive ? Color.white : Color.black;
            _lstCells[index].isAlive = isAlive;
        }

    #endregion

    #region Json

        public JsonMap GetJsonMap(string name)
    {
        JsonMap map = new JsonMap();
        map.name = name;
        map.nbrColumns = _nbrColumns;
        map.nbrLines = _nbrLines;
        map.lstCell = new JsonCell[_nbrLines * _nbrColumns];
        for (int i = 0; i < _lstCells.Length; i++)
        {
            map.lstCell[i] = _lstCells[i].toJsonCell();
        }
        
        return map;
    }

    #endregion
}
