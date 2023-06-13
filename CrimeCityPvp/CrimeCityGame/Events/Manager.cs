using BrokeProtocol.API;
using BrokeProtocol.Entities;
using BrokeProtocol.Managers;
using CrimeCityCore.Utils;
using System.Linq;
using UnityEngine;

namespace CrimeCityGame.Events
{
    internal class Manager : ManagerEvents
    {
        EntityHandler EntityHandler = EntityHandler.Instance;
        Main main = Main.Instance;
        EntityHandler entities = EntityHandler.Instance;

        [Execution(ExecutionMode.Additive)]
        public override bool Start()
        {
            SvManager.Instance.SvSetSkyColor(Color.cyan);
            SvManager.Instance.SvSetSkyColor(Color.gray);
            SvManager.Instance.SvSetDayFraction(1.25f);
            foreach (ShItem item in EntityHandler.Items)
            {
                if (item is ShWeapon weapon)
                {
                    if (weapon.name != "Hands" || weapon.name != "HandsCat" || weapon.name != "HandsDog")
                    {
                        if (weapon is ShGun w)
                        {
                            main.ShGun.Add(w);
                        }
                        else
                        {
                            main.ShMelee.Add(weapon);
                        }
                    }
                }
            }

            for (int i = 1; i < SceneManager.Instance.places.Count; i++)
            {
                var mrk = main.GetMarkersFromInterior(SceneManager.Instance.places[i]).First();
                Map map = new Map()
                {
                    MapName = mrk.locationName,
                    MapID = i,
                    MapTaked = false,
                    Sps = main.GetSpawnLocationsFromInterior(SceneManager.Instance.places[i])
                };
                if (mrk.name == "country") map.MapType = "PVPffa";
                if (map.Sps.Count > 0) main.Maps.Add(map);
            }
            Debug.Log("[MAPS Registered] : " + main.Maps.Count);

            main.LobbySpawn = BrokeProtocol.GameSource.Types.Manager.spawnLocations.Find(x => x.locationName == "Lobby");
            main.LobbyMrks = entities.markers.Find(x => x.locationName == "Lobby");

            return true;
        }

        [Execution(ExecutionMode.Additive)]
        public override bool Save()
        {
            return true;
        }
    }
}
