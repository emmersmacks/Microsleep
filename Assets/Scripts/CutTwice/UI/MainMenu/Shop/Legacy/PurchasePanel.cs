using System;
using CutTwice.Gameplay;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using YG;
using YG.EditorScr;
using YG.Insides;
using YG.Utils.Pay;
#if UNITY_EDITOR
#endif

namespace CutTwice.UI.MainMenu.Shop.Legacy
{
    public class PurchasePanel : MonoBehaviour
    {
        public string id;
#if UNITY_EDITOR
        [Tooltip(Langs.t_purchaseImageLoad)]
#endif
        public ImageLoadYG purchaseImageLoad;
#if UNITY_EDITOR
        [Tooltip(Langs.t_currencyImageLoad)]
#endif
        public ImageLoadYG currencyImageLoad;
#if UNITY_EDITOR
        [Tooltip(Langs.t_showCurrencyCode)]
#endif
        public bool showCurrencyCode;
        
        public Button PurchaseButton;
        public TextMeshProUGUI PurchaseButtonText;

        [Serializable]
        public struct TextLegasy
        {
            public Text title, description, priceValue;
        }
        public TextLegasy textLegasy;

        public UnityEvent OnSelect;

#if TMP_YG2
        [Serializable]
        public struct TextMP
        {
            public TextMeshProUGUI title, description, priceValue;
        }
        public TextMP textMP;
#endif

        private void Start()
        {
            PurchaseButton.onClick.AddListener(BuyOrSelectPurchase);
            UpdateEntries(YG2.PurchaseByID(id));
        }

        public void UpdateEntries(Purchase data)
        {
            if (data == null)
            {
                Debug.LogError($"No product with ID found: {id}");
                return;
            }
            
            if (textLegasy.title) textLegasy.title.text = data.title;
            if (textLegasy.description) textLegasy.description.text = data.description;
            if (textLegasy.priceValue)
            {
                if (showCurrencyCode) textLegasy.priceValue.text = data.price;
                else textLegasy.priceValue.text = data.priceValue;
            }

#if TMP_YG2
            if (textMP.title) textMP.title.text = data.title;
            if (textMP.description) textMP.description.text = data.description;
            if (textMP.priceValue)
            {
                if (showCurrencyCode) textMP.priceValue.text = data.price;
                else textMP.priceValue.text = data.priceValue;
            }
#endif
            if (purchaseImageLoad)
            {
#if UNITY_EDITOR
                if (data.imageURI == InfoYG.DEMO_IMAGE)
                    purchaseImageLoad.Load(ServerInfo.saveInfo.purchaseImage);
                else
                    purchaseImageLoad.Load(data.imageURI);
#else
                purchaseImageLoad.Load(data.imageURI);
#endif
            }

            if (currencyImageLoad && data.currencyImageURL != string.Empty && data.currencyImageURL != null)
                currencyImageLoad.Load(data.currencyImageURL);
            
            var purchaseState = YG2.GetState($"payment_{id}");
            PurchaseButtonText.text = purchaseState == 0 ? data.price : PlayerData.SteeringWheelId == id ? "Selected" : "Select";
        }

        public void BuyOrSelectPurchase()
        {
            var purchaseState = YG2.GetState($"payment_{id}");

            if (purchaseState == 0)
            {
                YG2.BuyPayments(id);
            }
            else
            {
                if (PlayerData.SteeringWheelId != id)
                {
                    PlayerData.SteeringWheelId = id;
                    PlayerData.Save();
                }
                else
                {
                    PlayerData.SteeringWheelId = null;
                    PlayerData.Save();
                }
            }
            
            OnSelect.Invoke();
        }
    }
}