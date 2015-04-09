using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Infantry.Managers;
using Infantry.Helpers;

#if !XBOX || !XBOX360
using System.IO;
using System.Windows.Forms;
#endif

namespace Infantry
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
#if !XBOX || !XBOX360
        [STAThread]
#endif
        static void Main(string[] args)
        {
            StartGame();
        }

        internal static void StartGame()
        {
#if !XBOX || !XBOX360
            DirectoryHelper.RootDirectory = Directory.GetCurrentDirectory();
            try
            {
#endif
                using (GameManager game = new GameManager())
                {
                    GameManager.Game = game;
                    SetUpScene();
                    game.Run();
                }
#if !XBOX || !XBOX360
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
#endif
        }

        internal static void SetUpScene()
        {
            ScreenManager.AddScreen(new Screens.LoginScreen());
        }
    }
}

