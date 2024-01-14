using System.Collections.Generic;
using UnityEngine;
using Logic.Core;
using Lib.Core;
using Table;
using System;
using UnityEngine.UI;
namespace Logic
{
    public class UI_Pet_Demon_SkillPreview_Layout
    {
        private Button closeBtn;
        private List<Transform> skillTransformPartner = new List<Transform>(2);
        private Transform layoutTransform;
        private List<List<uint>> skills;
        private bool isInstance;
        private int index = 0;
        private int currentCount;
        private int needCount;
        //每次加载数量
        private readonly int FrameInstanceCount = 5;
        public void Init(Transform transform)
        {
            closeBtn = transform.Find("Animator/View_TipsBgNew02/Btn_Close").GetComponent<Button>();
            skillTransformPartner.Add(transform.Find("Animator/Scroll View/Viewport/Content/Item/SkillGroup"));
            skillTransformPartner.Add(transform.Find("Animator/Scroll View/Viewport/Content/Item (1)/SkillGroup"));
            layoutTransform = transform.Find("Animator/Scroll View");
        }

        public void RegisterEvents(IListener listener)
        {
            closeBtn.onClick.AddListener(listener.CloseBtnClicked);
        }

        public void SetView(List<uint> groudIds ,uint showType)
        {
            Dictionary<uint, List<uint>> skillDics = new Dictionary<uint, List<uint>>(2);
            
            if (null != groudIds)
            {
                for (int i = 0; i < groudIds.Count; i++)
                {
                    if (showType == 0)
                    {
                        skillDics.Add(groudIds[i], new List<uint>(16));
                    }
                    else if(showType == 1)
                    {
                        if(i == 0)
                        {
                            skillDics.Add(groudIds[i], new List<uint>(16));
                        }
                        else
                        {
                            skillDics.Add(0, new List<uint>(16));
                        }
                        
                    }
                    else if (showType == 2)
                    {
                        if (i == 1)
                        {
                            skillDics.Add(groudIds[i], new List<uint>(16));
                        }
                        else
                        {
                            skillDics.Add(0, new List<uint>(16));
                        }
                        
                    }
                }
                var grouds = CSVSoulBeadSkill.Instance.GetAll();
                var count = grouds.Count;
                for (int i = 0; i < count; i++)
                {
                    var data = grouds[i];
                    if (skillDics.TryGetValue(data.group_id, out List<uint> value))
                    {
                        value.Add(data.skill_id + 4);
                    }
                }
            }
            skills = new List<List<uint>>(skillDics.Values);
            
            if (null != skills && skills.Count > 0)
            {
                for (int i = 0; i < skills.Count; i++)
                {
                    var count = skills[i].Count;
                    if (i < skillTransformPartner.Count)
                    {
                        Transform trans = skillTransformPartner[i];
                        trans.parent.gameObject.SetActive(count > 0);
                    }
                }
                index = 0;
                currentCount = 0;
                needCount = skills[index].Count;
                isInstance = true;
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

        /// <summary>
        /// 一次性打开会造成内存瞬时压力,分开加载 每次加载FrameInstanceCount数量
        /// </summary>
        public void Applay()
        {
            if(isInstance)
            {
                if (null != skills)
                {
                    if (null != skills[index])
                    {
                        var skillCount = skills[index].Count;
                        
                        if (index < skillTransformPartner.Count)
                        {
                            Transform partner = skillTransformPartner[index];
                            var current = currentCount;
                            currentCount = Math.Min(currentCount + FrameInstanceCount, needCount);
                            if(currentCount <= needCount)
                            {
                                FrameworkTool.CreateChildList(partner, currentCount);
                                for (int i = current; i < currentCount; i++)
                                {
                                    Transform skillCeilTransform = partner.GetChild(i);
                                    PetSkillCeil entry = new PetSkillCeil();
                                    entry.BingGameObject(skillCeilTransform.gameObject);
                                    entry.AddClickListener(OnSkillSelect);
                                    entry.SetData(skills[index][i], false, false);
                                }

                                if(currentCount == needCount)
                                {
                                    if (index + 1 < skills.Count)
                                    {
                                        index += 1;
                                        currentCount = 0;
                                        needCount = skills[index].Count;
                                        isInstance = true;
                                    }
                                    else
                                    {
                                        isInstance = false;
                                    }
                                }
                                
                                FrameworkTool.ForceRebuildLayout(layoutTransform.gameObject);
                            }
                        }
                    }
                }
            }
        }

        public interface IListener
        {
            void CloseBtnClicked();
        }
    }

    public class UI_Pet_Demon_SkillPreview : UIBase, UI_Pet_Demon_SkillPreview_Layout.IListener
    {
        private UI_Pet_Demon_SkillPreview_Layout layout = new UI_Pet_Demon_SkillPreview_Layout();
        private uint currentType = 1;
        private uint showType = 0;
        private bool isInit = false;
        protected override void OnLoaded()
        {
            layout.Init(transform);
            layout.RegisterEvents(this);
        }

        protected override void OnOpen(object arg = null)
        {
            if (arg is Tuple<uint, uint> tuple)
            {
                currentType = tuple.Item1;
                showType = tuple.Item2;
            }
            else
            {
                currentType = 0;
                showType = 0;
            }
            
        }

        protected override void OnShow()
        {
            if(!isInit)
            {
                isInit = true;
                RefreshView();
            }
        }

        protected override void OnUpdate()
        {
            layout.Applay();
        }

        private void RefreshView()
        {
            layout.SetView(Sys_Pet.Instance.GetDemonSkillGroup(currentType), showType);
        }

        public void CloseBtnClicked()
        {
            UIManager.CloseUI(EUIID.UI_Pet_Demon_SkillPreview);
        }

    }
}