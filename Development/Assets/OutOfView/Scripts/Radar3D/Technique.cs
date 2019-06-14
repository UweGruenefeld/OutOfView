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
using System;

namespace Gruenefeld.OutOfView.Radar3D
{
    public class Technique : Core.Technique
    {
        public float    offsetX = 0f;
        public float    offsetY = -1.4f;
        public float    offsetZ = -6f;

        public float    rotation = 90f;

        public float    scaleX = 2f;
        public float    scaleY = 2f;
        public float    scaleZ = 1f;

        public Color    groundColor = new Color(1, 1, 1, .1f);

        public float    centerSize = .3f;
        public Color    centerColor = new Color(.9f, .9f, .9f);

        public float    outerLineWidth = 0.02f;
        public Color    outerLineColor = Color.white;

        public bool     zerolineVertical = true;
        public bool     zerolineHorizontal = true;
        public float    zerolineWidth = 0.02f;
        public Color    zerolineColor = Color.white;

        public float    proxySize = .2f;
        public Color    proxyColor = new Color(.9f, .9f, .9f);

        public Color    lineColor = Color.white;
        public float    lineWidth = 0.02f;

        public float    distanceMax = 10f;

        public override void Start()
        {
            base.Start();

            this.gameObject.name = "Radar3D";
            this.area = new Area(this);
        }

        public override void Update()
        {
            if (this.proxies == null)
            {
                this.proxies = new Proxy[this.targets.Length];
                for (int i = 0; i < this.targets.Length; i++)
                    this.proxies[i] = new Proxy(this, this.targets[i]);
            }

            base.Update();
        }
    }
}
