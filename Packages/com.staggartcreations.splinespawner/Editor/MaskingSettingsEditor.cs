// Spline Spawner by Staggart Creations (http://staggart.xyz)
// COPYRIGHT PROTECTED UNDER THE UNITY ASSET STORE EULA (https://unity.com/legal/as-terms)
//  • Copying or referencing source code for the production of new asset store, or public, content is strictly prohibited!
//  • Uploading this file to a public GitHub repository will subject it to an automated DMCA takedown request.

using System.Collections.Generic;
using sc.splines.spawner.runtime;
using UnityEditor;
using UnityEngine;

namespace sc.splines.spawner.editor
{
    public class MaskingSettingsEditor
    {
        private SerializedProperty maskingSettings;
        
        private SerializedProperty masks;
        
        private static Texture maskIcon;
        
        public static MaskingSettingsEditor CreateEditor(SerializedProperty property)
        {
            MaskingSettingsEditor editor = new MaskingSettingsEditor();
            editor.maskingSettings = property;
            maskIcon = AssetDatabase.LoadAssetAtPath<Texture>(SplineSpawner.kPackageRoot + "/Editor/Resources/spline-spawner-mask-icon-64px.psd");

            editor.OnEnable();
            
            return editor;
        }
        
        private void OnEnable()
        {
            masks = maskingSettings.FindPropertyRelative("masks");
        }

        public void OnInspectorGUI()
        {

        }
    }
}