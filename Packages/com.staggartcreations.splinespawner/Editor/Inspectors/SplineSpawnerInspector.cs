// Spline Spawner by Staggart Creations (http://staggart.xyz)
// COPYRIGHT PROTECTED UNDER THE UNITY ASSET STORE EULA (https://unity.com/legal/as-terms)
//  • Copying or referencing source code for the production of new asset store, or public, content is strictly prohibited!
//  • Uploading this file to a public GitHub repository will subject it to an automated DMCA takedown request.

using System;
using System.Collections.Generic;
using System.Reflection;
using sc.splines.spawner.runtime;
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEditor.SceneManagement;
using UnityEditor.Splines;
using UnityEditorInternal;
using UnityEngine;

namespace sc.splines.spawner.editor
{
    [CustomEditor(typeof(SplineSpawner))]
    [CanEditMultipleObjects]
    public class SplineSpawnerInspector : Editor
    {
        SplineSpawner spawner;

        private SerializedProperty id;
        
        private SerializedProperty splineContainer;
        private SerializedProperty respawningMode;
        private SerializedProperty root;
        private SerializedProperty hideInstances;
        
        private SerializedProperty inputObjects;
        private SerializedProperty distributionSettings;

        private SerializedProperty maskRules;
        
        private SerializedProperty modifiers;
        //private SerializedProperty modifierStack;

        private Vector2 modifierScrollPos;

        private Texture2D defaultThumb;
        private const float ThumbSize = 75f;
        private Texture2D maskIcon;
        private int newPrefabPickerWindowID;
        
        DistributionSettingsEditor distributionSettingsEditor;

        private static bool ShowStats
        {
            get => SessionState.GetBool("SPLINE_SPAWNER_DISPLAY_DATA", false);
            set => SessionState.SetBool("SPLINE_SPAWNER_DISPLAY_DATA", value);
        }
        
        private static bool ExpandPrefabs
        {
            get => SessionState.GetBool("SPLINE_SPAWNER_EXPAND_PREFABS", false);
            set => SessionState.SetBool("SPLINE_SPAWNER_EXPAND_PREFABS", value);
        }
        private static bool ExpandDistribution
        {
            get => SessionState.GetBool("SPLINE_SPAWNER_EXPAND_DISTRIBUTION", false);
            set => SessionState.SetBool("SPLINE_SPAWNER_EXPAND_DISTRIBUTION", value);
        }
        private static bool ExpandMasking
        {
            get => SessionState.GetBool("SPLINE_SPAWNER_EXPAND_MASKING", false);
            set => SessionState.SetBool("SPLINE_SPAWNER_EXPAND_MASKING", value);
        }
        private static bool ExpandModifiers
        {
            get => SessionState.GetBool("SPLINE_SPAWNER_EXPAND_MODIFIERS", false);
            set => SessionState.SetBool("SPLINE_SPAWNER_EXPAND_MODIFIERS", value);
        }
        
        private static int SelectedModifier
        {
            get => SessionState.GetInt("SPLINE_SPAWNER_SELECTED_MODIFIER", 0);
            set => SessionState.SetInt("SPLINE_SPAWNER_SELECTED_MODIFIER", value);
        }

        private bool inspectingPrefab;

        
        private ReorderableList modifierList;
        private void OnEnable()
        {
            spawner = (SplineSpawner)target;
            inspectingPrefab = PrefabUtility.GetPrefabAssetType(target) != PrefabAssetType.NotAPrefab;
            
            id = serializedObject.FindProperty("id");
            
            splineContainer = serializedObject.FindProperty("splineContainer");
            respawningMode = serializedObject.FindProperty("respawningMode");
            root = serializedObject.FindProperty("root");
            hideInstances = serializedObject.FindProperty("hideInstances");
            
            inputObjects = serializedObject.FindProperty("inputObjects");
            distributionSettings = serializedObject.FindProperty("distributionSettings");
            maskRules = serializedObject.FindProperty("maskRules");
            
            modifiers = serializedObject.FindProperty("modifiers");

            distributionSettingsEditor = DistributionSettingsEditor.CreateEditor(distributionSettings);
            
            defaultThumb = EditorGUIUtility.IconContent("GameObject Icon").image as Texture2D;

            IconAttribute maskIconAttribute = (IconAttribute)(typeof(SplineSpawnerMask)).GetCustomAttribute(typeof(IconAttribute));
            if (maskIconAttribute != null)
            {
                maskIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(maskIconAttribute.path);
            }
            
            ValidateTargets();

            if (spawner.HasMissingPrefabs())
            {
                ExpandPrefabs = true;
            }

            InitModifierStack();
        }
        
        // layer list view
        const int kElementHeight = 40;
        const int kElementObjectFieldHeight = 16;
        const int kElementPadding = 2;
        const int kElementObjectFieldWidth = 140;
        const int kElementToggleWidth = 20;
        const int kElementThumbSize = 40;

        private void InitModifierStack()
        {
            modifierList = new ReorderableList(serializedObject, modifiers, true, true, true, true);
            
            modifierList.drawHeaderCallback = (Rect rect) => {
                EditorGUI.LabelField(rect, "Stack");
            };

            modifierList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => {
                SerializedProperty element = modifiers.GetArrayElementAtIndex(index);
                
                Modifier modifier = element.managedReferenceValue as Modifier;

                if (modifier == null) return;
                
                rect.y = rect.y;
                var rectButton = new Rect((rect.x + kElementPadding), rect.y + kElementPadding, kElementToggleWidth,
                    kElementToggleWidth);
                var labelRect = new Rect(rect.x + rectButton.x - 19, rect.y+kElementPadding, 120, 17);
                
                ModifierAttribute attribute = ModifierAttribute.GetFor(modifier.GetType());

                if (attribute != null)
                {
                    EditorGUI.LabelField(labelRect, attribute.displayName, EditorStyles.boldLabel);
                }
                SerializedProperty enabled = element.FindPropertyRelative("enabled");
                
                enabled.boolValue = EditorGUI.Toggle(rectButton, GUIContent.none, enabled.boolValue);
            };

            modifierList.onSelectCallback += OnModifierSelect;

            modifierList.elementHeightCallback = (int index) =>
            {
                return EditorGUIUtility.singleLineHeight + kElementPadding;
                //return EditorGUI.GetPropertyHeight(modifiers.GetArrayElementAtIndex(index), true) + 4f;
            };

            modifierList.onAddDropdownCallback = (Rect rect, ReorderableList list) =>
            {
                GenericMenu menu = new GenericMenu();

                for (int i = 0; i < Modifier.names.Length; i++)
                {
                    int index = i;
                    menu.AddItem(new GUIContent(Modifier.names[index]), false, () =>
                    {
                        spawner.AddModifier(Modifier.types[index]);
                        spawner.Respawn();

                        modifierList.index = spawner.modifiers.Count-1;
                    
                        EditorUtility.SetDirty(spawner);
                    });
                        
                }
                            
                menu.ShowAsContext();
            };

            modifierList.onRemoveCallback = (ReorderableList list) => 
            {
                ReorderableList.defaultBehaviours.DoRemoveButton(list);
            };
            
            modifierList.drawElementBackgroundCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                var prevColor = GUI.color;
                var prevBgColor = GUI.backgroundColor;

                GUI.color = index % 2 == 0
                    ? Color.grey * (EditorGUIUtility.isProSkin ? 1f : 1.7f)
                    : Color.grey * (EditorGUIUtility.isProSkin ? 1.05f : 1.66f);
                
                //Selection outline (note: can't rely on isfocused. Focus and selection aren't the same thing)
                if (index == modifierList.index)
                {
                    GUI.color = EditorGUIUtility.isProSkin ? Color.grey * 1.1f : Color.grey * 1.5f;
                    //GUI.color = GUI.skin.settings.selectionColor * 1.6f;
                    
                    Rect outline = rect;
                    EditorGUI.DrawRect(outline, EditorGUIUtility.isProSkin ? Color.gray * 1.5f : Color.gray);

                    rect.x += 1;
                    rect.y += 1;
                    rect.width -= 2;
                    rect.height -= 2;
                }
                
                EditorGUI.DrawRect(rect, GUI.color);

                GUI.color = prevColor;
                GUI.backgroundColor = prevBgColor;
            };

            modifierList.drawNoneElementCallback = (Rect rect) =>
            {
                EditorGUI.LabelField(rect, "Empty, add a modifier using the + button");
            };
            
            SelectedModifier = Mathf.Clamp(SelectedModifier, 0, modifiers.arraySize);
            modifierList.index = SelectedModifier;
        }

        private void OnModifierSelect(ReorderableList list)
        {
            SelectedModifier = Mathf.Clamp(list.index, 0, modifiers.arraySize);
        }
        
        public override void OnInspectorGUI()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Space(60f);
                EditorGUILayout.LabelField($"Version {SplineSpawner.VERSION} " + (AssetInfo.VersionChecking.UPDATE_AVAILABLE ? "(update available)" : "(latest)"), EditorStyles.centeredGreyMiniLabel);
                if (GUILayout.Button(new GUIContent(EditorGUIUtility.IconContent(UI.iconPrefix + "Help").image, "Help window"), GUILayout.Width(30f), GUILayout.Height(21f)))
                {
                    HelpWindow.ShowWindow();
                }
                
                ShowStats = GUILayout.Toggle(ShowStats, new GUIContent(string.Empty, EditorGUIUtility.IconContent("d_UnityEditor.ProfilerWindow").image), "Button", GUILayout.MaxWidth(37f), GUILayout.MaxHeight(21f));
            }
            EditorGUILayout.Space();

            if (Unity.Burst.BurstCompiler.IsEnabled == false)
            {
                EditorGUILayout.HelpBox("Burst compilation is disabled, expect performance degradation", MessageType.Warning);
                EditorGUILayout.Separator();
            }

            if (inspectingPrefab)
            {
                EditorGUILayout.HelpBox("Inspecting a prefab, spawning has been disabled to avoid objects leaking into the current scene", MessageType.Info);
                EditorGUILayout.Separator();
            }
            
            #if !UNITY_2022_3_OR_NEWER
            EditorGUILayout.HelpBox("Minimum required and compatible Unity version is 2022.3.23f1", MessageType.Error);
            #endif
            
            #if SPLINES
            EditorGUI.BeginChangeCheck();

            //base.OnInspectorGUI();
            
            serializedObject.Update();
            
            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(splineContainer);

                if (EditorGUI.EndChangeCheck())
                {
                    foreach (var m_target in targets)
                    {
                        ((SplineSpawner)m_target).WarmupSplineCache();
                    }
                    ValidateTargets();
                }
                
                if (splineContainer.objectReferenceValue)
                {
                    if (GUILayout.Button("Edit", EditorStyles.miniButton, GUILayout.Width(50f)))
                    {
                        Selection.activeGameObject = spawner.splineContainer.gameObject;
                        EditorApplication.delayCall += ToolManager.SetActiveContext<SplineToolContext>;
                    }
                }
                else
                {
                    if (GUILayout.Button("Add", EditorStyles.miniButton, GUILayout.Width(50f)))
                    {
                        splineContainer.objectReferenceValue = SplineSpawnerEditor.AddSplineContainer(spawner.gameObject);
                    }
                }
            }

            if (splineContainer.objectReferenceValue || inspectingPrefab)
            {
                EditorGUI.indentLevel++;
                using (new EditorGUILayout.HorizontalScope())
                {
                    EditorGUILayout.PropertyField(respawningMode, GUILayout.MaxWidth(EditorGUIUtility.labelWidth + 200f));

                    if (respawningMode.intValue == (int)SplineSpawner.RespawningMode.None)
                    {
                        if (GUILayout.Button("Respawn", EditorStyles.miniButton, GUILayout.Width(100f)))
                        {
                            RespawnTargets();
                        }
                    }
                }

                EditorGUI.indentLevel--;

                using (new EditorGUILayout.HorizontalScope())
                {
                    EditorGUILayout.PropertyField(root);
                    if (GUILayout.Button("This", EditorStyles.miniButton, GUILayout.Width(50f)))
                    {
                        root.objectReferenceValue = spawner.gameObject;
                    }
                }

                EditorGUI.indentLevel++;
                using (new EditorGUILayout.HorizontalScope())
                {
                    EditorGUILayout.PropertyField(hideInstances);
                }
                EditorGUI.indentLevel--;

                EditorGUILayout.Space();

                DrawPrefabs();

                DistributionSettings.DistributionMode distributionMode = (DistributionSettings.DistributionMode)distributionSettings.FindPropertyRelative("mode").enumValueIndex;
                ExpandDistribution = UI.DrawFoldout(ExpandDistribution, "Distribution", () =>
                {
                    distributionSettingsEditor.OnInspectorGUI();
                }, $"({System.Text.RegularExpressions.Regex.Replace(distributionMode.ToString(), "(\\B[A-Z])", " $1")})");
                
                ExpandMasking = UI.DrawFoldout(ExpandMasking, "Masking", () =>
                {
                    DrawMasking();
                }, GetMaskLayerNames());

                int modifierCount = spawner.modifiers.Count;
                
                ExpandModifiers = UI.DrawFoldout(ExpandModifiers, "Modifiers", () =>
                {
                    if (modifierCount == 0)
                    {
                        EditorGUILayout.HelpBox("\n" +
                                                "Modifiers perform specific actions on each spawned object." +
                                                "\n\n" +
                                                "They can be used to add randomization, rotations or to further control positioning" +
                                                "\n", MessageType.Info);
                        
                        EditorGUILayout.Space();
                    }
                    
                    modifierList.DoLayoutList();

                    if (modifierList.count > 0 && modifierList.index >= 0 && modifierList.index < modifierList.count)
                    {
                        EditorGUILayout.Space(-10f);
                        SerializedProperty modifier = modifiers.GetArrayElementAtIndex(modifierList.index);
                        
                        ModifierAttribute attributes = ModifierAttribute.GetFor(modifier);
                        if (attributes != null)
                        {
                            if (attributes.IsIncompatibleDistributionMode(spawner.distributionSettings.mode))
                            {
                                EditorGUILayout.HelpBox("This modifier is incompatible with the current distribution mode", MessageType.Warning);
                            }
                        }
                        EditorGUILayout.PropertyField(modifier, GUIContent.none);
                    }
                }, $"({modifierCount})");
            }
            else
            {
                EditorGUILayout.HelpBox("Assign a Spline container to spawn with...", MessageType.Info);
            }
            
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();

                //EditorApplication.delayCall += RespawnTargets;
                if(!inspectingPrefab) RespawnTargets();
            }

            if (splineContainer.objectReferenceValue && !inspectingPrefab)
            {
                EditorGUILayout.Space();
                
                if (ShowStats)
                {
                    using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
                    {
                        EditorGUILayout.LabelField($"Instance count: {spawner.InstanceCount}", EditorStyles.boldLabel);

                        EditorGUILayout.Separator();

                        EditorGUILayout.LabelField($"Distribution: {spawner.lastDistributionTime}ms", EditorStyles.miniLabel);
                        EditorGUILayout.LabelField($"Masking: {spawner.lastMaskingTime}ms", EditorStyles.miniLabel);
                        EditorGUILayout.LabelField($"Modifiers: {spawner.lastModifierStackTime}ms", EditorStyles.miniLabel);
                        EditorGUILayout.LabelField($"Instantiating: {spawner.lastInstantiateTime}ms", EditorStyles.miniLabel);
                        EditorGUILayout.LabelField($"Processing time: {spawner.lastDistributionTime + spawner.lastMaskingTime + spawner.lastModifierStackTime + spawner.lastInstantiateTime}ms", EditorStyles.boldLabel);

                        EditorGUILayout.Separator();
                    }
                }
                EditorGUILayout.EndFoldoutHeaderGroup();

                if (spawner.spawnObjects == false)
                {
                    EditorGUILayout.HelpBox("Spawning of objects has been disabled", MessageType.Warning);
                }

                using (new EditorGUILayout.HorizontalScope())
                {
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button(" - Respawn - ", EditorStyles.miniButtonMid))
                    {
                        RespawnTargets();
                    }
                    GUILayout.FlexibleSpace();
                }
            }
            #else
            EditorGUILayout.HelpBox("The Splines package isn't installed, component unavailable", MessageType.Error);
            #endif

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("- Staggart Creations -", EditorStyles.centeredGreyMiniLabel);
        }

        #if SPLINES
        private void DrawPrefabs()
        {
            int prefabCount = inputObjects.arraySize;
            
            ExpandPrefabs = UI.DrawFoldout(ExpandPrefabs, $"Prefabs", () =>
            {
                Event curEvent = Event.current;

                PrefabPickingActions();
                
                inputObjects.isExpanded = ExpandPrefabs;

                using (new EditorGUILayout.HorizontalScope())
                {
                    using (new EditorGUILayout.VerticalScope())
                    {
                        GUILayout.Space(5f);

                        for (int i = 0; i < inputObjects.arraySize; i++)
                        {
                            SerializedProperty prefab = inputObjects.GetArrayElementAtIndex(i);
                            SerializedProperty prefabObject = prefab.FindPropertyRelative("prefab");
                            
                            using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar))
                            {
                                EditorGUI.BeginChangeCheck();
                                EditorGUILayout.PropertyField(prefabObject);
                                if (EditorGUI.EndChangeCheck())
                                {
                                    var index = i;
                                    EditorApplication.delayCall += () =>
                                    {
                                        RefreshThumbnail(spawner.inputObjects[index]);
                                    };
                                }
                                
                                if (GUILayout.Button("X", EditorStyles.toolbarButton, GUILayout.MaxWidth(30f)))
                                {
                                    inputObjects.DeleteArrayElementAtIndex(i);
                                    //spawner.RemoveObject(i);
                                    EditorUtility.SetDirty(spawner);
                                    return; 
                                }
                            }

                            if (prefabObject.objectReferenceValue == null)
                            {
                                EditorGUILayout.HelpBox("An object or prefab must be assigned.", MessageType.Warning);
                            }
                            else
                            {
                                using (new EditorGUILayout.HorizontalScope())
                                {
                                    SplineSpawner.SpawnableObject spawnableObject = spawner.inputObjects[i];
                                    
                                    if(spawnableObject == null) continue;
                                    
                                    if (!spawnableObject.thumbnail) RefreshThumbnail(spawnableObject);
                                    GUILayout.Box(new GUIContent(null, spawnableObject.thumbnail), GUILayout.Width(ThumbSize), GUILayout.Height(ThumbSize));
                                    
                                    Rect thumbRect = GUILayoutUtility.GetLastRect();
            
                                    //Right click
                                    if (curEvent.isMouse && curEvent.type == EventType.MouseUp && curEvent.button == 1)
                                    {
                                        if (thumbRect.Contains(curEvent.mousePosition))
                                        {
                                            GenericMenu menu = new GenericMenu();
                                            
                                            menu.AddItem(new GUIContent("Refresh thumbnail"), false, () =>
                                            {
                                                RefreshThumbnail(spawnableObject);
                                            });

                                            menu.AddItem(new GUIContent("Debug prefab data"), false, () =>
                                            {
                                                SplineSpawnerEditor.DrawPrefabDataPopup(thumbRect, spawnableObject);
                                            });
                                            
                                            menu.ShowAsContext();
                                        }
                                    }
                                    
                                    GUILayout.Space(8f);
                                    
                                    using (new EditorGUILayout.VerticalScope())
                                    {
                                        EditorGUILayout.Separator();
                                        
                                        //Expiremental
                                        //EditorGUILayout.PropertyField(prefab.FindPropertyRelative("forwardDirection"));
                                        //EditorGUILayout.PropertyField(prefab.FindPropertyRelative("pivot"));

                                        //SerializedProperty selectBySplineDistance = prefab.FindPropertyRelative("selectBySplineDistance");
                                        //EditorGUILayout.PropertyField(selectBySplineDistance);
                                        
                                        SerializedProperty baseScale = prefab.FindPropertyRelative("baseScale");
                                        EditorGUILayout.PropertyField(baseScale);
                                        
                                        SerializedProperty probability = prefab.FindPropertyRelative("probability");
                                        using (new EditorGUILayout.HorizontalScope())
                                        {
                                            EditorGUILayout.PropertyField(probability);
                                            EditorGUILayout.LabelField("%", EditorStyles.boldLabel, GUILayout.MaxWidth(13f));
                                        }
                                    }
                                    
                                    GUILayout.Space(10f);
                                }
                            }
                            
                            EditorGUILayout.Separator();
                        }
                    }
                }

                void RefreshThumbnail(SplineSpawner.SpawnableObject obj)
                {
                    if (obj.prefab == null)
                    {
                        obj.thumbnail = defaultThumb;
                        return;
                    }
                    obj.thumbnail = AssetPreview.GetAssetPreview(obj.prefab);
                }
                
                void DropAreaGUI()
                {
                    Event currentEvent = Event.current;

                    Rect activeArea = GUILayoutUtility.GetRect(0, 50f, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));

                    string text = "+ Drag & drop prefabs to add here";
                    switch (currentEvent.type)
                    {
                        case EventType.DragUpdated:
                        case EventType.DragPerform:
                            if (!activeArea.Contains(currentEvent.mousePosition))
                            {
                                return;
                            }

                            DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                            if (currentEvent.type == EventType.DragPerform)
                            {
                                int objectCount = DragAndDrop.objectReferences.Length;

                                if (objectCount == 1)
                                {
                                    if (DragAndDrop.objectReferences[0] is not GameObject)
                                    {
                                        Event.current.Use();
                                        break;
                                    }
                                }
                                DragAndDrop.AcceptDrag();

                                if(DragAndDrop.objectReferences.Length > 0) Undo.RecordObjects(targets, "Drop & Drop prefabs into spawner");
                                
                                foreach (UnityEngine.Object draggedObject in DragAndDrop.objectReferences)
                                {
                                    int addedPrefabCount = 0;
                                    if (draggedObject is GameObject gameObject)
                                    {
                                        foreach (var m_target in targets)
                                        {
                                            ((SplineSpawner)m_target).AddObject(gameObject);
                                        }
                                        addedPrefabCount++;
                                    }
                                    
                                    if(addedPrefabCount > 0) RespawnTargets();
                                }
                                
                                DragAndDrop.activeControlID = 0;
                                Event.current.Use();
                            }

                            break;
                    }
                    
                    GUI.Box(activeArea, GUIContent.none, EditorStyles.textArea);
                    GUI.Label(activeArea, text, EditorStyles.centeredGreyMiniLabel);
                }

                //Drag & drop new prefabs
                DropAreaGUI();
                
                using (new EditorGUILayout.HorizontalScope())
                {
                    GUILayout.FlexibleSpace();

                    if (GUILayout.Button("Add from project"))
                    {
                        newPrefabPickerWindowID = EditorGUIUtility.GetControlID(FocusType.Passive) + 200; 
                        EditorGUIUtility.ShowObjectPicker<GameObject>(null, false, "t:prefab", newPrefabPickerWindowID);
                    }
                }
            },
                $"({prefabCount})");
        }

        private void PrefabPickingActions()
        {
            //New specifics (initial prefab)
            if (Event.current.commandName == "ObjectSelectorClosed" &&
                EditorGUIUtility.GetObjectPickerControlID() == newPrefabPickerWindowID)
            {
                GameObject pickedPrefab = (GameObject)EditorGUIUtility.GetObjectPickerObject();
                newPrefabPickerWindowID = -1;

                if (pickedPrefab == null) return;

                SplineSpawner.SpawnableObject spawnableObject = new SplineSpawner.SpawnableObject(pickedPrefab);

                spawner.AddObject(pickedPrefab);
                
                EditorUtility.SetDirty(spawner);

                spawner.Respawn();
            }
        }
        
        private void DrawMasking()
        {
            
            int ruleCount = maskRules.arraySize;

            if (ruleCount == 0)
            {
                EditorGUILayout.HelpBox("Masking prevents this spawner from instantiating objects near or within other splines ", MessageType.Info);
            }
            else
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    GUILayout.FlexibleSpace();

                    if (GUILayout.Button("Edit layer names", EditorStyles.miniButton, GUILayout.MaxWidth(110f)))
                    {
                        MaskLayerEditor.MaskLayerNamesEditorWindow.OpenWindow(EditorGUILayout.GetControlRect());
                    }
                }
            }
            
            for (int i = 0; i < ruleCount; i++)
            {
                SerializedProperty property = maskRules.GetArrayElementAtIndex(i);
                
                SerializedProperty layer = property.FindPropertyRelative("layer");
                SerializedProperty invert = property.FindPropertyRelative("invert");
                SerializedProperty minDistance = property.FindPropertyRelative("minDistance");
                SerializedProperty falloff = property.FindPropertyRelative("falloff");

                using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
                {
                    using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar))
                    {
                        EditorGUILayout.PropertyField(layer);
                        if (GUILayout.Button(EditorGUIUtility.TrIconContent("Toolbar Minus", "Choose layer add to the list"), EditorStyles.toolbarButton, GUILayout.Width(30)))
                        {
                            maskRules.DeleteArrayElementAtIndex(i);
                            return;
                        }
                    }
                    EditorGUILayout.PropertyField(invert);

                    EditorGUILayout.Space();

                    EditorGUILayout.PropertyField(minDistance);
                    EditorGUILayout.PropertyField(falloff);
                }
                
                EditorGUILayout.Separator();
            }
            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.FlexibleSpace();
                
                if (GUILayout.Button(EditorGUIUtility.TrIconContent("Toolbar Plus More", "Choose layer add to the list"), GUILayout.MaxWidth(40)))
                {
                    GenericMenu menu = new GenericMenu();

                    for (int j = 0; j < MaskLayerEditor.LayerCount; j++)
                    {
                        int index = j;
                        menu.AddItem(new GUIContent(MaskLayerEditor.IndexToName(index)), false, () =>
                        {
                            List<SplineSpawner.MaskRule> maskList = new List<SplineSpawner.MaskRule>(spawner.maskRules);
                            
                            maskList.Add(new SplineSpawner.MaskRule(index));

                            spawner.maskRules = maskList.ToArray();
                            spawner.Respawn();
                            
                            EditorUtility.SetDirty(spawner);
                        });
                    }
                    
                    menu.ShowAsContext();
                }
            }
            
            EditorGUILayout.LabelField($"Spline masks in scene: {SplineSpawnerMask.Instances.Count}", EditorStyles.boldLabel);
            
            if (SplineSpawnerMask.Instances.Count == 0)
            {
                EditorGUILayout.HelpBox("Spline masks can be created through GameObject->Spline->Spawner Mask", MessageType.Info);
            }
            else
            {
                //this.Repaint();
                using (new EditorGUILayout.VerticalScope(EditorStyles.textArea))
                {
                    foreach (SplineSpawnerMask splineMask in SplineSpawnerMask.Instances)
                    {
                        var rect = EditorGUILayout.BeginHorizontal(EditorStyles.miniLabel);

                        if (rect.Contains(Event.current.mousePosition))
                        {
                            EditorGUIUtility.AddCursorRect(new Rect(Event.current.mousePosition.x, Event.current.mousePosition.y, 27, 27), MouseCursor.Link);
                            EditorGUI.DrawRect(rect, Color.gray * (EditorGUIUtility.isProSkin ? 0.66f : 0.20f));
                        }

                        string maskLabel = $" {splineMask.name} {MaskLayerEditor.LayersToReadableList(splineMask.layers)}";
                        if (GUILayout.Button(new GUIContent(maskLabel, maskIcon), EditorStyles.miniLabel, GUILayout.Height(20f)))
                        {
                            EditorGUIUtility.PingObject(splineMask);
                            Selection.activeGameObject = splineMask.gameObject;
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                }
            }
        }
        #endif
        
        string GetMaskLayerNames()
        {
            List<string> names = new List<string>();
            for (int i = 0; i < maskRules.arraySize; i++)
            {
                SerializedProperty property = maskRules.GetArrayElementAtIndex(i);
                SerializedProperty layer = property.FindPropertyRelative("layer");

                names.Add(MaskLayerEditor.IndexToName(layer.intValue));
            }

            return names.Count > 0 ? $"({string.Join(", ", names)})" : string.Empty;
        }
        
        private void RespawnTargets()
        {
            foreach (var m_target in targets)
            {
                ((SplineSpawner)m_target).Respawn();
            }
        }

        private void ValidateTargets()
        {
            foreach (var m_target in targets)
            {
                ((SplineSpawner)m_target).Validate();
            }
        }
    }

    
}