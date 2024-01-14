using Framework;
using Lib.Core;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Logic
{
    /// <summary>
    /// 主模型(时装)着色索引
    /// </summary>
    public enum ETintIndex : int
    {
        None = -1,
        R = 0,
        G = 1,
        B = 2,
        A = 3,
    }

    public enum EHeroModelParts : int
    {
        None = -1,
        Main = 0,
        Weapon = 1,
        Jewelry_Head = 2,   //头饰
        Jewelry_Back = 3,   //背饰
        Jewelry_Waist = 4,  //腰饰
        Jewelry_Face = 5,   //脸饰
        Count,
    }

#if false
    public class HeroVirtualGameObject : VirtualGameObject
    {
        public const string kClothesMesh = "show_clothes_mesh";
        public const string kHairMesh = "show_hair_mesh";        

        public bool isHighModel = false;
        public Material mMaterial;
        public Material mHairMaterials;
        public List<Material> mAddtiveMaterials;

        protected override void _OnAssetProcess(GameObject go)
        {            
            //对于高模的时装特殊处理 因为由多个子模型组成
            if (id == (int)EHeroModelParts.Main && isHighModel)
            {                
                Renderer[] renderers = gameObject.GetComponentsInChildren<Renderer>();
                for (int i = 0; i < renderers.Length; ++i)
                {
                    Renderer renderer = renderers[i];
                    if (renderer.gameObject.name.EndsWith(kClothesMesh, System.StringComparison.Ordinal))
                    {
                        if (mMaterial == null)
                        {
                            mMaterial = renderer.material;
                        }
                        else
                        {
                            if (mAddtiveMaterials == null)
                            {
                                mAddtiveMaterials = new List<Material>();
                            }
                            mAddtiveMaterials.Add(renderer.material);
                        }
                    }
                    else if (renderer.gameObject.name.EndsWith(kHairMesh, System.StringComparison.Ordinal))
                    {
                        mHairMaterials = renderer.material;
                    }
                }
            }
            else
            {
                Renderer renderer = gameObject.GetComponentInChildren<Renderer>();
                mMaterial = renderer.material;
            }            
        }

        public override void Dispose()
        {
            isHighModel = false;
            base.Dispose();
        }

        protected override void _CleanGameObject()
        {
            if (eState == EState.Empty)
                return;

            if (mAddtiveMaterials != null)
            {
                for (int i = 0; i < mAddtiveMaterials.Count; ++i)
                {
                    GameObject.DestroyImmediate(mAddtiveMaterials[i]);
                }
                mAddtiveMaterials.Clear();
                mAddtiveMaterials = null;
            }

            GameObject.DestroyImmediate(mMaterial);
            GameObject.DestroyImmediate(mHairMaterials);
            mMaterial = null;
            mHairMaterials = null;

            base._CleanGameObject();
        }
    }
#endif

    public class HeroDisplayControl : DisplayControl<EHeroModelParts>
    {
        private List<VirtualGameObject> additionalWeaponVirtualGameObjects = new List<VirtualGameObject>();


        public static HeroDisplayControl Create(bool highMode)
        {
            HeroDisplayControl heroDisplayControl = PoolManager.Fetch(typeof(HeroDisplayControl)) as HeroDisplayControl;
            if (heroDisplayControl.mParts == null || heroDisplayControl.mParts.Length != (int)EHeroModelParts.Count)
            {
                heroDisplayControl.mParts = new VirtualGameObject[(int)EHeroModelParts.Count];
            }
#if false
            heroDisplayControl.isHighMode = highMode;
#endif
            return heroDisplayControl;
        }

        public static void Destory(ref HeroDisplayControl control)
        {
            if (control != null)
            {
                try
                {
                    control.Dispose();
                    PoolManager.Recycle(control, false);
                    control = null;
                }
                catch (Exception e)
                {
                    control = null;
                    DebugUtil.LogException(e);
                    DebugUtil.LogError("Destory HeroDisplayControl Exception");
                }
            }

        }

        public override void Dispose()
        {
            base.Dispose();
            for (int index = 0, len = additionalWeaponVirtualGameObjects.Count; index < len; index++)
            {
                additionalWeaponVirtualGameObjects[index].Dispose();
            }
            additionalWeaponVirtualGameObjects.Clear();
        }
        public HeroDisplayControl() { }

#if false
        private bool isHighMode = false;        

        /// <summary>
        /// 设置模型颜色
        /// </summary>
        /// <param name="part"></param>
        /// <param name="index"></param>
        /// <param name="color"></param>
        public void SetColor(EHeroModelParts heroModelParts, ETintIndex part, Color color)
        {
            if (heroModelParts == EHeroModelParts.None || heroModelParts == EHeroModelParts.Count)
                return;

            if (heroModelParts == EHeroModelParts.Main && part == ETintIndex.None)
                return;

            HeroVirtualGameObject virtualGameObject = GetPart(heroModelParts) as HeroVirtualGameObject;
            if (virtualGameObject == null || virtualGameObject.eState != VirtualGameObject.EState.Loaded || virtualGameObject.gameObject == null)
                return;
            //DebugUtil.LogFormat( ELogType.eFashion, " EHeroModelParts:{0}   ETintIndex:{1}   color{2} ", heroModelParts, part, color);
            int channelIndex = (int)part;
            
            if (heroModelParts == EHeroModelParts.Main && part == ETintIndex.A && virtualGameObject.isHighModel)
            {
                //高模的头发是独立的材质
                Material material = virtualGameObject.mHairMaterials;
                if(material != null)
                {
                    material.SetColor(HeroVirtualGameObject.kShaderTintIDs[channelIndex], color);                    
                    material.SetColor(HeroVirtualGameObject.kHairShaderOutLineID, color * 0.5f);    //高模头发特殊处理下描边颜色
                }                
                else
                {
                    DebugUtil.LogErrorFormat("高模 {0} 头发材质未找到, 先检查模型是否为高模, 再检查模型是否拥有 xx_{1}节点", virtualGameObject.sAssetPath, HeroVirtualGameObject.kHairMesh);
                }
            }
            else
            {
                Material material = virtualGameObject.mMaterial;
                if (material != null)
                {
                    material.SetColor(HeroVirtualGameObject.kShaderTintIDs[channelIndex], color);
                }
                else
                {
                    DebugUtil.LogErrorFormat("没有找到部位 {0}({1})的材质, 先检查模型", virtualGameObject.gameObject.name, heroModelParts);
                }

                List<Material> addtiveMaterials = virtualGameObject.mAddtiveMaterials;
                if (addtiveMaterials != null)
                {
                    for (int i = 0; i < addtiveMaterials.Count; ++i)
                    {
                        addtiveMaterials[i].SetColor(HeroVirtualGameObject.kShaderTintIDs[channelIndex], color);
                    }
                }
            }
        }

        protected override VirtualGameObject CreateVGo()
        {
            HeroVirtualGameObject vgo = PoolManager.Fetch<HeroVirtualGameObject>();
            vgo.isHighModel = isHighMode;
            return vgo;
        }
#else
        /// <summary>
        /// 设置模型颜色
        /// </summary>
        /// <param name="part"></param>
        /// <param name="index"></param>
        /// <param name="color"></param>
        public void SetColor(EHeroModelParts heroModelParts, ETintIndex tintIndex, Color color)
        {
            if (tintIndex == ETintIndex.None)
                return;

            VirtualGameObject gameObject = GetPart(heroModelParts);
            if (gameObject != null && gameObject.eState == VirtualGameObject.EState.Loaded && gameObject.gameObject != null)
            {
                if (gameObject.gameObject.TryGetComponent<TintController>(out TintController tintController))
                {
                    MaterialPropertyBlock materialPropertyBlock = tintController.GetOrCreate();
                    materialPropertyBlock.SetFloat(Constants.kUseTintColor, 1);
                    materialPropertyBlock.SetColor(Constants.kShaderTintIDs[(int)tintIndex], color);
                    tintController.Apply();
                }
            }
            //副手武器 
            if (heroModelParts == EHeroModelParts.Weapon && additionalWeaponVirtualGameObjects != null)
            {
                for (int i = 0; i < additionalWeaponVirtualGameObjects.Count; i++)
                {
                    var additionalWeaponGo = additionalWeaponVirtualGameObjects[i];
                    if (additionalWeaponGo != null && additionalWeaponGo.eState == VirtualGameObject.EState.Loaded && additionalWeaponGo.gameObject != null)
                    {
                        if (additionalWeaponGo.gameObject.TryGetComponent<TintController>(out TintController aw_tintController))
                        {
                            MaterialPropertyBlock materialPropertyBlock = aw_tintController.GetOrCreate();
                            materialPropertyBlock.SetFloat(Constants.kUseTintColor, 1);
                            materialPropertyBlock.SetColor(Constants.kShaderTintIDs[(int)tintIndex], color);
                            aw_tintController.Apply();
                        }
                    }
                }
            }
        }
#endif

        /// <summary>
        /// 加载武器model 
        /// </summary>
        /// <param name="modelPart"></param>
        /// <param name="cSVEquipmentData"></param>
        public void LoadWeaponModel(Table.CSVEquipment.Data cSVEquipmentData, bool _highModel)
        {
            if (cSVEquipmentData != null)
            {
                string modelPath = _highModel ? cSVEquipmentData.show_model : cSVEquipmentData.model;
                //主手武器
                LoadMainModel(EHeroModelParts.Weapon, modelPath, EHeroModelParts.Main, cSVEquipmentData.equip_pos);
                //副手武器
                for (int index = 0, len = additionalWeaponVirtualGameObjects.Count; index < len; index++)
                {
                    this.Remove(additionalWeaponVirtualGameObjects[index]);
                }
                additionalWeaponVirtualGameObjects.Clear();

                var additionalModelPath = _highModel ? cSVEquipmentData.additional_show_model : cSVEquipmentData.additional_model;
                if (additionalModelPath != null && additionalModelPath.Count > 0)
                {
                    for (int index = 0, len = additionalModelPath.Count; index < len; index++)
                    {
                        additionalWeaponVirtualGameObjects.Add(this.LoadAddition(EHeroModelParts.Weapon, additionalModelPath[index], EHeroModelParts.Main, cSVEquipmentData.additional_equip_pos[index]));
                    }
                }
            }
        }

        /// <summary>
        /// 加载武器model 
        /// </summary>
        /// <param name="modelPart"></param>
        /// <param name="cSVEquipmentData"></param>
        public void LoadWeaponModel(Table.CSVFashionWeaponModel.Data cSVFashionWeaponData, bool _highModel)
        {
            if (cSVFashionWeaponData != null)
            {
                string modelPath = _highModel ? cSVFashionWeaponData.model_show : cSVFashionWeaponData.model;
                //主手武器
                LoadMainModel(EHeroModelParts.Weapon, modelPath, EHeroModelParts.Main, cSVFashionWeaponData.equip_pos);
                //副手武器
                for (int index = 0, len = additionalWeaponVirtualGameObjects.Count; index < len; index++)
                {
                    this.Remove(additionalWeaponVirtualGameObjects[index]);
                }
                additionalWeaponVirtualGameObjects.Clear();
                var additionalModelPath = _highModel ? cSVFashionWeaponData.additional_show_model : cSVFashionWeaponData.additional_model;
                if (additionalModelPath != null && additionalModelPath.Count > 0)
                {
                    for (int index = 0, len = additionalModelPath.Count; index < len; index++)
                    {
                        additionalWeaponVirtualGameObjects.Add(this.LoadAddition(EHeroModelParts.Weapon, additionalModelPath[index], EHeroModelParts.Main, cSVFashionWeaponData.additional_equip_pos[index]));
                    }
                }
            }
        }
    }
}