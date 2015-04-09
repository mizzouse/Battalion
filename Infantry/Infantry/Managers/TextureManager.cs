using System;
using System.Collections.Generic;
using System.Threading;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Infantry.Objects;

namespace Infantry.Managers
{
    public sealed class TextureManager : GameComponent
    {
        private static Dictionary<string, InfTexture> _textures = new Dictionary<string, InfTexture>();
        private static bool _initialized = false;
        private static int _texturesLoaded = 0;

        /// <summary>
        /// Is the Texture Manager initialized? Used for startup
        /// </summary>
        public static bool Initialized
        {
            get { return _initialized; }
        }

        /// <summary>
        /// The number of textures currently loaded.
        /// Also can be used for loading bar x out of max
        /// </summary>
        public static int TexturesLoaded
        {
            get { return _texturesLoaded; }
        }

        /// <summary>
        /// Is our texture ready to render?
        /// </summary>
        /// <param name="name">Our texture name</param>
        /// <returns>Returns true if it is, false if not</returns>
        public static bool RenderReady(string name)
        {
            InfTexture obj = GetTextureObj(name);
            return obj != null ? obj.RenderReady : false;
        }

        /// <summary>
        /// Texture Manager Constructor
        /// </summary>
        /// <param name="game"></param>
        public TextureManager(Game game)
            : base(game)
        {
        }

        /// <summary>
        /// Creates an InfTexture object then adds it to our texture manager list
        /// </summary>
        /// <param name="name">Name of the texture</param>
        /// <param name="path">The file path</param>
        public static void AddTexture(string name, string path)
        {
            InfTexture texture = null;
            if (!String.IsNullOrEmpty(name) && !_textures.ContainsKey(name))
            {
                //Are we using a set path?
                if (!String.IsNullOrEmpty(path))
                    texture = new InfTexture(name, path);
                else
                    //We arent, use root content directory
                    texture = new InfTexture(name, GameManager.Contents.RootDirectory);
                AddTexture(name, texture);
            }
        }

        /// <summary>
        /// Adds a texture to our texture manager list
        /// </summary>
        /// <param name="name">Name of the texture</param>
        /// <param name="texture">The actual texture</param>
        public static void AddTexture(string name, InfTexture texture)
        {
            if (!String.IsNullOrWhiteSpace(name)
                && !_textures.ContainsKey(name))
            {
                _textures.Add(name, texture);
                if (_initialized)
                {
                    texture.LoadContent();
                    _texturesLoaded++;
                }
            }
        }

        /// <summary>
        /// Removes a texture from our manager list
        /// </summary>
        /// <param name="name">The texture name</param>
        public static void RemoveTexture(string name)
        {
            InfTexture texture = null;
            if (!String.IsNullOrEmpty(name))
                if (_textures.TryGetValue(name, out texture))
                {
                    texture.UnloadContent();
                    _textures.Remove(name);
                    _texturesLoaded--;
                }
        }

        /// <summary>
        /// Removes a texture from our manager list
        /// </summary>
        /// <param name="texture">Our Texture Object</param>
        public static void RemoveTexture(InfTexture texture)
        {
            if (_textures.ContainsValue(texture))
            {
                foreach (InfTexture text in _textures.Values)
                    if (text == texture)
                    {
                        string filename = text.FileName;
                        texture.UnloadContent();
                        _textures.Remove(filename);
                        _texturesLoaded--;
                    }
            }
        }

        /// <summary>
        /// Gets a texture object from our manager list
        /// </summary>
        /// <param name="name">Name of texture</param>
        /// <returns>Returns the Texture Object, null if not found</returns>
        public static InfTexture GetTextureObj(string name)
        {
            InfTexture texture = null;
            if (!String.IsNullOrEmpty(name)
                && _textures.TryGetValue(name, out texture))
                return texture;

            return null;
        }

        /// <summary>
        /// Gets a texture object from our list then gets the base texture
        /// </summary>
        /// <param name="name">Name of texture</param>
        /// <returns>Returns the base texture in our inf object, null if not found</returns>
        public static Texture2D GetTexture(string name)
        {
            InfTexture texture = null;
            if (!String.IsNullOrEmpty(name)
                && _textures.TryGetValue(name, out texture))
            {
                if (texture.BaseTexture != null)
                    return texture.BaseTexture;
            }

            return null;
        }

        /// <summary>
        /// Initializes the Manager and each texture in its list
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();

            foreach(InfTexture texture in _textures.Values)
                if (!texture.RenderReady)
                {
                    texture.LoadContent();
                    _texturesLoaded++;
                }

            _initialized = true;
        }
    }
}
