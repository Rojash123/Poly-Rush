using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{

    [SerializeField] GameObject panel;
    public void PlayAgain()
    {
        SceneManager.LoadScene(0);
    }


}
