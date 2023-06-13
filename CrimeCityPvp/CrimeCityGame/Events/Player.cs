using BrokeProtocol.API;
using BrokeProtocol.Entities;
using BrokeProtocol.Managers;
using BrokeProtocol.Utility;
using BrokeProtocol.Utility.Networking;
using ENet;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace CrimeCityGame.Events
{
    internal class Player : PlayerEvents
    {
        Main main = Main.Instance;

        //PlayerCharacter Join Event
        [Execution(ExecutionMode.Additive)]
        public override bool Ready(ShPlayer player)
        {
            player.svPlayer.SetMaxSpeed(12);

            //menu coroutine
            player.StartCoroutine(SendMainPanel(player));

            //tp spawn
            var spawn = main.LobbySpawn;
            player.svPlayer.SvRestore(spawn.mainT.position, spawn.mainT.rotation, 0);

            player.svPlayer.CustomData.AddOrUpdate("Game", "Lobby");

            //clear inventory
            foreach (var item in player.myItems.Values.ToArray())
            {
                if (!(item.item is ShWearable) && item.item.itemName != "Hands")
                {
                    player.TransferItem(DeltaInv.RemoveFromMe, item.item.index, player.MyItemCount(item.item));
                }
            }

            return true;
        }

        // Respawn event
        [Execution(ExecutionMode.Additive)]
        public override bool Respawn(ShEntity entity)
        {
            if (entity is ShPlayer player && player.svPlayer.CustomData.TryFetchCustomData("Game", out string game) && game == "Lobby")
            {
                //tp spawn
                var spawn = main.LobbySpawn;
                player.svPlayer.SvRestore(spawn.mainT.position, spawn.mainT.rotation, 0);

                player.StartCoroutine(SendMainPanel(player));
            }
            return true;
        }

        private IEnumerator SendMainPanel(ShPlayer p)
        {
            yield return new WaitForSeconds(0.5f);
            while (p.svPlayer.CustomData.TryFetchCustomData("Game", out string game) && game == "Lobby")
            {
                p.svPlayer.SendTextPanel($"{main.funmessage.GetRandom()} \r\n\r\n&f[ &dPlayerName &f] : {p.username}\r\n&f[ &6Connected Players &f] : {SvManager.Instance.connectedPlayers.Count}\r\n&f[ &eRegistered Players &f] : {SvManager.Instance.database.Users.Count()}", "mainpanel");

                yield return new WaitForSeconds(1f);

                p.svPlayer.DestroyTextPanel("mainpanel");
            }
        }

        [Execution(ExecutionMode.Override)]
        public override bool ChatVoice(ShPlayer player, byte[] voiceData)
        {
            if (player.svPlayer.callTarget && player.svPlayer.callActive)
            {
                player.svPlayer.callTarget.svPlayer.Send(SvSendType.Self, Channel.Unreliable, ClPacket.ChatVoiceCall, player.ID, voiceData);
            }
            else
            {
                switch (player.chatMode)
                {
                    case ChatMode.Public:
                        player.svPlayer.Send(SvSendType.LocalOthers, Channel.Unreliable, ClPacket.ChatVoice, player.ID, voiceData);
                        break;
                    case ChatMode.Job:
                        foreach (var p in player.svPlayer.job.info.members)
                        {
                            if (p.isHuman && p != player && p.chatMode == ChatMode.Job)
                            {
                                p.svPlayer.Send(SvSendType.Self, Channel.Unreliable, ClPacket.ChatVoiceJob, player.ID, voiceData);
                            }
                        }
                        break;
                }
            }

            return true;
        }

        [Execution(ExecutionMode.Override)]
        public override bool SetChatMode(ShPlayer player, ChatMode chatMode)
        {
            if (chatMode != ChatMode.Channel)
            {
                player.chatMode = chatMode;
                player.svPlayer.Send(SvSendType.Self, PacketFlags.Reliable, ClPacket.SetChatMode, (byte)player.chatMode);
            }
            return true;
        }
    }
}
