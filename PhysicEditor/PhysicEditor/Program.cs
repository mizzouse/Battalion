using System;
using PhysicEditor.Models;

namespace PhysicEditor
{
#if WINDOWS || XBOX
    public static class Program
    {
        public static int CurrentModelID;
        public static PType CurrentPType;
        public static int CurrentSphereID = 999;

        public static bool IsPlacingAnchor;
        public static bool IsLoading;
        public static bool IsWireMode;

        public static MainWindow mainGame;
        public static MainDialog mainDialog;

        public static AdjustPhysics PhysicsMode;

        public static bool Increasing;
        public static bool Decreasing;

        public static MovingModel ModelMovement;

        public enum MovingModel
        {
            None,
            Up,
            Down,
            Left,
            Right,            
        }

        public enum AdjustPhysics
        {
            None,
            Center,
            Radius,
            YAxis,
            ZAxis,
            XAxis,
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (MainWindow game = new MainWindow())
            {
                mainGame = game;
                game.Run();
            }
        }
    }
#endif
}

