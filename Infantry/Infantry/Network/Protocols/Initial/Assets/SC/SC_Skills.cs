using System;
using System.Collections.Generic;

using Lidgren.Network;
using Lidgren.Network.Xna;

using Microsoft.Xna.Framework;

using Infantry.Assets;
using Infantry.Managers;

namespace Infantry.Network
{
    public class SC_Skills
    {
        #region Declarations
        private NetIncomingMessage msg;

        private int count;
        #endregion

        public void Receive(NetIncomingMessage msg)
        {
            this.msg = msg;

            Process();
            Execute();
        }

        internal void Process()
        {
            AssetInfo.skills.Clear();

            count = msg.ReadInt32();
            Console.WriteLine("Receiving " + count + " Skill assets");
            //Get the packet data
            for (int i = 0; i < count; i++)
            {
                SkillInfo skill = new SkillInfo(i);

                skill.Buyable = msg.ReadBoolean();
                skill.BuyPrice = msg.ReadInt32();
                skill.Category = msg.ReadString();
                skill.Description = msg.ReadString();
                skill.DevOnly = msg.ReadBoolean();
                skill.Name = msg.ReadString();
                skill.Sellable = msg.ReadBoolean();
                skill.SellPrice = msg.ReadInt32();
                skill.SkillID = msg.ReadInt32();
                skill.UseExperience = msg.ReadBoolean();
                skill.VehicleID = msg.ReadInt32();
                skill.Visible = msg.ReadBoolean();

                AssetInfo.skills.Add(skill.SkillID, skill);
            }

            count = msg.ReadInt32();
            Console.WriteLine("Receiving " + count + " Attribute assets");
            //Get the packet data
            for (int i = 0; i < count; i++)
            {
                AttributeInfo attribute = new AttributeInfo(i);

                attribute.AttributeID = msg.ReadInt32();
                attribute.BAcceleration = msg.ReadInt32();
                attribute.BaseEnergy = msg.ReadInt32();
                attribute.Buyable = msg.ReadBoolean();
                attribute.BuyPrice = msg.ReadInt32();
                attribute.Category = msg.ReadString();
                attribute.Description = msg.ReadString();
                attribute.DevOnly = msg.ReadBoolean();
                attribute.EnergyRate = msg.ReadInt32();
                attribute.FAcceleration = msg.ReadInt32();
                attribute.HitPoints = msg.ReadInt32();
                attribute.MaxAcceleration = msg.ReadInt32();
                attribute.MaxVelocity = msg.ReadInt32();
                attribute.Multiplier = msg.ReadInt32();
                attribute.Name = msg.ReadString();
                attribute.NormalWeight = msg.ReadInt32();
                attribute.SAcceleration = msg.ReadInt32();
                attribute.Sellable = msg.ReadBoolean();
                attribute.SellPrice = msg.ReadInt32();
                attribute.StopWeight = msg.ReadInt32();
                attribute.UseExperience = msg.ReadBoolean();
                attribute.Visible = msg.ReadBoolean();
            }
        }

        internal void Execute()
        {
            //Act on the data received
            //Last packet received for our game state loading, we are in game
            GameManager.GameState = State.InGame;
        }
    }
}
