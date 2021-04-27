using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Mobiles
{
    public class DammyAI : BaseAI
    {
        public DammyAI(BaseCreature m) : base(m)
        {
            m_Mobile.Debug = true;
        }

        public override bool DoActionWander()
        {
            if (m_Mobile.synchro != null)
            {
                if (m_Mobile.GetDistanceToSqrt(m_Mobile.synchro.Location) > 1)
                {
                    Action = ActionType.Flee;
                }

            }
            return true;
        }

        public override bool DoActionFlee()
        {
            if (m_Mobile.Deleted || m_Mobile.Frozen || m_Mobile.Paralyzed || (m_Mobile.Spell != null && m_Mobile.Spell.IsCasting) || m_Mobile.DisallowAllMoves)
                return false;
            var master = m_Mobile.synchro.Location;

            m_Mobile.Direction = m_Mobile.synchro.Direction;
            m_Mobile.MoveToWorld(master + GetOffset(m_Mobile.synchro.Direction & Direction.Mask), m_Mobile.Map);

            return true;
        }

        private Point3D GetOffset(Direction d)
        {
            return d switch
            {
                Direction.North => new Point3D(-m_Mobile.offset.X, m_Mobile.offset.Y, m_Mobile.offset.Z),
                Direction.Right => new Point3D(m_Mobile.offset.Y, -m_Mobile.offset.X, m_Mobile.offset.Z),
                Direction.East => new Point3D(m_Mobile.offset.Y, -m_Mobile.offset.X, m_Mobile.offset.Z),
                Direction.Down => new Point3D(m_Mobile.offset.Y, -m_Mobile.offset.X, m_Mobile.offset.Z),
                Direction.South => new Point3D(m_Mobile.offset.X, m_Mobile.offset.Y, m_Mobile.offset.Z),
                Direction.Left => new Point3D(m_Mobile.offset.X, m_Mobile.offset.Y, m_Mobile.offset.Z),
                Direction.West => new Point3D(-m_Mobile.offset.Y, m_Mobile.offset.X, m_Mobile.offset.Z),
                Direction.Up => new Point3D(-m_Mobile.offset.X, m_Mobile.offset.Y, m_Mobile.offset.Z),
                _ => m_Mobile.offset
            };
        }
    }
}
