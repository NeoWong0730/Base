using System.Collections.Generic;
using UnityEngine;
using Logic.Core;
using Lib.Core;
using Table;
using System;
using UnityEngine.UI;
namespace Logic
{
    public class UI_Pet_MountContractParam
    {
        public int index;
        public ClientPet currentpet;
    }
    public class UI_Pet_MountPetInfo : UIComponent
    {
        private Text petNameText;
        private Text petGearsText;
        private PetItem01 petitem;

        private Transform skillTrans;
        private Transform remakeTrans;
        private Transform demonSpiritTrans;

        private GameObject skillNoneGo;
        private GameObject remekeNoneGo;
        private GameObject demonSpiritNoneGo;

        //private InfinityGrid skillInfinityGrid;
        //private InfinityGrid remakeInfinityGrid;
        protected override void Loaded()
        {
            petitem = new PetItem01();
            petitem.Bind(transform.Find("IconRoot/PetItem01"));
            petNameText = transform.Find("IconRoot/Text_Name").GetComponent<Text>();
            petGearsText = transform.Find("IconRoot/Gears/Text_Value").GetComponent<Text>();

            skillTrans = transform.Find("Scroll_View/GameObject/View_Skill/SkillGroup");
            skillNoneGo = transform.Find("Scroll_View/GameObject/View_Skill/None").gameObject;
            remakeTrans = transform.Find("Scroll_View/GameObject/View_RemouldSkill/SkillGroup");
            remekeNoneGo = transform.Find("Scroll_View/GameObject/View_RemouldSkill/None").gameObject;

            demonSpiritTrans = transform.Find("Scroll_View/GameObject/View_DemonSkill/SkillGroup");
            demonSpiritNoneGo = transform.Find("Scroll_View/GameObject/View_DemonSkill/None").gameObject;

            /*skillInfinityGrid = transform.Find("Scroll_View/GameObject").GetComponent<InfinityGrid>();
            skillInfinityGrid.onCreateCell += OnSkillCreateCell;
            skillInfinityGrid.onCellChange += OnSkillCellChange;*/

            /*remakeInfinityGrid = transform.Find("Scroll_View/GameObject").GetComponent<InfinityGrid>();
            remakeInfinityGrid.onCreateCell += OnRemakeCreateCell;
            remakeInfinityGrid.onCellChange += OnRemakeCellChange;*/
        }


        private List<uint> skillList = new List<uint>();
        private List<uint> remakeList = new List<uint>();
        private List<uint> demonSpiritList = new List<uint>();
        private ClientPet clientPet;

        public void SetPetInfo(ClientPet clientPet)
        {
            if(null != clientPet)
            {
                this.clientPet = clientPet;
                petitem.SetData(clientPet);
                petNameText.text = clientPet.GetPetNmae();
                uint lowG = clientPet.GetPetMaxGradeCount() - clientPet.GetPetGradeCount();
                bool isMax = lowG == 0;
                if (isMax)
                {
                    TextHelper.SetText(petGearsText, LanguageHelper.GetTextContent(11713, clientPet.GetPetCurrentGradeCount().ToString(), clientPet.GetPetBuildMaxGradeCount().ToString()));
                }
                else
                {
                    TextHelper.SetText(petGearsText, LanguageHelper.GetTextContent(11712, clientPet.GetPetCurrentGradeCount().ToString(), clientPet.GetPetBuildMaxGradeCount().ToString(), lowG.ToString()));
                }

                remakeList.Clear();
                remakeList.AddRange(clientPet.petUnit.BuildInfo.BuildSkills);

                int count = remakeList.Count;
                bool hasSkill = count > 0;
                remakeTrans.gameObject.SetActive(hasSkill);
                remekeNoneGo.SetActive(!hasSkill);
                
                if (hasSkill)
                {
                    FrameworkTool.CreateChildList(remakeTrans, remakeList.Count);
                    for (int i = 0; i < count; i++)
                    {
                        PetSkillCeil entry = new PetSkillCeil();
                        entry.BingGameObject(remakeTrans.GetChild(i).gameObject);
                        entry.AddClickListener(OnSkillSelect);
                        var skillId = remakeList[i];
                        entry.SetData(skillId, false, true, index: i, hasHight: skillId == 0 ? false : clientPet.IsHasHighBuildSkill(skillId));
                    }
                }
                
                //remakeInfinityGrid.CellCount = remakeList.Count;
                //remakeInfinityGrid.ForceRefreshActiveCell();

                skillList.Clear();
                skillList = clientPet.GetPetSkillList();

                count = skillList.Count;
                hasSkill = count > 0;
                skillTrans.gameObject.SetActive(hasSkill);
                skillNoneGo.SetActive(!hasSkill);
                
                if (hasSkill)
                {
                    FrameworkTool.CreateChildList(skillTrans, skillList.Count);
                    for (int i = 0; i < count; i++)
                    {
                        PetSkillCeil entry = new PetSkillCeil();
                        entry.BingGameObject(skillTrans.GetChild(i).gameObject);
                        entry.AddClickListener(OnSkillSelect);
                        var skillId = skillList[i];
                        //
                        entry.SetData(skillId, false, false, isDetaiBasic:true, hasHight: skillId == 0 ? false : clientPet.IsHasHighBuildSkill(skillId), showLevel: false);
                    }
                }

                demonSpiritList.Clear();
                demonSpiritList = clientPet.GetDemonSpiritSkills();

                count = demonSpiritList.Count;
                hasSkill = count > 0;
                demonSpiritTrans.gameObject.SetActive(hasSkill);
                demonSpiritNoneGo.SetActive(!hasSkill);

                if (hasSkill)
                {
                    FrameworkTool.CreateChildList(demonSpiritTrans, demonSpiritList.Count);
                    for (int i = 0; i < count; i++)
                    {
                        PetSkillCeil entry = new PetSkillCeil();
                        entry.BingGameObject(demonSpiritTrans.GetChild(i).gameObject);
                        entry.AddClickListener(OnSkillSelect);
                        var skillId = demonSpiritList[i];
                        //
                        entry.SetData(skillId, false, false, isDetaiBasic: true, hasHight: skillId == 0 ? false : clientPet.IsHasHighBuildSkill(skillId), showLevel: false);
                    }
                }

                //skillInfinityGrid.CellCount = skillList.Count;
                //skillInfinityGrid.ForceRefreshActiveCell();
                if (null != transform)
                {
                    //强刷刷新布局：处理嵌套layout 自动布局与content size fitter 时，布局刷新失败。后续有更好方法可以再进行优化
                    if (transform != null)
                    {
                        FrameworkTool.ForceRebuildLayout(transform.gameObject);
                    }
                }
            }
        }


        private void OnSkillSelect(PetSkillCeil petSkillCeil)
        {
            uint skillId = petSkillCeil.petSkillBase.skillId;
            if (skillId != 0)
            {
                UIManager.OpenUI(EUIID.UI_Skill_Tips, false, new Tuple<uint, uint>(skillId, 0));
            }
        }
    }

    public class UI_Pet_MountContract_Layout
    {
        private Button closeBtn;
        private Button cancelBtn;
        private Button changeBtn;
        private UI_Pet_MountPetInfo mountPetInfo;
        public void Init(Transform transform)
        {
            closeBtn = transform.Find("Blank").GetComponent<Button>();
            cancelBtn =  transform.Find("Animator/Tips01/Background_Root/View_Button/Btn_Cancel").GetComponent<Button>();
            changeBtn = transform.Find("Animator/Tips01/Background_Root/View_Button/Btn_Change").GetComponent<Button>();
            mountPetInfo = new UI_Pet_MountPetInfo();
            mountPetInfo.Init(transform.Find("Animator/Tips01/Background_Root"));
        }

        public void RefreshView(ClientPet clientPet)
        {
            mountPetInfo.SetPetInfo(clientPet);
        }

        public void RegisterEvents(IListener listener)
        {
            closeBtn.onClick.AddListener(listener.CloseBtnClicked);
            cancelBtn.onClick.AddListener(listener.OnCancelBtnClicked);
            changeBtn.onClick.AddListener(listener.OnChangeBtnClicked);
        }

        public interface IListener
        {
            void CloseBtnClicked();
            void OnCancelBtnClicked();
            void OnChangeBtnClicked();
        }
    }

    public class UI_Pet_MountContract : UIBase, UI_Pet_MountContract_Layout.IListener
    {
        private UI_Pet_MountContract_Layout layout = new UI_Pet_MountContract_Layout();
        private UI_Pet_MountContractParam paran;
        protected override void OnLoaded()
        {
            layout.Init(transform);
            layout.RegisterEvents(this);
        }

        protected override void OnOpen(object arg = null)
        {
            if (null != arg)
                paran = arg as UI_Pet_MountContractParam;
        }
        

        protected override void OnShow()
        {
            RefreshView();
        }

        private void RefreshView()
        {
            if(null != paran)
                layout.RefreshView(paran.currentpet);
        }

        protected override void OnHide()
        {
        }

        protected override void OnDestroy()
        {
        }

        public void CloseBtnClicked()
        {
            UIManager.CloseUI(EUIID.UI_Pet_MountContract);
        }

        public void OnCancelBtnClicked()
        {
            Sys_Pet.Instance.OnPetContractCancleReq(paran.currentpet.petUnit.SimpleInfo.ContractPetUid, new List<uint>() { (uint)paran.index});
            CloseBtnClicked();
        }

        public void OnChangeBtnClicked()
        {
            UI_Pet_MountSelectItemParam param = new UI_Pet_MountSelectItemParam();
            param.index = paran.index;
            param.petUid = paran.currentpet.petUnit.SimpleInfo.ContractPetUid;
            UIManager.OpenUI(EUIID.UI_Pet_MountSelectItem, false, param);
            CloseBtnClicked();
        }
    }
}