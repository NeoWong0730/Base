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

	sealed public partial class CSVNPCFavorability : Framework.Table.TableBase<CSVNPCFavorability.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint Age;
			public readonly uint Place;
			public readonly uint Occupation;
			public readonly uint Character;
			public readonly uint GiftTpye1;
			public readonly uint GiftTpye2;
			public readonly uint GiftTpye3;
			public readonly uint GiftPrompt1;
			public readonly uint GiftPrompt2;
			public readonly uint GiftPrompt3;
			public readonly List<uint> BanqueID;
			public readonly uint BackStory;
			public readonly uint UnlockPrompt;
			public readonly uint RewardPrompt;
			public readonly uint positionx;
			public readonly uint positiony;
			public readonly uint positionz;
			public readonly uint rotationx;
			public readonly uint rotationy;
			public readonly uint rotationz;
			public readonly uint scale;
			public readonly uint sortid;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				Age = ReadHelper.ReadUInt(binaryReader);
				Place = ReadHelper.ReadUInt(binaryReader);
				Occupation = ReadHelper.ReadUInt(binaryReader);
				Character = ReadHelper.ReadUInt(binaryReader);
				GiftTpye1 = ReadHelper.ReadUInt(binaryReader);
				GiftTpye2 = ReadHelper.ReadUInt(binaryReader);
				GiftTpye3 = ReadHelper.ReadUInt(binaryReader);
				GiftPrompt1 = ReadHelper.ReadUInt(binaryReader);
				GiftPrompt2 = ReadHelper.ReadUInt(binaryReader);
				GiftPrompt3 = ReadHelper.ReadUInt(binaryReader);
				BanqueID = shareData.GetShareData<List<uint>>(binaryReader, 0);
				BackStory = ReadHelper.ReadUInt(binaryReader);
				UnlockPrompt = ReadHelper.ReadUInt(binaryReader);
				RewardPrompt = ReadHelper.ReadUInt(binaryReader);
				positionx = ReadHelper.ReadUInt(binaryReader);
				positiony = ReadHelper.ReadUInt(binaryReader);
				positionz = ReadHelper.ReadUInt(binaryReader);
				rotationx = ReadHelper.ReadUInt(binaryReader);
				rotationy = ReadHelper.ReadUInt(binaryReader);
				rotationz = ReadHelper.ReadUInt(binaryReader);
				scale = ReadHelper.ReadUInt(binaryReader);
				sortid = ReadHelper.ReadUInt(binaryReader);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVNPCFavorability.bytes";
		}

		private static CSVNPCFavorability instance = null;			
		public static CSVNPCFavorability Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVNPCFavorability 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVNPCFavorability forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVNPCFavorability();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVNPCFavorability");

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
			shareData.ReadArrays<uint>(binaryReader, 0, ReadHelper.ReadArray_ReadUInt);

			return shareData;
		}
	}

#else

    sealed public partial class CSVNPCFavorability : FCSVNPCFavorability
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVNPCFavorability.bytes";
		}

		private static CSVNPCFavorability instance = null;			
		public static CSVNPCFavorability Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVNPCFavorability 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVNPCFavorability forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVNPCFavorability();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVNPCFavorability");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}