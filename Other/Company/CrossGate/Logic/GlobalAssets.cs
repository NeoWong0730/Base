using Framework;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.U2D;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets;

namespace Logic
{
    public static class GlobalAssets
    {
        private static readonly Dictionary<string, AsyncOperationHandle> mAssets = new Dictionary<string, AsyncOperationHandle>(32);

        //字体
        public const string sFont_AddHp = "AddHp_font.fontsettings";
        public const string sFont_Conver = "Conver_font.fontsettings";
        public const string sFont_Crit = "Crit_font.fontsettings";
        public const string sFont_Drunk = "Drunk_font.fontsettings";
        public const string sFont_Normal = "Normal_font.fontsettings";
        public const string sFont_Poison = "Poison_font.fontsettings";
        public const string sFont_Special = "Special_font.fontsettings";
        public const string sFont_zhanghaishanruixian = "zhanghaishanruixian.ttf";

        //表情
        public const string sEmoji_0 = "emoji";

        //图集
        public const string sAtlas_Chat = "ChatUIAtlas";

        //Material
        public const string sMat_ImageFade = "Material/Loading/ImageFade.mat";
        public const string sMat_Gray = "Material/Material_Gray.mat";
        public const string sMat_CusScreenFade = "Material/CusScreenFade.mat";

        //Texture
        public const string sTexture_Tt_paint = "Texture/Paint/brush-2.png";

        //prefab
        public const string sPrefab_PropIcon = "UI/Common/PropItem.prefab";
        public const string sPrefab_LightPrompt = "UI/Common/LightPrompt.prefab";
        public const string sPrefab_FlightItem = "UI/Common/FlightItem.prefab";
        public const string sPrefab_Fx_yindao = "Prefab/Fx/scene/Fx_yindao.prefab";
        public const string sPrefab_Fx_suoding_1 = "Prefab/Fx/Fx_suoding_1.prefab";
        public const string sPrefab_Fx_suoding_2 = "Prefab/Fx/Fx_suoding_2.prefab";
        public const string sPrefab_Fx_painting = "Prefab/Fx/UI/Fx_ui_PaintBoard.prefab";
        public const string sPrefab_Fx_endpaint = "Prefab/Fx/UI/Fx_ui_PaintBoard01.prefab";
        public const string sPrefab_Fx_limitpaint = "Prefab/Fx/UI/Fx_ui_PaintBoard02.prefab";
        public const string sPrefab_Fx_item = "Prefab/Fx/UI/Fx_ui_item.prefab";
        public const string sPrefab_RedPoint_Big = "UI/Cell/UI_RedTips_Big.prefab";
        public const string sPrefab_RedPoint_Small = "UI/Cell/UI_RedTips_Small.prefab";
        public const string sPrefab_BlockClick = "UI/UI_BlockClick.prefab";
        public const string sPrefab_ViewLock_Tips = "UI/Common/View_Lock_Tips.prefab";

        //TODO:这个可以异步动态加载
        public const string sPrefab_careerSceneShow = "Prefab/ShowScene/careerShowScene.prefab";
        public const string sPrefab_3dcamera = "Prefab/ShowScene/camera.prefab";

        private static void Preload<T>(string name)
        {
            AsyncOperationHandle<T> handle = Addressables.LoadAssetAsync<T>(name);
            mAssets.Add(name, handle);
        }

        public static void Preload()
        {
            //Material
            Preload<Material>(sMat_ImageFade);
            Preload<Material>(sMat_Gray);
            Preload<Material>(sMat_CusScreenFade);

            //Texture
            Preload<Texture>(sTexture_Tt_paint);

            //prefab
            Preload<GameObject>(sPrefab_PropIcon);
            Preload<GameObject>(sPrefab_LightPrompt);
            Preload<GameObject>(sPrefab_FlightItem);
            Preload<GameObject>(sPrefab_Fx_yindao);
            Preload<GameObject>(sPrefab_Fx_suoding_1);
            Preload<GameObject>(sPrefab_Fx_suoding_2);
            Preload<GameObject>(sPrefab_Fx_painting);
            Preload<GameObject>(sPrefab_Fx_endpaint);
            Preload<GameObject>(sPrefab_Fx_limitpaint);
            Preload<GameObject>(sPrefab_Fx_item);
            Preload<GameObject>(sPrefab_RedPoint_Big);
            Preload<GameObject>(sPrefab_RedPoint_Small);
            Preload<GameObject>(sPrefab_BlockClick);
            Preload<GameObject>(sPrefab_ViewLock_Tips);

            //TODO:这个可以异步动态加载
            Preload<GameObject>(sPrefab_careerSceneShow);
            Preload<GameObject>(sPrefab_3dcamera);
        }

        public static void Unload()
        {
            foreach (var font in mAssets)
            {
                AsyncOperationHandle handle = font.Value;
                Addressables.Release(handle);
            }
            mAssets.Clear();
        }

        public static T GetAsset<T>(string assetPath) where T : UnityEngine.Object
        {
            if (!mAssets.TryGetValue(assetPath, out AsyncOperationHandle handle))
            {
                handle = Addressables.LoadAssetAsync<T>(assetPath);

                if (!handle.IsDone)
                {
                    Debug.LogWarningFormat("资源阻塞加载 {0}", assetPath);
                    handle.WaitForCompletion();
                }

                mAssets.Add(assetPath, handle);
            }

            return handle.Result as T;
        }
    }
}