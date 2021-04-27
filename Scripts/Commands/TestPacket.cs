using System.Collections.Generic;
using System.Linq;
using Server.Gumps;
using Server.Mobiles;
using Server.Network;
using Server.Targeting;

namespace Server.Commands
{
    public static class TestPacket
    {
        private static List<Mobile> mList = new List<Mobile>();

        public static void Initialize()
        {
            CommandSystem.Register("TestPacket", AccessLevel.Administrator, TestPacket_OnCommand);
        }

        private static void TestPacket_OnCommand(CommandEventArgs e)
        {
            var mobile = e.Mobile;
            mobile.SendGump(new PacketTest());
        }

        private static void OnTarget(Mobile from, object targeted)
        {
            if (!(targeted is Mobile mobile))
            {
                from.SendGump(new PacketTest());
                return;
            }
            mList.Add(mobile);

            if (mList.Count != 1)
            {
                var master = World.FindMobile(mList[0].Serial) as BaseCreature;
                if(mobile is BaseCreature bc)
                {
                    bc.synchro = master;
                    if (mList.Count == 2)
                        (mobile as BaseCreature).offset = new Point3D(-1, 0, 0);
                    if (mList.Count == 3)
                        (mobile as BaseCreature).offset = new Point3D(1, 0, 0);
                    if (mList.Count == 4)
                        (mobile as BaseCreature).offset = new Point3D(-1, -1, 0);
                    if (mList.Count == 5)
                        (mobile as BaseCreature).offset = new Point3D(1, -1, 0);
                }
                else
                {
                    from.SendMessage("It's not creature");
                }
            }

            from.SendGump(new PacketTest());
        }


        private static void AddWaypointTarget(Mobile from, object targeted)
        {
            if (targeted is Items.WayPoint wp)
            {
                var mob = World.FindMobile(mList[0].Serial);
                if (mob is BaseCreature bc)
                {
                    bc.CurrentWayPoint = wp;
                    from.SendGump(new PacketTest());
                    return;
                }
            }
            from.SendMessage("This is not a waypoint");
            from.SendGump(new PacketTest());
        }


        private static void OnGroundTarget(Mobile from, object target)
        {
            if (target is IPoint3D p)
            {
                p = p switch
                {
                    Item item => item.GetWorldTop(),
                    Mobile mobile => mobile.Location,
                    _ => p
                };

                Point3D point = new Point3D(p);
                if (mList.Count != 0)
                {
                    var m = World.FindMobile(mList[0].Serial);
                    var wp = new Items.WayPoint();
                    wp.MoveToWorld(point, m.Map);
                    wp.Movable = false;
                    (m as BaseCreature).CurrentWayPoint = wp;
                    from.SendGump(new PacketTest());
                }
            }
        }

        public class PacketTest : Gump
        {
            public PacketTest() : base(30, 30)
            {
                AddPage(0);


                AddBackground(1, 1, 200, 300, 3600);

                foreach (var item in mList.Select((x, idx) => new { Value = x, Index = idx }))
                {
                    var xoffset = item.Index > 0 ? 20 : 0;
                    AddLabel(20 + xoffset, 20 + item.Index * 20, 56, item.Value.Serial.ToString());
                }
                AddLabel(10, 240, 67, "Add Mob");
                AddButton(20, 280, 4005, 4007, 1, GumpButtonType.Reply, 0);

                AddLabel(50, 260, 67, "Clear List");
                AddButton(60, 280, 4005, 4007, 3, GumpButtonType.Reply, 0);

                if (mList.Count > 0)
                {
                    AddLabel(100, 240, 67, "Target WP");
                    AddButton(110, 280, 4005, 4007, 2, GumpButtonType.Reply, 0);

                    AddLabel(140, 260, 67, "Target Ground");
                    AddButton(160, 280, 4005, 4007, 4, GumpButtonType.Reply, 0);
                }

            }

            public override void OnResponse(NetState sender, RelayInfo info)
            {
                switch (info.ButtonID)
                {
                    case 1:
                        {
                            sender.Mobile.BeginTarget(20, false, TargetFlags.None, OnTarget);
                            break;
                        }
                    case 2:
                        {
                            sender.Mobile.BeginTarget(20, true, TargetFlags.None, AddWaypointTarget);
                            break;
                        }
                    case 3:
                        {
                            mList.Clear();
                            sender.Mobile.SendGump(new PacketTest());
                            break;
                        }
                    case 4:
                        {
                            sender.Mobile.BeginTarget(20, true, TargetFlags.None, OnGroundTarget);
                            break;
                        }
                }
            }
        }
    }
}
