// Spline Spawner by Staggart Creations (http://staggart.xyz)
// COPYRIGHT PROTECTED UNDER THE UNITY ASSET STORE EULA (https://unity.com/legal/as-terms)
//  • Copying or referencing source code for the production of new asset store, or public, content is strictly prohibited!
//  • Uploading this file to a public GitHub repository will subject it to an automated DMCA takedown request.

using System;
using System.Collections.Generic;
using sc.splines.spawner.runtime;
using UnityEditor;
using UnityEngine;

namespace sc.splines.spawner.editor
{
    [CustomEditor(typeof(SplineInstanceContainer))]
    public class ContainerInspector : Editor
    {
        private SplineInstanceContainer component;

        private void OnEnable()
        {
            component = (SplineInstanceContainer)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            using (new EditorGUI.DisabledGroupScope(true))
            {
                EditorGUILayout.ObjectField("Belongs to:", component.owner, typeof(SplineSpawner), true);
            }
            if (component.owner == null)
            {
                EditorGUILayout.HelpBox("This container does not appear to belong to any Spline Spawner component, it has been orphaned", MessageType.Warning);
            }
            
            EditorGUILayout.Space();
            
            EditorGUILayout.HelpBox($"Instance count: {component.InstanceCount}", MessageType.None);
            EditorGUILayout.HelpBox($"Pool size: {component.PoolSize}", MessageType.None);

            foreach (KeyValuePair<GameObject, Queue<GameObject>> pool in component.prefabPools)
            {
                EditorGUILayout.LabelField($"Pool Key: {pool.Key} ({(pool.Key)})");
                EditorGUILayout.LabelField($"Pool Size: {pool.Value.Count}");

                foreach (GameObject queue in pool.Value)
                {
                    
                }
            }
        }
    }
}