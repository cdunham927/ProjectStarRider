// Spline Spawner by Staggart Creations (http://staggart.xyz)
// COPYRIGHT PROTECTED UNDER THE UNITY ASSET STORE EULA (https://unity.com/legal/as-terms)
//  • Copying or referencing source code for the production of new asset store, or public, content is strictly prohibited!
//  • Uploading this file to a public GitHub repository will subject it to an automated DMCA takedown request.

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace sc.splines.spawner.runtime
{
    [Serializable]
    public abstract class Modifier
    {
        public bool enabled = true;

        protected const int DEFAULT_BATCHSIZE = 32;
        
        public static Modifier Create(Type type)
        {
            Modifier modifier = null;
            
            if (type != null && type.IsSubclassOf(typeof(Modifier)))
            {
                try
                {
                    modifier = (Modifier)Activator.CreateInstance(type);
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
                
                return modifier;
            }

            Debug.LogWarning($"Failed to add modifier of type \"{type}\". It needs to derive from the Modifier class");
            
            return null;
        }
        
        public enum Space
        {
            SplineCurve,
            Object,
            World
        }
        
        [Flags]
        public enum Axis
        {
            None = 0,
            X = 1,
            Y = 2,
            Z = 4
        }

        public enum RandomMode
        {
            RandomBetween,
            Alternate
        }
        
        public sealed class MinMaxSlider : PropertyAttribute
        {
            public float min;
            public float max;

            public MinMaxSlider(float min, float max)
            {
                this.min = min;
                this.max = max;
            }
        }

        public virtual JobHandle CreateJob(SplineSpawner spawner, ref NativeList<SpawnPoint> spawnPoints)
        {
            return default(JobHandle);
        }

        public virtual void Dispose()
        {
            
        }

        public static Type[] types;
        public static string[] names;

        /// <summary>
        /// Finds all the types inherited from the Modifier class
        /// </summary>
        #if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
        #endif
        public static void LoadAllInProject()
        {
            List<Type> foundTypes = new List<Type>();
            List<string> typeNames = new List<string>();
                
            var allTypes = new List<Type>();
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            
            foreach (var assembly in assemblies)
            {
                Type[] assemblyTypes = assembly.GetTypes();
                foreach (Type type in assemblyTypes)
                {
                    //Base 'Modifier' class will be abstract, skip it
                    if (type.IsAbstract) continue;

                    if (type.IsSubclassOf(typeof(Modifier)))
                        allTypes.Add(type);
                }
            }

            foreach (Type t in allTypes)
            {
                ModifierAttribute attribute = (ModifierAttribute)t.GetCustomAttribute(typeof(ModifierAttribute));

                if (attribute == null)
                {
                    Debug.LogError($"The modifier class \"{t.Name}\" has no [Modifier] attribute. This is required to specify its name and description.");
                    continue;
                }
                
                //Insert blank space in between camel case strings
                string name = Regex.Replace(Regex.Replace(attribute.displayName, "([a-z])([A-Z])", "$1 $2", RegexOptions.Compiled),
                    "([A-Z])([A-Z][a-z])", "$1 $2", RegexOptions.Compiled);
                    
                foundTypes.Add(t);
                typeNames.Add(name);
            }
                
            types = foundTypes.ToArray();
            names = typeNames.ToArray();
        }
    }
}