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

namespace Gruenefeld.OutOfView.HaloWedge
{
    public class Proxy : MonoBehaviour
    {
        public const float DEPTH = 0.01f;
        public const float MAGIC = 0.003f;
        public const float WIDTH = 0.01f;
        public const float RADIUS = 1f;
        public const float MAX_DELAY = 1f;
        public const float DEGREES_TO_CENTER = 5;

        // Oculus = 25, Hololens = 5

        public Transform virtualObject;

        protected float size;

        protected Mapper.Mapping mapping;

        protected MeshFilter meshFilter { get; private set; }
        protected MeshRenderer meshRenderer { get; private set; }

        private Quaternion lastCamera;
        private Vector3 delayedMovement;

        public virtual void Start()
        {
            this.size = 0;

            this.meshFilter = this.gameObject.AddComponent<MeshFilter>();
            this.meshRenderer = this.gameObject.AddComponent<MeshRenderer>();

            this.SetColor(new Color(.8f, .8f, .8f, .8f));

            this.lastCamera = Camera.main.transform.rotation;
            this.delayedMovement = new Vector3(0, 0, 0);
        }

        public virtual void Update()
        {
            this.meshFilter.mesh.Clear();

            if (this.virtualObject == null || this.virtualObject.transform.localPosition.magnitude <= 0)
                return;

            this.size = Mathf.Deg2Rad * (Vector3.Angle(
                Camera.main.transform.forward,
                this.virtualObject.transform.position - Camera.main.transform.position
            ) - Proxy.DEGREES_TO_CENTER) * Proxy.RADIUS;

            this.gameObject.transform.position = Camera.main.transform.position +
                ((this.virtualObject.transform.position - Camera.main.transform.position).normalized * Proxy.RADIUS);

            this.gameObject.transform.LookAt(Camera.main.transform.position, -Camera.main.transform.forward);

            /*
            // Calculate camera movement relative to last frame
            Vector3 differenceCamera = new Vector3(
                Mathf.DeltaAngle(Camera.main.transform.rotation.eulerAngles.x, this.lastCamera.eulerAngles.x),
                Mathf.DeltaAngle(Camera.main.transform.rotation.eulerAngles.y, this.lastCamera.eulerAngles.y),
                Mathf.DeltaAngle(Camera.main.transform.rotation.eulerAngles.z, this.lastCamera.eulerAngles.z)
            );
            this.lastCamera = Camera.main.transform.rotation;
            this.delayedMovement += differenceCamera;

            if (this.delayedMovement.x > MAX_DELAY) this.delayedMovement.x = MAX_DELAY;
            else if (this.delayedMovement.x < -MAX_DELAY) this.delayedMovement.x = -MAX_DELAY;
            if (this.delayedMovement.y > MAX_DELAY) this.delayedMovement.y = MAX_DELAY;
            else if (this.delayedMovement.y < -MAX_DELAY) this.delayedMovement.y = -MAX_DELAY;
            if (this.delayedMovement.z > MAX_DELAY) this.delayedMovement.z = MAX_DELAY;
            else if (this.delayedMovement.z < -MAX_DELAY) this.delayedMovement.z = -MAX_DELAY;

            float time = Time.deltaTime * 5;

            if (this.delayedMovement.x > time) this.delayedMovement.x -= time;
            else if (this.delayedMovement.x < -time) this.delayedMovement.x += Time.time;
            else this.delayedMovement.x = 0;

            if (this.delayedMovement.y > time) this.delayedMovement.y -= time;
            else if (this.delayedMovement.y < -time) this.delayedMovement.y += time;
            else this.delayedMovement.y = 0;

            if (this.delayedMovement.z > time) this.delayedMovement.z -= time;
            else if (this.delayedMovement.z < -time) this.delayedMovement.z += time;
            else this.delayedMovement.z = 0;

            this.gameObject.transform.RotateAround(this.gameObject.transform.position, Camera.main.transform.right, delayedMovement.x);
            this.gameObject.transform.RotateAround(this.gameObject.transform.position, Camera.main.transform.up, delayedMovement.y);
            this.gameObject.transform.RotateAround(this.gameObject.transform.position, Camera.main.transform.forward, delayedMovement.z);
            */
        }

        public void Select()
        {
            this.SetColor(new Color(1, 0, 0, .8f));
        }

        public float Radius()
        {
            return this.size;
        }

        private void SetColor(Color color)
        {
            if (this.meshRenderer == null)
                return;

            this.meshRenderer.material.SetColor("_Color", color);
            this.meshRenderer.material.SetFloat("_Mode", 3);
            this.meshRenderer.material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            this.meshRenderer.material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            this.meshRenderer.material.SetInt("_ZWrite", 1);
            this.meshRenderer.material.DisableKeyword("_ALPHATEST_ON");
            this.meshRenderer.material.EnableKeyword("_ALPHABLEND_ON");
            this.meshRenderer.material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            this.meshRenderer.material.renderQueue = 3000 + UnityEngine.Random.Range(1, 1000);
        }
    }
}