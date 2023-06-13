using BrokeProtocol.API;
using BrokeProtocol.Entities;
using BrokeProtocol.Utility;
using System.Collections;
using UnityEngine;

namespace CrimeCityGame.Environment
{
    internal class AnimObject : IScript
    {
        [CustomTarget]
        public void Elevator(ShEntity target, ShPlayer caller)
        {
            target.svEntity.SvAnimatorBool("MoveElevator", !target.animator.GetBool("MoveElevator"));
        }

        [CustomTarget]
        public void Door(ShEntity target, ShPlayer caller)
        {
            target.svEntity.SvAnimatorBool("DoorState", !target.animator.GetBool("DoorState"));
        }

        [CustomTarget]
        public void SafeState(ShEntity target, ShPlayer caller)
        {
            if (caller.HasItem("Lockpick".GetPrefabIndex()))
            {
                caller.svPlayer.SvProgressBar(7f, 0.1f, "lpd");
                caller.svPlayer.StartCoroutine(Lockpick(target, caller));
            }
            else caller.svPlayer.SendText("&4need lockpick", 2f, new Vector2(0.5f, 0.3f));
        }
        private IEnumerator Lockpick(ShEntity target, ShPlayer caller)
        {
            yield return new WaitForSeconds(7f);
            target.svEntity.SvAnimatorBool("IsOpen", !target.animator.GetBool("IsOpen"));
            caller.TransferMoney(DeltaInv.AddToMe, Random.Range(117, 556));
            caller.svPlayer.SvProgressStop("lpd");
            yield return new WaitForSeconds(3f);
            target.svEntity.SvAnimatorBool("IsOpen", !target.animator.GetBool("IsOpen"));
        }
    }
}
