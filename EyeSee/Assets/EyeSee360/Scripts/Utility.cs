/*
 * MIT License
 * 
 * Copyright (c) 2017 Uwe Gruenefeld, Yvonne Brueck
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
using UnityEngine.Rendering;

using System;
using System.Collections.Generic;

namespace Gruenefeld.OutOfView
{
	public class Utility
	{
		public const int DEFAULT_SEGMENTS = 36;
		public const float DEFAULT_LINEWIDTH = 0.06f;

		#region GAMEOBJECTS

		public static GameObject GameObject(string name, Vector3 position, Quaternion rotation)
		{
			GameObject gameObject = new GameObject(name);
			gameObject.transform.position = position;
			gameObject.transform.rotation = rotation;
			return gameObject;
		}

		public static GameObject GameObject(string name, Vector3 localPosition, Quaternion rotation, Transform parent)
		{
			GameObject gameObject = new GameObject(name);
			gameObject.transform.parent = parent;
			gameObject.transform.localPosition = localPosition;
			gameObject.transform.rotation = rotation;
			return gameObject;
		}

		public static GameObject GameObject(string name, Vector3 localPosition, Transform parent)
		{
			GameObject gameObject = new GameObject(name);
			gameObject.transform.parent = parent;
			gameObject.transform.localPosition = localPosition;
			return gameObject;
		}

		public static GameObject GameObject(string name, Transform parent)
		{
			GameObject gameObject = new GameObject(name);
			gameObject.transform.parent = parent;
			return gameObject;
		}

		#endregion

		#region COMPONENTS

		public static GameObject AddCircle(GameObject gameObject, int segments = Utility.DEFAULT_SEGMENTS)
		{
			float angle = 360f / (float) segments;
			Quaternion quaternion = Quaternion.Euler(0.0f, 0.0f, angle);
			List<Vector3> vertexList = new List<Vector3> ();
			vertexList.Add (new Vector3 (0.0f, 0.0f, 0.0f));
			vertexList.Add (new Vector3 (0.0f, 0.5f, 0.0f));
			vertexList.Add(quaternion * vertexList[1]);
			List<int> triangleList = new List<int> ();
			triangleList.AddRange( new int[]{0, 1, 2} );

			for (int i = 0; i < segments - 1; i++)
			{
				triangleList.Add(0);
				triangleList.Add(vertexList.Count - 1);
				triangleList.Add(vertexList.Count);
				vertexList.Add(quaternion * vertexList[vertexList.Count - 1]);
			}

			Mesh mesh = new Mesh();
			mesh.name = "Circle";
			mesh.vertices = vertexList.ToArray();
			mesh.triangles = triangleList.ToArray();

			MeshFilter meshFilter = gameObject.GetComponent<MeshFilter> ();
			if(meshFilter == null)
				meshFilter = gameObject.AddComponent<MeshFilter> ();
			gameObject.AddComponent<MeshRenderer> ();
			meshFilter.mesh = mesh;

			return gameObject;
		}

		public static GameObject AddText (GameObject gameObject, string text, TextAnchor textAnchor = TextAnchor.LowerLeft)
		{
			TextMesh textMesh = (TextMesh)gameObject.GetComponent<TextMesh> ();
			textMesh.characterSize = 0.25f;
			textMesh.fontSize = 28;
			textMesh.anchor = textAnchor;

			Renderer rendererText = textMesh.gameObject.GetComponent<Renderer> ();
			rendererText.material.color = Color.black;

			return gameObject;
		}
			
		public static GameObject AddEllipse(GameObject gameObject, Vector2 radius, 
			float width = Utility.DEFAULT_LINEWIDTH, int segments = Utility.DEFAULT_SEGMENTS)
		{
			LineRenderer lineRenderer = Utility.AddLineRenderer (gameObject, width);
			lineRenderer.positionCount = segments;
			lineRenderer.loop = true;

			float angle = 0f;
			for (int i = 0; i < segments; i++)
			{
				lineRenderer.SetPosition (i, new Vector3(
					Mathf.Sin (Mathf.Deg2Rad * angle) * radius.x,
					Mathf.Cos (Mathf.Deg2Rad * angle) * radius.y,
					0));

				angle += (360f / segments);
			}

			return gameObject;
		}

		public static GameObject AddRectangle(GameObject gameObject, Vector2 size, float width = Utility.DEFAULT_LINEWIDTH)
		{
			LineRenderer lineRenderer = Utility.AddLineRenderer (gameObject, width);
			lineRenderer.positionCount = 5;
			lineRenderer.loop = true;

			float x = size.x, y = size.y;

			lineRenderer.SetPosition (0, new Vector3(-x, -y, 0f));
			lineRenderer.SetPosition (1, new Vector3(-x,  y, 0f));
			lineRenderer.SetPosition (2, new Vector3( x,  y, 0f));
			lineRenderer.SetPosition (3, new Vector3( x, -y, 0f));
			lineRenderer.SetPosition (4, new Vector3(-x, -y, 0f));

			return gameObject;
		}

		public static LineRenderer AddLineRenderer(GameObject gameObject, float width = Utility.DEFAULT_LINEWIDTH)
		{
			LineRenderer lineRenderer = gameObject.GetComponent<LineRenderer> ();
			if (lineRenderer == null)
				lineRenderer = gameObject.AddComponent<LineRenderer> ();
			lineRenderer.useWorldSpace = false;
			lineRenderer.startWidth = width;
			lineRenderer.endWidth = width;

			return lineRenderer;
		}

		#endregion

		#region ATTRIBUTES

		public static void RemoveShadows(GameObject gameObject)
		{
			if (gameObject == null)
				return;

			MeshRenderer renderer = gameObject.GetComponent<MeshRenderer> ();
			if (renderer == null)
				return;

			renderer.receiveShadows = false;
			renderer.shadowCastingMode = ShadowCastingMode.Off;
			renderer.lightProbeUsage = LightProbeUsage.Off;
			renderer.reflectionProbeUsage = ReflectionProbeUsage.Off;
		}

		public static GameObject FindComponent(GameObject gameObject, string type) {
			Transform[] ts = gameObject.GetComponentsInChildren<Transform>(true);
			foreach (Transform t in ts) {
				if (t.gameObject.name == type) {
					return t.gameObject;
				}
			}
			return null;
		}

		public static void SetColor(GameObject gameObject, Color color, bool recursive = false, bool dotted = false)
		{
			if (gameObject == null)
				return;

			Renderer renderer = gameObject.GetComponent<Renderer> ();
			if (renderer == null)
				return;

			Material material = new Material (Shader.Find(dotted ? "Unlit/Dotted" : "Unlit/Color"));
			material.color = color;
			renderer.material = material;

            material.renderQueue = 3010;

            if (!recursive)
				return;
			
			Transform[] transforms = gameObject.GetComponentsInChildren<Transform>(true);
			foreach (Transform transform in transforms) 
			{
				renderer = transform.gameObject.GetComponent<Renderer> ();
				if (renderer != null)
					renderer.material.color = color;
			}
		}
		
		public static void SetColor(LineRenderer lineRenderer, Color color, bool dotted = false)
		{
			if (lineRenderer == null)
				return;

			lineRenderer.material.shader = Shader.Find(dotted? "Unlit/Dotted" : "Unlit/Color");
			lineRenderer.material.color = color;
            lineRenderer.material.renderQueue = 3000;
            lineRenderer.startColor = color;
			lineRenderer.endColor = color;
		}

		public static void SetVisible(GameObject gameObject, bool visible)
		{
			if (gameObject == null)
				return;

			Renderer renderer = gameObject.GetComponent<Renderer> ();
			if (renderer == null)
				return;
			
			renderer.enabled = visible;
		}

		public static void SetSegments(GameObject gameObject, int count, float offset = 0)
		{
			if (gameObject == null)
				return;

			LineRenderer lineRenderer = gameObject.GetComponent<LineRenderer> ();
			if (lineRenderer == null)
				return;

			Utility.SetSegments (lineRenderer, count, offset);
		}

		public static void SetSegments(LineRenderer lineRenderer, int count, float offset = 0)
		{
			lineRenderer.positionCount = (int)count + 1;
			lineRenderer.material.SetFloat("_RepeatCount", count);
			lineRenderer.material.SetFloat("_Offset", offset);
		}

		#endregion

		#region GYROSCOPE

		/*
		private static bool gyroInitialized = false;

		public static bool HasGyroscope
		{
			get { return SystemInfo.supportsGyroscope; }
		}

		public static Quaternion GetRotation()
		{
			if (!CoreUtility.gyroInitialized)
				CoreUtility.InitGyroscope();

			return CoreUtility.HasGyroscope ? CoreUtility.GetRotationFromGyroscope() : Quaternion.identity;
		}

		private static void InitGyroscope()
		{
			if (CoreUtility.HasGyroscope)
			{
				Input.gyro.enabled = true;
				// interval highest value (60 Hz)
				Input.gyro.updateInterval = 0.0167f;    
			}
			CoreUtility.gyroInitialized = true;
		}

		private static Quaternion GetRotationFromGyroscope()
		{
			return new Quaternion(0.5f, 0.5f, -0.5f, 0.5f) * Input.gyro.attitude * new Quaternion(0, 0, 1, 0);
		}
		*/

		#endregion

		#region MATH

		public static bool VectorSphereColliderIntersection(Vector3 vector, SphereCollider sphereCollider)
		{
			if (vector.magnitude == 0f)
				return false;

			Ray ray = new Ray (Vector3.zero, vector);
			RaycastHit raycastHit;

			return sphereCollider.Raycast (ray, out raycastHit, Int32.MaxValue);
		}

		public static Vector3 WorldVector3ToSphereVector3(Camera camera, Vector3 position)
		{
			position = camera.worldToCameraMatrix * position;
			position.Normalize();

			return position;
		}

		public static Vector2 WorldVector3ProportionalTo2DPlane(Camera camera, Vector3 position, float z)
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

		public static Vector2 WorldVector3StereographicTo2DPlane(Camera camera, Vector3 position, float z)
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

		public static bool LinePlaneIntersection(out Vector3 intersectionPoint, 
			Vector3 linePoint, Vector3 lineVector, Vector3 planePoint, Vector3 planeNormal)
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

		#region DATSTRUCTURES

		/*
		 * Implementation of Fisher–Yates shuffle
		 */
		public static void Shuffle<T>(ref List<T> list)  
		{  
			System.Random random = new System.Random();  
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