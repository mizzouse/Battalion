using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Infantry.Screens;
using Infantry.Managers;

namespace Infantry.Screens
{
    abstract class MenuScreen : ScreenLayer
    {
        List<ButtonEntry> _buttonEntries = new List<ButtonEntry>();
        List<BoxEntry> _boxEntries = new List<BoxEntry>();
        List<CheckBoxEntry> _checkboxEntries = new List<CheckBoxEntry>();
        int _selectedButton = 0;
        int _selectedBox = 0;
        int _selectedCheckBox = 0;
        string _title;

        /// <summary>
        /// Gets the list of button entries so classes can add/remove contents
        /// </summary>
        protected IList<ButtonEntry> ButtonEntries
        {
            get { return _buttonEntries; }
        }

        /// <summary>
        /// Gets the list of box entries so classes can add/remove them
        /// </summary>
        protected IList<BoxEntry> BoxEntries
        {
            get { return _boxEntries; }
        }

        /// <summary>
        /// Gets the list of checkbox entries so classes can add/remove them
        /// </summary>
        protected IList<CheckBoxEntry> CheckBoxEntries
        {
            get { return _checkboxEntries; }
        }

        /// <summary>
        /// Gets the last fired button that was pushed
        /// </summary>
        public ButtonEntry LastButtonClicked
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the last fired box that was clicked on
        /// </summary>
        public BoxEntry LastBoxClicked
        {
            get;
            private set;
        }

        /// <summary>
        /// Menu Screen Constructor using a menu title
        /// </summary>
        /// <param name="Title">Title that will be displayed on top</param>
        public MenuScreen(string Title)
        {
            _title = Title;
            TransOnTime = TimeSpan.FromSeconds(0.5);
            TransOffTime = TimeSpan.FromSeconds(0.5);
        }

        /// <summary>
        /// Responds to user input, changing the entry.
        /// </summary>
        /// <param name="input"></param>
        public override void HandleInput(Handlers.InputHandler input)
        {
            //Move to the previous entry?
            if (input.MenuUp)
            {
                _selectedButton--;
                if (_selectedButton < 0)
                    _selectedButton = _buttonEntries.Count - 1;
            }
            
            //Move to the next entry?
            if (input.MenuDown)
            {
                _selectedButton++;
                if (_selectedButton >= _buttonEntries.Count)
                    _selectedButton = 0;
            }

            //Move to the previous box?
            if (input.MenuLeft)
            {
                _selectedBox--;
                if (_selectedBox < 0)
                    _selectedBox = _boxEntries.Count - 1;
            }

            //Move to the next box?
            if (input.MenuRight)
            {
                _selectedBox++;
                if (_selectedBox >= _boxEntries.Count)
                    _selectedBox = 0;
            }

            //Now search for mouse hovering and input
#if !XBOX || !XBOX360
            int i = 0;
            foreach (ButtonEntry entry in _buttonEntries)
            {
                if (input.isHighlighting(entry.GetRectangle))
                {
                    _selectedButton = i;
                    if (input.hasClickedOn(entry.GetRectangle))
                        OnSelectButton(_selectedButton);
                }

                i++;
            }

            i = 0;
            foreach (BoxEntry entry in _boxEntries)
            {
                if (input.isHighlighting(entry.GetRectangle))
                {
                    _selectedBox = i;
                    if (input.hasClickedOn(entry.GetRectangle))
                        OnSelectBox(_selectedBox);
                }

                i++;
            }

            i = 0;
            foreach (CheckBoxEntry entry in _checkboxEntries)
            {
                if (input.isHighlighting(entry.GetRectangle))
                {
                    _selectedCheckBox = i;
                    if (input.hasClickedOn(entry.GetRectangle))
                        OnSelectCheckBox(_selectedCheckBox);
                }

                i++;
            }
#endif
            //Accept or cancel the entry?
            if (input.MenuSelect)
            {
                OnSelectButton(_selectedButton);
                OnSelectBox(_selectedBox);
            }
            else if (input.MenuCancel)
                OnCancel();
        }

        /// <summary>
        /// Notifies classes that an entry has been chosen
        /// </summary>
        protected virtual void OnSelectButton(int index)
        {
            _buttonEntries[_selectedButton].OnSelect();
            LastButtonClicked = _buttonEntries[_selectedButton];
        }

        /// <summary>
        /// Notifies classes that a box entry has been chosen
        /// </summary>
        protected virtual void OnSelectBox(int index)
        {
            _boxEntries[_selectedBox].OnSelect();
            LastBoxClicked = _boxEntries[_selectedBox];
        }

        /// <summary>
        /// Notifies classes that a checkbox entry has been chosen
        /// </summary>
        protected virtual void OnSelectCheckBox(int index)
        {
            _checkboxEntries[_selectedCheckBox].OnSelect();
        }

        /// <summary>
        /// Notifies classes that an entry has been cancelled
        /// </summary>
        protected virtual void OnCancel()
        {
            ExitScreen();
        }

        /// <summary>
        /// Uses OnCancel as a button entry event handler
        /// </summary>
        protected void OnCancel(object sender, EventArgs e)
        {
            OnCancel();
        }

        /// <summary>
        /// Updates the menu, checks for user input activity on buttons
        /// </summary>
        /// <param name="screenFocus">Are we in focus?</param>
        /// <param name="covered">Are we covered by another?</param>
        public override void Update(GameTime gameTime, bool screenFocus, bool covered)
        {
            base.Update(gameTime, screenFocus, covered);

            for (int i = 0; i < _buttonEntries.Count; i++)
            {
                bool select = IsActive && (i == _selectedButton);
                _buttonEntries[i].Update(this, select, gameTime);
            }

            for (int i = 0; i < _boxEntries.Count; i++)
            {
                bool select = IsActive && (i == _selectedBox);
                _boxEntries[i].Update(this, select, gameTime);
            }

            for (int i = 0; i < _checkboxEntries.Count; i++)
            {
                bool select = IsActive && (i == _selectedCheckBox);
                _checkboxEntries[i].Update(this, select, gameTime);
            }
        }

        /// <summary>
        /// Draws the menu
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            Vector2 position = Position;

            //Makes the menu slide into place during transition
            //Using math.Pow, it makes the movement slow down near the end
            float offset = (float)Math.Pow(TransPosition, 2);
            bool StateOn = false;
            if (ScreenState == Screens.ScreenState.On)
            {
                position.X -= offset * 256;
                StateOn = true;
            }
            else
                position.X += offset * 512;

            ScreenManager.SpriteBatch.Begin();

            //Draws each button entry
            //Makes each entry slide horizontally into place
            //using a position modifier
            //State on = right to left
            //State off = left to right
            for (int i = 0; i < _buttonEntries.Count; i++)
            {
                ButtonEntry entry = _buttonEntries[i];
                bool selected = IsActive && (i == _selectedButton);

                entry.Draw(this, position, selected, gameTime);
                //Make sure to set our button spacing,
                //otherwise itll stack on top of eachother
                position.Y += entry.GetHeight(this);
            }

            //Draws each box entry
            //Makes each entry slide horizontally into place
            //using a position modifier
            //State on = left to right
            //State off = right to left
            for (int i = 0; i < _boxEntries.Count; i++)
            {
                BoxEntry box = _boxEntries[i];
                bool select = IsActive && (i == _selectedBox);
                Vector2 newPos = Vector2.Zero;
                if (StateOn)
                    newPos.X += offset * 256;
                else
                    newPos.X -= offset * 512;
                box.Draw(this, newPos, select, gameTime);
            }

            //Draws each checkbox entry
            //Internally in each entry, we fade in
            for (int i = 0; i < _checkboxEntries.Count; i++)
            {
                CheckBoxEntry check = _checkboxEntries[i];
                bool select = IsActive && (i == _selectedCheckBox);
                check.Draw(this, position, select, gameTime);
            }

            //Draws the menu title
            if (DrawTitle)
            {
                Viewport viewport = GameManager.Device.Viewport;
                Vector2 titlePos = new Vector2(viewport.Width / 2, viewport.Height / 25);
                Vector2 titleOrigin = ScreenManager.Font.MeasureString(_title) / 2;
                Color color = new Color(192, 192, 192, Alpha);
                float scale = 1.25f;

                titlePos.Y -= offset * 100;
                ScreenManager.SpriteBatch.DrawString(ScreenManager.Font, _title, titlePos, color,
                    0, titleOrigin, scale, SpriteEffects.None, 0);
            }

            ScreenManager.SpriteBatch.End();
        }
    }
}
