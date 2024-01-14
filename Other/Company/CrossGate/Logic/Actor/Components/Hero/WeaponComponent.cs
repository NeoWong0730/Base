using Table;
using UnityEngine;
using Lib.Core;
using System;
using Logic.Core;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Logic
{
    public interface IWeaponComponent
    {
        WeaponComponent GetWeaponComponent();
    }        

    public class WeaponComponent : Logic.Core.Component
    {
        private SceneActor sceneActor;
        private AsyncOperationHandle<GameObject> mHandle;       
        public Action<uint, uint> ChangeWeaponAction;

        private uint curWeaponId;
        public uint CurWeaponID
        {
            get{
                return curWeaponId;
            }

            set {
                curWeaponId = value;
                if (value == 0)
                {
                    curWeaponId = Constants.UMARMEDID;
                }
            }
        }

        public GameObject weaponObj
        {
            get;
            private set;
        }

        public Action<GameObject> onLoaded;

        protected override void OnConstruct()
        {
            base.OnConstruct();

            sceneActor = actor as SceneActor;
        }

        protected override void OnDispose()
        {
            sceneActor = null;
            weaponObj = null;
            onLoaded = null;
            UnloadWeapon();
            CurWeaponID = Constants.UMARMEDID;

            base.OnDispose();
        }
        
        
        public void UpdateWeapon(uint newWeaponID, bool NeedLoadWeapon = true, bool useHighQualityModel = false)
        {
            if (CurWeaponID != newWeaponID)
            {
                UnloadWeapon();
                uint oldWeaponId = CurWeaponID;
                CurWeaponID = newWeaponID;
                if (NeedLoadWeapon)
                {
                    LoadWeapon(useHighQualityModel);
                }

                ChangeWeaponAction?.Invoke(oldWeaponId, newWeaponID);
            }
        }

        public void UnloadWeapon()
        {
            if (mHandle.IsValid())
                AddressablesUtil.ReleaseInstance(ref mHandle, MHandle_Completed);
            weaponObj = null;
        }

        public void LoadWeapon(bool useHighQualityModel = false)
        {
            CSVEquipment.Data cSVEquipmentData = CSVEquipment.Instance.GetConfData(CurWeaponID);
            ///空手不加载武器///
            if (cSVEquipmentData.equipment_category == (uint)EEquipType.Weapon && cSVEquipmentData.equipment_type != (uint)EWeaponType.Unarmed)
            {
                string finalModelPath = (useHighQualityModel == true) ? cSVEquipmentData.show_model : cSVEquipmentData.model;

                Transform parent = sceneActor.gameObject.FindChildByName(cSVEquipmentData.equip_pos)?.transform;
                AddressablesUtil.InstantiateAsync(ref mHandle, finalModelPath, MHandle_Completed, true, parent);                
            }
        }

        private void MHandle_Completed(AsyncOperationHandle<GameObject> handle)
        {
            if (handle.IsValid() && handle.Status == AsyncOperationStatus.Succeeded)
            {
                weaponObj = handle.Result;
                sceneActor.SetLayer(weaponObj.transform);
                onLoaded?.Invoke(weaponObj);
            }
        }
    }
}
