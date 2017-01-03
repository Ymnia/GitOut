using FluentScheduler;
using GitOut.Utility;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace GitOut.Services
{
  public class LeagueOfLegends : Registry
  {
    private const string MatchUrl = "https://euw.api.pvp.net/api/lol/euw/v1.3/game/by-summoner/{0}/recent?api_key={1}";
    private const string ChampionUrl = "https://global.api.pvp.net/api/lol/static-data/euw/v1.2/champion/{0}?api_key={1}";
    private const string ApiKey = "e621c9f2-ca69-4f30-80b2-7aed2987e7b6";

    public LeagueOfLegends()
    {
      Schedule(Process).ToRunNow().AndEvery(15).Minutes();
    }

    public void Process()
    {
      try
      {
        var games = new Dictionary<long, GameStats>();

        foreach (var s in Summoners)
        {
          // do API call and get response
          var json = GetJSON(string.Format(MatchUrl, s.SummonerID, ApiKey));
          if (json == null) continue;

          // get and compare game ID
          var game = json["games"][0];
          var gameId = long.Parse((string) game["gameId"]);
          if (!s.LastMatchID.HasValue) s.LastMatchID = gameId;
          if (s.LastMatchID == gameId) continue;

          // save game data
          if (!games.ContainsKey(gameId)) games[gameId] = new GameStats(game);
          games[gameId].Players[s.Name] = new PlayerStats(s.Name, game);

          // update last game ID
          s.LastMatchID = gameId;
        }

        // report
        foreach (var game in games)
        {
          // report quadrakills
          foreach (var p in game.Value.Players.Where(p => p.Value.QuadraKills > 0 && p.Value.PentaKills == 0))
          {
            Slack.SendMessage("#lol", string.Format("{0} had {1} QUADRAKILL{2}!!",
              p.Value.ToString(),
              p.Value.QuadraKills == 1 ? "A" : p.Value.QuadraKills.ToString(CultureInfo.InvariantCulture),
              p.Value.QuadraKills > 1 ? "S" : ""
              ), ":quadra:", "LOL");
          }

          // report pentakills
          foreach (var p in game.Value.Players.Where(p => p.Value.PentaKills > 0))
          {
            Slack.SendMessage("#lol", string.Format("{0} had {1} PENTAKILL{2}!! @channel",
              p.Value.ToString(),
              p.Value.PentaKills == 1 ? "A" : p.Value.PentaKills.ToString(CultureInfo.InvariantCulture),
              p.Value.PentaKills > 1 ? "S" : ""
              ), ":penta:", "LOL");
          }

          // report game stats
          Slack.SendMessage("#lol", game.Value.ToString(), ":lol:", "LOL");
        }
      }
      catch
      {
        // ignored
      }
    }

    public class Summoner
    {
      public int SummonerID;
      public string Name;
      public long? LastMatchID;
    }

    public static List<Summoner> Summoners = new List<Summoner>
    {
      new Summoner {Name = "@coen", SummonerID = 20281265},
      new Summoner {Name = "@skinny", SummonerID = 44507786},
      new Summoner {Name = "@dennis", SummonerID = 23481282},
      new Summoner {Name = "@p.griep", SummonerID = 75118295},
      new Summoner {Name = "@c.christiaan", SummonerID = 30103401},
      new Summoner {Name = "@lukashuzen", SummonerID = 23447158}
    };

    #region JSON methods

    public static JObject GetJSON(string url)
    {
      try
      {
        using (var client = new WebClient())
        {
          using (var stream = client.OpenRead(url))
          {
            if (stream == null) return null;
            using (var reader = new StreamReader(stream))
            {
              return JObject.Parse(reader.ReadLine());
            }
          }
        }
      }
      catch
      {
        return null;
      }
    }

    public static int GetValue(JToken token)
    {
      return token == null ? 0 : int.Parse((string) token);
    }

    #endregion

    #region LOL objects

    private class GameStats
    {
      private readonly string _gameMode;
      private readonly string _subType;
      private readonly string _map;
      public readonly Dictionary<string, PlayerStats> Players;

      public GameStats(JToken game)
      {
        _gameMode = GetGameModeName((string) game["gameMode"]);
        _subType = GetSubTypeName((string) game["subType"]);
        _map = GetMapName(int.Parse((string) game["mapId"]));

        Players = new Dictionary<string, PlayerStats>();
      }

      public new string ToString()
      {
        var sb = new StringBuilder();

        var winners = Concat((from p in Players where p.Value.Win orderby p.Value.Kills descending select p.Value.ToString()).ToArray());
        var losers = Concat((from p in Players where !p.Value.Win orderby p.Value.Kills descending select p.Value.ToString()).ToArray());

        if (!string.IsNullOrEmpty(winners)) sb.Append(winners + " just WON");
        if (!string.IsNullOrEmpty(winners) && !string.IsNullOrEmpty(losers)) sb.Append(" against " + losers);
        else if (!string.IsNullOrEmpty(losers)) sb.Append(losers + " just LOST");

        sb.Append($" in {_subType}{_gameMode} on {_map}.");

        return sb.ToString();
      }

      #region Helper methods

      private static readonly Dictionary<int, string> Maps = new Dictionary<int, string>
      {
        {1, "Summoner's Rift"},
        {2, "Summoner's Rift"},
        {3, "The Proving Grounds"},
        {4, "Twisted Treeline"},
        {8, "The Crystal Scar"},
        {10, "Twisted Treeline"},
        {11, "Summoner's Rift (Beta)"},
        {12, "Howling Abyss"}
      };

      private static string GetMapName(int mapId)
      {
        return !Maps.ContainsKey(mapId) ? "Unknown" : Maps[mapId];
      }

      private static readonly Dictionary<string, string> GameModes = new Dictionary<string, string>
      {
        {"CLASSIC", "Classic"},
        {"ODIN", "Dominion"},
        {"ARAM", "ARAM"},
        {"TUTORIAL", "Tutorial"},
        {"ONEFORALL", "One for All"},
        {"ASCENSION", "Ascension"},
        {"FIRSTBLOOD", "Snowdown Showdown"},
        {"KINGPORO", "Poro King"}
      };

      private static string GetGameModeName(string gameMode)
      {
        var s = !GameModes.ContainsKey(gameMode) ? "Unknown" : GameModes[gameMode];
        return s == "ARAM" ? "" : " " + s;
      }

      private static readonly Dictionary<string, string> SubTypes = new Dictionary<string, string>
      {
        {"NONE", "Custom game"},
        {"NORMAL", "Summoner's Rift unranked"},
        {"NORMAL_3x3", "Twisted Treeline unranked"},
        {"ODIN_UNRANKED", "Dominion"},
        {"ARAM_UNRANKED_5x5", "ARAM"},
        {"BOT", "Bot 5x5 game"},
        {"BOT_3x3", "Bot 3x3 game"},
        {"RANKED_SOLO_5x5", "Summoner's Rift ranked solo"},
        {"RANKED_TEAM_3x3", "Twisted Treeline ranked team"},
        {"RANKED_TEAM_5x5", "Summoner's Rift ranked team"},
        {"ONEFORALL_5x5", "One for All"},
        {"FIRSTBLOOD_1x1", "Snowdown Showdown 1x1"},
        {"FIRSTBLOOD_2x2", "Snowdown Showdown 2x2"},
        {"SR_6x6", "Summoner's Rift 6x6 Hexakill"},
        {"CAP_5x5", "Team Builder"},
        {"URF", "Ultra Rapid Fire"},
        {"URF_BOT", "Ultra Rapid Fire bots"},
        {"NIGHTMARE_BOT", "Nightmare bots"},
        {"ASCENSION", "Ascension"},
        {"KING_PORO", "Legend of the"},
        {"HEXAKILL", "Twisted Treeline 6x6 Hexakill"}
      };

      private static string GetSubTypeName(string subType)
      {
        return !SubTypes.ContainsKey(subType) ? "Unknown" : SubTypes[subType];
      }

      private static string Concat(IList<string> s)
      {
        return s == null || !s.Any() ? string.Empty : s.Count == 1 ? s[0] : string.Format("{0} and {1}", string.Join(", ", s.Take(s.Count - 1)), s[s.Count - 1]);
      }

      #endregion
    }

    private class PlayerStats
    {
      private readonly string _name;
      private readonly string _champion;
      private readonly int _deaths;
      private readonly int _assists;

      public readonly bool Win;
      public int Kills { get; }
      public int QuadraKills { get; }
      public int PentaKills { get; }

      public PlayerStats(string name, JToken game)
      {
        _name = name;
        _champion = GetChampionName(int.Parse((string) game["championId"]));
        _deaths = GetValue(game["stats"]["numDeaths"]);
        _assists = GetValue(game["stats"]["assists"]);

        Win = (bool) game["stats"]["win"];
        Kills = GetValue(game["stats"]["championsKilled"]);
        QuadraKills = GetValue(game["stats"]["quadraKills"]);
        PentaKills = GetValue(game["stats"]["pentaKills"]);
      }

      public new string ToString()
      {
        return $"{_name} ({_champion} {Kills}/{_deaths}/{_assists})";
      }

      #region Helper methods

      private static readonly Dictionary<int, string> Champions = new Dictionary<int, string>();

      private static string GetChampionName(int championId)
      {
        if (Champions.ContainsKey(championId)) return !Champions.ContainsKey(championId) ? String.Format("Unknown ({0})", championId) : Champions[championId];
        var json = GetJSON(string.Format(ChampionUrl, championId, ApiKey));

        if (json != null)
        {
          Champions.Add(championId, (string) json["name"]);
        }

        return !Champions.ContainsKey(championId) ? $"Unknown ({championId})" : Champions[championId];
      }

      #endregion
    }

    #endregion
  }
}