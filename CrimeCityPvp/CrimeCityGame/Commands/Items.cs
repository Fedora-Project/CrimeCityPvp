using BrokeProtocol.API;
using BrokeProtocol.Entities;
using BrokeProtocol.Required;
using BrokeProtocol.Utility;
using System;
using System.Collections.Generic;

namespace CrimeCityGame.Commands
{
    internal class Items : PlayerEvents, IScript
    {
        public Items()
        {
            CommandHandler.RegisterCommand("Weapons", new Action<ShPlayer>(ListWeapon));
        }

        public void ListWeapon(ShPlayer p)
        {
            p.svPlayer.SendOptionMenu("List Weapons", p.ID, "ChooseTypeItems", new LabelID[] { new LabelID("MeleeList", "MeleeList"), new LabelID("GunList", "GunList") }, new LabelID[] { new LabelID("click", "click") });
        }

        [Execution(ExecutionMode.Additive)]
        public override bool OptionAction(ShPlayer player, int targetID, string id, string optionID, string actionID)
        {
            switch (id)
            {
                case "ChooseTypeItems":
                    var options = new List<LabelID>();
                    if (optionID == "GunList")
                    {
                        foreach (ShGun gun in Main.Instance.ShGun)
                        {
                            options.Add(new LabelID(gun.name, gun.name));
                        }
                        player.svPlayer.SendOptionMenu("List Weapons", player.ID, "GunList", options.ToArray(), new LabelID[] { new LabelID("click", "click") });
                    }
                    else
                    {
                        foreach (ShWeapon melee in Main.Instance.ShMelee)
                        {
                            options.Add(new LabelID(melee.name, melee.name));
                        }
                        player.svPlayer.SendOptionMenu("List Weapons", player.ID, "MeleeList", options.ToArray(), new LabelID[] { new LabelID("click", "click") });
                    }
                    break;
                case "MeleeList":
                    var Wmelee = Main.Instance.ShMelee.Find(x => x.name == optionID);
                    player.TransferItem(DeltaInv.AddToMe, Wmelee);
                    player.svPlayer.SvForceEquipable(Wmelee.index);
                    break;
                case "GunList":
                    var weapon = Main.Instance.ShGun.Find(x => x.name == optionID);
                    player.TransferItem(DeltaInv.AddToMe, weapon);
                    player.svPlayer.SvForceEquipable(weapon.index);
                    break;
            }
            return true;
        }
    }
}
