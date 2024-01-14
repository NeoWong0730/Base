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

	sealed public partial class CSVBraveTeamMeeting : Framework.Table.TableBase<CSVBraveTeamMeeting.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint TypeLan;
			public readonly uint TypeContentLan;
			public readonly List<uint> Condition;
			public readonly List<uint> ConditionLan;
			public readonly uint ContentLan;
			public readonly uint Pass;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				TypeLan = ReadHelper.ReadUInt(binaryReader);
				TypeContentLan = ReadHelper.ReadUInt(binaryReader);
				Condition = shareData.GetShareData<List<uint>>(binaryReader, 0);
				ConditionLan = shareData.GetShareData<List<uint>>(binaryReader, 0);
				ContentLan = ReadHelper.ReadUInt(binaryReader);
				Pass = ReadHelper.ReadUInt(binaryReader);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVBraveTeamMeeting.bytes";
		}

		private static CSVBraveTeamMeeting instance = null;			
		public static CSVBraveTeamMeeting Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVBraveTeamMeeting 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVBraveTeamMeeting forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVBraveTeamMeeting();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVBraveTeamMeeting");

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

    sealed public partial class CSVBraveTeamMeeting : FCSVBraveTeamMeeting
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVBraveTeamMeeting.bytes";
		}

		private static CSVBraveTeamMeeting instance = null;			
		public static CSVBraveTeamMeeting Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVBraveTeamMeeting 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVBraveTeamMeeting forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVBraveTeamMeeting();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVBraveTeamMeeting");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}