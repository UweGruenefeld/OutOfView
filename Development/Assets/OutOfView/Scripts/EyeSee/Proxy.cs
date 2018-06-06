/*
 * MIT License
 * 
 * Copyright (c) 2018 Uwe Gruenefeld, Dag Ennenga, Yvonne Brück
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
using UnityEngine;

namespace Gruenefeld.OutOfView.EyeSee
{
	public class Proxy : Core.Proxy
	{
        private Technique eyesee;

		public Proxy(Technique technique, GameObject target) : base(technique, target)
		{
            this.eyesee = (Technique)technique;

			Utility.AddCircleMesh (this.proxy);
			base.proxy.transform.localRotation = Quaternion.Euler(0, 180, 0);
		}
			
		protected override void UpdateColor()
		{
			if (!this.eyesee.distanceColor) 
			{
                Utility.SetColorAttribute(this.proxy, this.eyesee.proxyColor);
                return;
			}

			float distance = this.RelativeDistance ();
			float inverted = 1 - distance;

			Utility.SetColorAttribute (this.proxy, this.eyesee.distanceMinColor * inverted + this.eyesee.distanceMaxColor * distance);
		}

		protected override void UpdatePosition()
		{
			if (base.proxy == null)
				return;
	
			// Transform object position to camera space
			Matrix4x4 matrix = Matrix4x4.TRS (
				Camera.main.transform.position, 
				Quaternion.Euler (
					0, 
					Camera.main.transform.rotation.eulerAngles.y, 
					0
				), 
				Vector3.one
			);
			Vector3 objectToCamera = matrix.inverse.MultiplyPoint(this.target.transform.position);

            // Theta = y-angle of object position
            float theta = Mathf.Min(90, Vector3.Angle(
                new Vector3(0, objectToCamera.y, Mathf.Sqrt(Mathf.Pow(objectToCamera.x, 2) + Mathf.Pow(objectToCamera.z, 2))), 
                new Vector3(0, 0, 1))
            );

            // Phi = x-angle of object position
            float phi = Mathf.Min(180, Vector3.Angle(
                new Vector3(objectToCamera.x, 0, objectToCamera.z),
                new Vector3(0, 0, 1))
            ); // * Mathf.Cos(theta * Mathf.Deg2Rad));

            Vector2 position = new Vector2 (phi, theta);

			// Specify algebraic sign
			if (objectToCamera.x < 0)
				position.x *= -1;
			if (objectToCamera.y < 0)
				position.y *= -1;

            // Special cases
            if (objectToCamera.y == 0)
                theta = 0;
            if (objectToCamera.x == 0 && objectToCamera.z == 0)
                phi = 0;

			Vector2 outerBoundarySize = ((Area)this.technique.area).outerBoundarySize;

            // Calculate angle values to absolute values
            float factorX = position.x / (this.eyesee.space.x / 2f);
            float factorY = position.y / (this.eyesee.space.y / 2f);

            if (factorX > 1 || factorY > 1)
                Utility.SetColorAttribute(this.proxy, new Color(0, 0, 0, 0));

            position.x = Mathf.Min(position.x / this.eyesee.space.x, 1) * outerBoundarySize.x * Mathf.Cos(theta * Mathf.Deg2Rad) * 2;
            position.y = Mathf.Min(position.y / this.eyesee.space.y, 1) * outerBoundarySize.y * 2;

            // Calculate compression
            position = ((Area)this.technique.area).Compress(position);

            if(float.IsNaN(position.x) || float.IsNaN(position.y))
            {
                Debug.LogError("EyeSee fails calculating proxy position.");
                return;
            }

			this.proxy.transform.localPosition = new Vector3 (position.x, position.y, -0.001f);
	    }

		protected override void UpdateScale() 
		{
			Vector2 outerBoundarySize = ((Area)this.technique.area).outerBoundarySize;
			float factor = Mathf.Min(outerBoundarySize.x, outerBoundarySize.y) / 30f;

			if (!this.eyesee.distanceSize) 
			{
				this.proxy.transform.localScale = new Vector3 (this.eyesee.proxySize * factor, this.eyesee.proxySize * factor, 1);
				return;
			}

			float distance = this.RelativeDistance ();
			distance *= this.eyesee.distanceMaxSize * factor - this.eyesee.distanceMinSize * factor;
			distance += this.eyesee.proxySize * factor;

			this.proxy.transform.localScale = new Vector3 (distance, distance, 1);
		}

		protected override void UpdateRotation() {}
		
		private float RelativeDistance()
		{
			float distance = this.target.transform.position.magnitude;
			distance = Mathf.Max (this.eyesee.distanceMin, Mathf.Min(this.eyesee.distanceMax, distance));
			distance -= this.eyesee.distanceMin;
			distance /= this.eyesee.distanceMax - this.eyesee.distanceMin;
			return distance;
		}
	}
}