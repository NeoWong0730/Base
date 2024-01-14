using Logic.Core;
using Lib.Core;
using System;
using Table;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

namespace Logic
{
    public class UIRuneResultParam
    {
        public uint RuneId { get; set; } = 0;
        public uint Count { get; set; } = 0;
        public bool Select { get; set; } = false;
    }

    public class UI_RuneIcon
    {
        private UIRuneResultParam data;
        private Button iconBtn;
        private Image iconImg;
        private Image runeLevelImage;
        private Text numText;
        private Text idText;        

        public void Init(Transform transform)
        {
            iconImg = transform.Find("Btn_Item/Image_Icon").GetComponent<Image>();
            runeLevelImage = transform.Find("Image_RuneRank")?.GetComponent<Image>();
            numText = transform.Find("Text_Number").GetComponent<Text>();
            numText.gameObject.SetActive(true);
            iconBtn = transform.Find("Btn_Item").GetComponent<Button>();
            iconBtn.onClick.AddListener(() =>
            {
                if (null != data)
                    UIManager.OpenUI(EUIID.UI_PartnerRune_Tips, false, data.RuneId);
            });
            idText = transform.Find("id")?.GetComponent<Text>();
        }

        public void SetData(params object[] arg)
        {
            if (null != arg)
            {
                data = arg[0] as UIRuneResultParam;
                if (null != data)
                {
                    CSVRuneInfo.Data runeData = CSVRuneInfo.Instance.GetConfData(data.RuneId);
                    if (null != runeData)
                    {
                        ImageHelper.SetIcon(iconImg, runeData.icon);
                        ImageHelper.SetIcon(runeLevelImage, Sys_Partner.Instance.GetRuneLevelImageId(runeData.rune_lvl));
                        runeLevelImage.gameObject.SetActive(true);
                        iconImg.enabled = true;
                        TextHelper.SetText(numText, data.Count > 1 ? data.Count.ToString() : "");
                    }
                }

                if (idText != null)
                {
#if UNITY_EDITOR
                    idText.text = data.RuneId.ToString();
#else
            idText.text = " ";
#endif
                }
            }
        }
    }

    public class UI_Rune_Result : UIBase
    {
        private GameObject runeGo;
        private InfinityGridLayoutGroup infinity;
        private Dictionary<GameObject, UI_RuneIcon> runeCeilGrids = new Dictionary<GameObject, UI_RuneIcon>();
        private List<UI_RuneIcon> runeGrids = new List<UI_RuneIcon>();
        private Button closeBtn;
        private List<UIRuneResultParam> list = new List<UIRuneResultParam>();
        private Transform view2;

        private Animator ani;

        protected override void OnLoaded()
        {
            infinity = transform.Find("Aniamtor/Scroll_View/Viewport").gameObject.GetNeedComponent<InfinityGridLayoutGroup>();
            runeGo = transform.Find("Aniamtor/Scroll_View/Item").gameObject;
            view2 = transform.Find("Aniamtor/Scroll_View/Viewport2");
            ani = transform.Find("Aniamtor").GetComponent<Animator>();

            infinity.minAmount = 24;
            infinity.updateChildrenCallback = UpdateChildrenCallback;
            SetSkillItem();
            closeBtn = transform.Find("Aniamtor/Image_BG").GetComponent<Button>();
            closeBtn.onClick.AddListener(() =>
            {
                if(Sys_Partner.Instance.IsNeedShowRuneResult())
                {
                    list = Sys_Partner.Instance.GetRuneResultList();
                    ResetView();
                    ani.cullingMode = AnimatorCullingMode.AlwaysAnimate;
                    ani.Play("Open", -1, 0f);
                    
                }
                else
                {
                    CloseSelf();
                }
            });
        }

        private void SetSkillItem()
        {
            for (int i = 0; i < infinity.transform.childCount; i++)
            {
                Transform tran = infinity.transform.GetChild(i);
                UI_RuneIcon runeCeil = new UI_RuneIcon();
                runeCeil.Init(tran);
                runeCeilGrids.Add(tran.gameObject, runeCeil);
                runeGrids.Add(runeCeil);
            }
        }

        private void UpdateChildrenCallback(int index, Transform trans)
        {
            if (index < 0 || index >= list.Count)
                return;
            if (runeCeilGrids.ContainsKey(trans.gameObject))
            {
                UI_RuneIcon runeCeil = runeCeilGrids[trans.gameObject];
                runeCeil.SetData(list[index]);
            }
        }

        protected override void OnOpen(object arg)
        {
            if (null != arg)
                list = arg as List<UIRuneResultParam>;
        }

        protected override void OnShow()
        {
            ResetView();
        }

        private void ResetView()
        {
            if (list.Count > 12)
            {
                infinity.SetAmount(list.Count);
            }
            else
            {
                FrameworkTool.DestroyChildren(view2.gameObject);
                for (int i = 0; i < list.Count; i++)
                {
                    GameObject go = GameObject.Instantiate<GameObject>(runeGo, view2);
                    UI_RuneIcon icon = new UI_RuneIcon();
                    icon.Init(go.transform);
                    icon.SetData(list[i]);
                    go.SetActive(true);
                }
                infinity.SetAmount(0);
            }
            view2.gameObject.SetActive(!(list.Count > 12));

        }


    }
}
