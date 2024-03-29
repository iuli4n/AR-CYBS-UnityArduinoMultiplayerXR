﻿// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Unity.Profiling;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input
{
    //[AddComponentMenu("Scripts/MRTK/SDK/SpherePointerGrabPoint")]
    public class IuliStickPointerGrabPoint : MonoBehaviour
    {
        [SerializeField]
        private SpherePointerVisual pointerVisual;
        [SerializeField]
        private Mesh grabPointMesh = null;
        [SerializeField]
        private Material grabPointMaterial = null;
        [SerializeField]
        private float scale = 1f;

        private Matrix4x4 pointMatrix;

        private void OnEnable()
        {
            if (pointerVisual == null)
            {
                pointerVisual = GetComponent<SpherePointerVisual>();
                if (pointerVisual == null)
                {
                    enabled = false;
                }
            }
        }

        private static readonly ProfilerMarker LateUpdatePerfMarker = new ProfilerMarker("[MRTK] SpherePointerGrabPoint.LateUpdate");

        private void LateUpdate()
        {
            using (LateUpdatePerfMarker.Auto())
            {
                if (pointerVisual.TetherVisualsEnabled)
                {
                    pointMatrix = Matrix4x4.TRS(pointerVisual.TetherEndPoint.position, pointerVisual.TetherEndPoint.rotation, Vector3.one * scale);
                    Graphics.DrawMesh(grabPointMesh, pointMatrix, grabPointMaterial, pointerVisual.gameObject.layer);
                }
            }
        }
    }
}