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
    public class WedgeXR : Proxy
    {
        public const float MAX_SIZE = 0.3f;
        public const float MIN_SIZE = 0.07f;
        public const int SEGMENTS = 32;

        public override void Start()
        {
            base.Start();

            this.name = "CurvedWedge";

            this.mapping = Mapper.ToSphere;
            this.meshFilter.mesh.name = "CurvedWedge_v3";
        }

        public override void Update()
        {
            base.Update();

            if (this.size < 2 * Proxy.WIDTH)
                return;

#pragma warning disable 162

            List<Vector3> vertices = new List<Vector3>();
            List<int> triangles = new List<int>();

            float scale = (2f - this.size) / 5f;
            scale = Mathf.Max(WedgeXR.MIN_SIZE, scale);
            scale = Mathf.Min(WedgeXR.MAX_SIZE, scale);

            // Corner points
            Vector2 point0 = new Vector2(0, 0);
            Vector2 point1 = new Vector2(this.size, scale / 2f);
            Vector2 point2 = new Vector2(this.size, -scale / 2f);

            // Angles
            float angleAlpha = Mathf.Rad2Deg * 2f * Mathf.Asin(scale / (2f * point1.magnitude));
            float angleBeta = (180f - angleAlpha) / 2f;

            // Help points in tip
            Vector2 point3 = new Vector2(WedgeXR.WIDTH / Mathf.Tan(angleAlpha * Mathf.Deg2Rad / 2f), 0);

            // Help points in back
            Vector2 point4 = new Vector2(
                this.size - WedgeXR.WIDTH,
                (scale / 2f) - (WedgeXR.WIDTH / Mathf.Tan(angleBeta * Mathf.Deg2Rad / 2f))
            );
            Vector2 point5 = new Vector2(point4.x, -point4.y);

            Vector2 point0A = new Vector2(point0.x + WedgeXR.MAGIC, point0.y + WedgeXR.MAGIC);
            Vector2 point1A = new Vector2(point1.x + WedgeXR.MAGIC, point1.y + WedgeXR.MAGIC);
            Vector2 point2A = new Vector2(point2.x + WedgeXR.MAGIC, point2.y + WedgeXR.MAGIC);
            Vector2 point3A = new Vector2(point3.x + WedgeXR.MAGIC, point3.y + WedgeXR.MAGIC);
            Vector2 point4A = new Vector2(point4.x + WedgeXR.MAGIC, point4.y + WedgeXR.MAGIC);
            Vector2 point5A = new Vector2(point5.x + WedgeXR.MAGIC, point5.y + WedgeXR.MAGIC);

            int count = vertices.Count;
            this.DrawLine(vertices, triangles, point0, point2, point3, point5, Vector2.zero);
            this.DrawLine(vertices, triangles, point2, point1, point5, point4, Vector2.zero);
            this.DrawLine(vertices, triangles, point1, point0, point4, point3, Vector2.zero, false, count);

            count = vertices.Count;
            this.DrawLine(vertices, triangles, point0A, point2A, point3A, point5A, new Vector2(WedgeXR.DEPTH, WedgeXR.DEPTH), true);
            this.DrawLine(vertices, triangles, point2A, point1A, point5A, point4A, new Vector2(WedgeXR.DEPTH, WedgeXR.DEPTH), true);
            this.DrawLine(vertices, triangles, point1A, point0A, point4A, point3A, new Vector2(WedgeXR.DEPTH, WedgeXR.DEPTH), true, count);

            count = vertices.Count;
            this.DrawLine(vertices, triangles, point0A, point2A, point0, point2, new Vector2(WedgeXR.DEPTH, 0));
            this.DrawLine(vertices, triangles, point2A, point1A, point2, point1, new Vector2(WedgeXR.DEPTH, 0));
            this.DrawLine(vertices, triangles, point1A, point0A, point1, point0, new Vector2(WedgeXR.DEPTH, 0), false, count);

            count = vertices.Count;
            this.DrawLine(vertices, triangles, point3A, point5A, point3, point5, new Vector2(WedgeXR.DEPTH, 0), true);
            this.DrawLine(vertices, triangles, point5A, point4A, point5, point4, new Vector2(WedgeXR.DEPTH, 0), true);
            this.DrawLine(vertices, triangles, point4A, point3A, point4, point3, new Vector2(WedgeXR.DEPTH, 0), true, count);

            this.meshFilter.mesh.vertices = vertices.ToArray();
            this.meshFilter.mesh.triangles = triangles.ToArray();
            this.meshFilter.mesh.RecalculateNormals();

#pragma warning restore 162
        }

        private void DrawLine(List<Vector3> vertices, List<int> triangles, Vector2 x1, Vector2 x2, Vector2 y1, Vector2 y2, Vector2 depth, bool cw = false, int count = -1)
        {
            int tempCount = vertices.Count;

            Vector2 stepX = new Vector2(Mathf.Abs(x2.x - x1.x), Mathf.Abs(x2.y - x1.y)) / WedgeXR.SEGMENTS;
            Vector2 stepY = new Vector2(Mathf.Abs(y2.x - y1.x), Mathf.Abs(y2.y - y1.y)) / WedgeXR.SEGMENTS;

            for (int i = 0; i < WedgeXR.SEGMENTS; i++)
            {
                vertices.Add(this.mapping(new Vector2(
                    x1.x < x2.x ? x1.x + (stepX.x * i) : x1.x - (stepX.x * i),
                    x1.y < x2.y ? x1.y + (stepX.y * i) : x1.y - (stepX.y * i)
                ), WedgeXR.RADIUS + depth.x));

                vertices.Add(this.mapping(new Vector2(
                    y1.x < y2.x ? y1.x + (stepY.x * i) : y1.x - (stepY.x * i),
                    y1.y < y2.y ? y1.y + (stepY.y * i) : y1.y - (stepY.y * i)
                ), WedgeXR.RADIUS + depth.y));

                int a = (i * 2) + tempCount;
                int b = a + 1;
                int c = a + 2;
                int d = a + 3;

                if (count >= 0)
                {
                    c = a;
                    a += 1;
                    b += 2;
                    d -= 1;

                    if (i == WedgeXR.SEGMENTS - 1)
                    {
                        b = count + 1;
                        d = count;
                    }
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