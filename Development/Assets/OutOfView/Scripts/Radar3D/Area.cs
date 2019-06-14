/*
 * MIT License
 * 
 * Copyright (c) 2019 Uwe Gruenefeld, Ilja Köthe
 * University of Oldenburg (GERMANY)
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy 
 * of this software and associated documentation files (the "Software"), to deal 
 * in the Software without restriction, including without limitation the rights 
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is 
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */
using Gruenefeld.OutOfView.Core;
using UnityEngine;

namespace Gruenefeld.OutOfView.Radar3D
{
    public class Area : Core.Area
    {
        private Technique radar3d;

        private GameObject outer;
        private GameObject ground;
        private GameObject center;
        private GameObject zeroline;

        public Area(Technique technique) : base(technique)
        {
            this.radar3d = (Technique) technique;

            // Initialize area
            this.area.name = "Area";
            this.area.transform.localPosition = new Vector3(
                this.radar3d.offsetX,
                this.radar3d.offsetY,
                this.radar3d.offsetZ
            );

            MonoBehaviour.Destroy(this.area.GetComponent<SphereCollider>());

            // Initialize outer
            this.outer = new GameObject("Outer");
            this.outer.transform.parent = this.area.transform;
            this.outer.transform.localPosition = Vector3.zero;
            this.outer.transform.localRotation = Quaternion.identity;
            Utility.AddEllipseLine(
                this.outer, 
                new Vector2(this.radar3d.scaleX, this.radar3d.scaleZ),
                this.radar3d.outerLineColor,
                width: this.radar3d.outerLineWidth
            );
            Utility.SetColorAttribute(this.outer, this.radar3d.outerLineColor);

            // Initialize ground
            this.ground = new GameObject("Ground");
            this.ground.transform.parent = this.area.transform;
            this.ground.transform.localPosition = Vector3.zero;
            this.ground.transform.localRotation = Quaternion.Euler(180, 0, 0);
            this.ground.transform.localScale = new Vector3(this.radar3d.scaleX * 2, this.radar3d.scaleZ * 2, 1);
            Utility.AddCircleMesh(this.ground);

            MeshRenderer meshRenderer = this.ground.GetComponent<MeshRenderer>();
            meshRenderer.material = new Material(Shader.Find("Standard"));
            meshRenderer.material.color = this.radar3d.groundColor;
            meshRenderer.material.SetFloat("_Mode", 2);
            meshRenderer.material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            meshRenderer.material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            meshRenderer.material.SetInt("_ZWrite", 0);
            meshRenderer.material.DisableKeyword("_ALPHATEST_ON");
            meshRenderer.material.EnableKeyword("_ALPHABLEND_ON");
            meshRenderer.material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            meshRenderer.material.renderQueue = 3000;

            // Initialize center
            this.center = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            this.center.name = "Center";
            this.center.transform.parent = this.area.transform;
            this.center.transform.localPosition = Vector3.zero;
            this.center.transform.localScale = new Vector3(
                this.radar3d.centerSize,
                this.radar3d.centerSize,
                this.radar3d.centerSize
            );

            meshRenderer = this.center.GetComponent<MeshRenderer>();
            meshRenderer.material = new Material(Shader.Find("Standard"));
            meshRenderer.material.color = this.radar3d.centerColor;

            // Initialize zerolines
            this.zeroline = new GameObject("Zeroline");
            this.zeroline.transform.parent = this.area.transform;
            this.zeroline.transform.localPosition = Vector3.zero;

            if (this.radar3d.zerolineHorizontal)
            {
                GameObject horizontal = new GameObject("Horizontal");
                horizontal.transform.parent = this.area.transform;
                horizontal.transform.localPosition = Vector3.zero;

                LineRenderer lineRenderer = horizontal.AddComponent<LineRenderer>();
                lineRenderer.startColor = this.radar3d.zerolineColor;
                lineRenderer.endColor = this.radar3d.zerolineColor;
                lineRenderer.startWidth = this.radar3d.zerolineWidth;
                lineRenderer.endWidth = this.radar3d.zerolineWidth;
                lineRenderer.useWorldSpace = false;
                lineRenderer.SetPosition(0, new Vector3(-this.radar3d.scaleX, 0, 0));
                lineRenderer.SetPosition(1, new Vector3( this.radar3d.scaleX, 0, 0));
                lineRenderer.positionCount = 2;
                lineRenderer.material = new Material(Shader.Find("Custom/Line"));
                lineRenderer.material.color = this.radar3d.zerolineColor;
                lineRenderer.material.SetFloat("_RepeatCount", 10);
            }

            if (this.radar3d.zerolineVertical)
            {
                GameObject vertical = new GameObject("Vertical");
                vertical.transform.parent = this.area.transform;
                vertical.transform.localPosition = Vector3.zero;

                LineRenderer lineRenderer = vertical.AddComponent<LineRenderer>();
                lineRenderer.startColor = this.radar3d.zerolineColor;
                lineRenderer.endColor = this.radar3d.zerolineColor;
                lineRenderer.startWidth = this.radar3d.zerolineWidth;
                lineRenderer.endWidth = this.radar3d.zerolineWidth;
                lineRenderer.useWorldSpace = false;
                lineRenderer.SetPosition(0, new Vector3(0, -this.radar3d.scaleZ, 0));
                lineRenderer.SetPosition(1, new Vector3(0,  this.radar3d.scaleZ, 0));
                lineRenderer.positionCount = 2;
                lineRenderer.material = new Material(Shader.Find("Custom/Line"));
                lineRenderer.material.color = this.radar3d.zerolineColor;
                lineRenderer.material.SetFloat("_RepeatCount", 10);
            }

            this.area.transform.rotation = Quaternion.Euler(this.radar3d.rotation, 0, 0);
        }

        public override void Update() 
        {
            // Nothing to do here
        }
    }
}
