using Microsoft.Xna.Framework;

namespace Infantry.Screens
{
    /// <summary>
    /// Interface used to display notification messages such as "Player Entering Arena".
    /// This is registered as a service so any code wanting to display a message can 
    /// use it to look it up in Game.Services.
    /// </summary>
    interface IMessageDisplay : IDrawable, IUpdateable
    {
        void ShowMessage(string message, params object[] parameters);
    }
}
