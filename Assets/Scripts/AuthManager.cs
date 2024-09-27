using UnityEngine;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using Unity.Services.Core;
using Unity.Services.Authentication;
using System.Threading.Tasks;
using System;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using System.Collections;

public class AuthManager : MonoBehaviour
{
    bool isSignedIn;
    public bool IsSignedIn { get { return isSignedIn; } }
    public static AuthManager Instance;
    public string name, id, imageUrl;
    public Sprite avatar;

    private string token;

    void Start()
    {
        PlayGamesPlatform.Activate();
        SignInGooglePlay();
    }
    public void SubScribeEvents()
    {
        onInternetConnectionRestored += AuthManager_onInternetConnectionRestored;
        onInternetConnectionLost += AuthManager_onInternetConnectionLost;
    }

    private void OnDestroy()
    {
        onInternetConnectionRestored -= AuthManager_onInternetConnectionRestored;
        onInternetConnectionLost -= AuthManager_onInternetConnectionLost;
    }

    private void AuthManager_onInternetConnectionLost()
    {
        Debug.Log("Internet Check was Lost");
        AdManager.Instance.PleaseCheckInternetConnection.SetActive(true);
    }

    private void AuthManager_onInternetConnectionRestored()
    {
        Debug.Log("Internet Check was Restored");
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
        try
        {
            await AuthenticationService.Instance.SignInWithGooglePlayGamesAsync(token);
        }
        catch (AuthenticationException ex)
        {
            isSignedIn = false;
            Debug.LogException(ex);
        }
        catch (RequestFailedException ex)
        {
            isSignedIn = false;
            Debug.LogException(ex);
        }
        isSignedIn = true;
    }

    public void SignInGooglePlay()
    {
        PlayGamesPlatform.Instance.Authenticate(ProcessAuthentication);
    }
    internal void ProcessAuthentication(SignInStatus status)
    {
        Debug.Log(status);
        if (status == SignInStatus.Success)
        {
            SaveAndLoadData.LoadFileDatas();
            name = PlayGamesPlatform.Instance.GetUserDisplayName();
            id = PlayGamesPlatform.Instance.GetUserId();
            imageUrl = PlayGamesPlatform.Instance.GetUserImageUrl();
            PlayGamesPlatform.Instance.RequestServerSideAccess(true, Response);
        }
        else
        {
            isSignedIn = false;
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
        Debug.Log("Internet Check was Called");
        Instance = this;
        PlayGamesPlatform.Activate();
        UnityServices.InitializeAsync();
        InvokeRepeating(nameof(CheckNetwork), 0, 0.15f);
    }
    void CheckNetwork()
    {
        StartCoroutine(GetRequest("https://google.com"));
    }

    IEnumerator GetRequest(string uri)
    {
        GameObject currentSelected = EventSystem.current.currentSelectedGameObject;
        // Check if the current selected object is an InputField
        if (currentSelected != null && currentSelected.GetComponent<TMP_InputField>() == null)
        {
            // Suppress the selected UI element only if it's not an input field
            EventSystem.current.SetSelectedGameObject(null);
        }
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            yield return webRequest.SendWebRequest();

            switch (webRequest.result)
            {

                case UnityWebRequest.Result.ConnectionError:
                    IsInternetAvailable = false;
                    break;

                case UnityWebRequest.Result.DataProcessingError:
                    IsInternetAvailable = false;
                    break;

                case UnityWebRequest.Result.ProtocolError:
                    IsInternetAvailable = false;
                    break;

                case UnityWebRequest.Result.Success:
                    IsInternetAvailable = true;
                    break;
            }
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
