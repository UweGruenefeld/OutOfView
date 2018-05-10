/*
 * MIT License
 * 
 * Copyright (c) 2018 Uwe Gruenefeld
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
using System.Collections.Generic;
using UnityEngine;

namespace Gruenefeld.OutOfView.HaloWedge
{
    public class HaloXR : Proxy
    {
        public const int SEGMENTS = 128;

        public override void Start()
        {
            base.Start();

            this.name = "HaloXR";

            this.mapping = Mapper.ToSphere;
            this.meshFilter.mesh.name = "HaloXR";
        }

        public override void Update()
        {
            base.Update();

            if (this.size < 2 * Proxy.WIDTH || this.size > Proxy.RADIUS * Mathf.PI)
                return;

#pragma warning disable 162

            List<Vector3> vertices = new List<Vector3>();
            List<int> triangles = new List<int>();

            // front
            this.AddSurface(vertices, triangles, new Vector2(Proxy.WIDTH, 0),
                Vector2.zero);
            // back
            this.AddSurface(vertices, triangles, new Vector2(Proxy.WIDTH + Proxy.MAGIC, Proxy.MAGIC),
                new Vector2(Proxy.DEPTH, Proxy.DEPTH), true);
            // outer
            this.AddSurface(vertices, triangles, new Vector2(Proxy.MAGIC, 0),
                new Vector2(Proxy.DEPTH, 0), true);
            // inner
            this.AddSurface(vertices, triangles, new Vector2(Proxy.WIDTH + Proxy.MAGIC, Proxy.WIDTH),
                new Vector2(Proxy.DEPTH, 0));

            this.meshFilter.mesh.vertices = vertices.ToArray();
            this.meshFilter.mesh.triangles = triangles.ToArray();
            this.meshFilter.mesh.RecalculateNormals();

#pragma warning restore 162
        }

        private void AddSurface(List<Vector3> vertices, List<int> triangles, Vector2 width, Vector2 depth, bool cw = false)
        {
            int count = vertices.Count;
            float angle = 0;

            for (int i = 0; i < HaloXR.SEGMENTS; i++)
            {
                float radian = Mathf.Deg2Rad * angle;

                Vector2 inner = Vector2.zero, outer = Vector2.zero;
                Vector2 vector = new Vector2(Mathf.Sin(radian), Mathf.Cos(radian));

                inner = (new Vector2(vector.x + 1, vector.y) * this.size * .5f) - (vector * width.x);
                outer = (new Vector2(vector.x + 1, vector.y) * this.size * .5f) - (vector * width.y);

                Vector3 mappedInner = this.mapping(inner, Proxy.RADIUS + depth.x);
                Vector3 mappedOuter = this.mapping(outer, Proxy.RADIUS + depth.y);

                vertices.Add(mappedInner);
                vertices.Add(mappedOuter);

                angle += (360f / (float)HaloXR.SEGMENTS);
            }

            for (int i = 0; i < HaloXR.SEGMENTS; i++)
            {
                int a = (i * 2) + count;
                int b = a + 1;
                int c = a + 2;
                int d = a + 3;

                if (i == (HaloXR.SEGMENTS - 1))
                {
                    c = count;
                    d = count + 1;
                }

                if (cw)
                {
                    int temp = a;
                    a = d;
                    d = temp;
                }

                triangles.Add(a);
                triangles.Add(d);
                triangles.Add(b);

                triangles.Add(a);
                triangles.Add(c);
                triangles.Add(d);
            }
        }
    }
}