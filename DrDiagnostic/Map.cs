using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DrDiagnostic
{
    class MapStatistics
    {
        public MapStatistics()
        { }

        public int PlayedTimes { get; set; }
        public int PlayersConnected { get; set; }
        public int PlayersDisconnected { get; set; }

        public int PopRateSum { get; set; }
        public int PopRateTimes { get; set; }

        public int Score { get; set; }
        public int Kills { get; set; }
        public int Assists { get; set; }
        public int Deaths { get; set; }
    }

    class ItemAvailable
    {
        public ItemAvailable()
        {}

        public bool? IsFree { get; private set; }
        public int? RankId { get; private set; }
        public int? Prestige { get; private set; }
        public int? ChallengeType { get; private set; }
        public string ChallengeName { get; private set; }
        public int? AccessFlag { get; private set; }

        public Dictionary<int, ItemAvailable> AccessInfo { get; private set; }

        public static ItemAvailable Parse(string[] toks)
        {
            ItemAvailable a = new ItemAvailable();
            a.IsFree = String.IsNullOrEmpty(toks[0]) ? (bool?)null : Int32.Parse(toks[0]) == 1;
            a.RankId = String.IsNullOrEmpty(toks[1]) ? (int?)null : Int32.Parse(toks[1]);
            a.Prestige = String.IsNullOrEmpty(toks[2]) ? (int?)null : Int32.Parse(toks[2]);
            a.ChallengeType = String.IsNullOrEmpty(toks[3]) ? (int?)null : Int32.Parse(toks[3]);
            a.ChallengeName = toks[4];
            a.AccessFlag = String.IsNullOrEmpty(toks[5]) ? (int?)null : Int32.Parse(toks[5]);

            if (!String.IsNullOrEmpty(toks[6]))
            {
                a.AccessInfo = new Dictionary<int, ItemAvailable>();
                int startI;
                int length;
                string content = toks[6].GetFoldingContent(out startI, out length);
                string[] contentToks = content.SplitWithFolding();
                foreach (string contentTok in contentToks)
                {
                    if (String.IsNullOrEmpty(contentTok))
                        continue;

                    string accessContent = contentTok.GetFoldingContent(out startI, out length);
                    string[] accessToks = accessContent.SplitWithFolding();
                    int iAccessFlag = Int32.Parse(accessToks[0]);

                    ItemAvailable access = new ItemAvailable();
                    access.IsFree = String.IsNullOrEmpty(accessToks[1]) ? (bool?)null : Int32.Parse(accessToks[1]) == 1;
                    access.RankId = String.IsNullOrEmpty(accessToks[2]) ? (int?)null : Int32.Parse(accessToks[2]);
                    access.Prestige = String.IsNullOrEmpty(accessToks[3]) ? (int?)null : Int32.Parse(accessToks[3]);
                    access.ChallengeType = String.IsNullOrEmpty(accessToks[4]) ? (int?)null : Int32.Parse(accessToks[4]);
                    access.ChallengeName = accessToks[5];             

                    if (!a.AccessInfo.Keys.Contains(iAccessFlag))
                        a.AccessInfo.Add(iAccessFlag, access);

                    a.AccessInfo[iAccessFlag] = access;
                }
            }

            return a;
        }
    }

    public static class StrFolding
    {
        public static string[] SplitWithFolding(this string source)
        {
            int level = 0;
            List<string> toks = new List<string>();
            StringBuilder curTok = new StringBuilder();
            for (int i = 0; i < source.Length; i++)
            {
                if (source[i] == ';' && level == 0)
                {
                    toks.Add(curTok.ToString());
                    curTok = new StringBuilder();
                    continue;
                }

                if (source[i] == '{')
                    level++;
                else if (source[i] == '}')
                    level--;

                curTok.Append(source[i]);
            }
            toks.Add(curTok.ToString());
            return toks.ToArray();
        }

        public static string GetFoldingContent(this string source, out int startI, out int length)
        {
            startI = source.IndexOf('{');
            length = (source.IndexOf('}', startI + 1) + 1) - startI;
            return source.Substring(startI + 1, length - 2); 
        }
    }

    class ChItem
    {
        public string Name { get; private set; }
        public string FullName { get; private set; }
        public string Description { get; private set; }
        public int MaxValue { get; private set; }
        public bool IsCollecting { get; private set; }

        public ItemAvailable Available { get; private set; }

        public ChItem(string name, string fullName, string des, int maxValue, bool isCollecting)
        {
            Name = name;
            FullName = fullName;
            Description = des;
            MaxValue = maxValue;
            IsCollecting = isCollecting;
        }

        public static ChItem Parse(string fromLog)
        {
            string[] toks = fromLog.SplitWithFolding();
            return Parse(toks);
        }

        public static ChItem Parse(string[] toks)
        {
            ChItem chItem = new ChItem(toks[0], toks[1], toks[2], Int32.Parse(toks[3]), (toks[4] == "1" ? true : false));
            chItem.Available = ItemAvailable.Parse(toks.Skip(4).ToArray());
            return chItem;
        }
    }

    class ChStageList
    {
        public string Name { get; private set; }
        public List<ChItem> Items { get; private set; }

        public ChStageList(string name, List<ChItem> items)
        {
            Name = name;
            Items = items;
        }

        public static ChStageList Parse(string fromLog)
        {
            string[] toks = fromLog.SplitWithFolding();
            return Parse(toks);
        }

        public static ChStageList Parse(string[] toks)
        {
            List<ChItem> items = new List<ChItem>();
            for (int i = 1; !String.IsNullOrEmpty(toks[i]); i++)
            {
                int startI;
                int length;
                string itemContent = toks[i].GetFoldingContent(out startI, out length);
                items.Add(ChItem.Parse(itemContent));
            }

            return new ChStageList(toks[0], items);
        }
    }

    class MapChInfo
    {
        public MapChInfo()
        {
            Items = new List<ChItem>();
            Stages = new List<ChStageList>();

            ProceedCount = new Dictionary<string, int>();
        }

        public List<ChItem> Items { get; set; }
        public List<ChStageList> Stages { get; set; }

        public Dictionary<string, int> ProceedCount { get; set; }
    }

    class MapSupportInfo
    {
        public MapSupportInfo()
        {
            TTTouchedCount = new Dictionary<int, int>();

            TJTouchedCount = new Dictionary<int, int>();
        }

        public int SCODJumpersCount { get; set; }
        public int SCODActivatorsCount { get; set; }
        public int SCODSpectatorsCount { get; set; }

        public int SDR1JumpersCount { get; set; }
        public int SDR1ActivatorsCount { get; set; }
        public int SDR1SpectatorsCount { get; set; }

        public int SDR2JumpersCount { get; set; }
        public int SDR2ActivatorsCount { get; set; }
        public int SDR2SpectatorsCount { get; set; }

        public int TTRegisteredCount { get; set; }
        public Dictionary<int, int> TTTouchedCount { get; set; }

        public int TJRegisteredCount { get; set; }
        public Dictionary<int, int> TJTouchedCount { get; set; }

        public bool EMIsRegistered { get; set; }
        public int EMTouchedCount { get; set; }
    }

    class MapBaseInfo
    {
        public MapBaseInfo()
        {
            Mappers = new Dictionary<string, string>();
        }

        public string FullName { get; set; }

        public Dictionary<string, string> Mappers { get; set; }

        public int Difficulty { get; set; }
        public int Length { get; set; }

        public bool IsInMapList { get; set; }
        public bool IsNew { get; set; }
    }

    class Map
    {
        public string MapName { get; private set; }

        //public List<MapLogicEvent> _mapLogicEvents;

        public MapBaseInfo SVBaseInfo { get; private set; }
        public MapBaseInfo MBaseInfo { get; private set; }

        public MapSupportInfo SupportInfo { get; private set; }

        public MapChInfo SVChInfo { get; private set; }
        public MapChInfo MChInfo { get; private set; }

        public MapStatistics StatisticsInfo { get; private set; }

        public Map(string mapName)
        {
            MapName = mapName;

            SVBaseInfo = new MapBaseInfo();
            MBaseInfo = new MapBaseInfo();

            SupportInfo = new MapSupportInfo();

            SVChInfo = new MapChInfo();
            MChInfo = new MapChInfo();

            StatisticsInfo = new MapStatistics();
        }

        public void RegisterMapLogicEvent(MapLogicEvent ml)
        {
            ml.Process(this);
            //_mapLogicEvents.Add(ml);
        }

        public void RegisterModEvent(ModEvent me)
        {
            me.ProcessMap(this);
        }
    }
}
