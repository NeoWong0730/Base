using Lib.Core;
using Logic;
using Logic.Core;
using Packet;
using System.Text;
using Table;
using UnityEngine;
using UnityEngine.UI;
public class UI_Mine_Result : UIBase
{
    public Image img_roleHeadIcon;
    public Image img_petHeadIcon;
    public Text txt_roleExp;
    public Text txt_petExp;
    public GameObject petPanel;
    public GameObject skillTextItem;
    public GameObject skillPanel;
    public GameObject skill_View_Content;
    public GameObject propItem_View_Content;
    public GameObject propItem; 
    public Button btn_Close;
    private Timer m_timer;
    private Toggle tg_Close;

    public void Init(Transform transform)
    {
        btn_Close=transform.Find("Animator/Content/off-bg").GetComponent<Button>();
        img_roleHeadIcon = transform.Find("Animator/Content/Role/Head").GetComponent<Image>();
        txt_roleExp= transform.Find("Animator/Content/Role/EXP").GetComponent<Text>();
        img_petHeadIcon = transform.Find("Animator/Content/Pet/Head").GetComponent<Image>();
        txt_petExp= transform.Find("Animator/Content/Pet/EXP").GetComponent<Text>();
        petPanel= transform.Find("Animator/Content/Pet").gameObject;
        skillTextItem = transform.Find("Animator/Content/TextItem").gameObject;
        skillPanel = transform.Find("Animator/Content/Skill_View").gameObject;
        skill_View_Content = transform.Find("Animator/Content/Skill_View/Viewport/Content").gameObject;
        propItem= transform.Find("Animator/Content/PropItem").gameObject;
        propItem_View_Content = transform.Find("Animator/Content/PropItem_View/Viewport/Content").gameObject;
        tg_Close= transform.Find("Animator/Content/Toggle").GetComponent<Toggle>();
        tg_Close.onValueChanged.AddListener((bool value) => OnThisLoginClicked(value));

    }

    protected override void OnLoaded()
    {
        Init(transform);
        btn_Close.onClick.AddListener(OnCloseButtonClicked);
    }

 

    protected override void OnShow()
    {        
        float animTime = float.Parse(CSVParam.Instance.GetConfData(1089).str_value);
        m_timer?.Cancel();
        m_timer = Timer.Register(animTime, OnCloseButtonClicked);
        RoleExpShow();
        PetExpShow();
        AwardShow();
        SkillExpAddShow();
       



    }
    public void RoleExpShow()
    {
        Sys_Head.Instance.SetHeadAndFrameData(img_roleHeadIcon);
        txt_roleExp.text = "+" + Sys_Attr.Instance.addExpInBattleEnd;
        Sys_Attr.Instance.addExpInBattleEnd = 0;
    }
    public void PetExpShow()
    {
        if (!Sys_Pet.Instance.fightPet.HasFightPet())//没有出战宠物时
        {
            petPanel.SetActive(false);
            return;
        }
        petPanel.SetActive(true);
        txt_petExp.text= "+" + Sys_Pet.Instance.addexpInBattleEnd;
        Sys_Pet.Instance.addexpInBattleEnd = 0;
        
        ClientPet cPet=Sys_Pet.Instance.GetPetByUId(Sys_Pet.Instance.fightPet.GetUid());
        if (cPet.petData!=null)
        {
            ImageHelper.SetIcon(img_petHeadIcon, cPet.petData.icon_id);
        }
        else
        {
            DebugUtil.LogError("petdate is null");
        }

    }

    public void SkillExpAddShow()
    {
        UnityEngine.Object.Instantiate(skillTextItem, skill_View_Content.transform);
        FrameworkTool.CreateChildList(skill_View_Content.transform, Sys_Settlement.Instance.skillList.Count);
        for (int i = 0; i < Sys_Settlement.Instance.skillList.Count; i++)
        {
            GameObject go = skill_View_Content.transform.GetChild(i).gameObject;
            go.SetActive(true);
            Text name = go.transform.Find("Name").GetComponent<Text>();
            name.text = LanguageHelper.GetTextContent(CSVActiveSkillInfo.Instance.GetConfData(Sys_Settlement.Instance.skillList[i].skillid).name);
            Text exp = go.transform.Find("Num").GetComponent<Text>();
            exp.text = "+" + Sys_Settlement.Instance.skillList[i].addexp.ToString()+ LanguageHelper.GetTextContent(1012004);
        }

    }

    public void AwardShow()
    {
        int count = Sys_Settlement.Instance.itemList.Count;
        for (int i = 0; i < Sys_Settlement.Instance.itemList.Count; i++)
        {
            if (Sys_Settlement.Instance.itemList[i].infoid == 4)//排除经验掉落
            {
                count -= 1;
            }
        }
        if (count == 0)
        {
            return;
        }
        UnityEngine.Object.Instantiate(propItem, propItem_View_Content.transform);
        FrameworkTool.CreateChildList(propItem_View_Content.transform, count);
        for (int i = 0, j = 0; i < Sys_Settlement.Instance.itemList.Count; i++)
        {
            if (Sys_Settlement.Instance.itemList[i].infoid != 4)
            {
                GameObject go = propItem_View_Content.transform.GetChild(j).gameObject;
                if (j < count)
                {
                    j++;
                }
                go.SetActive(true);
                PropItem propItem = new PropItem();
                propItem.BindGameObject(go);
                propItem.SetData(new MessageBoxEvt(EUIID.UI_Mine_Result, new PropIconLoader.ShowItemData(Sys_Settlement.Instance.itemList[i].infoid, Sys_Settlement.Instance.itemList[i].count, true, false, false, false, false,
                        _bShowCount: true, _bUseClick: true, _bShowBagCount: false)));
            }


        }

    }
    protected override void OnHide()
    {        
        m_timer?.Cancel();

    }
    #region 按钮
    private void OnCloseButtonClicked()
    {
        UIManager.CloseUI(EUIID.UI_Mine_Result);
    }
    private void OnThisLoginClicked(bool value)
    {
        if (value)
        {
            Sys_Settlement.Instance.IsOpen = false;

        }
        else
        {
            Sys_Settlement.Instance.IsOpen = true;
        }
        
    }
    #endregion


}