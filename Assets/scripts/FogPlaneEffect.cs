using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[ExecuteInEditMode]
public class FogPlaneEffect : MonoBehaviour
{
    public BlitFeatureRenderer.BlitSettings blitSettings;

    private BlitFeatureRenderer.BlitPass blitPass;

    private Material material;

    void OnEnable()
    {
        if (blitSettings.blitMaterial != null)
        {
            material = new Material(blitSettings.blitMaterial);
            material.SetFloat("_FogY", transform.position.y);

            blitPass = new BlitFeatureRenderer.BlitPass(blitSettings.Event, blitSettings, material, name);

            RenderPipelineManager.beginCameraRendering += Render;
        }
    }
    
    void OnDisable()
    {
        RenderPipelineManager.beginCameraRendering -= Render;
        material = null;
        blitPass = null;
    }

    void Render(ScriptableRenderContext context, Camera camera)
    {
        if (camera != Camera.main && camera != UnityEditor.SceneView.lastActiveSceneView?.camera) return;

        var renderer = camera.GetUniversalAdditionalCameraData().scriptableRenderer;

        blitPass.Setup(renderer);
        renderer.EnqueuePass(blitPass);
    }

    void Start()
    {
        if (material != null)
        {
            material.SetFloat("_FogY", transform.position.y);
            Shader.SetGlobalFloat("_FogY", transform.position.y);
        }
    }

    void Update()
    {
        if (material != null)
        {
            if (!Application.isPlaying)
            {
                material.CopyPropertiesFromMaterial(blitSettings.blitMaterial);
            }

            material.SetFloat("_FogY", transform.position.y);
            Shader.SetGlobalFloat("_FogY", transform.position.y);
        }
    }
}
