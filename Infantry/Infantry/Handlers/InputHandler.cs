using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using Infantry.Plugins;

namespace Infantry.Handlers
{
    public class InputHandler : GameComponent
    {
        /// <summary>
        /// Current Keyboard State
        /// </summary>
        public KeyboardState CurrentKB;
        /// <summary>
        /// Current GamePad State
        /// </summary>
        public GamePadState CurrentGP;
        /// <summary>
        /// Last Keyboard State
        /// </summary>
        public KeyboardState LastKB;
        /// <summary>
        /// Last GamePad State
        /// </summary>
        public GamePadState LastGP;
        /// <summary>
        /// Array of keys we use
        /// </summary>
        public Keys[] keys;
        private TimeSpan mouseHeldTime = TimeSpan.Zero;
        private TimeSpan keyHeldTime = TimeSpan.Zero;
        private bool capsLock = WinFormKeys.IsCapsLocked();
        private bool numLock = WinFormKeys.IsNumLocked();
        private bool scrollLock = WinFormKeys.IsScrollLocked();

        #region Controller Vibration Settings
        /// <summary>
        /// GamePad Left Side Vibration Motor
        /// </summary>
        public float LeftMotor;
        /// <summary>
        /// GamePad Right Side Vibration Motor
        /// </summary>
        public float RightMotor;
        /// <summary>
        /// Vibration Duration
        /// </summary>
        public float Duration;
        /// <summary>
        /// When to Vibrate
        /// </summary>
        public float Timer = 0f;
        #endregion

#if !XBOX || !XBOX360
        /// <summary>
        /// Current Mouse State
        /// </summary>
        public MouseState CurrentMouse;
        /// <summary>
        /// Last Mouse State
        /// </summary>
        public MouseState LastMouse;
        private Point lastMouseLocation;
        private Vector2 _mouseMoved;
        /// <summary>
        /// The coordinates that our mouse moved to
        /// </summary>
        public Vector2 MouseMoved
        {
            get { return _mouseMoved; }
        }
#endif

        #region Constructors and Initiator
        /// <summary>
        /// Our input handler Constructor
        /// </summary>
        /// <param name="game"></param>
        public InputHandler(Game game)
            : base(game)
        {
            //Calls our update method when game update is called
            Enabled = true;

            Init();
        }

        /// <summary>
        /// Initializes a new key structure
        /// Note: mainly used for the chatbox
        /// </summary>
        internal void Init()
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
        /// Creates a vibration setting for a given player.
        /// </summary>
        /// <param name="player">Which player index is it</param>
        /// <param name="left">Left motors strength</param>
        /// <param name="right">Right Motors strength</param>
        /// <param name="duration">How long is the vibration for</param>
        public void Vibrate(PlayerIndex player, float left, float right, float duration)
        {
            LeftMotor = left;
            RightMotor = right;
            Duration = duration;
            Timer = 0f;

            //Sets our vibration
            GamePad.SetVibration(player, left, right);
        }
        #endregion

        #region Input Updates
        /// <summary>
        /// Internally updates the vibration of game pads
        /// </summary>
        internal void UpdateVibration()
        {
            if (Timer >= Duration)
                GamePad.SetVibration(PlayerIndex.One, 0, 0);
            else
            {
                float progress = Timer / Duration;
                float left = LeftMotor * (1f - progress);
                float right = RightMotor * (1f - progress);

                GamePad.SetVibration(PlayerIndex.One, left, right);
            }
        }

        /// <summary>
        /// Reads the input states of connected controllers
        /// </summary>
        public void InputUpdate()
        {
            LastKB = CurrentKB;
            LastGP = CurrentGP;

            CurrentKB = Keyboard.GetState();
            CurrentGP = GamePad.GetState(PlayerIndex.One);

#if !XBOX || !XBOX360
            LastMouse = CurrentMouse;
            CurrentMouse = Mouse.GetState();
            _mouseMoved = new Vector2(LastMouse.X - CurrentMouse.X,
                LastMouse.Y - CurrentMouse.Y);
            lastMouseLocation = new Point(CurrentMouse.X, CurrentMouse.Y);
#endif

            //Update lock buttons
            capsLock = WinFormKeys.IsCapsLocked();
            numLock = WinFormKeys.IsNumLocked();
            scrollLock = WinFormKeys.IsScrollLocked();
        }

        /// <summary>
        /// Updates the scroll time upon gametime update
        /// </summary>
        public override void Update(GameTime gameTime)
        {
            if (keyHeldTime.TotalMilliseconds > TimeSpan.Zero.TotalMilliseconds)
                keyHeldTime -= gameTime.ElapsedGameTime;

            if (mouseHeldTime.TotalMilliseconds > TimeSpan.Zero.TotalMilliseconds)
                mouseHeldTime -= gameTime.ElapsedGameTime;

            float time = (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (Timer < Duration)
            {
                Timer = (float)Math.Min(Timer + time, Duration);
                UpdateVibration();
            }
        }
        #endregion

        #region Keyboard and GamePad Actions
        /// <summary>
        /// Gets a current typed input and returns a string(character)
        /// </summary>
        public string GetTypedInput
        {
            get { return HandleInput(); }
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

        /// <summary>
        /// Is the scroll lock on?
        /// </summary>
        public bool isScrollLockOn
        {
            get { return ScrollLockOn(); }
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

        /// <summary>
        /// Determins if the key was just released
        /// </summary>
        /// <param name="key">Key in question</param>
        /// <returns>Returns true if released</returns>
        public bool isReleased(Keys key)
        {
            if (CurrentKB.IsKeyUp(key) && LastKB.IsKeyDown(key))
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
            if (CurrentKB.IsKeyDown(key) && LastKB.IsKeyUp(key))
            {
                keyHeldTime = TimeSpan.FromSeconds(0.15f);
                return true;
            }

            return false;
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

            if (CurrentKB.IsKeyDown(key) && LastKB.IsKeyDown(key) 
                && keyHeldTime.TotalMilliseconds <= TimeSpan.Zero.TotalMilliseconds)
            {
                keyHeldTime = TimeSpan.FromSeconds(0.15f);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Checks for a menu/button up input action either on KB or GamePad
        /// </summary>
        public bool MenuUp
        {
            get
            {
                return isPressed(Keys.Up) || (CurrentGP.DPad.Up == ButtonState.Pressed
                    && LastGP.DPad.Up == ButtonState.Released) || (CurrentGP.ThumbSticks.Left.Y > 0
                    && LastGP.ThumbSticks.Left.Y <= 0);
            }
        }

        /// <summary>
        /// Checks for a menu/button down input action either on KB or GamePad
        /// </summary>
        public bool MenuDown
        {
            get
            {
                return isPressed(Keys.Down) || (CurrentGP.DPad.Down == ButtonState.Pressed
                    && LastGP.DPad.Down == ButtonState.Released) || (CurrentGP.ThumbSticks.Left.Y < 0
                    && LastGP.ThumbSticks.Left.Y >= 0);
            }
        }

        /// <summary>
        /// Checks for a menu/button left input action either on KB or GamePad
        /// </summary>
        public bool MenuLeft
        {
            get
            {
                return isPressed(Keys.Left) || (CurrentGP.DPad.Left == ButtonState.Pressed
                    && LastGP.DPad.Left == ButtonState.Released) || (CurrentGP.ThumbSticks.Left.X > 0
                    && LastGP.ThumbSticks.Left.X <= 0)
                    || (ShiftDown() && isPressed(Keys.Tab));
            }
        }

        /// <summary>
        /// Checks for a menu/button right input action either on KB or GamePad
        /// </summary>
        public bool MenuRight
        {
            get
            {
                return isPressed(Keys.Tab) || isPressed(Keys.Right) || (CurrentGP.DPad.Right == ButtonState.Pressed
                    && LastGP.DPad.Right == ButtonState.Released) || (CurrentGP.ThumbSticks.Right.X > 0
                    && LastGP.ThumbSticks.Right.X <= 0);
            }
        }

        /// <summary>
        /// Checks for a menu select input action on either KB or GamePad
        /// </summary>
        public bool MenuSelect
        {
            get
            {
                return /*isPressed(Keys.Space) ||*/ isPressed(Keys.Enter)
                    || isPressed(Keys.RightControl) || isPressed(Keys.LeftControl)
                    || (CurrentGP.Buttons.A == ButtonState.Pressed && LastGP.Buttons.A == ButtonState.Released)
                    || (CurrentGP.Buttons.Start == ButtonState.Pressed && LastGP.Buttons.Start == ButtonState.Released);
            }
        }

        /// <summary>
        /// Checks for a menu cancel input action on either KB or GamePad
        /// </summary>
        public bool MenuCancel
        {
            get
            {
                return isPressed(Keys.Escape) || (CurrentGP.Buttons.B == ButtonState.Pressed
                    && LastGP.Buttons.B == ButtonState.Released) || (CurrentGP.Buttons.Back == ButtonState.Pressed
                    && LastGP.Buttons.Back == ButtonState.Released);
            }
        }
        #endregion

        #region Mouse Actions
        /// <summary>
        /// Determines if the mouse button left/right is being held
        /// </summary>
        public bool isHolding
        {
            get
            {
                if (((CurrentMouse.LeftButton == ButtonState.Pressed
                    && LastMouse.LeftButton == ButtonState.Pressed) ||
                    (CurrentMouse.RightButton == ButtonState.Pressed
                    && LastMouse.RightButton == ButtonState.Pressed))
                    && mouseHeldTime.TotalMilliseconds <= TimeSpan.Zero.TotalMilliseconds)
                {
                    mouseHeldTime = TimeSpan.FromSeconds(0.15f);
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// Determines if a mouse button left/right has been released
        /// </summary>
        public bool hasReleased
        {
            get
            {
                if ((CurrentMouse.LeftButton == ButtonState.Released
                    && LastMouse.LeftButton == ButtonState.Pressed) ||
                    (CurrentMouse.RightButton == ButtonState.Released
                    && LastMouse.RightButton == ButtonState.Pressed))
                    return true;

                return false;
            }
        }

        /// <summary>
        /// Determines if a mouse button left/right has been pressed
        /// </summary>
        public bool hasPressed
        {
            get
            {
                if ((CurrentMouse.LeftButton == ButtonState.Pressed
                    && LastMouse.LeftButton == ButtonState.Released) ||
                    (CurrentMouse.RightButton == ButtonState.Pressed
                    && LastMouse.RightButton == ButtonState.Released))
                {
                    mouseHeldTime = TimeSpan.FromSeconds(0.15f);
                    return true;
                }

                return false;
            }
        }

        /// <summary>
        /// Is the mouse hovering over a specified rectangle?
        /// </summary>
        public bool isHighlighting(Rectangle rect)
        {
            if (rect.Contains(lastMouseLocation))
                return true;

            return false;
        }

        /// <summary>
        /// Determines if a rectangle is being dragged
        /// </summary>
        /// <param name="rect">The Rectangle</param>
        /// <returns>Returns true if is, false if not</returns>
        public bool isDragging(Rectangle rect)
        {
            if (rect.Contains(lastMouseLocation) && CurrentMouse.LeftButton == ButtonState.Pressed
                && LastMouse.LeftButton == ButtonState.Pressed)
                return true;

            return false;
        }

        /// <summary>
        /// Determines if the mouse clicked on a specified rectangle
        /// </summary>
        /// <param name="rect">The rectangle</param>
        /// <returns>Returns true if has, false if not</returns>
        public bool hasClickedOn(Rectangle rect)
        {
            if (rect.Contains(lastMouseLocation) && CurrentMouse.LeftButton == ButtonState.Pressed
                && LastMouse.LeftButton == ButtonState.Released)
                return true;

            return false;
        }
        #endregion

        #region Private Input Returns
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

        private bool ScrollLockOn()
        {
            if (scrollLock)
                return true;

            return false;
        }

        private string HandleInput()
        {
            string letter = "";
            for (int i = 0; i < keys.Length; i++)
            {
                if (isPressed(keys[i]))
                {
                    switch (keys[i])
                    {
                        case Keys.OemMinus:
                            if (ShiftDown())
                                letter = "_";
                            else
                                letter = "-";
                            break;
                        case Keys.OemComma:
                            if (ShiftDown())
                                letter = "<";
                            else
                                letter = ",";
                            break;
                        case Keys.OemQuestion:
                            if (ShiftDown())
                                letter = "?";
                            else
                                letter = "/";
                            break;
                        case Keys.OemQuotes:
                            if (ShiftDown())
                                letter = "\"";
                            else
                                letter = "'";
                            break;
                        case Keys.OemTilde:
                            if (ShiftDown())
                                letter = "`";
                            else
                                letter = "~";
                            break;
                        case Keys.OemSemicolon:
                            if (ShiftDown())
                                letter = ":";
                            else
                                letter = ";";
                            break;
                        case Keys.OemPlus:
                            if (ShiftDown())
                                letter = "+";
                            else
                                letter = "=";
                            break;
                        case Keys.OemOpenBrackets:
                            if (ShiftDown())
                                letter = "{";
                            else
                                letter = "[";
                            break;
                        case Keys.OemCloseBrackets:
                            if (ShiftDown())
                                letter = "}";
                            else
                                letter = "]";
                            break;
                        case Keys.OemBackslash:
                            letter = "\\";
                            break;
                        case Keys.OemPipe:
                            letter = "|";
                            break;
                        case Keys.OemPeriod:
                            if (ShiftDown())
                                letter = ">";
                            else
                                letter = ".";
                            break;
                        case Keys.D0:
                            if (ShiftDown())
                                letter = ")";
                            else
                                letter = "0";
                            break;
                        case Keys.D1:
                            if (ShiftDown())
                                letter = "!";
                            else
                                letter = "1";
                            break;
                        case Keys.D2:
                            if (ShiftDown())
                                letter = "@";
                            else
                                letter = "2";
                            break;
                        case Keys.D3:
                            if (ShiftDown())
                                letter = "#";
                            else
                                letter = "3";
                            break;
                        case Keys.D4:
                            if (ShiftDown())
                                letter = "$";
                            else
                                letter = "4";
                            break;
                        case Keys.D5:
                            if (ShiftDown())
                                letter = "%";
                            else
                                letter = "5";
                            break;
                        case Keys.D6:
                            if (ShiftDown())
                                letter = "^";
                            else
                                letter = "6";
                            break;
                        case Keys.D7:
                            if (ShiftDown())
                                letter = "&";
                            else
                                letter = "7";
                            break;
                        case Keys.D8:
                            if (ShiftDown())
                                letter = "*";
                            else
                                letter = "8";
                            break;
                        case Keys.D9:
                            if (ShiftDown())
                                letter = "(";
                            else
                                letter = "9";
                            break;
                        case Keys.Add:
                            letter = "+";
                            break;
                        case Keys.Subtract:
                            letter = "-";
                            break;
                        case Keys.Multiply:
                            letter = "*";
                            break;
                        case Keys.Divide:
                            letter = "/";
                            break;

                        default:
                            letter = keys[i].ToString();

                            if (isCapsLockOn)
                            {
                                if (ShiftDown())
                                    letter = letter.ToLower();
                                else
                                    letter = letter.ToUpper();
                            }
                            else
                            {
                                if (ShiftDown())
                                    letter = letter.ToUpper();
                            }
                            break;
                    }
                }
            }

            return letter;
        }
        #endregion
    }
}