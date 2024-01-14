using UnityEngine;
using Logic.Core;
using UnityEngine.UI;
using Table;
using System.Collections.Generic;
using Lib.Core;
using System;
using System.Json;

namespace Logic
{
    public class UI_Uplifted_Item : UIComponent
    {
        private uint Id;
        private CSVUplifted.Data csvdate;
        private ClientPet clientpet;
        private ClientPet fightClientPet;

        private Text title;
        private Text message;
        private Image icon;
        private Button goBtn;
        private Button closeBtn;

        private uint breakthroughLv;
        private uint maxLv;

        public UI_Uplifted_Item(uint _id) : base()
        {
            Id = _id;
        }

        protected override void Loaded()
        {
            title = transform.Find("Text_Titel").GetComponent<Text>();
            message = transform.Find("Text_Description").GetComponent<Text>();
            icon = transform.Find("Icon").GetComponent<Image>();
            goBtn = transform.Find("Btn_04").GetComponent<Button>();
            goBtn.onClick.AddListener(OngoBtnClicked);
            closeBtn = transform.Find("Btn_Close").GetComponent<Button>();
            closeBtn.onClick.AddListener(OncloseBtnClicked);
            ProcessEvents(true);
        }

        public  void ProcessEvents(bool toRegister)
        {
            Sys_Family.Instance.eventEmitter.Handle(Sys_Family.EEvents.UpdateFamilyInfo, OnUpdateFamilyInfo, toRegister);
        }

        private void OnUpdateFamilyInfo()
        {
            uint skillId = Sys_Family.Instance.familyData.GetSkillId(Sys_Family.FamilyData.FamilySkillType.BreakthroughTraining);
            if (skillId == 0)
            {
                breakthroughLv = 0;
            }
            else
            {
                breakthroughLv = CSVFamilySkillUp.Instance.GetConfData(skillId).SkillLevel;
            }
            maxLv = Sys_Experience.Instance.GetMaxLv(breakthroughLv);

             if (csvdate.Type == 7) //Family Experience Level
            {
                if (!CSVExperienceLevel.Instance.ContainsKey(Sys_Experience.Instance.exPerienceLevel + 1))
                {
                    gameObject.SetActive(false);
                    return;
                }
                CSVExperienceLevel.Data data = CSVExperienceLevel.Instance.GetConfData(Sys_Experience.Instance.exPerienceLevel + 1);
                uint costId01 = data.cost[0][0];
                uint costCount01 = data.cost[0][1];
                uint costId02 = data.cost[1][0];
                uint costCount02 = data.cost[1][1];

                if (Sys_Experience.Instance.exPerienceLevel < maxLv && Sys_Bag.Instance.GetItemCount(costId01) >= costCount01 && Sys_Bag.Instance.GetItemCount(costId02) >= costCount02)
                {
                    gameObject.SetActive(!SetItemIsClose());
                }
            }
        }

        private void OngoBtnClicked()
        {         
            uint type = csvdate.Type;
            switch (type)
            {
                case 1:  //SkillUplift
                    int id;
                    Sys_Skill.Instance.ExistedUpgradeRank(out id);
                    UIManager.OpenUI(EUIID.UI_SkillUpgrade, false, new List<int> { 0, id });
                    break;
                case 2: // RoleAddPoint
                    UIManager.OpenUI(EUIID.UI_Attribute, false, 2);
                    break;
                case 3://PetAddPoint
                    UIManager.OpenUI(EUIID.UI_Pet_AddPoint, false, clientpet);
                    break;
                case 4://HpPool
                    UIManager.OpenUI(EUIID.UI_Attribute, false, 6);
                    break;
                case 5://MpPool
                    UIManager.OpenUI(EUIID.UI_Attribute, false, 6);
                    break;
                case 6://Energe
                    UIManager.OpenUI(EUIID.UI_Vitality);
                    break;
                case 7:  
                    Sys_Experience.Instance.InfoReq();
                    if (Sys_Family.Instance.familyData.isInFamily)
                    {
                        Sys_Family.Instance.SendGuildGetGuildInfoReq();
                    }
                    UIManager.OpenUI(EUIID.UI_Family_Empowerment);
                    UIManager.OpenUI(EUIID.UI_Family_DeedsLv_Popup);
                    break;
                case 8:
                    Sys_Experience.Instance.InfoReq();
                    UIManager.OpenUI(EUIID.UI_Family_Empowerment);
                    break;
                case 9:
                    int id2;
                    Sys_Skill.Instance.ExistedUpgradeLevel(out id2);
                    UIManager.OpenUI(EUIID.UI_SkillUpgrade, false, new List<int> { 0, id2 });
                    break;
                case 10:
                    Sys_LivingSkill.Instance.SkipToLivingSkillForLevelUp();
                    break;
                default:
                    break;
            }
            UIManager.CloseUI(EUIID.UI_Uplifted);

        }

        private void OncloseBtnClicked()
        {
            gameObject.SetActive(false);
            if (Sys_Uplifted.Instance.upliftedClosedDic.TryGetValue(Id,out UpliftOpenClose value)&& value!=null)
            {
                value.closeTime = Sys_Time.Instance.GetServerTime();
                value.isClose = true;
            }
            else
            {
                UpliftOpenClose info = new UpliftOpenClose();
                info.closeTime = Sys_Time.Instance.GetServerTime();
                info.isClose = true;
                info.upliftId = Id;
                Sys_Uplifted.Instance.upliftedClosedDic.Add(Id, info);
            }
            foreach (UpliftOpenClose uplift in Sys_Uplifted.Instance.upliftedClosedDic.Values)
            {
               Sys_Uplifted.Instance.Info.upliftedInfosList.Add(uplift);
            }
            FileStore.WriteJson("Uplifts.txt", Sys_Uplifted.Instance.Info);
            Sys_Uplifted.Instance.eventEmitter.Trigger(Sys_Uplifted.EEvents.OnCloseUpliftItem);
        }


        public void RefreshView()
        {
            csvdate = CSVUplifted.Instance.GetConfData(Id);
            title.text = LanguageHelper.GetTextContent(csvdate.Name_id);
            message.text = LanguageHelper.GetTextContent(csvdate.Describe_id);
            ImageHelper.SetIcon(icon,csvdate.Icon_id);
            SetUpliftData(csvdate.Type);
        }

        private void SetUpliftData(uint type)
        {
            switch (type)
            {
                case 1://SkillUplift
                    int id;
                    if (Sys_Skill.Instance.ExistedUpgradeRank(out id))
                    {
                        gameObject.SetActive(!SetItemIsClose());
                    }
                    break;
                case 2:// RoleAddPoint
                    if (Sys_Attr.Instance.surplusPoint >= csvdate.Parameter)
                    {
                        gameObject.SetActive(!SetItemIsClose());
                    }
                    break;
                case 3: //PetAddPoint
                    for (int i = 0; i < Sys_Pet.Instance.petsList.Count; ++i)
                    {
                        if (Sys_Pet.Instance.petsList[i].baseAttrs[EBaseAttr.SurplusPoint] >= csvdate.Parameter)
                        {
                            clientpet = Sys_Pet.Instance.petsList[i];
                            break;
                        }
                    }

                    if (Sys_Pet.Instance.fightPet != null)
                    {
                        fightClientPet = Sys_Pet.Instance.GetFightPetClient(Sys_Pet.Instance.fightPet.GetUid());
                        if (fightClientPet!=null&& fightClientPet.baseAttrs[EBaseAttr.SurplusPoint] >= csvdate.Parameter)
                        {
                            clientpet = fightClientPet;
                            gameObject.SetActive(!SetItemIsClose());
                        }
                        else
                        {
                            if (clientpet != null)
                            {
                                gameObject.SetActive(!SetItemIsClose());
                            }
                        }
                    }
                    else
                    {
                        if (clientpet != null)
                        {
                            gameObject.SetActive(!SetItemIsClose());
                        }
                    }
                    break;
                case 4: //HpPool
                    if (Sys_Attr.Instance.hpPool <= csvdate.Parameter)
                    {
                        gameObject.SetActive(!SetItemIsClose());
                    }
                    break;
                case 5: //MpPool
                    if (Sys_Attr.Instance.mpPool <= csvdate.Parameter)
                    {
                        gameObject.SetActive(!SetItemIsClose());
                    }
                    break;
                case 6: //Energe
                    uint vitalityMax = Sys_Vitality.Instance.GetMaxVitality(); 
                    if (Sys_Bag.Instance.GetItemCount(5) / (float)vitalityMax * 100 >= csvdate.Parameter)
                    {
                        gameObject.SetActive(!SetItemIsClose());
                    }
                    break;
                case 7: //Family Experience Level
                    if (Sys_Family.Instance.familyData.isInFamily)
                    {
                        Sys_Family.Instance.SendGuildGetGuildInfoReq();
                    }
                    else
                    {
                        maxLv = Sys_Experience.Instance.GetMaxLv(0);
                        if (!CSVExperienceLevel.Instance.ContainsKey(Sys_Experience.Instance.exPerienceLevel + 1))
                        {
                            gameObject.SetActive(false);
                            return;
                        }
                        CSVExperienceLevel.Data data = CSVExperienceLevel.Instance.GetConfData(Sys_Experience.Instance.exPerienceLevel + 1);
                        uint costId01 = data.cost[0][0];
                        uint costCount01 = data.cost[0][1];
                        uint costId02 = data.cost[1][0];
                        uint costCount02 = data.cost[1][1];

                        if (Sys_Experience.Instance.exPerienceLevel < maxLv && Sys_Bag.Instance.GetItemCount(costId01) >= costCount01 && Sys_Bag.Instance.GetItemCount(costId02) >= costCount02)
                        {
                            gameObject.SetActive(!SetItemIsClose());
                        }

                    }
                    break;
                case 8: //Family Experience Attribute Addpoint
                    if (Sys_Experience.Instance.experiencePlanDatas[0].LeftPoint >= csvdate.Parameter)
                    {
                        gameObject.SetActive(!SetItemIsClose());
                    }
                    break;
                case 9: //Skill Up  Level
                    int id2;
                    if (Sys_Skill.Instance.ExistedUpgradeLevel(out id2))
                    {
                        gameObject.SetActive(!SetItemIsClose());
                    }
                    break;
                case 10: //Living Skill Up Level
                    if (Sys_LivingSkill.Instance.HasLivingSkillLevelUp())
                    {
                        gameObject.SetActive(!SetItemIsClose());
                    }
                    break;
                default:
                    break;
            }
        }

        private bool SetItemIsClose()
        {
            if (Sys_Uplifted.Instance.upliftedClosedDic.ContainsKey(Id))
            {
                uint nowtime = Sys_Time.Instance.GetServerTime();
                uint datatime = Sys_Uplifted.Instance.upliftedClosedDic[Id].closeTime;
         
                bool isSameDay = Sys_Time.IsServerSameDay(nowtime, datatime);
                if (!isSameDay)
                {
                    Sys_Uplifted.Instance.upliftedClosedDic.Remove(Id);
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return false;
            }
        }
    }

    public class UI_Uplifted : UIBase
    {
        public GameObject upItem;
        public List<UI_Uplifted_Item> list = new List<UI_Uplifted_Item>();

        protected override void OnLoaded()
        {
            upItem = transform.Find("Animator/Scroll_View/Viewport/Item").gameObject;
        }

        protected override void OnShow()
        {
            JsonObject json = FileStore.ReadJson("Uplifts.txt");
            if (json != null)
            {
                Sys_Uplifted.Instance.Info.DeserializeObject(json);
            }
            AddList();
        }

        protected override void OnHide()
        {
            DefaultItem();
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {            
            if (toRegister)
            {
                Sys_Input.Instance.onTouchUp += OnTpuchUp;
            }
            else
            {
                Sys_Input.Instance.onTouchUp -= OnTpuchUp;
            }
            Sys_Uplifted.Instance.eventEmitter.Handle(Sys_Uplifted.EEvents.OnCloseUpliftItem, OnCloseUpliftItem, toRegister);
            UIManager.GetStackEventEmitter().Handle<uint, int>(UIStack.EUIStackEvent.BeginEnter, OnBeginEnter, toRegister);
        }

        private void OnCloseUpliftItem( )
        {
            for(int i=0;i< list.Count; ++i)
            {
                if (list[i].gameObject.activeInHierarchy)
                {
                    return;
                }
            }
            UIManager.CloseUI(EUIID.UI_Uplifted);
        }

        private void OnBeginEnter(uint stackID, int nID)
        {
            if (nID != (int)EUIID.UI_Uplifted && nID != (int)EUIID.UI_ForceGuide && nID != (int)EUIID.UI_UnForceGuide)
            {
                UIManager.CloseUI(EUIID.UI_Uplifted);
            }
        }

        private void OnTpuchUp(Vector2 pos)
        {
            UIManager.CloseUI(EUIID.UI_Uplifted);
        }

        private void AddList()
        {
            foreach(var data in CSVUplifted.Instance.GetAll())
            {
                GameObject go = GameObject.Instantiate<GameObject>(upItem, upItem.transform.parent);
                go.SetActive(false);
                go.transform.name = data.id.ToString();
                UI_Uplifted_Item item = new UI_Uplifted_Item(data.id);
                item.Init(go.transform);
                list.Add(item);
                item.RefreshView();
            }
            upItem.SetActive(false);
        }

        private void DefaultItem()
        {
            upItem.SetActive(true);
            for (int i=0;i< list.Count;++i) { list[i].OnDestroy(); }
            for (int i = 0; i < list.Count; ++i) { list[i].ProcessEvents(false); }
            list.Clear(); 
            FrameworkTool.DestroyChildren(upItem.transform.parent.gameObject, upItem.transform.name, upItem.transform.name);
        }
    }
}
