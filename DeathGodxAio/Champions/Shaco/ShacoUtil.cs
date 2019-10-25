using System;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using EnsoulSharp.SDK;
using EnsoulSharp.SDK.MenuUI;
using EnsoulSharp.SDK.MenuUI.Values;
using EnsoulSharp.SDK.Prediction;
using EnsoulSharp.SDK.Utility;
using EnsoulSharp;
using SharpDX;
using static EnsoulSharp.SDK.Items;

namespace ChewyMoonsShaco
{
    internal class ShacoUtil
    {
        public static Vector3 GetQPos(AIHeroClient target, bool serverPos, int distance = 150)
        {
            var enemyPos = serverPos ? target.Position : target.Position;
            var myPos = serverPos ? ObjectManager.Player.Position : ObjectManager.Player.Position;

            return enemyPos + Vector3.Normalize(enemyPos - myPos) * distance;
        }

        public static Vector2 GetShortestWayPoint(List<Vector2> waypoints)
        {
            return waypoints.MinOrDefault(x => x.Distance(ObjectManager.Player));
        }
    }
}