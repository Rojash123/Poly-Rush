using UnityEngine;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine.SocialPlatforms.Impl;
public class AuthManager : MonoBehaviour
{
    bool isSignedIn;
    public bool IsSignedIn { get { return isSignedIn; } }
    public static AuthManager Instance;
    public string name, id, imageUrl;

    async void Start()
    {
        SignInGooglePlay();
        PlayGamesPlatform.Activate();
        Instance = this;
    }


    public void ShowLeaderBoard()
    {
        if (IsSignedIn)
        {
            Social.ShowLeaderboardUI();
        }
        else
        {
            SignInGooglePlay();
        }
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
            SaveAndLoadData.Load();
            Social.ReportScore((long)GameLoadState.highScore, "CgkIpYyxrb4UEAIQAQ", null);
            string name = PlayGamesPlatform.Instance.GetUserDisplayName();
            string id = PlayGamesPlatform.Instance.GetUserId();
            string imageUrl = PlayGamesPlatform.Instance.GetUserImageUrl();

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

}
