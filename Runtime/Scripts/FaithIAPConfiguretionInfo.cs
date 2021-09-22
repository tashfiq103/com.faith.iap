﻿namespace com.faith.iap
{

    using UnityEngine;

    //[CreateAssetMenu(menuName = "AP/IAP", fileName = "APIAPConfiguretionInfo")]
    public class FaithIAPConfiguretionInfo : ScriptableObject
    {
        public bool ShowAPSdkLogInConsole { get { return _showAPSdkLogInConsole; } }

        public Color InfoLogColor { get { return _infoLogColor; } }
        public Color WarningLogColor { get { return _warningLogColor; } }
        public Color ErrorLogColor { get { return _errorLogColor; } }

        public float DelayOnRestorePurchaseAndroid { get { return _delayOnRestorePurchaseAndroid; } }
        public FaithIAPProduct[] IAPProducts { get { return _iapProducts; } }


#if UNITY_EDITOR

        [SerializeField] private bool _generateProductId = false;

#endif

        

        [SerializeField] private bool _showAPSdkLogInConsole = true;

        [SerializeField] private Color _infoLogColor = Color.cyan;
        [SerializeField] private Color _warningLogColor = Color.yellow;
        [SerializeField] private Color _errorLogColor = Color.red;

        [SerializeField, Range(1, 5)] private float _delayOnRestorePurchaseAndroid = 1f;

        [SerializeField] private FaithIAPProduct[] _iapProducts;
    }

}

