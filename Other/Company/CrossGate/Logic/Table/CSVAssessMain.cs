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

	sealed public partial class CSVAssessMain : Framework.Table.TableBase<CSVAssessMain.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint RankType;
			public readonly uint Subtype;
			public readonly uint Name;
			public readonly uint Describe;
			public readonly List<uint> UnlockCondition;
			public bool Bar { get { return ReadHelper.GetBoolByIndex(boolArray0, 0); } }
			public readonly List<uint> Jump;
			public readonly byte Tips;
			public readonly uint Icon;
			public readonly List<uint> LIst;
		private readonly byte boolArray0;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				RankType = ReadHelper.ReadUInt(binaryReader);
				Subtype = ReadHelper.ReadUInt(binaryReader);
				Name = ReadHelper.ReadUInt(binaryReader);
				Describe = ReadHelper.ReadUInt(binaryReader);
				UnlockCondition = shareData.GetShareData<List<uint>>(binaryReader, 0);
				Jump = shareData.GetShareData<List<uint>>(binaryReader, 0);
				Tips = ReadHelper.ReadByte(binaryReader);
				Icon = ReadHelper.ReadUInt(binaryReader);
				LIst = shareData.GetShareData<List<uint>>(binaryReader, 0);

			boolArray0 = ReadHelper.ReadByte(binaryReader);
            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVAssessMain.bytes";
		}

		private static CSVAssessMain instance = null;			
		public static CSVAssessMain Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVAssessMain 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVAssessMain forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVAssessMain();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVAssessMain");

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

    sealed public partial class CSVAssessMain : FCSVAssessMain
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVAssessMain.bytes";
		}

		private static CSVAssessMain instance = null;			
		public static CSVAssessMain Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVAssessMain 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVAssessMain forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVAssessMain();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVAssessMain");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}