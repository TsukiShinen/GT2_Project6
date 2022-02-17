using System.Collections;
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
    private TMP_Dropdown filesDropdown;
    private List<string> filesList = new List<string>();
    public string selectedFilePath;
    #endregion

    public void LoadButton()
    {
        filesList.Clear();
        filesDropdown.ClearOptions();
        filesDropdown.RefreshShownValue();

        string path = Application.persistentDataPath;
        string[] jsonFilesPaths = Directory.GetFiles(@path, "*.json");
        string[] pngFilesPaths = Directory.GetFiles(@path, "*.png");

        if(jsonFilesPaths.Length == 0 && pngFilesPaths.Length == 0) { return; }

        foreach (string file in jsonFilesPaths)
        {
            filesList.Add(Path.GetFileName(file));
        }
        foreach (string file in pngFilesPaths)
        {
            filesList.Add(Path.GetFileName(file));
        }

        selectedFilePath = filesList[filesDropdown.value];

        filesDropdown.AddOptions(filesList);
        filesDropdown.RefreshShownValue();
    }

    public void DropdownSelect(int index)
    {
        string filePath = Application.persistentDataPath + "/";
        selectedFilePath = Path.Combine(filePath,filesDropdown.options[filesDropdown.value].text);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
