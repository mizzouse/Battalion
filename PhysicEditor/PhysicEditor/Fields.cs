using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PhysicEditor.Fields
{
    public class Fields
    {
        public static AxisLock AxisMode;

        public static bool AdjustingModel;
        public static ModelMode Models;
        
        public static bool AdjustingSphere;
        public static SphereMode Spheres;

        public static bool AdjustingCamera;
        public static CameraMode Camera;              

        public static bool AnimationRunning;
        public static int AnimationStep;
        public static int Animation;

        public static bool Increasing;

        public static void SetAllToFalse()
        {
            AdjustingModel = false;
            AdjustingSphere = false;
            AdjustingCamera = false;            
        }
    }

    public enum AxisLock
    {
        X,
        Y,
        Z
    }

    public enum SphereMode
    {
        Radius,
        Anchor,
    }

    public enum CameraMode
    {
        ZoomIn,
        ZoomOut,
        Default
    }

    public enum ModelMode
    {
        X,
        Y,
        Z,
        MoveUp,
        MoveDown,
        MoveRight,
        MoveLeft,
        Moving
    }
}
