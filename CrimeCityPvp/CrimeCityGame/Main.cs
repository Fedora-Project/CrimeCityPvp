using BrokeProtocol.API;
using BrokeProtocol.Entities;
using BrokeProtocol.GameSource;
using BrokeProtocol.Managers;
using BrokeProtocol.Utility;
using CrimeCityCore;
using CrimeCityCore.Utils;
using CrimeCityGame.Events;
using CrimeCityGame.GameManager;
using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace CrimeCityGame
{
    public class Game
    {
        public string GameType;
        public int GameID;
        public List<ShPlayer> plrs = new List<ShPlayer>();
        public int slots;
        public Dictionary<ShPlayer, string> Votes = new Dictionary<ShPlayer, string>();
    }

    public class Map
    {
        public int MapID;
        public string MapName;
        public string MapType;
        public bool MapTaked;
        public List<SpawnLocation> Sps = new List<SpawnLocation>();
    }

    internal class Main : Plugin
    {
        public static Main Instance { get; private set; }

        //weapons types
        public List<ShGun> ShGun = new List<ShGun>();
        public List<ShWeapon> ShMelee = new List<ShWeapon>();

        public List<Map> Maps = new List<Map>();
        public List<Game> Games = new List<Game>();

        public SpawnLocation LobbySpawn { get; set; }
        public MapMarker LobbyMrks { get; set; } 

        public string[] funmessage =
        {
            "&f- &cWelcome On CrimeCity &f-",
            "&f- &cServer Is In Alpha &f-",
            "&f- &cGet a ragedose &f-",
            "&f- &chttps://discord.gg/CjZMMYTfyr &f-"
        };

        public Main()
        {
            var harmony = new Harmony("fr.CrimeCityCore");
            Info = new PluginInfo("CrimeCityPvp", "cg", "plugin by Mr Babouche#2365", "https://github.com/YA80/");
            if (Instance != null)
            {
                Debug.Log("[CrimeCityGame] Instance Core Bug");
                return;
            }
            Instance = this;
            var assembly = Assembly.GetExecutingAssembly();
            harmony.PatchAll(assembly);
        }

        public int FindOrCreateAGamePublic(string GameType, int slots = 16)
        {
            if (Games.Exists(x => x.GameType == GameType && x.plrs.Count < x.slots))
            {
                return Games.First(x => x.GameType == GameType && x.plrs.Count < x.slots).GameID;
            }
            else
            {
                Game game = new Game()
                {
                    GameID = GetFreeMap(GameType),
                    GameType = GameType,
                    slots = slots
                };
                if (game.GameID == 0) return 0;
                StartAGame(game);
                return game.GameID;
            }
        }

        public void StartAGame(Game game)
        {
            Games.Add(game);
            switch (game.GameType)
            {
                case "PVPffa":
                    PvpFfa pvpFfa = new PvpFfa();
                    SvManager.Instance.StartCoroutine(pvpFfa.Event(game));
                    break;
                case "BattleRoyal":
                    BattleRoyal Battleroyal = new BattleRoyal();
                    break;
                case "Football":
                    Football Football = new Football();
                    break;
                case "Parcours":
                    Parcours Parcours = new Parcours();
                    break;
            }
        }

        public int GetFreeMap(string GameType)
        {
            if (Maps.Exists(x => x.MapTaked == false && x.MapType == GameType))
            {
                var mps = Maps.FindAll(x => x.MapTaked == false && x.MapType == GameType).ToArray();
                return mps.GetRandom().MapID;
            }
            return 0;
        }

        //place Function
        public List<SpawnLocation> GetSpawnLocationsFromInterior(Place p)
        {
            List<SpawnLocation> spawnLocations = new List<SpawnLocation>();
            foreach (SpawnLocation spawn in BrokeProtocol.GameSource.Types.Manager.spawnLocations)
            {
                if (spawn.mainT.parent.GetSiblingIndex() == p.mTransform.GetSiblingIndex()) spawnLocations.Add(spawn);
            }
            return spawnLocations;
        }

        public List<MapMarker> GetMarkersFromInterior(Place p)
        {
            List<MapMarker> Markers = new List<MapMarker>();
            foreach (MapMarker marker in EntityHandler.Instance.markers)
            {
                if (marker.mainT.parent.GetSiblingIndex() == p.mTransform.GetSiblingIndex()) Markers.Add(marker);
            }
            return Markers;
        }
    }
}
