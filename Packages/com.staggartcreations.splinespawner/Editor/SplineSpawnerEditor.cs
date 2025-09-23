// Spline Spawner by Staggart Creations (http://staggart.xyz)
// COPYRIGHT PROTECTED UNDER THE UNITY ASSET STORE EULA (https://unity.com/legal/as-terms)
//  • Copying or referencing source code for the production of new asset store, or public, content is strictly prohibited!
//  • Uploading this file to a public GitHub repository will subject it to an automated DMCA takedown request.

using System;
using sc.splines.spawner.runtime;
using Unity.Mathematics;
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Splines;
using UnityEditor.Splines;

namespace sc.splines.spawner.editor
{
    public static class SplineSpawnerEditor
    {
        #if SPLINES
        #region Menu items
        private const string DEFAULT_PREFAB_GUID = "0d8f3ff3ffb9e7b48984d748f06a3215";
        
        [MenuItem("GameObject/Spline/Spawner", false, 3000)]
        public static GameObject CreateSplineSpawner()
        {
            Transform parent = Selection.activeGameObject ? Selection.activeGameObject.transform : null;
            GameObject gameObject = new GameObject(GameObjectUtility.GetUniqueNameForSibling(parent, "Spline Spawner"));
            Undo.RegisterCreatedObjectUndo(gameObject, "Created Spline Spawner Object");
            
            SplineSpawner component = gameObject.AddComponent<SplineSpawner>();
            
            if (parent) gameObject.transform.parent = parent;
            
            Selection.activeGameObject = gameObject;

            if (!parent) EditorApplication.ExecuteMenuItem("GameObject/Move To View");
            else gameObject.transform.localPosition = Vector3.zero;

            return gameObject;
        }
        
        [MenuItem("CONTEXT/Transform/Spawn on spline", false, 3000)]
        private static void AddSpawnerToMeshFilter(MenuCommand cmd)
        {
            Transform transform = (Transform)cmd.context;
            
            GameObject go = new GameObject($"{transform.name} Spline");
            Undo.RegisterCreatedObjectUndo(go, "Created Spline Spawner");
            
            SceneManager.MoveGameObjectToScene(go, transform.gameObject.scene);
            go.transform.SetSiblingIndex(transform.gameObject.transform.GetSiblingIndex() + 1);
            go.transform.position = transform.transform.position;
            
            SplineSpawner component = go.AddComponent<SplineSpawner>();

            if (EditorUtility.DisplayDialog("Spawn object on spline", "Create with a new spline?", "Yes", "No"))
            {
                SplineContainer splineContainer = go.AddComponent<SplineContainer>();
                Undo.RegisterCreatedObjectUndo(splineContainer, "Created Spline Spawner");
                
                //Create new spline
                {
                    //One knot every 5 units
                    int knots = Mathf.RoundToInt(5f);
                    knots = Math.Max(2, knots);
                    
                    Spline spline = new Spline(knots, false);

                    for (int i = 0; i <= knots; i++)
                    {
                        float t = (float)i / (float)knots;
                        BezierKnot knot = new BezierKnot();
                        knot.Position = new Vector3(0, 0f, (t * 10) - (10 * 0.5f));
                        spline.Add(knot, TangentMode.Linear);
                    }

                    //Automatically recalculate tangents
                    spline.SetTangentMode(new SplineRange(0, spline.Count), TangentMode.AutoSmooth);
                    
                    //Spline container will be instantiated with a default spline, so overwrite it
                    splineContainer.Spline = spline;
                }

                component.splineContainer = splineContainer;

                Selection.activeGameObject = splineContainer.gameObject;
                //EditorApplication.ExecuteMenuItem("GameObject/Move To View");
                
                //Activate the spline editor
                EditorApplication.delayCall += ToolManager.SetActiveContext<SplineToolContext>;
            }
            
            bool sourceIsPrefab = PrefabUtility.GetPrefabAssetType(transform.gameObject) != PrefabAssetType.NotAPrefab;

            GameObject source = transform.gameObject;
            if (sourceIsPrefab)
            {
                //If its a prefab
                if (PrefabUtility.GetPrefabAssetType(transform.gameObject) != PrefabAssetType.Variant)
                {
                    source = PrefabUtility.GetCorrespondingObjectFromOriginalSource(source);
                    Undo.DestroyObjectImmediate(transform.gameObject);
                }
            }
            
            component.AddObject(source);
            
            component.Respawn();

            Selection.activeGameObject = component.gameObject;

            if (Application.isPlaying == false) EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
            
        }
        
        [MenuItem("CONTEXT/SplineContainer/Add Spline Spawner", true, 3000)]
        private static bool AddSpawnerToSplineValidation(MenuCommand cmd)
        {
            SplineContainer splineContainer = (SplineContainer)cmd.context;
            
            return !splineContainer.GetComponent<SplineSpawner>();
        }

        [MenuItem("CONTEXT/SplineContainer/Add Spline Spawner", false, 3000)]
        private static void AddSpawnerToSpline(MenuCommand cmd)
        {
            SplineContainer splineContainer = (SplineContainer)cmd.context;

            SplineSpawner component = splineContainer.GetComponent<SplineSpawner>();

            if (component) return;
            
            component = splineContainer.gameObject.AddComponent<SplineSpawner>();

            string defaultPrefabPath = AssetDatabase.GUIDToAssetPath(DEFAULT_PREFAB_GUID);
            if (defaultPrefabPath != string.Empty)
            {
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(defaultPrefabPath);
                component.AddObject(prefab);
                
                component.Respawn();
            }
            
            Undo.RegisterCreatedObjectUndo(component, $"Add Spline Spawner to {splineContainer.name}");
            
            component.splineContainer = splineContainer;
            
            EditorUtility.SetDirty(splineContainer.gameObject);
        }

        [MenuItem("GameObject/Spline/Spawner Mask", false, 3000)]
        private static void CreateSplineSpawnerMask()
        {
            Transform parent = Selection.activeGameObject ? Selection.activeGameObject.transform : null;
            GameObject gameObject = new GameObject(GameObjectUtility.GetUniqueNameForSibling(parent, "Spline Spawner Mask"));
            Undo.RegisterCreatedObjectUndo(gameObject, "Created Spline Spawner Mask");
            
            SplineSpawnerMask component = gameObject.AddComponent<SplineSpawnerMask>();
            
            if (parent) gameObject.transform.parent = parent;
            
            Selection.activeGameObject = gameObject;

            if (!parent) EditorApplication.ExecuteMenuItem("GameObject/Move To View");
            else gameObject.transform.localPosition = Vector3.zero;
        }
        
        [MenuItem("CONTEXT/SplineContainer/Add Spawner Mask", true, 3000)]
        private static bool AddMaskToSplineValidation(MenuCommand cmd)
        {
            SplineContainer splineContainer = (SplineContainer)cmd.context;
            
            return !splineContainer.GetComponent<SplineSpawnerMask>();
        }
        
        [MenuItem("CONTEXT/SplineContainer/Add Spawner Mask", false, 3000)]
        private static void AddMaskToSpline(MenuCommand cmd)
        {
            SplineContainer splineContainer = (SplineContainer)cmd.context;
            SplineSpawnerMask mask = splineContainer.GetComponent<SplineSpawnerMask>();

            if (mask) return;

            mask = Undo.AddComponent<SplineSpawnerMask>(splineContainer.gameObject);
            mask.splineContainer = splineContainer;
        }
        #endregion
        
        public static SplineContainer AddSplineContainer(GameObject target)
        {
            if (target == null)
            {
                throw new Exception("Cannot add a Spline Container component to a null GameObject");
            }
            
            SplineContainer splineContainer = target.AddComponent<SplineContainer>();
            splineContainer.AddSpline(CreateDefaultSpline());

            EditorUtility.SetDirty(target);
            
            return splineContainer;
        }
        
        private static Spline CreateDefaultSpline()
        {
            int knots = 4;
            float length = 25f;
            
            Spline spline = new Spline(knots, false);

            for (int i = 0; i < knots; i++)
            {
                float t = (float)i / (float)knots;
                
                BezierKnot knot = new BezierKnot();
                knot.Position = new Vector3(Mathf.Sin(t * length), 0f, (t * length) - (length * 0.5f));
                spline.Add(knot, TangentMode.AutoSmooth);
            }

            //Automatically recalculate tangents
            spline.SetTangentMode(new SplineRange(0, spline.Count), TangentMode.AutoSmooth);

            return spline;
        }

        public static void DrawPrefabDataPopup(Rect rect, SplineSpawner.SpawnableObject prefab)
        {
            PrefabDataPopupWindow.Show(rect, prefab);
        }

        private class PrefabDataPopupWindow : EditorWindow
        {
            private SplineSpawner.SpawnableObject _prefab;
            private PrefabData _prefabData;
    
            public static void Show(Rect rect, SplineSpawner.SpawnableObject prefab)
            {
                var window = CreateInstance<PrefabDataPopupWindow>();
                window._prefab = prefab;
                window._prefabData = PrefabData.Create(prefab, 0);
        
                // Set window size and position
                Vector2 size = new Vector2(400, 150);
                window.position = new Rect(
                    rect.x + (rect.width - size.x) * 0.5f,
                    rect.y + (rect.height - size.y) * 0.5f,
                    size.x,
                    size.y
                );
        
                window.titleContent = new GUIContent("Prefab Data");
                window.ShowModal();
            }

            private void OnGUI()
            {
                EditorGUILayout.BeginVertical(new GUIStyle { padding = new RectOffset(10, 10, 10, 10) });

                string FormatFloat3(float3 value)
                {
                    return $"({Math.Round(value.x, 4)}, {Math.Round(value.y, 4)}, {Math.Round(value.z, 4)})";
                }
                
                EditorGUILayout.LabelField("Bounds", EditorStyles.boldLabel);
                EditorGUILayout.LabelField($"Min: {FormatFloat3(_prefabData.boundsMin)}");
                EditorGUILayout.LabelField($"Max: {FormatFloat3(_prefabData.boundsMax)}");
                EditorGUILayout.LabelField($"Size: {FormatFloat3(_prefabData.boundsSize)}");
        
                EditorGUILayout.Space();
        
                EditorGUILayout.LabelField($"Pivot: {FormatFloat3(_prefabData.pivotOffset)}");
                EditorGUILayout.LabelField($"Scale: {FormatFloat3(_prefabData.gameObjectScale)}");

                EditorGUILayout.Space();
        
                if (GUILayout.Button("Close"))
                {
                    Close();
                }
        
                EditorGUILayout.EndVertical();
            }
        }
        #endif
    }
}