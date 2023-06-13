using BrokeProtocol.API;
using BrokeProtocol.Entities;
using System;

namespace CrimeCityGame.Commands
{
    internal class StatsManage : IScript
    {
        public StatsManage()
        {
            CommandHandler.RegisterCommand("Addkill", new Action<ShPlayer, string, int>(AddStats));
        }
        public void AddStats(ShPlayer p, string targetname, int count)
        {
            CrimeCityAdministrator.Main.Instance.TryGetPlayerByName(targetname, out ShPlayer target);
            if (!target.Player.svPlayer.CustomData.Data.TryGetValue("kill", out var kill)) target.Player.svPlayer.CustomData.AddOrUpdate("kill", 0);
            target.svPlayer.CustomData.AddOrUpdate("kill", Convert.ToDecimal(kill) + count);
        }
    }
}
