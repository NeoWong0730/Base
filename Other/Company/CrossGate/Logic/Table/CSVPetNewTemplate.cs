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

	sealed public partial class CSVPetNewTemplate : Framework.Table.TableBase<CSVPetNewTemplate.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public bool is_band { get { return ReadHelper.GetBoolByIndex(boolArray0, 0); } }
			public bool is_domestication { get { return ReadHelper.GetBoolByIndex(boolArray0, 1); } }
			public readonly uint timelimit;
			public readonly uint lv;
			public readonly byte endurance;
			public readonly byte strength;
			public readonly byte strong;
			public readonly byte speed;
			public readonly byte magic;
			public readonly List<List<uint>> required_skills;
			public readonly List<List<uint>> unique_skills;
			public readonly List<uint> remake_skills;
			public readonly byte default_point;
			public readonly List<List<uint>> mount_skills;
		private readonly byte boolArray0;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				timelimit = ReadHelper.ReadUInt(binaryReader);
				lv = ReadHelper.ReadUInt(binaryReader);
				endurance = ReadHelper.ReadByte(binaryReader);
				strength = ReadHelper.ReadByte(binaryReader);
				strong = ReadHelper.ReadByte(binaryReader);
				speed = ReadHelper.ReadByte(binaryReader);
				magic = ReadHelper.ReadByte(binaryReader);
				required_skills = shareData.GetShareData<List<List<uint>>>(binaryReader, 1);
				unique_skills = shareData.GetShareData<List<List<uint>>>(binaryReader, 1);
				remake_skills = shareData.GetShareData<List<uint>>(binaryReader, 0);
				default_point = ReadHelper.ReadByte(binaryReader);
				mount_skills = shareData.GetShareData<List<List<uint>>>(binaryReader, 1);

			boolArray0 = ReadHelper.ReadByte(binaryReader);
            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVPetNewTemplate.bytes";
		}

		private static CSVPetNewTemplate instance = null;			
		public static CSVPetNewTemplate Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVPetNewTemplate 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVPetNewTemplate forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVPetNewTemplate();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVPetNewTemplate");

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
			TableShareData shareData = new TableShareData(2);
			shareData.ReadArrays<uint>(binaryReader, 0, ReadHelper.ReadArray_ReadUInt);
			shareData.ReadArray2s<uint>(binaryReader, 1, 0);

			return shareData;
		}
	}

#else

    sealed public partial class CSVPetNewTemplate : FCSVPetNewTemplate
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVPetNewTemplate.bytes";
		}

		private static CSVPetNewTemplate instance = null;			
		public static CSVPetNewTemplate Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVPetNewTemplate 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVPetNewTemplate forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVPetNewTemplate();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVPetNewTemplate");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}