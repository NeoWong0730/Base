using Lib.AssetLoader;
using Lib.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Table;
using Logic.Core;
using UnityEngine.EventSystems;
using UnityEngine.Playables;
using Framework;

namespace Logic
{
    public partial class UI_Team_Player_Layout
    {

        #region    class CommandItem
        public class CommandItem
        {
            public Transform m_transform;
            public Button m_Btn;
            public Text m_Text;

            public int Index { get; set; }

            public int CommandID { get; set; }
            private bool m_Active;
            public bool Active { get { return m_Active; } set { m_Active = value; m_transform.gameObject.SetActive(m_Active); } }

            public uint Command { set { TextHelper.SetText(m_Text, value); } }
            public string CommandTex { set { TextHelper.SetText(m_Text, value); } }

            public ClickItemEvent ClickCommand = new ClickItemEvent();
            public void OnClick()
            {
                ClickCommand.Invoke(CommandID);
            }
        }
        #endregion
    }
    public partial class UI_Team_Player_Layout
    {
        //点击人物的信息
        private Transform m_RoleInfoView;
        private Image m_RoleInfoIcon;

        private Text m_TextLevel;

        private Text m_TextName;
        private Text m_TextProfession;//职称
        private Image m_IProfession;//职称
        private Text m_TextID;//id
        private Text m_TextLabor; //工会名称
        private Text m_TextLaborName;

        private Transform m_ItemPranent;
        private Transform m_Item;

        // private Button m_Close;

        private Button m_Close1;

        private Transform m_TransRelation;
        private Text m_TetRelationNum;
        private Text m_TxtRelationTitle;

        private List<CommandItem> m_CommandItems = new List<CommandItem>();

        public ClickItemEvent ClickCommand = new ClickItemEvent();
        public void SetActivePlayeInfo(bool active)
        {
            m_RoleInfoView.gameObject.SetActive(active);
            m_BtnClose.gameObject.SetActive(active);
        }

        public bool isPlayerInfoActive()
        {
            return m_RoleInfoView.gameObject.activeSelf;
        }

        public bool isPlayerListActive()
        {
            return m_PlayerListTransform.gameObject.activeSelf;
        }
        public void LoadRoleInfo(Transform parent)
        {
            m_RoleInfoView = parent.Find("Team_Player");

            //  m_Close = parent.Find("close").GetComponent<Button>();

            m_Close1 = m_RoleInfoView.Find("View_Tips_Team/Btn_Close").GetComponent<Button>();

            m_RoleInfoIcon = m_RoleInfoView.Find("Head").GetComponent<Image>();

            m_TextLevel = m_RoleInfoView.Find("Image_Icon/Text_Number").GetComponent<Text>();

            m_TextName = m_RoleInfoView.Find("Text_Name").GetComponent<Text>();

            m_TextProfession = m_RoleInfoView.Find("Text_Profession").GetComponent<Text>();


            m_TextID = m_RoleInfoView.Find("Text_Id/Text_Id_Number").GetComponent<Text>();

            m_TextLabor = m_RoleInfoView.Find("Text_Labor").GetComponent<Text>();
            m_TextLaborName = m_RoleInfoView.Find("Text_Labor/Text_Labor_Name").GetComponent<Text>();


            m_IProfession = m_RoleInfoView.Find("Image_IconPro").GetComponent<Image>();

            m_ItemPranent = m_RoleInfoView.Find("ButtonScroll/Viewport");
            m_Item = m_RoleInfoView.Find("ButtonScroll/Viewport/Item");

            m_Item.gameObject.SetActive(false);


            m_TransRelation = m_RoleInfoView.Find("Relation");
            m_TetRelationNum = m_TransRelation.Find("Num").GetComponent<Text>() ;
            m_TxtRelationTitle = m_TransRelation.Find("Title").GetComponent<Text>();

        }
      

        private void LoadCommandItem(Transform transform, CommandItem item)
        {
            item.m_transform = transform;

            item.m_Btn = transform.Find("Button").GetComponent<Button>();
            item.m_Text = transform.Find("Button/Text").GetComponent<Text>();

            item.m_Btn.onClick.AddListener(item.OnClick);

            item.ClickCommand.AddListener(OnClickCommandItem);
        }

        private CommandItem CloneCommandItem()
        {
            GameObject go = GameObject.Instantiate<GameObject>(m_Item.gameObject);

            CommandItem roleItem = new CommandItem();

            LoadCommandItem(go.transform, roleItem);

            go.transform.SetParent(m_ItemPranent, false);

            return roleItem;
        }

        public void SetCommandSize(int size)
        {
            if (m_CommandItems.Count < size)
            {
                int offsize = size - m_CommandItems.Count;
                for (int i = 0; i < offsize; i++)
                {
                    CommandItem member = CloneCommandItem();
                    m_CommandItems.Add(member);
                }
            }

            for (int i = 0; i < m_CommandItems.Count; i++)
            {
                m_CommandItems[i].Active = (i < size);

                m_CommandItems[i].Index = i;

                m_CommandItems[i].m_transform.SetSiblingIndex(i + 1);
            }
        }

        public CommandItem getAtCommandItem(int index)
        {
            if (index >= m_CommandItems.Count)
                return null;

            return m_CommandItems[index];
        }
        private void OnClickCommandItem(int index)
        {
            ClickCommand.Invoke(index);
        }

        public void SetPlayerCommandText(int index, uint textID)
        {
            CommandItem item = getAtCommandItem(index);

            item.Command = textID;
        }
        public void SetPlayerCommandText(int index, string text)
        {
            CommandItem item = getAtCommandItem(index);

            item.CommandTex = text;
        }

        public void SetPlayerCommandInteractable(int index, bool b)
        {
            CommandItem item = getAtCommandItem(index);

            item.m_Btn.interactable = b;
        }
        public void SetPlayerCommandID(int index, int ID)
        {
            CommandItem item = getAtCommandItem(index);

            item.CommandID = ID;
        }

        public void SetPlayerNameText(string name)
        {
            TextHelper.SetText(m_TextName, name);
        }

        public void SetPlayerProfession(uint name, uint rank)//职业名称
        {
            uint icon = OccupationHelper.GetTeamIconID(name);


            m_TextProfession.gameObject.SetActive(icon != 0);
            m_IProfession.gameObject.SetActive(icon != 0);

            if (icon == 0)
                return;

            TextHelper.SetText(m_TextProfession, OccupationHelper.GetTextID(name,rank));
            ImageHelper.SetIcon(m_IProfession, icon);
        }

        public void SetPlayerLabor(string name)//工会名称
        {
            m_TextLabor.gameObject.SetActive(!string.IsNullOrEmpty(name));
            TextHelper.SetText(m_TextLaborName, name);
        }

        public void SetPlayerLevel(uint level)
        {
            TextHelper.SetText(m_TextLevel, level.ToString());
        }

        public void SetPlayerId(ulong id)
        {
            TextHelper.SetText(m_TextID, id.ToString());
        }

        public void SetPlayerIcon(uint id,uint head,uint headframe) 
        {
            if (id == 0)
                return;
            ImageHelper.SetIcon(m_RoleInfoIcon, head);
            Image headFrame = m_RoleInfoIcon.transform.Find("Image_Before_Frame").GetComponent<Image>();
            if (headframe == 0)
            {
                headFrame.gameObject.SetActive(false);
            }
            else
            {
                headFrame.gameObject.SetActive(true);
                ImageHelper.SetIcon(headFrame, headframe);
            }
        }

        public void SetRelationActive(bool active)
        {
            if (m_TransRelation.gameObject.activeSelf != active)
                m_TransRelation.gameObject.SetActive(active);
        }

        public void SetRelation(uint value, string valuestring)
        {
            m_TetRelationNum.text = value.ToString();
            m_TxtRelationTitle.text = valuestring;
        }
    }

    public partial class UI_Team_Player_Layout
    {
        private ToggleGroup m_ToggleGroup;
        private List<Toggle> m_Toggles = new List<Toggle>();

        private Button m_SetStatus;

        public void LoadFamilyInfo(Transform parent)
        {
            m_ToggleGroup = parent.Find("View_Family/ScrollView_Rank/TabList").GetComponent<ToggleGroup>();
            Toggle[] arr_toggles = m_ToggleGroup.gameObject.GetComponentsInChildren<Toggle>();

            foreach (Toggle toggle in arr_toggles)
            {
                m_Toggles.Add(toggle);
            }
            m_SetStatus = parent.Find("View_Family/Button_OK").GetComponent<Button>();
        }

        public Toggle getAtToggleItem(int index)
        {
            if (index >= m_Toggles.Count)
                return null;

            return m_Toggles[index];
        }

        public void SetToggle(int index, string id, string name, string number)
        {
            Toggle toggle = getAtToggleItem(index);
            if (null == toggle) return;

            toggle.name = id;
            Text text_Name = toggle.transform.Find("Text_Job").GetComponent<Text>();
            text_Name.text = name;
            Text text_Number = toggle.transform.Find("Text_Number").GetComponent<Text>();
            text_Number.text = number;
        }

        public void SetToggleActive(int index, bool isActive)
        {
            Toggle toggle = getAtToggleItem(index);
            if (null == toggle) return;
            toggle.gameObject.SetActive(isActive);
        }

        public Toggle GetSelectToggle()
        {
            var ActiveToggles = m_ToggleGroup.ActiveToggles();
            foreach (var child in ActiveToggles)
            {
                if (child.isOn)
                {
                    return child;
                }
            }
            return null;
        }

        public uint GetSelectStatus()
        {
            uint id = 0;
            Toggle toggle = GetSelectToggle();
            if (null == toggle) return id;
            uint.TryParse(toggle.name,out id);
            return id;
        }

        public List<Toggle> GetToggles(int needCount)
        {
            if(needCount > m_Toggles.Count)
            {
                var count = needCount - m_Toggles.Count;
                for (int i = 0; i < count; i++)
                {
                    GameObject go = GameObject.Instantiate<GameObject>(m_Toggles[0].gameObject, m_ToggleGroup.transform);
                    m_Toggles.Add(go.GetComponent<Toggle>());
                }
            }
            return m_Toggles;
        }
    }

}
