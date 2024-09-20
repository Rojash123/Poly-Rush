using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private static UIManager instance;
    public static UIManager Instance { get { return instance; } }


    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
    }

    public TextMeshProUGUI score, thisCoinAmount, menuScore, menuCoin, gameOverCoin, gameOverScore, countDownText,highScoreText;
    public GameObject mainMenuPanel, overLayPanel, loadingPanel, countDownPanel;

    #region MainMenu
    public void StartGame()
    {
        mainMenuPanel.SetActive(false);
        overLayPanel.SetActive(true);
        PlayerController.Instance.isSwipeAllowed = true;
        PlayerController.Instance.canMove = true;
        SoundManager.Instance.PlayInGameMusic();
    }

    public void GameOver()
    {
        PlayerController.Instance.isSwipeAllowed = false;

        PlayerController.Instance.canMove = false;
        PlayerController.Instance.ExplosionBoom();
        pause.interactable = false;
        SoundManager.Instance.bgMusic.Pause();

        StartCoroutine(ShowAdBanner());
    }

    GameObject currentPanel;
    [SerializeField] GameObject currentMiniPanel, currentInfoPage;
    private void Start()
    {
        currentPanel = null;
        CheckPlayerPrefsSound();
        watchAdCount = 0;

        if (GameLoadState.loadedFromStart)
        {
            mainMenuPanel.SetActive(true);
            SoundManager.Instance.PlayMenuMusic();
        }
        else
        {
            StartGame();
        }
    }

    [SerializeField] GameObject closeButton, behindCloseButton;

    public void GoBackToMenu()
    {
        SoundManager.Instance.UIClickSound();
        loadingPanel.SetActive(true);
        SceneManager.LoadScene(0);
    }
    public void OpenPanel(GameObject temp)
    {
        highScoreText.text = GameLoadState.highScore.ToString("f2");
        SoundManager.Instance.UIClickSound();
        currentPanel = temp;
        LeanTween.scale(currentPanel, Vector3.one, tweenTime).setEaseOutBack();
        closeButton.gameObject.SetActive(true);
        behindCloseButton.gameObject.SetActive(true);
    }

    [SerializeField] float tweenTime;
    public void CloseThisPanel()
    {
        SoundManager.Instance.UIClickSound();
        LeanTween.scale(currentPanel, Vector3.zero, tweenTime).setEaseInBack();
        closeButton.gameObject.SetActive(false);
        behindCloseButton.gameObject.SetActive(false);
        currentPanel = null;
    }
    public void OpenMiniPanel(GameObject temp)
    {
        SoundManager.Instance.UIClickSound();
        currentMiniPanel.SetActive(false);
        currentMiniPanel = temp;
        currentMiniPanel.SetActive(true);
    }
    public void InfoSwitchPage(GameObject temp)
    {
        SoundManager.Instance.UIClickSound();
        currentInfoPage.SetActive(false);
        currentInfoPage = temp;
        currentInfoPage.SetActive(true);
    }
    public void OpenLink()
    {
        Application.OpenURL("https://rojashshahi.com/");
    }

    #endregion
    #region GameOverlay

    [SerializeField] GameObject pauseMenu, adMenu, confirmationMenu, gameOverMenu;

    [SerializeField] Button pause;
    public void PauseOrResumeGame()
    {
        SoundManager.Instance.UIClickSound();
        if (Time.timeScale > 0)
        {
            SoundManager.Instance.bgMusic.Pause();
            pauseMenu.SetActive(true);
            PlayerController.Instance.isSwipeAllowed = false;
            PlayerController.Instance.canMove = false;
            Time.timeScale = 0;
        }
        else
        {
            SoundManager.Instance.bgMusic.Play();
            pauseMenu.SetActive(false);
            StartCoroutine(ResumeGameCountDown());
        }
    }
    IEnumerator ResumeGameCountDown()
    {
        countDownPanel.SetActive(true);
        int val = 3;
        countDownText.text = val.ToString();

        while (val > 0)
        {
            yield return new WaitForSecondsRealtime(1f);
            val--;
            countDownText.text = val.ToString();
        }
        Time.timeScale = 1;
        PlayerController.Instance.isSwipeAllowed = true;
        PlayerController.Instance.canMove = true;
        countDownPanel.SetActive(false);

    }
    public void MainMenuFromPause()
    {
        SoundManager.Instance.UIClickSound();
        confirmationMenu.SetActive(true);
    }
    public void YesConfirmMenu(int sceneIndex)
    {
        SoundManager.Instance.UIClickSound();
        GameLoadState.loadedFromStart = true;
        AdManager.Instance.ShowAdOnAppropriateCondition();
        SceneManager.LoadScene(sceneIndex);
        Time.timeScale = 1;
    }


    public void NoConfirmMenu()
    {
        SoundManager.Instance.UIClickSound();
        confirmationMenu.SetActive(false);
    }

    [SerializeField] TextMeshProUGUI timerText;
    int watchAdCount;
    public IEnumerator ShowAdBanner()
    {
        yield return new WaitForSeconds(2f);
        if (watchAdCount == 0 && AdManager.Instance.isRewardAdAvailable)
        {
            StartCoroutine(_ShowAdBanner());
        }
        else
        {
            GameOverMenu();
        }
    }
    IEnumerator _ShowAdBanner()
    {
        adMenu.SetActive(true);
        int i = 5;
        timerText.text = i.ToString();

        while (i > 0)
        {
            yield return new WaitForSeconds(1f);
            i--;
            timerText.text = i.ToString();
        }
        GameOverMenu();
    }

    public void WatchAd()
    {
        if (AdManager.Instance.isRewardAdAvailable)
        {
            AdManager.Instance.ShowRewardedAd();
            SoundManager.Instance.UIClickSound();
        }
    }
    public void OnRewardAdWatched()
    {
        pause.interactable = true;
        watchAdCount = 1;
        StopAllCoroutines();
        adMenu.SetActive(false);
        overLayPanel.SetActive(true);
        PlayerController.Instance.ActivateShield();
    }

    public void SkipAd()
    {
        SoundManager.Instance.UIClickSound();
        GameOverMenu();
    }
    public void GameOverMenu()
    {
        StopAllCoroutines();
        PlayerController.Instance.GameOver();
        adMenu.SetActive(false);
        gameOverMenu.SetActive(true);
    }

    public void RestartGame(int sceneIndex)
    {
        SoundManager.Instance.UIClickSound();
        GameLoadState.loadedFromStart = false;
        AdManager.Instance.ShowAdOnAppropriateCondition();
        SceneManager.LoadScene(sceneIndex);
    }
    private void OnApplicationFocus(bool focus)
    {
        if (!focus)
        {
            if (PlayerController.Instance.canMove)
            {
                SoundManager.Instance.bgMusic.Pause();
                pauseMenu.SetActive(true);
                PlayerController.Instance.isSwipeAllowed = false;
                PlayerController.Instance.canMove = false;
                Time.timeScale = 0;
            }
        }
    }
    public void ShowAdAStore()
    {

        AdManager.Instance.ShowRewardedAStore();
        SoundManager.Instance.UIClickSound();
    }
    #endregion
    #region HandleSound;

    private string canPlaySoundPrefs = "canPlaySound";
    private string canVibratePrefs = "canVibrate";

    void CheckPlayerPrefsSound()
    {
        if (!PlayerPrefs.HasKey(canPlaySoundPrefs) || !PlayerPrefs.HasKey(canVibratePrefs))
        {
            PlayerPrefs.SetInt(canPlaySoundPrefs, 1);
            PlayerPrefs.SetInt(canVibratePrefs, 1);
            canVibrate = true;
        }
        else
        {
            if (PlayerPrefs.GetInt(canPlaySoundPrefs) == 0)
            {
                TurnOffSound();
            }
            else
            {
                TurnOnSound();
            }
            if (PlayerPrefs.GetInt(canVibratePrefs) == 0)
            {
                TurnOffVibration();
            }
            else
            {
                TurnOnVibration();
            }
        }

    }

    private bool canVibrate;
    [SerializeField] GameObject sounOffButton, vibtrateOffButton;

    public void TurnOnSound()
    {
        PlayerPrefs.SetInt(canPlaySoundPrefs, 1);
        AudioListener.volume = PlayerPrefs.GetInt(canPlaySoundPrefs);
        sounOffButton.SetActive(false);
    }
    public void TurnOffSound()
    {
        PlayerPrefs.SetInt(canPlaySoundPrefs, 0);
        AudioListener.volume = PlayerPrefs.GetInt(canPlaySoundPrefs);
        sounOffButton.SetActive(true);
    }
    public void TurnOnVibration()
    {
        PlayerPrefs.SetInt(canVibratePrefs, 1);
        canVibrate = true;
        vibtrateOffButton.SetActive(false);
    }
    public void TurnOffVibration()
    {
        PlayerPrefs.SetInt(canVibratePrefs, 0);
        canVibrate = false;
        vibtrateOffButton.SetActive(true);

    }
    #endregion;
}
