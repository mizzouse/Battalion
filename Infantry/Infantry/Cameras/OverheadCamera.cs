using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Infantry.Cameras
{
    public class OverheadCamera : Camera
    {
        private Vector3 _cameraReference = new Vector3(0, 200, -200);
        private float _roll = 0.0f;

        /// <summary>
        /// The spot in 3D space the camera is looking
        /// </summary>
        public Vector3 CameraReference
        {
            get { return _cameraReference; }
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
        /// Rotates the camera around the Z axis(roll)
        /// </summary>
        /// <param name="angle">The angle in degrees</param>
        public override void RotateZ(float angle)
        {
            angle = MathHelper.ToRadians(angle);
            if (_roll >= MathHelper.ToRadians(75))
                _roll = MathHelper.ToRadians(75);
            else if (_roll <= MathHelper.ToRadians(-75))
                _roll = MathHelper.ToRadians(-75);

            _roll += angle;
        }

        /// <summary>
        /// Updates the camera
        /// </summary>
        public override void Update(GameTime gameTime)
        {
            Matrix transform = Matrix.CreateTranslation(new Vector3(Position.X, Position.Y, 0)) *
                                                        Matrix.CreateRotationZ(_roll) *
                                                        Matrix.CreateScale(1.0f) *
                                                        Matrix.CreateTranslation(GameBase.Device.Viewport.Width / 2,
                                                        GameBase.Device.Viewport.Height / 2, 0);
            Vector3 transformed = Vector3.Transform(_cameraReference, transform);
            Vector3 lookAt = Position + transformed;

            View = Matrix.CreateLookAt(Position, lookAt, Vector3.UnitZ);

            Frustum = new BoundingFrustum(Matrix.Multiply(View, Projection));
            ReflectedFrustum = new BoundingFrustum(Matrix.Multiply(ReflectedView, Projection));
        }
    }
}
