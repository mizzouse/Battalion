using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Infantry.Cameras
{
    public class FirstPersonCamera : Camera
    {
        private Vector3 _cameraReference = new Vector3(0, 0, 10);
        private float _yaw = 0.0f;
        private float _pitch = 0.0f;
        private float _zoom;

        /// <summary>
        /// The spot in 3D space the camera is looking
        /// </summary>
        public Vector3 CameraReference
        {
            get { return _cameraReference; }
        }

        /// <summary>
        /// Our zoom in/out first person mode
        /// </summary>
        public float Zoom
        {
            get { return _zoom; }
            set 
            {
                _zoom = value;
                if (_zoom < 0.1f)
                    _zoom = 0.1f;
                else if (_zoom > 5000f)
                    _zoom = 5000f;
            }
        }

        /// <summary>
        /// Sets the position of our camera in 3D space
        /// </summary>
        public override void SetPosition(Vector3 pos)
        {
            Position = pos;
        }

        /// <summary>
        /// Sets the point in 3D space where the camera is looking
        /// </summary>
        public override void SetReference(Vector3 reference)
        {
            _cameraReference = reference;
        }

        /// <summary>
        /// Moves the camera in 3D space
        /// </summary>
        public override void Translate(Vector3 move)
        {
            Matrix forward = Matrix.CreateRotationY(_yaw);
            Vector3 vect = new Vector3(0, 0, 0);
            vect = Vector3.Transform(move, forward);

            Position += new Vector3(vect.X, vect.Y, vect.Z);
        }

        /// <summary>
        /// Rotates the camera around the X axis(pitch)
        /// </summary>
        /// <param name="angle">The angle in degrees</param>
        public override void RotateX(float angle)
        {
            angle = MathHelper.ToRadians(angle);
            if (_pitch >= MathHelper.ToRadians(75))
                _pitch = MathHelper.ToRadians(75);
            else if (_pitch <= MathHelper.ToRadians(-75))
                _pitch = MathHelper.ToRadians(-75);

            _pitch += angle;
        }

        /// <summary>
        /// Rotates the camera around the Y axis(yaw)
        /// </summary>
        /// <param name="angle">The angle in degrees</param>
        public override void RotateY(float angle)
        {
            angle = MathHelper.ToRadians(angle);
            if (_yaw >= MathHelper.TwoPi)
                _yaw = MathHelper.ToRadians(0.0f);
            else if (_yaw <= -MathHelper.TwoPi)
                _yaw = MathHelper.ToRadians(0.0f);

            _yaw += angle;
        }

        /// <summary>
        /// Updates the camera
        /// </summary>
        public override void Update(GameTime gameTime)
        {
            Vector3 cameraPos = Position;
            Matrix rotation = Matrix.CreateRotationY(_yaw);
            Matrix pitch = Matrix.Multiply(Matrix.CreateRotationX(_pitch), rotation);
            Vector3 transformed = Vector3.Transform(_cameraReference, pitch);
            LookAt = cameraPos + transformed;

            View = Matrix.CreateLookAt(cameraPos, LookAt, Vector3.Up);

            Frustum = new BoundingFrustum(Matrix.Multiply(View, CullingProjection));
            ReflectedFrustum = new BoundingFrustum(Matrix.Multiply(ReflectedView, CullingProjection));

            UpdateShake(gameTime);
        }
    }
}
