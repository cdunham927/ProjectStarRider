// Spline Spawner by Staggart Creations (http://staggart.xyz)
// COPYRIGHT PROTECTED UNDER THE UNITY ASSET STORE EULA (https://unity.com/legal/as-terms)
//  • Copying or referencing source code for the production of new asset store, or public, content is strictly prohibited!
//  • Uploading this file to a public GitHub repository will subject it to an automated DMCA takedown request.

using Unity.Mathematics;

namespace sc.splines.spawner.runtime
{
    public struct SpawnPoint
    {
        public bool isValid;
            
        public int prefabIndex;
        public float3 pivotOffset;
        public float3 position;
        public quaternion rotation;
        /// <summary>
        /// Scale of the object's bounds
        /// </summary>
        public float3 scale;
            
        public struct Context
        {
            public float t;
            public float splineLength;
            public float random01;
            public float2 noiseCoord;
            public float curvature;

            public float3 position;
            public float3 forward;
            public float3 right;
            public float3 up;
            
            public bool invertDistance;
        }
        public Context context;
    }
}