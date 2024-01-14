using Framework;
using Lib.Core;
using Logic.Core;
using System;
using System.Collections.Generic;
using Table;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Logic
{
   
    public class UI_Lotto_PetGet : UIComponent, UI_Lotto_PetGet_Layout.IListener
    {
        //缓存数据
        private UI_Lotto_PetGet_Layout layout = new UI_Lotto_PetGet_Layout();
     
       // private List<PetSkillCeil> skillGrids = new List<PetSkillCeil>();

        private List<uint> skillIdList = new List<uint>();



        private uint m_PetID = 0;

        protected override void Loaded()
        {
            base.Loaded();

            layout.Init(transform);
            layout.RegisterEvents(this);

       
            //SetSkillItem();
        }

        public override void Show()
        {
            if (gameObject.activeSelf)
                return;

            base.Show();

            layout.ShowPetModel(m_PetID);

            layout.SetPetAttr(m_PetID);

            var data = CSVPetNew.Instance.GetConfData(m_PetID);
            layout.SetPetInfo(data);

            layout.SetPetSkill(m_PetID);

            layout.closeBtn.enabled = false;


        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Lib.Core.EventTrigger eventListener = Lib.Core.EventTrigger.Get(layout.eventImage);
            //eventListener.AddEventListener(EventTriggerType.Drag, OnDrag);
        }

        public override void Hide()
        {
            if (gameObject.activeSelf == false)
                return;

            base.Hide();
           // timeclose.Cancel();
            OnDestroyModel();

            m_PetID = 0;
        }

        public void SetPetID(uint id)
        {
            m_PetID = id;
        }

        #region 模型展示代码

        private void OnDestroyModel()
        {
            layout._UnloadShowContent();
        }



        //public void OnDrag(BaseEventData eventData)
        //{
        //    PointerEventData ped = eventData as PointerEventData;
        //    Vector3 angle = new Vector3(0f, ped.delta.x * -0.36f, 0f);
        //    AddEulerAngles(angle);
        //}

        //public void AddEulerAngles(Vector3 angle)
        //{
        //    if (showSceneControl.mModelPos.transform != null)
        //    {
        //        Vector3 localAngle = showSceneControl.mModelPos.transform.localEulerAngles;
        //        //localAngle.Set(localAngle.x + angle.x, localAngle.y + angle.y, localAngle.z + angle.z);
        //        Vector3 newAngle = new Vector3(localAngle.x + angle.x, localAngle.y + angle.y, localAngle.z + angle.z);
        //        showSceneControl.mModelPos.transform.localEulerAngles = newAngle;
        //    }
        //}

        #endregion

        #region Function

        //private void Init()
        //{
        //    layout.petname.text = LanguageHelper.GetTextContent(CSVPet.Instance.GetConfData(clientpet.petUnit.PetId).name);
        //    layout.grade.text = clientpet.petUnit.Power.ToString();
        //    layout.level.text = LanguageHelper.GetTextContent(2009330, clientpet.petUnit.Level);
        //    layout.rare.gameObject.SetActive(clientpet.petUnit.Rare);
        //    layout.grownum.text = ((float)clientpet.petUnit.Growth / 1000).ToString("0.000");
        //    AddEleAttr();
        //}

        //private void AddEleAttr()
        //{
        //    int needCount = clientpet.eleAttrs.Count;
        //    FrameworkTool.CreateChildList(layout.eleicon.transform.parent, needCount);
        //    for (int i = 0; i < needCount; i++)
        //    {
        //        GameObject go = layout.eleicon.transform.parent.transform.GetChild(i).gameObject;
        //        uint id = Sys_Pet.Instance.pkAttrs2Id[clientpet.eleAttrs[i]];
        //        ImageHelper.SetIcon(go.transform.Find("Image_Attr").GetComponent<Image>(), CSVAttr.Instance.GetConfData(id).attr_icon);
        //        go.transform.Find("Image_Attr/Text").GetComponent<Text>().text = clientpet.pkAttrs[clientpet.eleAttrs[i]].ToString();
        //    }
        //}


        //private void UpdateChildrenCallback(int index, Transform trans)
        //{
        //    if (index < 0 || index >= skillIdList.Count)
        //        return;
        //    if (skillCeilGrids.ContainsKey(trans.gameObject))
        //    {
        //        PetSkillCeil petSkillCeil = skillCeilGrids[trans.gameObject];
        //        petSkillCeil.SetDate(skillIdList[index], clientpet.petUnit.LockSkill.Contains(skillIdList[index]), skillIdList[index] == clientpet.petUnit.RareSkill);
        //    }
        //}

        //private void SetSkillItem()
        //{

        //    skillIdList.Clear();


        //    for (int i = 0; i < clientpet.petUnit.SkillList.Count; ++i)
        //    {
        //        skillIdList.Add(clientpet.petUnit.SkillList[i]);
        //    }
        //    skillIdList.Sort((var1, var2) => { return clientpet.petUnit.LockSkill.Contains(var1) ? -1 : 1; });
        //    if (clientpet.petUnit.RareSkill != 0)
        //    {
        //        skillIdList.Add(clientpet.petUnit.RareSkill);
        //    }

        //}

        private void OnSkillSelect(PetSkillCeil petSkillCeil)
        {
            // UIManager.OpenUI(EUIID.UI_Skill_Tips, false, petSkillCeil.petSkillBase.skillId);
        }


        #endregion

        #region ButtonClick
        public void OncloseBtnClicked()
        {
            
        }

        public void OnLoadPetModelOver(int part)
        {
            layout.OnShowModelLoaded(part, m_PetID);
        }

        public void OnSkillItemUpdate(int index)
        {

        }
        #endregion
    }
}
