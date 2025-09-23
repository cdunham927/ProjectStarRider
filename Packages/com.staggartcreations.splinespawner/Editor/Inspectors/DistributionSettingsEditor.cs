// Spline Spawner by Staggart Creations (http://staggart.xyz)
// COPYRIGHT PROTECTED UNDER THE UNITY ASSET STORE EULA (https://unity.com/legal/as-terms)
//  • Copying or referencing source code for the production of new asset store, or public, content is strictly prohibited!
//  • Uploading this file to a public GitHub repository will subject it to an automated DMCA takedown request.

using sc.splines.spawner.runtime;
using UnityEditor;
using UnityEngine;

namespace sc.splines.spawner.editor
{
    public class DistributionSettingsEditor
    {
        private SerializedProperty distributionSettings;

        class OnCurveEditor
        {
            private SerializedProperty settings;
            
            private SerializedProperty instanceCountMode;
            private SerializedProperty instanceCount;
        
            private SerializedProperty curveSpacingMode;
            private SerializedProperty curveSpacing;
            private SerializedProperty curveSpacingMinMax;
            private SerializedProperty startEndTrimming;

            private SerializedProperty rotateToFitAxis;
            
            public static OnCurveEditor Create(SerializedProperty property)
            {
                OnCurveEditor editor = new OnCurveEditor();
                editor.settings = property;

                editor.OnEnable();
            
                return editor;
            }
            
            private void OnEnable()
            {
                instanceCountMode = settings.FindPropertyRelative("instanceCountMode");
                instanceCount = settings.FindPropertyRelative("instanceCount");
            
                curveSpacingMode = settings.FindPropertyRelative("spacingMode");
                curveSpacing = settings.FindPropertyRelative("spacing");
                curveSpacingMinMax = settings.FindPropertyRelative("spacingMinMax");
                startEndTrimming = settings.FindPropertyRelative("startEndTrimming");
                rotateToFitAxis = settings.FindPropertyRelative("rotateToFitAxis");
            }

            public void OnInspectorGUI(ref bool changed)
            {
                EditorGUI.BeginChangeCheck();
                
                float dropdownWidth = EditorGUIUtility.labelWidth + EditorGUIUtility.fieldWidth + 100f;
                EditorGUILayout.PropertyField(instanceCountMode, GUILayout.MaxWidth(dropdownWidth));

                if (instanceCountMode.intValue == (int)DistributionSettings.InstanceCountMode.Specific)
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(instanceCount);
                    EditorGUI.indentLevel--;
                }
                
                EditorGUILayout.Separator();
                
                if (instanceCountMode.intValue == (int)DistributionSettings.InstanceCountMode.Auto)
                {
                    EditorGUILayout.PropertyField(curveSpacingMode, GUILayout.MaxWidth(dropdownWidth));
                    EditorGUI.indentLevel++;
                    if ((DistributionSettings.OnCurve.SpacingMode)curveSpacingMode.intValue == DistributionSettings.OnCurve.SpacingMode.Exact)
                    {
                        EditorGUILayout.PropertyField(curveSpacing, new GUIContent("Units", curveSpacing.tooltip));
                    }
                    else
                    {
                        EditorGUILayout.PropertyField(curveSpacingMinMax, new GUIContent("Units (Min/Max)", curveSpacingMinMax.tooltip));
                    }
                    EditorGUI.indentLevel--;
                }
                EditorGUILayout.Separator();
                
                EditorGUILayout.PropertyField(startEndTrimming, new GUIContent("Start-End Trimming", startEndTrimming.tooltip));
                
                EditorGUILayout.Separator();
                
                EditorGUILayout.PropertyField(rotateToFitAxis, new GUIContent("Rotate To Fit"), GUILayout.MaxWidth(dropdownWidth));
                
                changed = EditorGUI.EndChangeCheck();
            }
        }

        class InsideAreaEditor
        {
            private SerializedProperty settings;
            
            private SerializedProperty spacing;
            private SerializedProperty padding;
            private SerializedProperty overlapAccuracy;
            private SerializedProperty borderAccuracy;
            
            public static InsideAreaEditor Create(SerializedProperty property)
            {
                InsideAreaEditor editor = new InsideAreaEditor();
                editor.settings = property;

                editor.OnEnable();
            
                return editor;
            }

            void OnEnable()
            {
                spacing = settings.FindPropertyRelative("spacing");
                padding = settings.FindPropertyRelative("padding");
                overlapAccuracy = settings.FindPropertyRelative("overlapAccuracy");
                borderAccuracy = settings.FindPropertyRelative("borderAccuracy");
            }

            public void OnInspectorGUI(ref bool changed)
            {
                EditorGUI.BeginChangeCheck();
                
                EditorGUILayout.PropertyField(spacing);
                EditorGUILayout.PropertyField(padding);
                
                EditorGUILayout.Separator();
                
                EditorGUILayout.PropertyField(overlapAccuracy);
                EditorGUILayout.PropertyField(borderAccuracy);
                
                changed = EditorGUI.EndChangeCheck();
            }
        }

        class OnKnotEditor
        {
            private SerializedProperty settings;
            
            private SerializedProperty selection;
            private SerializedProperty linearOnly;
            private SerializedProperty mirrorLastRotation;
            private SerializedProperty range;
            private SerializedProperty linkedKnotFilter;
            
            public static OnKnotEditor Create(SerializedProperty property)
            {
                OnKnotEditor editor = new OnKnotEditor();
                editor.settings = property;

                editor.OnEnable();
            
                return editor;
            }
            
            void OnEnable()
            {
                selection = settings.FindPropertyRelative("selection");
                linearOnly = settings.FindPropertyRelative("linearOnly");
                mirrorLastRotation = settings.FindPropertyRelative("mirrorLastRotation");
                range = settings.FindPropertyRelative("range");
                linkedKnotFilter = settings.FindPropertyRelative("linkedKnotFilter");
            }
            
            public void OnInspectorGUI(ref bool changed)
            {
                EditorGUI.BeginChangeCheck();
                
                EditorGUILayout.PropertyField(selection);
                if ((DistributionSettings.OnKnots.Selection)selection.intValue == DistributionSettings.OnKnots.Selection.Range)
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(range);
                    EditorGUI.indentLevel--;
                }
                
                EditorGUILayout.Separator();
                
                EditorGUILayout.PropertyField(linearOnly);
                EditorGUILayout.PropertyField(mirrorLastRotation);
                
                EditorGUILayout.Separator();
                
                EditorGUILayout.PropertyField(linkedKnotFilter);
                
                changed = EditorGUI.EndChangeCheck();
            }
        }

        class RadialEditor
        {
            private SerializedProperty settings;
            
            private SerializedProperty radialSpacing;
            private SerializedProperty offset;
            private SerializedProperty spacing;
            private SerializedProperty center;
            
            public static RadialEditor Create(SerializedProperty property)
            {
                RadialEditor editor = new RadialEditor();
                editor.settings = property;

                editor.OnEnable();
            
                return editor;
            }
            
            void OnEnable()
            {
                radialSpacing = settings.FindPropertyRelative("radialSpacing");
                offset = settings.FindPropertyRelative("offset");
                spacing = settings.FindPropertyRelative("spacing");
                center = settings.FindPropertyRelative("center");
            }
            
            public void OnInspectorGUI(ref bool changed)
            {
                EditorGUI.BeginChangeCheck();
                
                EditorGUILayout.PropertyField(radialSpacing);
                EditorGUILayout.PropertyField(offset);
                EditorGUILayout.PropertyField(spacing);
                EditorGUILayout.PropertyField(center);
                
                changed = EditorGUI.EndChangeCheck();
            }
        }
        
        class GridEditor
        {
            private SerializedProperty settings;
    
            private SerializedProperty createRows;
            private SerializedProperty rowSpacing;
            private SerializedProperty spacingOnRows;
            
            private SerializedProperty createColumns;
            private SerializedProperty columnSpacing;
            private SerializedProperty spacingOnColumns;
            
            private SerializedProperty margin;
            private SerializedProperty angle;
            
            private SerializedProperty minimumLength;
            private SerializedProperty concaveSupport;
            private SerializedProperty accuracy;
    
            public static GridEditor Create(SerializedProperty property)
            {
                GridEditor editor = new GridEditor();
                editor.settings = property;

                editor.OnEnable();
    
                return editor;
            }
    
            void OnEnable()
            {
                createRows = settings.FindPropertyRelative("createRows");
                rowSpacing = settings.FindPropertyRelative("rowSpacing");
                spacingOnRows = settings.FindPropertyRelative("spacingOnRows");
                
                createColumns = settings.FindPropertyRelative("createColumns");
                columnSpacing = settings.FindPropertyRelative("columnSpacing");
                spacingOnColumns = settings.FindPropertyRelative("spacingOnColumns");
                
                margin = settings.FindPropertyRelative("margin");
                angle = settings.FindPropertyRelative("angle");
                
                minimumLength = settings.FindPropertyRelative("minimumLength");
                concaveSupport = settings.FindPropertyRelative("concaveSupport");
                accuracy = settings.FindPropertyRelative("accuracy");
            }
    
            public void OnInspectorGUI(ref bool changed)
            {
                EditorGUI.BeginChangeCheck();
                
                EditorGUILayout.Space(-17f);
                EditorGUILayout.BeginVertical();
                EditorGUI.BeginProperty(EditorGUILayout.GetControlRect(), new GUIContent("Rows", createRows.tooltip), createRows);
                createRows.boolValue = EditorGUILayout.ToggleLeft("Rows", createRows.boolValue);
                EditorGUI.EndProperty();
                EditorGUILayout.EndVertical();
                
                if (createRows.boolValue)
                {
                    EditorGUILayout.PropertyField(rowSpacing);
                    EditorGUILayout.PropertyField(spacingOnRows);
                }
                
                EditorGUILayout.Space(-17f);
                EditorGUILayout.BeginVertical();
                EditorGUI.BeginProperty(EditorGUILayout.GetControlRect(), new GUIContent("Columns", createColumns.tooltip), createColumns);
                createColumns.boolValue = EditorGUILayout.ToggleLeft("Columns", createColumns.boolValue);
                EditorGUI.EndProperty();
                EditorGUILayout.EndVertical();
                
                if (createColumns.boolValue)
                {
                    EditorGUILayout.PropertyField(columnSpacing);
                    EditorGUILayout.PropertyField(spacingOnColumns);
                }
                
                EditorGUILayout.Space();
                
                EditorGUILayout.PropertyField(margin);
                //EditorGUILayout.PropertyField(angle);
                EditorGUILayout.PropertyField(minimumLength);
                
                EditorGUILayout.Space();
                
                EditorGUILayout.LabelField("Precision", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(concaveSupport);
                EditorGUILayout.PropertyField(accuracy);

                changed = EditorGUI.EndChangeCheck();
            }
        }
        
        private SerializedProperty mode;
        private SerializedProperty seed;
        
        private SerializedProperty grid;
        
        OnCurveEditor onCurveEditor;
        InsideAreaEditor insideAreaEditor;
        OnKnotEditor onKnotsEditor;
        RadialEditor radialEditor;
        GridEditor gridEditor;

        public static DistributionSettingsEditor CreateEditor(SerializedProperty property)
        {
            DistributionSettingsEditor editor = new DistributionSettingsEditor();
            editor.distributionSettings = property;

            editor.OnEnable();
            
            return editor;
        }
        
        private void OnEnable()
        {
            mode = distributionSettings.FindPropertyRelative("mode");
            seed = distributionSettings.FindPropertyRelative("seed");
            
            onCurveEditor = OnCurveEditor.Create(distributionSettings.FindPropertyRelative("onCurve"));
            insideAreaEditor = InsideAreaEditor.Create(distributionSettings.FindPropertyRelative("insideArea"));
            onKnotsEditor = OnKnotEditor.Create(distributionSettings.FindPropertyRelative("onKnots"));
            radialEditor = RadialEditor.Create(distributionSettings.FindPropertyRelative("radial"));
            gridEditor = GridEditor.Create(distributionSettings.FindPropertyRelative("grid"));
        }

        public void OnInspectorGUI()
        {
            float dropdownWidth = EditorGUIUtility.labelWidth + EditorGUIUtility.fieldWidth + 100f;
            EditorGUI.BeginChangeCheck();
            
            EditorGUILayout.PropertyField(mode, GUILayout.MaxWidth(dropdownWidth));
            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.PropertyField(seed, GUILayout.MaxWidth(EditorGUIUtility.labelWidth + 60f));
                if (GUILayout.Button("Randomize", EditorStyles.miniButton, GUILayout.Width(80f)))
                {
                    seed.intValue = Random.Range(0, 99999);
                }
            }
            bool changed = false;
            
            if (mode.intValue == (int)DistributionSettings.DistributionMode.OnCurve)
            {
                onCurveEditor.OnInspectorGUI(ref changed);
            }

            EditorGUILayout.Separator();

            if ((DistributionSettings.DistributionMode)mode.enumValueIndex == DistributionSettings.DistributionMode.InsideArea)
            {
                insideAreaEditor.OnInspectorGUI(ref changed);
            }
            else if ((DistributionSettings.DistributionMode)mode.enumValueIndex == DistributionSettings.DistributionMode.OnKnots)
            {
                onKnotsEditor.OnInspectorGUI(ref changed);
            }
            else if ((DistributionSettings.DistributionMode)mode.enumValueIndex == DistributionSettings.DistributionMode.Radial)
            {
                radialEditor.OnInspectorGUI(ref changed);
            }
            else if ((DistributionSettings.DistributionMode)mode.enumValueIndex == DistributionSettings.DistributionMode.Grid)
            {
                gridEditor.OnInspectorGUI(ref changed);
            }

            if (EditorGUI.EndChangeCheck() || changed)
            {
                
            }
        }
    }
}