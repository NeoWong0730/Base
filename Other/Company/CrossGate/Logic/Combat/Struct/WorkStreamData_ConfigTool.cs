using System.Collections.Generic;
using System.IO;


public class WorkStreamData_ConfigTool
{
	public static WorkStreamData Load(BinaryReader br)
	{
		return GetWorkStreamData(br);
	}

	public static WorkStreamData GetWorkStreamData(BinaryReader br) 
	{
		bool haveRef = br.ReadBoolean();
		if (!haveRef)
			return null;

		WorkStreamData workstreamdata = new WorkStreamData();
		workstreamdata.AttackWorkBlockDatas = GetWorkBlockDataList(br);
		workstreamdata.TargetWorkBlockDatas = GetWorkBlockDataList(br);

		return workstreamdata;
	}

	public static List<WorkBlockData> GetWorkBlockDataList(BinaryReader br) 
	{
		bool haveRef = br.ReadBoolean();
		if (!haveRef)
			return null;

		List<WorkBlockData> workblockdataList = new List<WorkBlockData>();
		int listCount = br.ReadUInt16();
		if (listCount <= 0)
			return workblockdataList;

		for (int i = 0; i < listCount; i++)
		{
			workblockdataList.Add(GetWorkBlockData(br));
		}

		return workblockdataList;
	}

	public static WorkBlockData GetWorkBlockData(BinaryReader br) 
	{
		bool haveRef = br.ReadBoolean();
		if (!haveRef)
			return null;

		WorkBlockData workblockdata = new WorkBlockData();
		workblockdata.CurWorkBlockType = br.ReadInt32();
		workblockdata.AttachType = br.ReadByte();
		workblockdata.NextBlockType = br.ReadInt32();
		workblockdata.TopWorkNodeData = GetWorkNodeData(br);

		return workblockdata;
	}

	public static WorkNodeData GetWorkNodeData(BinaryReader br) 
	{
		bool haveRef = br.ReadBoolean();
		if (!haveRef)
			return null;

		WorkNodeData worknodedata = new WorkNodeData();
		worknodedata.Id = br.ReadUInt16();
		worknodedata.InParentGroupNodeListIndex = br.ReadInt16();
		worknodedata.GroupIndex = br.ReadSByte();
		worknodedata.IsMainLine = br.ReadBoolean();
		worknodedata.IsConcurrent = br.ReadBoolean();
		worknodedata.NodeType = br.ReadInt32();
		worknodedata.NodeContent = CombatHelp.ReadString(br);
		worknodedata.LayerIndex = br.ReadSByte();
		worknodedata.TransitionWorkGroupList = GetWorkGroupNodeDataList(br);
		worknodedata.SkipWorkBlockType = br.ReadInt32();

		return worknodedata;
	}

	public static List<WorkGroupNodeData> GetWorkGroupNodeDataList(BinaryReader br) 
	{
		bool haveRef = br.ReadBoolean();
		if (!haveRef)
			return null;

		List<WorkGroupNodeData> workgroupnodedataList = new List<WorkGroupNodeData>();
		int listCount = br.ReadUInt16();
		if (listCount <= 0)
			return workgroupnodedataList;

		for (int i = 0; i < listCount; i++)
		{
			workgroupnodedataList.Add(GetWorkGroupNodeData(br));
		}

		return workgroupnodedataList;
	}

	public static WorkGroupNodeData GetWorkGroupNodeData(BinaryReader br) 
	{
		bool haveRef = br.ReadBoolean();
		if (!haveRef)
			return null;

		WorkGroupNodeData workgroupnodedata = new WorkGroupNodeData();
		workgroupnodedata.GroupIndex = br.ReadSByte();
		workgroupnodedata.NodeList = GetWorkNodeDataList(br);

		return workgroupnodedata;
	}

	public static List<WorkNodeData> GetWorkNodeDataList(BinaryReader br) 
	{
		bool haveRef = br.ReadBoolean();
		if (!haveRef)
			return null;

		List<WorkNodeData> worknodedataList = new List<WorkNodeData>();
		int listCount = br.ReadUInt16();
		if (listCount <= 0)
			return worknodedataList;

		for (int i = 0; i < listCount; i++)
		{
			worknodedataList.Add(GetWorkNodeData(br));
		}

		return worknodedataList;
	}
}