using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using PhysicEditor.Fields;
using PhysicEditor.Models;

namespace PhysicEditor.Input
{
    public class KeyboardInput
    {
        public KeyboardState current, previous;
        public Keys[] keys;
        private TimeSpan scrollTime = TimeSpan.Zero;
        private bool capsLock = false;
        private bool numLock = false;


        //Generic Constructor
        public KeyboardInput()
        {
            keys = new Keys[] {
                                  Keys.A, Keys.B, Keys.C, Keys.D, Keys.E, Keys.F, Keys.G, Keys.H, Keys.I,
                                  Keys.J, Keys.K, Keys.L, Keys.M, Keys.N, Keys.O, Keys.P, Keys.Q, Keys.R, 
                                  Keys.S, Keys.T, Keys.U, Keys.V, Keys.W, Keys.X, Keys.Y, Keys.Z, 
                                  Keys.OemMinus, Keys.OemComma, Keys.OemQuestion, Keys.OemQuotes,
                                  Keys.OemTilde, Keys.OemSemicolon, Keys.OemPlus, Keys.OemOpenBrackets,
                                  Keys.OemCloseBrackets, Keys.OemBackslash, Keys.OemPeriod, Keys.OemPipe,
                                  Keys.D0, Keys.D1, Keys.D2, Keys.D3, Keys.D4, Keys.D5, Keys.D6, Keys.D7,
                                  Keys.D8, Keys.D9, Keys.Divide, Keys.Multiply, Keys.Subtract, Keys.Add
                              };

        }

        /// <summary>
        /// Is either shift key being held?
        /// </summary>
        /// <returns>Returns true if held</returns>
        public bool isShiftDown
        {
            get { return ShiftDown(); }
        }

        /// <summary>
        /// Is the caps lock on?
        /// </summary>
        public bool isCapsLockOn
        {
            get { return CapsLockOn(); }
        }

        /// <summary>
        /// Is the num lock on?
        /// </summary>
        public bool isNumLockOn
        {
            get { return NumLockOn(); }
        }

        public void InputUpdate(GameTime gameTime)
        {
            scrollTime -= gameTime.ElapsedGameTime;

            previous = current;
            current = Keyboard.GetState();

            foreach (Keys k in current.GetPressedKeys())
            {
                if (k == Keys.OemMinus)
                {
                    Fields.Fields.Increasing = false;

                    if (Fields.Fields.AdjustingSphere)
                    {
                        switch (Fields.Fields.Spheres)
                        {
                            case SphereMode.Anchor:
                                return;
                            case SphereMode.Radius:
                                PSphere sphere = Collection.GetGameModelByID(Program.CurrentModelID).GetSphere(Program.CurrentSphereID);
                                sphere.Radius -= 0.1f;
                                return;
                        }
                    }
                }
                if (k == Keys.OemPlus)
                {
                    Fields.Fields.Increasing = true;
                    if (Fields.Fields.AdjustingSphere)
                    {
                        switch (Fields.Fields.Spheres)
                        {
                            case SphereMode.Anchor:
                                return;
                            case SphereMode.Radius:
                                PSphere sphere = Collection.GetGameModelByID(Program.CurrentModelID).GetSphere(Program.CurrentSphereID);
                                sphere.Radius += 0.1f;
                                return;
                        }
                    }
                }
                
                if (ShiftDown())
                {
                    switch (k)
                    {
                        case Keys.X:
                            Fields.Fields.AxisMode = AxisLock.X;
                            return;
                        case Keys.Y:
                            Fields.Fields.AxisMode = AxisLock.Y;
                            return;
                        case Keys.Z:
                            Fields.Fields.AxisMode = AxisLock.Z;
                            return;
                    }
                }

                if (isHeld(Keys.RightAlt) || isHeld(Keys.LeftAlt))
                {
                    switch (k)
                    {
                        case Keys.R:
                            Fields.Fields.AdjustingSphere = true;
                            Fields.Fields.Spheres = SphereMode.Radius;
                            return;
                        case Keys.S:
                            Fields.Fields.AnimationRunning = !Fields.Fields.AnimationRunning;
                            return;
                        case Keys.N:
                            Program.mainDialog.GoToNextAnimation();                            
                            return;
                        case Keys.I:
                            Fields.Fields.AnimationStep++;
                            return;
                        case Keys.M:
                            Fields.Fields.AdjustingSphere = true;
                            Fields.Fields.Spheres = SphereMode.Anchor;
                            return;
                    }
                }

                if (isHeld(Keys.RightControl) || isHeld(Keys.LeftControl))
                {
                    switch (k)
                    {
                        case Keys.Q:
                            Camera.SetCameraOverhead();
                            Collection.GetGameModelByID(Program.CurrentModelID).ResetRotations();
                            return;
                        case Keys.W:
                            Camera.SetCameraFrontal();
                            Collection.GetGameModelByID(Program.CurrentModelID).ResetRotations();
                            return;
                        case Keys.E:
                            Camera.SetCameraSide();
                            Collection.GetGameModelByID(Program.CurrentModelID).ResetRotations();
                            return;
                        case Keys.A:
                            //Add model
                            Collection.GetGameModelByID(Program.CurrentModelID).AddSphere();
                            return;
                        case Keys.D:
                            //Remove current model
                            Collection.GetGameModelByID(Program.CurrentModelID).RemoveSphere(Program.CurrentSphereID);
                            return;
                        case Keys.M:
                            Fields.Fields.AdjustingModel = true;
                            Fields.Fields.Models = ModelMode.Moving;
                            return;
                        case Keys.X:
                            Fields.Fields.AdjustingModel = true;
                            Fields.Fields.Models = ModelMode.X;
                            return;
                        case Keys.Y:
                            Fields.Fields.AdjustingModel = true;
                            Fields.Fields.Models = ModelMode.Y;
                            return;
                        case Keys.Z:
                            Fields.Fields.AdjustingModel = true;
                            Fields.Fields.Models = ModelMode.Z;
                            return;
                        

                        case Keys.I:
                            Camera.ZoomControl(true);
                            return;
                        case Keys.O:
                            Camera.ZoomControl(false);
                            return;
                        case Keys.R:
                            Camera.DefaultZoom();
                            return;
                    }
                }
               
            }
        }

        /// <summary>
        /// Use this to determine if the key is being held
        /// </summary>
        /// <param name="key">Key in question</param>
        /// <returns>Returns true if held</returns>
        public bool isHeld(Keys key)
        {
            if (key == Keys.RightShift || key == Keys.LeftShift)
                return ShiftDown();

            if (current.IsKeyDown(key) && previous.IsKeyDown(key) && scrollTime <= TimeSpan.Zero)
            {
                scrollTime = TimeSpan.FromSeconds(0.05f);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Determins if the key was just released
        /// </summary>
        /// <param name="key">Key in question</param>
        /// <returns>Returns true if released</returns>
        public bool isReleased(Keys key)
        {
            if (current.IsKeyUp(key) && previous.IsKeyDown(key))
                return true;

            return false;
        }

        /// <summary>
        /// Determins if the key is being pressed
        /// </summary>
        /// <param name="key">Key in question</param>
        /// <returns>Returns true if being pressed</returns>
        public bool isPressed(Keys key)
        {
            if (current.IsKeyDown(key) && previous.IsKeyUp(key))
            {
                scrollTime = TimeSpan.FromSeconds(0.25f);
                return true;
            }

            return false;
        }

        private bool ShiftDown()
        {
            KeyboardState k = Keyboard.GetState();
            if (k.IsKeyDown(Keys.LeftShift) || k.IsKeyDown(Keys.RightShift))
                return true;

            return false;
        }

        private bool CapsLockOn()
        {
            if (capsLock)
                return true;

            return false;
        }

        private bool NumLockOn()
        {
            if (numLock)
                return true;

            return false;
        }

        /// <summary>
        /// Toggles the caps lock button state
        /// </summary>
        public void toggleCapsLock()
        {
            capsLock = !capsLock;
        }

        /// <summary>
        /// Toggles the num lock button state
        /// </summary>
        public void toggleNumLock()
        {
            numLock = !numLock;
        }
    }
}
