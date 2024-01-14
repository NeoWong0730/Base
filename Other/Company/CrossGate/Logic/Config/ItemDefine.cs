namespace Logic
{
    /// <summary>
    /// 对应道具表(CSVItem) 道具类型(type_id)字段
    /// </summary>
    public enum EItemType
    {
        Equipment = 1000,           //装备
        PetEquipment = 1001,        //宠物装备
        Jewel = 1101,               //宝石
        Pet = 1103,                 //宠物
        SingleServerHorn = 3000,    //本服喇叭
        FullServerHorn = 3001,      //全服喇叭
        PetRemakeItem = 3010,  //宠物改造道具
        PetRemakeSkillBook = 3014,  //宠物改造技能书
        PetSkillBook = 3015,        //宠物技能书
        PetMountSkillBook = 3030,        //宠物骑术技能书
        PetEquipSmeltItem = 3033, //宠物装备炼化道具类型
        PetFruitItem = 3035, //宠物档位果实
        Enchant = 4001,             //附魔符文
        SuitPaper = 4005,           //套装图纸(装备套装)
        Meat = 1901,                  //肉/蛋
        SeaFood = 1902,               //海鲜
        Fruit = 1903,                 //果蔬
        Ingredients = 1904,           //配料
        Crystal = 1150,               //元素水晶
        Ornament = 1011,            //饰品-项链
        ChangeCard= 1704,       //变身卡
    }

    public enum EQualityType 
    {
        White = 1,
        Green = 2,
        Blue = 3,
        Purple = 4,
        Orange = 5,
    }

    /// <summary>
    /// 宝石类型
    /// </summary>
    public enum EJewelType
    {
        All = 0,
        One = 1, //红包石
        Two = 2, //绿宝石
        Three = 3, //青宝石
        Four = 4, //黄宝石
        Five = 5, //紫晶石
        Six = 6,  //蓝宝石
        Seven = 7, //石榴石
        Eight = 8, //圣魔石
        Nine = 9, //O零件
        Ten = 10, //骑士宝石
        Eleven = 11, //Q零件
        Twelve = 12, //玫红宝石
    }
}