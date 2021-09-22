﻿namespace com.faith.iap
{
#if FaithIAP
    using UnityEngine;
    using UnityEditor;
    using System.IO;
    using System.Collections.Generic;

    public class APIAPEditorUtility : Editor
    {
#region Editor Module   :   GUI

        internal static void ShowScriptReference(SerializedObject serializedObject)
        {

            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Script"));
            EditorGUI.EndDisabledGroup();
        }

        internal static void DrawHorizontalLine()
        {
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        }

        internal static void DrawHorizontalLineOnGUI(Rect rect)
        {
            EditorGUI.LabelField(rect, "", GUI.skin.horizontalSlider);
        }

        internal static void DrawSettingsEditor(Object settings, System.Action OnSettingsUpdated, ref bool foldout, ref Editor editor, int indentValue = 0)
        {

            if (settings != null)
            {

                using (var check = new EditorGUI.ChangeCheckScope())
                {

                    foldout = EditorGUILayout.InspectorTitlebar(foldout, settings);

                    if (foldout)
                    {

                        CreateCachedEditor(settings, null, ref editor);

                        EditorGUI.indentLevel += indentValue;
                        editor.OnInspectorGUI();
                        EditorGUI.indentLevel -= indentValue;

                        if (check.changed)
                        {

                            if (OnSettingsUpdated != null)
                            {

                                OnSettingsUpdated.Invoke();
                            }
                        }
                    }
                }
            }

        }

#endregion

#region Editor Module   :   Asset

        internal static List<T> GetAsset<T>(bool returnIfGetAny = false, params string[] directoryFilters)
        {

            return GetAsset<T>("t:" + typeof(T).ToString().Replace("UnityEngine.", ""), returnIfGetAny, directoryFilters);
        }

        internal static List<T> GetAsset<T>(string nameFilter, bool returnIfGetAny = false, params string[] directoryFilters)
        {

            List<T> listOfAsset = new List<T>();
            string[] GUIDs;
            if (directoryFilters == null) GUIDs = AssetDatabase.FindAssets(nameFilter);
            else GUIDs = AssetDatabase.FindAssets(nameFilter, directoryFilters);

            foreach (string GUID in GUIDs)
            {

                string assetPath = AssetDatabase.GUIDToAssetPath(GUID);
                listOfAsset.Add((T)System.Convert.ChangeType(AssetDatabase.LoadAssetAtPath(assetPath, typeof(T)), typeof(T)));
                if (returnIfGetAny)
                    break;
            }

            return listOfAsset;
        }

#endregion

#region Editor Module   :   CodeGenerator

        internal static void GenerateEnum(string path, string nameSpace, string nameOfEnum, params string[] enumValue)
        {

            string code = "";

            code += "namespace "+ nameSpace + "\n{\n\t";
            code += "public enum " + nameOfEnum + "\n\t{\n";

            //code += string.Format("namespace {0}\n{\n\t", nameSpace);
            //code += string.Format("public enum {0}\n{\n", nameOfEnum);

            int numberOfEnumValue = enumValue.Length;

            for (int i = 0; i < numberOfEnumValue; i++)
            {

                code += string.Format("\t\t{0}{1}\n", enumValue[i], (i < numberOfEnumValue - 1) ? "," : "");
            }

            code += "\t}\n}";

            using (StreamWriter streamWriter = new StreamWriter(path))
            {
                streamWriter.Write(code);
            }

            AssetDatabase.Refresh();
        }

#endregion

#region Editor Module   :   UnityTechnology

        internal static bool DropDownToggle(ref bool toggled, GUIContent content, GUIStyle toggleButtonStyle)
        {
            Rect toggleRect = GUILayoutUtility.GetRect(content, toggleButtonStyle);
            Rect arrowRightRect = new Rect(toggleRect.xMax - toggleButtonStyle.padding.right, toggleRect.y, toggleButtonStyle.padding.right, toggleRect.height);
            bool clicked = EditorGUI.DropdownButton(arrowRightRect, GUIContent.none, FocusType.Passive, GUIStyle.none);

            if (!clicked)
            {
                toggled = GUI.Toggle(toggleRect, toggled, content, toggleButtonStyle);
            }

            return clicked;
        }

        //Extended  :   Tashfiq


#endregion

#region Editor Module   :   Utility

        public static string TruncateAllWhiteSpace(string t_ModifiableString)
        {

            string[] t_SplitByWhiteSpace = t_ModifiableString.Split(' ');
            string t_NewString = "";
            foreach (string t_SubString in t_SplitByWhiteSpace)
            {

                t_NewString += t_SubString;
            }

            return t_NewString;
        }

#endregion
    }
#endif
}

