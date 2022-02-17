using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class PNGLoader : MonoBehaviour
{
    public static async Task SaveMapToPNG(string name)
    {
        Texture2D texture = GridManager.Instance.GetTexture();
        byte[] data = texture.EncodeToPNG();
        string filePath = Application.persistentDataPath;
        string path = Path.Combine(filePath, $"{name}.png");
        File.WriteAllBytes(path, data);
    }

    public static async Task<Texture2D> LoadMapFromPNG(string path)
    {
        Texture2D texture = new Texture2D(0,0);
        byte[] data;
        if (File.Exists(path))
        {
            data = File.ReadAllBytes(path);
            texture.LoadImage(data);
        }
        return texture;
    }
}
