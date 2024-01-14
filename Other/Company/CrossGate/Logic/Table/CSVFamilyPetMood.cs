﻿//
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

	sealed public partial class CSVFamilyPetMood : Framework.Table.TableBase<CSVFamilyPetMood.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint Name;
			public readonly uint Des;
			public readonly uint Icon;
			public readonly uint trainingTips;
			public readonly List<uint> Range;
			public readonly uint addGrowthRatio;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				Name = ReadHelper.ReadUInt(binaryReader);
				Des = ReadHelper.ReadUInt(binaryReader);
				Icon = ReadHelper.ReadUInt(binaryReader);
				trainingTips = ReadHelper.ReadUInt(binaryReader);
				Range = shareData.GetShareData<List<uint>>(binaryReader, 0);
				addGrowthRatio = ReadHelper.ReadUInt(binaryReader);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVFamilyPetMood.bytes";
		}

		private static CSVFamilyPetMood instance = null;			
		public static CSVFamilyPetMood Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVFamilyPetMood 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVFamilyPetMood forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVFamilyPetMood();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVFamilyPetMood");

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

    sealed public partial class CSVFamilyPetMood : FCSVFamilyPetMood
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVFamilyPetMood.bytes";
		}

		private static CSVFamilyPetMood instance = null;			
		public static CSVFamilyPetMood Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVFamilyPetMood 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVFamilyPetMood forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVFamilyPetMood();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVFamilyPetMood");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}