// Spline Spawner by Staggart Creations (http://staggart.xyz)
// COPYRIGHT PROTECTED UNDER THE UNITY ASSET STORE EULA (https://unity.com/legal/as-terms)
//  • Copying or referencing source code for the production of new asset store, or public, content is strictly prohibited!
//  • Uploading this file to a public GitHub repository will subject it to an automated DMCA takedown request.

using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;
using Debug = UnityEngine.Debug;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine.Rendering;

namespace sc.splines.spawner.runtime
{
    [ExecuteAlways]
    [Icon(SplineSpawner.kPackageRoot + "/Editor/Resources/spline-spawner-mask-icon-64px.psd")]
    public class SplineSpawnerMask : MonoBehaviour
    {
        /// <summary>
        /// List of all SplineSpawnerMask instances in the scene
        /// </summary>
        public static readonly List<SplineSpawnerMask> Instances = new List<SplineSpawnerMask>(32);
        
        //Avoids streaking when sampling at the edges
        //Also to allow for sampling wide margins
        private const float BOUNDS_PADDING = 5f;
        
        //Public properties
        [Tooltip("The SplineContainer that defines the shape of the mask")]
        public SplineContainer splineContainer;
        public enum Precision
        {
            VeryLow,
            Low,
            Normal,
            High,
            VeryHigh
        }
        [Tooltip("Controls the resolution/quality of the mask generation. High precision is rarely needed, opt for the lowest precision viable as this increases processing speeds as well.")]
        public Precision precision = Precision.Normal;

        [SplineSpawnerMask.MaskLayer(true)]
        [Tooltip("Layer this mask is on, for spawners to filter with")]
        public int layers = -1;

        [Tooltip("When enabled, inverts the mask effect")]
        public bool invert;

        public enum Shape
        {
            Line,
            Area
        }
        [Tooltip("Determines if the mask follows the spline line or fills the enclosed area")]
        public Shape shape = Shape.Line;
        [Tooltip("Distance in units that the mask extends from the spline curve")]
        public float margin = 2f;
        [Tooltip("Controls how rounded the ends of line segments are (0 = sharp, 1 = fully rounded)")]
        [Range(0f,1f)]
        public float capRounding = 1f;
        
        [Tooltip("Controls the visibility of the mask overlay in the scene view")]
        [Range(0f,1f)]
        public float overlay = 0.25f;
        
        [Tooltip("Compute shader used for mask generation")]
        [HideInInspector]
        public ComputeShader computeShader;
        
        [SerializeField] 
        [HideInInspector]
        internal Vector3 boundsCenter;
        [SerializeField]
        [HideInInspector]
        internal Vector3 boundsSize;
        [SerializeField]
        [HideInInspector]
        internal int2 textureResolution;
        [SerializeField]
        [HideInInspector]
        private float texelSize;

        [NonSerialized]
        public RenderTexture sdf;

        private NativeArray<float> sdfData;

        private bool isDirty;
        private float lastRenderTime;
        public float LastRenderTime => lastRenderTime;

        private const string CS_RENDER_SDF_KERNEL_NAME = "RenderSDF";
        private const string CS_SAMPLE_SDF_KERNEL_NAME = "SampleSDF";
        
        [AttributeUsage(AttributeTargets.Field)]
        public class MaskLayerAttribute : PropertyAttribute
        {
            public readonly bool showEditButton;
            public bool multiSelect;

            public MaskLayerAttribute(bool multiSelect, bool showEditButton = true)
            {
                this.multiSelect = multiSelect;
                this.showEditButton = showEditButton;
            }
        }
        
        void Reset()
        {
            splineContainer = GetComponent<SplineContainer>();
        }

        private void OnEnable()
        {
            Instances.Add(this);
            
            Spline.Changed += OnSplineChanged;
            SplineContainer.SplineAdded += OnSplineCountChanged;
            SplineContainer.SplineRemoved += OnSplineCountChanged;

            if (RequiresUpdate())
            {
                //Debug.Log($"SplineSpawnerMask requires rendering its SDF {RequiresUpdate()} OnEnable");
                //RenderSDFIfNeeded();
            }
        }


        void UpdateBounds()
        {
            int splineCount = splineContainer.Splines.Count;
            
            Vector3 min = Vector3.one * float.MaxValue;
            Vector3 max = Vector3.one * float.MinValue;
            
            for (int i = 0; i < splineCount; i++)
            {
                //Note: Also includes tangents, but provides a natural amount of padding
                Bounds splineBounds = splineContainer[i].GetBounds(splineContainer.transform.localToWorldMatrix);

                min = Vector3.Min(splineBounds.min, min);
                max = Vector3.Max(splineBounds.max, max);
            }

            Bounds bounds = new Bounds();
            bounds.SetMinMax(min, max);

            float m_margin = Mathf.Max(0, margin) + texelSize + BOUNDS_PADDING;
            m_margin *= Mathf.Max(m_margin, texelSize);

            m_margin = Mathf.Abs(m_margin);

            Vector3 padding = new Vector3(m_margin, 0f, m_margin);
            bounds.Expand(padding);
            
            boundsCenter = bounds.center;
            boundsCenter.y = this.transform.position.y;
            boundsSize = bounds.size;
            boundsSize.y = 0f;
        }

        public void Setup()
        {
            UpdateBounds();

            texelSize = precision switch
            {
                Precision.VeryLow => 2f,
                Precision.Low => 1.5f,
                Precision.Normal => 1f,
                Precision.High => 0.75f,
                Precision.VeryHigh => 0.5f,
                _ => 1f
            };

            textureResolution.x = (int)Mathf.CeilToInt(boundsSize.x / texelSize);
            textureResolution.y = (int)Mathf.CeilToInt(boundsSize.z / texelSize);
            
            textureResolution.x = Mathf.Max(4, textureResolution.x);
            textureResolution.y = Mathf.Max(4, textureResolution.y);
            
            bool resized = sdf && ((sdf.width != textureResolution.x || sdf.height != textureResolution.y));
        
            if (resized)
            {
                sdf.Release();
            }

            if (!sdf || resized)
            {
                sdf = new RenderTexture(textureResolution.x, textureResolution.y, 0, RenderTextureFormat.RFloat);
                sdf.wrapMode = TextureWrapMode.Clamp;
                //sdf.filterMode = FilterMode.Point;
                sdf.enableRandomWrite = true;
                sdf.Create();
            }
        }

        private void OnSplineChanged(Spline spline, int knotIndex, SplineModification arg3)
        {
            if (!splineContainer) return;

            if (splineContainer.Splines.Contains(spline))
            {
                isDirty = true;
                
                RenderSDFIfNeeded();
                RespawnAffectedSpawners();
            }
        }
        
        private void OnSplineCountChanged(SplineContainer container, int splineCount)
        {
            if (!splineContainer) return;

            if (splineContainer != container) return;
            
            isDirty = true;
                
            RenderSDFIfNeeded();
            RespawnAffectedSpawners();
        }
        
        public bool RequiresUpdate()
        {
            return isDirty || !sdf || boundsSize == Vector3.zero;
        }
        
        public void ForceUpdate()
        {
            isDirty = true;
            RenderSDFIfNeeded();
        }
        
        List<Vector3> splinePositions = new List<Vector3>();
        int2[] positionIndices = Array.Empty<int2>();
        
        public void RenderSDFIfNeeded()
        {
            if (RequiresUpdate() == false || !splineContainer || !computeShader) return;

            //Do this immediately
            isDirty = false;
            
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            
            int splineCount = splineContainer.Splines.Count;

            if (splineCount == 0)
            {
                return;
            }
            
            //Calculate bounds and set up RT
            Setup();

            if (splinePositions == null)
                splinePositions = new List<Vector3>(splineCount * 1000);
            else
                splinePositions.Clear();
            
            if (positionIndices == null || positionIndices.Length != splineCount)
                positionIndices = new int2[splineCount];

            float sampleDistance = (texelSize * texelSize);

            //Sample all the splines, store the positions and their start- & end indices in the array
            int lastIndex = 0;
            for (int i = 0; i < splineCount; i++)
            {
                float splineLength = splineContainer[i].CalculateLength(splineContainer.transform.localToWorldMatrix);
                int sampleCount = Mathf.CeilToInt(splineLength / sampleDistance);

                for (int j = 0; j < sampleCount; j++)
                {
                    float t = j / (float)(sampleCount-1);

                    float3 position = splineContainer[i].EvaluatePosition(t);
                    position = splineContainer.transform.TransformPoint(position);

                    splinePositions.Add(position);
                }
                
                positionIndices[i] = new int2(lastIndex, (lastIndex) + sampleCount);
                lastIndex = positionIndices[i].y;
            }

            if (computeShader.HasKernel(CS_RENDER_SDF_KERNEL_NAME) == false)
            {
                Debug.LogError($"Compute shader does not have a kernel named {CS_RENDER_SDF_KERNEL_NAME}", this);
                return;
            }
            int kernel = computeShader.FindKernel(CS_RENDER_SDF_KERNEL_NAME);
            
            int splinePointCount = splinePositions.Count;
            ComputeBuffer splineBuffer = new ComputeBuffer(splinePointCount, sizeof(float) * 3);
            splineBuffer.SetData(splinePositions);

            ComputeBuffer splineSampleIndicesBuffer = new ComputeBuffer(splineCount, sizeof(int) * 2);
            splineSampleIndicesBuffer.SetData(positionIndices);

            //Command buffer
            CommandBuffer cmd = new CommandBuffer();
            cmd.name = "Render SDF";

            //Compute shader parameters
            cmd.SetComputeBufferParam(computeShader, kernel, "_PositionIndices", splineSampleIndicesBuffer);
            cmd.SetComputeIntParam(computeShader, "_SplineCount", splineCount);
            cmd.SetComputeTextureParam(computeShader, kernel, "_SDF", sdf);
            cmd.SetComputeBufferParam(computeShader, kernel, "_SplinePoints", splineBuffer);
            
            cmd.SetComputeFloatParam(computeShader, "_Margin", shape == SplineSpawnerMask.Shape.Area ? -margin : margin);
            cmd.SetComputeIntParam(computeShader, "_UseFill", shape == SplineSpawnerMask.Shape.Area ? 1 : 0);
            cmd.SetComputeFloatParam(computeShader, "_CapBlend", capRounding);
            cmd.SetComputeIntParam(computeShader, "_Invert", invert ? 1 : 0);

            cmd.SetComputeVectorParam(computeShader, "_AreaCenter", boundsCenter);
            cmd.SetComputeVectorParam(computeShader, "_AreaSize", boundsSize);
            cmd.SetComputeVectorParam(computeShader, "_Resolution", new Vector4(textureResolution.x, textureResolution.y));

            cmd.SetComputeIntParam(computeShader, "_IsClosedSpline", shape == SplineSpawnerMask.Shape.Area ? 1 : 0);

            int threadGroupsX = Mathf.CeilToInt(textureResolution.x / 8.0f);
            int threadGroupsY = Mathf.CeilToInt(textureResolution.y / 8.0f);

            //Dispatch compute shader
            cmd.DispatchCompute(computeShader, kernel, threadGroupsX, threadGroupsY, 1);

            //Execute command buffer
            Graphics.ExecuteCommandBuffer(cmd);

            //Clean up
            cmd.Release();
            splineSampleIndicesBuffer.Release();
            splineBuffer.Release();

            stopwatch.Stop();
            lastRenderTime = (float)stopwatch.Elapsed.TotalMilliseconds;
        }

        public float[] SampleSDF(Vector3[] spawnPositions)
        {
            int spawnPointCount = spawnPositions.Length;

            if (spawnPointCount == 0) return Array.Empty<float>();
            
            float[] samples = new float[spawnPointCount];

            if (splineContainer.Splines.Count == 0 || !computeShader)
            {
                for (int i = 0; i < samples.Length; i++)
                {
                    samples[i] = 1f;
                }
                return samples;
            }
            
            //In case this function is called straight away
            RenderSDFIfNeeded();

            ComputeBuffer positionBuffer = new ComputeBuffer(spawnPointCount, sizeof(float) * 3);
            positionBuffer.SetData(spawnPositions);

            ComputeBuffer sampleResultBuffer = new ComputeBuffer(spawnPointCount, sizeof(float));
            sampleResultBuffer.SetData(samples);

            int kernel = computeShader.FindKernel(CS_SAMPLE_SDF_KERNEL_NAME);

            //Command buffer
            CommandBuffer cmd = new CommandBuffer();
            cmd.name = "Sample SDF";
            
            cmd.SetComputeBufferParam(computeShader, kernel, "_SampleResults", sampleResultBuffer);
            cmd.SetComputeBufferParam(computeShader, kernel, "_SamplePositions", positionBuffer);
            cmd.SetComputeIntParam(computeShader, "_SamplePositionCount", spawnPointCount);

            cmd.SetComputeVectorParam(computeShader, "_AreaCenter", boundsCenter);
            cmd.SetComputeVectorParam(computeShader, "_AreaSize", boundsSize);
            cmd.SetComputeVectorParam(computeShader, "_Resolution", new Vector4(textureResolution.x, textureResolution.y));

            cmd.SetComputeTextureParam(computeShader, kernel, "_SDF", sdf);

            //Dispatch
            cmd.DispatchCompute(computeShader, kernel, Mathf.CeilToInt(spawnPointCount / 64.0f), 1, 1);

            //Execute
            Graphics.ExecuteCommandBuffer(cmd);

            //Get results
            sampleResultBuffer.GetData(samples);

            //Cleanup
            cmd.Release();
            positionBuffer.Dispose();
            sampleResultBuffer.Dispose();

            return samples;
        }

        public bool IsOnLayer(int layer)
        {
            return (layers & (1 << layer)) != 0;
        }
        
        public bool IsOnLayers(int layerMask)
        {
            //Nothing
            if (layers == 0 || layerMask == 0) return false;
            
            return (layers & layerMask) != 0;
        }

        public static bool ContainsLayer(int layerMask, int layer)
        {
            return (layerMask & (1 << layer)) != 0;
        }
        
        public List<SplineSpawner> GetSpawnersAffectedBy()
        {
            SplineSpawner[] spawners = FindObjectsByType<SplineSpawner>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);

            List<SplineSpawner> affected = new List<SplineSpawner>();
            for (int i = 0; i < spawners.Length; i++)
            {
                if(spawners[i].enabled == false || spawners[i].gameObject.activeSelf == false) continue;
                
                //Could not possibly spawn using a spline already used for masking
                if(spawners[i].splineContainer == splineContainer) continue;
                
                //TODO: Check for overlap first

                var hasLayer = false;
                for (int j = 0; j < spawners[i].maskRules.Length; j++)
                {
                    SplineSpawner.MaskRule rule = spawners[i].maskRules[j];

                    //Check this mask is on the layer configured on the spawner
                    hasLayer |= IsOnLayer(rule.layer);
                }
                
                if (hasLayer)
                {
                    affected.Add(spawners[i]);
                }
            }

            return affected;
        }
        
        public void RespawnAffectedSpawners()
        {
            List<SplineSpawner> spawners = GetSpawnersAffectedBy();

            foreach (SplineSpawner spawner in spawners)
            {
                spawner.Respawn();
            }
        }
        
        private void OnDisable()
        {
            Instances.Remove(this);

            if(sdfData.IsCreated) sdfData.Dispose();

            Spline.Changed -= OnSplineChanged;
            SplineContainer.SplineAdded -= OnSplineCountChanged;
            SplineContainer.SplineRemoved -= OnSplineCountChanged;

            RespawnAffectedSpawners();
        }

        [NonSerialized]
        private Material overlayMaterial;
        private CommandBuffer overlayCommandBuffer;
        private void OnDrawGizmosSelected()
        {
            if (!splineContainer) return;
            
            if (splineContainer.transform.hasChanged || this.transform.hasChanged)
            {
                splineContainer.transform.hasChanged = false;
                this.transform.hasChanged = false;
                
                ForceUpdate();
            }
            
            Gizmos.DrawWireCube(boundsCenter, boundsSize);

            if (overlay > 0f && sdf)
            {
                if (!overlayMaterial) overlayMaterial = new Material(Shader.Find("Hidden/MaskOverlay"));
                
                overlayMaterial.SetPass(0);
                overlayMaterial.SetTexture("_MainTex", sdf);
                overlayMaterial.SetFloat("_CellSize", texelSize);
                overlayMaterial.SetFloat("_Alpha", overlay);
                
                //Draw the plane with the correct scaling based on bounds
                float halfWidth = (boundsSize.x) * 0.5f;
                float halfHeight = (boundsSize.z) * 0.5f;
                Vector3 center = boundsCenter;
                
                //Four corners of the plane, matching the bounds
                Vector3 p0 = new Vector3(center.x - halfWidth, center.y, center.z - halfHeight);
                Vector3 p1 = new Vector3(center.x + halfWidth, center.y, center.z - halfHeight);
                Vector3 p2 = new Vector3(center.x + halfWidth, center.y, center.z + halfHeight);
                Vector3 p3 = new Vector3(center.x - halfWidth, center.y, center.z + halfHeight);
                
                Vector3[] corners = new Vector3[4] { p0, p1, p2, p3 };
                
                Mesh quadMesh = new Mesh();
                quadMesh.vertices = corners;
                quadMesh.triangles = new int[] { 0, 2, 1, 0, 3, 2 };
                quadMesh.uv = new Vector2[]
                {
                    new Vector2(0, 0), // Bottom Left
                    new Vector2(1, 0), // Bottom Right
                    new Vector2(1, 1), // Top Right
                    new Vector2(0, 1)  // Top Left
                };

                if(overlayCommandBuffer == null) overlayCommandBuffer = new CommandBuffer();
                overlayCommandBuffer.DrawMesh(quadMesh, Matrix4x4.identity, overlayMaterial, 0, 0);
                
                Graphics.ExecuteCommandBuffer(overlayCommandBuffer);
                overlayCommandBuffer.Clear();
                
                /*
                GL.PushMatrix();
                GL.Begin(GL.QUADS);

                

                GL.TexCoord2(0f, 1f);
                GL.Vertex(p3);
                GL.TexCoord2(1f, 1f);
                GL.Vertex(p2);
                GL.TexCoord2(1f, 0f);
                GL.Vertex(p1);
                GL.TexCoord2(0f, 0f);
                GL.Vertex(p0);

                GL.End();
                GL.PopMatrix();
                */
            }
        }
    }
}