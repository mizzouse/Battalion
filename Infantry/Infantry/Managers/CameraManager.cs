using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;

using Infantry.Cameras;

namespace Infantry.Managers
{
    public sealed class CameraManager : GameComponent
    {
        public enum CameraType
        {
            Overhead = 0,
            FirstPerson = 1,
            ThirdPerson = 2,
        }

        private static Dictionary<int, Camera> _cameras = new Dictionary<int, Camera>();
        private static bool _initialized = false;
        private static Camera _activeCamera;

        /// <summary>
        /// Is the Camera Manager initialized?
        /// </summary>
        public static bool Initialized
        {
            get { return _initialized; }
        }

        /// <summary>
        /// The current action camera
        /// </summary>
        public static Camera ActiveCamera
        {
            get { return _activeCamera; }
        }

        /// <summary>
        /// Our Camera Manager Constructor
        /// </summary>
        public CameraManager(Game game)
            : base(game)
        {
            Enabled = true;
        }

        /// <summary>
        /// Creates the camera's then initializes them
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();

            AddCamera((int)CameraType.Overhead, new OverheadCamera());
            SetActiveCamera((int)CameraType.Overhead);

            AddCamera((int)CameraType.FirstPerson, new FirstPersonCamera());
            AddCamera((int)CameraType.ThirdPerson, new ThirdPersonCamera());

            _initialized = true;
        }

        /// <summary>
        /// Updates the active camera
        /// </summary>
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            _activeCamera.Update(gameTime);
        }

        /// <summary>
        /// Changes the active camera
        /// </summary>
        /// <param name="camType">What cam type we are using</param>
        /// <param name="cam">The camera we are adding</param>
        public static void AddCamera(int camType, Camera cam)
        {
            if (!_cameras.ContainsKey(camType))
                _cameras.Add(camType, cam);
        }

        /// <summary>
        /// Sets the camera to active
        /// </summary>
        /// <param name="camType">The camera number</param>
        public static void SetActiveCamera(int camType)
        {
            if (_cameras.ContainsKey(camType))
                _activeCamera = _cameras[camType];
        }

        /// <summary>
        /// Sets all the culling projections(frustum trapezoid) viewable area
        /// </summary>
        public static void SetAllFrustums(float near, float far, float aspectRatio)
        {
            foreach (Camera cam in _cameras.Values)
                cam.CullingProjection = Matrix.CreatePerspectiveFieldOfView(cam.FieldOfView,
                    aspectRatio, near, far);
        }

        /// <summary>
        /// Sets all the projection matrices for all cameras including culling projection
        /// </summary>
        /// <param name="aspect">The aspect ratio</param>
        public static void SetAllProjections(float aspect)
        {
            foreach (Camera cam in _cameras.Values)
            {
                cam.Projection = Matrix.CreatePerspectiveFieldOfView(cam.FieldOfView,
                    aspect, cam.NearPlane, cam.FarPlane);
                cam.CullingProjection = Matrix.CreatePerspectiveFieldOfView(cam.FieldOfView, 
                    cam.NearPlane, cam.FarPlane, aspect);
            }
        }
    }
}
