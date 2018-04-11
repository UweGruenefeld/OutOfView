/*
 * MIT License
 * 
 * Copyright (c) 2018 Daniel Lange, Lasse Hammer, Uwe Gruenefeld
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

namespace Gruenefeld.OutOfView.FlyingARrow
{
    public class Proxy : Core.Proxy
    {
        private Technique flyingarrow;
        private bool finished;

        public Proxy(Core.Technique technique, GameObject target) : base(technique, target)
        {
            this.flyingarrow = (Technique)technique;

            Technique.Destroy(this.proxy);
            this.proxy = GameObject.Instantiate((GameObject)Resources.Load("Prefabs/Arrow"));
            this.proxy.transform.parent = technique.gameObject.transform;

            this.proxy.transform.localScale = new Vector3(this.flyingarrow.scale, this.flyingarrow.scale, this.flyingarrow.scale);
            this.ResetPosition();

            this.finished = false;
        }

        protected override void UpdateColor()
        {
        }

        protected override void UpdatePosition()
        {
            if (this.finished)
                return;

            this.proxy.transform.GetComponentInChildren<MeshRenderer>().enabled = true;

            if ((this.proxy.transform.position - this.target.transform.position).magnitude > flyingarrow.minDistance)
                this.proxy.transform.Translate(0, 0, this.flyingarrow.speed * Time.deltaTime);
            else
            {
                this.proxy.transform.GetComponentInChildren<MeshRenderer>().enabled = false;
                AudioSource.PlayClipAtPoint(this.flyingarrow.sound, this.flyingarrow.spatial ? this.proxy.transform.position : Vector3.zero);
                this.ResetPosition();
                this.finished = this.flyingarrow.repeated ? false : true;
            }
        }

        protected override void UpdateRotation()
        {
            this.proxy.transform.LookAt(this.target.transform.position);
        }

        protected override void UpdateScale()
        {
        }

        private void ResetPosition()
        {
            this.proxy.transform.localPosition = new Vector3(0, this.flyingarrow.height, -this.flyingarrow.distance);
        }
    }
}


        /*
        // Unity-related objects
        public GameObject pointer;
        public Transform mTarget;
        private AudioSource[] audioSources;
        private AudioSource beep;
        private AudioSource click;
        private Animator mAnimator;
        private GestureRecognizer recognizer;

        // Public parameters


        // Internals
        private const float EPSILON = 0.1f;
        private bool mouseClicked = false;
        private Vector3 startPosition;
        private float clickCounter;
        private float volumeCounter;
        private bool soundPlayed = false;

        void Start()
        {
            //recognizer = new GestureRecognizer();
            //recognizer.TappedEvent += Recognizer_TappedEvent;
            //recognizer.StartCapturingGestures();

            clickCounter = mClickDistance;
            volumeCounter = mVolumeDistance;
            this.mAnimator = GetComponent<Animator>();

            initializeSoundFiles();
            saveStartPosition();
        }

        void Update()
        {
            updatePointer();
            distanceClickingSound();
            distanceVolumeSound();
        }

        void OnTriggerEnter(Collider col)
        {
            if (col.gameObject.name == "CoreObject")
            {
                if (!soundPlayed)
                {
                    soundPlayed = true;
                    beep.Play();
                }
                // hide (destroy) the pointer
                MeshRenderer mr = pointer.GetComponent<MeshRenderer>();
                mr.enabled = false;
                Destroy(pointer, beep.clip.length);
            }
        }

        private void Recognizer_TappedEvent(InteractionSourceKind source, int tapCount, Ray headRay)
        {
            //hideTapToStart ();
            //Invoke ("shootPointer", 0.3f);
            //recognizer.StopCapturingGestures ();
        }

        public void shootPointer()
        {
            mouseClicked = true;
            transform.parent = null;
            if (mAnimator != null)
            {
                mAnimator.SetTrigger("startFade");
            }
        }

        // Follows the out of view object and reacts to mouse-click
        private void updatePointer()
        {
            if (mTarget != null)
            {
                transform.LookAt(mTarget.position);
                if (mouseClicked)
                {
                    if ((transform.position - mTarget.position).magnitude > EPSILON)
                    {
                        transform.Translate(0.0f, 0.0f, mSpeed * Time.deltaTime);
                    }
                }
                else if (Input.GetMouseButtonDown(1))
                {
                    mouseClicked = true;
                    transform.parent = null;
                    mAnimator.SetTrigger("startFade");
                }
            }
        }

        private void initializeSoundFiles()
        {
            audioSources = GetComponents<AudioSource>();

            foreach (AudioSource s in audioSources)
            {
                if (s.clip.name == "Beep")
                {
                    this.beep = s;
                }
                if (s.clip.name == "Click")
                {
                    this.click = s;
                }
            }
        }

        // Saves the start position for further use within the distance methods
        private void saveStartPosition()
        {
            startPosition.x = transform.position.x;
            startPosition.y = transform.position.y;
            startPosition.z = transform.position.z;
        }

        private void distanceClickingSound()
        {
            if (mDistClickingSound)
            {
                if (getDistanceToStartPostition() >= clickCounter)
                {
                    clickCounter += mClickDistance;
                    click.Play();
                }
            }
        }

        private void distanceVolumeSound()
        {
            if (mDistVolumeSound)
            {
                if (getDistanceToStartPostition() >= volumeCounter)
                {
                    volumeCounter += mVolumeDistance;
                    beep.volume -= mVolumeDistance;
                    print(beep.volume);
                }
            }
        }

        // Getter and Setter
        private float getDistanceToStartPostition()
        {
            return Vector3.Distance(startPosition, transform.position);
        }

        public void setTarget(Transform target)
        {
            this.mTarget = target;
        }

        public float getSizeScaleFactor()
        {
            return this.mSizeScaleFactor;
        }

        public float getDistance()
        {
            return this.mDistance;
        }

        public float getHeight()
        {
            return this.mHeight;
        }
    }
}*/