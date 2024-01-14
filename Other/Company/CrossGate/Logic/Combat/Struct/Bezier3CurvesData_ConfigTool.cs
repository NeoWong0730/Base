using System.Collections.Generic;
using System.IO;


public class Bezier3CurvesData_ConfigTool
{
	public static Bezier3CurvesData Load(BinaryReader br)
	{
		return GetBezier3CurvesData(br);
	}

	public static Bezier3CurvesData GetBezier3CurvesData(BinaryReader br) 
	{
		bool haveRef = br.ReadBoolean();
		if (!haveRef)
			return null;

		Bezier3CurvesData bezier3curvesdata = new Bezier3CurvesData();
		bezier3curvesdata.EulerAngleX = br.ReadSingle();
		bezier3curvesdata.EulerAngleY = br.ReadSingle();
		bezier3curvesdata.EulerAngleZ = br.ReadSingle();
		bezier3curvesdata.m_BezierPosDatas = GetBezier3PosDataArray(br);
		bezier3curvesdata.m_BezierParams = GetintArray(br);

		return bezier3curvesdata;
	}

        public static Bezier3PosData[] GetBezier3PosDataArray(BinaryReader br)
		{
            bool haveRef = br.ReadBoolean();
			if (!haveRef)
				return null;

			int listCount = br.ReadUInt16();
			if (listCount <= 0)
				return null;

			Bezier3PosData[] bezier3posdataArray = new Bezier3PosData[listCount];

			for (int i = 0; i < listCount; i++)
			{
                bezier3posdataArray[i] = GetBezier3PosData(br);
			}

			return bezier3posdataArray;
		}

	public static Bezier3PosData GetBezier3PosData(BinaryReader br) 
	{
		bool haveRef = br.ReadBoolean();
		if (!haveRef)
			return null;

		Bezier3PosData bezier3posdata = new Bezier3PosData();
		bezier3posdata.BezierCurverLen = br.ReadSingle();
		bezier3posdata.Segment = br.ReadUInt32();
		bezier3posdata.LeftHelpPos = CombatHelp.GetVector3(br);
		bezier3posdata.Pos = CombatHelp.GetVector3(br);
		bezier3posdata.RightHelpPos = CombatHelp.GetVector3(br);
		bezier3posdata.EventInfoArray = GetBezierCurverEventArray(br);

		return bezier3posdata;
	}

        public static BezierCurverEvent[] GetBezierCurverEventArray(BinaryReader br)
		{
            bool haveRef = br.ReadBoolean();
			if (!haveRef)
				return null;

			int listCount = br.ReadUInt16();
			if (listCount <= 0)
				return null;

			BezierCurverEvent[] beziercurvereventArray = new BezierCurverEvent[listCount];

			for (int i = 0; i < listCount; i++)
			{
                beziercurvereventArray[i] = GetBezierCurverEvent(br);
			}

			return beziercurvereventArray;
		}

	public static BezierCurverEvent GetBezierCurverEvent(BinaryReader br) 
	{
		bool haveRef = br.ReadBoolean();
		if (!haveRef)
			return null;

		BezierCurverEvent beziercurverevent = new BezierCurverEvent();
		beziercurverevent.EventId = br.ReadInt32();
		beziercurverevent.EventRatio = br.ReadSingle();
		beziercurverevent.EventCount = br.ReadInt32();
		beziercurverevent.EventFlagName = CombatHelp.ReadString(br);

		return beziercurverevent;
	}

        public static int[] GetintArray(BinaryReader br)
		{
            bool haveRef = br.ReadBoolean();
			if (!haveRef)
				return null;

			int listCount = br.ReadUInt16();
			if (listCount <= 0)
				return null;

			int[] intArray = new int[listCount];

			for (int i = 0; i < listCount; i++)
			{
                intArray[i] = br.ReadInt32();
			}

			return intArray;
		}
}