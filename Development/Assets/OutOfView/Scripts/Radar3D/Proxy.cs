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
    public class Proxy : Core.Proxy
    {
        private Technique radar3d;

        private LineRenderer line;

        public Proxy(Technique technique, GameObject target) : base(technique, target)
        {
            this.radar3d = (Technique)technique;

            MonoBehaviour.Destroy(this.proxy);

            this.proxy = GameObject.CreatePrimitive(PrimitiveType.Cube);
            this.proxy.name = "Proxy";
            //this.proxy.transform.parent = this.radar3d.area.area.transform;
            this.proxy.transform.parent = this.radar3d.transform;
            this.proxy.transform.position = Vector3.zero;

            MeshRenderer meshRenderer = this.proxy.GetComponent<MeshRenderer>();
            meshRenderer.material = new Material(Shader.Find("Standard"));
            meshRenderer.material.color = this.radar3d.proxyColor;

            GameObject lineObject = new GameObject("Line");
            lineObject.transform.parent = this.proxy.transform;
            lineObject.transform.localPosition = Vector3.zero;
            lineObject.transform.rotation = Quaternion.Euler(180, 0, 0);

            this.line = lineObject.AddComponent<LineRenderer>();
            line.startColor = this.radar3d.lineColor;
            line.endColor = this.radar3d.lineColor;
            line.startWidth = this.radar3d.lineWidth;
            line.endWidth = this.radar3d.lineWidth;
            line.useWorldSpace = false;
            line.SetPosition(0, Vector3.zero);
            line.SetPosition(1, Vector3.zero);
            line.positionCount = 2;
            line.material = new Material(Shader.Find("Unlit/Color"));
            line.material.color = this.radar3d.lineColor;
        }

        protected override void UpdatePosition()
        {
            Vector3 position = this.target.transform.position;
            position = Camera.main.transform.InverseTransformPoint(position);

            if (position.magnitude > this.radar3d.distanceMax)
                position = (position / position.magnitude) * this.radar3d.distanceMax;

            position.x = position.x * (this.radar3d.scaleX / this.radar3d.distanceMax) + this.radar3d.offsetX;
            position.y = position.y * (this.radar3d.scaleY / this.radar3d.distanceMax) + this.radar3d.offsetY;
            position.z = position.z * (this.radar3d.scaleZ / this.radar3d.distanceMax) + this.radar3d.offsetZ;

            this.proxy.transform.localPosition = position;

            line.SetPosition(1, new Vector3(0, (this.proxy.transform.localPosition.y - this.radar3d.offsetY) * 5, 0));
        }

        protected override void UpdateRotation()
        {
            // Nothing to do here
        }

        protected override void UpdateScale()
        {
            this.proxy.transform.localScale = new Vector3(
                this.radar3d.proxySize,
                this.radar3d.proxySize,
                this.radar3d.proxySize
            );
        }

        protected override void UpdateColor()
        {
            // Nothing to do here
        }
    }
}
