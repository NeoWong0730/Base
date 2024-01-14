using System.Collections.Generic;
using System.IO;


public class Bezier1CurvesData_ConfigTool
{
	public static Bezier1CurvesData Load(BinaryReader br)
	{
		return GetBezier1CurvesData(br);
	}

	public static Bezier1CurvesData GetBezier1CurvesData(BinaryReader br) 
	{
		bool haveRef = br.ReadBoolean();
		if (!haveRef)
			return null;

		Bezier1CurvesData bezier1curvesdata = new Bezier1CurvesData();
		bezier1curvesdata.Y_MaxVal = br.ReadSingle();
		bezier1curvesdata.Y_MinVal = br.ReadSingle();
		bezier1curvesdata.LeftTopY = br.ReadSingle();
		bezier1curvesdata.Hight = br.ReadSingle();
		bezier1curvesdata.Bezier1GroupPosDataArray = GetBezier1GroupPosDataArray(br);

		return bezier1curvesdata;
	}

        public static Bezier1GroupPosData[] GetBezier1GroupPosDataArray(BinaryReader br)
		{
            bool haveRef = br.ReadBoolean();
			if (!haveRef)
				return null;

			int listCount = br.ReadUInt16();
			if (listCount <= 0)
				return null;

			Bezier1GroupPosData[] bezier1groupposdataArray = new Bezier1GroupPosData[listCount];

			for (int i = 0; i < listCount; i++)
			{
                bezier1groupposdataArray[i] = GetBezier1GroupPosData(br);
			}

			return bezier1groupposdataArray;
		}

	public static Bezier1GroupPosData GetBezier1GroupPosData(BinaryReader br) 
	{
		bool haveRef = br.ReadBoolean();
		if (!haveRef)
			return null;

		Bezier1GroupPosData bezier1groupposdata = new Bezier1GroupPosData();
		bezier1groupposdata.LeftPos = br.ReadSingle();
		bezier1groupposdata.Pos = br.ReadSingle();
		bezier1groupposdata.RightPos = br.ReadSingle();
		bezier1groupposdata.LeftRatioInTotal = br.ReadSingle();
		bezier1groupposdata.PosRatioInTotal = br.ReadSingle();
		bezier1groupposdata.RightRatioInTotal = br.ReadSingle();

		return bezier1groupposdata;
	}
}