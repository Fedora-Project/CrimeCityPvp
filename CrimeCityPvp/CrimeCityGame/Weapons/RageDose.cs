using BrokeProtocol.API;
using BrokeProtocol.Entities;
using BrokeProtocol.Managers;
using BrokeProtocol.Required;
using System.Collections;
using UnityEngine;

namespace CrimeCityGame.Events
{
    internal class RageDose : PlayerEvents
    {
        CrimeCityAdministrator.Main MainA = CrimeCityAdministrator.Main.Instance;
        Color CurSkyColor = new Color(0, 0, 0);
        ShPlayer ragedose = null;

        [Execution(ExecutionMode.Event)]
        public override bool Consume(ShPlayer player, ShConsumable consumable, ShPlayer healer)
        {
            if (consumable.name == "RageDose")
            {
                if (ragedose)
                {
                    player.svPlayer.SendGameMessage("&4cooldown say nop to rage you");
                    return false;
                }
                InterfaceHandler.SendTextToAll("&4Rage dose !!!", 3, new Vector2(0.5f, 0.8f));
                ragedose = player;
                ragedose.svPlayer.SvHeal(ragedose);
                SvManager.Instance.StartCoroutine(MainA.SoundCoroutine(player, "EternalRageSound", 120));
                SvManager.Instance.StartCoroutine(StartRageDose());
            }
            return true;
        }

        [Execution(ExecutionMode.Additive)]
        public override bool Damage(ShDamageable damageable, DamageIndex damageIndex, float amount, ShPlayer attacker, Collider collider, Vector3 source, Vector3 hitPoint)
        {
            if (damageable.IsDead) return false;

            if (damageable is ShDestroyable e && ragedose == attacker && damageIndex == DamageIndex.Melee)
            {
                amount += 35;
                e.health -= amount;
                if (e.health <= 0f)
                {
                    e.Die(attacker);
                }
                e.svDestroyable.UpdateHealth(source, hitPoint);
            }
            else if (damageable is ShDestroyable r && r.Player == ragedose)
            {
                if (attacker)
                {
                    amount -= 5;
                    r.health -= amount;
                    if (r.health <= 0f)
                    {
                        r.Die(attacker);
                    }
                    r.svDestroyable.UpdateHealth(source, hitPoint);
                }
            }
            return true;
        }

        public IEnumerator StartRageDose()
        {
            ragedose.svPlayer.SetMaxSpeed(22);
            while (CurSkyColor.r < 3)
            {
                CurSkyColor.r += 0.01f;

                SvManager.Instance.SvSetSkyColor(CurSkyColor);

                yield return new WaitForSeconds(0.01f);
            }
            int time = 0;
            while (true)
            {
                if (ragedose.IsDead || time == 100)
                {
                    SvManager.Instance.StartCoroutine(EndRageDose());
                    yield break;
                }
                yield return new WaitForSeconds(1f);
                time++;
            }
        }

        public IEnumerator EndRageDose()
        {
            while (CurSkyColor.r > 0)
            {
                CurSkyColor.r -= 0.01f;

                SvManager.Instance.SvSetSkyColor(CurSkyColor);

                yield return new WaitForSeconds(0.01f);
            }
            ragedose.svPlayer.SetMaxSpeed(12);
            CrimeCityAdministrator.Main.Instance.Sounds.Remove(CrimeCityAdministrator.Main.Instance.Sounds.Find(x => x.name == "EternalRageSound"));
            yield return new WaitForSeconds(60f);
            ragedose = null;
            yield break;
        }
    }
}
