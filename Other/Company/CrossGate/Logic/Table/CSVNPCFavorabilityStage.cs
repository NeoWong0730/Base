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

	sealed public partial class CSVNPCFavorabilityStage : Framework.Table.TableBase<CSVNPCFavorabilityStage.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint Favorabilityid;
			public readonly uint Stage;
			public readonly uint FavorabilityValueMax;
			public readonly uint WishTask;
			public readonly uint RewardType;
			public readonly uint LetterLan;
			public readonly uint Reward;
			public readonly uint SickDia;
			public readonly uint InteractiveDia;
			public readonly uint WishTaskDia;
			public readonly uint LetterDia;
			public readonly uint TalkDia;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				Favorabilityid = ReadHelper.ReadUInt(binaryReader);
				Stage = ReadHelper.ReadUInt(binaryReader);
				FavorabilityValueMax = ReadHelper.ReadUInt(binaryReader);
				WishTask = ReadHelper.ReadUInt(binaryReader);
				RewardType = ReadHelper.ReadUInt(binaryReader);
				LetterLan = ReadHelper.ReadUInt(binaryReader);
				Reward = ReadHelper.ReadUInt(binaryReader);
				SickDia = ReadHelper.ReadUInt(binaryReader);
				InteractiveDia = ReadHelper.ReadUInt(binaryReader);
				WishTaskDia = ReadHelper.ReadUInt(binaryReader);
				LetterDia = ReadHelper.ReadUInt(binaryReader);
				TalkDia = ReadHelper.ReadUInt(binaryReader);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVNPCFavorabilityStage.bytes";
		}

		private static CSVNPCFavorabilityStage instance = null;			
		public static CSVNPCFavorabilityStage Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVNPCFavorabilityStage 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVNPCFavorabilityStage forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVNPCFavorabilityStage();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVNPCFavorabilityStage");

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
			TableShareData shareData = null;

			return shareData;
		}
	}

#else

    sealed public partial class CSVNPCFavorabilityStage : FCSVNPCFavorabilityStage
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVNPCFavorabilityStage.bytes";
		}

		private static CSVNPCFavorabilityStage instance = null;			
		public static CSVNPCFavorabilityStage Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVNPCFavorabilityStage 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVNPCFavorabilityStage forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVNPCFavorabilityStage();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVNPCFavorabilityStage");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}