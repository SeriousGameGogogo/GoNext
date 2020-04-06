using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bloom : PostEffectsBase
{
    public Shader bloomShader;
    private Material bloomMaterial = null;
    public Material material
    {
        get
        {
            bloomMaterial = CheckShaderAndCreateMaterial(bloomShader, bloomMaterial);
            return bloomMaterial;
        }
    }

    //模糊迭代数——越大模糊越大
    [Range(0,4)]
    public int iterations = 3;

    //BlurSpread可以控制_BlurSize, 用于控制模糊大小
    [Range(0.2f, 3.0f)]
    public float blurSpread = 0.6f;

    //downSample是像素缩放大小
    [Range(1,8)]
    public int downSample = 2;

    //亮度域值，用于提取较亮区域的的域值大小，一般不超过1，但是开启HDR会超过1
    [Range(0.0f, 4.0f)]
    public float luminanceTreshold = 0.6f;

    void OnRenderImage(RenderTexture src, RenderTexture dest) 
    {
        if(material != null)
        {
            material.SetFloat("_LuminanceThreshold", luminanceTreshold);

            int rtW = src.width/downSample;
            int rtH = src.height/downSample;

            RenderTexture buffer0 = RenderTexture.GetTemporary(rtW, rtH, 0);
            buffer0.filterMode = FilterMode.Bilinear;

            Graphics.Blit(src, buffer0, material, 0);

            //开始迭代
            for(int i = 0; i < iterations; i++)
            {
                material.SetFloat("_BlurSize", 1.0f + i * blurSpread);

                RenderTexture buffer1 = RenderTexture.GetTemporary(rtW, rtH, 0);

                //Render the vertical pass
                Graphics.Blit(buffer0, buffer1, material, 1);

                RenderTexture.ReleaseTemporary(buffer0);
                buffer0 = buffer1;
                buffer1 = RenderTexture.GetTemporary(rtW, rtH, 0);

                //Render the horizontal pass
                Graphics.Blit(buffer0, buffer1, material, 2);

                RenderTexture.ReleaseTemporary(buffer0);
                buffer0 = buffer1;
            } 

            material.SetTexture ("_Bloom", buffer0);
            Graphics.Blit(src, dest, material, 3);

            RenderTexture.ReleaseTemporary(buffer0);
        }
        else
        {
            Graphics.Blit(src, dest);
        }
    }
}
