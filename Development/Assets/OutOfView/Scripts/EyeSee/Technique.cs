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

namespace Gruenefeld.OutOfView.EyeSee
{
	public class Technique : Core.Technique
	{
        [Header("Space")]
        public Vector2          space = new Vector2(360, 180);

        [Header("Field of view")]
        public EnumFOV          fov = EnumFOV.Camera;
        public Vector2          fovCustomSize = new Vector2(180, 90);
        public EnumShape        fovCustomShape = EnumShape.ELLIPSE;

        [Header("Rotation")]
		public bool             roll = false;
		public bool             pitch = true;
        public bool             yaw = false;

        [Header("Compression")]
		public EnumCompression  compressX = EnumCompression.None;
		public EnumCompression  compressY = EnumCompression.None;
        [Range(1, 4)]
        public float            root = 2;

        [Header("Outer Area")]
        public bool             outer = true;
		public EnumFOV          outerSize = EnumFOV.Camera;
		public Vector2          outerSizeCustom = new Vector2(10, 10);
		public float            outerLineWidth = 0.01f;
		public Color            outerLineColor = Color.white;

        [Header("Inner Area")]
        public bool             inner = true;
        public float            innerLineWidth = 0.01f;
        public Color            innerLineColor = Color.white;

        [Header("Zeroline")]
		public bool             zerolineVertical = true;
		public bool             zerolineHorizontal = true;
		public float            zerolineWidth = 0.01f;
		public Color 			zerolineColor = Color.white;
        public bool             zerolineDotted = true;

        [Header("Helpline")]
        public bool             helplineVertical = true;
		public int 				helplineVerticalStep = 45;
        public bool             helplineHorizontal = true;
		public int				helplineHorizontalStep = 45;
		public float            helplineWidth = 0.01f;
		public Color 			helplineColor = Color.white;

        [Header("Proxy")]
		public float            proxySize = 5f;
        public Color            proxyColor = Color.white;

        [Header("Proxy Distance")]
        public float            distanceMin = 1;
        public float            distanceMax = 10;
        public bool             distanceColor = false;
        public Color            distanceMinColor = Color.red;
		public Color            distanceMaxColor = Color.blue;
		public bool             distanceSize = false;
		public float            distanceMinSize = 1f;
		public float            distanceMaxSize = 10f;

        [Header("Proxy Label")]
        public bool             proxyLabel = true;
        public float            proxyLabelSize = 0.1f;
        public bool             proxyLabelToCenter = true;
        public Vector3          proxyLabelPosition = new Vector3(.1f, .1f, .1f);
        public Vector3          proxyLabelRotation = new Vector3(0, 0, 0);

        public override void Start()
        {
            base.Start();

            this.gameObject.name = "EyeSee";
            this.area = new Area(this);

            ToLayer(this.gameObject);
        }

        public override void Update()
        {
            if(this.proxies == null)
            {
                this.proxies = new Proxy[this.targets.Length];
                for (int i = 0; i < this.targets.Length; i++)
                    this.proxies[i] = new Proxy(this, this.targets[i]);
            }

            base.Update();
        }

        public void ToLayer(GameObject gameObject)
        {
            int layer = LayerMask.NameToLayer("EyeSee");

            if (layer < 0)
                return;

            gameObject.layer = layer;
        }
    }
}