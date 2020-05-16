using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DrDiagnostic
{
    class ModEvent
    {
        List<string> _args;

        private ModEvent(List<string> args)
        {
            _args = args;
        }

        public static ModEvent Parse(string fromLog)
        {
            if (fromLog.StartsWith("J;") || fromLog.StartsWith("Q;"))
            {
                string[] toks = fromLog.SplitWithFolding();
                return new ModEvent(toks.ToList());
            }

            return null;
        }

        public void ProcessMap(Map map)
        {
            switch (_args[0])
            { 
                case "J":
                    map.StatisticsInfo.PlayersConnected++;
                    break;
                case "Q":
                    map.StatisticsInfo.PlayersDisconnected++;
                    break;
                default:
                    break;
            }
        }
    }
}
