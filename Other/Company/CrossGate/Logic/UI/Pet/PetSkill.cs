using System;
using Table;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Logic
{

    public class PetSkillBase
    {
        public uint skillId;
        public bool isUnique = false; // 专属
        public bool isBuild = false;// 改造
        public bool isHasHight = false;// 是否有高级技能/低级将失效
        public bool isPreview = false; // 是否是预览格子
        public bool isDetailBasic = false;  //是否是宠物详情基础技能
        public bool isMountSkill = false;  //是否是宠物骑术技能
        public bool isDemonSpiritSkill = false;  //是否魔魂技能
        public bool showLevel = true; // 是否显示等级
        public int  index;
        public bool IsActive
        {
            get
            {
                if (isHasSkill)
                {
                    return Sys_Skill.Instance.IsActiveSkill(skillId);
                }
                else
                {
                    return false;
                }
            }
        }
        public string SkillName
        {
            get
            {
                string name = "";
                if (isHasSkill)
                {
                   if(IsActive)
                    {
                        CSVActiveSkillInfo.Data cSVActiveSkillInfoData = CSVActiveSkillInfo.Instance.GetConfData(skillId);
                        if(null != cSVActiveSkillInfoData)
                        {
                            name = LanguageHelper.GetTextContent(cSVActiveSkillInfoData.name);
                        }
                    }
                   else
                    {
                        CSVPassiveSkillInfo.Data cSVPassiveSkillInfo = CSVPassiveSkillInfo.Instance.GetConfData(skillId);
                        if (null != cSVPassiveSkillInfo)
                        {
                            name = LanguageHelper.GetTextContent(cSVPassiveSkillInfo.name);
                        }
                    }
                }

                return name;
            }
        }
        public uint SkillLevel
        {
            get
            {
                uint level = 0u;
                if (isHasSkill)
                {
                    if (IsActive)
                    {
                        CSVActiveSkillInfo.Data cSVActiveSkillInfoData = CSVActiveSkillInfo.Instance.GetConfData(skillId);
                        if (null != cSVActiveSkillInfoData)
                        {
                            level = cSVActiveSkillInfoData.level;
                        }
                    }
                    else
                    {
                        CSVPassiveSkillInfo.Data cSVPassiveSkillInfo = CSVPassiveSkillInfo.Instance.GetConfData(skillId);
                        if (null != cSVPassiveSkillInfo)
                        {
                            level = cSVPassiveSkillInfo.level;
                        }
                    }
                }
                return level;
            }
        }
        public bool isHasSkill
        {
            get { return skillId != 0; }
        }

        public PetSkillBase(uint _skillId, bool _isUnique, bool _isBuild, int _index, bool _hasHight, bool _isPreview, bool _isDetailBasic, bool _isMountSkill, bool _showLevel, bool _isDemonSpiritSkill)
        {
            Reset(_skillId, _isUnique, _isBuild, _index, _hasHight, _isPreview, _isDetailBasic, _isMountSkill, _showLevel, _isDemonSpiritSkill);
        }

        public void Reset(uint _skillId, bool _isUnique, bool _isBuild, int _index, bool _hasHight, bool _isPreview, bool _isDetailBasic, bool _isMountSkill, bool _showLevel, bool _isDemonSpiritSkill)
        {
            skillId = _skillId;
            isUnique = _isUnique;
            isBuild = _isBuild;
            index = _index;
            isHasHight = _hasHight;
            isPreview = _isPreview;
            isDetailBasic = _isDetailBasic;
            isMountSkill = _isMountSkill;
            showLevel = _showLevel;
            isDemonSpiritSkill = _isDemonSpiritSkill;
        }

    }

    public class PetSkillCeil
    {
        public Transform transform;
        public PetSkillBase petSkillBase;
        public PetSkillItem02 skillItem;
        private Text skillNameText;
        private Text skillLevelText;
        private Action<PetSkillCeil> onClick;

        public Button addBtn;
        public Image itemicon;
        public Text itemName;
        public Image imgSkillBook;

        public Image buildQuality; //改造专属品质
        public Button buildLockBtn; //改造锁
        public GameObject lockGo; //改造锁
        public GameObject previewGo; //改造预览
        public Text buildText; // 哪一改
        public Image bgImage;
        public uint itemId;
        public void BingGameObject(GameObject go)
        {
            transform = go.transform;
            bgImage = transform.GetComponent<Image>();
            skillItem = new PetSkillItem02();
            skillItem.Bind(transform.Find("PetSkillItem01").gameObject);
            skillNameText = transform.Find("Text_Name").GetComponent<Text>();
            skillLevelText = transform.Find("Text_Lv").GetComponent<Text>();
            addBtn = transform.Find("Button_Add").GetComponent<Button>();
            addBtn.onClick.AddListener(OnClicked);
            itemicon = transform.Find("Image_Icon").GetComponent<Image>();
            itemicon.transform.GetComponent<Button>().onClick.AddListener(OnClicked);
            itemName = transform.Find("Image_Icon/Text_Name").GetComponent<Text>();
            imgSkillBook = transform.Find("Image_Icon/Image_Skill")?.GetComponent<Image>();

            buildText = transform.Find("Image_Name/Text")?.GetComponent<Text>();
            buildLockBtn =  transform.Find("Btn_Lock")?.GetComponent<Button>();
            lockGo = transform.Find("Btn_Lock/Image")?.gameObject;
            previewGo = transform.Find("Btn_Lock/Image_Ques")?.gameObject;
            buildLockBtn?.onClick.AddListener(OnClicked);
            skillItem.EnableLongPress(false);
            skillItem.AddClickListener(OnClicked);
        }
        
        public void AddClickListener(Action<PetSkillCeil> onclicked = null)
        {
            onClick = onclicked;
        }

        private void OnClicked()
        {
            onClick?.Invoke(this);
        }

        /// <summary>
        /// index 只有在是改造技能是有效
        /// </summary>
        /// <param name="skillId"></param>
        /// <param name="isUnique"></param>
        /// <param name="isBuild"></param>
        /// <param name="itemId"></param>
        /// <param name="index"></param>
        public void SetData(uint skillId, bool isUnique, bool isBuild, uint itemId = 0u, int index = 0, 
            bool hasHight = false, bool isPreview = false, bool isDetaiBasic = false, bool isMountSkill = false, bool showLevel = true, bool isDemonSpiritSkill = false)
        {
            if (petSkillBase == null)
            {
                petSkillBase = new PetSkillBase(skillId, isUnique, isBuild, index, hasHight, isPreview,isDetaiBasic, isMountSkill, showLevel, isDemonSpiritSkill);
            }
            else
            {
                petSkillBase.Reset(skillId, isUnique, isBuild, index, hasHight, isPreview, isDetaiBasic, isMountSkill, showLevel, isDemonSpiritSkill);
            }
            SetPetSkillInfo();
            SetItemInfo(itemId);
        }
        
        private void SetPetSkillInfo()
        {
            bool hasSkill = petSkillBase.isHasSkill;
            if (hasSkill)
            {
                skillItem.SetDate(petSkillBase.skillId);
                skillItem.skillImage.gameObject.SetActive(true);
                skillLevelText.text = LanguageHelper.GetTextContent(10963, petSkillBase.SkillLevel.ToString());
            }
            ImageHelper.SetImageGray(transform, petSkillBase.isHasHight, true);
            skillItem.transform.gameObject.SetActive(hasSkill);
            skillNameText.text = petSkillBase.SkillName;            
            skillLevelText.gameObject.SetActive(!petSkillBase.isBuild && hasSkill && petSkillBase.showLevel);
            bgImage.enabled = !petSkillBase.isBuild&& !petSkillBase.isDetailBasic;
            skillItem.mountGo?.SetActive(petSkillBase.isMountSkill);
            skillItem.uniqueGo.SetActive(petSkillBase.isUnique);
            skillItem.demonSpiritGo?.SetActive(petSkillBase.isDemonSpiritSkill);
            skillItem.buildGo.SetActive(false);
            if (null != buildLockBtn )
            {
                if (petSkillBase.isBuild)
                {                 
                    var notSkill = petSkillBase.isBuild && petSkillBase.skillId == 0;
                    if (notSkill)
                    {
                        if (null != lockGo)
                            lockGo.SetActive(!petSkillBase.isPreview);
                        if (null != previewGo)
                            previewGo.SetActive(petSkillBase.isPreview);
                    }
                    buildLockBtn.gameObject.SetActive(notSkill);
                }
                else
                {
                    buildLockBtn.gameObject.SetActive(false);
                }
            }
                
            if(null != buildText)
            {
                buildText.transform.parent.gameObject.SetActive(petSkillBase.isBuild);
                if (petSkillBase.isBuild)
                {
                    var lst = Sys_Pet.Instance.BuildSkillNumByIndex;
                    buildText.text = LanguageHelper.GetTextContent(10798 + lst[petSkillBase.index]);
                }
            }  
        }

        public void SetItemInfo(uint itemId)
        {
            this.itemId = itemId;
            bool hasItem = itemId != 0u;
            itemicon.gameObject.SetActive(hasItem);
            if (hasItem)
            {
                CSVItem.Data itemData = CSVItem.Instance.GetConfData(itemId);
                if (null != itemData)
                {
                    //技能书等级特殊显示
                    if (null != imgSkillBook)
                    {
                        Sys_Skill.Instance.ShowPetSkillBook(imgSkillBook, itemData);
                    }
                    ImageHelper.SetIcon(itemicon, itemData.icon_id);
                    TextHelper.SetText(itemName, itemData.name_id);
                    itemicon.enabled = true;
                }
                else
                {
                    if (null != imgSkillBook && imgSkillBook.gameObject.activeSelf)
                    {
                        //当作为添加不定道具格时，空格状态强制隐藏
                        imgSkillBook.gameObject.SetActive(false);
                    }
                }
            }
        }

        public void LockSelect(bool state)
        {
            skillItem.select01Go.SetActive(state);
        }

        public void Invert()
        {
            skillItem.select02Go.SetActive(!skillItem.select02Go.activeSelf);
        }

        public void ReSelect2()
        {
            skillItem.select02Go.SetActive(false);
        }

        public bool IsLockActive()
        {
            if (petSkillBase.IsActive)
            {
                CSVActiveSkillInfo.Data activeInfoSkill = CSVActiveSkillInfo.Instance.GetConfData(petSkillBase.skillId);
                if (null != activeInfoSkill && activeInfoSkill.can_lock == 1)
                {
                    return true;
                }
                return false;
            }
            else
            {
                return false;
            }
        }
    }
}

