using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class JsonLoader
{
    public static async Task SaveMap(string name)
    {
        string json = JsonUtility.ToJson(GridManager.Instance.GetJsonMap(name));
        byte[] encodedText = Encoding.UTF8.GetBytes(json);

        string filePath = Application.persistentDataPath;
        string path = Path.Combine(filePath, $"{name}.json");

        // Write the file
        using (FileStream sourceStream = new FileStream(path,
            FileMode.Create, FileAccess.Write, FileShare.Write,
            bufferSize: 4096, useAsync: true))
        {
            await sourceStream.WriteAsync(encodedText, 0, encodedText.Length);
        };
    }

    public static async Task<string> LoadMap(string name)
    {
        string filePath = Application.persistentDataPath;
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
}


[Serializable]
public class JsonMap
{
    public string name;
    public int nbrLines;
    public int nbrColumns;
    public JsonCell[] lstCell;
}

[Serializable]
public class JsonCell
{
    public int x;
    public int y;
    public bool isAlive;
}