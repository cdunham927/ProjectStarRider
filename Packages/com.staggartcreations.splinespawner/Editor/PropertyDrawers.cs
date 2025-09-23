// Spline Spawner by Staggart Creations (http://staggart.xyz)
// COPYRIGHT PROTECTED UNDER THE UNITY ASSET STORE EULA (https://unity.com/legal/as-terms)
//  • Copying or referencing source code for the production of new asset store, or public, content is strictly prohibited!
//  • Uploading this file to a public GitHub repository will subject it to an automated DMCA takedown request.

using sc.splines.spawner.runtime;
using UnityEditor;
using UnityEngine;

namespace sc.splines.spawner.editor
{
    internal sealed class PropertyDrawers
    {
        [CustomPropertyDrawer(typeof(Modifier.MinMaxSlider))]
        public sealed class MinMaxSliderDrawer : PropertyDrawer
        {
            private float minBrightness;
            private float maxBrightness;

            private Rect rect;

            public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
            {
                this.rect = rect;
                Modifier.MinMaxSlider range = attribute as Modifier.MinMaxSlider;

                minBrightness = property.vector2Value.x;
                maxBrightness = property.vector2Value.y;

                using (new EditorGUILayout.HorizontalScope())
                {
                    EditorGUILayout.LabelField(property.displayName, GUILayout.MaxWidth(EditorGUIUtility.labelWidth));
                    EditorGUILayout.FloatField(minBrightness, GUILayout.Width(30f));
                    EditorGUILayout.MinMaxSlider(ref minBrightness, ref maxBrightness, range.min, range.max);
                    EditorGUILayout.FloatField(maxBrightness, GUILayout.Width(30f));
                }

                property.vector2Value = new Vector2(minBrightness, maxBrightness);
            }

            public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
            {
                return rect.height;
            }
        }
    }
}