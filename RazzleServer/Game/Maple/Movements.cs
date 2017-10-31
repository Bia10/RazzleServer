﻿using System.Collections.Generic;
using RazzleServer.Common.Constants;
using RazzleServer.Common.Packet;

namespace RazzleServer.Game.Maple
{

    public sealed class Movement
    {
        public MovementType Type { get; set; }
        public Point Position { get; set; }
        public Point Velocity { get; set; }
        public short FallStart { get; set; }
        public short Foothold { get; set; }
        public short Duration { get; set; }
        public byte Stance { get; set; }
        public byte Statistic { get; set; }
    }

    public sealed class Movements : List<Movement>
    {
        public static Movements Decode(PacketReader iPacket)
        {
            return new Movements(iPacket);
        }

        public Point Origin { get; private set; }
        public Point Position { get; private set; }
        public short Foothold { get; private set; }
        public byte Stance { get; private set; }

        public Movements(PacketReader iPacket)
            : base()
        {
            short foothold = 0;
            byte stance = 0;
            Point position = new Point(iPacket.ReadShort(), iPacket.ReadShort());

            this.Origin = position;

            byte count = iPacket.ReadByte();

            while (count-- > 0)
            {
                MovementType type = (MovementType)iPacket.ReadByte();

                Movement movement = new Movement();

                movement.Type = type;
                movement.Foothold = foothold;
                movement.Position = position;
                movement.Stance = stance;

                switch (type)
                {
                    case MovementType.Normal:
                    case MovementType.Normal2:
                    case MovementType.JumpDown:
                    case MovementType.WingsFalling:
                        {
                            movement.Position = new Point(iPacket.ReadShort(), iPacket.ReadShort());
                            movement.Velocity = new Point(iPacket.ReadShort(), iPacket.ReadShort());
                            movement.Foothold = iPacket.ReadShort();

                            if (movement.Type == MovementType.JumpDown)
                            {
                                movement.FallStart = iPacket.ReadShort();
                            }

                            movement.Stance = iPacket.ReadByte();
                            movement.Duration = iPacket.ReadShort();
                        }
                        break;

                    case MovementType.Jump:
                    case MovementType.JumpKnockback:
                    case MovementType.FlashJump:
                    case MovementType.ExcessiveKnockback:
                    case MovementType.RecoilShot:
                        {
                            movement.Velocity = new Point(iPacket.ReadShort(), iPacket.ReadShort());
                            movement.Stance = iPacket.ReadByte();
                            movement.Duration = iPacket.ReadShort();
                        }
                        break;

                    case MovementType.Immediate:
                    case MovementType.Teleport:
                    case MovementType.Assaulter:
                    case MovementType.Assassinate:
                    case MovementType.Rush:
                    case MovementType.Chair:
                        {
                            movement.Position = new Point(iPacket.ReadShort(), iPacket.ReadShort());
                            movement.Foothold = iPacket.ReadShort();
                            movement.Stance = iPacket.ReadByte();
                            movement.Duration = iPacket.ReadShort();
                        }
                        break;

                    case MovementType.Falling:
                        {
                            movement.Statistic = iPacket.ReadByte();
                        }
                        break;

                    case MovementType.Unknown:
                        {
                            movement.Velocity = new Point(iPacket.ReadShort(), iPacket.ReadShort());
                            movement.FallStart = iPacket.ReadShort();
                            movement.Stance = iPacket.ReadByte();
                            movement.Duration = iPacket.ReadShort();
                        }
                        break;

                    default:
                        {
                            movement.Stance = iPacket.ReadByte();
                            movement.Duration = iPacket.ReadShort();
                        }
                        break;
                }

                position = movement.Position;
                foothold = movement.Foothold;
                stance = movement.Stance;

                this.Add(movement);
            }

            byte keypadStates = iPacket.ReadByte();

            for (byte i = 0; i < keypadStates; i++)
            {
                if (i % 2 == 0)
                {
                    iPacket.ReadByte(); // NOTE: Unknown.
                }
            }

            // NOTE: Rectangle for bounds checking.
            iPacket.ReadShort(); // NOTE: Left.
            iPacket.ReadShort(); // NOTE: Top.
            iPacket.ReadShort(); // NOTE: Right.
            iPacket.ReadShort(); // NOTE: Bottom.

            this.Position = position;
            this.Stance = stance;
            this.Foothold = foothold;
        }

        public byte[] ToByteArray()
        {
            using (var oPacket = new PacketWriter())
            {

                oPacket.WritePoint(Origin);
                oPacket.WriteByte(Count);

                foreach (Movement movement in this)
                {
                    oPacket.WriteByte((byte)movement.Type);

                    switch (movement.Type)
                    {
                        case MovementType.Normal:
                        case MovementType.Normal2:
                        case MovementType.JumpDown:
                        case MovementType.WingsFalling:
                            {

                                oPacket.WritePoint(movement.Position);
                                oPacket.WritePoint(movement.Velocity);
                                oPacket.WriteShort(movement.Foothold);

                                if (movement.Type == MovementType.JumpDown)
                                {
                                    oPacket.WriteShort(movement.FallStart);
                                }

                                oPacket.WriteByte(movement.Stance);
                                oPacket.WriteShort(movement.Duration);
                            }
                            break;

                        case MovementType.Jump:
                        case MovementType.JumpKnockback:
                        case MovementType.FlashJump:
                        case MovementType.ExcessiveKnockback:
                        case MovementType.RecoilShot:
                            {
                                oPacket.WriteShort(movement.Velocity.X);
                                oPacket.WriteShort(movement.Velocity.Y);
                                oPacket.WriteByte(movement.Stance);
                                oPacket.WriteShort(movement.Duration);
                            }
                            break;

                        case MovementType.Immediate:
                        case MovementType.Teleport:
                        case MovementType.Assaulter:
                        case MovementType.Assassinate:
                        case MovementType.Rush:
                        case MovementType.Chair:
                            {
                                oPacket.WritePoint(movement.Position);
                                oPacket.WriteShort(movement.Foothold);
                                oPacket.WriteByte(movement.Stance);
                                oPacket.WriteShort(movement.Duration);
                            }
                            break;

                        case MovementType.Falling:
                            {
                                oPacket.WriteByte(movement.Statistic);
                            }
                            break;

                        case MovementType.Unknown:
                            {

                                oPacket.WritePoint(movement.Velocity);
                                oPacket.WriteShort(movement.FallStart);
                                oPacket.WriteByte(movement.Stance);
                                oPacket.WriteShort(movement.Duration);
                            }
                            break;

                        default:
                            {

                                oPacket.WriteByte(movement.Stance);
                                oPacket.WriteShort(movement.Duration);
                            }
                            break;
                    }
                }

                // NOTE: Keypad and boundary values are not read on the client side.
                return oPacket.ToArray();
            }
        }
    }
}
