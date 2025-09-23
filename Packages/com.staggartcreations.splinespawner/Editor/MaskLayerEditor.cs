// Spline Spawner by Staggart Creations (http://staggart.xyz)
// COPYRIGHT PROTECTED UNDER THE UNITY ASSET STORE EULA (https://unity.com/legal/as-terms)
//  • Copying or referencing source code for the production of new asset store, or public, content is strictly prohibited!
//  • Uploading this file to a public GitHub repository will subject it to an automated DMCA takedown request.

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using sc.splines.spawner.runtime;

namespace sc.splines.spawner.editor
{
    public class MaskLayerEditor : Editor
    {
        public static int LayerCount => MaskLayerSettings.instance.LayerNames.Length;
            
        public static string IndexToName(int index)
        {
            return MaskLayerSettings.instance.LayerNames[index];
        }

        public static string LayersToReadableList(int layerMask)
        {
            if (layerMask == 0) return string.Empty;

            List<string> names = new List<string>();
            for (int i = 0; i < LayerCount; i++)
            {
                if (SplineSpawnerMask.ContainsLayer(layerMask, i))
                {
                    names.Add(IndexToName(i));
                }
            }

            return names.Count > 0 ? $"({string.Join(", ", names)})" : string.Empty;
        }
        
        [FilePath(FILE_PATH, FilePathAttribute.Location.ProjectFolder)]
        public class MaskLayerSettings : ScriptableSingleton<MaskLayerSettings>
        {
            private const string FILE_PATH = "ProjectSettings/SplineSpawnerMaskLayers.asset";
            
            [SerializeField]
            public string[] LayerNames = new[]
            {
                "Layer 1", "Layer 2", "Layer 3", "Layer 4", "Layer 5", "Layer 6", "Layer 7", "Layer 8"
            };
            
            public void Save()
            {
                Save(true);
            }
        }
        
        [CustomPropertyDrawer(typeof(SplineSpawnerMask.MaskLayerAttribute))]
        public class MaskLayerDrawer : PropertyDrawer
        {
            private const float width = 200f;
            public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
            {
                if (property.propertyType == SerializedPropertyType.Integer)
                {
                    var maskAttribute = (SplineSpawnerMask.MaskLayerAttribute)attribute;
                    
                    float labelWidth = EditorGUIUtility.labelWidth;
                    
                    var fieldRect = new Rect(position.x, position.y,labelWidth + width, position.height);

                    if (maskAttribute.multiSelect)
                    {
                        property.intValue = EditorGUI.MaskField(fieldRect, label, property.intValue, MaskLayerSettings.instance.LayerNames);
                    }
                    else
                    {
                        property.intValue = EditorGUI.Popup(fieldRect, label.text, property.intValue, MaskLayerSettings.instance.LayerNames);
                    }

                    if (maskAttribute.showEditButton)
                    {
                        var buttonRect = new Rect(position.x + labelWidth + width, position.y, 45f, position.height);
                        if (GUI.Button(buttonRect, "Edit"))
                        {
                            Rect windowRect = EditorGUILayout.GetControlRect();
                            MaskLayerNamesEditorWindow.OpenWindow(windowRect);
                        }
                    }
                }
                else
                {
                    EditorGUI.LabelField(position, label.text, "The [MaskField] needs to be used with an integer.");
                }
            }
        }

        [CustomEditor(typeof(MaskLayerSettings))]
        private class MaskLayerSettingsEditor : Editor
        {
            private MaskLayerSettings settings;
            private SerializedProperty LayerNames;
            
            private Vector2 scrollPos;
            private void OnEnable()
            {
                settings = (MaskLayerSettings)target;
                LayerNames = serializedObject.FindProperty("LayerNames");
            }

            public override void OnInspectorGUI()
            {
                serializedObject.Update();
                EditorGUI.BeginChangeCheck();

                LayerNames.isExpanded = true;
                
                scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
                EditorGUILayout.PropertyField(LayerNames);
                EditorGUILayout.EndScrollView();
                
                if (EditorGUI.EndChangeCheck())
                {
                    serializedObject.ApplyModifiedProperties();
                    
                    settings.Save();
                }
            }
        }
        
        public class MaskLayerNamesEditorWindow : EditorWindow
        {
            private const float width = 300f;
            private const float height = 225f;
            
            private Vector2 scrollPos;
            private Editor editor;
            
            public static void OpenWindow(Rect position)
            {
                var window = GetWindow<MaskLayerNamesEditorWindow>(true, "Edit Mask Layer Names");
                window.minSize = new Vector2(width, height);
                window.maxSize = new Vector2(width, height);
                
                Vector2 mousePos = GUIUtility.GUIToScreenPoint(Event.current.mousePosition);
                window.position = new Rect(mousePos.x - (width * 0.5f), mousePos.y + 10f, 0, 0);
                
                window.Show();
            }

            public void OnEnable()
            {
                CreateCachedEditor(MaskLayerSettings.instance, typeof(MaskLayerSettingsEditor), ref editor);
            }

            private void OnGUI()
            {
                editor.OnInspectorGUI();
            }
        }
    }
}
