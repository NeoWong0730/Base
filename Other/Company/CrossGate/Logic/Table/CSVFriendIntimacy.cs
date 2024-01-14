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

	sealed public partial class CSVFriendIntimacy : Framework.Table.TableBase<CSVFriendIntimacy.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint IntimacyLvlLan;
			public readonly uint IntimacyLvl;
			public readonly uint IntimacyNeed;
			public readonly uint PresentLimited1;
			public readonly uint PresentLimited2;
			public readonly uint PresentLimited3;
			public readonly uint RoleViewLevel;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				IntimacyLvlLan = ReadHelper.ReadUInt(binaryReader);
				IntimacyLvl = ReadHelper.ReadUInt(binaryReader);
				IntimacyNeed = ReadHelper.ReadUInt(binaryReader);
				PresentLimited1 = ReadHelper.ReadUInt(binaryReader);
				PresentLimited2 = ReadHelper.ReadUInt(binaryReader);
				PresentLimited3 = ReadHelper.ReadUInt(binaryReader);
				RoleViewLevel = ReadHelper.ReadUInt(binaryReader);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVFriendIntimacy.bytes";
		}

		private static CSVFriendIntimacy instance = null;			
		public static CSVFriendIntimacy Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVFriendIntimacy 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVFriendIntimacy forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVFriendIntimacy();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVFriendIntimacy");

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

    sealed public partial class CSVFriendIntimacy : FCSVFriendIntimacy
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVFriendIntimacy.bytes";
		}

		private static CSVFriendIntimacy instance = null;			
		public static CSVFriendIntimacy Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVFriendIntimacy 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVFriendIntimacy forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVFriendIntimacy();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVFriendIntimacy");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}