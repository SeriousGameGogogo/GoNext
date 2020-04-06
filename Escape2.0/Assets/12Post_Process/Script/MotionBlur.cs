
using System.Collections.Generic;
using UnityEngine;

public class MotionBlur : PostEffectsBase
{
    public Shader motionBlurShader;
    private Material motionBlurMaterial = null;

    public Material material
    {
        get
        {
            motionBlurMaterial = CheckShaderAndCreateMaterial(motionBlurShader,motionBlurMaterial);
            return motionBlurMaterial;
        }
    }

    [Range(0.0f, 0.9f)]
    public float blurAmount = 0.5f;

    private RenderTexture accumulationTexture;
    private RenderTexture accumulationTexture_1;
    void OnDisable() 
    {
        DestroyImmediate(accumulationTexture);
        DestroyImmediate(accumulationTexture_1);
    }

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if(material != null)
        {
            if(accumulationTexture == null || accumulationTexture.width != src.width || accumulationTexture.height != src.height)
            {
                DestroyImmediate(accumulationTexture);
                accumulationTexture = new RenderTexture(src.width, src.height, 0);
                accumulationTexture.hideFlags = HideFlags.HideAndDontSave;
                Graphics.Blit(src, accumulationTexture);
            }
                //无需提前清除或者丢弃的渲染纹理
                //accumulationTexture.MarkRestoreExpected();

                material.SetFloat("_BlurAmount", 1.0f - blurAmount);

                
                    Graphics.Blit(src, accumulationTexture, material);
                    Graphics.Blit(accumulationTexture, dest);
        }
        else
        {
            Graphics.Blit(src,dest);
        }
    }
}
