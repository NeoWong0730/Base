using Logic.Core;
using Table;
using UnityEngine.UI;

namespace Logic
{
    public class UI_SignBuy : UIBase
    {
        private Text buyCount;
        private Text price;
        private Text limitCount;
        private Image costIcon;
        private Button addBtn;
        private Button subBtn;
        private Button okBtn;
        private Button closeBtn;

        private uint count;
        private uint costId;
        private uint costNum;
        private uint limitBuyCount;
        protected override void OnLoaded()
        {
            buyCount = transform.Find("Animator/Number/Value").GetComponent<Text>();
            price = transform.Find("Animator/Cost/Value").GetComponent<Text>();
            limitCount= transform.Find("Animator/Text_Tips").GetComponent<Text>();
            costIcon = transform.Find("Animator/Cost/Icon").GetComponent<Image>();
            addBtn = transform.Find("Animator/Number/Btn_Add").GetComponent<Button>();
            subBtn = transform.Find("Animator/Number/Btn_Minus").GetComponent<Button>();
            okBtn = transform.Find("Animator/Button_Confirm").GetComponent<Button>();
            closeBtn = transform.Find("Animator/View_TipsBg01_Small/Btn_Close").GetComponent<Button>();
            addBtn.onClick.AddListener(OnAddBtn);
            subBtn.onClick.AddListener(OnSubBtn);
            okBtn.onClick.AddListener(OnOkBtn);
            closeBtn.onClick.AddListener(OnCloseBtn);
        }

        protected override void OnShow()
        {
            string str = CSVParam.Instance.GetConfData(1020).str_value;
            uint.TryParse(str.Split('|')[0], out costId);
            uint.TryParse(str.Split('|')[1], out costNum);
            uint.TryParse(CSVParam.Instance.GetConfData(1007).str_value, out limitBuyCount);
            ImageHelper.SetIcon(costIcon,CSVItem.Instance.GetConfData(costId).icon_id);
            count = 1;
            buyCount.text = count.ToString();
            price.text = (costNum * count).ToString();
            limitCount.text = LanguageHelper.GetTextContent(5211, limitBuyCount.ToString());
        }

        protected override void OnHide()
        {
        }

        private void OnAddBtn()
        {
            if (count >= limitBuyCount)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(5220));
                return;
            }
            count++;
            buyCount.text = count.ToString();
            price.text = (costNum * count).ToString();
        }

        private void OnSubBtn()
        {
            if (count <= 1)
            {
                return;
            }
            count--;
            buyCount.text = count.ToString();
            price.text = (costNum * count).ToString();
        }

        private void OnOkBtn()
        {
            if (count == 0)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(5221));
            }
            else if(Sys_Bag.Instance.GetItemCount(costId)< costNum)
            {
                CSVItem.Data data = CSVItem.Instance.GetConfData(costId);
                Sys_Hint.Instance.PushContent_Normal( LanguageHelper.GetTextContent(5222, LanguageHelper.GetTextContent(data.name_id)));

            }
            else
            { 
                Sys_Sign.Instance.SignDailySignBuyReq(count);
                UIManager.CloseUI(EUIID.UI_SignBuy);
            }
        }

        private void OnCloseBtn()
        {
            UIManager.CloseUI(EUIID.UI_SignBuy);
        }
    }
}
