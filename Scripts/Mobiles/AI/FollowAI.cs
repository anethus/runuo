namespace Server.Mobiles
{
    public class FollowAI : BaseAI
    {
        public FollowAI(BaseCreature m) : base(m)
        {
            m_Mobile.Debug = true;
        }

        public override bool DoActionWander()
        {
            Action = ActionType.Combat;
            return true;
        }

        public override bool DoActionCombat()
        {
            if (m_Mobile.CurrentWayPoint != null)
            {
                var wp = m_Mobile.CurrentWayPoint;
                
                if ((wp.X != m_Mobile.Location.X || wp.Y != m_Mobile.Location.Y) && !wp.Deleted)
                    DoMove(m_Mobile.GetDirectionTo(m_Mobile.CurrentWayPoint) | Direction.Running, true);
                else
                {
                    if (wp.NextPoint != null)
                    {
                        m_Mobile.CurrentWayPoint = wp.NextPoint;
                        m_Mobile.Direction = m_Mobile.GetDirectionTo(wp.NextPoint.Location);
                    }
                }
            }
            return true;
        }
    }
}
