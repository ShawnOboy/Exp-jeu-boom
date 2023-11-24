using System.Collections.Generic;
using UnityEngine;

namespace Fusion.XR.Shared
{
    public class DebugRoot : MonoBehaviour
    {
        public GameObject primitiveRoot;
        public PrimitiveType primitiveType = PrimitiveType.Cube;
        public LineRenderer lineRenderer;
        public List<Vector3> points = new List<Vector3>();
        public List<string> pointDescriptions = new List<string>();
        public Material lineMaterial;
        public Material primitiveMaterial;
        public Dictionary<int, GameObject> primitives = new Dictionary<int, GameObject>();
        int lastCreatedprimitives = -1;

        bool isLineDisplayed = false;
        bool isPrimitiveRootsDisplayed = false;

        public float scale = 0.001f;

        public static DebugRoot Find(Dictionary<string, DebugRoot> roots, string name, Material lineMaterial, Material primitiveMaterial, bool hideLinesAtCreation = false, bool hidePrimitivesAtCreation = false)
        {
            if (roots.ContainsKey(name)) return roots[name];

            var rootGO = new GameObject("name");
            var root = rootGO.AddComponent<DebugRoot>();
            root.name = name;
            root.primitiveMaterial = primitiveMaterial;
            root.lineMaterial = lineMaterial;

            if (!hideLinesAtCreation) root.ToggleDisplayLine();
            if (!hidePrimitivesAtCreation) root.ToggleDisplayPrimitives();

            roots.Add(name, root);
            return root;
        }

        private void Start()
        {
            RefreshLine();
            RefreshPrimitives();
        }

        public void ToggleDisplayLine()
        {
            DisplayLine(!isLineDisplayed);
            isLineDisplayed = !isLineDisplayed;

        }

        public void RefreshLine()
        {
            DisplayLine(isLineDisplayed);
        }

        public void AddPoint(Vector3 pos, string description = null)
        {
            if (description == null) description = $"{points.Count}";

            points.Add(pos);
            pointDescriptions.Add(description);

            RefreshLine();
            RefreshPrimitives();
        }

        public void DisplayLine(bool visible)
        {
            if (isLineDisplayed && visible == false && lineRenderer)
            {
                Debug.Log("Hiding line for " + name);
                lineRenderer.gameObject.SetActive(false);
            }
            if (isLineDisplayed == false && visible)
            {
                Debug.Log("Displaying line for " + name);

                if (!lineRenderer)
                {
                    var lineRendererRoot = new GameObject($"{name}-Linerenderer");
                    lineRendererRoot.transform.parent = transform;

                    lineRenderer = lineRendererRoot.AddComponent<LineRenderer>();
                    lineRenderer.useWorldSpace = true;
                    lineRenderer.startWidth = scale;
                    lineRenderer.endWidth = scale;
                    lineRenderer.material = lineMaterial;
                }
                lineRenderer.gameObject.SetActive(true);
            }

            if (visible && points.Count != lineRenderer.positionCount)
            {
                lineRenderer.positionCount = points.Count;
                lineRenderer.SetPositions(points.ToArray());
            }
        }

        float lastScale = -1;
        private void Update()
        {
            if (scale != lastScale)
            {
                if (lineRenderer)
                {
                    lineRenderer.startWidth = scale;
                    lineRenderer.endWidth = scale;
                }
                foreach (var p in primitives.Values)
                {
                    p.transform.localScale = scale * Vector3.one;
                }
            }
        }

        public void ToggleDisplayPrimitives()
        {
            DisplayPrimitives(!isPrimitiveRootsDisplayed);
            isPrimitiveRootsDisplayed = !isPrimitiveRootsDisplayed;
        }
        public void RefreshPrimitives()
        {
            DisplayPrimitives(isPrimitiveRootsDisplayed);
        }

        public void DisplayPrimitives(bool visible)
        {
            if (isPrimitiveRootsDisplayed && visible == false && primitiveRoot)
            {
                Debug.Log("Hidding primitives for " + name);
                primitiveRoot.SetActive(false);
            }
            if (isPrimitiveRootsDisplayed == false && visible)
            {
                Debug.Log("Displaying primitives for " + name);
                if (primitiveRoot == null)
                {
                    primitiveRoot = new GameObject($"{name}-Primitives");
                    primitiveRoot.transform.parent = transform;
                }
                primitiveRoot.SetActive(true);
            }

            if (visible && points.Count != primitives.Count)
            {
                for (int i = lastCreatedprimitives + 1; i < points.Count; i++)
                {
                    var pos = points[i];
                    var n = i < pointDescriptions.Count ? pointDescriptions[i] : null;
                    var p = Fusion.XR.Shared.DebugExtension.DebugPrimitive(pos, primitiveMaterial, primitiveType, n == null ? "" : n);
                    p.transform.localScale = scale * Vector3.one;
                    p.transform.parent = primitiveRoot.transform;
                    lastCreatedprimitives = i;
                    primitives.Add(i, p);
                }
            }
        }


        public void ResetPoints()
        {
            points.Clear();
            pointDescriptions.Clear();
            foreach (var primitive in primitives.Values) Destroy(primitive);
            primitives.Clear();
            if (lineRenderer)
            {
                lineRenderer.positionCount = points.Count;
                lineRenderer.SetPositions(points.ToArray());
            }
        }
    }
}

