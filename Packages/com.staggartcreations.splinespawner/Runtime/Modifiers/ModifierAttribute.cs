// Spline Spawner by Staggart Creations (http://staggart.xyz)
// COPYRIGHT PROTECTED UNDER THE UNITY ASSET STORE EULA (https://unity.com/legal/as-terms)
//  • Copying or referencing source code for the production of new asset store, or public, content is strictly prohibited!
//  • Uploading this file to a public GitHub repository will subject it to an automated DMCA takedown request.

using System;
using System.Reflection;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace sc.splines.spawner.runtime
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ModifierAttribute : Attribute
    {
        public readonly string displayName;
        public readonly string description;
        public readonly DistributionSettings.DistributionMode[] incompatibleDistributionModes;
        
        public ModifierAttribute(string displayName, string description, DistributionSettings.DistributionMode[] disallowedDistributionModes = null)
        {
            this.displayName = displayName;
            this.description = description;
            this.incompatibleDistributionModes = disallowedDistributionModes;
        }

        public bool IsIncompatibleDistributionMode(DistributionSettings.DistributionMode mode)
        {
            if (incompatibleDistributionModes == null) return false;
            
            for (int i = 0; i < incompatibleDistributionModes.Length; i++)
            {
                if(incompatibleDistributionModes[i] == mode) return true;
            }

            return false;
        }

        #if UNITY_EDITOR
        public static ModifierAttribute GetFor(SerializedProperty modifierProperty)
        {
            Modifier modifier = modifierProperty.managedReferenceValue as Modifier;
            return GetFor(modifier.GetType());
        }
        #endif
        
        public static ModifierAttribute GetFor(Type type)
        {
            return (ModifierAttribute)type.GetCustomAttribute(typeof(ModifierAttribute));
        }
    }
}