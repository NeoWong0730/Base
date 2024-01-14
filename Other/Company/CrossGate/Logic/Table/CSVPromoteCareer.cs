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

	sealed public partial class CSVPromoteCareer : Framework.Table.TableBase<CSVPromoteCareer.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly int serverConditions;
			public readonly uint mercheetLevel;
			public readonly int teamCondition;
			public readonly List<uint> conditions;
			public readonly List<uint> task_dislan;
			public readonly List<uint> task_hintlan;
			public readonly List<uint> advanced_description;
			public readonly List<uint> titleConditions;
			public readonly uint isFull;
			public readonly List<uint> skillLimit;
			public readonly int pvePoint;
			public readonly int pvpAddPoint;
			public readonly List<uint> battleLevel;
			public readonly uint head;
			public readonly uint professionLan;
			public readonly uint title;
			public readonly List<uint> skill;
			public readonly List<uint> skill_lv;
			public readonly List<uint> skill_max;
			public readonly List<List<uint>> propertyAdd;
			public readonly int propertyPoint;
			public readonly uint levelLimit;
			public readonly uint advanceNpc;
			public readonly uint advanceDia;
			public readonly List<uint> equip;
			public readonly List<uint> equip_name;
			public readonly List<uint> equip_lv;
			public readonly List<uint> equip_max;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				serverConditions = ReadHelper.ReadInt(binaryReader);
				mercheetLevel = ReadHelper.ReadUInt(binaryReader);
				teamCondition = ReadHelper.ReadInt(binaryReader);
				conditions = shareData.GetShareData<List<uint>>(binaryReader, 0);
				task_dislan = shareData.GetShareData<List<uint>>(binaryReader, 0);
				task_hintlan = shareData.GetShareData<List<uint>>(binaryReader, 0);
				advanced_description = shareData.GetShareData<List<uint>>(binaryReader, 0);
				titleConditions = shareData.GetShareData<List<uint>>(binaryReader, 0);
				isFull = ReadHelper.ReadUInt(binaryReader);
				skillLimit = shareData.GetShareData<List<uint>>(binaryReader, 0);
				pvePoint = ReadHelper.ReadInt(binaryReader);
				pvpAddPoint = ReadHelper.ReadInt(binaryReader);
				battleLevel = shareData.GetShareData<List<uint>>(binaryReader, 0);
				head = ReadHelper.ReadUInt(binaryReader);
				professionLan = ReadHelper.ReadUInt(binaryReader);
				title = ReadHelper.ReadUInt(binaryReader);
				skill = shareData.GetShareData<List<uint>>(binaryReader, 0);
				skill_lv = shareData.GetShareData<List<uint>>(binaryReader, 0);
				skill_max = shareData.GetShareData<List<uint>>(binaryReader, 0);
				propertyAdd = shareData.GetShareData<List<List<uint>>>(binaryReader, 1);
				propertyPoint = ReadHelper.ReadInt(binaryReader);
				levelLimit = ReadHelper.ReadUInt(binaryReader);
				advanceNpc = ReadHelper.ReadUInt(binaryReader);
				advanceDia = ReadHelper.ReadUInt(binaryReader);
				equip = shareData.GetShareData<List<uint>>(binaryReader, 0);
				equip_name = shareData.GetShareData<List<uint>>(binaryReader, 0);
				equip_lv = shareData.GetShareData<List<uint>>(binaryReader, 0);
				equip_max = shareData.GetShareData<List<uint>>(binaryReader, 0);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVPromoteCareer.bytes";
		}

		private static CSVPromoteCareer instance = null;			
		public static CSVPromoteCareer Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVPromoteCareer 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVPromoteCareer forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVPromoteCareer();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVPromoteCareer");

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

    sealed public partial class CSVPromoteCareer : FCSVPromoteCareer
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVPromoteCareer.bytes";
		}

		private static CSVPromoteCareer instance = null;			
		public static CSVPromoteCareer Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVPromoteCareer 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVPromoteCareer forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVPromoteCareer();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVPromoteCareer");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}