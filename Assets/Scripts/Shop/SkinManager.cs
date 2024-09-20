using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class SkinManager : MonoBehaviour
{
    public SkinManager Instance;
    public List<GameSkinComponents> skinComponents;
    public SkinData skinData;

    string fileName = "Hmm";
    private List<int> unlockedData = new List<int>();
    private int selectedData = 0;


    private void Start()
    {
        Instance = this;
        foreach (var skinComponent in skinComponents)
        {
            skinComponent.OnItemPurchasedEvent += BuySkin;
            skinComponent.onItemSelectedEvent += SelectSkin;
        }
        LoadDataFromSaveFile();
    }
    void LoadDataFromSaveFile()
    {
        if (File.Exists(Path.Combine(Application.persistentDataPath, fileName)))
        {
            string destination = Path.Combine(Application.persistentDataPath, fileName);
            FileStream file;
            file = File.OpenRead(destination);

            BinaryFormatter bf = new BinaryFormatter();
            var data = (SkinData)bf.Deserialize(file);
            file.Close();


            foreach (char c in data.UnlockedStateData)
            {
                unlockedData.Add(c - '0');
            }
            selectedData = data.selectedSkinIndex;
        }
        else
        {
            SaveData("0", 0);
            unlockedData.Add(0);
            selectedData = 0;
        }
        AllSkinStateHandler();
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
    void SaveData(string unlockedSkin, int selectedData)
    {
        string destination = Path.Combine(Application.persistentDataPath, fileName);
        FileStream file;

        if (File.Exists(destination)) file = File.OpenWrite(destination);
        else file = File.Create(destination);

        var data = new SkinData(unlockedSkin, selectedData);
        BinaryFormatter bf = new BinaryFormatter();
        bf.Serialize(file, data);
        file.Close();
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
