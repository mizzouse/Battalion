using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Infantry.Cameras
{
    public class ThirdPersonCamera : Camera
    {
        private Vector3 _position;
        private Vector3 _velocity;
        private Vector3 _chasePosition;
        private Vector3 _chaseDirection;
        private Vector3 _up = Vector3.Up;
        private Vector3 _desiredPosition;

        /// <summary>
        /// Updates our camera position in 3D space
        /// </summary>
        public override void SetPosition(Vector3 pos)
        {
            Position = pos;
        }

        /// <summary>
        /// Updates the camera
        /// </summary>
        public override void Update(GameTime gameTime)
        {
            UpdatePosition();

            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Vector3 force = -1800f * _position - 600f * _velocity;

            Vector3 accel = force / 50.0f;
            _velocity += accel * elapsed;
            _position += _velocity * elapsed;

            UpdateMatrix();

            UpdateShake(gameTime);
        }

        private void UpdatePosition()
        {
            Matrix transform = Matrix.Identity;
            transform.Forward = _chaseDirection;
            transform.Up = _up;
            transform.Right = Vector3.Cross(_up, _chaseDirection);

            _desiredPosition = _chasePosition + Vector3.TransformNormal(new Vector3(0, 2.0f, 2.0f),
                transform);
            LookAt = _chasePosition + Vector3.TransformNormal(new Vector3(0, 2.8f, 0),
                transform);
        }

        private void UpdateMatrix()
        {
            View = Matrix.CreateLookAt(Position, LookAt, Vector3.Up);

            Frustum = new BoundingFrustum(Matrix.Multiply(View, Projection));
            ReflectedFrustum = new BoundingFrustum(Matrix.Multiply(ReflectedView, Projection));
        }

        /// <summary>
        /// Resets our camera's position and matrix
        /// </summary>
        public override void Reset()
        {
            UpdatePosition();

            _velocity = Vector3.Zero;
            Position = _desiredPosition;

            UpdateMatrix();
        }
    }
}
