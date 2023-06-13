using BrokeProtocol.API;
using BrokeProtocol.Entities;
using BrokeProtocol.Managers;
using BrokeProtocol.Required;
using BrokeProtocol.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CrimeCityGame.Events
{
    public class PvpFfa
    {
        Main main = Main.Instance;
        public IEnumerator Event(Game Game)
        {
            while (main.Games.Contains(Game))
            {
                for (int i = 0; i < 3; i++)
                {
                    yield return new WaitForSeconds(200f);
                    foreach (var plr in Game.plrs) plr.svPlayer.SendText("&4- [Ragedose drop] -!", 3f, new Vector2(0.5f, 0.7f));
                    if (Game.plrs.Count > 0) Game.plrs.GetRandom().TransferItem(DeltaInv.AddToMe, "RageDose".GetPrefabIndex());
                }

                if (Game.plrs.Count > 0)
                {
                    foreach (var plr in Game.plrs)
                    {
                        var pos = main.LobbyMrks.mainT;
                        plr.svPlayer.SvRestore(pos.position, pos.rotation, 0);
                        plr.svPlayer.SendText("&4- [Game Ended] -!", 3f, new Vector2(0.5f, 0.7f));
                        plr.svPlayer.SendGameMessage("&2Time to Vote next map !");
                    }
                    yield return new WaitForSeconds(25f);
                    foreach()
                    Game.Votes.TryGetValue(, out string mapname);
                    Map map = main.Maps.Find(x => x.MapName == mapname);
                    Game.GameID = map.MapID;
                    foreach (var plr in Game.plrs)
                    {
                        var sp = map.Sps.GetRandom();
                        plr.svPlayer.SendText("&2- [Game Start] -!", 3f, new Vector2(0.5f, 0.7f));
                        plr.svPlayer.SvRestore(sp.mainT.position, sp.mainT.rotation, map.MapID);
                    }
                    Game.Votes.Clear();
                }
                else
                {
                    main.Games.Remove(Game);
                }
            }
        }
    }

    public class PlayerFFA : PlayerEvents
    {
        Main main = Main.Instance;
        public void SetupKits(ShPlayer player)
        {
            player.svPlayer.SetMaxSpeed(17f);
            var melee = main.ShMelee.GetRandom();
            var gun = main.ShGun.GetRandom();
            player.TransferItem(DeltaInv.AddToMe, gun.AmmoItem.index, 500);
            player.TransferItem(DeltaInv.AddToMe, melee.index, 1);
            player.TransferItem(DeltaInv.AddToMe, gun.index, 1);
            player.svPlayer.SvForceEquipable(gun.index);
            player.StartCoroutine(SendPvpPanel(player));
        }

        [CustomTarget]
        public void NukeTown(Serialized trigger, ShPhysical physical)
        {
            if (physical is ShPlayer player)
            {
                var game = main.Games.Find(x => x.plrs.Exists(plr => plr == player));
                if (game.Votes.ContainsKey(player)) game.Votes.Remove(player);
                game.Votes.Add(player, "NukeTown");
            }
        }

        [CustomTarget]
        public void SciFiCity(Serialized trigger, ShPhysical physical)
        {
            if (physical is ShPlayer player)
            {
                var game = main.Games.Find(x => x.plrs.Exists(plr => plr == player));
                if (game.Votes.ContainsKey(player)) game.Votes.Remove(player);
                game.Votes.Add(player, "SciFiCity");
            }
        }

        [Execution(ExecutionMode.Additive)]
        public override bool Respawn(ShEntity entity)
        {
            if (entity is ShPlayer player && player.svPlayer.CustomData.TryFetchCustomData("Game", out string game) && game == "PVPffa")
            {
                //tp spawn
                var spawn = main.Maps.Find(x => x.MapID == player.GetPlace.GetIndex).Sps.GetRandom();
                player.svPlayer.SvRestore(spawn.mainT.position, spawn.mainT.rotation, 0);

                SetupKits(player);
            }
            return true;
        }

        [Execution(ExecutionMode.Additive)]
        public override bool OptionAction(ShPlayer player, int targetID, string id, string optionID, string actionID)
        {
            player.svPlayer.DestroyMenu(id);
            if (id == "gml" && optionID == "PVPffa")
            {
                var gid = main.FindOrCreateAGamePublic(optionID);
                if (gid == 0)
                {
                    player.svPlayer.SendGameMessage("&4no game free");
                    return false;
                }
                var game = main.Games.Find(x => x.GameID == gid);
                SetupKits(player);
                game.plrs.Add(player);
                var SP = main.Maps.Find(x => x.MapID == game.GameID).Sps.GetRandom();
                player.svPlayer.SvRestore(SP.mainT.position, SP.mainT.rotation, game.GameID);
                player.svPlayer.CustomData.AddOrUpdate("Game", "PVPffa");
            }
            return true;
        }

        [Execution(ExecutionMode.Additive)]
        public override bool Death(ShDestroyable destroyable, ShPlayer attacker)
        {
            var player = destroyable.Player;
            if (player && attacker)
            {
                if (player.svPlayer.CustomData.TryFetchCustomData("Game", out string game) && game == "PVPffa")
                {
                    if (!attacker.Player.svPlayer.CustomData.Data.TryGetValue("kill", out var kill)) attacker.Player.svPlayer.CustomData.AddOrUpdate("kill", 0);
                    attacker.svPlayer.CustomData.AddOrUpdate("kill", Convert.ToDecimal(kill) + 1);

                    if (!player.Player.svPlayer.CustomData.Data.TryGetValue("death", out var death)) player.Player.svPlayer.CustomData.AddOrUpdate("death", 0);
                    player.Player.svPlayer.CustomData.AddOrUpdate("death", Convert.ToDecimal(death) + 1);
                }
            }
            return true;
        }

        public IEnumerator SendPvpPanel(ShPlayer p)
        {
            p.svPlayer.DestroyTextPanel("mainpanel");
            yield return new WaitForSeconds(0.5f);
            while (p.svPlayer.CustomData.TryFetchCustomData("Game", out string game) && game == "PVPffa")
            {
                p.svPlayer.CustomData.Data.TryGetValue("kill", out var kill);
                p.svPlayer.CustomData.Data.TryGetValue("death", out var death);
                p.svPlayer.SendTextPanel($"{main.funmessage.GetRandom()} \r\n\r\n&f[ &dPlayerName &f] : {p.username}\r\n&f[ &6Connected Players &f] : {SvManager.Instance.connectedPlayers.Count}\r\n&f[ &eKills &f] : {kill} \r\n&f[ &4Death &f] : {death}", "mainpanel");

                yield return new WaitForSeconds(1f);

                p.svPlayer.DestroyTextPanel("mainpanel");
            }
        }
    }
}
