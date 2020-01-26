using UnityEngine;

[ExecuteAlways]
public class FX_ShaderVariableSlider : MonoBehaviour
{
    public bool isEditor = false;
    public GameObject obj;
    [Range(0, 1)]
    public float value = 1;
    public string shaderVarName;

    private int i;
    private Renderer[] rends;

    void Update()
    {
        if (Application.isPlaying) TransmitToShader();
        if (isEditor) TransmitToShaderEditor();
    }

    void TransmitToShader()
    {
        rends = obj.GetComponentsInChildren<Renderer>();
        for (i = 0; i < (rends.Length); i++)
            rends[i].material.SetFloat(shaderVarName, value);
    }

    void TransmitToShaderEditor()
    {
        rends = obj.GetComponentsInChildren<Renderer>();
        for (i = 0; i < (rends.Length); i++)
            rends[i].sharedMaterial.SetFloat(shaderVarName, value);
    }
}