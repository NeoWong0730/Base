
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Packet;
using Lib.Core;
using Logic.Core;

namespace Logic
{
    public class UI_Tips_Pet_Skill
    {
        private class SkillInfo
        {
            public uint skillId;
            public bool uniq;
            public bool build;
        }

        private class SkillCell
        {
            private Transform transform;

            private PetSkillItem02 _skillItem;

            private SkillInfo _skillInfo;
            public void Init(Transform trans)
            {
                transform = trans;

                _skillItem = new PetSkillItem02();
                _skillItem.Bind(transform.gameObject);
                _skillItem.skillImage.gameObject.SetActive(true);
                //_skillItem.AddClickListener(OnClick);
            }

            public void UpdateInfo(SkillInfo skillInfo)
            {
                _skillInfo = skillInfo;

                _skillItem.SetDate(_skillInfo.skillId);
                _skillItem.uniqueGo.gameObject.SetActive(_skillInfo.uniq);
                _skillItem.buildGo.gameObject.SetActive(_skillInfo.build);
            }

            //private void OnClick()
            //{
            //    UIManager.OpenUI(EUIID.UI_Skill_Tips, false, _skillInfo.skillId);
            //}
        }

        private Transform transform;
        private Transform _grid;
        private InfinityGridLayoutGroup infinity;
        private Dictionary<GameObject, SkillCell> skillCeilGrids = new Dictionary<GameObject, SkillCell>();
        //private List<PetSkillCeil> skillGrids = new List<PetSkillCeil>();
        private List<SkillInfo> _skillList = new List<SkillInfo>();
        private ClientPet _clientPet;
        public void Init(Transform trans)
        {
            transform = trans;
            _grid = trans.Find("Grid");
            infinity = _grid.gameObject.GetNeedComponent<InfinityGridLayoutGroup>();
            infinity.minAmount = 8;
            infinity.updateChildrenCallback = UpdateChildrenCallback;
            SetSkillItem();
        }

        public void Show()
        {
            transform.gameObject.SetActive(true);
        }

        public void Hide()
        {
            transform.gameObject.SetActive(false);
        }

        public void SetData(PetUnit pet)
        {
            _clientPet = new ClientPet(pet);
            _skillList.Clear();
            if (_clientPet != null)
            {
                List<uint> ids = _clientPet.GetPetAllSkillList();                for (int i = 0; i < ids.Count; ++i)
                {
                    SkillInfo info = new SkillInfo();
                    info.skillId = ids[i];
                    info.uniq = _clientPet.IsUniqueSkill(ids[i]);
                    info.build = _clientPet.IsBuildSkill(ids[i]);

                    _skillList.Add(info);
                }
            }

            infinity.SetAmount(_skillList.Count);
        }

        private void SetSkillItem()
        {
            for (int i = 0; i < _grid.childCount; i++)
            {
                GameObject go = _grid.GetChild(i).gameObject;

                SkillCell cell = new SkillCell();
                cell.Init(_grid.GetChild(i));

                skillCeilGrids.Add(go, cell);
            }
        }       

        private void UpdateChildrenCallback(int index, Transform trans)
        {
            if (index < 0 || index >= _skillList.Count)
                return;
            if (skillCeilGrids.ContainsKey(trans.gameObject))
            {
                SkillCell cell = skillCeilGrids[trans.gameObject];
                cell.UpdateInfo(_skillList[index]);
            }
        }

        //private void OnSkillSelect(PetSkillCeil petSkillCeil)
        //{
        //    UIManager.OpenUI(EUIID.UI_Skill_Tips, false, petSkillCeil.petSkillBase.skillId);
        //}
    }
}
