using UnityEngine;

[StateComponent((int)StateCategoryEnum.CombatBehaveAI, (int)CombatBehaveAIEnum.HideBossMesh)]
public class WS_CombatBehaveAI_HideBossMesh_SComponent : StateBaseComponent, IUpdate
{
    private float _totalTime;

    private float _time;

    private SkinnedMeshRenderer _smr;
    private Material[] _mats;
    private Color[] _cols;

    public override void Dispose()
    {
        _smr = null;
        if (_mats != null)
        {
            for (int i = 0, count = _mats.Length; i < count; i++)
            {
                Object.DestroyImmediate(_mats[i]);
            }
            _mats = null;
        }
        _cols = null;

        base.Dispose();
    }

    public override void Init(string str)
    {
        var combatBossComponent = CombatSceneEntity.Instance.GetNeedComponent<CombatBossComponent>();
        string[] strs = CombatHelp.GetStrParse1Array(str);

        _smr = combatBossComponent.GetMesh(strs[0]);
        if (_smr == null)
        {
            m_CurUseEntity.TranstionMultiStates(this);
            return;
        }
        
        _totalTime = float.Parse(strs[1]);
        if (_totalTime <= 0f)
        {
            _smr.gameObject.SetActive(false);
            m_CurUseEntity.TranstionMultiStates(this);
            return;
        }

        _time = Time.time + _totalTime;

        _mats = _smr.materials;
        _cols = new Color[_mats.Length];
        for (int i = 0, length = _mats.Length; i < length; i++)
        {
            var mat = _mats[i];
            if (mat == null)
                continue;

            mat.SetInt(ShaderController.SrcBlendId, 5);
            mat.SetInt(ShaderController.DstBlendId, 10);
            mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;

            _cols[i] = mat.GetColor(ShaderController.Instance.ShaderToInt_BaseColor);
        }
    }

    public void Update()
    {
        if (Time.time > _time)
        {
            _smr.gameObject.SetActive(false);
            m_CurUseEntity.TranstionMultiStates(this);
            return;
        }

        for (int i = 0, length = _mats.Length; i < length; i++)
        {
            var mat = _mats[i];
            if (mat == null)
                continue;

            _cols[i].a = (_time - Time.time) / _totalTime;
            mat.SetColor(ShaderController.Instance.ShaderToInt_BaseColor, _cols[i]);
        }
    }
}