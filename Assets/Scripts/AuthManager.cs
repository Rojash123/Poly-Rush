using UnityEngine;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine.SocialPlatforms.Impl;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.CloudSave;
using System.Threading.Tasks;
using System;

public class AuthManager : MonoBehaviour
{
    bool isSignedIn;
    public bool IsSignedIn { get { return isSignedIn; } }
    public static AuthManager Instance;
    public string name, id, imageUrl;

    void Start()
    {
        Instance = this;
        SignInGooglePlay();
        PlayGamesPlatform.Activate();
    }


    public void ShowLeaderBoard()
    {
        if (IsSignedIn)
        {
            Social.ShowLeaderboardUI();
        }
    }

    async Task SigninWithGooglePlay(string token)
    {
        try
        {
            await AuthenticationService.Instance.SignInWithGooglePlayGamesAsync(token);
        }
        catch (AuthenticationException ex)
        {
            Debug.LogException(ex);
        }
        catch (RequestFailedException ex)
        {
            Debug.LogException(ex);
        }
        LoadGameData();
    }

    public void SignInGooglePlay()
    {
        PlayGamesPlatform.Instance.Authenticate(ProcessAuthentication);
    }
    internal void ProcessAuthentication(SignInStatus status)
    {
        if (status == SignInStatus.Success)
        {
            isSignedIn = true;
            SaveAndLoadData.LoadFileDatas();
            Social.ReportScore((long)GameLoadState.highScore, "CgkIpYyxrb4UEAIQAQ", null);
            string name = PlayGamesPlatform.Instance.GetUserDisplayName();
            string id = PlayGamesPlatform.Instance.GetUserId();
            string imageUrl = PlayGamesPlatform.Instance.GetUserImageUrl();
            PlayGamesPlatform.Instance.RequestServerSideAccess(IsSignedIn, CallBack);
            // Continue with Play Games Services
        }
        else
        {
            isSignedIn = false;
            // Disable your integration with Play Games Services or show a login button
            // to ask users to sign-in. Clicking it should call
            // PlayGamesPlatform.Instance.ManuallyAuthenticate(ProcessAuthentication).
        }
    }
    async void CallBack(string token)
    {
        if (IsSignedIn)
        {
            await SigninWithGooglePlay(token);
        }
        else
        {

        }
    }

    void LoadGameData()
    {

    }
}
