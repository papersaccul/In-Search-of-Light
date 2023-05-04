using UnityEngine;
using UnityEngine.Rendering;

public class SortingLayerTexture : MonoBehaviour
{
    private RenderTexture rt;

    void Awake()
    {
        rt = new RenderTexture(Screen.width, Screen.height, 0, RenderTextureFormat.Default);
        rt.Create();
    }

    void OnRenderObject()
    {
        Graphics.Blit(null, rt);
    }

    void OnDestroy()
    {
        RenderTexture.ReleaseTemporary(rt);
    }
}