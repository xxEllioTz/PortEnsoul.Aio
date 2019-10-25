#region
using System;
using System.Linq;
using System.Collections.Generic;
using EnsoulSharp.SDK;
using EnsoulSharp.SDK.MenuUI;
using EnsoulSharp.SDK.MenuUI.Values;
using EnsoulSharp.SDK.Prediction;
using EnsoulSharp.SDK.Utility;
using EnsoulSharp;
using SharpDX;
using Color = System.Drawing.Color;
using static EnsoulSharp.SDK.Items;
using SharpDX.Direct3D9;
using static EnsoulSharp.SDK.Prediction.SpellPrediction;

#endregion

namespace ChewyMoonsLux
{
    public class SpellCombo
    {
       // public static bool AnalyzeQ(PredictionInput input, PredictionOutput output)
        //{
          //      var posList = new List<Vector3> { ObjectManager.Player.Position, output.CastPosition };
         //   var collision = Collision.GetCollision(posList, input);
          //  var minions = collision.Count(collisionObj => collisionObj.IsMinion);
            //return minions > 1;
        //}

        public static void CastQ(AIHeroClient target)
        {
            Console.Clear();

            var prediction = ChewyMoonsLux.Q.GetPrediction(target, true);
            var minions = prediction.CollisionObjects.Count(thing => thing.IsMinion);

            if (ChewyMoonsLux.Debug)
            {
                Console.WriteLine("Minions: {0}\nToo Many: {1}", minions, minions > 1);
            }

            if (minions > 1)
            {
                return;
            }
            var useR = ChewyMoonsLux.comboMenu["useR"].GetValue<MenuBool>();
            ChewyMoonsLux.Q.Cast(prediction.CastPosition, ChewyMoonsLux.PacketCast);
            var targeto = TargetSelector.GetTarget(1000);
            if (ChewyMoonsLux.R.IsReady() && useR)
            {
                ChewyMoonsLux.R.Cast(targeto);
            }
        }
    }
}