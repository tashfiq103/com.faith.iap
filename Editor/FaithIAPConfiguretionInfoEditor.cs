namespace com.faith.iap
{
#if FaithIAP

    using UnityEngine;
    using UnityEditor;

    [CustomEditor(typeof(FaithIAPConfiguretionInfo))]
    public class FaithIAPConfiguretionInfoEditor : Editor
    {
#region Variables

        private FaithIAPConfiguretionInfo _reference;

        private SerializedProperty _generateProductId;

        private SerializedProperty _showAPSdkLogInConsole;

        private SerializedProperty _infoLogColor;
        private SerializedProperty _warningLogColor;
        private SerializedProperty _errorLogColor;

        private SerializedProperty _delayOnRestorePurchaseAndroid;

        private SerializedProperty _iapProducts;

#endregion

#region Editor

        private void OnEnable()
        {
            _reference = (FaithIAPConfiguretionInfo)target;

            if (_reference == null)
                return;

            _generateProductId = serializedObject.FindProperty("_generateProductId");

            _showAPSdkLogInConsole = serializedObject.FindProperty("_showAPSdkLogInConsole");

            _infoLogColor = serializedObject.FindProperty("_infoLogColor");
            _warningLogColor = serializedObject.FindProperty("_warningLogColor");
            _errorLogColor = serializedObject.FindProperty("_errorLogColor");

            _delayOnRestorePurchaseAndroid = serializedObject.FindProperty("_delayOnRestorePurchaseAndroid");
            _iapProducts = serializedObject.FindProperty("_iapProducts");
        }

        public override void OnInspectorGUI()
        {
            APIAPEditorUtility.ShowScriptReference(serializedObject);

            serializedObject.Update();

            EditorGUILayout.PropertyField(_showAPSdkLogInConsole);

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(_infoLogColor);
            EditorGUILayout.PropertyField(_warningLogColor);
            EditorGUILayout.PropertyField(_errorLogColor);

            if (_generateProductId.boolValue) {

                CheckIfAnyProductNameNullOrEmpty();

                APIAPEditorUtility.DrawHorizontalLine();
                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.HelpBox("Generate Unique EnumID for IAP Product", MessageType.Info);
                    if (GUILayout.Button("Generate")) {

                        int numberOfProduct = _reference.IAPProducts.Length;
                        string[] enumValue = new string[numberOfProduct + 1];

                        for (int i = 0; i < numberOfProduct; i++) {
                            enumValue[i] = APIAPEditorUtility.TruncateAllWhiteSpace(_reference.IAPProducts[i].ProductName);
                        }
                        enumValue[numberOfProduct] = "None";

                        APIAPEditorUtility.GenerateEnum(
                            "Assets/Faith/com.faith.iap/Runtime/Scripts/IAPProduct.cs",
                            "com.faith.iap",
                            "IAPProduct", enumValue);

                        _generateProductId.boolValue = false;
                        _generateProductId.serializedObject.ApplyModifiedProperties();
                    }
                }
                EditorGUILayout.EndHorizontal();

            }

            APIAPEditorUtility.DrawHorizontalLine();

            EditorGUILayout.PropertyField(_delayOnRestorePurchaseAndroid);
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(_iapProducts, true);
            if (EditorGUI.EndChangeCheck()) {

                _iapProducts.serializedObject.ApplyModifiedProperties();

                _generateProductId.boolValue = true;
                _generateProductId.serializedObject.ApplyModifiedProperties();
            }

            serializedObject.ApplyModifiedProperties();
        }

#endregion

#region Configuretion

        private void CheckIfAnyProductNameNullOrEmpty() {

            int numberOfProduct = _reference.IAPProducts.Length;
            for (int i = 0; i < numberOfProduct; i++) {

                if (string.IsNullOrEmpty(_reference.IAPProducts[i].ProductName))
                {
                    SerializedProperty _productNameProperty = _iapProducts.GetArrayElementAtIndex(i).FindPropertyRelative("_productName");
                    _productNameProperty.stringValue = "Product" + i;
                    _productNameProperty.serializedObject.ApplyModifiedProperties();

                    _iapProducts.serializedObject.ApplyModifiedProperties();
                };
            }
        }

#endregion
    }
#endif
}

