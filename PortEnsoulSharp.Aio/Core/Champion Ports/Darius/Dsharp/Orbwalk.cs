using System;

using EnsoulSharp;
using EnsoulSharp.SDK;

namespace Darius.Orb
{

    public static class Orbwalk
    {
        private static int _lastAutoAttackSent;
        private static bool _autoAttackStarted;

        internal static bool _waitingPostAttackEvent;
        internal static bool _waitingForAutoAttackReset;
        private static float _lastCastEndTime;
        private static bool _setCastEndTime;
        private static bool _autoAttackCompleted;

        public delegate void PostAttackHandler(AttackableUnit target, EventArgs args);
        public static event PostAttackHandler OnPostAttack;

        internal static void TriggerPostAttackEvent(AttackableUnit target, EventArgs args = null)
        {
            if (OnPostAttack != null)
            {
                NotifyEventListeners("OnPostAttack", OnPostAttack.GetInvocationList(), target, args ?? EventArgs.Empty);
            }
        }

        internal static void NotifyEventListeners(string eventName, Delegate[] invocationList, params object[] args)
        {
            foreach (var listener in invocationList)
            {
                listener.DynamicInvoke(args);

            }
        }
        public static bool CanAutoAttack
        {
            get
            {
                if (!ObjectManager.Player.CanAttack && _waitingForAutoAttackReset)
                {
                    return false;
                }
                if (ObjectManager.Player.Spellbook.IsChanneling)
                {
                    return false;
                }
                switch (ObjectManager.Player.CharacterName)
                {
                    case "Jhin":
                        if (ObjectManager.Player.HasBuff("JhinPassiveReload"))
                        {
                            return false;
                        }
                        break;
                    case "Kalista":
                        if (ObjectManager.Player.IsDashing())
                        {
                            return false;
                        }
                        break;
                }
                return CanIssueOrder;
            }
            internal set
            {
                if (value)
                {
                    _autoAttackStarted = false;
                    _autoAttackCompleted = true;
                    LastAutoAttack = 0;
                    _lastCastEndTime = 0;
                    _lastAutoAttackSent = 0;
                }
                else
                {
                    _autoAttackStarted = true;
                    _autoAttackCompleted = false;
                    _waitingPostAttackEvent = true;
                    _setCastEndTime = true;
                }

            }
        }


        public static bool GotAutoAttackReset { get; internal set; }
        public static void ResetAutoAttack()
        {
            CanAutoAttack = true;
            GotAutoAttackReset = true;
        }
        public static int LastAutoAttack { get; internal set; }
        private static bool CanIssueOrder
        {
            get
            {
                if (Variables.GameTimeTickCount - _lastAutoAttackSent <= 100 + Game.Ping)
                {
                    return false;
                }
                return Variables.GameTimeTickCount - LastAutoAttack + Game.Ping + 70 >= AttackDelay * 1000;
            }
        }
        public static float AttackDelay
        {
            get
            {
                switch (ObjectManager.Player.CharacterName)
                {
                    case "Graves":
                        if (ObjectManager.Player.HasBuff("GravesBasicAttackAmmo1"))
                        {
                            return 1.0740296828f * ObjectManager.Player.AttackDelay - 0.7162381256175f;
                        }
                        break;
                }
                return ObjectManager.Player.AttackDelay;
            }
        }
    }
}
