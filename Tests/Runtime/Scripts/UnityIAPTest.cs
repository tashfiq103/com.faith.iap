namespace com.faith.iap
{
#if FaithIAP
    using UnityEngine;
    using UnityEngine.UI;
    public class UnityIAPTest : MonoBehaviour
    {
#region Variables

        [SerializeField] private Button _smallCoinButton;
        [SerializeField] private Button _largeCoinButton;
        [SerializeField] private Button _noAdButton;
        [SerializeField] private Button _divaPackButton;
        [SerializeField] private Button _restorePurchaseButton;

#endregion

#region Mono Behaviour

        private void Awake()
        {
            _smallCoinButton.onClick.AddListener(() =>
            {
                FaithIAPManager.Instance.BuyIAPProduct(IAPProduct.IAP_SmallCoinPack, (product)=> { });
            });

            _largeCoinButton.onClick.AddListener(() =>
            {
                FaithIAPManager.Instance.BuyIAPProduct(IAPProduct.IAP_LargeCoinPack, (product) => { });
            });

            _noAdButton.onClick.AddListener(() =>
            {
                FaithIAPManager.Instance.BuyIAPProduct(IAPProduct.IAP_RemoveADs, (product) => { });
            });

            _divaPackButton.onClick.AddListener(() =>
            {
                FaithIAPManager.Instance.BuyIAPProduct(IAPProduct.IAP_DivaPack, (product) => { });
            });



#if UNITY_IOS
            _restorePurchaseButton.onClick.AddListener(() =>
            {
                FaithIAPManager.Instance.RestorePurchase((iapProducts) =>
                {
                    int numberOfRestorePurchasedProduct = iapProducts.Count;
                    for (int i = 0; i < numberOfRestorePurchasedProduct; i++) {

                        FaithIAPLogger.Log(
                            string.Format("({0}) : Name = {1}, ID = {2}\n",
                            i,
                            FaithIAPManager.Instance.ProductName(iapProducts[i]),
                            FaithIAPManager.Instance.ProductID(iapProducts[i])
                            ));
                    }
                });
            });
            _restorePurchaseButton.gameObject.SetActive(true);
#else
            _restorePurchaseButton.gameObject.SetActive(false);
#endif

        }

#endregion
    }
#endif
}

