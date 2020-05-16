using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DrDiagnostic
{
    class MapList
    {
        List<Map> _maps = new List<Map>();

        Map _currentMap;

        public MapList()
        { }

        public void SetCurrentMap(string mapName)
        {
            Map map = _maps.Find(a => String.Equals(a.MapName, mapName, StringComparison.InvariantCultureIgnoreCase));
            if (map == null)
            {
                map = new Map(mapName);
                _maps.Add(map);
            }

            map.StatisticsInfo.PlayedTimes++;
            _currentMap = map;
        }

        public void RegisterMapLogicEvent(MapLogicEvent ml)
        {
            _currentMap.RegisterMapLogicEvent(ml);
        }

        public void RegisterModEvent(ModEvent me)
        {
            _currentMap.RegisterModEvent(me);
        }
    }
}
