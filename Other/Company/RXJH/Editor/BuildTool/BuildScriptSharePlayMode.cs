using System;
using System.IO;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.Initialization;


namespace UnityEditor.AddressableAssets.Build.DataBuilders
{
    [CreateAssetMenu(fileName = "BuildScriptSharePlayMode.asset", menuName = "Addressables/Content Builders/Use Share Build (requires share bundles)")]
    public class BuildScriptSharePlayMode : BuildScriptBase
    {
        /// <inheritdoc />
        public override string Name
        {
            get
            {
                return "Use Share Build (requires share bundles)";
            }
        }
        
        private bool m_DataBuilt;

        public override void ClearCachedData()
        {
            m_DataBuilt = false;
        }

        /// <inheritdoc />
        public override bool IsDataBuilt()
        {
            return m_DataBuilt;
        }

        /// <inheritdoc />
        public override bool CanBuildData<T>()
        {
            return typeof(T).IsAssignableFrom(typeof(AddressablesPlayModeBuildResult));
        }
        
        protected override TResult BuildDataImplementation<TResult>(AddressablesDataBuilderInput builderInput)
        {
            var timer = new System.Diagnostics.Stopwatch();
            timer.Start();
            //var settingsPath = Addressables.BuildPath + "/settings.json"; 
            //查找共享资源里的Settings.json路径
            DirectoryInfo info = new DirectoryInfo(Application.dataPath);
            string papaPath = info.Parent.Parent.FullName.Replace('\\','/');       
            string path = string.Format("{0}/{1}/{2}", papaPath, "AddressableBundle",PlatformMappingService.GetPlatformPathSubFolder());
            var settingsPath = path  +"/settings.json";
            
            if (!File.Exists(settingsPath))
            {
                IDataBuilderResult resE = new AddressablesPlayModeBuildResult() { Error = "Player content must be built before entering play mode with packed data.  This can be done from the Addressables window in the Build->Build Player Content menu command." };
                return (TResult)resE;
            }
            var rtd = JsonUtility.FromJson<ResourceManagerRuntimeData>(File.ReadAllText(settingsPath));
            if (rtd == null)
            {
                IDataBuilderResult resE = new AddressablesPlayModeBuildResult() { Error = string.Format("Unable to load initialization data from path {0}.  This can be done from the Addressables window in the Build->Build Player Content menu command.", settingsPath) };
                return (TResult)resE;
            }

            BuildTarget dataBuildTarget = BuildTarget.NoTarget;
            if (!Enum.TryParse(rtd.BuildTarget, out dataBuildTarget))
                Debug.LogWarningFormat("Unable to parse build target from initialization data: '{0}'.", rtd.BuildTarget);

            if (BuildPipeline.GetBuildTargetGroup(dataBuildTarget) != BuildTargetGroup.Standalone)
                Debug.LogWarningFormat("Asset bundles built with build target {0} may not be compatible with running in the Editor.", dataBuildTarget);
           
            //TODO: detect if the data that does exist is out of date..
            //var runtimeSettingsPath = "{UnityEngine.AddressableAssets.Addressables.RuntimePath}/settings.json";
            var runtimeSettingsPath = "{UnityEngine.AddressableAssets.PathRebuild.LocalLoadPath}/settings.json";
            PlayerPrefs.SetString(Addressables.kAddressablesRuntimeDataPath, runtimeSettingsPath);
            
            IDataBuilderResult res = new AddressablesPlayModeBuildResult() { OutputPath = settingsPath, Duration = timer.Elapsed.TotalSeconds };
            m_DataBuilt = true;
            return (TResult)res;
        }
    }
}
