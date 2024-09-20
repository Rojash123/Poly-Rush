using System;
using System.Diagnostics.Tracing;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameSkinComponents : MonoBehaviour
{
    public int SkinIndex;
    public int SkinValue;

    [SerializeField] Material skinMaterial,trailMaterial;
    [SerializeField] private Button buyBtn;
    public bool haveTrail;
    public TextMeshProUGUI skinStatus;
    public GameObject Selected;
    public Action<int> OnItemPurchasedEvent,onItemSelectedEvent;

    public bool isPurchased;
    public bool isSelected;

    private void Start()
    {
    }

    public void LockedState()
    {
        Selected.SetActive(false);
        isPurchased = false;
        isSelected = false;
        buyBtn.interactable = true;

        buyBtn.onClick.RemoveAllListeners();
        buyBtn.onClick.AddListener(SkinBuy);
        skinStatus.text = SkinValue.ToString();
    }

    public void UnlockedState()
    {
        isPurchased = true;
        isSelected = false;
        Selected.SetActive(false);
        buyBtn.interactable = true;

        buyBtn.onClick.RemoveAllListeners();
        buyBtn.onClick.AddListener(SkinSelected);
        skinStatus.text = "Equip";

    }

    public void SelectedState()
    {
        isSelected = true;
        buyBtn.interactable = false;
        Selected.SetActive(true);
        skinStatus.text = "Equipped";

        PlayerController.Instance.ballSkin.material = skinMaterial;
        if (haveTrail)
        {
            PlayerController.Instance.trailSkin.material = trailMaterial;
            PlayerController.Instance.trailSkin.enabled = true;
        }
        else
        {
            PlayerController.Instance.trailSkin.enabled = false;
        }
    }

    public void SetSkinOfBall()
    {
    }

    public void SkinBuy()
    {
        if(GameLoadState.coinAmt < SkinValue) 
        {
            return;
        }

        
        GameLoadState.coinAmt -= SkinValue;
        OnItemPurchasedEvent?.Invoke(SkinIndex);
        skinStatus.text = "Equip";
    }

    public void SkinSelected()
    {
        if(isPurchased && !isSelected)
        {
            onItemSelectedEvent?.Invoke(SkinIndex);
        }
    }
}
