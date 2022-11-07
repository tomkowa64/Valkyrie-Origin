using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveSystem
{
    private static string path = Application.persistentDataPath + "/";
    private static string fileExtension = ".kjg";

    public static void Save (string saveName, GameObject player, SceneController scene)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path + saveName + fileExtension, FileMode.Create);

        SaveData data = new SaveData(player, scene);

        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static SaveData Load (string saveName)
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
}
