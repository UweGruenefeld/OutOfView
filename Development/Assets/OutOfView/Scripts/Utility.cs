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
using UnityEngine;

using System;
using System.Collections.Generic;

namespace Gruenefeld.OutOfView
{
	public class Utility
	{
		public const int DEFAULT_SEGMENTS = 64;
		public const float DEFAULT_LINEWIDTH = 0.06f;

        #region GAMEOBJECT

        /// <summary>
        /// Create new GameObject
        /// </summary>
        /// <param name="name">Name of GameObject</param>
        /// <param name="parent">Parent of GameObject</param>
        /// <returns>Created GameObject</returns>
        public static GameObject GameObject (
            string name, 
            Transform parent)
        {
            GameObject gameObject = new GameObject(name);
            gameObject.transform.parent = parent;
            return gameObject;
        }

        /// <summary>
        /// Create new GameObject
        /// </summary>
        /// <param name="name">Name of GameObject</param>
        /// <param name="position">Global position of GameObject</param>
        /// <param name="rotation">Rotation of GameObject</param>
        /// <returns>Created GameObject</returns>
        public static GameObject GameObject (
            string name, 
            Vector3 position, 
            Quaternion rotation)
		{
			GameObject gameObject = new GameObject(name);
			gameObject.transform.position = position;
			gameObject.transform.rotation = rotation;
			return gameObject;
		}

        /// <summary>
        /// Create new GameObject
        /// </summary>
        /// <param name="name">Name of GameObject</param>
        /// <param name="localPosition">Local position of GameObject</param>
        /// <param name="parent">Parent of GameObject</param>
        /// <returns>Created GameObject</returns>
        public static GameObject GameObject (
            string name, 
            Vector3 localPosition, 
            Transform parent)
        {
            GameObject gameObject = new GameObject(name);
            gameObject.transform.parent = parent;
            gameObject.transform.localPosition = localPosition;
            return gameObject;
        }

        /// <summary>
        /// Create new GameObject
        /// </summary>
        /// <param name="name">Name of GameObject</param>
        /// <param name="localPosition">Local position of GameObject</param>
        /// <param name="rotation">Rotation of GameObject</param>
        /// <param name="parent">Parent of GameObject</param>
        /// <returns>Created GameObject</returns>
        public static GameObject GameObject (
            string name, 
            Vector3 localPosition, 
            Quaternion rotation, 
            Transform parent)
		{
			GameObject gameObject = new GameObject(name);
			gameObject.transform.parent = parent;
			gameObject.transform.localPosition = localPosition;
			gameObject.transform.rotation = rotation;
			return gameObject;
		}

        #endregion

        #region COMPONENTS

        /// <summary>
        /// Adds a LineRenderer component to a GameObject
        /// </summary>
        /// <param name="gameObject">The GameObject the LineRenderer is added to</param>
        /// <param name="width">OPTIONAL: The line width of that renderer</param>
        /// <returns>The created LineRenderer</returns>
        public static LineRenderer AddLineRendererComponent (
            GameObject gameObject, 
            float width = Utility.DEFAULT_LINEWIDTH)
        {
            // Check if LineRenderer already exists
            LineRenderer lineRenderer = gameObject.GetComponent<LineRenderer>();
            if (lineRenderer == null)
                lineRenderer = gameObject.AddComponent<LineRenderer>();

            // Set parameter for default LineRenderer
            lineRenderer.useWorldSpace = false;
            lineRenderer.startWidth = width;
            lineRenderer.endWidth = width;

            return lineRenderer;
        }

        #endregion

        #region MESHES

        /// <summary>
        /// Adds a circle mesh to a GameObject
        /// </summary>
        /// <param name="gameObject">The GameObject the circle mesh is added to</param>
        /// <param name="segments">OPTIONAL: Number of segments used for the circle mesh</param>
        /// <returns>GameObject the circle mesh was added to</returns>
        public static GameObject AddCircleMesh (
            GameObject gameObject, 
            int segments = Utility.DEFAULT_SEGMENTS)
		{
            // Create quaternion from angle
            float angle = 360f / (float)segments;
            Quaternion quaternion = Quaternion.Euler(0.0f, 0.0f, angle);

            // Create vertex list for circle mesh
			List<Vector3> vertexList = new List<Vector3> ();
			vertexList.Add (new Vector3 (0.0f, 0.0f, 0.0f));
			vertexList.Add (new Vector3 (0.0f, 0.5f, 0.0f));
			vertexList.Add(quaternion * vertexList[1]);

            // Create triangle list for circle mesh
            List<int> triangleList = new List<int> ();
			triangleList.AddRange( new int[] {0, 1, 2} );

            // Create all triangles used for the circle mesh
			for (int i = 0; i < segments - 1; i++)
			{
				triangleList.Add(0);
				triangleList.Add(vertexList.Count - 1);
				triangleList.Add(vertexList.Count);
				vertexList.Add(quaternion * vertexList[vertexList.Count - 1]);
			}

            // Create the mesh object
			Mesh mesh = new Mesh();
			mesh.name = "Circle";
			mesh.vertices = vertexList.ToArray();
			mesh.triangles = triangleList.ToArray();

            // Create the mesh filter object
			MeshFilter meshFilter = gameObject.GetComponent<MeshFilter> ();
			if(meshFilter == null)
				meshFilter = gameObject.AddComponent<MeshFilter> ();
			gameObject.AddComponent<MeshRenderer> ();
			meshFilter.mesh = mesh;

			return gameObject;
		}

        /// <summary>
        /// Adds a text mesh to a GameObject
        /// </summary>
        /// <param name="gameObject">The GameObject the text mesh is added to</param>
        /// <param name="text">Text as a string</param>
        /// <param name="textAnchor">OPTIONAL: The position the text is placed at</param>
        /// <returns>GameObject the text mesh was added to</returns>
		public static GameObject AddTextMesh (
            GameObject gameObject, 
            string text, 
            TextAnchor textAnchor = TextAnchor.LowerLeft)
		{
            // Create TextMesh
			TextMesh textMesh = (TextMesh)gameObject.GetComponent<TextMesh> ();
			textMesh.characterSize = 0.25f;
			textMesh.fontSize = 28;
			textMesh.anchor = textAnchor;

            // Create Renderer
			Renderer renderer = textMesh.gameObject.GetComponent<Renderer> ();
			renderer.material.color = Color.black;

			return gameObject;
		}

        #endregion

        #region LINES

        /// <summary>
        /// Adds a ellipse to a GameObject
        /// </summary>
        /// <param name="gameObject">The GameObject the ellipse is added to</param>
        /// <param name="radius">Radius of the ellipse as a Vector2</param>
        /// <param name="compress">Function that manipulates the positions used in the LineRenderer</param>
        /// <param name="width">OPTIONAL: Width of the lines</param>
        /// <param name="segments">OPTIONAL: Number of segments used for the ellipse</param>
        /// <returns>GameObject the ellipse was added to</returns>
        public static GameObject AddEllipseLine(
            GameObject gameObject,
            Vector2 size,
            Color color,
            Func<Vector2, bool> hide,
            Func<Vector2, Vector2> compress,
            bool dotted = false,
            float width = Utility.DEFAULT_LINEWIDTH, 
            int segments = Utility.DEFAULT_SEGMENTS)
		{
            // Create new LineRenderer
			LineRenderer lineRenderer = Utility.AddLineRendererComponent (gameObject, width);
			lineRenderer.positionCount = segments;
            lineRenderer.startColor = color;
            lineRenderer.endColor = color;
            lineRenderer.loop = true;

            // Calculate the points
			float angle = 0f;
			for (int i = 0; i < segments; i++)
			{
                Vector2 linePoint = new Vector2(
                    Mathf.Sin(Mathf.Deg2Rad * angle) * size.x,
                    Mathf.Cos(Mathf.Deg2Rad * angle) * size.y
                );
                lineRenderer.SetPosition(i, Utility.Position(linePoint, hide, compress));
				angle += (360f / segments);
			}

            lineRenderer.material = new Material(Shader.Find("Custom/Line"));
            lineRenderer.material.SetFloat("_Dotted", dotted ? 1 : 0);
            lineRenderer.material.SetFloat("_Repeat", lineRenderer.positionCount);
            lineRenderer.material.SetFloat("_Offset", 0);
            lineRenderer.material.renderQueue = 3000;
            lineRenderer.material.color = color;

            return gameObject;
		}

        /// <summary>
        /// Adds a rectangle to a GameObject
        /// </summary>
        /// <param name="gameObject">The GameObject the rectangle is added to</param>
        /// <param name="radius">Radius of the ellipse as a Vector2</param>
        /// <param name="compress">Function that manipulates the positions used in the LineRenderer</param>
        /// <param name="width">OPTIONAL: Width of the lines</param>
        /// <param name="segments">OPTIONAL: Number of segments used for the ellipse</param>
        /// <returns>GameObject the rectangle was added to</returns>
		public static GameObject AddRectangleLine (
            GameObject gameObject, 
            Vector2 size,
            Color color,
            Func<Vector2, bool> hide,
            Func<Vector2, Vector2> compress,
            bool dotted = false,
            float width = Utility.DEFAULT_LINEWIDTH, 
            int segments = Utility.DEFAULT_SEGMENTS)
		{
            // Calculate the segments per side of rectangle
            int segmentsEachSide = segments / 4;

            // Create new LineRenderer
			LineRenderer lineRenderer = Utility.AddLineRendererComponent (gameObject, width);
			lineRenderer.positionCount = 1 + (segmentsEachSide * 4);
            lineRenderer.startColor = color;
            lineRenderer.endColor = color;
            lineRenderer.loop = true;

			float x = size.x, y = size.y;
            float stepX = (x * 2) / segmentsEachSide, stepY = (y * 2) / segmentsEachSide;

            // Calculate the points
            for (int i = 0; i < segmentsEachSide; i++)
                lineRenderer.SetPosition(
                    i,
                    Utility.Position(new Vector2(-x, -y + (i * stepY)), hide, compress)
                );

            for (int i = 0; i < segmentsEachSide; i++)
			    lineRenderer.SetPosition (
                    i + segmentsEachSide, 
                    Utility.Position(new Vector2(-x + (i * stepX), y), hide, compress)
                );

            for (int i = 0; i < segmentsEachSide; i++)
			    lineRenderer.SetPosition (
                    i + (segmentsEachSide * 2),
                    Utility.Position(new Vector2(x, y - (i * stepY)), hide, compress)
                );

            for (int i = 0; i < segmentsEachSide; i++)
                lineRenderer.SetPosition (
                    i + (segmentsEachSide * 3),
                    Utility.Position(new Vector2(x - (i * stepX), -y), hide, compress)
                );

			lineRenderer.SetPosition ((segmentsEachSide * 4), Utility.Position(new Vector2(-x, -y), hide, compress));

            lineRenderer.material = new Material(Shader.Find("Custom/Line"));
            lineRenderer.material.SetFloat("_Dotted", dotted ? 1 : 0);
            lineRenderer.material.SetFloat("_Repeat", lineRenderer.positionCount);
            lineRenderer.material.SetFloat("_Offset", 0);
            lineRenderer.material.renderQueue = 3000;
            lineRenderer.material.color = color;

            return gameObject;
		}

        /// <summary>
        /// Adds a line to a GameObject
        /// </summary>
        /// <param name="gameObject">The GameObject the line is added to</param>
        /// <param name="start">Start position of line</param>
        /// <param name="end">End position of line</param>
        /// <param name="compress">Function that manipulates the positions used in the LineRenderer</param>
        /// <param name="width">OPTIONAL: Width of the lines</param>
        /// <param name="segments">OPTIONAL: Number of segments used for the line</param>
        /// <returns>GameObject the line was added to</returns>
        public static GameObject AddLine (
            GameObject gameObject, 
            Vector2 start, 
            Vector2 end,
            Color color,
            Func<Vector2, bool> hide,
            Func<Vector2, Vector2> compress,
            bool dotted = false,
            float width = Utility.DEFAULT_LINEWIDTH,
            int segments = Utility.DEFAULT_SEGMENTS)
        {
            LineRenderer lineRenderer = Utility.AddLineRendererComponent(gameObject, width);
            lineRenderer.positionCount = segments + 1;
            lineRenderer.startColor = color;
            lineRenderer.endColor = color;
            lineRenderer.loop = false;

            Vector2 step = (end - start) / segments;
            for (int i = 0; i <= segments; i++)
                lineRenderer.SetPosition(i, Utility.Position(start + (step * i), hide, compress));

            lineRenderer.material = new Material(Shader.Find("Custom/Line"));
            lineRenderer.material.SetFloat("_Dotted", dotted ? 1 : 0);
            lineRenderer.material.SetFloat("_Repeat", lineRenderer.positionCount);
            lineRenderer.material.SetFloat("_Offset", 0);
            lineRenderer.material.renderQueue = 3000;
            lineRenderer.material.color = color;

            return gameObject;
        }

        /// <summary>
        /// Change a position with use of hide and compress
        /// </summary>
        /// <param name="origin">Original position</param>
        /// <param name="hide">Function when to hide a vector</param>
        /// <param name="compress">Function how to compress the vector</param>
        /// <returns>Returns transformed position</returns>
        private static Vector3 Position (
            Vector2 origin,
            Func<Vector2, bool> hide,
            Func<Vector2, Vector2> compress)
        {
            float z = hide == null ? 0 : hide(origin) ? 1 : 0;
            origin = compress == null ? origin : compress(origin);
            return new Vector3(origin.x, origin.y, z);
        }

        #endregion

        #region ATTRIBUTES

        /// <summary>
        /// Set a GameObject visible or not
        /// </summary>
        /// <param name="gameObject">The GameObject that should be visible or not</param>
        /// <param name="visible">True if the GameObject should be visible, otherwise false</param>
        public static void SetVisibleAttribute(
            GameObject gameObject, 
            bool visible)
		{
			if (gameObject == null)
				return;

			Renderer renderer = gameObject.GetComponent<Renderer> ();
			if (renderer == null)
				return;
			
			renderer.enabled = visible;
		}

        /// <summary>
        /// Set color of GameObject
        /// </summary>
        /// <param name="gameObject">The GameObject for which to change the color</param>
        /// <param name="color">The color</param>
        public static void SetColorAttribute(
            GameObject gameObject,
            Color color)
        {
            if (gameObject == null)
                return;

            Renderer renderer = gameObject.GetComponent<Renderer>();
            if (renderer == null)
                return;

            renderer.material.color = color;
        }

		#endregion

		#region MATH

        /// <summary>
        /// Calculates if vector is colliding with sphere
        /// </summary>
        /// <param name="vector">Vector to check</param>
        /// <param name="sphereCollider">Sphere collider</param>
        /// <returns>Returns true if collides, otherwise false</returns>
		public static bool VectorSphereColliderIntersection(
            Vector3 vector, 
            SphereCollider sphereCollider)
		{
			if (vector.magnitude == 0f)
				return false;

			Ray ray = new Ray (Vector3.zero, vector);
			RaycastHit raycastHit;

			return sphereCollider.Raycast (ray, out raycastHit, Int32.MaxValue);
		}

        /// <summary>
        /// Transform world vector to sphere vector
        /// </summary>
        /// <param name="camera">Used Camera</param>
        /// <param name="position">Position in world space</param>
        /// <returns>Position of world vector around camera sphere</returns>
		public static Vector3 WorldVector3ToSphereVector3(
            Camera camera, 
            Vector3 position)
		{
			position = camera.worldToCameraMatrix * position;
			position.Normalize();

			return position;
		}

        /// <summary>
        /// Transforms world vector proportional onto plane
        /// </summary>
        /// <param name="camera">Used Camera</param>
        /// <param name="position">Position in world space</param>
        /// <param name="z">Distance of plane</param>
        /// <returns>Position of world vector on plane</returns>
		public static Vector2 WorldVector3ProportionalTo2DPlane(
            Camera camera,
            Vector3 position, 
            float z)
		{
			position = Utility.WorldVector3ToSphereVector3(camera, position);

			Vector3 origin = new Vector3(0, 0, -1);
			float distance = Mathf.Abs(Mathf.Acos(origin.x * position.x + origin.y * position.y + origin.z * position.z));

			Vector2 transformedPosition = new Vector2(position.x, position.y);
			transformedPosition.Normalize();
			transformedPosition.x = transformedPosition.x * distance * Mathf.Abs(z);
			transformedPosition.y = transformedPosition.y * distance * Mathf.Abs(z);

			return transformedPosition;
		}

        /// <summary>
        /// Transforms world vector stereographic onto plane
        /// </summary>
        /// <param name="camera">Used Camera</param>
        /// <param name="position">Position in world space</param>
        /// <param name="z">Distance of plane</param>
        /// <returns>Position of world vector on plane</returns>
		public static Vector2 WorldVector3StereographicTo2DPlane(
            Camera camera, 
            Vector3 position, 
            float z)
		{
			position = Utility.WorldVector3ToSphereVector3(camera, position);

			Vector3 intersectionPoint;

			if (Utility.LinePlaneIntersection(out intersectionPoint,
				new Vector3(0, 0, -1),
				new Vector3(position.x, position.y, position.z - 1),
				new Vector3(0, 0, Mathf.Abs(z)),
				new Vector3(0, 0, 1)))
			{
				return new Vector2(-intersectionPoint.x, -intersectionPoint.y);
			}

			return new Vector2();
		}

        /// <summary>
        /// Calculates if a line intersects with a plane
        /// </summary>
        /// <param name="intersectionPoint">If true it returns the intersection point</param>
        /// <param name="linePoint">Any point on line</param>
        /// <param name="lineVector">Vector of line</param>
        /// <param name="planePoint">Any point on plane</param>
        /// <param name="planeNormal">Normal of plane</param>
        /// <returns>Returns true if it intersects, otherwise false</returns>
		public static bool LinePlaneIntersection(
            out Vector3 intersectionPoint, 
			Vector3 linePoint, 
            Vector3 lineVector, 
            Vector3 planePoint, 
            Vector3 planeNormal)
		{
			// If no intersection piont gets calculated
			intersectionPoint = Vector3.zero;

			// Calculate the distances
			float dotDenominator = Vector3.Dot(lineVector, planeNormal);
			float dotNumerator = Vector3.Dot((planePoint - linePoint), planeNormal);

			// If line and plane are not parallel
			if(dotDenominator != 0.0f) 
			{
				float length =  dotNumerator / dotDenominator;
				Vector3 vector = lineVector.normalized * length;
				intersectionPoint = linePoint + vector;	

				// Intersection point found
				return true;	
			}

			return false;
		}

        #endregion

        #region DATASTRUCTURES

        /// <summary>
        /// Implementation of Fisher–Yates shuffle
        /// </summary>
        /// <typeparam name="T">Type of list</typeparam>
        /// <param name="list">List to shuffle</param>
        public static void Shuffle<T>(
            ref List<T> list)  
		{  
            // Generate randomness
			System.Random random = new System.Random();  

            // Shuffle through the list
			for (int n = list.Count; n > 2; n--) 
			{  
				int rand = random.Next(n);  
				T value = list[rand];  
				list[rand] = list[n - 1];  
				list[n - 1] = value;
			}  
		}
			
		#endregion
	}
}