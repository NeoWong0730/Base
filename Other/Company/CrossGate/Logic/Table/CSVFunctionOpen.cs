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

	sealed public partial class CSVFunctionOpen : Framework.Table.TableBase<CSVFunctionOpen.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint Condition_id;
			public readonly uint functionClose;
			public readonly uint Active_UI;
			public readonly uint FunctionUiId;
			public readonly uint FunctionSonId;
			public readonly uint Active_Name;
			public readonly string Active_Icon;
			public readonly string Active_Target;
			public readonly string Hide_Target;
			public bool Lock { get { return ReadHelper.GetBoolByIndex(boolArray0, 0); } }
			public readonly List<float> Target_pos;
			public readonly uint Icon_UI;
			public readonly string Icon_Path;
			public readonly uint Effect_ActiveUIid;
			public readonly string Effect_Target;
			public readonly string Effect;
		private readonly byte boolArray0;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				Condition_id = ReadHelper.ReadUInt(binaryReader);
				functionClose = ReadHelper.ReadUInt(binaryReader);
				Active_UI = ReadHelper.ReadUInt(binaryReader);
				FunctionUiId = ReadHelper.ReadUInt(binaryReader);
				FunctionSonId = ReadHelper.ReadUInt(binaryReader);
				Active_Name = ReadHelper.ReadUInt(binaryReader);
				Active_Icon = shareData.GetShareData<string>(binaryReader, 0);
				Active_Target = shareData.GetShareData<string>(binaryReader, 0);
				Hide_Target = shareData.GetShareData<string>(binaryReader, 0);
				Target_pos = shareData.GetShareData<List<float>>(binaryReader, 1);
				Icon_UI = ReadHelper.ReadUInt(binaryReader);
				Icon_Path = shareData.GetShareData<string>(binaryReader, 0);
				Effect_ActiveUIid = ReadHelper.ReadUInt(binaryReader);
				Effect_Target = shareData.GetShareData<string>(binaryReader, 0);
				Effect = shareData.GetShareData<string>(binaryReader, 0);

			boolArray0 = ReadHelper.ReadByte(binaryReader);
            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVFunctionOpen.bytes";
		}

		private static CSVFunctionOpen instance = null;			
		public static CSVFunctionOpen Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVFunctionOpen 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVFunctionOpen forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVFunctionOpen();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVFunctionOpen");

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
			shareData.ReadStrings(binaryReader, 0);
			shareData.ReadArrays<float>(binaryReader, 1, ReadHelper.ReadArray_ReadFloat);

			return shareData;
		}
	}

#else

    sealed public partial class CSVFunctionOpen : FCSVFunctionOpen
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVFunctionOpen.bytes";
		}

		private static CSVFunctionOpen instance = null;			
		public static CSVFunctionOpen Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVFunctionOpen 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVFunctionOpen forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVFunctionOpen();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVFunctionOpen");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}