using Table;
using System.Collections.Generic;
using UnityEngine;

namespace Logic
{
    public class WarriorGroupModelShow
    {
        public VirtualGameObject Parent { get; set; }

        public ulong UID { get; set; }

        public bool IsUsed { get; set; } = false;

        public uint ModelID { get; set; }

        public ulong RoleID { get; set; }

        public uint DressID { get; set; } = 0;

        public uint WeaponID { get; set; }

        public HeroLoader heroLoader;

        public void LoadModel(uint heroID, uint occ, uint weaponID, uint dressID, Dictionary<uint, List<dressData>> DressValue)
        {
            ModelID = heroID;
            DressID = dressID;
            heroLoader = HeroLoader.Create(true);
            heroLoader.LoadHero(heroID, weaponID, ELayerMask.ModelShow, DressValue, o =>
            {
                heroLoader.heroDisplay.GetPart(EHeroModelParts.Main).SetParent(Parent, null);
            });

            heroLoader.heroDisplay.onLoaded += DisplayControlLoaded;
        }

        void DisplayControlLoaded(int intValue)
        {
            if (heroLoader == null || heroLoader.heroDisplay.bMainPartFinished == false)
                return;

            CSVCharacter.Data cSVCharacterData = CSVCharacter.Instance.GetConfData(ModelID);

            if (cSVCharacterData == null)
                return;

            if (DressID == 0)
                return;

            //uint dressID = Sys_Fashion.Instance.GetDressedId(EHeroModelParts.Main);
            uint id = (uint)(DressID * 10000 + ModelID);
            CSVFashionModel.Data cSVFashionModelData = CSVFashionModel.Instance.GetConfData(id);
            GameObject go = heroLoader.heroDisplay?.GetPart(EHeroModelParts.Main).gameObject;
            go?.SetActive(false);
            heroLoader.heroDisplay?.mAnimation.UpdateHoldingAnimations(cSVFashionModelData.action_show_id, WeaponID == 0 ? cSVCharacterData.show_weapon_id : WeaponID, Constants.IdleAndRunAnimationClipHashSet, EStateType.Idle, go);
        }

        public void ChangeWeapon(uint weaponId)
        {
            heroLoader.LoadWeaponPart(heroLoader.showParts[(int)EHeroModelParts.Weapon], weaponId);
        }

        public void SetParent(VirtualGameObject parent)
        {
            if (Parent == parent)
                return;

            Parent = parent;

            if (heroLoader == null || heroLoader.heroDisplay == null)
                return;

            var value = heroLoader.heroDisplay.GetPart(EHeroModelParts.Main);

            if (value == null)
                return;

            value.SetParent(Parent, null);
        }

        public void SetActive(bool active)
        {
            if (heroLoader == null || heroLoader.heroDisplay == null)
                return;

            var value = heroLoader.heroDisplay.GetPart(EHeroModelParts.Main);

            if (value == null)
                return;

            if (value.gameObject == null)
                return;

            if (value.gameObject.activeSelf != active)
            {
                value.gameObject.SetActive(active);
                heroLoader.heroDisplay?.mAnimation.Play((uint)EStateType.Idle);
            }
        }

        public void Dispose()
        {
            Parent = null;
            heroLoader?.Dispose();
            heroLoader = null;
        }
    }
}
