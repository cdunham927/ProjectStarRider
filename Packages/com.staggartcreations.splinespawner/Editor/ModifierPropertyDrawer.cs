// Spline Spawner by Staggart Creations (http://staggart.xyz)
// COPYRIGHT PROTECTED UNDER THE UNITY ASSET STORE EULA (https://unity.com/legal/as-terms)
//  • Copying or referencing source code for the production of new asset store, or public, content is strictly prohibited!
//  • Uploading this file to a public GitHub repository will subject it to an automated DMCA takedown request.

using sc.splines.spawner.runtime;
using UnityEditor;
using UnityEngine;

namespace Dev.Scripts
{
    [CustomPropertyDrawer(typeof(Modifier))]
    public class ModifierPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUILayout.HelpBox("No UI available, a custom ModifierPropertyDrawer is required to draw a modifier's parameters", MessageType.Info);
        }
    }
    
    [CustomPropertyDrawer(typeof(Offset))]
    public class OffsetModifierPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            
            using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar))
            {
                ModifierAttribute attributes = ModifierAttribute.GetFor(property);
                EditorGUILayout.LabelField(new GUIContent(attributes.displayName, attributes.description), EditorStyles.boldLabel);
            }
            
            EditorGUILayout.PropertyField(property.FindPropertyRelative("direction"), new GUIContent("Direction"));
            EditorGUILayout.PropertyField(property.FindPropertyRelative("offset"), new GUIContent("Offset"));
            //EditorGUILayout.PropertyField(property.FindPropertyRelative("noiseOffset"), new GUIContent("Noise Offset"));

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Randomization", EditorStyles.boldLabel);

            EditorGUILayout.PropertyField(property.FindPropertyRelative("randomMode"), new GUIContent("Mode"));
            EditorGUILayout.PropertyField(property.FindPropertyRelative("randomMin"), new GUIContent("Min"));
            EditorGUILayout.PropertyField(property.FindPropertyRelative("randomMax"), new GUIContent("Max"));
            
            EditorGUILayout.PropertyField(property.FindPropertyRelative("randomnessFrequency"), new GUIContent("Frequency"));

            EditorGUI.EndProperty();
        }
    }
    
    [CustomPropertyDrawer(typeof(Rotate))]
    public class RotateModifierDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            
            using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar))
            {
                ModifierAttribute attributes = ModifierAttribute.GetFor(property);
                EditorGUILayout.LabelField(new GUIContent(attributes.displayName, attributes.description), EditorStyles.boldLabel);
            }

            SerializedProperty direction = property.FindPropertyRelative("direction");
            SerializedProperty rotation = property.FindPropertyRelative("rotation");
            SerializedProperty randomMode = property.FindPropertyRelative("randomMode");
            SerializedProperty randomMin = property.FindPropertyRelative("randomMin");
            SerializedProperty randomMax = property.FindPropertyRelative("randomMax");
            SerializedProperty randomnessFrequency = property.FindPropertyRelative("randomnessFrequency");

            EditorGUILayout.PropertyField(direction, new GUIContent("Direction"));
            EditorGUILayout.PropertyField(rotation, new GUIContent("Rotation"));

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Randomization", EditorStyles.boldLabel);

            EditorGUILayout.PropertyField(randomMode, new GUIContent("Mode"));
            EditorGUILayout.PropertyField(randomMin, new GUIContent("Min"));
            EditorGUILayout.PropertyField(randomMax, new GUIContent("Max"));

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(randomnessFrequency, new GUIContent("Frequency"));

            EditorGUI.EndProperty();
        }
    }
    
    [CustomPropertyDrawer(typeof(Scale))]
    public class ScaleModifierDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar))
            {
                ModifierAttribute attributes = ModifierAttribute.GetFor(property);
                EditorGUILayout.LabelField(new GUIContent(attributes.displayName, attributes.description), EditorStyles.boldLabel);
            }
            
            SerializedProperty operation = property.FindPropertyRelative("operation");
            SerializedProperty scaleMode = property.FindPropertyRelative("scaleMode");

            SerializedProperty uniform = property.FindPropertyRelative("uniform");
            SerializedProperty axis = property.FindPropertyRelative("axis");

            SerializedProperty randomUniformMin = property.FindPropertyRelative("randomUniformMin");
            SerializedProperty randomUniformMax = property.FindPropertyRelative("randomUniformMax");
            SerializedProperty randomAxisMin = property.FindPropertyRelative("randomAxisMin");
            SerializedProperty randomAxisMax = property.FindPropertyRelative("randomAxisMax");

            SerializedProperty randomnessFrequency = property.FindPropertyRelative("randomnessFrequency");

            SerializedProperty byCurveDistance = property.FindPropertyRelative("byCurveDistance");
            SerializedProperty invertCurveDistance = property.FindPropertyRelative("invertCurveDistance");
            SerializedProperty minMaxDistance = property.FindPropertyRelative("minMaxDistance");
            SerializedProperty distanceScaleMultiplier = property.FindPropertyRelative("distanceScaleMultiplier");
            
            SerializedProperty overCurveLength = property.FindPropertyRelative("overCurveLength");
            SerializedProperty scaleOverLength = property.FindPropertyRelative("scaleOverLength");

            EditorGUILayout.PropertyField(operation, new GUIContent("Operation"));
            EditorGUILayout.PropertyField(scaleMode, new GUIContent("Mode"));

            bool hasZeroScale = false;
            var mode = (Scale.ScaleMode)scaleMode.enumValueIndex;

            if (mode == Scale.ScaleMode.Uniform)
            {
                EditorGUILayout.PropertyField(uniform, new GUIContent("Scale"));
                hasZeroScale |= uniform.floatValue <= 0;
            }
            else if (mode == Scale.ScaleMode.PerAxis)
            {
                EditorGUILayout.PropertyField(axis, new GUIContent("Scale"));
                hasZeroScale |= axis.vector3Value.magnitude <= 0;
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Randomization", EditorStyles.boldLabel);

            if (mode == Scale.ScaleMode.Uniform)
            {
                EditorGUILayout.PropertyField(randomUniformMin, new GUIContent("Min"));
                EditorGUILayout.PropertyField(randomUniformMax, new GUIContent("Max"));

                hasZeroScale |= randomUniformMin.floatValue <= 0;
                hasZeroScale |= randomUniformMax.floatValue <= 0;
            }
            else if (mode == Scale.ScaleMode.PerAxis)
            {
                EditorGUILayout.PropertyField(randomAxisMin, new GUIContent("Min"));
                EditorGUILayout.PropertyField(randomAxisMax, new GUIContent("Max"));

                hasZeroScale |= randomAxisMin.vector3Value.magnitude <= 0;
                hasZeroScale |= randomAxisMax.vector3Value.magnitude <= 0;
            }

            if (hasZeroScale)
            {
                EditorGUILayout.HelpBox("A scale value is 0 or lower, some or all objects may be infinitely small.", MessageType.Warning);
            }

            EditorGUILayout.PropertyField(randomnessFrequency, new GUIContent("Frequency"));

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("By curve distance", EditorStyles.boldLabel);

            EditorGUILayout.PropertyField(byCurveDistance, new GUIContent("Enable"));
            if (byCurveDistance.boolValue)
            {
                EditorGUILayout.PropertyField(invertCurveDistance, new GUIContent("Invert"));
                EditorGUILayout.PropertyField(minMaxDistance, new GUIContent("Min/max distance"));
                EditorGUILayout.PropertyField(distanceScaleMultiplier, new GUIContent("Multiplier"));
            }
            
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Over curve length", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(overCurveLength, new GUIContent("Enable"));
            if (overCurveLength.boolValue)
            {
                EditorGUILayout.PropertyField(scaleOverLength, new GUIContent("Curve"));
            }
            
            EditorGUI.EndProperty();
        }
    }
    
    [CustomPropertyDrawer(typeof(SnapToColliders))]
    public class DropModifierDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
    #if UNITY_2022_3_OR_NEWER
            EditorGUI.BeginProperty(position, label, property);

            using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar))
            {
                ModifierAttribute attributes = ModifierAttribute.GetFor(property);
                EditorGUILayout.LabelField(new GUIContent(attributes.displayName, attributes.description), EditorStyles.boldLabel);
            }
            
            SerializedProperty direction = property.FindPropertyRelative("direction");
            SerializedProperty rayHeightOffset = property.FindPropertyRelative("rayHeightOffset");
            SerializedProperty layerMask = property.FindPropertyRelative("layerMask");
            SerializedProperty positioning = property.FindPropertyRelative("positioning");
            SerializedProperty heightOffset = property.FindPropertyRelative("heightOffset");
            SerializedProperty alignRotationX = property.FindPropertyRelative("alignRotationX");
            SerializedProperty alignRotationZ = property.FindPropertyRelative("alignRotationZ");
            SerializedProperty slopeLimit = property.FindPropertyRelative("slopeLimit");
            
            EditorGUILayout.PropertyField(direction, new GUIContent("Direction"));
            EditorGUILayout.PropertyField(rayHeightOffset, new GUIContent("Ray height offset"));

            // Draw LayerMask as MaskField (tooltip optional)
            int maskValue = layerMask.intValue;
            maskValue = EditorGUILayout.MaskField(new GUIContent("Layer mask", "Layer(s) to drop onto."), maskValue, UnityEditorInternal.InternalEditorUtility.layers);
            layerMask.intValue = maskValue;

            if (maskValue == 0)
            {
                EditorGUILayout.HelpBox("Layer mask is zero, no raycasts will hit", MessageType.Warning);
            }

            EditorGUILayout.Space();

            EditorGUILayout.Slider(positioning, 0f, 1f, new GUIContent("Positioning"));

            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(heightOffset, new GUIContent("Height Offset"));
            EditorGUI.indentLevel--;

            EditorGUILayout.Slider(alignRotationX, 0f, 1f, new GUIContent("Align X rotation"));
            EditorGUILayout.Slider(alignRotationZ, 0f, 1f, new GUIContent("Align Z rotation"));

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(slopeLimit, new GUIContent("Slope limit"));

            EditorGUI.EndProperty();
    #else
            EditorGUILayout.HelpBox("Not supported in Unity 2021.3 or older. Requires Collections package v2.5.1+.", MessageType.Error);
    #endif
        }
    }
    
    [CustomPropertyDrawer(typeof(SnapToGrid))]
    public class SnapToGridModifierDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar))
            {
                ModifierAttribute attributes = ModifierAttribute.GetFor(property);
                EditorGUILayout.LabelField(new GUIContent(attributes.displayName, attributes.description), EditorStyles.boldLabel);
            }
            
            SerializedProperty gridSize = property.FindPropertyRelative("gridSize");
            SerializedProperty axis = property.FindPropertyRelative("axis");
            
            EditorGUILayout.Separator();

            EditorGUILayout.PropertyField(gridSize, new GUIContent("Grid size"));
            EditorGUILayout.PropertyField(axis, new GUIContent("Axis"));

            EditorGUI.EndProperty();
        }
    }

    [CustomPropertyDrawer(typeof(CurvatureFilter))]
    public class CurvatureFilterModifierDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            
            using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar))
            {
                ModifierAttribute attributes = ModifierAttribute.GetFor(property);
                EditorGUILayout.LabelField(new GUIContent(attributes.displayName, attributes.description), EditorStyles.boldLabel);
            }
            
            SerializedProperty minAngle = property.FindPropertyRelative("minAngle");
            SerializedProperty maxAngle = property.FindPropertyRelative("maxAngle");
            
            EditorGUILayout.Separator();

            EditorGUILayout.Slider(minAngle, 0f, 90f, new GUIContent("Min angle"));
            EditorGUILayout.Slider(maxAngle, 0f, 90f, new GUIContent("Max angle"));

            EditorGUI.EndProperty();
        }
    }
    
    [CustomPropertyDrawer(typeof(HeightFilter))]
    public class HeightFilterModifierDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            
            using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar))
            {
                ModifierAttribute attributes = ModifierAttribute.GetFor(property);
                EditorGUILayout.LabelField(new GUIContent(attributes.displayName, attributes.description), EditorStyles.boldLabel);
            }
            
            SerializedProperty minHeight = property.FindPropertyRelative("minHeight");
            SerializedProperty minFalloff = property.FindPropertyRelative("minFalloff");
            SerializedProperty maxHeight = property.FindPropertyRelative("maxHeight");
            SerializedProperty maxFalloff = property.FindPropertyRelative("maxFalloff");

            EditorGUILayout.Separator();
            
            EditorGUILayout.PropertyField(minHeight);
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(minFalloff, new GUIContent("Falloff", minFalloff.tooltip));
            EditorGUI.indentLevel--;
            
            EditorGUILayout.Separator();
            
            EditorGUILayout.PropertyField(maxHeight);
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(maxFalloff, new GUIContent("Falloff", maxFalloff.tooltip));
            EditorGUI.indentLevel--;

            EditorGUI.EndProperty();
        }
    }
    
    [CustomPropertyDrawer(typeof(Cull))]
    public class CullModifierPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar))
            {
                ModifierAttribute attributes = ModifierAttribute.GetFor(property);
                EditorGUILayout.LabelField(new GUIContent(attributes.displayName, attributes.description), EditorStyles.boldLabel);
            }
            
            SerializedProperty noiseFrequency = property.FindPropertyRelative("noiseFrequency");
            EditorGUILayout.PropertyField(noiseFrequency);
            SerializedProperty noiseOffset = property.FindPropertyRelative("noiseOffset");
            EditorGUILayout.PropertyField(noiseOffset);
            
            EditorGUILayout.Separator();
            
            SerializedProperty noiseCutoffProperty = property.FindPropertyRelative("noiseCutoff");
            EditorGUILayout.PropertyField(noiseCutoffProperty);
            SerializedProperty noiseFalloff = property.FindPropertyRelative("noiseFalloff");
            EditorGUILayout.PropertyField(noiseFalloff);
        }
    }
}