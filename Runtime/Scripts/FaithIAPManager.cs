namespace com.faith.iap
{
#if FaithIAP

    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Events;

    using UnityEngine.Purchasing;

    public class FaithIAPManager : MonoBehaviour, IStoreListener
    {
        #region Variables

        public static FaithIAPManager Instance;

        public bool IsIAPInitialized { get; private set; } = false;
        public bool IsIAPRestored { get; private set; } = false;
        public List<IAPProduct> ListOfPurchasedIAPProduct { get; private set; } = new List<IAPProduct>();

        public IStoreController IAPStoreController { get; private set; }
        public IExtensionProvider IAPExtensionProvider { get; private set; }

        public FaithIAPConfiguretionInfo IAPConfiguretionInfo { get; private set; }
        public ConfigurationBuilder IAPConfiguretionBuilder { get; private set; }

        private UnityAction<Product> _OnPurchasedSucceed;
        private UnityAction<Product> _OnPurchasedFailed;

        #endregion

        #region Configuretion

        private int GetIAPProductIndex(IAPProduct iapProduct) {

            return iapProduct == IAPProduct.None ? -1 : (int)iapProduct;
        }

        private void UpdateListOfPurchasedIAPProduct() {

            ListOfPurchasedIAPProduct = new List<IAPProduct>();

            int numberOfIAPItem = IAPConfiguretionInfo.IAPProducts.Length;
            for (int i = 0; i < numberOfIAPItem; i++)
            {

                if (IAPConfiguretionInfo.IAPProducts[i].ProductIAPType != ProductType.Consumable)
                {
                    if (IAPStoreController.products.WithID(IAPConfiguretionInfo.IAPProducts[i].ProductID).hasReceipt)
                    {
                        ListOfPurchasedIAPProduct.Add((IAPProduct)i);
                    }
                }
            }

            IsIAPRestored = true;
        }

        private IEnumerator RestorePurchaseAndroid()
        {
            yield return new WaitForSeconds(IAPConfiguretionInfo.DelayOnRestorePurchaseAndroid);

            UpdateListOfPurchasedIAPProduct();

            StopCoroutine(RestorePurchaseAndroid());
        }



        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void OnGameStart()
        {
            if(Instance == null)
            {
                GameObject FaithIAPManagerObject = new GameObject("FaithIAPManager");
                Instance = FaithIAPManagerObject.AddComponent<FaithIAPManager>();
                DontDestroyOnLoad(FaithIAPManagerObject);

                Instance.IAPConfiguretionInfo = Resources.Load<FaithIAPConfiguretionInfo>("FaithIAPConfiguretionInfo");

                Instance.IAPConfiguretionBuilder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
                foreach (FaithIAPProduct iapProduct in Instance.IAPConfiguretionInfo.IAPProducts)
                {
                    Instance.IAPConfiguretionBuilder.AddProduct(iapProduct.ProductID, iapProduct.ProductIAPType);
                }

                UnityPurchasing.Initialize(Instance, Instance.IAPConfiguretionBuilder);
            }
        }

#endregion

#region Interface : IStoreListener

        public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            IAPStoreController = controller;
            IAPExtensionProvider = extensions;

            IsIAPInitialized = true;

#if UNITY_ANDROID
            StartCoroutine(RestorePurchaseAndroid());
#endif

            FaithIAPLogger.Log(string.Format("{0} Initialized", "IAP"));
        }

        public void OnInitializeFailed(InitializationFailureReason error)
        {
            FaithIAPLogger.Log(string.Format("{0} Failed Initialization. Error = {1}", "IAP", error));
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
        {
            FaithIAPLogger.Log(string.Format("Purchased Failed = {0}, Reason = {1}", product.definition.id, failureReason));
            _OnPurchasedFailed?.Invoke(product);
        }

        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent)
        {
            FaithIAPLogger.Log(string.Format("Purchased Succeed = {0}", purchaseEvent.purchasedProduct.definition.id));
            _OnPurchasedSucceed?.Invoke(purchaseEvent.purchasedProduct);

            return PurchaseProcessingResult.Complete;
        }

        #endregion

        #region Public Callback :   Product Information

        public string ProductID(IAPProduct iapProduct) {

            int productIndex = GetIAPProductIndex(iapProduct);
            if (productIndex == -1)
            {
                FaithIAPLogger.LogError(string.Format("Invalid IAPProduct = {0}", iapProduct));
                return null;
            }

            return IAPConfiguretionInfo.IAPProducts[productIndex].ProductID;
        }

        public ProductType ProductIAPID(IAPProduct iapProduct)
        {
            int productIndex = GetIAPProductIndex(iapProduct);
            if (productIndex == -1)
            {
                FaithIAPLogger.LogError(string.Format("Invalid IAPProduct = {0}", iapProduct));
                return ProductType.Consumable;
            }

            return IAPConfiguretionInfo.IAPProducts[productIndex].ProductIAPType;
        }

        public string ProductName(IAPProduct iapProduct)
        {

            int productIndex = GetIAPProductIndex(iapProduct);
            if (productIndex == -1)
            {
                FaithIAPLogger.LogError(string.Format("Invalid IAPProduct = {0}", iapProduct));
                return null;
            }

            return IAPConfiguretionInfo.IAPProducts[productIndex].ProductName;
        }

        public string ProductDescription(IAPProduct iapProduct)
        {

            int productIndex = GetIAPProductIndex(iapProduct);
            if (productIndex == -1)
            {
                FaithIAPLogger.LogError(string.Format("Invalid IAPProduct = {0}", iapProduct));
                return null;
            }

            return IAPConfiguretionInfo.IAPProducts[productIndex].ProductDescription;
        }

        public float ProductPrice(IAPProduct iapProduct)
        {

            int productIndex = GetIAPProductIndex(iapProduct);
            if (productIndex == -1)
            {
                FaithIAPLogger.LogError(string.Format("Invalid IAPProduct = {0}", iapProduct));
                return -1;
            }

            return IAPConfiguretionInfo.IAPProducts[productIndex].ProductPrice;
        }


        #endregion

        #region Public Callback :   PurchaseCall

        public void BuyIAPProduct(IAPProduct iapProduct, UnityAction<Product> OnPurchasedSucceed, UnityAction<Product> OnPurchasedFailed = null)
        {

            if (IsIAPInitialized)
            {

                int productIndex = GetIAPProductIndex(iapProduct);

                if (productIndex == -1)
                {
                    FaithIAPLogger.LogError(string.Format("Invalid IAPProduct = {0}", iapProduct.ToString()));
                    return;
                }

                _OnPurchasedSucceed = OnPurchasedSucceed;
                _OnPurchasedFailed = OnPurchasedFailed;

                string productId = IAPConfiguretionInfo.IAPProducts[productIndex].ProductID;
                Product product = IAPStoreController.products.WithID(productId);

                if (product == null)
                {
                    FaithIAPLogger.LogError(string.Format("No such product available with the following id = {0}. return value = null", productId));
                    _OnPurchasedFailed?.Invoke(product);
                    return;
                }

                if (!product.availableToPurchase)
                {
                    FaithIAPLogger.LogError(string.Format("product is not available for purchase = {0}", productId));
                    _OnPurchasedFailed?.Invoke(product);
                    return;
                }

                IAPStoreController.InitiatePurchase(product);
            }
            else
            {

                FaithIAPLogger.LogError("IAP is not initialized");
            }
        }

        public void RestorePurchase(UnityAction<List<IAPProduct>> OnRestorePurchase)
        {

            if (IsIAPInitialized)
            {

                FaithIAPLogger.Log("RestorePurchase : Start");

#if !UNITY_EDITOR && (UNITY_IOS || UNITY_STANDALONE_OSX)
                FaithIAPLogger.Log("RestorePurchase : Start");
                IAppleExtensions appleExtensions = IAPExtensionProvider.GetExtension<IAppleExtensions>();
                appleExtensions.RestoreTransactions((result) =>
                {
                    if (result)
                    {
                        UpdateListOfPurchasedIAPProduct();

                        OnRestorePurchase?.Invoke(ListOfPurchasedIAPProduct);
                    }
                    else {

                        FaithIAPLogger.LogError(string.Format("IAPRestore failed"));
                    }
                });
#else
                FaithIAPLogger.LogWarning(string.Format("'RestorePurchase' only available on {0}, {1}", "iOS", "MacOSX"));
#endif
            }
            else
            {

                FaithIAPLogger.LogError("IAP is not initialized");
            }
        }


        #endregion
    }

#endif

}

