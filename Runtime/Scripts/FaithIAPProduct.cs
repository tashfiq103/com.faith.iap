
namespace com.faith.iap
{
    using UnityEngine;
    using UnityEngine.Purchasing;

    [System.Serializable]
    public class FaithIAPProduct
    {
        public string ProductID
        {

            get
            {
#if UNITY_ANDROID
            return _productIdAndroid;
#elif UNITY_IOS
                return _productIdIOS;
#else
        return "InvalidPlatform";
#endif
            }
        }
        public ProductType ProductIAPType { get { return _productType; } }

        public string ProductName { get { return _productName; } }
        public string ProductDescription { get { return _productDescription; } }
        public float ProductPrice { get { return _productPrice; } }

        [Header("Settings   :   Basic")]
        [SerializeField] private string _productIdAndroid;
        [SerializeField] private string _productIdIOS;
        [SerializeField] private ProductType _productType;

        [Header("Settings   :   Optional")]
        [SerializeField] private string _productName;
        [SerializeField] private string _productDescription;
        [SerializeField] private float _productPrice;
    }

}

