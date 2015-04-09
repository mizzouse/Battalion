using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;

using Infantry.Handlers;
using Infantry.Managers;

namespace Infantry.Screens
{
    public enum ScreenState
    {
        On,
        Off,
        Active,
        Hidden
    }

    /// <summary>
    /// A Screen Layer is a single base screen that has the update and draw
    /// logics that can be added together with other screens.
    /// </summary>
    public abstract class ScreenLayer
    {
        private bool _isPopup = false;
        private TimeSpan _transOnTime = TimeSpan.Zero;
        private TimeSpan _transOffTime = TimeSpan.Zero;
        private float _transPosition = 1;
        private ScreenState _screenState = ScreenState.On;
        private bool _isExiting = false;
        private bool _otherHasFocus;
        //private Vector2 _position = new Vector2(300, 400);
        private Vector2 _position = new Vector2(GameManager.ViewingSize.Width / 2, GameManager.ViewingSize.Height / 1.2f);
        private bool _drawTitle = true;

        /// <summary>
        /// This indicates whether this screen is only a popup screen/menu
        /// </summary>
        public bool IsPopup
        {
            get { return _isPopup; }
            set { _isPopup = value; }
        }

        /// <summary>
        /// Gets or sets the starting text position(buttons) for this layer
        /// </summary>
        public Vector2 Position
        {
            get { return _position; }
            set { _position = value; }
        }

        /// <summary>
        /// Do we want to draw our screen title?
        /// </summary>
        public bool DrawTitle
        {
            get { return _drawTitle; }
            set { _drawTitle = value; }
        }

        /// <summary>
        /// Indicates how long the screen takes to transition on
        /// </summary>
        public TimeSpan TransOnTime
        {
            get { return _transOnTime; }
            set { _transOnTime = value; }
        }

        /// <summary>
        /// Indicates how long the screen takes to transition off
        /// </summary>
        public TimeSpan TransOffTime
        {
            get { return _transOffTime; }
            set { _transOffTime = value; }
        }

        /// <summary>
        /// Gets the current position of the screen, range is 0(active) - 1(off)
        /// </summary>
        public float TransPosition
        {
            get { return _transPosition; }
            set { _transPosition = value; }
        }

        /// <summary>
        /// Gets the current alpha color position of the transition using
        /// a percentage from 0-1 of the transition position
        /// </summary>
        public byte Alpha
        {
            get { return (byte)(255 - TransPosition * 255); }
        }

        /// <summary>
        /// Gets the current screen transition state
        /// </summary>
        public ScreenState ScreenState
        {
            get { return _screenState; }
            set { _screenState = value; }
        }

        /// <summary>
        /// Is the screen exiting? If so itll remove itself
        /// </summary>
        public bool IsExiting
        {
            get { return _isExiting; }
            set { _isExiting = value; }
        }

        /// <summary>
        /// Is this screen active?
        /// </summary>
        public bool IsActive
        {
            get
            {
                return !_otherHasFocus
                    && (_screenState == ScreenState.On || _screenState == ScreenState.Active);
            }
        }

        public virtual void LoadContent() { }
        public virtual void UnloadContent() { }

        /// <summary>
        /// Allows the screen to run logic
        /// </summary>
        /// <param name="screenFocus">Is the other screen in focus?</param>
        /// <param name="covered">Are we covered?</param>
        public virtual void Update(GameTime gameTime, bool screenFocus, bool covered)
        {
            _otherHasFocus = screenFocus;
            if (_isExiting)
            {
                //If the screen is going away
                _screenState = ScreenState.Off;
                if (!UpdateTransition(gameTime, _transOffTime, 1))
                {
                    //When the transition finishes
                    ScreenManager.RemoveScreen(this);
                    _isExiting = false;
                }
            }
            else if (covered)
            {
                //If the screen is covered
                if (UpdateTransition(gameTime, _transOffTime, 1))
                    _screenState = ScreenState.Off;
                else
                    _screenState = ScreenState.Hidden;
            }
            else
            {
                if (UpdateTransition(gameTime, _transOnTime, -1))
                    //Still busy transitioning
                    _screenState = ScreenState.Off;
                else
                    _screenState = ScreenState.Active;
            }
        }

        /// <summary>
        /// Updates the screen transition position
        /// </summary>
        private bool UpdateTransition(GameTime gameTime, TimeSpan time, int dir)
        {
            //How much should we move by?
            float transDelta;

            if (time == TimeSpan.Zero)
                transDelta = 1;
            else
                transDelta = (float)(gameTime.ElapsedGameTime.TotalMilliseconds / time.TotalMilliseconds);

            //Update the transition position
            _transPosition += transDelta * dir;

            //Did we reach the end?
            if ((_transPosition <= 0) || (_transPosition >= 1))
            {
                _transPosition = MathHelper.Clamp(_transPosition, 0, 1);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Allows the screen to handle user input
        /// </summary>
        public virtual void HandleInput(InputHandler input) { }

        /// <summary>
        /// Called when the screen should draw itself
        /// </summary>
        public virtual void Draw(GameTime gameTime) { }

        /// <summary>
        /// This method checks the transition time first before exiting
        /// </summary>
        public void ExitScreen()
        {
            if (TransOffTime == TimeSpan.Zero)
                ScreenManager.RemoveScreen(this);
            else
                _isExiting = true;
        }
    }
}
