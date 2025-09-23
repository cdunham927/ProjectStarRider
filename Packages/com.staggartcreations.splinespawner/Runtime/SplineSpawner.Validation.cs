// Spline Spawner by Staggart Creations (http://staggart.xyz)
// COPYRIGHT PROTECTED UNDER THE UNITY ASSET STORE EULA (https://unity.com/legal/as-terms)
//  • Copying or referencing source code for the production of new asset store, or public, content is strictly prohibited!
//  • Uploading this file to a public GitHub repository will subject it to an automated DMCA takedown request.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace sc.splines.spawner.runtime
{
    public partial class SplineSpawner : MonoBehaviour
    {
        private void ValidateContainers()
        {
            containers.RemoveAll(item => !item || item.transform.parent != root);

            SplineInstanceContainer[] childContainers = this.gameObject.GetComponentsInChildren<SplineInstanceContainer>();

            int adopted = 0;
            for (int i = 0; i < childContainers.Length; i++)
            {
                SplineInstanceContainer container = childContainers[i];
                
                if (container.owner == null || container.owner != this)
                {
                    container.owner = this;
                    adopted++;
                }
            }
            
            if(adopted > 0) Debug.Log($"[Spline Spawner] {adopted} orphaned instances containers were found under {this.name}. So they have been adopted. This may happen when duplicating a Spline Spawner");
            
            if (splineCount != containers.Count)
            {
                //Debug.LogWarning($"Mismatching number of object containers ({containers.Count}) relative to the number of splines ({splineCount}). Recreating them now. This may happen if containers are manually deleted, or the spline container was changed.");
                
                for (var i = 0; i < containers.Count; i++)
                {
                    containers[i].Destroy();
                }
                
                containers.Clear();
                for (int i = 0; i < splineCount; i++)
                {
                    containers.Add(SplineInstanceContainer.Create(this, i));
                }
            }
        }
        
        public bool HasMissingPrefabs()
        {
            for (int i = 0; i < inputObjects.Count; i++)
            {
                if (!inputObjects[i].prefab) return true;
            }

            return false;
        }
    }
}