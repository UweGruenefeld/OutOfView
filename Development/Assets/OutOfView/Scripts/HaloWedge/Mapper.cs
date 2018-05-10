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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gruenefeld.OutOfView.HaloWedge
{
    public class Mapper
    {
        public delegate Vector3 Mapping(Vector2 point, float radius);

        public static Vector3 ToPlane(Vector2 point, float radius)
        {
            return new Vector3(
                radius * point.x,
                radius * point.y,
                0
            );
        }

        public static Vector3 ToCylinder(Vector2 point, float radius)
        {
            float alpha = point.x - (Mathf.PI / 2f);

            return new Vector3(
                radius * Mathf.Cos(alpha),
                radius * point.y,
                (radius * Mathf.Sin(alpha)) + 1
            );
        }

        public static Vector3 ToSphere(Vector2 point, float radius)
        {
            float alpha = point.x - (Mathf.PI / 2f);
            float beta = point.y - (Mathf.PI / 2f);

            return new Vector3(
                radius * Mathf.Cos(beta),
                radius * Mathf.Cos(alpha) * Mathf.Sin(beta),
                1 - (radius * Mathf.Sin(alpha) * Mathf.Sin(beta))
            );
        }
    }
}