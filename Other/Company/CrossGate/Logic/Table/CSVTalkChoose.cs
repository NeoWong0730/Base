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

	sealed public partial class CSVTalkChoose : Framework.Table.TableBase<CSVTalkChoose.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint InitialTalkChooseId;
			public readonly uint TalkChoose1;
			public readonly List<uint> ChooseType1;
			public readonly List<uint> ChooseValue1;
			public readonly uint ChooseEndTalk1;
			public readonly uint ChooseRightAndWrong1;
			public readonly uint TalkChoose2;
			public readonly List<uint> ChooseType2;
			public readonly List<uint> ChooseValue2;
			public readonly uint ChooseEndTalk2;
			public readonly uint ChooseRightAndWrong2;
			public readonly uint TalkChoose3;
			public readonly List<uint> ChooseType3;
			public readonly List<uint> ChooseValue3;
			public readonly uint ChooseEndTalk3;
			public readonly uint ChooseRightAndWrong3;
			public readonly uint TalkChoose4;
			public readonly List<uint> ChooseType4;
			public readonly List<uint> ChooseValue4;
			public readonly uint ChooseEndTalk4;
			public readonly uint ChooseRightAndWrong4;
			public readonly uint TalkChoose5;
			public readonly List<uint> ChooseType5;
			public readonly List<uint> ChooseValue5;
			public readonly uint ChooseEndTalk5;
			public readonly uint ChooseRightAndWrong5;
			public readonly uint ChooseWrongResult;
			public readonly uint DetachWrong;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				InitialTalkChooseId = ReadHelper.ReadUInt(binaryReader);
				TalkChoose1 = ReadHelper.ReadUInt(binaryReader);
				ChooseType1 = shareData.GetShareData<List<uint>>(binaryReader, 0);
				ChooseValue1 = shareData.GetShareData<List<uint>>(binaryReader, 0);
				ChooseEndTalk1 = ReadHelper.ReadUInt(binaryReader);
				ChooseRightAndWrong1 = ReadHelper.ReadUInt(binaryReader);
				TalkChoose2 = ReadHelper.ReadUInt(binaryReader);
				ChooseType2 = shareData.GetShareData<List<uint>>(binaryReader, 0);
				ChooseValue2 = shareData.GetShareData<List<uint>>(binaryReader, 0);
				ChooseEndTalk2 = ReadHelper.ReadUInt(binaryReader);
				ChooseRightAndWrong2 = ReadHelper.ReadUInt(binaryReader);
				TalkChoose3 = ReadHelper.ReadUInt(binaryReader);
				ChooseType3 = shareData.GetShareData<List<uint>>(binaryReader, 0);
				ChooseValue3 = shareData.GetShareData<List<uint>>(binaryReader, 0);
				ChooseEndTalk3 = ReadHelper.ReadUInt(binaryReader);
				ChooseRightAndWrong3 = ReadHelper.ReadUInt(binaryReader);
				TalkChoose4 = ReadHelper.ReadUInt(binaryReader);
				ChooseType4 = shareData.GetShareData<List<uint>>(binaryReader, 0);
				ChooseValue4 = shareData.GetShareData<List<uint>>(binaryReader, 0);
				ChooseEndTalk4 = ReadHelper.ReadUInt(binaryReader);
				ChooseRightAndWrong4 = ReadHelper.ReadUInt(binaryReader);
				TalkChoose5 = ReadHelper.ReadUInt(binaryReader);
				ChooseType5 = shareData.GetShareData<List<uint>>(binaryReader, 0);
				ChooseValue5 = shareData.GetShareData<List<uint>>(binaryReader, 0);
				ChooseEndTalk5 = ReadHelper.ReadUInt(binaryReader);
				ChooseRightAndWrong5 = ReadHelper.ReadUInt(binaryReader);
				ChooseWrongResult = ReadHelper.ReadUInt(binaryReader);
				DetachWrong = ReadHelper.ReadUInt(binaryReader);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVTalkChoose.bytes";
		}

		private static CSVTalkChoose instance = null;			
		public static CSVTalkChoose Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVTalkChoose 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVTalkChoose forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVTalkChoose();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVTalkChoose");

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

    sealed public partial class CSVTalkChoose : FCSVTalkChoose
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVTalkChoose.bytes";
		}

		private static CSVTalkChoose instance = null;			
		public static CSVTalkChoose Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVTalkChoose 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVTalkChoose forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVTalkChoose();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVTalkChoose");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}