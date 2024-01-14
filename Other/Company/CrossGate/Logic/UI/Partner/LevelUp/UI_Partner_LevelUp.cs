using UnityEngine.UI;
using Logic.Core;
using Packet;
using Table;

namespace Logic
{
    public class UI_Partner_LevelUp : UIBase
    {
        //lv
        private Text textLv1;
        private Text textLv2;
        private Slider sliderAdd;
        private Slider sliderExp;
        private Text textExp;

        //cost
        private PropItem propItem;
        private Text textItemName;

        private Button btnMinus;
        private Button btnAdd;
        private Text textUseNum;

        private Text textHaveNum;

        private Button btnConfirm;

        private uint _infoId;
        private long _itemNum;
        private uint _useNum;

        private const uint _ItemId = 201501;

        private Sys_Partner.LevelUpProgress progress;

        private uint levelDiffMax;

        protected override void OnLoaded()
        {            
            Button btnClose = transform.Find("Animator/View_TipsBg01_Small/Btn_Close").GetComponent<Button>();
            btnClose.onClick.AddListener(() =>
            {
                UIManager.CloseUI(EUIID.UI_PartnerLevelUp);
            });

            textLv1 = transform.Find("Animator/View_Lv/Text_Lv01").GetComponent<Text>();
            textLv2 = transform.Find("Animator/View_Lv/Text_Lv02").GetComponent<Text>();
            sliderAdd = transform.Find("Animator/View_Lv/Slider_Add").GetComponent<Slider>();
            sliderExp = transform.Find("Animator/View_Lv/Slider_Exp").GetComponent<Slider>();
            textExp = transform.Find("Animator/View_Lv/Text_Percent").GetComponent<Text>();

            propItem = new PropItem();
            propItem.BindGameObject(transform.Find("Animator/View_Cost/PropItem").gameObject);
            textItemName = transform.Find("Animator/View_Cost/Text_Name").GetComponent<Text>();

            btnMinus = transform.Find("Animator/View_Cost/View_SelectNum/Btn_Min").GetComponent<Button>();
            //btnMinus.onClick.AddListener(OnClickMinus);
            UI_LongPressButton LongPressSubButton = btnMinus.gameObject.AddComponent<UI_LongPressButton>();
            LongPressSubButton.interval = 0.3f;
            LongPressSubButton.bPressAcc = true;
            LongPressSubButton.onRelease.AddListener(OnClickMinus);
            LongPressSubButton.OnPressAcc.AddListener(OnClickMinus);

            btnAdd = transform.Find("Animator/View_Cost/View_SelectNum/Btn_Add").GetComponent<Button>();
            //btnAdd.onClick.AddListener(OnClickAdd);
            UI_LongPressButton LongPressAddButton = btnAdd.gameObject.AddComponent<UI_LongPressButton>();
            LongPressAddButton.interval = 0.3f;
            LongPressAddButton.bPressAcc = true;
            LongPressAddButton.onRelease.AddListener(OnClickAdd);
            LongPressAddButton.OnPressAcc.AddListener(OnClickAccAdd);

            textUseNum = transform.Find("Animator/View_Cost/View_SelectNum/Btn_Num/Text").GetComponent<Text>();

            textHaveNum = transform.Find("Animator/View_Cost/Text_Have/Text").GetComponent<Text>();

            btnConfirm = transform.Find("Animator/Btn_01").GetComponent<Button>();
            btnConfirm.onClick.AddListener(OnClickUpgrade);
        }

        protected override void OnOpen(object arg)
        {            
            _infoId = (uint)arg;
        }

        //protected override void ProcessEventsForEnable(bool toRegister)
        //{            
        //    //Sys_Partner.Instance.eventEmitter.Handle<uint>(Sys_Partner.EEvents.OnAttrChangeNotification, OnAttrChange, toRegister);
        //}

        protected override void OnShow()
        {            
            UpdateInfo(_infoId);
        }        

        private void OnClickMinus()
        {
            if (_useNum > 1)
            {
                _useNum--;
            }

            UpdateExpData();
        }

        private void OnClickAccAdd()
        {
            if (progress != null && progress.isMaxLevel)
            {
                //Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2006194u));
                return;
            }

            if (progress != null && progress.curLevel >= Sys_Role.Instance.Role.Level + levelDiffMax)
            {
                //Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2006191, levelDiffMax));
                return;
            }

            if (_useNum < _itemNum)
            {
                _useNum++;
            }

            UpdateExpData();
        }

        private void OnClickAdd()
        {
            if (progress != null && progress.isMaxLevel)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2006194u));
                return;
            }

            if (progress != null && progress.curLevel >= Sys_Role.Instance.Role.Level + levelDiffMax)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2006191, levelDiffMax.ToString()));
                return;
            }

            if (_useNum < _itemNum)
            {
                _useNum++;
            }

            UpdateExpData();
        }

        private void OnClickUpgrade()
        {
            if (_useNum > _itemNum)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2006192u));
            }
            else
            {
                Sys_Partner.Instance.AddExpReq(_infoId, _ItemId, _useNum);
                this.CloseSelf();
            }
        }

        //private void OnAttrChange(uint infoId)
        //{
        //    if (_infoId == infoId)
        //    {
        //        UIManager.CloseUI(EUIID.UI_PartnerLevelUp);
        //    }
        //}

        private void UpdateInfo(uint infoId)
        {
            CSVPartner.Data infoData = CSVPartner.Instance.GetConfData(infoId);
            levelDiffMax = infoData.limit_level;

            //道具显示
            _itemNum = Sys_Bag.Instance.GetItemCount(_ItemId);
            PropIconLoader.ShowItemData itemData = new PropIconLoader.ShowItemData(_ItemId, _itemNum, true, false, false, false, false, true);
            propItem.SetData(new MessageBoxEvt( EUIID.UI_PartnerLevelUp, itemData));
            TextHelper.SetText(textItemName, CSVItem.Instance.GetConfData(_ItemId).name_id);
            textHaveNum.text = _itemNum.ToString();

            _useNum = 1;

            Partner partnerData = Sys_Partner.Instance.GetPartnerInfo(infoId);
            textLv1.text = LanguageHelper.GetTextContent(2006104, partnerData.Level.ToString());

            UpdateExpData();
        }

        private void UpdateExpData()
        {
            progress = Sys_Partner.Instance.CalProgress(_infoId, _useNum, _ItemId);
            sliderExp.gameObject.SetActive(progress.showCurProgress);

            //UnityEngine.Debug.LogErrorFormat("cur={0}", progress.curProgress);
            //UnityEngine.Debug.LogErrorFormat("add={0}", progress.addProgress);
            sliderExp.value = progress.curProgress;
            sliderAdd.value = progress.addProgress;

            uint preLevel = progress.targetLevel;
            preLevel = preLevel <= 1 ? preLevel : preLevel - 1; //最小1级
            CSVPartnerLevel.Data preLevelData = CSVPartnerLevel.Instance.GetUniqData(_infoId, preLevel);
            CSVPartnerLevel.Data targetLevelData = CSVPartnerLevel.Instance.GetUniqData(_infoId, progress.targetLevel);

            textExp.text = string.Format("{0}/{1}", progress.totalExp - preLevelData.totol_exp, targetLevelData.upgrade_exp);

            textUseNum.text = _useNum.ToString();
            textLv2.text = LanguageHelper.GetTextContent(2006104, progress.curLevel.ToString());
        }
    }
}


