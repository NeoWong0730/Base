//
#define USE_HOTFIX_LOGIC

using Lib.AssetLoader;
using Lib.Core;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Framework.Table;

namespace Table
{
#if USE_HOTFIX_LOGIC

	sealed public partial class CSVSubmitPetCondition : Framework.Table.TableBase<CSVSubmitPetCondition.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint tips;
			public readonly uint NoSubmitPetstips;
			public readonly uint PetsID;
			public readonly uint PetsGear;
			public readonly uint PetsGearMore;
			public readonly uint PetsLessenGear;
			public readonly uint PetsLessenGearMore;
			public readonly uint PetsArrest;
			public readonly uint PetsArrestMore;
			public readonly uint PetsScore;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				tips = ReadHelper.ReadUInt(binaryReader);
				NoSubmitPetstips = ReadHelper.ReadUInt(binaryReader);
				PetsID = ReadHelper.ReadUInt(binaryReader);
				PetsGear = ReadHelper.ReadUInt(binaryReader);
				PetsGearMore = ReadHelper.ReadUInt(binaryReader);
				PetsLessenGear = ReadHelper.ReadUInt(binaryReader);
				PetsLessenGearMore = ReadHelper.ReadUInt(binaryReader);
				PetsArrest = ReadHelper.ReadUInt(binaryReader);
				PetsArrestMore = ReadHelper.ReadUInt(binaryReader);
				PetsScore = ReadHelper.ReadUInt(binaryReader);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVSubmitPetCondition.bytes";
		}

		private static CSVSubmitPetCondition instance = null;			
		public static CSVSubmitPetCondition Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVSubmitPetCondition 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVSubmitPetCondition forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVSubmitPetCondition();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVSubmitPetCondition");

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

    sealed public partial class CSVSubmitPetCondition : FCSVSubmitPetCondition
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVSubmitPetCondition.bytes";
		}

		private static CSVSubmitPetCondition instance = null;			
		public static CSVSubmitPetCondition Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVSubmitPetCondition 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVSubmitPetCondition forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVSubmitPetCondition();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVSubmitPetCondition");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}