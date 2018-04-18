/*
 * MIT License
 * 
 * Copyright (c) 2017 Uwe Gruenefeld
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

namespace Gruenefeld.OutOfView.Core
{
	public abstract class Proxy
	{
        public Technique technique { get; private set; }
        public GameObject target { get; private set; }
        public GameObject proxy { get; protected set; }

        public Proxy(Technique technique, GameObject target) 
		{
            this.technique = technique;
            this.target = target;

            this.proxy = new GameObject ("Proxy");
			this.proxy.transform.parent = technique.gameObject.transform;
		}

		public void SetVisible(bool visible)
		{
			Utility.SetVisibleAttribute (this.proxy, visible);
		}
			
		public virtual void Update()
		{
			if (this.proxy == null)
				return;

			this.UpdateColor ();
			this.UpdatePosition ();
			this.UpdateScale ();
			this.UpdateRotation ();
		}

        protected abstract void UpdateColor();
		protected abstract void UpdatePosition ();
		protected abstract void UpdateScale ();
		protected abstract void UpdateRotation ();
	}		
}