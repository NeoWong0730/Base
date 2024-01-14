using System.Collections.Generic;
using UnityEngine;
using Logic.Core;
using UnityEngine.UI;
using Table;
using Lib.Core;
using Packet;
using System.Linq;

namespace Logic
{
    public class UI_Pet_Develope_SelecItem_Layout
    {
        public Transform transform;
        public GameObject itemGo;
        public Button closeBtn;

        public void Init(Transform transform)
        {
            this.transform = transform;
            closeBtn = transform.Find("Blank (1)").GetComponent<Button>();
            itemGo = transform.Find("Animator/Scroll_View/Grid/Item").gameObject;
        }

        public void RegisterEvents(IListener listener)
        {
            closeBtn.onClick.AddListener(listener.OncloseBtnClicked);
        }

        public interface IListener
        {
            void OncloseBtnClicked();
        }
    }

    public class UI_Pet_Develope_UseItem : UIComponent
    {
        public Text itemname;
        public Image itemicon;
        public Text itemmessage;
        public Text itemcount;
        public Button itemBtn;

        private uint id;
        private uint changePetExp;
        private PetUnit petUnit;

        public UI_Pet_Develope_UseItem(uint _id, PetUnit _petUnit) : base()
        {
            id = _id;
            petUnit = _petUnit;
        }

        protected override void Loaded()
        {
            itemicon = transform.Find("PropItem/Btn_Item/Image_Icon").GetComponent<Image>();
            itemname = transform.Find("Text_Name").GetComponent<Text>();
            itemmessage = transform.Find("Text").GetComponent<Text>();
            itemcount = transform.Find("PropItem/Text_Number").GetComponent<Text>();
            itemBtn = transform.GetComponent<Button>();
            itemBtn.onClick.AddListener(OnitemBtnClicked);
        }

        public void RefreshItem()
        {
            if (id != 0)
            {
                itemname.text = LanguageHelper.GetTextContent(CSVItem.Instance.GetConfData(id).name_id);
                itemmessage.text = LanguageHelper.GetTextContent(CSVItem.Instance.GetConfData(id).describe_id);
            }
            else
            {
                ImageHelper.SetIcon(itemicon, 10000004);
                itemname.text = LanguageHelper.GetTextContent(10886);
                string[] str = CSVPetNewParam.Instance.GetConfData(22).str_value.Split('|');
                changePetExp = uint.Parse(str[0]);
                itemmessage.text = LanguageHelper.GetTextContent(10887, str[0], str[1]);
                itemcount.text = string.Empty;
            }
        }

        private void OnitemBtnClicked()
        {
            uint manxLv = CSVPetNewEnhance.Instance.GetKeys().Max();
            long maxCount = GetMaxCount(manxLv);
            if (petUnit.EnhancePlansData.Enhancelvl== manxLv&& maxCount==0)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(12378));
                return;
            }
            if (id == 0)
            {
                if (petUnit.SimpleInfo.Exp < changePetExp)
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(6101));
                }
                else if (maxCount == 0)
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(12377));
                }
                else
                {
                    UI_Pet_Usage.UI_Pet_UsageParam param = new UI_Pet_Usage.UI_Pet_UsageParam();
                    param.itemUid = 0;
                    param.isInputCaculate = true;
                    param.action = SelectItem;
                    param.maxCount = maxCount;
                    param.isExpChange = true;
                    UIManager.OpenUI(EUIID.UI_Pet_Usage, false, param);
                }
            }
            else
            {
                if (Sys_Bag.Instance.GetItemCount(id) == 0)
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(4052));
                }
                else if (maxCount==0)
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(12377));
                }
                else
                {
                    UI_Pet_Usage.UI_Pet_UsageParam param = new UI_Pet_Usage.UI_Pet_UsageParam();                 
                    param.itemUid = Sys_Bag.Instance.GetUuidsByItemId(id)[0];
                    param.isInputCaculate = true;
                    param.action = SelectItem;
                    param.maxCount = maxCount;
                    UIManager.OpenUI(EUIID.UI_Pet_Usage, false, param);
                }
            }
        }

        private void SelectItem(long itemNum)
        {
            Sys_Hint.Instance.PushEffectInNextFight();
            Sys_Pet.Instance.OnGroomUseItemReq(petUnit.Uid, id, (uint)itemNum, (uint)GroomType.Enhance);
        }

        private long GetMaxCount(uint maxLv)
        {
            long count = 0;
            uint maxEnHanceExp = 0;
            foreach (var data in CSVPetNewEnhance.Instance.GetAll())
            {
                if (data.pet_lv > petUnit.SimpleInfo.Level)
                {
                    break;
                }
                if (data.id == petUnit.EnhancePlansData.Enhancelvl + 1)
                {
                    maxEnHanceExp = data.exp - petUnit.EnhancePlansData.Enhanceexp;
                }
                else if (data.id > petUnit.EnhancePlansData.Enhancelvl + 1)
                {       
                    maxEnHanceExp += data.exp;
                }
            }
            if (id == 0)
            {
                long.TryParse(CSVPetNewParam.Instance.GetConfData(22).str_value.Split('|')[0], out long perExp);
                long.TryParse(CSVPetNewParam.Instance.GetConfData(22).str_value.Split('|')[1], out long perAddEnExp);
                long maxEnExpCount;
                long maxExpCount;
                maxExpCount = (long)petUnit.SimpleInfo.Exp / perExp;

                if (maxEnHanceExp > perAddEnExp)
                {
                    maxEnExpCount = maxEnHanceExp / perAddEnExp;
                }
                else if(maxEnHanceExp==0)
                {
                    maxEnExpCount = 0;
                }
                else
                {
                    maxEnExpCount = 1;
                }
                count = maxExpCount < maxEnExpCount ? maxExpCount : maxEnExpCount;
            }
            else
            {
                long.TryParse(CSVPetNewParam.Instance.GetConfData(13).str_value.Split('&')[1], out long perAddExp);
                long num = maxEnHanceExp % perAddExp;
                if (num > 0)
                {
                    count = maxEnHanceExp / perAddExp+1;
                }
                else
                {
                    count = maxEnHanceExp / perAddExp;
                } 
            }

            return count;
        }
    }

    public class UI_Pet_Develope_SelecItem : UIBase, UI_Pet_Develope_SelecItem_Layout.IListener
    {
        private PetUnit petUnit;
        private PropItem propItem;
        private UI_Pet_Develope_SelecItem_Layout layout = new UI_Pet_Develope_SelecItem_Layout();
        private List<UI_Pet_Develope_UseItem> itemlist = new List<UI_Pet_Develope_UseItem>();

        protected override void OnLoaded()
        {
            layout.Init(transform);
            layout.RegisterEvents(this);
        }

        protected override void OnOpen(object arg)
        {
            petUnit = (PetUnit)arg;
        }

        protected override void OnShow()
        {
            SetItem();
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_Pet.Instance.eventEmitter.Handle(Sys_Pet.EEvents.OnChangeItemCount, OnChangeItemCount, toRegister);
        }

        private void OnChangeItemCount()
        {
            DefaultItem();
            SetItem();
        }

        protected override void OnHide()
        {
            DefaultItem();
        }

        private void SetItem()
        {
            itemlist.Clear();
            for (int i = 0; i < Sys_Pet.Instance.selectItemItem.Count; ++i)
            {
                GameObject go = GameObject.Instantiate<GameObject>(layout.itemGo, layout.itemGo.transform.parent);
                UI_Pet_Develope_UseItem item = new UI_Pet_Develope_UseItem(Sys_Pet.Instance.selectItemItem[i], petUnit);
                item.Init(go.transform);
                item.RefreshItem();
                itemlist.Add(item);
                if (Sys_Pet.Instance.selectItemItem[i] != 0)
                {
                    propItem = new PropItem();
                    propItem.BindGameObject(go.transform.Find("PropItem").gameObject);
                    propItem.SetData(new MessageBoxEvt(EUIID.UI_Pet_Develope_SelecItem, new PropIconLoader.ShowItemData(Sys_Pet.Instance.selectItemItem[i],
                        1, true, false, false, false, false, true, true,  true,null,false,true)));
                }
            }
            layout.itemGo.SetActive(false);
        }

        private void DefaultItem()
        {
            layout.itemGo.SetActive(true);
            FrameworkTool.DestroyChildren(layout.itemGo.transform.parent.gameObject, layout.itemGo.transform.name);
        }

        public void OncloseBtnClicked()
        {
            UIManager.CloseUI(EUIID.UI_Pet_Develope_SelecItem);
        }
    }
}