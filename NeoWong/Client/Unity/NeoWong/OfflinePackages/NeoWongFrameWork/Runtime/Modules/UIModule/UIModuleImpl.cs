using System.Collections.Generic;

namespace NWFramework
{
    [UpdateModule]
    internal sealed partial class UIModuleImpl : ModuleImp
    {
        private List<UIWindow> _stack;

        internal void Initialize(List<UIWindow> stack)
        {
            _stack = stack;
        }

        internal override void Shutdown()
        {

        }

        internal override void Update(float elapseSeconds, float realElapseSeconds)
        {
            if (_stack == null)
            {
                return;
            }

            int count = _stack.Count;
            for (int i = 0; i < count; i++)
            {
                if (_stack.Count != count)
                {
                    break;
                }

                var window = _stack[i];
                window.InternalUpdate();
            }
        }
    }
}