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

#endregion

namespace ChewyMoonsLux
{
    internal class QGapCloser
    {
        internal static void OnEnemyGapCloser(AIHeroClient sender, Gapcloser.GapcloserArgs args)
        {
            if (!ChewyMoonsLux.miscMenu["antiGapCloserQ"].GetValue<MenuBool>().Enabled)
            {
                return;
            }
            ChewyMoonsLux.Q.Cast(sender, ChewyMoonsLux.PacketCast);
        }
    }
}