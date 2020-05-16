using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DrDiagnostic
{
    class MapLogicEvent : Event
    {
        string _name;
        List<string> _args;

        private MapLogicEvent(string name, List<string> args)
        {
            _name = name;
            _args = args;
        }

        public static MapLogicEvent Parse(string fromLog)
        { 
            // "DIAG;ML;...."
            string[] toks = fromLog.SplitWithFolding();
            if (toks[1] != "ML" || toks.Length < 3)
                return null;

            return new MapLogicEvent(toks[2], toks.Skip(3).ToList());
        }

        public void Process(Map map)
        {
            string name;
            int index;
            int lastValue;
            List<string> chItems;
            string strIndex;

            switch (_name)
            {
                // MapBaseInfo
                case "M":
                    map.MBaseInfo.FullName = _args[0];
                    if (!String.IsNullOrEmpty(_args[1]))
                        map.MBaseInfo.Difficulty = Int32.Parse(_args[1]);
                    if (!String.IsNullOrEmpty(_args[2]))
                        map.MBaseInfo.Length = Int32.Parse(_args[2]);

                    List<string> mappers = _args.Skip(3).ToList();
                    for (int i = 0; (i * 2 + 1) < mappers.Count && !String.IsNullOrEmpty(mappers[(i * 2) + 0]) && !String.IsNullOrEmpty(mappers[(i * 2) + 1]); i++)
                    {
                        name = mappers[(i * 2) + 0];
                        string guid = null;
                        if (!map.MBaseInfo.Mappers.TryGetValue(name, out guid))
                            map.MBaseInfo.Mappers.Add(name, "");

                        map.MBaseInfo.Mappers[name] = mappers[(i * 2) + 1];                  
                    }
                    break;
                case "L":
                    map.SVBaseInfo.FullName = _args[0];
                    if (!String.IsNullOrEmpty(_args[1]))
                        map.SVBaseInfo.Difficulty = Int32.Parse(_args[1]);
                    if (!String.IsNullOrEmpty(_args[2]))
                        map.SVBaseInfo.Length = Int32.Parse(_args[2]);
                    map.SVBaseInfo.IsNew = _args[3] == "1";
                    map.SVBaseInfo.IsInMapList = _args[4] == "1";
                    break;
                // MapSupportInfo
                case "S":
                    map.SupportInfo.SCODSpectatorsCount = Int32.Parse(_args[0]);
                    map.SupportInfo.SCODJumpersCount = Int32.Parse(_args[1]);
                    map.SupportInfo.SCODActivatorsCount = Int32.Parse(_args[2]);
                    map.SupportInfo.SDR1SpectatorsCount = Int32.Parse(_args[0]);
                    map.SupportInfo.SDR1JumpersCount = Int32.Parse(_args[3]);
                    map.SupportInfo.SDR1ActivatorsCount = Int32.Parse(_args[4]);
                    map.SupportInfo.SDR2SpectatorsCount = Int32.Parse(_args[5]);
                    map.SupportInfo.SDR2JumpersCount = Int32.Parse(_args[6]);
                    map.SupportInfo.SDR2ActivatorsCount = Int32.Parse(_args[7]);
                    map.SupportInfo.TTRegisteredCount = Int32.Parse(_args[8]);
                    map.SupportInfo.TJRegisteredCount = Int32.Parse(_args[9]);
                    map.SupportInfo.EMIsRegistered = _args[10] == "1";
                    break;
                case "T":
                    index = Int32.Parse(_args[0]);
                    lastValue = 0;
                    if (!map.SupportInfo.TTTouchedCount.TryGetValue(index, out lastValue))
                        map.SupportInfo.TTTouchedCount.Add(index, 0);

                    map.SupportInfo.TTTouchedCount[index] = lastValue + 1;
                    break;
                case "J":
                    index = Int32.Parse(_args[0]);
                    lastValue = 0;
                    if (!map.SupportInfo.TJTouchedCount.TryGetValue(index, out lastValue))
                        map.SupportInfo.TJTouchedCount.Add(index, 0);

                    map.SupportInfo.TJTouchedCount[index] = lastValue + 1;
                    break;
                case "E":
                    map.SupportInfo.EMTouchedCount++;
                    break;
                // MapChInfo
                case "CI":
                    map.MChInfo.Items.Add(ChItem.Parse(_args.ToArray()));
                    break;
                case "CS":
                    map.MChInfo.Stages.Add(ChStageList.Parse(_args.ToArray()));
                    break;
                case "CP":
                    strIndex = _args[0];
                    lastValue = 0;
                    if (!map.MChInfo.ProceedCount.TryGetValue(strIndex, out lastValue))
                        map.MChInfo.ProceedCount.Add(strIndex, 0);

                    map.MChInfo.ProceedCount[strIndex] = lastValue + 1;

                    if (!map.SVChInfo.ProceedCount.TryGetValue(strIndex, out lastValue))
                        map.SVChInfo.ProceedCount.Add(strIndex, 0);

                    map.SVChInfo.ProceedCount[strIndex] = lastValue + 1;
                    break;
                case "CC": // TODO: support for collecting items
                    strIndex = _args[0];
                    lastValue = 0;
                    if (!map.MChInfo.ProceedCount.TryGetValue(strIndex, out lastValue))
                        map.MChInfo.ProceedCount.Add(strIndex, 0);

                    map.MChInfo.ProceedCount[strIndex] = lastValue + 1;

                    if (!map.SVChInfo.ProceedCount.TryGetValue(strIndex, out lastValue))
                        map.SVChInfo.ProceedCount.Add(strIndex, 0);

                    map.SVChInfo.ProceedCount[strIndex] = lastValue + 1;
                    break;
                /*case "SI":
                    map.SVChInfo.Items.Add(ChItem.Parse(_args.ToArray()));
                    break;
                case "SS":
                    map.SVChInfo.Stages.Add(ChStageList.Parse(_args.ToArray()));
                    break;*/
                // MapStatisticInfo
                case "D":
                    if (_args[2] != "")
                        map.StatisticsInfo.Kills++;

                    map.StatisticsInfo.Deaths++;
                    break;
                case "C":
                    map.StatisticsInfo.Score += Int32.Parse(_args[0]);
                    break;
                case "A":
                    map.StatisticsInfo.Assists++;
                    break;
                case "RL":
                case "RV":
                    map.StatisticsInfo.PopRateSum = Int32.Parse(_args[0]);
                    map.StatisticsInfo.PopRateTimes = Int32.Parse(_args[1]);
                    break;
                default:
                    throw new ApplicationException(String.Format("Unknown MapLogicEvent arg '{0}'", _args[0]));
            }
        }
    }
}
