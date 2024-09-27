using System;
using Unity.Services.CloudSave;
using UnityEngine;
using System.Collections.Generic;
using Newtonsoft.Json;

public class SaveAndLoadData:MonoBehaviour
{
    private static string saveData="saveData";

    public static async void SaveData()
    {
        if (!AuthManager.Instance.IsSignedIn) return;

        var currentData = new Data(GameLoadState.coinAmt, GameLoadState.highScore);
        var serializedData= JsonConvert.SerializeObject(currentData);
        var data = new Dictionary<string, object> { { saveData, serializedData } };
        await CloudSaveService.Instance.Data.Player.SaveAsync(data);
    }
    public static async void LoadFileDatas()
    {
        if (!AuthManager.Instance.IsSignedIn) return;

        var coinData = await CloudSaveService.Instance.Data.Player.LoadAsync(new HashSet<string> 
        {
          saveData
        });

        if (coinData.ContainsKey(saveData))
        {
            string response = coinData[saveData].Value.GetAs<string>();
            var data=JsonConvert.DeserializeObject<Data>(response);
            GameLoadState.coinAmt = data.coin;
            GameLoadState.highScore = data.highScore;
        }
        else
        {
            GameLoadState.coinAmt=100;
            GameLoadState.highScore=0;
            SaveData();
        }
    }
}
[Serializable]
public struct Data
{
    public int coin;
    public float highScore;

    public Data(int coin,float highScore)
    {
        this.coin = coin;
        this.highScore = highScore;
    }
}
