// Spline Spawner by Staggart Creations (http://staggart.xyz)
// COPYRIGHT PROTECTED UNDER THE UNITY ASSET STORE EULA (https://unity.com/legal/as-terms)
//  • Copying or referencing source code for the production of new asset store, or public, content is strictly prohibited!
//  • Uploading this file to a public GitHub repository will subject it to an automated DMCA takedown request.

using System;
using sc.splines.spawner.runtime;
using UnityEditor;
using UnityEngine;

namespace sc.splines.spawner.editor
{
    public class AssetInfo
    {
        public const int ASSET_ID = 305974;
        public const string ASSET_NAME = "Spline Spawner";
        
        public const string DOC_URL = "https://staggart.xyz/support/documentation/spline-spawner/";
        public const string FORUM_URL = "https://discussions.unity.com/t/1659478";
        public const string DISCORD_INVITE_URL = "https://discord.gg/GNjEaJc8gw";

        private const string MIN_SPLINES_VERSION = "2.8.1";
        
        private static bool STARTUP_PERFORMED
        {
            get => SessionState.GetBool("SPLINE_SPAWNER_EDITOR_STARTED", false);
            set => SessionState.SetBool("SPLINE_SPAWNER_EDITOR_STARTED", value);
        }
        
        [InitializeOnLoadMethod]
        static void Initialize()
        {
            #if !SPLINES
            if (EditorUtility.DisplayDialog(ASSET_NAME, $"This asset requires the \"Splines\" (v{MIN_SPLINES_VERSION}) package dependency, which is not installed.", "Install now", "Later"))
            {
                //Note: Mathematics package is a dependency, so will also be installed
                UnityEditor.PackageManager.Client.Add($"com.unity.splines@{MIN_SPLINES_VERSION}");
            }
            #endif
            
            #if !SP_DEV
            if (STARTUP_PERFORMED == false)
            #endif
            {
                VersionChecking.CheckForUpdate();
                STARTUP_PERFORMED = true;
            }
        }
        
        public static void OpenInPackageManager()
        {
            Application.OpenURL("com.unity3d.kharma:content/" + ASSET_ID);
        }
        
        public static void OpenReviewsPage()
        {
            Application.OpenURL($"https://assetstore.unity.com/packages/slug/{ASSET_ID}?aid=1011l7Uk8&pubref=speditor#reviews");
        }
        
        internal static class VersionChecking
        {
            public static bool UPDATE_AVAILABLE
            {
                get => SessionState.GetBool("SPLINE_SPAWNER_UPDATE_AVAILABLE", false);
                set => SessionState.SetBool("SPLINE_SPAWNER_UPDATE_AVAILABLE", value);
            }
            
            public static string latestVersion = SplineSpawner.VERSION;
            private static string apiResult;

            public static void CheckForUpdate()
            {
                //Default, in case of a fail
                UPDATE_AVAILABLE = false;
                
                //Offline
                if (Application.internetReachability == NetworkReachability.NotReachable) return;
                
                //Debug.Log("Checking for version update");
                
                var url = $"https://api.assetstore.unity3d.com/package/latest-version/{ASSET_ID}";

                using (System.Net.WebClient webClient = new System.Net.WebClient())
                {
                    webClient.DownloadStringCompleted += OnRetrievedAPIContent;
                    webClient.DownloadStringAsync(new System.Uri(url), apiResult);
                }
            }

            private class AssetStoreItem
            {
                public string name;
                public string version;
            }

            private static void OnRetrievedAPIContent(object sender, System.Net.DownloadStringCompletedEventArgs e)
            {
                if (e.Error == null && !e.Cancelled)
                {
                    string result = e.Result;

                    AssetStoreItem asset = (AssetStoreItem)JsonUtility.FromJson(result, typeof(AssetStoreItem));

                    latestVersion = asset.version;

                    Version remoteVersion = new Version(asset.version);
                    Version installedVersion = new Version(SplineSpawner.VERSION);

                    UPDATE_AVAILABLE = remoteVersion > installedVersion;

                    if (UPDATE_AVAILABLE)
                    {
                        //Debug.Log($"[{asset.name} v{installedVersion}] New version ({asset.version}) is available");
                    }
                }
            }
        }
    }
}