using System;
using UnityEngine;

namespace Framework
{
    public class Consts
    {
        public static readonly string GameNameSpaceStr = "Framework";
        public static readonly string IsILRuntimeModeKey = "IsILRuntimeMode";

        public static readonly int ID_Emissive_Color = Shader.PropertyToID("_Emissive_Color");
        public static readonly int ID_NightSchedule = Shader.PropertyToID("_NightSchedule");

        public static readonly int ID_Schedule = Shader.PropertyToID("_Schedule");
        public static readonly int ID_Color = Shader.PropertyToID("_Color");

        public static readonly int ID_SnowColor = Shader.PropertyToID("_SnowColor");
        public static readonly int ID_SnowColorDark = Shader.PropertyToID("_SnowColorDark");
        //public static readonly int ID_WaterHeight = Shader.PropertyToID("_WaterHeight");
        //public static readonly int ID_SnowFade = Shader.PropertyToID("_SnowFade");
        //public static readonly int ID_SnowRatio = Shader.PropertyToID("_SnowRatio");
        //public static readonly int ID_SnowHeight = Shader.PropertyToID("_SnowHeight");        
        public static readonly int ID_WeatherTexture = Shader.PropertyToID("_SnowNoise");
        public static readonly int ID_SnowData = Shader.PropertyToID("_SnowData");
        public static readonly int ID_WindParam = Shader.PropertyToID("_WindParam");
        public static readonly int ID_WindDensity = Shader.PropertyToID("_WindDensity");
        public static readonly int ID_RipperData = Shader.PropertyToID("_RipperData");
        public static readonly int ID_NoiseScale = Shader.PropertyToID("_NoiseScale");        
                
        public static readonly int _AmbientSH_ID = Shader.PropertyToID("_AmbientSH");
        public static readonly int _CollideInfo_ID = Shader.PropertyToID("_CollideInfo");
        public static readonly int _MatrixBuffer_ID = Shader.PropertyToID("_MatrixBuffer");

        public static readonly string _SNOW_ON_Keyword = "_SNOW_ON";
        public static readonly string _RIPPLE_ON_Keyword = "_RIPPLE_ON";
        public static readonly string _INTERACTIVE_ON_Keyword = "_INTERACTIVE_ON";

        public static readonly string _DEPTHMAP_ON_Keyword = "_DEPTHMAP_ON";
        public static readonly string _OPAQUE_TEXTURE_ON_Keyword = "_OPAQUE_TEXTURE_ON";

        public static readonly DateTime START_TIME = new DateTime(1970, 1, 1);

        public static readonly string persistentDataPath = Application.persistentDataPath;

        public static readonly string sLogicPassword = "SGq%+beiey2wx%3~";
        public static readonly DateTime sConfigDate = new DateTime(2022, 8, 9);
    }
}
