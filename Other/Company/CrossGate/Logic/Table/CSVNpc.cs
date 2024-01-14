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

	sealed public partial class CSVNpc : Framework.Table.TableBase<CSVNpc.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint id_index;
			public readonly uint mapId;
			public readonly uint type;
			public readonly uint subtype;
			public readonly List<uint> mark_type;
			public readonly uint mark_lan;
			public readonly List<float> mark_move;
			public readonly uint ActivationRecord;
			public readonly uint NpcTriggerFrequency;
			public readonly uint mark_des;
			public readonly uint name;
			public readonly uint appellation;
			public readonly uint nameShow;
			public readonly uint ShowState;
			public readonly uint WhetherScopenTrigger;
			public readonly uint TriggerScopen;
			public readonly uint GreaterThanLvCond;
			public readonly uint LessThanLvCond;
			public readonly uint ConditionGroupCond;
			public readonly uint RebirthTime;
			public readonly uint CombatCooling;
			public readonly List<uint> NPCBubbleID;
			public readonly List<uint> NPCBubbleInteral;
			public readonly List<int> signPositionShifting;
			public readonly string model;
			public readonly string model_show;
			public readonly uint action_id;
			public readonly uint action_show_id;
			public readonly List<List<uint>> function;
			public readonly List<uint> OpenShop;
			public readonly uint CollectionID;
			public readonly List<uint> functionCondition;
			public readonly uint InteractiveRange;
			public readonly List<uint> acquiesceDialogue;
			public readonly uint behaviorid;
			public readonly uint TriggerPerformRange;
			public readonly uint PerformCooling;
			public readonly int LeftLocationX;
			public readonly int LeftLocationY;
			public readonly int LeftLocationZ;
			public readonly int LeftLocationRotateX;
			public readonly int LeftLocationRotateY;
			public readonly int LeftLocationRotateZ;
			public readonly int LeftLocationMirrorImage;
			public readonly int RightLocationX;
			public readonly int RightLocationY;
			public readonly int RightLocationZ;
			public readonly int RightLocationRotateX;
			public readonly int RightLocationRotateY;
			public readonly int RightLocationRotateZ;
			public readonly int RightLocationMirrorImage;
			public readonly List<int> dialogueParameter;
			public readonly List<int> dialogueEndParameter;
			public readonly int BubbleLocationX;
			public readonly int BubbleLocationY;
			public readonly int BubbleLocationZ;
			public readonly int BubbleLocationRotateX;
			public readonly int BubbleLocationRotateY;
			public readonly int BubbleLocationRotateZ;
			public readonly int BubbleLocationMirrorImage;
			public readonly uint InteractiveVoice;
			public readonly List<uint> ResourecePara;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				id_index = ReadHelper.ReadUInt(binaryReader);
				mapId = ReadHelper.ReadUInt(binaryReader);
				type = ReadHelper.ReadUInt(binaryReader);
				subtype = ReadHelper.ReadUInt(binaryReader);
				mark_type = shareData.GetShareData<List<uint>>(binaryReader, 1);
				mark_lan = ReadHelper.ReadUInt(binaryReader);
				mark_move = shareData.GetShareData<List<float>>(binaryReader, 2);
				ActivationRecord = ReadHelper.ReadUInt(binaryReader);
				NpcTriggerFrequency = ReadHelper.ReadUInt(binaryReader);
				mark_des = ReadHelper.ReadUInt(binaryReader);
				name = ReadHelper.ReadUInt(binaryReader);
				appellation = ReadHelper.ReadUInt(binaryReader);
				nameShow = ReadHelper.ReadUInt(binaryReader);
				ShowState = ReadHelper.ReadUInt(binaryReader);
				WhetherScopenTrigger = ReadHelper.ReadUInt(binaryReader);
				TriggerScopen = ReadHelper.ReadUInt(binaryReader);
				GreaterThanLvCond = ReadHelper.ReadUInt(binaryReader);
				LessThanLvCond = ReadHelper.ReadUInt(binaryReader);
				ConditionGroupCond = ReadHelper.ReadUInt(binaryReader);
				RebirthTime = ReadHelper.ReadUInt(binaryReader);
				CombatCooling = ReadHelper.ReadUInt(binaryReader);
				NPCBubbleID = shareData.GetShareData<List<uint>>(binaryReader, 1);
				NPCBubbleInteral = shareData.GetShareData<List<uint>>(binaryReader, 1);
				signPositionShifting = shareData.GetShareData<List<int>>(binaryReader, 3);
				model = shareData.GetShareData<string>(binaryReader, 0);
				model_show = shareData.GetShareData<string>(binaryReader, 0);
				action_id = ReadHelper.ReadUInt(binaryReader);
				action_show_id = ReadHelper.ReadUInt(binaryReader);
				function = shareData.GetShareData<List<List<uint>>>(binaryReader, 4);
				OpenShop = shareData.GetShareData<List<uint>>(binaryReader, 1);
				CollectionID = ReadHelper.ReadUInt(binaryReader);
				functionCondition = shareData.GetShareData<List<uint>>(binaryReader, 1);
				InteractiveRange = ReadHelper.ReadUInt(binaryReader);
				acquiesceDialogue = shareData.GetShareData<List<uint>>(binaryReader, 1);
				behaviorid = ReadHelper.ReadUInt(binaryReader);
				TriggerPerformRange = ReadHelper.ReadUInt(binaryReader);
				PerformCooling = ReadHelper.ReadUInt(binaryReader);
				LeftLocationX = ReadHelper.ReadInt(binaryReader);
				LeftLocationY = ReadHelper.ReadInt(binaryReader);
				LeftLocationZ = ReadHelper.ReadInt(binaryReader);
				LeftLocationRotateX = ReadHelper.ReadInt(binaryReader);
				LeftLocationRotateY = ReadHelper.ReadInt(binaryReader);
				LeftLocationRotateZ = ReadHelper.ReadInt(binaryReader);
				LeftLocationMirrorImage = ReadHelper.ReadInt(binaryReader);
				RightLocationX = ReadHelper.ReadInt(binaryReader);
				RightLocationY = ReadHelper.ReadInt(binaryReader);
				RightLocationZ = ReadHelper.ReadInt(binaryReader);
				RightLocationRotateX = ReadHelper.ReadInt(binaryReader);
				RightLocationRotateY = ReadHelper.ReadInt(binaryReader);
				RightLocationRotateZ = ReadHelper.ReadInt(binaryReader);
				RightLocationMirrorImage = ReadHelper.ReadInt(binaryReader);
				dialogueParameter = shareData.GetShareData<List<int>>(binaryReader, 3);
				dialogueEndParameter = shareData.GetShareData<List<int>>(binaryReader, 3);
				BubbleLocationX = ReadHelper.ReadInt(binaryReader);
				BubbleLocationY = ReadHelper.ReadInt(binaryReader);
				BubbleLocationZ = ReadHelper.ReadInt(binaryReader);
				BubbleLocationRotateX = ReadHelper.ReadInt(binaryReader);
				BubbleLocationRotateY = ReadHelper.ReadInt(binaryReader);
				BubbleLocationRotateZ = ReadHelper.ReadInt(binaryReader);
				BubbleLocationMirrorImage = ReadHelper.ReadInt(binaryReader);
				InteractiveVoice = ReadHelper.ReadUInt(binaryReader);
				ResourecePara = shareData.GetShareData<List<uint>>(binaryReader, 1);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVNpc.bytes";
		}

		private static CSVNpc instance = null;			
		public static CSVNpc Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVNpc 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVNpc forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVNpc();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVNpc");

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
			TableShareData shareData = new TableShareData(5);
			shareData.ReadStrings(binaryReader, 0);
			shareData.ReadArrays<uint>(binaryReader, 1, ReadHelper.ReadArray_ReadUInt);
			shareData.ReadArrays<float>(binaryReader, 2, ReadHelper.ReadArray_ReadFloat);
			shareData.ReadArrays<int>(binaryReader, 3, ReadHelper.ReadArray_ReadInt);
			shareData.ReadArray2s<uint>(binaryReader, 4, 1);

			return shareData;
		}
	}

#else

    sealed public partial class CSVNpc : FCSVNpc
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVNpc.bytes";
		}

		private static CSVNpc instance = null;			
		public static CSVNpc Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVNpc 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVNpc forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVNpc();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVNpc");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}