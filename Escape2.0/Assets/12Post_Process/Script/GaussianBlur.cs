using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GaussianBlur : PostEffectsBase
{
    public Shader gaussianBlurShader;
    private Material gaussianBlurMaterial = null;
    public Material material
    {
        get 
        {
            gaussianBlurMaterial = CheckShaderAndCreateMaterial(gaussianBlurShader, gaussianBlurMaterial);
            return gaussianBlurMaterial;
        }
    }
    [Range(0,4)]
    public int iterations = 3;
    [Range(0.2f, 3.0f)]
    public float blurSpread = 0.6f;
    [Range(1, 8)]
    public int downSample = 2;

    //1st edition: just apply blur
    /*void OnRenderImage(RenderTexture src, RenderTexture dest) 
    {
        if(material != null)
        {
            int rtW = src.width;
            int rtH = src.height;
            RenderTexture buffer = RenderTexture.GetTemporary(rtW, rtH, 0);

            //渲染垂直通道
            Graphics.Blit(src, buffer, material, 0);
            //渲染水平通道
            Graphics.Blit(buffer, dest, material, 1);

            RenderTexture.ReleaseTemporary(buffer);
        }
        else
        {
            Graphics.Blit(src, dest);
        }
    }*/

    //2nd edition: scale the render texture
    /*void OnRenderImage(RenderTexture src, RenderTexture dest) 
    {
        if(material != null)
        {
            int rtW = src.width/downSample;
            int rtH = src.height/downSample;
            RenderTexture buffer = RenderTexture.GetTemporary(rtW, rtH, 0);
            buffer.filterMode = FilterMode.Bilinear;

            //渲染垂直通道
            Graphics.Blit(src, buffer, material, 0);
            //渲染水平通道
            Graphics.Blit(buffer, dest, material, 1);
            
            RenderTexture.ReleaseTemporary(buffer);
        }
        else
        {
            Graphics.Blit(src, dest);
        }
    }*/

    //3rd edition: use iteration for larger blur
    void OnRenderImage(RenderTexture src, RenderTexture dest) 
    {
        if(material != null)
        {
            int rtW = src.width/downSample;
            int rtH = src.height/downSample;

            RenderTexture buffer0 = RenderTexture.GetTemporary(rtW, rtH, 0);
            buffer0.filterMode = FilterMode.Bilinear;

            Graphics.Blit(src, buffer0);

            //其他相同，对buffer进行迭代
            for(int i = 0; i < iterations; i++)
            {
                material.SetFloat("_BlurSize", 1.0f + i * blurSpread);

                RenderTexture buffer1 = RenderTexture.GetTemporary(rtW, rtH, 0);

                //渲染垂直通道
                Graphics.Blit(buffer0, buffer1, material, 0);

                RenderTexture.ReleaseTemporary(buffer0);
                buffer0 = buffer1;
                buffer1 = RenderTexture.GetTemporary(rtW, rtH, 0);

                //渲染水平通道
                Graphics.Blit(buffer0, buffer1, material, 1);

                RenderTexture.ReleaseTemporary(buffer0);
                buffer0 = buffer1;
            }

            Graphics.Blit(buffer0, dest);
            RenderTexture.ReleaseTemporary(buffer0);
        }
        else
        {
            Graphics.Blit(src, dest);
        }
    }
}
