/*
 * MIT License
 * 
 * Copyright (c) 2018 Uwe Gruenefeld, Daniel Lange, Lasse Hammer
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

namespace Gruenefeld.OutOfView.FlyingARrow
{
	public class Technique : Core.Technique
	{
        [Header("Movement")]
        public float speed = 6.5f;
        public bool repeated = true;

        [Header("Transform")]
        public float distance = 1.0f;
        public float height = 0.5f;
        public float scale = 1.0f;

        [Header("Target")]
        public float minDistance = 0.1f;

        [Header("Sound")]
        public AudioClip sound;
        public bool spatial = false;
        public float volume = 10.0f;

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