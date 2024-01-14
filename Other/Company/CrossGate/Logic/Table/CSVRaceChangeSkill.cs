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

	sealed public partial class CSVRaceChangeSkill : Framework.Table.TableBase<CSVRaceChangeSkill.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint type;
			public readonly uint rank;
			public readonly List<uint> skill_group;
			public readonly List<List<uint>> relearn_cost;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				type = ReadHelper.ReadUInt(binaryReader);
				rank = ReadHelper.ReadUInt(binaryReader);
				skill_group = shareData.GetShareData<List<uint>>(binaryReader, 0);
				relearn_cost = shareData.GetShareData<List<List<uint>>>(binaryReader, 1);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVRaceChangeSkill.bytes";
		}

		private static CSVRaceChangeSkill instance = null;			
		public static CSVRaceChangeSkill Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVRaceChangeSkill 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVRaceChangeSkill forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVRaceChangeSkill();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVRaceChangeSkill");

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

    sealed public partial class CSVRaceChangeSkill : FCSVRaceChangeSkill
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVRaceChangeSkill.bytes";
		}

		private static CSVRaceChangeSkill instance = null;			
		public static CSVRaceChangeSkill Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVRaceChangeSkill 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVRaceChangeSkill forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVRaceChangeSkill();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVRaceChangeSkill");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}