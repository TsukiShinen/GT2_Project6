using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;

public class MainMenu : MonoBehaviour
{
    #region Singleton

        public static MainMenu Instance;

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

    #region Show in editor

        [SerializeField]
        private TMP_Dropdown _filesDropdown;
        private List<string> _filesList = new List<string>();
        public string m_selectedFilePath;

    #endregion

    public void LoadDropdown()
    {
        _filesList.Clear();
        _filesDropdown.ClearOptions();
        _filesDropdown.RefreshShownValue();

        string path = Application.persistentDataPath;
        string[] jsonFilesPaths = Directory.GetFiles(@path, "*.json");
        string[] pngFilesPaths = Directory.GetFiles(@path, "*.png");

        if(jsonFilesPaths.Length == 0 && pngFilesPaths.Length == 0) { return; }

        foreach (string file in jsonFilesPaths)
        {
            _filesList.Add(Path.GetFileName(file));
        }
        foreach (string file in pngFilesPaths)
        {
            _filesList.Add(Path.GetFileName(file));
        }

        m_selectedFilePath = _filesList[_filesDropdown.value];

        _filesDropdown.AddOptions(_filesList);
        _filesDropdown.RefreshShownValue();
    }

    public void DropdownSelect(int index)
    {
        string filePath = Application.persistentDataPath + "/";
        m_selectedFilePath = Path.Combine(filePath,_filesDropdown.options[_filesDropdown.value].text);
    }

    public void QuitButton()
    {
        Application.Quit();
    }
}
