using BrokeProtocol.API;
using BrokeProtocol.Entities;
using BrokeProtocol.Required;

namespace CrimeCityGame.AnimObject
{
    internal class GameModeMenu : PlayerEvents
    {
        [CustomTarget]
        public void Play(ShEntity target, ShPlayer caller)
        {
            var options = new LabelID[]
            {
                new LabelID("&4PVP &2FFA","PVPffa"),
                new LabelID("&6Parcours [Soon]","-parcours"),
                new LabelID("&2Football [Soon]","-Football"),
                new LabelID("&9BattleRoyal [Soon]","-BattleRoyal")
            };
            caller.svPlayer.SendOptionMenu("&f- &2GameList &f-", caller.ID, "gml", options, new LabelID[] { new LabelID("click", "click") }, 0.25f, 0.3f, 0.75f, 0.8f);
        }
    }
}
