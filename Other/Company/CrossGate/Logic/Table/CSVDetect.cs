//
//#define USE_HOTFIX_LOGIC

using Lib.AssetLoader;
using Lib.Core;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Framework.Table;

namespace Table
{
#if USE_HOTFIX_LOGIC

	sealed public partial class CSVDetect : Framework.Table.TableBase<CSVDetect.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint TaskID;
			public readonly uint TaskTargetNum;
			public readonly uint duration;
			public readonly string DetectAction;
			public readonly uint Fx;
			public readonly string EndAction;
			public readonly uint EndFx;
			public readonly uint Result;
			public readonly uint ClueText;
			public readonly uint Reward;
			public readonly uint Favorabilityid;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				TaskID = ReadHelper.ReadUInt(binaryReader);
				TaskTargetNum = ReadHelper.ReadUInt(binaryReader);
				duration = ReadHelper.ReadUInt(binaryReader);
				DetectAction = shareData.GetShareData<string>(binaryReader, 0);
				Fx = ReadHelper.ReadUInt(binaryReader);
				EndAction = shareData.GetShareData<string>(binaryReader, 0);
				EndFx = ReadHelper.ReadUInt(binaryReader);
				Result = ReadHelper.ReadUInt(binaryReader);
				ClueText = ReadHelper.ReadUInt(binaryReader);
				Reward = ReadHelper.ReadUInt(binaryReader);
				Favorabilityid = ReadHelper.ReadUInt(binaryReader);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVDetect.bytes";
		}

		private static CSVDetect instance = null;			
		public static CSVDetect Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVDetect 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVDetect forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVDetect();
                instance.ReadByFilePath(ConfigPath(), OnCreat, OnReadShareData);
            }
            else if (forceReload)
            {
                instance.Clear();
                instance.ReadByFilePath(ConfigPath(), OnCreat, OnReadShareData);
            }
        }

        public static void Unload()
        {
            DebugUtil.Log(ELogType.eTable, "卸载CSVDetect");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }

        private static Data OnCreat(uint id, BinaryReader binaryReader, TableShareData shareData)
        {
            Data data = new Data(id, binaryReader, shareData);
            return data;
        }

        private static TableShareData OnReadShareData(BinaryReader binaryReader)
		{
			TableShareData shareData = new TableShareData(1);
			shareData.ReadStrings(binaryReader, 0);

			return shareData;
		}
	}

#else

    sealed public partial class CSVDetect : FCSVDetect
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVDetect.bytes";
		}

		private static CSVDetect instance = null;			
		public static CSVDetect Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVDetect 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVDetect forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVDetect();
                instance.ReadByFilePath(ConfigPath());
            }
            else if (forceReload)
            {
                instance.Clear();
                instance.ReadByFilePath(ConfigPath());
            }
        }

        public static void Unload()
        {
            DebugUtil.Log(ELogType.eTable, "卸载CSVDetect");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}