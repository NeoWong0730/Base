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

	sealed public partial class CSVAdventureLevel : Framework.Table.TableBase<CSVAdventureLevel.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint name;
			public readonly string icon;
			public readonly uint exp;
			public readonly uint score;
			public readonly List<List<uint>> addAttribute;
			public readonly List<List<uint>> addPrivilegeAttribute;
			public readonly List<List<uint>> addPrivilege;
			public readonly List<uint> privilegeText;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				name = ReadHelper.ReadUInt(binaryReader);
				icon = shareData.GetShareData<string>(binaryReader, 0);
				exp = ReadHelper.ReadUInt(binaryReader);
				score = ReadHelper.ReadUInt(binaryReader);
				addAttribute = shareData.GetShareData<List<List<uint>>>(binaryReader, 2);
				addPrivilegeAttribute = shareData.GetShareData<List<List<uint>>>(binaryReader, 2);
				addPrivilege = shareData.GetShareData<List<List<uint>>>(binaryReader, 2);
				privilegeText = shareData.GetShareData<List<uint>>(binaryReader, 1);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVAdventureLevel.bytes";
		}

		private static CSVAdventureLevel instance = null;			
		public static CSVAdventureLevel Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVAdventureLevel 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVAdventureLevel forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVAdventureLevel();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVAdventureLevel");

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
			TableShareData shareData = new TableShareData(3);
			shareData.ReadStrings(binaryReader, 0);
			shareData.ReadArrays<uint>(binaryReader, 1, ReadHelper.ReadArray_ReadUInt);
			shareData.ReadArray2s<uint>(binaryReader, 2, 1);

			return shareData;
		}
	}

#else

    sealed public partial class CSVAdventureLevel : FCSVAdventureLevel
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVAdventureLevel.bytes";
		}

		private static CSVAdventureLevel instance = null;			
		public static CSVAdventureLevel Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVAdventureLevel 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVAdventureLevel forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVAdventureLevel();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVAdventureLevel");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}