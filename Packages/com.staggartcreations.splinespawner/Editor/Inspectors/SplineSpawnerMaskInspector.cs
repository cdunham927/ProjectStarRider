using sc.splines.spawner.runtime;
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEditor.Splines;
using UnityEngine;
using UnityEngine.Splines;

namespace sc.splines.spawner.editor
{
    [CustomEditor(typeof(SplineSpawnerMask))]
    class SplineSpawnerMaskInspector : Editor
    {
        private SplineSpawnerMask script;
        
        private SerializedProperty splineContainer;
        private SerializedProperty precision;
        private SerializedProperty layers;
        private SerializedProperty invert;
        private SerializedProperty shape;
        private SerializedProperty margin;
        private SerializedProperty capRounding;
        private SerializedProperty overlay;
        private SerializedProperty computeShader;

        
        private void OnEnable()
        {
            script = (SplineSpawnerMask)target;
            
            splineContainer = serializedObject.FindProperty("splineContainer");
            precision = serializedObject.FindProperty("precision");
            layers = serializedObject.FindProperty("layers");
            invert = serializedObject.FindProperty("invert");
            shape = serializedObject.FindProperty("shape");
            margin = serializedObject.FindProperty("margin");
            capRounding = serializedObject.FindProperty("capRounding");
            overlay = serializedObject.FindProperty("overlay");
            computeShader = serializedObject.FindProperty("computeShader");
            
            if(script.RequiresUpdate()) script.ForceUpdate();
        }

        public override void OnInspectorGUI()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField($"Spline Spawner - v{SplineSpawner.VERSION}", EditorStyles.centeredGreyMiniLabel);
            }
            EditorGUILayout.Space();

            #if SPLINES
            var requiresUpdate = false;
            serializedObject.Update();

            if (computeShader.objectReferenceValue == null)
            {
                EditorGUILayout.HelpBox("Compute shader is missing", MessageType.Warning);
                if (GUILayout.Button("Try to find"))
                {
                    string guid = "30a0bbe53b574e89839a3bcce718fb76";
                    string path = AssetDatabase.GUIDToAssetPath(guid);

                    if (path != null)
                    {
                        ComputeShader shader = AssetDatabase.LoadAssetAtPath<ComputeShader>(path);
                        computeShader.objectReferenceValue = shader;
                        EditorUtility.SetDirty(target);
                    }
                    else
                    {
                        Debug.LogError("Compute shader couldn't be found in the project, re-import it from the asset store");
                    }
                }
            }
            
            EditorGUI.BeginChangeCheck();

            
            using (new EditorGUILayout.HorizontalScope())
            {
                using (var check = new EditorGUI.ChangeCheckScope())
                {
                    EditorGUILayout.PropertyField(splineContainer);
                    
                    requiresUpdate |= check.changed;
                }
                if (splineContainer.objectReferenceValue)
                {
                    if (GUILayout.Button("Edit", EditorStyles.miniButton, GUILayout.Width(50f)))
                    {
                        Selection.activeGameObject = ((SplineContainer)splineContainer.objectReferenceValue).gameObject;
                        EditorApplication.delayCall += ToolManager.SetActiveContext<SplineToolContext>;
                    }
                }
                else
                {
                    if (GUILayout.Button("Add", EditorStyles.miniButton, GUILayout.Width(50f)))
                    {
                        splineContainer.objectReferenceValue = SplineSpawnerEditor.AddSplineContainer(script.gameObject);
                    }
                }
            }

            if (!splineContainer.objectReferenceValue)
            {
                EditorGUILayout.HelpBox("Assign a Spline container to work with", MessageType.Info);
            }
            else
            {
                using (var check = new EditorGUI.ChangeCheckScope())
                {
                    EditorGUILayout.PropertyField(precision);
                    requiresUpdate |= check.changed;
                }
            }

            if (splineContainer.objectReferenceValue)
            {
                EditorGUILayout.Space();

                EditorGUILayout.PropertyField(layers);

                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Settings", EditorStyles.boldLabel);

                using (var check = new EditorGUI.ChangeCheckScope())
                {
                    //EditorGUILayout.PropertyField(shape);
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PrefixLabel(shape.displayName);
                    shape.enumValueIndex = GUILayout.Toolbar(shape.enumValueIndex, System.Enum.GetNames(typeof(SplineSpawnerMask.Shape)));
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.PropertyField(invert);
                    EditorGUILayout.PropertyField(margin);

                    if ((SplineSpawnerMask.Shape)shape.enumValueIndex == SplineSpawnerMask.Shape.Line)
                    {
                        EditorGUILayout.PropertyField(capRounding);
                    }
                    
                    requiresUpdate |= check.changed;
                }

                EditorGUILayout.Space();

                EditorGUILayout.LabelField("Scene View", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(overlay);
            }
            
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }
            
            if(requiresUpdate) Update();
            #endif
            
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("- Staggart Creations -", EditorStyles.centeredGreyMiniLabel);
        }

        void Update()
        {
            //EditorApplication.delayCall = () =>
            {
                foreach (var m_target in targets)
                {
                    SplineSpawnerMask mask = (SplineSpawnerMask)m_target;
                    mask.ForceUpdate();
                    mask.RespawnAffectedSpawners();
                }
            };
        }

        public override GUIContent GetPreviewTitle()
        {
            return new GUIContent("Spline Distance Field");
        }

        public override bool HasPreviewGUI()
        {
            return script.sdf;
        }
        
        public override void OnPreviewGUI(Rect r, GUIStyle background)
        {
            if (script && script.sdf)
            {
                GUI.DrawTexture(r, script.sdf, ScaleMode.ScaleToFit);
            
                Rect btnRect = r;
                btnRect.x += 10f;
                btnRect.y += 10f;
                btnRect.width = 60f;
                btnRect.height = 20f;
            
                if (GUI.Button(btnRect, "Render"))
                {
                    script.ForceUpdate();
                }

                string memorySize = (UnityEngine.Profiling.Profiler.GetRuntimeMemorySizeLong(script.sdf) / 1024f / 1024f).ToString("F2");
                
                GUI.Label(new Rect(r.width * 0.5f - (225 * 0.5f), 30, 225, 25),
                    $"{script.sdf.width}x{script.sdf.height}px. ({script.sdf.width * script.sdf.height} pixels) ({memorySize}mb)", EditorStyles.toolbarButton);
                
                GUI.Label(new Rect(r.width * 0.5f - (150 * 0.5f), r.height - 10, 150, 25),
                    $"Render time: {script.LastRenderTime}ms", EditorStyles.toolbarButton);
            }
        }
    }
}