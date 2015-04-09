using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using PhysicEditor;
using PhysicEditor.Models;
using PhysicEditor.Fields;

namespace PhysicEditor.Input
{
    public static class MouseControl
    {
        public static MouseState OldState, NewState;
        public static Vector3 OldMousePosition, NewMousePosition;
        
        public static void Update()
        {
            OldState = NewState;
            NewState = Mouse.GetState();

            OldMousePosition = NewMousePosition;
            NewMousePosition = mouseCoordsTo3D(NewState);
            
            //Adjusting values or placing an anchor
            if (NewState.LeftButton == ButtonState.Pressed && OldState.LeftButton == ButtonState.Pressed)
            {
                int Width = Program.mainGame.GraphicsDevice.PresentationParameters.BackBufferWidth / 2;
                if (Fields.Fields.AdjustingModel)
                {
                    GameModel model = Collection.GetGameModelByID(Program.CurrentModelID);

                    switch (Fields.Fields.Models)
                    {
                        case ModelMode.Moving:
                            model.Position = mouseCoordsTo3D(NewState);// - model.Position;
                            return;
                        case ModelMode.X:
                            if (NewState.X < Width)
                            {
                                model.RotationX -= 0.01f;                                
                            }
                            else
                            {
                                model.RotationX += 0.01f;
                            }
                            return;
                        case ModelMode.Y:
                            if (NewState.X < Width)
                            {
                                model.RotationY -= 0.01f;
                            }
                            else
                            {
                                model.RotationY += 0.01f;
                            }
                            return;
                        case ModelMode.Z:
                            if (NewState.X < Width)
                            {
                                model.RotationZ -= 0.01f;
                            }
                            else
                            {
                                model.RotationZ += 0.01f;
                            }
                            return;
                    }
                }

                if (Fields.Fields.AdjustingSphere)
                {
                    PSphere sphere = Collection.GetGameModelByID(Program.CurrentModelID).GetSphere(Program.CurrentSphereID);
                    switch (Fields.Fields.Spheres)
                    {
                        case SphereMode.Anchor:
                            switch (Fields.Fields.AxisMode)
                            {
                                case AxisLock.X:
                                    if (NewState.X < Width)
                                    {
                                        sphere.AnchorPoint.X -= 0.01f;
                                    }
                                    else
                                    {
                                        sphere.AnchorPoint.X += 0.01f;
                                    }
                                    return;
                                case AxisLock.Y:
                                    if (NewState.X < Width)
                                    {
                                        sphere.AnchorPoint.Y -= 0.01f;
                                    }
                                    else
                                    {
                                        sphere.AnchorPoint.Y += 0.01f;
                                    }
                                    return;
                                case AxisLock.Z:
                                    if (NewState.X < Width)
                                    {
                                        sphere.AnchorPoint.Z -= 0.01f;
                                    }
                                    else
                                    {
                                        sphere.AnchorPoint.Z += 0.01f;
                                    }
                                    return;
                            }
                            //Todo
                            return;
                        case SphereMode.Radius:
                         //   sphere.Sphere.Radius = Math.Abs(NewMousePosition.Length() - sphere.AnchorPoint.Length());
                            return;
                    }
                }

                //Placing an anchor for a sphere
                if (Program.IsPlacingAnchor)
                {
                    //Set the anchor point
                        if (Program.CurrentSphereID != 999)
                        {
                            Collection.GetGameModelByID(Program.CurrentModelID).GetSphere(Program.CurrentSphereID).SetAnchorPoint(NewMousePosition.X, 0, NewMousePosition.Z);
                        }
                      
                } 
            }

            //Stop adjusting mode
            if (NewState.RightButton == ButtonState.Pressed && OldState.RightButton == ButtonState.Pressed)
            {
             //   AdjustingCamera = false;
              //  AdjustingPhysics = false;
//AdjustingModel = false;
                Fields.Fields.SetAllToFalse();
                Program.IsPlacingAnchor = false;
            }

        }

        private static Vector3 mouseCoordsTo3D(MouseState ms)
        {
            //http://stackoverflow.com/questions/11503226/c-sharp-xna-mouse-position-projected-to-3d-plane

            Vector3 nearScreenPoint = new Vector3(ms.X, ms.Y, -5000);
            Vector3 farScreenPoint = new Vector3(ms.X, ms.Y, 5000);
            
            Matrix World = //Matrix.CreateScale(Collection.Models[Program.CurrentModelID].Scale);
                (Collection.Models.Count != 0 ? Collection.Models[Program.CurrentModelID].World : Matrix.Identity);

            Vector3 nearWorldPoint = Program.mainGame.GraphicsDevice.Viewport.Unproject(nearScreenPoint, Camera.Projection, Camera.View, Matrix.Identity);
            Vector3 farWorldPoint = Program.mainGame.GraphicsDevice.Viewport.Unproject(farScreenPoint, Camera.Projection, Camera.View, Matrix.Identity);

            Vector3 direction = farWorldPoint - nearWorldPoint;

            float zFactor = -nearWorldPoint.Y / direction.Y;
            Vector3 zeroWorldPoint = nearWorldPoint + direction * zFactor;
            //  
            return zeroWorldPoint;
        }

    }
}
