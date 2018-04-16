/*
 * MIT License
 * 
 * Copyright (c) 2017 Uwe Gruenefeld, Dag Ennenga, Dana Hsiao
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

namespace Gruenefeld.OutOfView.EyeSee360
{
	public class Area : Core.Area
	{
        // TODO 0° lines not dotted
        private Technique eyesee;

		private GameObject innerBoundary;
		private GameObject[] outerBoundary;

		public Vector2 innerBoundarySize { get; private set; }
		public Vector2 outerBoundarySize { get; private set; }

		private Dictionary<int, GameObject[]> verticalHelpLines;
		private Dictionary<int, GameObject[]> horizontalHelpLines;

		public Area(Technique technique) : base (technique)
        {
            this.eyesee = (Technique)technique;

			this.verticalHelpLines = new Dictionary<int, GameObject[]>();
			this.horizontalHelpLines = new Dictionary<int, GameObject[]>();

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
			this.DrawVerticalHelplines ();
			this.DrawHorizontalHelplines ();
			this.DrawVerticalZeroline ();
			this.DrawHorizontalZeroline ();

			if (this.eyesee.roll) 
			{
				Vector3 rotation = this.area.transform.rotation.eulerAngles;
				this.area.transform.rotation = Quaternion.Euler (rotation.x, rotation.y, 0);
			}
		}

        #region BOUNDARIES

        public float Compress(EnumCompression compression, float value, float maximum) 
		{
			switch (compression) 
			{
			case EnumCompression.Maximum:
                value = 0;
				break;
			case EnumCompression.SquareRoot:
				float factor = 1f / 2048f;
				value = value < 0 ? -(Mathf.Pow (Mathf.Abs (value) + 1, factor) - 1) : Mathf.Pow (Mathf.Abs (value) + 1, factor) - 1;
				value /= Mathf.Pow (maximum + 1, factor) - 1;
				value *= maximum;
				break;
			}
			return value;
		}

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

        private void DrawInnerBoundary()
		{
			if (this.innerBoundary == null)
				this.innerBoundary = Utility.GameObject ("InnerBoundary", Vector3.zero, this.area.transform);

			if (this.eyesee.fov == EnumFOV.Custom && this.eyesee.fovCustomShape == EnumShape.ELLIPSE)
				Utility.AddEllipse (this.innerBoundary, this.innerBoundarySize, this.eyesee.innerLineWidth);
			else
				Utility.AddRectangle (this.innerBoundary, this.innerBoundarySize, this.eyesee.innerLineWidth);

			Utility.SetColor (this.innerBoundary, this.eyesee.innerLineColor);

			if(this.eyesee.pitch)
				this.Locate ();
		}

		private void DrawOuterBoundary()
		{
			if (this.outerBoundary == null) 
			{
				this.outerBoundary = new GameObject[2];
				this.outerBoundary[0] = Utility.GameObject("OuterBoundaryA", Vector3.zero, this.area.transform);
				this.outerBoundary[1] = Utility.GameObject("OuterBoundaryB", Vector3.zero, this.area.transform);
			}

			if (!this.DrawEllipse (this.outerBoundary [0], this.outerBoundarySize, true, this.eyesee.outerLineWidth))
				this.DrawEllipse (this.outerBoundary [1], this.outerBoundarySize, false, this.eyesee.outerLineWidth);
		    else 
				Utility.SetSegments (this.outerBoundary [1], 0);

			Utility.SetColor (this.outerBoundary[0], this.eyesee.outerLineColor);
			Utility.SetColor (this.outerBoundary[1], this.eyesee.outerLineColor);
		}

		private void Locate()
		{
			Vector3 position = this.innerBoundary.transform.localPosition;
			float rotation = 0;

			//rotation = CoreUtilities.GetRotation ().eulerAngles.x;
			rotation = Camera.main.transform.rotation.eulerAngles.x;

			if (rotation > 180) 
			{
				rotation = 360 - rotation;
				rotation = Mathf.Min (90, rotation);
			} else
				rotation = -Mathf.Min (90, rotation);

			position.y = (this.outerBoundarySize.y / 10f) * (rotation / 9);
			this.innerBoundary.transform.localPosition = position;
		}
			
		private Vector2 OuterBoundarySize()
		{
			Vector2 outerBoundarySize = new Vector2();
            Vector2 degreeFOV = this.OuterFOV();

            outerBoundarySize.x = CalculateSize (degreeFOV.x);
			outerBoundarySize.y = CalculateSize (degreeFOV.y);
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
				this.Compress(this.eyesee.compressX, innerBoundarySize.x, this.outerBoundarySize.x),
				this.Compress(this.eyesee.compressY, innerBoundarySize.y, this.outerBoundarySize.y)
			);
		}

		private float CalculateSize(float degree) 
		{
			return Mathf.Tan ((degree * Mathf.Deg2Rad) / 2) * Technique.DISTANCE;
		}

		private bool OnInner(Vector2 point)
		{
			point = new Vector2 (point.x, point.y - innerBoundary.transform.localPosition.y);

			// Returns true if on inner and false if not
			if (this.eyesee.fov == EnumFOV.Custom && this.eyesee.fovCustomShape == EnumShape.ELLIPSE)
				return ((Mathf.Pow (point.x, 2) / Mathf.Pow (innerBoundarySize.x, 2)) + 
					(Mathf.Pow (point.y, 2) / Mathf.Pow (innerBoundarySize.y, 2))) <= 1;
            else
                return ((point.y >= -innerBoundarySize.y) && (point.y <= innerBoundarySize.y)) &&
                    ((point.x >= -innerBoundarySize.x) && (point.x <= innerBoundarySize.x));
        }

		#endregion

		#region HELPLINES

		private void DrawVerticalHelplines()
		{
			int step = this.eyesee.helplineVertical;
			int i = 1;
			while (step < (this.eyesee.space.x / 2)) 
			{
				if (!this.verticalHelpLines.ContainsKey (i) || this.verticalHelpLines [i] == null)
					this.AddVerticalHelpline (i);

				GameObject[] helpline = this.verticalHelpLines [i];

				// Calculate size
				float ratio = ((float)180 / step);
				Vector2 size = outerBoundarySize;
				size.x = (size.x / ratio );
				size.x = this.Compress (this.eyesee.compressX, size.x, this.outerBoundarySize.x);

				if (!this.DrawEllipse (helpline [0], size, true, this.eyesee.helplineWidth))
					this.DrawEllipse (helpline [1], size, false, this.eyesee.helplineWidth);
				else
					Utility.SetSegments (helpline [1], 0);

				Utility.SetColor (helpline [0].GetComponent<LineRenderer>(), this.eyesee.helplineColor, true);
				Utility.SetColor (helpline [1].GetComponent<LineRenderer>(), this.eyesee.helplineColor, true);

				step += this.eyesee.helplineVertical;
				i++;
			}
		}

		private void DrawHorizontalHelplines()
		{
			float yPositionDistance = 0;
			int i = 1;
			int step = this.eyesee.helplineHorizontal;
			while (step < this.eyesee.space.y / 2) 
			{
				if (!this.horizontalHelpLines.ContainsKey (i) || this.horizontalHelpLines [i] == null)
					this.AddHorizontalHelpline (i);
				
				GameObject[] helpline = this.horizontalHelpLines [i];

				Vector2 start = Vector2.zero;
				float ratio = (float)90 / step;
				yPositionDistance = outerBoundarySize.y / (ratio);
				yPositionDistance = this.Compress (this.eyesee.compressY, yPositionDistance, this.outerBoundarySize.y);
				start.y = yPositionDistance * Mathf.Pow (-1, i);

				start.x = (outerBoundarySize.x / outerBoundarySize.y) 
					* Mathf.Sqrt (Mathf.Pow (outerBoundarySize.y, 2) - Mathf.Pow (start.y, 2));
				Vector2 end = new Vector2 (-start.x, start.y);
				if (!this.DrawLine (helpline [0], start, end, this.eyesee.helplineWidth))
					this.DrawLine (helpline [1], end, start, this.eyesee.helplineWidth);
				else
					Utility.SetSegments (helpline [1], 0);

				Utility.SetColor (helpline [0].GetComponent<LineRenderer> (), this.eyesee.helplineColor, true);
				Utility.SetColor (helpline [1].GetComponent<LineRenderer> (), this.eyesee.helplineColor, true);
			
				if (i % 2 == 0)
					step += this.eyesee.helplineHorizontal;
			
				i++;
			}
		}

		private void DrawVerticalZeroline()
		{
			if (!this.eyesee.zerolineVertical)
				return;
			
			GameObject[] zeroline = null;
			if (!this.verticalHelpLines.ContainsKey (0) || this.verticalHelpLines [0] == null)
				zeroline = this.AddVerticalHelpline (0);
			else
				zeroline = this.verticalHelpLines [0];

			Vector2 start = new Vector2 (0, this.outerBoundarySize.y);
			Vector2 end = new Vector2 (0, -this.outerBoundarySize.y);
	
			if (!this.DrawLine(zeroline[0], start, end, this.eyesee.zerolineWidth))
				this.DrawLine(zeroline[1], end, start, this.eyesee.zerolineWidth);
			else
				Utility.SetSegments (zeroline [1], 0);

			Utility.SetColor (zeroline [0].GetComponent<LineRenderer>(), this.eyesee.zerolineColor, true);
			Utility.SetColor (zeroline [1].GetComponent<LineRenderer>(), this.eyesee.zerolineColor, true);
		}

		private void DrawHorizontalZeroline()
		{
			if (!this.eyesee.zerolineHorizontal)
				return;

			GameObject[] zeroline = null;
			if (!this.horizontalHelpLines.ContainsKey (0) || this.horizontalHelpLines [0] == null)
				zeroline = this.AddHorizontalHelpline (0);
			else
				zeroline = this.horizontalHelpLines [0];

			Vector2 start = new Vector2 (this.outerBoundarySize.x, 0);
			Vector2 end = new Vector2 (-this.outerBoundarySize.x, 0);

			if (!this.DrawLine(zeroline[0], start, end, this.eyesee.zerolineWidth))
				this.DrawLine(zeroline[1], end, start, this.eyesee.zerolineWidth);
			else
				Utility.SetSegments (zeroline [1], 0);

			Utility.SetColor (zeroline [0].GetComponent<LineRenderer>(), this.eyesee.zerolineColor, true);
			Utility.SetColor (zeroline [1].GetComponent<LineRenderer>(), this.eyesee.zerolineColor, true);
		}

		private bool DrawEllipse(GameObject gameObject, Vector2 radius, bool direction, float width)
		{
			LineRenderer lineRenderer = Utility.AddLineRenderer (gameObject, width);
			lineRenderer.loop = false;

			int count = (int)((radius.x + radius.y) * Mathf.PI * 3);
			Utility.SetSegments(lineRenderer, count);
			float angle = 90;
			float step = (360f / count);
			int runs = 0;
			for (int i = 0; i <= count; i++)
			{
				if (runs >= count) 
				{
					Utility.SetSegments(lineRenderer, Mathf.Max(0, i - 1));
					return false;
				}
				runs++;

				Vector2 ellipsePoint = new Vector2(
					Mathf.Sin (Mathf.Deg2Rad * angle) * radius.x,
					Mathf.Cos (Mathf.Deg2Rad * angle) * radius.y);
				
				if(this.OnInner(ellipsePoint))
				{
					if(i == 0) 
					{
						angle = direction ? angle + step : angle - step;
						i--;
						continue;
					}
					Utility.SetSegments(lineRenderer, Mathf.Max(0, i - 1));
					return false;
				}
				lineRenderer.SetPosition (i, ellipsePoint);
				angle = direction ? angle + step : angle - step;
			}

			return true;
		}

		private bool DrawLine(GameObject gameObject, Vector2 start, Vector2 end, float width)
		{
			LineRenderer lineRenderer = Utility.AddLineRenderer (gameObject, width);

			int count = (int)(Vector2.Distance(start, end) * 3);
			Utility.SetSegments(lineRenderer, count);

			Vector2 step = (end - start) / count;

			for(int i = 0; i <= count; i++)
			{
				Vector2 linePoint = start + (step * i);
				if(this.OnInner(linePoint))
				{
					if(i == 0)
					{
						lineRenderer.positionCount = 0;
						return false;
					}
						
					Utility.SetSegments(lineRenderer, Mathf.Max(0, i - 1));

					return false;
				}
				lineRenderer.SetPosition(i, linePoint);
			}

			return true;
		}

		private GameObject[] AddVerticalHelpline(int i)
		{
			GameObject[] helpline = this.AddHelpline(i, "Vertical");
			this.verticalHelpLines.Add (i, helpline);
			return helpline;
		}

		private GameObject[] AddHorizontalHelpline(int i)
		{
			GameObject[] helpline = this.AddHelpline(i, "Horizontal");
			this.horizontalHelpLines.Add (i, helpline);
			return helpline;
		}

		private GameObject[] AddHelpline(int i, string name)
		{
			GameObject helplineA = Utility.GameObject (name + "Helpline" + i + "a", Vector3.zero, this.area.transform);
			GameObject helplineB = Utility.GameObject (name + "Helpline" + i + "b", Vector3.zero, this.area.transform);
			return new GameObject[]{ helplineA, helplineB };
		}

		#endregion
	}
}