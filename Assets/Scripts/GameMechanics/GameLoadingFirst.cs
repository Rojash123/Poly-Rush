using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameLoadingFirst : MonoBehaviour
{
    [SerializeField] Slider loadingSlider;
    int sliderValue;

    [SerializeField] GameObject loadingPanel,gameLoading,selectLevelScreen;

    [SerializeField] GameObject normalRush, neonRush, neonRushLock;

    [SerializeField] TextMeshProUGUI dynamicText;

    private const string adCount="AdWatchLeft";

    private static GameLoadingFirst instance;
    public static GameLoadingFirst Instance { get { return instance; } }

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        if (!PlayerPrefs.HasKey(adCount))
        {
            PlayerPrefs.SetInt(adCount, 3);
        }
        else
        {
            SetDynamicText();
        }
        sliderValue = 0;
        if(!(Application.internetReachability == NetworkReachability.NotReachable))
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
        dynamicText.text = "Watch " + PlayerPrefs.GetInt(adCount) + " Ads to unlock";
        if(PlayerPrefs.GetInt(adCount) <= 0) 
        {
            neonRushLock.SetActive(false);
        }
    }
    IEnumerator LoadSceneSlider()
    {
        Debug.Log("Hello");
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

    public void ChangLevel(GameObject gameobject)
    {
        normalRush.SetActive(false);
        neonRush.SetActive(false);
        gameobject.SetActive(true);
    }

    public void LoadGame(int index)
    {
        GameLoadState.loadedFromStart = true;
        gameLoading.SetActive(true);
        SceneManager.LoadScene(index);
    }


    public void ShowAdlevelUnlock()
    {
        AdManager.Instance.ShowRewardedAdLevelUnlock();
    }

}
