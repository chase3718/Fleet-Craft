using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace INab.AdvancedEdgeDetection.URP
{
    public class DepthMaskPass : ScriptableRenderPass
    {
        public readonly RenderTargetHandle DepthMaskRT;
        private readonly Material m_Material;

        private readonly List<ShaderTagId> shaderTagIdList;
        private FilteringSettings filteringSettings;

        public DepthMaskPass(RenderPassEvent renderPassEvent, int layerMask)
        {
            this.renderPassEvent = renderPassEvent;

            filteringSettings = new FilteringSettings(RenderQueueRange.all, layerMask);

            m_Material = CoreUtils.CreateEngineMaterial(Shader.Find("Shader Graphs/DepthMask"));

            shaderTagIdList = new List<ShaderTagId>()
            {
                new ShaderTagId("UniversalForward"),
                new ShaderTagId("UniversalForwardOnly"),
                new ShaderTagId("LightweightForward"),
                new ShaderTagId("SRPDefoultUnlit"),
                new ShaderTagId("DepthOnly"),
                new ShaderTagId("UniversalGBuffer"),
                new ShaderTagId("DepthNormalsOnly"),
                new ShaderTagId("Universal2D"),
                new ShaderTagId("SRPDefaultUnlit"),
            };

            DepthMaskRT.Init("_DepthMaskRT");
        }

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            RenderTextureDescriptor textureDescriptor = cameraTextureDescriptor;
            textureDescriptor.colorFormat = RenderTextureFormat.RFloat;
            textureDescriptor.msaaSamples = 1;

            cmd.GetTemporaryRT(DepthMaskRT.id, textureDescriptor, FilterMode.Point);
            ConfigureTarget(DepthMaskRT.Identifier());
            ConfigureClear(ClearFlag.All, Color.black);
        }


        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (!m_Material)
                return;

            CommandBuffer cmd = CommandBufferPool.Get();
            using (new ProfilingScope(cmd, new ProfilingSampler(
                "Edge Detection Depth Mask")))
            {
                DrawingSettings drawingSettings = CreateDrawingSettings(shaderTagIdList, ref renderingData, renderingData.cameraData.defaultOpaqueSortFlags);
                drawingSettings.overrideMaterial = m_Material;
                drawingSettings.enableDynamicBatching = true;

                context.DrawRenderers(renderingData.cullResults, ref drawingSettings, ref filteringSettings);
            }

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public override void FrameCleanup(CommandBuffer cmd)
        {
            cmd.ReleaseTemporaryRT(DepthMaskRT.id);
        }
    }
}