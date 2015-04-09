using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Infantry.Cameras
{
    public class Camera
    {
        private Vector3 _position = Vector3.Zero;
        private float _fieldOfView = MathHelper.Pi / 2.0f;
        private float _nearPlane = 0.1f;
        private float _farPlane = 5000f;
        private Matrix _view;
        private Matrix _reflectedView;
        private Matrix _projection;
        private Matrix _culling;
        private BoundingFrustum _frustum;
        private BoundingFrustum _reflectedFrustum;
        private bool shaking;
        private float shakeMagnitude;
        private float shakeDuration;
        private float shakeTimer;
        private Vector3 shakeOffset;
        private Vector3 _lookAt;

        /// <summary>
        /// Position of the camera, Note: set this as virtual so it can
        /// be overridden during gameplay
        /// </summary>
        public virtual Vector3 Position
        {
            get { return _position; }
            set { _position = value; }
        }

        /// <summary>
        /// The field of view angle
        /// </summary>
        public float FieldOfView
        {
            get { return _fieldOfView; }
            set { _fieldOfView = value; }
        }

        /// <summary>
        /// The near plane used to determine the near sight viewing area.
        /// </summary>
        public float NearPlane
        {
            get { return _nearPlane; }
            set { _nearPlane = value; }
        }

        /// <summary>
        /// The far plane used to determine the far sight viewing area.
        /// </summary>
        public float FarPlane
        {
            get { return _farPlane; }
            set { _farPlane = value; }
        }

        /// <summary>
        /// The view matrix for this camera, the angle of our camera
        /// Will return a shaking view if shaking is turned on
        /// </summary>
        public Matrix View
        {
            get
            {
                Vector3 position = Position;
                Vector3 lookAT = LookAt;
                if (Shaking)
                {
                    position += shakeOffset;
                    lookAT += shakeOffset;
                }
                return Matrix.CreateLookAt(position, lookAT, Vector3.Up);
            }
            set { _view = value; }
        }

        /// <summary>
        /// The reflected viewing matrix this camera has around a plane
        /// </summary>
        public Matrix ReflectedView
        {
            get { return _reflectedView; }
            set { _reflectedView = value; }
        }

        /// <summary>
        /// Slightly smaller field of view for culling
        /// </summary>
        public float CullingFOV
        {
            get { return FieldOfView / 1.125f; }
        }

        /// <summary>
        /// The projection matrix this camera uses, shows what
        /// we can see within the viewing angle
        /// </summary>
        public Matrix Projection
        {
            get { return _projection; }
            set { _projection = value; }
        }


        public Matrix CullingProjection
        {
            get { return _culling; }
            set { _culling = value; }
        }

        /// <summary>
        /// The trapezoid that contains the viewable area
        /// </summary>
        public BoundingFrustum Frustum
        {
            get { return _frustum; }
            set { _frustum = value; }
        }

        /// <summary>
        /// The reflected trapezoid that contains everything the camera can see on a plane
        /// </summary>
        public BoundingFrustum ReflectedFrustum
        {
            get { return _reflectedFrustum; }
            set { _reflectedFrustum = value; }
        }

        /// <summary>
        /// Gets or sets if our screen is shaking
        /// </summary>
        public virtual Boolean Shaking
        {
            get { return shaking; }
            set { shaking = value; }
        }

        /// <summary>
        /// Gets or sets our temporary look at vector for matrix creating
        /// </summary>
        public Vector3 LookAt
        {
            get { return _lookAt; }
            set { _lookAt = value; }
        }

        /// <summary>
        /// Sets the position in 3D space
        /// </summary>
        public virtual void SetPosition(Vector3 pos) { }

        /// <summary>
        /// Sets the point in 3D space where the camera is looking
        /// </summary>
        /// <param name="reference"></param>
        public virtual void SetReference(Vector3 reference) { }

        /// <summary>
        /// Moves the camera in 3D space
        /// </summary>
        public virtual void Translate(Vector3 move) { }

        /// <summary>
        /// Rotate around the Z axis(roll)
        /// </summary>
        /// <param name="angle"></param>
        public virtual void RotateZ(float angle) { }

        /// <summary>
        /// Rotate around the Y axis(yaw)
        /// </summary>
        /// <param name="angle">Angle in degrees</param>
        public virtual void RotateY(float angle) { }

        /// <summary>
        /// Rotate around the X axis(pitch)
        /// </summary>
        /// <param name="angle"></param>
        public virtual void RotateX(float angle) { }

        /// <summary>
        /// Updates the camera
        /// </summary>
        public virtual void Update(GameTime gameTime) { }

        /// <summary>
        /// Resets our camera
        /// </summary>
        public virtual void Reset() { }

        /// <summary>
        /// Math Helper to generate a random float in a range from -1 to 1
        /// </summary>
        private float NextFloat
        {
            get { return (float)GameBase.Random.NextDouble() * 2f - 1f; }
        }

        /// <summary>
        /// Sets our shaking related information to our camera
        /// </summary>
        public void Shake(float magnitude, float duration)
        {
            //We're now shaking
            Shaking = true;

            //Store our info
            shakeMagnitude = magnitude;
            shakeDuration = duration;

            //Reset timer
            shakeTimer = 0;

            //Set our vibration
            GameBase.Input.Vibrate(PlayerIndex.One, 0.5f, 0.5f, duration);
        }

        /// <summary>
        /// Updates our shake duration and strength
        /// </summary>
        public void UpdateShake(GameTime gameTime)
        {
            if (Shaking)
            {
                //Move our timer ahead
                shakeTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

                //If we are at max or over, stop shaking
                if (shakeTimer >= shakeDuration)
                {
                    Shaking = false;
                    shakeTimer = shakeDuration;
                }

                //Compute our progress in a range of 0-1;
                float progress = shakeTimer / shakeDuration;

                //Now compute our magnitude based on progress, this will
                //reduce the shake as time goes on.
                float magnitude = shakeMagnitude * (1f - (progress * 2));

                shakeOffset = new Vector3(NextFloat, NextFloat, NextFloat) * magnitude;
            }
        }
    }
}
