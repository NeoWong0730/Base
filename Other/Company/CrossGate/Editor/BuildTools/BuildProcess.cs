using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Build;
using UnityEditor.Rendering;
using UnityEngine.Rendering;

public class BuildProcess : IPreprocessShaders
{
    ShaderKeyword m_GlobalKeyword_xx;

    public int callbackOrder { get { return 0; } }
    public void OnProcessShader(Shader shader, ShaderSnippetData snippet, IList<ShaderCompilerData> data)
    {
        if(shader.name == "Universal Render Pipeline/Terrain/Lit")
        {
            data.Clear();
            Debug.LogFormat("Remove All Shader {0}", shader.name);
            return;
        }

        if (shader.name.StartsWith("Nature/"))
        {
            data.Clear();
            Debug.LogFormat("Remove All Shader {0}", shader.name);
            return;
        }

        if (shader.name.StartsWith("Standard"))
        {
            data.Clear();
            Debug.LogFormat("Remove All Shader {0}", shader.name);
            return;
        }

        for (int i = data.Count - 1; i >= 0; --i)
        {

        }
    }
}
