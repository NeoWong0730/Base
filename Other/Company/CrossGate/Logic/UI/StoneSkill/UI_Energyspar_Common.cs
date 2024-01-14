using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Logic.Core;
using Table;
using Lib.Core;
using System.Collections.Generic;

namespace Logic
{

    public class ItemCost : UI_CostItem
    {
        public override void SetGameObject(GameObject go)
        {
            this.gameObject = go;
            content = go.transform.Find("Text_Num").GetComponent<Text>();
            icon = go.transform.Find("Image_Icon").GetComponent<Image>();
        }

        public new void Refresh(ItemIdCount idCount, ItemCostLackType lackType = ItemCostLackType.LeftLackRG, string content = null, bool useSmallIcon = true)
        {
            this.idCount = idCount;

            if (idCount != null)
            {
                if (idCount.CSV != null)
                {
                    ImageHelper.SetIcon(this.icon, useSmallIcon ? idCount.CSV.small_icon_id : idCount.CSV.icon_id);
                }

                uint contentId = 1001; // 白色的一个id
                if (lackType == ItemCostLackType.LeftLackRG)
                {
                    contentId = idCount.Enough ? 1601000006u : 1601000004u;
                    TextHelper.SetText(this.right, Sys_Bag.Instance.GetValueFormat(idCount.count));
                    TextHelper.SetText(this.content, contentId, Sys_Bag.Instance.GetValueFormat(idCount.CountInBag), Sys_Bag.Instance.GetValueFormat(idCount.count));
                }
            }
        }
    }

    public class UI_Energyspar_LevelInfo
    {
        private Image iconImage;
        private Slider levelSlider;
        private Text stoneNameText;
        private Text rightLevelText;
        private GameObject starGo;
        private Transform starGroup;
        private RectTransform sliderStarGroup;

        public void Init(Transform transform)
        {
            iconImage = transform.Find("Image_SkillIcon").GetComponent<Image>();
            levelSlider = transform.Find("Slider_Star").GetComponent<Slider>();
            rightLevelText = transform.Find("Slider_Star/Text_Level_2/Value").GetComponent<Text>();
            stoneNameText = transform.Find("Text_SkillName").GetComponent<Text>();
            starGo = transform.Find("Text_SkillName/Star_Dark").gameObject;
            starGroup = transform.Find("Text_SkillName/StarGroup");
            sliderStarGroup = transform.Find("Slider_Star/SliderStarGroup").GetComponent<RectTransform>();            
        }

        public void UpdateInfo(CSVStone.Data stoneData)
        {
            if(null != stoneData)
            {
                FrameworkTool.DestroyChildren(starGroup.gameObject);
                FrameworkTool.DestroyChildren(sliderStarGroup.gameObject);
                ImageHelper.SetIcon(iconImage, stoneData.icon);
                TextHelper.SetText(stoneNameText, stoneData.stone_name);
                StoneSkillData severData = Sys_StoneSkill.Instance.GetServerDataById(stoneData.id);
                if(null != severData)
                {
                    TextHelper.SetText(rightLevelText, LanguageHelper.GetTextContent(2021031, stoneData.max_level.ToString()));
                    int state = (int)severData.powerStoneUnit.Stage;
                    levelSlider.value = severData.powerStoneUnit.Level;

                    List<CSVStoneStage.Data>  allStage = Sys_StoneSkill.Instance.GetAllStoneStageDataById(stoneData.id);
                    float width = sliderStarGroup.rect.width;
                    for (int i = 0; i < allStage.Count; i++)
                    {                        
                        GameObject go = GameObject.Instantiate<GameObject>(starGo, sliderStarGroup);
                        Small_Star small_Star = new Small_Star();
                        small_Star.BindGameobject(go.transform);
                        go.transform.localPosition = new Vector3(((float)allStage[i].stone_level / stoneData.max_level) * width, 0, 0);
                        small_Star.SetState(severData.powerStoneUnit.Level >= allStage[i].stone_level && (i + 1) <= state);
                    }

                    for (int i = 0; i < state; i++)
                    {
                        GameObject go = GameObject.Instantiate<GameObject>(starGo, starGroup);
                        Small_Star small_Star = new Small_Star();
                        small_Star.BindGameobject(go.transform);
                        small_Star.SetState(true);
                    }

                    if (Sys_StoneSkill.Instance.CanAdvance(stoneData.id))//即将进阶 显示一个暗
                    {
                        GameObject go = GameObject.Instantiate<GameObject>(starGo, starGroup);
                        Small_Star small_Star = new Small_Star();
                        small_Star.BindGameobject(go.transform);
                        small_Star.SetState(false);
                    }
                }
            }
        }

    }

    public class Small_Star
    {
        private Transform transform;
        private GameObject lightStarGo;
        public void BindGameobject( Transform trans)
        {
            transform = trans;
            lightStarGo = transform.Find("Star_Light").gameObject;
        }

        public void SetState(bool isLight)
        {
            lightStarGo.SetActive(isLight);
            transform.gameObject.SetActive(true);
        }
    }
}
