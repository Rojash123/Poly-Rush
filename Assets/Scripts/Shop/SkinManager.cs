using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Unity.Services.CloudSave;
using UnityEngine;

public class SkinManager : MonoBehaviour
{
    public SkinManager Instance;
    public List<GameSkinComponents> skinComponents;
    public SkinData skinData;

    private List<int> unlockedData = new List<int>();
    private int selectedData = 0;

    private string skinSaveFile = "SkinCollection";

    public async void LoadFileDatas()
    {
        var coinData = await CloudSaveService.Instance.Data.Player.LoadAsync(new HashSet<string>
        {
          skinSaveFile
        });

        if (coinData.ContainsKey(skinSaveFile))
        {
            string response = coinData[skinSaveFile].Value.GetAs<string>();
            var data = JsonConvert.DeserializeObject<SkinData>(response);
            foreach (char c in data.UnlockedStateData)
            {
                unlockedData.Add(c - '0');
            }
            selectedData = data.selectedSkinIndex;
        }
        else
        {
            unlockedData.Clear();
            selectedData = 0;
            SaveData("0", 0);
        }
        AllSkinStateHandler();
    }

    private void Start()
    {
        Instance = this;
        foreach (var skinComponent in skinComponents)
        {
            skinComponent.OnItemPurchasedEvent += BuySkin;
            skinComponent.onItemSelectedEvent += SelectSkin;
        }
        LoadFileDatas();
    }
   
    void AllSkinStateHandler()
    {
        foreach (var item in skinComponents)
        {
            item.LockedState();
        }
        foreach (var item in unlockedData)
        {
            skinComponents[item].UnlockedState();
        }
        skinComponents[selectedData].SelectedState();
    }
    async void SaveData(string unlockedSkin, int selectedData)
    {
        var currentData = new SkinData(unlockedSkin, selectedData);
        var serializedData = JsonConvert.SerializeObject(currentData);
        var data = new Dictionary<string, object> { { skinSaveFile, serializedData } };
        await CloudSaveService.Instance.Data.Player.SaveAsync(data);
    }
    private void OnDestroy()
    {
        foreach (var skinComponent in skinComponents)
        {
            skinComponent.OnItemPurchasedEvent -= BuySkin;
            skinComponent.onItemSelectedEvent -= SelectSkin;
        }
    }

    public void BuySkin(int index)
    {
        for (int i = 0; i < skinComponents.Count; i++)
        {
            if (i == index)
            {
                skinComponents[i].UnlockedState();
            }
        }
        unlockedData.Add(index);
        SaveData(string.Join("", unlockedData), selectedData);
    }
    public void SelectSkin(int index)
    {
        selectedData = index;
        for (int i = 0; i < skinComponents.Count; i++)
        {
            if (i == index)
            {
                skinComponents[i].SelectedState();
            }
            else
            {
                if (skinComponents[i].isPurchased)
                {
                    skinComponents[i].UnlockedState();
                }
                else
                {
                    skinComponents[i].LockedState();
                }
            }
        }
        SaveData(string.Join("", unlockedData), selectedData);
    }
}

[Serializable]
public class SkinData
{
    public string UnlockedStateData;
    public int selectedSkinIndex;

    public SkinData(string unlockedStateData, int selectedSkinIndex)
    {
        UnlockedStateData = unlockedStateData;
        this.selectedSkinIndex = selectedSkinIndex;
    }
}
