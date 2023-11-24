using System.Collections.Generic;
using UnityEngine;

namespace Fusion.XR.Shared
{
    /**
     * Created line renderer and primitives to visual display the position of this objects's transform during certain phases.
     * 
     * If hideLinesAtCreation/hidePrimitivesAtCreation are true at start, in the inspector
     * use "Toggle display line" / "Toggle display primitives" buttons
     * 
     */
    public class PositionTracker : NetworkBehaviour
    {
        Dictionary<string, DebugRoot> roots = new Dictionary<string, DebugRoot>();

        public bool hideLinesAtCreation = true;
        public bool hidePrimitivesAtCreation = true;

        [System.Flags]
        public enum LoggedState
        {
            None = 0,
            FUNForward = 1,
            FUNResim = 2,
            Render = 4,
            RenderInterpolationTarget = 8,
            FUNFirstResim = 32,
            FUN = 64,
        }
        public LoggedState stateAuthorityLoggedStates = LoggedState.None;
        public LoggedState othersLoggedStates = LoggedState.None;

        [System.Serializable]
        public struct MaterialSettings
        {
            public Material lineMaterial;
            public Material primitiveMaterial;
            public Material stateAuthLineMaterial;
            public Material stateAuthPrimitiveMaterial;
        }

        [Header("Default material settings")]
        public MaterialSettings defaultMaterialSettings;
        [Header("Phase specific material settings")]
        public MaterialSettings renderMaterialSettings;
        public MaterialSettings renderFromMaterialSettings;
        public MaterialSettings renderToMaterialSettings;
        public MaterialSettings funForwardMaterialSettings;
        public MaterialSettings funResimMaterialSettings;
        public MaterialSettings funFirstResimMaterialSettings;

        public float scale = 0.001f;
        public void SelectMaterial(MaterialSettings settings, out Material lineMaterial, out Material primitiveMaterial)
        {
            lineMaterial = Object.HasStateAuthority ?
                (settings.stateAuthLineMaterial != null ? settings.stateAuthLineMaterial : defaultMaterialSettings.stateAuthLineMaterial)
                : (settings.lineMaterial != null ? settings.lineMaterial : defaultMaterialSettings.lineMaterial);

            primitiveMaterial = Object.HasStateAuthority ?
                (settings.stateAuthPrimitiveMaterial != null ? settings.stateAuthPrimitiveMaterial : defaultMaterialSettings.stateAuthPrimitiveMaterial)
                : (settings.primitiveMaterial != null ? settings.primitiveMaterial : defaultMaterialSettings.primitiveMaterial);
        }

        NetworkTransform networkTransform;

        private void Awake()
        {
            networkTransform = GetComponent<NetworkTransform>();
        }

        public override void Spawned()
        {
            base.Spawned();
        }

        Vector3 previousRBPos;

        bool ShouldLogState(LoggedState phase)
        {
            if (Object.HasStateAuthority)
            {
                return (stateAuthorityLoggedStates & phase) != 0;
            }
            else
            {
                return (othersLoggedStates & phase) != 0;
            }
        }

        public override void FixedUpdateNetwork()
        {
            base.FixedUpdateNetwork();

            if (ShouldLogState(LoggedState.FUN))
            {
                if (Runner.IsForward)
                {
                    SelectMaterial(funForwardMaterialSettings, out var lineMaterial, out var primitiveMaterial);
                    CreateDebugPoint(transform.position, PrimitiveType.Cube, $"[Forward] {Runner.Tick}", $"FUN-{name}-{Object.Id}", lineMaterial, primitiveMaterial);
                }
                else
                {
                    SelectMaterial(funResimMaterialSettings, out var lineMaterial, out var primitiveMaterial);
                    CreateDebugPoint(transform.position, PrimitiveType.Cylinder, $"{((Runner.IsFirstTick) ? "(FirstResim)" : "(Resim)")}{Runner.Tick}", $"FUN-{name}-{Object.Id}", lineMaterial, primitiveMaterial);
                }
            }

            if (Runner.IsForward)
            {
                if (ShouldLogState(LoggedState.FUNForward))
                {
                    SelectMaterial(funForwardMaterialSettings, out var lineMaterial, out var primitiveMaterial);
                    CreateDebugPoint(transform.position, PrimitiveType.Cube, $"[Forward] {Runner.Tick}", $"(Forward)FUN-{name}-{Object.Id}", lineMaterial, primitiveMaterial);
                }
            }
            else
            {
                if (Runner.IsFirstTick && ShouldLogState(LoggedState.FUNFirstResim))
                {
                    SelectMaterial(funFirstResimMaterialSettings, out var lineMaterial, out var primitiveMaterial);
                    CreateDebugPoint(transform.position, PrimitiveType.Cylinder, $"[FirstResim] {Runner.Tick}", $"(FirstResim)-FUN-{name}-{Object.Id}", lineMaterial, primitiveMaterial);
                }
                if (ShouldLogState(LoggedState.FUNResim))
                {
                    SelectMaterial(funResimMaterialSettings, out var lineMaterial, out var primitiveMaterial);
                    CreateDebugPoint(transform.position, PrimitiveType.Cylinder, $"{((Runner.IsFirstTick) ? "(FirstResim)" : "(Resim)")}{Runner.Tick}", $"(Resim)FUN-{name}-{Object.Id}", lineMaterial, primitiveMaterial);
                }
            }
        }

        public override void Render()
        {
            base.Render();

            if (ShouldLogState(LoggedState.Render))
            {
                SelectMaterial(renderMaterialSettings, out var lineMaterialRender, out var primitiveMaterialRender);
                CreateDebugPoint(transform.position, PrimitiveType.Sphere, $"{Time.time}", $"Render-{name}-{Object.Id}", lineMaterialRender, primitiveMaterialRender);
            }
            if (ShouldLogState(LoggedState.RenderInterpolationTarget) && networkTransform)
            {
                SelectMaterial(renderMaterialSettings, out var lineMaterialRender, out var primitiveMaterialRender);
                CreateDebugPoint(networkTransform.InterpolationTarget.position, PrimitiveType.Sphere, $"{Time.time}", $"Render-{name}-{Object.Id}", lineMaterialRender, primitiveMaterialRender);
            }
        }

        void CreateDebugPoint(Vector3 pos, PrimitiveType type, string n, string rootName, Material lineMaterial, Material primitiveMaterial)
        {
            var root = DebugRoot.Find(roots, rootName, lineMaterial, primitiveMaterial, hideLinesAtCreation, hidePrimitivesAtCreation);
            root.scale = scale;
            if (Runner.Config.PeerMode == NetworkProjectConfig.PeerModes.Multiple)
            {
                UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(root.gameObject, gameObject.scene);
            }
            root.primitiveType = type;
            root.AddPoint(pos, n);
        }

        [BehaviourButtonAction("Toggle display line")]
        public void ToggleDisplayLine()
        {
            foreach (var root in roots.Values) root.ToggleDisplayLine();
        }

        [BehaviourButtonAction("Toggle display primitives")]
        public void ToggleDisplayPrimitives()
        {
            foreach (var root in roots.Values) root.ToggleDisplayPrimitives();
        }


        [BehaviourButtonAction("Reset points")]
        public void ResetPoints()
        {
            foreach (var root in roots.Values) root.ResetPoints();

        }
    }
}
