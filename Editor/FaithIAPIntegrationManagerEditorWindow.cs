namespace com.faith.iap
{
#if UNITY_EDITOR && FaithIAP

    using UnityEditor;
    using UnityEngine;
    using UnityEngine.Events;

    public class FaithIAPIntegrationManagerEditorWindow : EditorWindow
    {
        #region Private Variables

        private const string _linkForDownload = "https://github.com/tashfiq103/com.faith.sdk.analytics";
        private const string _linkForDocumetation = "https://github.com/tashfiq103/com.faith.sdk.analytics/blob/main/README.md";

        private const int _widthOfGenerateEnumButton = 120;
        private const int _widthOfAddIAPProductButton = 50;

        private static EditorWindow _reference;

        private bool                _IsInformationFetched = false;
        private Vector2             _scrollPosition;

        private GUIStyle            _settingsTitleStyle;
        private GUIStyle            _hyperlinkStyle;

        private FaithIAPConfiguretionInfo _faithIAPConfiguretionInfo;
        private SerializedObject _serializedFaithIAPConfiguretionInfo;

        private GUIContent _generalSettingContent;
        private GUIContent _iapSettingContent;
        private GUIContent _debuggingSettingContent;

        private SerializedProperty _generateProductId;

        private SerializedProperty _showGeneralSettings;
        private SerializedProperty _showIAPSettings;
        private SerializedProperty _showDebuggingSettings;

        private SerializedProperty _showIAPLogInConsole;

        private SerializedProperty _infoLogColor;
        private SerializedProperty _warningLogColor;
        private SerializedProperty _errorLogColor;

        #endregion

        #region Editor

        [MenuItem("Faith/FaithIAP Integration Manager")]
        public static void Create() {

            if (_reference == null)
            {
                _reference = GetWindow<FaithIAPIntegrationManagerEditorWindow>("FaithIAP Integration Manager", typeof(FaithIAPIntegrationManagerEditorWindow));
                _reference.minSize = new Vector2(340, 240);
            }
            else {
                _reference.Show();
            }

            _reference.Focus();
        }

        private void OnEnable()
        {
            FetchAllTheReference();
        }

        private void OnDisable()
        {
            _IsInformationFetched = false;
            EnforceEnumGeneration();
        }

        private void OnFocus()
        {
            FetchAllTheReference();
        }

        private void OnLostFocus()
        {
            _IsInformationFetched = false;
            EnforceEnumGeneration();
        }

        private void OnGUI()
        {
            if (!_IsInformationFetched)
            {
                FetchAllTheReference();
                _IsInformationFetched = true;
            }

            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition, false, false);
            {
                EditorGUILayout.Space();

                EditorGUI.indentLevel += 1;
                {
                    GeneralSettingGUI();

                    EditorGUILayout.Space();
                    IAPSettingGUI();

                    EditorGUILayout.Space();
                    DebuggingSettingsGUI();
                }
                EditorGUI.indentLevel -= 1;
            }
            EditorGUILayout.EndScrollView();
        }

        #endregion

        #region CustomGUI

        private void DrawHeaderGUI(string title, ref GUIContent gUIContent, ref GUIStyle gUIStyle, ref SerializedProperty serializedProperty, UnityAction PostGUI = null, float widthAdjustment = 0)
        {
            EditorGUILayout.BeginHorizontal(GUI.skin.box);
            {
                if (GUILayout.Button(gUIContent, gUIStyle, GUILayout.Width(EditorGUIUtility.currentViewWidth - widthAdjustment)))
                {
                    serializedProperty.boolValue = !serializedProperty.boolValue;
                    serializedProperty.serializedObject.ApplyModifiedProperties();

                    gUIContent = new GUIContent(
                        "[" + (!serializedProperty.boolValue ? "+" : "-") + "] " + title
                    );
                }

                PostGUI?.Invoke();
            }
            EditorGUILayout.EndHorizontal();
        }


        private void GeneralSettingGUI()
        {
            DrawHeaderGUI("General", ref _generalSettingContent, ref _settingsTitleStyle, ref _showGeneralSettings);

            if (_showGeneralSettings.boolValue)
            {

                EditorGUI.indentLevel += 1;
                {
                    EditorGUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.LabelField("Reference/Link", GUILayout.Width(FaithIAPConfiguretionInfo.EDITOR_LABEL_WIDTH + 30));
                        if (GUILayout.Button("Download", _hyperlinkStyle, GUILayout.Width(100)))
                        {
                            Application.OpenURL(_linkForDownload);
                        }
                        if (GUILayout.Button("Documentation", _hyperlinkStyle, GUILayout.Width(100)))
                        {
                            Application.OpenURL(_linkForDocumetation);
                        }
                    }
                    EditorGUILayout.EndHorizontal();

                    
                }
                EditorGUI.indentLevel -= 1;
            }
        }

        private void IAPSettingGUI() {

            bool showGenerateIAPEnumButton = _generateProductId.boolValue;
            
            float adjustedWidth = _showIAPSettings.boolValue ? ((showGenerateIAPEnumButton ? _widthOfGenerateEnumButton : 0) + _widthOfAddIAPProductButton + 20) : 0;

            DrawHeaderGUI(
                "In App Purchase",
                ref _iapSettingContent,
                ref _settingsTitleStyle,
                ref _showIAPSettings,
                ()=> {

                    if (_showIAPSettings.boolValue) {

                        if (showGenerateIAPEnumButton)
                        {

                            if (GUILayout.Button("Generate IAPEnum", GUILayout.Width(_widthOfGenerateEnumButton)))
                            {
                                GenerateEnum();
                            }
                        }

                        if (GUILayout.Button("+Add ", GUILayout.Width(_widthOfAddIAPProductButton)))
                        {

                            SerializedProperty _iapProducts = _serializedFaithIAPConfiguretionInfo.FindProperty("_iapProducts");

                            int numberOfIAPProducts = _iapProducts.arraySize;
                            _iapProducts.arraySize = (numberOfIAPProducts + 1);
                            _iapProducts.serializedObject.ApplyModifiedProperties();

                            _serializedFaithIAPConfiguretionInfo.ApplyModifiedProperties();

                            SetIAPEnumGenerationStatus(true);
                        }

                    }

                },
                adjustedWidth);

            if (_showIAPSettings.boolValue)
            {
                if (_generateProductId.boolValue) {
                    EditorGUILayout.HelpBox("You must generate of the 'ProductID' before working with the iap. The 'ProductID' gets generated by their 'Unique-ProductName'", MessageType.Error);
                }

                int numberOfIAPProducts = _faithIAPConfiguretionInfo.IAPProducts.Length;
                for (int i = 0; i < numberOfIAPProducts; i++)
                {

                    SerializedProperty serializedIAPProduct = _serializedFaithIAPConfiguretionInfo.FindProperty("_iapProducts").GetArrayElementAtIndex(i);

                    SerializedProperty _showOnEditor = serializedIAPProduct.FindPropertyRelative("_showOnEditor");

                    SerializedProperty _productIdAndroid = serializedIAPProduct.FindPropertyRelative("_productIdAndroid");
                    SerializedProperty _productIdIOS = serializedIAPProduct.FindPropertyRelative("_productIdIOS");

                    SerializedProperty _productType = serializedIAPProduct.FindPropertyRelative("_productType");

                    SerializedProperty _productName = serializedIAPProduct.FindPropertyRelative("_productName");
                    SerializedProperty _productDescription = serializedIAPProduct.FindPropertyRelative("_productDescription");
                    SerializedProperty _productPrice = serializedIAPProduct.FindPropertyRelative("_productPrice");

                    EditorGUILayout.BeginVertical(GUI.skin.box);
                    {
                        EditorGUILayout.BeginHorizontal(GUI.skin.box);
                        {
                            string iapProductName = string.IsNullOrEmpty(_productName.stringValue) ? ("Product" + i) : _productName.stringValue;

                            _showOnEditor.boolValue = EditorGUILayout.Foldout(
                                _showOnEditor.boolValue,
                                iapProductName,
                                true);

                            if (GUILayout.Button("Remove", GUILayout.Width(80)))
                            {
                                _serializedFaithIAPConfiguretionInfo.FindProperty("_iapProducts").DeleteArrayElementAtIndex(i);
                                _serializedFaithIAPConfiguretionInfo.ApplyModifiedProperties();

                                SetIAPEnumGenerationStatus(true);

                                break;
                            }

                            EditorGUILayout.LabelField("", GUILayout.Width(5));

                        }
                        EditorGUILayout.EndHorizontal();

                        if (_showOnEditor.boolValue)
                        {

                            EditorGUI.indentLevel += 2;
                            {
                                EditorGUI.BeginChangeCheck();
                                EditorGUILayout.PropertyField(_productName);
                                if (EditorGUI.EndChangeCheck())
                                {

                                    SetIAPEnumGenerationStatus(true);
                                }

                                EditorGUILayout.PropertyField(_productIdAndroid);
                                EditorGUILayout.PropertyField(_productIdIOS);
                                EditorGUILayout.PropertyField(_productType);

                                

                                EditorGUILayout.PropertyField(_productDescription);
                                EditorGUILayout.PropertyField(_productPrice);
                            }
                            EditorGUI.indentLevel -= 2;
                        }
                    }
                    EditorGUILayout.EndVertical();
                }
            }
        }

        private void DebuggingSettingsGUI()
        {

            DrawHeaderGUI("Debugging", ref _debuggingSettingContent, ref _settingsTitleStyle, ref _showDebuggingSettings);

            if (_showDebuggingSettings.boolValue)
            {
                EditorGUI.indentLevel += 1;

                EditorGUILayout.BeginVertical();
                {
                    EditorGUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.LabelField(_showIAPLogInConsole.displayName, GUILayout.Width(FaithIAPConfiguretionInfo.EDITOR_LABEL_WIDTH));
                        EditorGUI.BeginChangeCheck();
                        _showIAPLogInConsole.boolValue = EditorGUILayout.Toggle(_showIAPLogInConsole.boolValue);
                        if (EditorGUI.EndChangeCheck())
                        {
                            _showIAPLogInConsole.serializedObject.ApplyModifiedProperties();
                        }
                    }
                    EditorGUILayout.EndHorizontal();


                    EditorGUILayout.BeginHorizontal(GUI.skin.box);
                    {
                        EditorGUI.BeginChangeCheck();
                        EditorGUILayout.PropertyField(_infoLogColor);
                        if (EditorGUI.EndChangeCheck())
                        {

                            _infoLogColor.serializedObject.ApplyModifiedProperties();
                        }

                        EditorGUI.BeginChangeCheck();
                        EditorGUILayout.PropertyField(_warningLogColor);
                        if (EditorGUI.EndChangeCheck())
                        {
                            _warningLogColor.serializedObject.ApplyModifiedProperties();
                        }

                        EditorGUI.BeginChangeCheck();
                        EditorGUILayout.PropertyField(_errorLogColor);
                        if (EditorGUI.EndChangeCheck())
                        {
                            _errorLogColor.serializedObject.ApplyModifiedProperties();
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndVertical();

                EditorGUI.indentLevel -= 1;
            }
        }


        #endregion

        #region Configuretion

        private void GenerateEnum() {

            SerializedProperty _iapProducts = _serializedFaithIAPConfiguretionInfo.FindProperty("_iapProducts");
            int numberOfIAPProducts = _iapProducts.arraySize;

            string[] enumValue = new string[numberOfIAPProducts + 1];

            for (int i = 0; i < numberOfIAPProducts; i++)
            {
                enumValue[i] = "IAP_" + FaithIAPEditorUtility.TruncateAllWhiteSpace(_iapProducts.GetArrayElementAtIndex(i).FindPropertyRelative("_productName").stringValue);
            }
            enumValue[numberOfIAPProducts] = "None";

            FaithIAPEditorUtility.GenerateEnum(
                "Assets/Faith/com.faith.iap/Runtime/Scripts/IAPProduct.cs",
                "com.faith.iap",
                "IAPProduct", enumValue);

            SetIAPEnumGenerationStatus(false);
        }

        private void FetchAllTheReference() {

            _faithIAPConfiguretionInfo              = Resources.Load<FaithIAPConfiguretionInfo>("FaithIAPConfiguretionInfo");
            _serializedFaithIAPConfiguretionInfo    = new SerializedObject(_faithIAPConfiguretionInfo);

            _generateProductId                      = _serializedFaithIAPConfiguretionInfo.FindProperty("_generateProductId");

            _showGeneralSettings                    = _serializedFaithIAPConfiguretionInfo.FindProperty("_showGeneralSettings");
            _showIAPSettings                        = _serializedFaithIAPConfiguretionInfo.FindProperty("_showIAPSettings");
            _showDebuggingSettings                  = _serializedFaithIAPConfiguretionInfo.FindProperty("_showDebuggingSettings");

            _generalSettingContent = new GUIContent(
                        "[" + (!_showGeneralSettings.boolValue ? "+" : "-") + "] General"
                    );
            _iapSettingContent = new GUIContent(
                       "[" + (!_showIAPSettings.boolValue ? "+" : "-") + "] " + "In App Purchase"
                   );

            _debuggingSettingContent = new GUIContent(
                        "[" + (!_showDebuggingSettings.boolValue ? "+" : "-") + "] Debugging"
                    );

            _settingsTitleStyle = new GUIStyle();
            _settingsTitleStyle.normal.textColor = Color.white;
            _settingsTitleStyle.fontStyle = FontStyle.Bold;
            _settingsTitleStyle.alignment = TextAnchor.MiddleLeft;

            _hyperlinkStyle = new GUIStyle();
            _hyperlinkStyle.normal.textColor = new Color(50 / 255.0f, 139 / 255.0f, 217 / 255.0f);
            _hyperlinkStyle.fontStyle = FontStyle.BoldAndItalic;
            _hyperlinkStyle.wordWrap = true;
            _hyperlinkStyle.richText = true;

            _showIAPLogInConsole = _serializedFaithIAPConfiguretionInfo.FindProperty("_showIAPLogInConsole");

            _infoLogColor = _serializedFaithIAPConfiguretionInfo.FindProperty("_infoLogColor");
            _warningLogColor = _serializedFaithIAPConfiguretionInfo.FindProperty("_warningLogColor");
            _errorLogColor = _serializedFaithIAPConfiguretionInfo.FindProperty("_errorLogColor");
        }

        private void EnforceEnumGeneration() {

            if (_generateProductId.boolValue) {

                if (EditorUtility.DisplayDialog(
                    "Must Recompile",
                    "In reference the IAPProduct using 'EnumReference', we must re-write the enum class for IAPProducts",
                    "Sure")) {

                    GenerateEnum();
                }
            }
        }

        private void SetIAPEnumGenerationStatus(bool status) {

            _generateProductId.boolValue = status;
            _generateProductId.serializedObject.ApplyModifiedProperties();
        }

        #endregion

    }

#endif
}

