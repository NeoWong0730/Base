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

	sealed public partial class CSVPetRemake : Framework.Table.TableBase<CSVPetRemake.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint remake_num;
			public readonly uint item_id;
			public readonly uint success_rate;
			public readonly ushort add_luck;
			public readonly byte add_attr_rate;
			public readonly List<uint> add_skill_num;
			public readonly List<uint> skill_weight;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				remake_num = ReadHelper.ReadUInt(binaryReader);
				item_id = ReadHelper.ReadUInt(binaryReader);
				success_rate = ReadHelper.ReadUInt(binaryReader);
				add_luck = ReadHelper.ReadUShort(binaryReader);
				add_attr_rate = ReadHelper.ReadByte(binaryReader);
				add_skill_num = shareData.GetShareData<List<uint>>(binaryReader, 0);
				skill_weight = shareData.GetShareData<List<uint>>(binaryReader, 0);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVPetRemake.bytes";
		}

		private static CSVPetRemake instance = null;			
		public static CSVPetRemake Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVPetRemake 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVPetRemake forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVPetRemake();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVPetRemake");

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

    sealed public partial class CSVPetRemake : FCSVPetRemake
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVPetRemake.bytes";
		}

		private static CSVPetRemake instance = null;			
		public static CSVPetRemake Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVPetRemake 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVPetRemake forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVPetRemake();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVPetRemake");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}