using System;
using RazzleServer.Common.Constants;
using RazzleServer.Net.Packet;

namespace RazzleServer.Game
{
    public static class GamePackets
    {
        public static PacketWriter Notify(string message, NoticeType type = NoticeType.Popup)
        {
            using (var pw = new PacketWriter(ServerOperationCode.Notice))
            {
                pw.WriteByte((byte)type);

                if (type == NoticeType.ScrollingText)
                {
                    pw.WriteBool(!string.IsNullOrEmpty(message));
                }

                pw.WriteString(message);
                return pw;
            }
        }

        public static PacketWriter ShowStatusInfo(MessageType type, bool isMeso = false, int amount = 0,
            bool isWhite = false, bool inChat = false, int mapleId = 0,
            QuestStatus questStatus = QuestStatus.NotStarted, string questString = null)
        {
            var pw = new PacketWriter(ServerOperationCode.Message);

            pw.WriteByte((byte)type);

            if (type == MessageType.DropPickup)
            {
                pw.WriteBool(isMeso);

                if (isMeso)
                {
                    pw.WriteInt(amount);
                    pw.WriteShort(0); // iNet Cafe Meso Gain
                }
                else
                {
                    pw.WriteInt(mapleId);
                    pw.WriteInt(amount);
                    pw.WriteInt(0);
                    pw.WriteInt(0);
                }
            }
            else if (type == MessageType.IncreaseMeso)
            {
                pw.WriteInt(amount);
                pw.WriteShort(0); // iNet Cafe Meso Gain
            }
            else if (type == MessageType.IncreaseExp)
            {
                pw.WriteBool(isWhite);
                pw.WriteInt(amount);
                pw.WriteBool(inChat);
                pw.WriteInt(0);
                pw.WriteInt(0);
                pw.WriteInt(0);
            }
            else if (type == MessageType.IncreaseFame)
            {
                pw.WriteInt(amount);
            }
            else if (type == MessageType.QuestRecord)
            {
                pw.WriteShort(mapleId);
                pw.WriteByte((byte)questStatus);

                if (questStatus == QuestStatus.NotStarted)
                {
                    pw.WriteByte(0);
                    pw.WriteByte(0);
                    pw.WriteLong(0);
                }
                else if (questStatus == QuestStatus.InProgress)
                {
                    pw.WriteString(questString);

                    if (questString != null)
                    {
                        pw.WriteLong(0);
                    }
                }
                else if (questStatus == QuestStatus.Complete)
                {
                    pw.WriteByte(0);
                    pw.WriteDateTime(DateTime.Now);
                }
            }


            return pw;
        }
    }
}
