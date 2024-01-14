using Logic.Core;

namespace Logic
{
    /// <summary>
    /// 交互事件数据///
    /// </summary>
    public class InteractiveEvtData
    {
        public EInteractiveAimType eInteractiveAimType;
        public ISceneActor sceneActor;
        public bool immediately;
        public object data;
    }
}
