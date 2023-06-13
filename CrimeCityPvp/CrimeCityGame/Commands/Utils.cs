using BrokeProtocol.API;
using BrokeProtocol.Entities;
using System;

namespace CrimeCityGame.Commands
{
    internal class Utils
    {
        public Utils()
        {
            CommandHandler.RegisterCommand("discord", new Action<ShPlayer>(SendDiscordLink));
        }

        public void SendDiscordLink(ShPlayer player)
        {
            player.svPlayer.SvOpenURL("https://discord.gg/CjZMMYTfyr", "discord link");
        }
    }
}
