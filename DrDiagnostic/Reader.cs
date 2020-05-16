using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace DrDiagnostic
{
    class Reader
    {
        private Listener _listener;
        private MapList _mapList = new MapList();

        public Reader(Listener listener)
        {
            _listener = listener;
        }

        public void ReadLogFile(string fileFullPath)
        {
            string[] lines = File.ReadAllLines(fileFullPath);

            const int contentI = 7;
            string content;
            foreach (string line in lines)
            {
                content = line.Substring(contentI);
                if (content.Length <= 0)
                    continue;

                if (content.StartsWith("InitGame: "))
                {
                    string[] toks = content.Split('\\');
                    bool isMap = false;
                    foreach (string tok in toks)
                    {
                        if (isMap)
                        {
                            _mapList.SetCurrentMap(tok);
                            isMap = false;
                        }

                        if (tok == "mapname")
                            isMap = true;
                    }
                }
                else if (content.StartsWith("DIAG;"))
                {
                    MapLogicEvent ml = MapLogicEvent.Parse(content);
                    if (ml != null)
                        _mapList.RegisterMapLogicEvent(ml);
                }
                else
                {
                    ModEvent me = ModEvent.Parse(content);
                    if (me != null)
                        _mapList.RegisterModEvent(me);
                }
            }
        }
    }
}
