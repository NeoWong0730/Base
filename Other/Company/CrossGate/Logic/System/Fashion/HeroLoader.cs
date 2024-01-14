using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Table;
using Lib.Core;
using System.Linq;

namespace Logic
{
    public class dressData
    {
        public ETintIndex tintIndex; //染色部位
        public Color color;          //染色色值
    }

    public class HeroLoader
    {
        public static HeroLoader Create(bool highModel)
        {
            HeroLoader heroLoader = new HeroLoader();
            heroLoader.heroDisplay = HeroDisplayControl.Create(highModel);
            heroLoader.heroDisplay.onLoaded += heroLoader.OnShowModelLoaded;
            heroLoader._highModel = highModel;
            return heroLoader;
        }

        protected HeroLoader() { }

        private VirtualGameObject _parent;
        public HeroDisplayControl heroDisplay;

        public Action<GameObject> action;
        private Action m_UpdateVirturlWeaponAnimation;
        public uint[] showParts = new uint[(int)EHeroModelParts.Count];
        public Dictionary<uint, List<dressData>> datas;

        protected uint heroId;
        private uint equipId;

        protected bool _highModel;

        private bool _showWeapon = true;
        public bool showWeapon
        {
            get { return _showWeapon; }
            set
            {
                if (_showWeapon != value)
                {
                    _showWeapon = value;

                    if (_showWeapon)
                    {
                        LoadWeaponPart(showParts[(int)EHeroModelParts.Weapon], equipId);
                    }
                    else
                    {
                        heroDisplay.GetPart(EHeroModelParts.Weapon)?.Dispose();
                    }
                }
            }
        }

        public void LoadHero(uint _heroId, uint _equipId, ELayerMask eLayerMask, Dictionary<uint, List<dressData>> _datas, Action<GameObject> _action, bool _loadFirstModel = false)
        {
            heroId = _heroId;
            equipId = _equipId;
            action = _action;
            datas = _datas;

            heroDisplay.eLayerMask = eLayerMask;
            EHeroModelParts eHeroModelParts = EHeroModelParts.None;

            if (_loadFirstModel)
            {
                datas.Clear();
                //datas.Add(CSVFashionClothes.Instance.GetDictData().First().Key, new List<dressData>());
                datas.Add(CSVFashionClothes.Instance.GetByIndex(0).id, new List<dressData>());
            }
            showParts[(int)EHeroModelParts.Weapon] = 0;
            foreach (var id in datas.Keys)
            {
                if (!Sys_Fashion.Instance.parts.TryGetValue(id, out eHeroModelParts))
                {
                    continue;
                }
                if (eHeroModelParts == EHeroModelParts.Weapon)
                {
                    showParts[(int)EHeroModelParts.Weapon] = id;
                    continue;
                }

                LoadModelParts(id, eHeroModelParts);
            }
            LoadWeaponPart(showParts[(int)EHeroModelParts.Weapon], equipId);
        }

        public void LoadWeaponPart(uint fashionWeaponId, uint _equipId)
        {
            equipId = _equipId;
            showParts[(int)EHeroModelParts.Weapon] = fashionWeaponId;

            if (!_showWeapon)
                return;

#if UNITY_EDITOR
            if (equipId == Constants.UMARMEDID)
            {
                DebugUtil.Log(ELogType.eFashion, "当前身上没有装备");
            }
#endif
            if (fashionWeaponId != 0)
            {
                CSVEquipment.Data cSVEquipmentData = CSVEquipment.Instance.GetConfData(equipId);
                if (cSVEquipmentData != null)
                {
                    FashionWeapon fashionWeapon = Sys_Fashion.Instance._FashionWeapons.Find(x => x.Id == fashionWeaponId);
                    if (fashionWeapon != null)
                    {
                        string modelPath = null;
                        string socketName = null;
                        uint _weaponfashionModeId = fashionWeaponId * 10 + cSVEquipmentData.equipment_type;
                        CSVFashionWeaponModel.Data _cSVFashionWeaponModelData = CSVFashionWeaponModel.Instance.GetConfData(_weaponfashionModeId);
                        if (_cSVFashionWeaponModelData != null)
                        {
                            modelPath = _highModel ? _cSVFashionWeaponModelData.model_show : _cSVFashionWeaponModelData.model;
                            socketName = _cSVFashionWeaponModelData.equip_pos;
                            //heroDisplay.LoadMainModel(EHeroModelParts.Weapon, modelPath, EHeroModelParts.Main, socketName);
                            heroDisplay.LoadWeaponModel(_cSVFashionWeaponModelData, _highModel);
                        }
                    }
                }
            }
            else
            {
                CSVEquipment.Data cSVEquipmentData = CSVEquipment.Instance.GetConfData(equipId);
                if (cSVEquipmentData != null)
                {
                    string modelPath = _highModel ? cSVEquipmentData.show_model : cSVEquipmentData.model;
                    //heroDisplay.LoadMainModel(EHeroModelParts.Weapon, modelPath, EHeroModelParts.Main, cSVEquipmentData.equip_pos);
                    heroDisplay.LoadWeaponModel(cSVEquipmentData, _highModel);
                }
            }

            VirtualGameObject part = heroDisplay.GetPart(EHeroModelParts.Weapon);
            if (part != null)
            {
                if (part.eState == VirtualGameObject.EState.Loaded)
                {
                    OnShowModelLoaded((int)EHeroModelParts.Weapon);
                }
            }
        }

        public void LoadVirtualWeapon(uint equipId, Action updateVirturlWeaponAnimation)
        {

            VirtualGameObject vgo = heroDisplay.GetPart(EHeroModelParts.Weapon);
            if (vgo.eState == VirtualGameObject.EState.Loaded)
            {
                updateVirturlWeaponAnimation?.Invoke();
            }
            CSVEquipment.Data cSVEquipmentData = CSVEquipment.Instance.GetConfData(equipId);

            if (cSVEquipmentData != null)
            {
                //heroDisplay.LoadMainModel(EHeroModelParts.Weapon, cSVEquipmentData.model, EHeroModelParts.Main, cSVEquipmentData.equip_pos);
                heroDisplay.LoadWeaponModel(cSVEquipmentData, false);
            }
            m_UpdateVirturlWeaponAnimation = updateVirturlWeaponAnimation;
        }

        public void LoadModelParts(uint id, EHeroModelParts eHeroModelParts)
        {
            if (eHeroModelParts == EHeroModelParts.Main)
            {
                uint clothesfashionModelId = id * 10000 + heroId;
                CSVFashionModel.Data cSVFashionModelData = CSVFashionModel.Instance.GetConfData(clothesfashionModelId);
                if (cSVFashionModelData != null)
                {
                    string modelPath = _highModel ? cSVFashionModelData.model_show : cSVFashionModelData.model;
                    heroDisplay.LoadMainModel(EHeroModelParts.Main, modelPath, EHeroModelParts.None, null, true);
                }
                showParts[(uint)eHeroModelParts] = id;

                VirtualGameObject part = heroDisplay.GetPart(eHeroModelParts);
                part.SetParent(_parent, null);
                if (part.eState == VirtualGameObject.EState.Loaded)
                {
                    OnShowModelLoaded((int)eHeroModelParts);
                }
            }
            else if (eHeroModelParts == EHeroModelParts.Jewelry_Back
                || eHeroModelParts == EHeroModelParts.Jewelry_Face
                || eHeroModelParts == EHeroModelParts.Jewelry_Head
                || eHeroModelParts == EHeroModelParts.Jewelry_Waist)
            {
                CSVFashionAccessory.Data cSVFashionAccessoryData = CSVFashionAccessory.Instance.GetConfData(id);
                if (cSVFashionAccessoryData != null)
                {
                    string modelPath = _highModel ? cSVFashionAccessoryData.model_show : cSVFashionAccessoryData.model;
                    string socketName = cSVFashionAccessoryData.AccSlot;
                    heroDisplay.LoadMainModel(eHeroModelParts, modelPath, EHeroModelParts.Main, socketName);
                    showParts[(uint)eHeroModelParts] = id;

                    VirtualGameObject part = heroDisplay.GetPart(eHeroModelParts);
                    if (part.eState == VirtualGameObject.EState.Loaded)
                    {
                        OnShowModelLoaded((int)eHeroModelParts);
                    }
                }
                else
                {
                    heroDisplay.LoadMainModel(eHeroModelParts, null, EHeroModelParts.Main, null);
                }
            }
        }

        public void UnloadModelParts(EHeroModelParts eHeroModelParts)
        {
            heroDisplay.LoadMainModel(eHeroModelParts, null, EHeroModelParts.Main, null);
        }

        public void SetParent(Transform parent)
        {
            if (parent == null)
            {
                if (_parent != null)
                {
                    _parent.SetGameObject(null, true);
                }
                return;
            }

            if (_parent == null)
            {
                _parent = new VirtualGameObject();
            }
            _parent.SetGameObject(parent.gameObject, true);

            VirtualGameObject main = heroDisplay.GetPart(EHeroModelParts.Main);
            if (main != null)
            {
                main.SetParent(_parent, null);
            }
        }

        public void OnShowModelLoaded(int part)
        {
            if (part < 0)
                return;

            uint id = showParts[part];
            EHeroModelParts eHeroModelParts = (EHeroModelParts)part;

            if (datas.TryGetValue(id, out List<dressData> dressData) && dressData != null)
            {
                for (int i = 0; i < dressData.Count; i++)
                {
                    SetColor(eHeroModelParts, dressData[i].tintIndex, dressData[i].color);
                }
            }
            if (eHeroModelParts == EHeroModelParts.Main)
            {
                VirtualGameObject vgo = heroDisplay.GetPart(eHeroModelParts);
                if (vgo.eState == VirtualGameObject.EState.Failed)
                {
                    DebugUtil.LogErrorFormat("主体加载失败 {0}", vgo.sAssetPath);
                }
                action?.Invoke(vgo.gameObject);
                action = null;
            }
            if (eHeroModelParts == EHeroModelParts.Weapon)
            {
                VirtualGameObject vgo = heroDisplay.GetPart(eHeroModelParts);
                if (vgo.eState == VirtualGameObject.EState.Failed)
                {
                    DebugUtil.LogErrorFormat("武器加载失败 {0}", vgo.sAssetPath);
                }
                m_UpdateVirturlWeaponAnimation?.Invoke();
                m_UpdateVirturlWeaponAnimation = null;
            }
        }

        private void SetColor(EHeroModelParts eHeroModelParts, ETintIndex eTintIndex, Color color)
        {
            heroDisplay.SetColor(eHeroModelParts, eTintIndex, color);
        }

        public void Dispose()
        {

            HeroDisplayControl.Destory(ref heroDisplay);
            action = null;
            for (int i = 0; i < showParts.Length; ++i)
            {
                showParts[i] = 0;
            }
        }
    }
}


