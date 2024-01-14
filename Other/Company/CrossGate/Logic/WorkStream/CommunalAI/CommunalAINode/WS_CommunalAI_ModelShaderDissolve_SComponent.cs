using UnityEngine.Rendering;

[StateComponent((int)StateCategoryEnum.CommunalAI, (int)CommunalAIEnum.ModelShaderDissolve)]
public class WS_CommunalAI_ModelShaderDissolve_SComponent : StateBaseComponent
{
	public override void Init(string str)
	{
        WS_CommunalAIManagerEntity communalAIManagerEntity = ((BaseStreamControllerEntity)m_CurUseEntity.m_StateControllerEntity).m_WorkStreamManagerEntity as WS_CommunalAIManagerEntity;
        if (communalAIManagerEntity.m_ShaderControllerMaterialInfoList != null)
        {
            float[] ps = CombatHelp.GetStrParseFloat1Array(str);
            bool isOpenDissolve = ps[0] == 1;
            for (int i = 0, len = communalAIManagerEntity.m_ShaderControllerMaterialInfoList.Count; i < len; i++)
            {
                ShaderControllerMaterialInfo shaderControllerMaterialInfo = communalAIManagerEntity.m_ShaderControllerMaterialInfoList[i];
                if (shaderControllerMaterialInfo == null || shaderControllerMaterialInfo.Mat == null)
                    continue;
                
                RenderQueue renderQueue;
                if (shaderControllerMaterialInfo.OriginRenderQueue > (int)RenderQueue.GeometryLast)
                {
                    renderQueue = 0;
                }
                else
                {
                    renderQueue = isOpenDissolve ? (ps[1] == 1 ? RenderQueue.Transparent : RenderQueue.AlphaTest) :
                                    ((RenderQueue)shaderControllerMaterialInfo.OriginRenderQueue);
                }

                ShaderController.Instance.ShowDissolveBloom(shaderControllerMaterialInfo.Mat, isOpenDissolve, ps[2], ps[3], ps[4] + (ps.Length > 5 ? 100 * (int)ps[5] : 0),
                    renderQueue, shaderControllerMaterialInfo.OriginOutlineState, ps.Length > 6 ? ps[6] : 0f, ps.Length > 7 ? ps[7] : 0f);
            }
        }

        m_CurUseEntity.TranstionMultiStates(this);
    }
}