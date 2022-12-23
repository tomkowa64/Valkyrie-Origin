using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
using System.Linq;

public static class SaveSystem
{
    private static string path = Application.persistentDataPath + "/saves/";
    private static string fileExtension = ".vo";

    public static void NewSave(string saveName)
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path + saveName + fileExtension, FileMode.Create);

        SaveData data = new SaveData();

        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static void Save(string saveName, GameObject player, SceneController scene, GameObject[] skills, GameManager gameManager)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path + saveName + fileExtension, FileMode.Create);

        SaveData data = new SaveData(player, scene, skills, gameManager);

        Debug.Log("Game saved");

        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static bool SaveExists(string saveName)
    {
        return File.Exists(path + saveName + fileExtension);
    }

    public static SaveData Load(string saveName)
    {
        if (File.Exists(path + saveName + fileExtension))
        {
            Debug.Log("Loaded save file from path: " + path + saveName + fileExtension);

            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path + saveName + fileExtension, FileMode.Open);

            SaveData data = formatter.Deserialize(stream) as SaveData;
            stream.Close();

            return data;
        }
        else
        {
            Debug.LogError("Save file not found in " + path);
            return null;
        }
    }

    public static List<FileInfo> GetAllSaves()
    {
        if (!Directory.Exists(path))
        {
            return new List<FileInfo>();
        }
        else
        {
            List<FileInfo> files = new DirectoryInfo(path).GetFiles().OrderBy(p => p.CreationTime).ToList();
            return files;
        }

    }
}
