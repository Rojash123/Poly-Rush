using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public static class GameLoadState
{
    public static bool loadedFromStart;

    private static int coinAmount;
    private static float highScoreAmount;
    public static int coinAmt { get { return coinAmount; } set { coinAmount = value; UIManager.Instance.menuCoin.text = value.ToString(); } }
    public static float highScore { get { return highScoreAmount; } set { highScoreAmount = value; UIManager.Instance.menuScore.text = value.ToString("f2"); } }
}

