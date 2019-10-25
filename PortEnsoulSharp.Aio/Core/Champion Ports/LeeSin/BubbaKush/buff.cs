using EnsoulSharp;
using EnsoulSharp.SDK;
using SharpDX;
namespace BubbaKushsLeeSin
{
    internal class buff
    {

        public static void Orbwalk(AttackableUnit target = null, Vector3? position = null)
        {
            if (Orbwalker.CanAttack() && Orbwalker.AttackState)
            {
                var gTarget = target ?? Orbwalker.GetTarget();
                if (gTarget.InAutoAttackRange())
                {
                    Orbwalker.Attack(gTarget);
                }
            }

            if (Orbwalker.CanMove() && Orbwalker.MovementState)
            {
                Orbwalker.Move(position.HasValue && position.Value.IsValid() ? position.Value : Game.CursorPos);
            }
        }


    }

}
