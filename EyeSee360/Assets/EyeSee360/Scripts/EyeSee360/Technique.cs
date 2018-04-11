/*
 * MIT License
 * 
 * Copyright (c) 2017 Uwe Gruenefeld, Dag Ennenga, Dana Hsiao, Yvonne Brueck
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

namespace Gruenefeld.OutOfView.EyeSee360
{
	public class Technique : Core.Technique
	{
        [Header("General")]
        public Vector2          space = new Vector2(360, 180);

        [Header("Field of view")]
        public EnumFOV          fov = EnumFOV.Camera;
        public Vector2          fovCustomSize = new Vector2(180, 90);
        public EnumShape        fovCustomShape = EnumShape.ELLIPSE;

        [Header("Rotation")]
		public bool             roll = false;
		public bool             pitch = true;
		public bool             yaw = true;		// TODO implement

        [Header("Compression")]
		public EnumCompression  compressX = EnumCompression.None;
		public EnumCompression  compressY = EnumCompression.None;

        [Header("Outer Area")]
		public EnumFOV          outerSize = EnumFOV.Camera;
		public Vector2          outerSizeCustom = new Vector2(10, 10);
		public float            outerLineWidth = 0.06f;
		public Color            outerLineColor = Color.black;

        [Header("Inner Area")]
        public float            innerLineWidth = 0.06f;
        public Color            innerLineColor = Color.black;

        [Header("Zeroline")]
		public bool             zerolineVertical = true;
		public bool             zerolineHorizontal = true;
		public float            zerolineWidth = 0.06f;
		public Color 			zerolineColor = Color.black;

        [Header("Helpline")]
		public int 				helplineVertical = 45;
		public int				helplineHorizontal = 45;
		public float            helplineWidth = 0.06f;
		public Color 			helplineColor = Color.black;

        [Header("Proxy")]
		public float            proxySize = 5f;
        public Color            proxyColor = Color.blue;

        [Header("Distance")]
        public float            distanceMin = 1;
        public float            distanceMax = 10;
        public bool             distanceColor = true;
		public Color            distanceMinColor = Color.red;
		public Color            distanceMaxColor = Color.blue;
		public bool             distanceSize = false;
		public float            distanceMinSize = 1f;
		public float            distanceMaxSize = 10f;

        public override void Start()
        {
            base.Start();

            this.gameObject.name = "EyeSee";
            this.area = new Area(this);
        }

        public override void Update()
        {
            if(this.proxies != null)
                foreach (Proxy proxy in this.proxies)
                    Destroy(proxy.proxy);
            this.proxies = new Proxy[this.targets.Length];

            for (int i = 0; i < this.targets.Length; i++)
                this.proxies[i] = new Proxy(this, this.targets[i]);

            base.Update();
        }
    }
}