﻿//
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

	sealed public partial class CSVPassiveSkill : Framework.Table.TableBase<CSVPassiveSkill.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint behavior_name;
			public readonly uint behavior_work_id;
			public readonly uint effect_trigger;
			public readonly List<uint> passive_effective;
			public readonly uint quality;
			public readonly uint score;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				behavior_name = ReadHelper.ReadUInt(binaryReader);
				behavior_work_id = ReadHelper.ReadUInt(binaryReader);
				effect_trigger = ReadHelper.ReadUInt(binaryReader);
				passive_effective = shareData.GetShareData<List<uint>>(binaryReader, 0);
				quality = ReadHelper.ReadUInt(binaryReader);
				score = ReadHelper.ReadUInt(binaryReader);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVPassiveSkill.bytes";
		}

		private static CSVPassiveSkill instance = null;			
		public static CSVPassiveSkill Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVPassiveSkill 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVPassiveSkill forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVPassiveSkill();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVPassiveSkill");

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

    sealed public partial class CSVPassiveSkill : FCSVPassiveSkill
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVPassiveSkill.bytes";
		}

		private static CSVPassiveSkill instance = null;			
		public static CSVPassiveSkill Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVPassiveSkill 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVPassiveSkill forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVPassiveSkill();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVPassiveSkill");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}