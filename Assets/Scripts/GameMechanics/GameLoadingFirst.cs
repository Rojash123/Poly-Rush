using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameLoadingFirst : MonoBehaviour
{
    [SerializeField] Slider loadingSlider;
    int sliderValue;
    [SerializeField] GameObject loadingPanel, gameLoading, selectLevelScreen, signInObject;
    [SerializeField] GameObject normalRush, neonRush, neonRushLock;
    [SerializeField] TextMeshProUGUI dynamicText;
    private const string adCount = "AdWatchLeft";
    private static GameLoadingFirst instance;

    public Sprite[] gameloadingScreen;
    public Image loadingScreen;

    public Button neonLevel;
    public static GameLoadingFirst Instance { get { return instance; } }

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        StopAllCoroutines();
        if (!PlayerPrefs.HasKey(adCount))
        {
            PlayerPrefs.SetInt(adCount, 3);
        }
        else
        {
            SetDynamicText();
        }
        sliderValue = 0;
        if (!(Application.internetReachability == NetworkReachability.NotReachable))
        {
            StartCoroutine(LoadSceneSlider());
        }
        else
        {
            AdManager.Instance.PleaseCheckInternetConnection.SetActive(true);
        }
    }
    public void SetDynamicText()
    {
        dynamicText.text = "Watch " + PlayerPrefs.GetInt(adCount) + " Ads to unlock Level";
        if (PlayerPrefs.GetInt(adCount) <= 0)
        {
            neonLevel.interactable = true;
            neonRushLock.SetActive(false);
        }
    }
    IEnumerator LoadSceneSlider()
    {
        yield return new WaitUntil(() => AuthManager.Instance.IsSignedIn);
        signInObject.SetActive(false);

        while (sliderValue < 100)
        {
            sliderValue++;
            loadingSlider.value = sliderValue;
            yield return new WaitForSeconds(0.01f);
        }
        if (!AuthManager.Instance.isSubscribed)
        {
            AuthManager.Instance.SubScribeEvents();
        }

        selectLevelScreen.SetActive(true);
        loadingPanel.SetActive(false);
    }

    public void LoadGame(int index)
    {
        loadingScreen.sprite = gameloadingScreen[index - 1];
        GameLoadState.loadedFromStart = true;
        gameLoading.SetActive(true);
        SceneManager.LoadScene(index+1);
    }
    public void ShowAdlevelUnlock()
    {
        AdManager.Instance.ShowRewardedAdLevelUnlock();
    }

}
