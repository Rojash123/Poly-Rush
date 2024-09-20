using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class SaveAndLoadData
{
    private static string file = "State";
    private static string directoryName = "Hello";
    public static void Save()
    {
        string path =Path.Combine(Application.persistentDataPath, directoryName);

        BinaryFormatter bf = new BinaryFormatter();
        FileStream saveFile = File.Create(path);

        Data datas = new Data();

        datas.coin = GameLoadState.coinAmt;
        datas.highScore = GameLoadState.highScore;

        bf.Serialize(saveFile, datas);

        saveFile.Close();

        if (AuthManager.Instance.IsSignedIn)
        {
            AuthManager.Instance.SaveDataToCloud();
        }
    }


    public static bool isFilePresent { get { return File.Exists(Path.Combine(Application.persistentDataPath, directoryName)); } }
    public static void Load()
    {
        if (isFilePresent)
        {
            LoadFileDatas();
        }
        else
        {
            CreateFile(100, 0);
        }

    }
    static void LoadFileDatas()
    {
        string path = Path.Combine(Application.persistentDataPath, directoryName);
        Debug.Log(path);
        BinaryFormatter bf = new BinaryFormatter();
        FileStream saveFile = File.Open(path, FileMode.Open);
        Data gameDatas = (Data)bf.Deserialize(saveFile);

        GameLoadState.coinAmt = gameDatas.coin;
        GameLoadState.highScore = gameDatas.highScore;

        saveFile.Close();
    }
    public static void CreateFile(int coin, float data)
    {
        BinaryFormatter bf = new BinaryFormatter();

        string path = Path.Combine(Application.persistentDataPath, directoryName);
        FileStream saveFile = File.Create(path);
        Data datas = new Data();

        datas.coin = coin;
        datas.highScore = data;
        bf.Serialize(saveFile, datas);

        saveFile.Close();
    }
}
[Serializable]
public struct Data
{
    public int coin;
    public float highScore;
    public int menuAdCount;
}
