using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework.Graphics;

using Infantry.Managers;

namespace Infantry.Objects
{
    public class InfTexture
    {
        private string _filename;
        private string _location;
        private Texture2D _base;
        private bool _renderReady = false;

        /// <summary>
        /// The file name of the asset
        /// </summary>
        public string FileName
        {
            get { return _filename; }
            set { _filename = value; }
        }

        /// <summary>
        /// Gets the path location of our texture
        /// </summary>
        public string PathLocation
        {
            get { return _location; }
        }

        /// <summary>
        /// Gets the loaded texture
        /// </summary>
        public Texture2D BaseTexture
        {
            get { return _base; }
        }

        /// <summary>
        /// Is this texture ready to be rendered?
        /// </summary>
        public bool RenderReady
        {
            get { return _renderReady; }
        }

        /// <summary>
        /// Texture Object Constructor
        /// </summary>
        public InfTexture() { }

        /// <summary>
        /// Texture Object Constructor with file name and path
        /// </summary>
        /// <param name="filename">The name of our file</param>
        /// <param name="path">The path location of our file</param>
        public InfTexture(string filename, string path)
        {
            _filename = filename;
            _location = path;
        }

        /// <summary>
        /// Loads the texture using our game content loader
        /// </summary>
        public void LoadContent()
        {
            if (!String.IsNullOrEmpty(_location))
            {
                _base = GameManager.Contents.Load<Texture2D>(_location);
                _renderReady = true;
            }
        }

        /// <summary>
        /// Unloads the texture
        /// </summary>
        public void UnloadContent()
        {
            if (_base != null && !_base.IsDisposed)
                _base.Dispose();
        }
    }
}
