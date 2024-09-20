using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Authentication;
using System.Threading.Tasks;
using Unity.Services.Core;
using Unity.Services.CloudSave;
using System;
public class AuthManager : MonoBehaviour
{
    bool isSignedIn;
    public bool IsSignedIn { get { return isSignedIn; } }
    public static AuthManager Instance;

    const string signInStatus = "isSignedOnce";

    async void Start()
    {
        SaveAndLoadData.Load();
        Instance = this;
        //await UnityServices.InitializeAsync();
        //isSignedIn = false;
        //SignIn();
    }
    async void SignIn()
    {
        await SignInAnonymous();
    }
    async Task SignInAnonymous()
    {
        if (!AuthenticationService.Instance.SessionTokenExists)
        {
            return;
        }
        try
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            string name = AuthenticationService.Instance.PlayerName;
            isSignedIn = true;
            if (!PlayerPrefs.HasKey(signInStatus))
            {
                GetDataFromCloud();
            }
            Debug.Log("signed in successfully");

        }
        catch (AuthenticationException ex)
        {
            SaveAndLoadData.Load();
            Debug.LogException(ex);
        }
        catch (RequestFailedException ex)
        {
            SaveAndLoadData.Load();
            Debug.LogException(ex);
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }
    }
    public async void SaveDataToCloud()
    {
        var data = new Dictionary<string, object>() { { "coin", GameLoadState.coinAmt }, { "score", GameLoadState.highScore } };
        await CloudSaveService.Instance.Data.Player.SaveAsync(data);
    }
    async void GetDataFromCloud()
    {
        var serverData = await CloudSaveService.Instance.Data.Player.LoadAsync(new HashSet<string> { "coin", "score" });
        int coinTemp = 0; float scoreTemp = 0;
        bool isPlayerPresent = false;

        if (serverData.TryGetValue("coin", out var coin))
        {
            coinTemp = coin.Value.GetAs<int>();
            isPlayerPresent = true;
        }
        if (serverData.TryGetValue("score", out var score))
        {
            score.Value.GetAs<float>();
            scoreTemp = score.Value.GetAs<float>();
        }
        if (isPlayerPresent)
        {
            if (SaveAndLoadData.isFilePresent)
            {
                Debug.Log("case1");
                GameLoadState.coinAmt = coinTemp;
                GameLoadState.highScore = scoreTemp;
                SaveAndLoadData.Save();
            }
            else
            {
                Debug.Log("case2");
                SaveAndLoadData.CreateFile(coinTemp, scoreTemp);
            }
            PlayerPrefs.SetInt(signInStatus, 1);
        }
        if (!isPlayerPresent)
        {
            if (SaveAndLoadData.isFilePresent)
            {
                Debug.Log("case3");
                SaveAndLoadData.Load();
                SaveDataToCloud();
            }
            else
            {
                Debug.Log("case4");
                GameLoadState.coinAmt = 100;
                GameLoadState.highScore = 0;
                SaveAndLoadData.CreateFile(100, 0);
                SaveDataToCloud();
            }
            PlayerPrefs.SetInt(signInStatus, 1);
        }
    }

}
