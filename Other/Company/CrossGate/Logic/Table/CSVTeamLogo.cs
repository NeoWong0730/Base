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

	sealed public partial class CSVTeamLogo : Framework.Table.TableBase<CSVTeamLogo.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint TeamName;
			public readonly uint TeamDescribe;
			public readonly uint Lock;
			public readonly uint LimitedTime;
			public readonly uint TeamGetFor;
			public readonly List<uint> TeamParamFor;
			public readonly uint TeamGetLimit;
			public readonly List<uint> TeamParamLimit;
			public readonly uint TeamIcon;
			public readonly uint FullTeamIcon;
			public readonly uint Unlocktips;
			public readonly List<string> SubPackageShow;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				TeamName = ReadHelper.ReadUInt(binaryReader);
				TeamDescribe = ReadHelper.ReadUInt(binaryReader);
				Lock = ReadHelper.ReadUInt(binaryReader);
				LimitedTime = ReadHelper.ReadUInt(binaryReader);
				TeamGetFor = ReadHelper.ReadUInt(binaryReader);
				TeamParamFor = shareData.GetShareData<List<uint>>(binaryReader, 1);
				TeamGetLimit = ReadHelper.ReadUInt(binaryReader);
				TeamParamLimit = shareData.GetShareData<List<uint>>(binaryReader, 1);
				TeamIcon = ReadHelper.ReadUInt(binaryReader);
				FullTeamIcon = ReadHelper.ReadUInt(binaryReader);
				Unlocktips = ReadHelper.ReadUInt(binaryReader);
				SubPackageShow = shareData.GetShareData<List<string>>(binaryReader, 2);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVTeamLogo.bytes";
		}

		private static CSVTeamLogo instance = null;			
		public static CSVTeamLogo Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVTeamLogo 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVTeamLogo forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVTeamLogo();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVTeamLogo");

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
			shareData.ReadStringArrays(binaryReader, 2, 0);

			return shareData;
		}
	}

#else

    sealed public partial class CSVTeamLogo : FCSVTeamLogo
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVTeamLogo.bytes";
		}

		private static CSVTeamLogo instance = null;			
		public static CSVTeamLogo Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVTeamLogo 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVTeamLogo forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVTeamLogo();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVTeamLogo");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}