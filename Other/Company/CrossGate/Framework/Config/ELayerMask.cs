using System.Collections.Generic;
using UnityEngine;
using Lib.Core;

public enum ELayerMask : int
{
    None = 0,
    Default = 1 << 0,
    TransparentFX = 1 << 1,
    IgnoreRaycast = 1 << 2,
    //1 << 3,
    Water = 1 << 4,
    UI = 1 << 5,
    //1 << 6,
    //1 << 7,
    PostProcessing = 1 << 8,
    Grass = 1 << 9,
    Build = 1 << 10,
    Player = 1 << 11,
    Partner = 1 << 12,      //FightActor
    OtherActor = 1 << 13,
    Monster = 1 << 14,
    Terrain = 1 << 15,
    Tree = 1 << 16,
    ModelShow = 1 << 17,
    HidingSceneActor = 1 << 18,
    WalkArea = 1 << 19,
    SmallObject = 1 << 20,
    NPC = 1 << 21,
    UnimportantFX = 1 << 22,
    UnimportantUIFX = 1 << 23,
    WeatherFX = 1 << 24,
    Video = 1 << 25,
    ModelShowScene = 1 << 26,
    ExpandUI = 1 << 28,
    CutSceneActor = 1 << 29,
}

public static class LayerMaskUtil
{
    readonly static Dictionary<ELayerMask, int> maskToInt = new Dictionary<ELayerMask, int>()
    {
        { ELayerMask.Default, 0 },
        { ELayerMask.TransparentFX, 1 },
        { ELayerMask.IgnoreRaycast, 2 },

        { ELayerMask.Water, 4 },
        { ELayerMask.UI, 5 },


        { ELayerMask.PostProcessing, 8 },
        { ELayerMask.Grass, 9 },
        { ELayerMask.Build, 10 },
        { ELayerMask.Player, 11 },
        { ELayerMask.Partner, 12 },
        { ELayerMask.OtherActor, 13 },
        { ELayerMask.Monster, 14 },
        { ELayerMask.Terrain, 15 },
        { ELayerMask.Tree, 16 },
        { ELayerMask.ModelShow, 17 },
        { ELayerMask.HidingSceneActor, 18 },
        { ELayerMask.WalkArea, 19 },
        { ELayerMask.SmallObject, 20 },
        { ELayerMask.NPC, 21 },
        { ELayerMask.UnimportantFX, 22 },
        { ELayerMask.UnimportantUIFX, 23 },
        { ELayerMask.WeatherFX, 24 },
        { ELayerMask.Video, 25 },
        { ELayerMask.ModelShowScene, 26 },
        { ELayerMask.ExpandUI, 28 },
        { ELayerMask.CutSceneActor, 29 },
    };

    public static bool ContainLayer(ELayerMask layerMasks, ELayerMask layerMask)
    {
        return ((int)layerMask & (int)layerMasks) == (int)layerMask;
    }

    public static bool ContainLayer(int layerMasks, int layerMask)
    {
        return ((int)layerMask & layerMasks) == layerMask;
    }

    public static bool ContainLayerInt(ELayerMask layerMasks, int layerInt)
    {
        return ContainLayer((int)layerMasks, GetMask(layerInt));
    }

    public static bool EqualsLayerInt(ELayerMask layerMask, int layerInt)
    {
        return (int)layerMask == (1 << layerInt);
    }

    public static bool EqualsLayerMask(ELayerMask layerMask, int layerMaskInt)
    {
        return layerMaskInt == (int)layerMask;
    }

    public static int GetMask(int layerInt)
    {
        return 1 << layerInt;
    }

    public static int MaskToLayer(ELayerMask layerMask)
    {        
        if (maskToInt.TryGetValue(layerMask, out int layer))
        {
            return layer;
        }
        return -1;
    }    

    public static void Setlayer(this Transform trans, ELayerMask layerMask, bool direct = false)
    {
        if (trans == null)
            return;

        int layer = MaskToLayer(layerMask);
        trans.gameObject.layer = layer;
        if (!direct)
        {
            int count = trans.childCount;
            for (int i = 0; i < count; ++i)
            {
                trans.GetChild(i).Setlayer(layer);
            }
        }
    }
}