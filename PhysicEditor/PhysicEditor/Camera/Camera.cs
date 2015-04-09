using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PhysicEditor
{
    public static class Camera
    {
        public static Matrix View;
        public static Matrix Projection;
        public static float Zoom = 1.0f;
        public static Vector3 CameraReference = new Vector3(0,-1,1);
        
        public static void Update()
        {
            View = 
             Matrix.CreateLookAt(CameraReference, Vector3.Zero, Vector3.Up);
            
            Projection = Matrix.CreateOrthographic((Program.mainGame.GraphicsDevice.Viewport.Width / 2) + Zoom, 
                (Program.mainGame.GraphicsDevice.Viewport.Height / 2) + Zoom, -5000f, 5000f);
        }

        public static void SetCameraOverhead()
        {
            CameraReference = new Vector3(0, 0.1f, 1);
        }

        public static void SetCameraFrontal()
        {
            CameraReference = new Vector3(0, -1, 1);
       
        }

        public static void SetCameraSide()
        {
            CameraReference = new Vector3(-1,-1,0);
        }


        public static void DefaultZoom()
        {
            Zoom = 0;
        }

        public static void ZoomControl(bool In)
        {
            if (In)
            {
                Zoom += 10f;
            }
            else
            {
                Zoom -= 10f;
            }


        }

    }
}
