using UnityEngine;

[StateComponent((int)StateCategoryEnum.CombatBehaveAI, (int)CombatBehaveAIEnum.JustTest)]
public class WS_CombatBehaveAI_JustTest_SComponent : StateBaseComponent, IUpdate
{
    private string _str;
    public int _selectType;
    private float _total;

    private float _time;

    public override void Init(string str)
	{
        string[] strs = CombatHelp.GetStrParse1Array(str);
        _str = strs[0];
        _total = float.Parse(strs[1]);
        if (strs.Length == 3)
            _selectType = int.Parse(strs[2]);
        else
            _selectType = -1;

        Debug.Log($"{Time.realtimeSinceStartup.ToString()}---{m_CurUseEntity.m_StateControllerEntity.Id.ToString()}--{_str}--开始");

        if (_total == 0f)
        {
            m_CurUseEntity.TranstionMultiStates(this, 1, _selectType);
            return;
        }

        _time = Time.realtimeSinceStartup + _total;
    }

    public override void Dispose()
    {
        Debug.Log($"{Time.realtimeSinceStartup.ToString()}---{m_CurUseEntity.m_StateControllerEntity.Id.ToString()}--{_str}--结束");

        base.Dispose();

        _time = 0f;
    }

    public void Update()
    {
        if (Base_IsPause)
            return;

        if (Time.realtimeSinceStartup > _time)
        {
            m_CurUseEntity.TranstionMultiStates(this, 1, _selectType);
            return;
        }
    }
}