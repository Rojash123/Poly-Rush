using UnityEngine;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using Unity.Services.Core;
using Unity.Services.Authentication;
using System.Threading.Tasks;
using System;
using UnityEngine.Networking;
using System.Collections;

public class AuthManager : MonoBehaviour
{
    public bool isSubscribed;
    public bool IsSignedIn;
    public static AuthManager Instance;
    public string name, id, imageUrl;
    public Sprite avatar;

    public enum loginType
    {
        requireAuth,
        donotrequireAuth
    }
    public loginType loginMode;
    

    private string token;

    void Start()
    {
        if(loginMode == loginType.requireAuth)
        {
            isInternetAvailable = !(Application.internetReachability == NetworkReachability.NotReachable);
            if (isInternetAvailable)
            {
                SignInGooglePlay();
            }
            else
            {
                AuthManager_onInternetConnectionLost();
            }
        }
        else
        {
            IsSignedIn = true;
        }
       
    }
    public void SubScribeEvents()
    {
        onInternetConnectionRestored += AuthManager_onInternetConnectionRestored;
        onInternetConnectionLost += AuthManager_onInternetConnectionLost;

        isSubscribed = true;
    }

    private void OnDestroy()
    {
        onInternetConnectionRestored -= AuthManager_onInternetConnectionRestored;
        onInternetConnectionLost -= AuthManager_onInternetConnectionLost;
    }

    private void AuthManager_onInternetConnectionLost()
    {
        AdManager.Instance.PleaseCheckInternetConnection.SetActive(true);
    }

    private void AuthManager_onInternetConnectionRestored()
    {
        AdManager.Instance.PleaseCheckInternetConnection.SetActive(false);
        PlayGamesPlatform.Instance.RequestServerSideAccess(true, Response);
    }

    public void ShowLeaderBoard()
    {
        if (IsSignedIn)
        {
            Social.ShowLeaderboardUI();
        }
    }

    async Task SigninWithGooglePlay()
    {
        if(AuthenticationService.Instance.IsSignedIn) { return; }
        try
        {
            await AuthenticationService.Instance.SignInWithGooglePlayGamesAsync(token);
        }
        catch (AuthenticationException ex)
        {
            IsSignedIn = false;
            Debug.LogException(ex);
        }
        catch (RequestFailedException ex)
        {
            IsSignedIn = false;
            Debug.LogException(ex);
        }
        IsSignedIn = true;
    }

    public void SignInGooglePlay()
    {
        PlayGamesPlatform.Instance.Authenticate(ProcessAuthentication);
    }
    internal void ProcessAuthentication(SignInStatus status)
    {
        if (status == SignInStatus.Success)
        {
            name = PlayGamesPlatform.Instance.GetUserDisplayName();
            id = PlayGamesPlatform.Instance.GetUserId();
            imageUrl = PlayGamesPlatform.Instance.GetUserImageUrl();
            StartCoroutine(DownloadSprite(imageUrl));
            PlayGamesPlatform.Instance.RequestServerSideAccess(true, Response);
        }
        else
        {
            IsSignedIn = false;
        }
    }

    public async void Response(string response) 
    {
        token = response;
        await SigninWithGooglePlay();
    }

    IEnumerator DownloadSprite(string url)
    {
        using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(url))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(www.error);
            }
            else
            {
                // Get the texture
                Texture2D texture = DownloadHandlerTexture.GetContent(www);
                // Create a sprite
                Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                // You can now use the sprite (for example, assign it to a SpriteRenderer)
                avatar = sprite;
            }
        }
    }
    void Awake()
    {
        Instance = this;

        PlayGamesPlatform.Activate();
        UnityServices.InitializeAsync();
        InvokeRepeating(nameof(CheckNetwork), 0, 0.15f);
    }
    void CheckNetwork()
    {
        if(Application.internetReachability== NetworkReachability.NotReachable)
        {
            IsInternetAvailable = false;
        }
        else
        {
            IsInternetAvailable= true;
        }
    }

    private bool isInternetAvailable;
    public event Action onInternetConnectionLost, onInternetConnectionRestored;
    public bool IsInternetAvailable
    {
        get { return isInternetAvailable; }
        set
        {
            if (value != isInternetAvailable)
            {
                if (value)
                {
                    onInternetConnectionRestored?.Invoke();
                }
                else
                {
                    onInternetConnectionLost?.Invoke();
                }
                isInternetAvailable = value;
            }
        }
    }
}
