// Spline Spawner by Staggart Creations (http://staggart.xyz)
// COPYRIGHT PROTECTED UNDER THE UNITY ASSET STORE EULA (https://unity.com/legal/as-terms)
//  • Copying or referencing source code for the production of new asset store, or public, content is strictly prohibited!
//  • Uploading this file to a public GitHub repository will subject it to an automated DMCA takedown request.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.Splines;

#if !SPLINES
#error The Splines package has not been installed, or does not meet the minimum version requirement. The Spline Spawner package relies on this to be the case. To resolve this error install or update the Splines package through the Package Manager.
#endif

namespace sc.splines.spawner.runtime
{
    public partial class SplineSpawner : MonoBehaviour
    {
        [SerializeField]
        private int splineCount; //Change tracking
        private int containerID;
        
        //Creating a NativeSpline is costly, and isn't necessary if only spawning parameters are changed
        //Hence they are cached and rebuild when they change.
        private List<NativeSpline> nativeSplines = new List<NativeSpline>();
        
        //TODO: Further improve by caching Bounds and Length of splines, since calculating these involves resampling the entire spline

        public enum RespawningMode
        {
            [InspectorName("Manually")]
            None,
            DuringSplineChanges,
            AfterSplineChanges,
        }
        [Tooltip("When respawning should occur. For best performance in the editor, opt to use \"AfterSplineChanges\"")]
        public RespawningMode respawningMode = RespawningMode.DuringSplineChanges;
        
        private partial void SubscribeSplineCallbacks()
        {
            SplineContainer.SplineAdded += OnSplineAdded;
            SplineContainer.SplineRemoved += OnSplineRemoved;
            Spline.Changed += OnSplineChanged;

            RebuildSplineCache();
        }

        private partial void UnsubscribeSplineCallbacks()
        {
            SplineContainer.SplineAdded -= OnSplineAdded;
            SplineContainer.SplineRemoved -= OnSplineRemoved;
            Spline.Changed -= OnSplineChanged;
            
            DisposeSplineCache();
        }

        /// <summary>
        /// Spline are converted to native arrays for fast parallel read access. The conversion process is fairly slow, use this function to perform it for all splines within the assigned container.
        /// Doing so before respawning makes the first respawning job faster
        /// </summary>
        public void WarmupSplineCache()
        {
            RebuildSplineCache();
        }
        
        void RebuildSplineCache()
        {
            if (!splineContainer) return;
            
            //When first adding the component, ensure count is updated
            splineCount = splineContainer.Splines.Count;
            
            nativeSplines = new List<NativeSpline>();
            
            DisposeSplineCache();
            
            foreach (var spline in splineContainer.Splines)
            {
                CacheSpline(spline);
            }
        }

        void DisposeSplineCache()
        {
            foreach (var nativeSpline in nativeSplines)
            {
                nativeSpline.Dispose();
            }
            nativeSplines.Clear();
        }

        private void RemoveSpline(int index)
        {
            if (index >= nativeSplines.Count) return;
            
            nativeSplines[index].Dispose();
            nativeSplines.RemoveAt(index);
        }

        private NativeSpline CreateNativeSpline(ISpline spline)
        {
            return new NativeSpline(spline, spline.Closed, splineContainer.transform.localToWorldMatrix, true, Allocator.Persistent);
        }
        
        private void UpdateSpline(Spline spline, int index)
        {
            if (index >= nativeSplines.Count) return;

            nativeSplines[index].Dispose();
            nativeSplines[index] = CreateNativeSpline(spline);
        }
        
        private void CacheSpline(Spline spline)
        {
            NativeSpline nativeSpline = CreateNativeSpline(spline);
            nativeSplines.Add(nativeSpline);
        }
        
        private Spline lastEditedSpline;
        private int lastEditedSplineIndex = -1;
        
        private void OnSplineChanged(Spline spline, int knotIndex, SplineModification modificationType)
        {
            if (!splineContainer) return;

            //if (rebuildTriggers.HasFlag(RebuildTriggers.OnSplineChanged) == false) return;

            //Spline belongs to the assigned container?
            var splineIndex = Array.IndexOf(splineContainer.Splines.ToArray(), spline);
            if (splineIndex < 0)
                return;

            splineCount = splineContainer.Splines.Count;
            
            int hashCode = splineContainer.GetHashCode();
            if (hashCode != containerID)
            {
                ValidateContainers();
            }
            containerID = splineContainer.GetHashCode();

            lastEditedSpline = spline;
            lastEditedSplineIndex = splineIndex;
            
            if (respawningMode == RespawningMode.AfterSplineChanges)
            {
                lastChangeTime = Time.realtimeSinceStartup;

                if (Application.isPlaying)
                {
                    //Coroutines only work in play mode and builds
                    
                    //Cancel any existing debounce coroutine
                    if (debounceCoroutine != null) StopCoroutine(debounceCoroutine);
                
                    debounceCoroutine = StartCoroutine(DebounceCoroutine());
                }
                else
                {
                    if (!isTrackingChanges)
                    {
                        isTrackingChanges = true;
                        
                        #if UNITY_EDITOR
                        UnityEditor.EditorApplication.update += EditorUpdate;
                        #endif
                    }
                    
                }
            }
            else if (respawningMode == RespawningMode.DuringSplineChanges)
            {
                ExecuteAfterSplineChanges();
            }
        }
        
        /// <summary>
        /// Time in milliseconds after the last change to the spline until respawning occurs. Applies only when the <see cref="respawningMode"/> is set to "After Spline Changes"
        /// </summary>
        public float debounceTime = 100f;

        private float lastChangeTime = -1f;
        private bool isTrackingChanges = false;
        
        private void EditorUpdate()
        {
            if (isTrackingChanges && Time.realtimeSinceStartup - lastChangeTime >= (debounceTime * 0.001f))
            {
                ExecuteAfterSplineChanges();
                
                isTrackingChanges = false;
                #if UNITY_EDITOR
                UnityEditor.EditorApplication.update -= EditorUpdate;
                #endif
            }
            
        }
        
        private Coroutine debounceCoroutine;
        private IEnumerator DebounceCoroutine()
        {
            yield return new WaitForSeconds((debounceTime * 0.001f));
            
            ExecuteAfterSplineChanges();
        }

        private void ExecuteAfterSplineChanges()
        {
            if(lastEditedSplineIndex < 0) return;
            
            Profiler.BeginSample(PROFILER_PREFIX + "Update Spline Data");
            {
                UpdateSpline(lastEditedSpline, lastEditedSplineIndex);
            }
            Profiler.EndSample();
            
            Respawn(lastEditedSplineIndex);
        }
        
        private void OnSplineAdded(SplineContainer container, int index)
        {
            if (!splineContainer) return;
            
            //if (rebuildTriggers.HasFlag(RebuildTriggers.OnSplineAdded) == false) return;

            if (container.GetHashCode() != splineContainer.GetHashCode())
                return;

            //Inserting a new Knot also triggers this function, bail out if no new spline was actually added
            //Should still execute, since it may be that a number of splines were assigned that are totally different
            //if (splineCount == splineContainer.Splines.Count) return;
            
            splineCount = splineContainer.Splines.Count;

            containers.Add(SplineInstanceContainer.Create(this, index));
            
            CacheSpline(container.Splines[index]);
            
            //Causes issues with SendMessage. Adding splines must never be done from an OnValidate function
            Respawn(index);
        }

        private void OnSplineRemoved(SplineContainer targetContainer, int index)
        {
            if (!splineContainer) return;
            
            if (targetContainer.GetHashCode() != splineContainer.GetHashCode())
                return;
            
            splineCount = splineContainer.Splines.Count;

            if (index <= nativeSplines.Count)
            {
                RemoveSpline(index);

                if (index < containers.Count && index >= 0)
                {
                    //Debug.Log($"Spline removed at index {index}. Container count: {containers.Count}");
                    SplineInstanceContainer container = containers[index];
                    container.Destroy();
                    
                    containers.RemoveAt(index);
                }
                else
                {
                    //throw new Exception($"Error when removing Spline #{index} from {targetContainer.name}. Index out of range ({containers.Count} containers).");
                }
            }

            //No need to respawn, the container for the spline has been removed and all objects with it
        }
    }
}