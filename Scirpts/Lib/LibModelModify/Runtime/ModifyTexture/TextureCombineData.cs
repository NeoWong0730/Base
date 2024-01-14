using System.Collections.Generic;
using UnityEngine.Rendering;

public interface ITextureCombine
{
    public void OnExecute(ScriptableRenderContext context, CommandBuffer cmd, TextureCombinePass textureCombinePass);
}

public static class TextureCombine
{
    internal static Queue<ITextureCombine> datas = new Queue<ITextureCombine>();

    public static void Add(ITextureCombine textureCombine)
    {
        datas.Enqueue(textureCombine);
    }
}