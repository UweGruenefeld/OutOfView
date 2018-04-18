/*
 * MIT License
 * 
 * Copyright (c) 2018 Uwe Gruenefeld, Dag Ennenga, Dana Hsiao
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

using System.Collections.Generic;

namespace Gruenefeld.OutOfView.EyeSee
{
	public class Area : Core.Area
	{
        private Technique eyesee;

		public Vector2 innerBoundarySize { get; private set; }
		public Vector2 outerBoundarySize { get; private set; }

        private GameObject innerBoundary;
        private GameObject outerBoundary;

        private GameObject verticalZeroline;
        private GameObject horizontalZeroline;

        private Dictionary<int, GameObject> verticalHelpLines;
		private Dictionary<int, GameObject> horizontalHelpLines;

		public Area(Technique technique) : base (technique)
        {
            this.eyesee = (Technique)technique;

			this.verticalHelpLines = new Dictionary<int, GameObject>();
			this.horizontalHelpLines = new Dictionary<int, GameObject>();

			this.outerBoundarySize = this.OuterBoundarySize();
			this.innerBoundarySize = this.InnerBoundarySize(this.outerBoundarySize);
		}
			
		public override void Update()
		{
            if(this.outerBoundarySize.magnitude <= 0)
            {
                Debug.LogError("EyeSee fails setting dimensions.");
                return;
            }

            this.DrawInnerBoundary ();
			this.DrawOuterBoundary ();

            this.DrawVerticalZeroline();
            this.DrawHorizontalZeroline();

            this.DrawVerticalHelplines ();
            this.DrawHorizontalHelplines ();

            if (this.eyesee.roll) 
			{
				Vector3 rotation = this.area.transform.rotation.eulerAngles;
				this.area.transform.rotation = Quaternion.Euler (rotation.x, rotation.y, 0);
			}
		}

        #region COMPRESSION

        public Vector2 Compress(Vector2 position)
        {
            float limitY = outerBoundarySize.y;
            position.y *= 180 / this.eyesee.space.y;
            position.y = this.Compress(this.eyesee.compressY, position.y, limitY);

            float limitX = Mathf.Sin(Mathf.Acos(Mathf.Abs(position.y) / outerBoundarySize.y)) * outerBoundarySize.x;
            position.x *= 360 / this.eyesee.space.x;
            position.x = this.Compress(this.eyesee.compressX, position.x, limitX);
                
            return position;
        }

        public float Compress(EnumCompression compression, float value, float scaling) 
		{
			switch (compression) 
			{
			    case EnumCompression.Maximum:
                    value = 0;
				    break;
			    case EnumCompression.Root:
                    if (value == 0 || scaling == 0) break;
                    float factor = 1f / this.eyesee.root;
				    value = (value < 0 ? -1 : 1) * Mathf.Pow (Mathf.Abs (value), factor);
				    value /= Mathf.Pow (scaling, factor);
				    value *= scaling;
				    break;
			}
			return value;
		}

        #endregion

        #region FOCUS

        public bool Hide(Vector2 point)
        {
            point = new Vector2(point.x, point.y - innerBoundary.transform.localPosition.y);

            if (this.eyesee.fov == EnumFOV.Custom && this.eyesee.fovCustomShape == EnumShape.ELLIPSE)
                return ((Mathf.Pow(point.x, 2) / Mathf.Pow(innerBoundarySize.x, 2)) +
                    (Mathf.Pow(point.y, 2) / Mathf.Pow(innerBoundarySize.y, 2))) <= 1;
            else
                return ((point.y >= -innerBoundarySize.y) && (point.y <= innerBoundarySize.y)) &&
                    ((point.x >= -innerBoundarySize.x) && (point.x <= innerBoundarySize.x));
        }

        #endregion

        #region FIELDOFVIEW

        private Vector2 CameraFOV()
        {
            float angle = Camera.main.fieldOfView * Mathf.Deg2Rad;
            float width = Mathf.Rad2Deg * 2f * Mathf.Atan(Mathf.Tan(angle / 2f) * Camera.main.aspect);
            float height = width / Camera.main.aspect;

            return new Vector2(width, height);
        }

        private Vector2 InnerFOV()
        {
            switch (this.eyesee.fov)
            {
                case EnumFOV.Camera:
                    return this.CameraFOV();
                default:
                    return this.eyesee.fovCustomSize;
            }
        }

        private Vector2 OuterFOV()
        {
            switch (this.eyesee.outerSize)
            {
                case EnumFOV.Camera:
                    return this.CameraFOV();
                default:
                    return this.eyesee.outerSizeCustom;
            }
        }

        #endregion

        #region BOUNDARIES

        private void Locate()
        {
            Vector3 position = this.innerBoundary.transform.localPosition;
            float rotation = 0;

            rotation = Camera.main.transform.rotation.eulerAngles.x;

            if (rotation > 180)
            {
                rotation = 360 - rotation;
                rotation = Mathf.Min(90, rotation);
            }
            else
                rotation = -Mathf.Min(90, rotation);

            position.y = (this.outerBoundarySize.y / 10f) * (rotation / 9);
            this.innerBoundary.transform.localPosition = position;
        }

        private float CalculateSize(float degree)
        {
            return Mathf.Tan((degree * Mathf.Deg2Rad) / 2) * Technique.DISTANCE;
        }

        private Vector2 OuterBoundarySize()
        {
            Vector2 outerBoundarySize = new Vector2();
            Vector2 degreeFOV = this.OuterFOV();

            outerBoundarySize.x = CalculateSize(degreeFOV.x) * (this.eyesee.space.x / 360);
            outerBoundarySize.y = CalculateSize(degreeFOV.y) * (this.eyesee.space.y / 180); ;
            return outerBoundarySize;
        }

        private Vector2 InnerBoundarySize(Vector2 outerBoundarySize)
        {
            Vector2 innerBoundarySize = new Vector2();
            Vector2 degreeFOV = this.InnerFOV();

            innerBoundarySize.x = (outerBoundarySize.x / 360) * degreeFOV.x;
            innerBoundarySize.y = (outerBoundarySize.y / 180) * degreeFOV.y;

            // Portrait camera
            if (degreeFOV.x < degreeFOV.y)
                innerBoundarySize.y = innerBoundarySize.x / 2;

            return new Vector2(
                innerBoundarySize.x,
                innerBoundarySize.y
            );
        }

        private void DrawInnerBoundary()
		{
			if (this.innerBoundary == null)
				this.innerBoundary = Utility.GameObject ("InnerBoundary", Vector3.zero, this.area.transform);

			if (this.eyesee.fov == EnumFOV.Custom && this.eyesee.fovCustomShape == EnumShape.ELLIPSE)
				Utility.AddEllipseLine (
                    this.innerBoundary, 
                    this.innerBoundarySize,
                    this.eyesee.innerLineColor,
                    null,
                    Compress, 
                    false,
                    this.eyesee.innerLineWidth);
			else
				Utility.AddRectangleLine (
                    this.innerBoundary,
                    this.innerBoundarySize, 
                    this.eyesee.innerLineColor,
                    null,
                    Compress,
                    false,
                    this.eyesee.innerLineWidth);

			if(this.eyesee.pitch)
				this.Locate ();
		}

		private void DrawOuterBoundary()
		{
            if (this.outerBoundary == null)
                this.outerBoundary = Utility.GameObject("OuterBoundary", Vector3.zero, this.area.transform);

            Utility.AddEllipseLine(
                this.outerBoundary, 
                this.outerBoundarySize,
                this.eyesee.outerLineColor,
                Hide,
                Compress,
                false,
                this.eyesee.outerLineWidth);
		}

        #endregion

        #region ZEROLINES

        private void DrawVerticalZeroline()
        {
            if (!this.eyesee.zerolineVertical)
                return;

            if (this.verticalZeroline == null)
                this.verticalZeroline = Utility.GameObject("VerticalZeroline", Vector3.zero, this.area.transform);

            Vector2 start = new Vector2(0, this.outerBoundarySize.y);
            Vector2 end = new Vector2(0, -this.outerBoundarySize.y);

            // Draw line
            Utility.AddLine(
                this.verticalZeroline,
                start,
                end,
                this.eyesee.zerolineColor,
                Hide,
                Compress,
                this.eyesee.zerolineDotted,
                this.eyesee.zerolineWidth
            );
        }

        private void DrawHorizontalZeroline()
        {
            if (!this.eyesee.zerolineHorizontal)
                return;

            if (this.horizontalZeroline == null)
                this.horizontalZeroline = Utility.GameObject("HorizontalZeroline", Vector3.zero, this.area.transform);

            Vector2 start = new Vector2(this.outerBoundarySize.x, 0);
            Vector2 end = new Vector2(-this.outerBoundarySize.x, 0);

            // Draw line
            Utility.AddLine(
                this.horizontalZeroline,
                start,
                end,
                this.eyesee.zerolineColor,
                Hide,
                Compress,
                this.eyesee.zerolineDotted,
                this.eyesee.zerolineWidth
            );
        }

        #endregion

        #region HELPLINES

        private void DrawVerticalHelplines()
		{
			int step = this.eyesee.helplineVertical;
			int i = 1;

			while (step < (this.eyesee.space.x / 2)) 
			{
                if (!this.verticalHelpLines.ContainsKey(i) || this.verticalHelpLines[i] == null)
                    this.verticalHelpLines.Add(i, Utility.GameObject("VerticalHelpline" + i, Vector3.zero, this.area.transform));

                GameObject helpline = this.verticalHelpLines[i];

				// Calculate size
				float ratio = ((float)(this.eyesee.space.x / 2f) / step);
				Vector2 size = outerBoundarySize;
				size.x = (size.x / ratio);

                // Draw ellipse
                Utility.AddEllipseLine(
                    helpline,
                    size,
                    this.eyesee.helplineColor,
                    Hide,
                    Compress,
                    true,
                    this.eyesee.helplineWidth
                );

                step += this.eyesee.helplineVertical;
				i++;
			}
		}

		private void DrawHorizontalHelplines()
		{
			int i = 1;
			int step = this.eyesee.helplineHorizontal;

			while (step < this.eyesee.space.y / 2) 
			{
				if (!this.horizontalHelpLines.ContainsKey (i) || this.horizontalHelpLines [i] == null)
                    this.horizontalHelpLines.Add(i, Utility.GameObject("HorizontalHelpline" + i, Vector3.zero, this.area.transform));
				
				GameObject helpline = this.horizontalHelpLines [i];

                // Calculate size
                float ratio = (float)(this.eyesee.space.y / 2f) / step;
                Vector2 start = Vector2.zero;
				start.y = (outerBoundarySize.y / (ratio)) * Mathf.Pow (-1, i);
				start.x = (outerBoundarySize.x / outerBoundarySize.y) * 
                    Mathf.Sqrt (Mathf.Pow (outerBoundarySize.y, 2) - Mathf.Pow (start.y, 2));
				Vector2 end = new Vector2 (-start.x, start.y);

                // Draw line
                Utility.AddLine(
                    helpline,
                    start,
                    end,
                    this.eyesee.helplineColor,
                    Hide,
                    Compress,
                    true,
                    this.eyesee.helplineWidth
                );

                if (i % 2 == 0)
					step += this.eyesee.helplineHorizontal;
			
				i++;
			}
		}

        #endregion
    }
}