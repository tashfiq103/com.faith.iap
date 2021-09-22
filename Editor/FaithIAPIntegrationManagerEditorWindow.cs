namespace com.faith.iap
{
#if UNITY_EDITOR && FaithIAP

    using UnityEngine;
    using UnityEditor;

    public class FaithIAPIntegrationManagerEditorWindow : EditorWindow
    {
        #region Private Variables

        private const string _linkForDownload = "https://github.com/tashfiq103/com.faith.sdk.analytics";
        private const string _linkForDocumetation = "https://github.com/tashfiq103/com.faith.sdk.analytics/blob/main/README.md";

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
        }

        private void OnFocus()
        {
            FetchAllTheReference();
        }

        private void OnLostFocus()
        {
            _IsInformationFetched = false;
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
                }
                EditorGUI.indentLevel -= 1;
            }
            EditorGUILayout.EndScrollView();
        }

        #endregion

        #region CustomGUI

        private void DrawHeaderGUI(string title, ref GUIContent gUIContent, ref GUIStyle gUIStyle, ref SerializedProperty serializedProperty)
        {

            EditorGUILayout.BeginVertical(GUI.skin.box);
            {
                if (GUILayout.Button(gUIContent, gUIStyle, GUILayout.Width(EditorGUIUtility.currentViewWidth)))
                {
                    serializedProperty.boolValue = !serializedProperty.boolValue;
                    serializedProperty.serializedObject.ApplyModifiedProperties();

                    gUIContent = new GUIContent(
                        "[" + (!serializedProperty.boolValue ? "+" : "-") + "] " + title
                    );
                }
            }
            EditorGUILayout.EndVertical();
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


        #endregion

        #region Configuretion

        private void FetchAllTheReference() {

            _faithIAPConfiguretionInfo              = Resources.Load<FaithIAPConfiguretionInfo>("FaithIAPConfiguretionInfo");

            Debug.Log(_faithIAPConfiguretionInfo.name);

            _serializedFaithIAPConfiguretionInfo    = new SerializedObject(_faithIAPConfiguretionInfo);



            _showGeneralSettings                    = _serializedFaithIAPConfiguretionInfo.FindProperty("_showGeneralSettings");
            _showIAPSettings                        = _serializedFaithIAPConfiguretionInfo.FindProperty("_showIAPSettings");
            _showDebuggingSettings                  = _serializedFaithIAPConfiguretionInfo.FindProperty("_showDebuggingSettings");

            _generalSettingContent = new GUIContent(
                        "[" + (!_showGeneralSettings.boolValue ? "+" : "-") + "] General"
                    );
            _iapSettingContent = new GUIContent(
                       "[" + (!_showIAPSettings.boolValue ? "+" : "-") + "] " + "IAP"
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

        #endregion

    }

#endif
}

