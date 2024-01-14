using System.Collections.Generic;
using Packet;

namespace Logic
{
    public interface IMapInfoProtoParse<T>
    {
        void ParseValue(T value);
    }

    public interface ISerialize
    {
        void Serialize(OutputStream Writter);
        void DeSerialize(InputStream Reader);

        int CalcSize();
    }
    [System.Serializable]
    public class BrushArray : IMapInfoProtoParse<MapCfgInfo.Types.BrushArray>, ISerialize
    {
        public List<uint> BrushIndex = new List<uint>();//下标1开始,0留空（每个格子对应的笔刷index）

        public static BrushArray Parse(MapCfgInfo.Types.BrushArray value)
        {
            var obj = new BrushArray();
            obj.ParseValue(value);
            return obj;
        }

        public void ParseValue(MapCfgInfo.Types.BrushArray value)
        {
            BrushIndex.AddRange(value.BrushIndex);
        }

        public int CalcSize()
        {
            int size = 0;
            if (BrushIndex.Count > 0)
                size += 1 + ComputorSize.ComputiListSize<uint>(BrushIndex, ComputorSize.ComputeUInt32Size);

            return size;
        }

        public void DeSerialize(InputStream Reader)
        {
            int tage = 0;
            while ((tage = Reader.ReadTag()) != 0)
            {
                switch (tage)
                {
                    case 21:
                        Reader.ReadList(BrushIndex, Reader.ReadUInt);
                        break;
                    default:
                        break;
                }

            }
        }
        public void Serialize(OutputStream Writter)
        {
            if (BrushIndex.Count > 0)
            {
                Writter.WriteTag(MakeTarge.Targe(1, ETargetType.Array));
                Writter.WriteList<uint>(BrushIndex, Writter.WriteUInt);
            }

        }
    }


    //地图跳转
    [System.Serializable]
    public class BITeleporter : IMapInfoProtoParse<BrushInfo.Types.Teleporter>, ISerialize
    {
        //下面是目标地图的信息
        public uint mapId = 1;
        public uint posX = 2;
        public uint posY = 3;
        public float offX = 4; //偏转坐标，不大于屏幕
        public float offY = 5;
        public int rangeX = 6;//范围
        public int rangeY = 7;

        public static BITeleporter Parse(BrushInfo.Types.Teleporter value)
        {
            if (value == null)
                return null;

            var obj = new BITeleporter();
            obj.ParseValue(value);
            return obj;
        }

        public void ParseValue(BrushInfo.Types.Teleporter value)
        {
            mapId = value.MapId;
            posX = value.PosX;
            posY = value.PosY;
            offX = value.OffX;
            offY = value.OffY;
            rangeX = value.RangeX;
            rangeY = value.RangeY;
        }
        public int CalcSize()
        {
            int size = 0;

            size += 1 + ComputorSize.ComputeUInt32Size(mapId);
            size += 1 + ComputorSize.ComputeUInt32Size(posX);
            size += 1 + ComputorSize.ComputeUInt32Size(posY);
            size += 1 + ComputorSize.ComputeFloatSize(offX);
            size += 1 + ComputorSize.ComputeFloatSize(offY);
            size += 1 + ComputorSize.ComputeInt32Size(rangeX);
            size += 1 + ComputorSize.ComputeInt32Size(rangeY);


            return size;
        }

        public void DeSerialize(InputStream Reader)
        {
            byte tage = 0;
            while ((tage = Reader.ReadTag()) != 0)
            {
                uint id = MakeTarge.UnTargeID(tage);

                switch (id)
                {
                    case 1:
                        mapId = Reader.ReadUInt();
                        break;
                    case 2:
                        posX = Reader.ReadUInt();
                        break;
                    case 3:
                        posY = Reader.ReadUInt();
                        break;
                    case 4:
                        offX = Reader.ReadFloat();
                        break;
                    case 5:
                        offY = Reader.ReadFloat();
                        break;
                    case 6:
                        rangeX = Reader.ReadInt();
                        break;
                    case 7:
                        rangeY = Reader.ReadInt();
                        break;
                    default:
                        break;
                }

            }
        }
        public void Serialize(OutputStream Writter)
        {
            Writter.WriteTag(MakeTarge.Targe(1, ETargetType.UInt));
            Writter.WriteUInt(mapId);

            Writter.WriteTag(MakeTarge.Targe(2, ETargetType.UInt));
            Writter.WriteUInt(posX);

            Writter.WriteTag(MakeTarge.Targe(3, ETargetType.UInt));
            Writter.WriteUInt(posY);

            Writter.WriteTag(MakeTarge.Targe(4, ETargetType.Float));
            Writter.WriteFloat(offX);

            Writter.WriteTag(MakeTarge.Targe(5, ETargetType.Float));
            Writter.WriteFloat(offY);

            Writter.WriteTag(MakeTarge.Targe(6, ETargetType.Int));
            Writter.WriteInt(rangeX);

            Writter.WriteTag(MakeTarge.Targe(7, ETargetType.Int));
            Writter.WriteInt(rangeY);
        }
    }

    //固定npc
    [System.Serializable]
    public class BIRigidNpc : IMapInfoProtoParse<BrushInfo.Types.RigidNpc>, ISerialize
    {
        [System.Serializable]
        public class NpcUnit : IMapInfoProtoParse<BrushInfo.Types.RigidNpc.Types.NpcUnit>, ISerialize
        {
            public uint npcId = 1;
            public uint width = 2; //区域宽
            public uint height = 3; //区域高
            public int offX = 4; //x偏移
            public int offY = 5; //y偏移
            public float rotaX = 6;
            public float rotaY = 7;
            public float rotaZ = 8;

            public static NpcUnit Parse(BrushInfo.Types.RigidNpc.Types.NpcUnit value)
            {
                var obj = new NpcUnit();
                obj.ParseValue(value);
                return obj;
            }


            public void ParseValue(BrushInfo.Types.RigidNpc.Types.NpcUnit value)
            {
                npcId = value.NpcId;
                width = value.Width;
                height = value.Height;
                offX = value.OffX;
                offY = value.OffY;
                rotaX = value.RotaX;
                rotaY = value.RotaY;
                rotaZ = value.RotaZ;
            }

            public int CalcSize()
            {
                int size = 0;
                size += 1 + ComputorSize.ComputeUInt32Size(npcId);
                size += 1 + ComputorSize.ComputeUInt32Size(width);
                size += 1 + ComputorSize.ComputeUInt32Size(height);
                size += 1 + ComputorSize.ComputeInt32Size(offX);
                size += 1 + ComputorSize.ComputeInt32Size(offY);
                size += 1 + ComputorSize.ComputeFloatSize(rotaX);
                size += 1 + ComputorSize.ComputeFloatSize(rotaY);
                size += 1 + ComputorSize.ComputeFloatSize(rotaZ);

                return size;
            }

            public void DeSerialize(InputStream Reader)
            {
                byte tage = 0;
                while ((tage = Reader.ReadTag()) != 0)
                {
                    uint id = MakeTarge.UnTargeID(tage);

                    switch (id)
                    {
                        case 1:
                            npcId = Reader.ReadUInt();
                            break;
                        case 2:
                            width = Reader.ReadUInt();
                            break;
                        case 3:
                            height = Reader.ReadUInt();
                            break;
                        case 4:
                            offX = Reader.ReadInt();
                            break;
                        case 5:
                            offY = Reader.ReadInt();
                            break;
                        case 6:
                            rotaX = Reader.ReadFloat();
                            break;
                        case 7:
                            rotaY = Reader.ReadFloat();
                            break;
                        case 8:
                            rotaZ = Reader.ReadFloat();
                            break;
                        default:
                            break;
                    }

                }
            }

            public void Serialize(OutputStream Writter)
            {
                Writter.WriteTag(MakeTarge.Targe(1, ETargetType.UInt));
                Writter.WriteUInt(npcId);

                Writter.WriteTag(MakeTarge.Targe(2, ETargetType.UInt));
                Writter.WriteUInt(width);

                Writter.WriteTag(MakeTarge.Targe(3, ETargetType.UInt));
                Writter.WriteUInt(height);

                Writter.WriteTag(MakeTarge.Targe(4, ETargetType.Int));
                Writter.WriteInt(offX);

                Writter.WriteTag(MakeTarge.Targe(5, ETargetType.Int));
                Writter.WriteInt(offY);

                Writter.WriteTag(MakeTarge.Targe(6, ETargetType.Float));
                Writter.WriteFloat(rotaX);

                Writter.WriteTag(MakeTarge.Targe(7, ETargetType.Float));
                Writter.WriteFloat(rotaY);

                Writter.WriteTag(MakeTarge.Targe(8, ETargetType.Float));
                Writter.WriteFloat(rotaZ);
            }
        }

        public List<NpcUnit> npcs = new List<NpcUnit>(); //npc数组

        public static BIRigidNpc Parse(BrushInfo.Types.RigidNpc value)
        {
            if (value == null)
                return null;

            var obj = new BIRigidNpc();
            obj.ParseValue(value);
            return obj;

        }

        public void ParseValue(BrushInfo.Types.RigidNpc value)
        {
            MapConfigLocalHelper.ListParse<NpcUnit, BrushInfo.Types.RigidNpc.Types.NpcUnit>(npcs, value.Npcs);
        }

        public void Serialize(OutputStream Writter)
        {
            if (npcs.Count > 0)
            {
                Writter.WriteTag(MakeTarge.Targe(1, ETargetType.Array));
                Writter.WriteList<NpcUnit>(npcs, Writter.WriteISerialize);
            }

        }

        public void DeSerialize(InputStream Reader)
        {
            byte tage = 0;
            while ((tage = Reader.ReadTag()) != 0)
            {
                uint id = MakeTarge.UnTargeID(tage);

                switch (id)
                {
                    case 1:
                        Reader.ReadList(npcs, Reader.ReadSerialize);
                        break;

                    default:
                        break;
                }

            }
        }

        public int CalcSize()
        {
            int size = 0;

            if (npcs.Count > 0)
            {
                size += 1 + ComputorSize.ComputiListSize<NpcUnit>(npcs, ComputorSize.ComputeMessageSize);
            }


            return size;
        }
    }

    //随机npc
    [System.Serializable]
    public class BIRandomNpc : IMapInfoProtoParse<BrushInfo.Types.RandomNpc>, ISerialize
    {
        public uint npcGroup = 1;
        public uint minNum = 2; //随机个数
        public uint maxNum = 3;

        public static BIRandomNpc Parse(BrushInfo.Types.RandomNpc value)
        {
            if (value == null)
                return null;

            var obj = new BIRandomNpc();

            obj.ParseValue(value);

            return obj;
        }

        public void ParseValue(BrushInfo.Types.RandomNpc value)
        {
            npcGroup = value.NpcGroup;
            minNum = value.MinNum;
            maxNum = value.MaxNum;
        }

        public int CalcSize()
        {
            int size = 0;
            size += 1 + ComputorSize.ComputeUInt32Size(npcGroup);
            size += 1 + ComputorSize.ComputeUInt32Size(minNum);
            size += 1 + ComputorSize.ComputeUInt32Size(maxNum);

            return size;
        }

        public void DeSerialize(InputStream Reader)
        {
            byte tage = 0;
            while ((tage = Reader.ReadTag()) != 0)
            {
                uint id = MakeTarge.UnTargeID(tage);

                switch (id)
                {
                    case 1:
                        npcGroup = Reader.ReadUInt();
                        break;
                    case 2:
                        minNum = Reader.ReadUInt();
                        break;
                    case 3:
                        maxNum = Reader.ReadUInt();
                        break;

                    default:
                        break;
                }

            }
        }

        public void Serialize(OutputStream Writter)
        {
            Writter.WriteTag(MakeTarge.Targe(1, ETargetType.UInt));
            Writter.WriteUInt(npcGroup);

            Writter.WriteTag(MakeTarge.Targe(2, ETargetType.UInt));
            Writter.WriteUInt(minNum);

            Writter.WriteTag(MakeTarge.Targe(3, ETargetType.UInt));
            Writter.WriteUInt(maxNum);
        }
    }

    [System.Serializable]
    public enum EBrushType
    {
        BrushType_None = 0,
        BrushType_RigNPC = 1,//固定npc
        BrushType_RanNPC = 2, //随机npc
        BrushType_Tel = 4, //跳转点
    }

    [System.Serializable]
    public class BrushInfoLocal : IMapInfoProtoParse<BrushInfo>, ISerialize
    {
        public EBrushType type = EBrushType.BrushType_None; // 画刷类型
        public BITeleporter tel;
        public BIRigidNpc rigidNpc;
        public BIRandomNpc randomNpc;

        public static BrushInfoLocal Parse(BrushInfo value)
        {
            var obj = new BrushInfoLocal();
            obj.ParseValue(value);
            return obj;
        }


        public void ParseValue(BrushInfo value)
        {
            type = (EBrushType)value.Type;

            tel = BITeleporter.Parse(value.Tel);
            rigidNpc = BIRigidNpc.Parse(value.RigidNpc);
            randomNpc = BIRandomNpc.Parse(value.RandomNpc);
        }

        public int CalcSize()
        {
            int size = 0;

            size += 1+ ComputorSize.ComputeEnumSize((int)type);

            if (tel != null)
                size += 1 + ComputorSize.ComputeMessageSize(tel);

            if (rigidNpc != null)
                size += 1 + ComputorSize.ComputeMessageSize(rigidNpc);

            if (randomNpc != null)
                size += 1 + ComputorSize.ComputeMessageSize(randomNpc);

            return size;
        }

        public void DeSerialize(InputStream Reader)
        {
            byte tage = 0;
            while ((tage = Reader.ReadTag()) != 0)
            {
                uint id = MakeTarge.UnTargeID(tage);

                switch (id)
                {
                    case 1:
                        type = (EBrushType)Reader.ReadInt();
                        break;
                    case 2:
                        tel = new BITeleporter();
                        Reader.ReadSerialize(tel);
                        break;
                    case 3:
                        rigidNpc = new BIRigidNpc();
                        Reader.ReadSerialize(rigidNpc);
                        break;
                    case 4:
                        randomNpc = new BIRandomNpc();
                        Reader.ReadSerialize(randomNpc);
                        break;

                    default:
                        break;
                }

            }
        }


        public void Serialize(OutputStream Writter)
        {
            Writter.WriteTag(MakeTarge.Targe(1, ETargetType.Int));
            Writter.WriteInt((int)type);

            if (tel != null)
            {
                Writter.WriteTag(MakeTarge.Targe(2, ETargetType.Class));
                Writter.WriteISerialize(tel);
            }

            if (rigidNpc != null)
            {
                Writter.WriteTag(MakeTarge.Targe(3, ETargetType.Class));
                Writter.WriteISerialize(rigidNpc);
            }

            if (randomNpc != null)
            {
                Writter.WriteTag(MakeTarge.Targe(4, ETargetType.Class));
                Writter.WriteISerialize(randomNpc);
            }
        }
    }

    [System.Serializable]
    public class PosInfoLocal : IMapInfoProtoParse<NpcCfg.Types.PosInfo>, ISerialize
    {
        public uint posX = 1;
        public uint posY = 2;

        public static PosInfoLocal Parse(NpcCfg.Types.PosInfo value)
        {
            var obj = new PosInfoLocal();
            obj.ParseValue(value);

            return obj;
        }

        public void ParseValue(NpcCfg.Types.PosInfo value)
        {
            posX = value.PosX;
            posY = value.PosY;
        }

        public int CalcSize()
        {
            int size = 0;

            size += 1 + ComputorSize.ComputeUInt32Size(posX);
            size += 1 + ComputorSize.ComputeUInt32Size(posY);

            return size;
        }

        public void DeSerialize(InputStream Reader)
        {
            byte tage = 0;
            while ((tage = Reader.ReadTag()) != 0)
            {
                uint id = MakeTarge.UnTargeID(tage);

                switch (id)
                {
                    case 1:
                        posX = Reader.ReadUInt();
                        break;
                    case 2:

                        posY = Reader.ReadUInt();
                        break;


                    default:
                        break;
                }

            }
        }

        public void Serialize(OutputStream Writter)
        {
            Writter.WriteTag(MakeTarge.Targe(1, ETargetType.UInt));
            Writter.WriteUInt(posX);

            Writter.WriteTag(MakeTarge.Targe(2, ETargetType.UInt));
            Writter.WriteUInt(posY);
        }
    }

    [System.Serializable]
    public class NpcCfgLocal : IMapInfoProtoParse<NpcCfg>, ISerialize
    {
        public uint type = 1; //1 固定  2 随机

        public uint BrushIndex = 2;

        public  List<PosInfoLocal> posInfos = new List<PosInfoLocal>(); //位置信息

        public static NpcCfgLocal Parse(NpcCfg value)
        {
            var obj = new NpcCfgLocal();

            obj.ParseValue(value);

            return obj;
        }

        public void ParseValue(NpcCfg value)
        {
            type = value.Type;
            BrushIndex = value.BrushIndex;
            MapConfigLocalHelper.ListParse<PosInfoLocal, NpcCfg.Types.PosInfo>(posInfos, value.PosInfos);

        }

        public int CalcSize()
        {
            int size = 0;

            size += 1 + ComputorSize.ComputeUInt32Size(type);
            size += 1 + ComputorSize.ComputeUInt32Size(BrushIndex);
           // size += 1 + ComputorSize.ComputeLengthSize(posInfos.Count);

            if (posInfos.Count > 0)
                size += 1 + ComputorSize.ComputiListSize<PosInfoLocal>(posInfos, ComputorSize.ComputeMessageSize);

            return size;
        }

        public void DeSerialize(InputStream Reader)
        {
            byte tage = 0;
            while ((tage = Reader.ReadTag()) != 0)
            {
                uint id = MakeTarge.UnTargeID(tage);

                switch (id)
                {
                    case 1:
                        type = Reader.ReadUInt();
                        break;
                    case 2:

                        BrushIndex = Reader.ReadUInt();
                        break;

                    case 3:
                        Reader.ReadList(posInfos, Reader.ReadSerialize);
                        break;
                    default:
                        break;
                }

            }
        }

        public void Serialize(OutputStream Writter)
        {
            Writter.WriteTag(MakeTarge.Targe(1, ETargetType.UInt));
            Writter.WriteUInt(type);

            Writter.WriteTag(MakeTarge.Targe(2, ETargetType.UInt));
            Writter.WriteUInt(BrushIndex);

            if (posInfos.Count > 0)
            {
                Writter.WriteTag(MakeTarge.Targe(3, ETargetType.Array));
                Writter.WriteList<PosInfoLocal>(posInfos, Writter.WriteISerialize);
            }

        }
    }

    [System.Serializable]
    public class TriggerMonsterGrpLocal : IMapInfoProtoParse<TriggerMonsterGrp>, ISerialize
    {
        public uint monsterGrpId = 0;//怪物组的id 因为怪物组合要写在另一张表里这里配置id
        uint minRate = 0;//最小概率
        uint maxRate = 0;
        uint continueRate = 0;
        List<uint> continueIndex = new List<uint>();
        int monstergroupIndex = 0;//怪物组index客户端编辑器不需要管

        public static TriggerMonsterGrpLocal Parse(TriggerMonsterGrp value)
        {
            var obj = new TriggerMonsterGrpLocal();

            obj.ParseValue(value);

            return obj;
        }


        public void ParseValue(TriggerMonsterGrp value)
        {
            monsterGrpId = value.MonsterGrpId;
            minRate = value.MinRate;
            maxRate = value.MaxRate;
            continueRate = value.ContinueRate;

            continueIndex.AddRange(value.ContinueIndex);

            monstergroupIndex = value.MonstergroupIndex;
        }
        public int CalcSize()
        {
            int size = 0;

            size += 1 + ComputorSize.ComputeUInt32Size(monsterGrpId);
            size += 1 + ComputorSize.ComputeUInt32Size(minRate);
            size += 1 + ComputorSize.ComputeUInt32Size(maxRate);
            size += 1 + ComputorSize.ComputeUInt32Size(continueRate);

            if (continueIndex.Count > 0)
                size += ComputorSize.ComputiListSize<uint>(continueIndex, ComputorSize.ComputeUInt32Size);

            size += ComputorSize.ComputeInt32Size(monstergroupIndex);

            return size;
        }

        public void DeSerialize(InputStream Reader)
        {
            byte tage = 0;
            while ((tage = Reader.ReadTag()) != 0)
            {
                uint id = MakeTarge.UnTargeID(tage);

                switch (id)
                {
                    case 1:
                        monsterGrpId = Reader.ReadUInt();
                        break;
                    case 2:

                        minRate = Reader.ReadUInt();
                        break;

                    case 3:
                        maxRate = Reader.ReadUInt();
                        break;
                    case 4:
                        continueRate = Reader.ReadUInt();
                        break;
                    case 5:
                        Reader.ReadList(continueIndex, Reader.ReadUInt);
                        break;

                    case 6:
                        monstergroupIndex = Reader.ReadInt();
                        break;
                    default:
                        break;
                }

            }
        }

        public void Serialize(OutputStream Writter)
        {
            Writter.WriteTag(MakeTarge.Targe(1, ETargetType.UInt));
            Writter.WriteUInt(monsterGrpId);

            Writter.WriteTag(MakeTarge.Targe(2, ETargetType.UInt));
            Writter.WriteUInt(minRate);

            Writter.WriteTag(MakeTarge.Targe(3, ETargetType.UInt));
            Writter.WriteUInt(maxRate);

            Writter.WriteTag(MakeTarge.Targe(4, ETargetType.UInt));
            Writter.WriteUInt(continueRate);

            if (continueIndex.Count > 0)
            {
                Writter.WriteTag(MakeTarge.Targe(5, ETargetType.Array));
                Writter.WriteList<uint>(continueIndex, Writter.WriteUInt);
            }


            Writter.WriteTag(MakeTarge.Targe(6, ETargetType.Int));
            Writter.WriteInt(monstergroupIndex);
        }
    }
    [System.Serializable]
    public class MapConfigLocal : IMapInfoProtoParse<MapCfgInfo>, ISerialize
    {
        public uint width = 1; //地图宽
        public uint height = 2; //高
        public List<BrushArray> BrushIndex = new List<BrushArray>();  //地图每个格子对应的笔刷数组
        public List<BrushInfoLocal> brushInfos = new List<BrushInfoLocal>(); //所有笔刷

        public List<NpcCfgLocal> npcInfos = new List<NpcCfgLocal>(); //所有npc
        public uint mapId = 0; //地图id
        public bool bStatic = false; //是否静态地图

        public List<TriggerMonsterGrpLocal> monsters = new List<TriggerMonsterGrpLocal>();//地图每个格子中怪物暗雷 

        public static MapConfigLocal Parse(MapCfgInfo value)
        {
            var obj = new MapConfigLocal();

            obj.ParseValue(value);

            return obj;

        }

        public void ParseValue(MapCfgInfo value)
        {
            width = value.Width;
            height = value.Height;
            MapConfigLocalHelper.ListParse<BrushArray, MapCfgInfo.Types.BrushArray>(BrushIndex, value.BrushIndex);
            MapConfigLocalHelper.ListParse<BrushInfoLocal, BrushInfo>(brushInfos, value.BrushInfos);
            MapConfigLocalHelper.ListParse<NpcCfgLocal, NpcCfg>(npcInfos, value.NpcInfos);

            mapId = value.MapId;
            bStatic = value.BStatic;

            MapConfigLocalHelper.ListParse<TriggerMonsterGrpLocal, TriggerMonsterGrp>(monsters, value.Monsters);
        }

        public int CalcSize()
        {
            int size = 0;

            size += 1 + ComputorSize.ComputeUInt32Size(width);
            size += 1 + ComputorSize.ComputeUInt32Size(height);

            if (BrushIndex.Count > 0)
                size += 1 + ComputorSize.ComputiListSize<BrushArray>(BrushIndex, ComputorSize.ComputeMessageSize);
            if (brushInfos.Count > 0)
                size += 1 + ComputorSize.ComputiListSize<BrushInfoLocal>(brushInfos, ComputorSize.ComputeMessageSize);
            if (npcInfos.Count > 0)
                size += 1 + ComputorSize.ComputiListSize<NpcCfgLocal>(npcInfos, ComputorSize.ComputeMessageSize);

            size += 1 + ComputorSize.ComputeUInt32Size(mapId);
            size += 1 + ComputorSize.ComputeBoolSize(bStatic);

            if (monsters.Count > 0)
                size += 1 + ComputorSize.ComputiListSize<TriggerMonsterGrpLocal>(monsters, ComputorSize.ComputeMessageSize);

            return size;
        }

        public void DeSerialize(InputStream Reader)
        {
            byte tage = 0;
            while ((tage = Reader.ReadTag()) != 0)
            {
                uint id = MakeTarge.UnTargeID(tage);

                switch (id)
                {
                    case 1:
                        width = Reader.ReadUInt();
                        break;
                    case 2:

                        height = Reader.ReadUInt();
                        break;

                    case 3:
                        Reader.ReadList(BrushIndex, Reader.ReadSerialize);
                        break;
                    case 4:
                        Reader.ReadList(brushInfos, Reader.ReadSerialize);
                        break;
                    case 5:
                        Reader.ReadList(npcInfos, Reader.ReadSerialize);
                        break;

                    case 6:
                        mapId = Reader.ReadUInt();
                        break;
                    case 7:
                        bStatic = Reader.ReadBool();
                        break;
                    case 8:
                        Reader.ReadList(monsters, Reader.ReadSerialize);
                        break;
                    default:
                        break;
                }

            }
        }

        public void Serialize(OutputStream Writter)
        {
            Writter.WriteTag(MakeTarge.Targe(1, ETargetType.UInt));
            Writter.WriteUInt(width);

            Writter.WriteTag(MakeTarge.Targe(2, ETargetType.UInt));
            Writter.WriteUInt(height);

            if (BrushIndex.Count > 0)
            {
                Writter.WriteTag(MakeTarge.Targe(3, ETargetType.Array));
                Writter.WriteList<BrushArray>(BrushIndex, Writter.WriteISerialize);
            }

            if (brushInfos.Count > 0)
            {
                Writter.WriteTag(MakeTarge.Targe(4, ETargetType.Array));
                Writter.WriteList<BrushInfoLocal>(brushInfos, Writter.WriteISerialize);
            }

            if (npcInfos.Count > 0)
            {
                Writter.WriteTag(MakeTarge.Targe(5, ETargetType.Array));
                Writter.WriteList<NpcCfgLocal>(npcInfos, Writter.WriteISerialize);
            }


            Writter.WriteTag(MakeTarge.Targe(6, ETargetType.UInt));
            Writter.WriteUInt(mapId);

            Writter.WriteTag(MakeTarge.Targe(7, ETargetType.UInt));
            Writter.WriteBool(bStatic);

            if (monsters.Count > 0)
            {
                Writter.WriteTag(MakeTarge.Targe(8, ETargetType.Array));
                Writter.WriteList<TriggerMonsterGrpLocal>(monsters, Writter.WriteISerialize);
            }

        }
    }






}
