﻿namespace RazzleServer.Net.Packet
{
    public enum ClientOperationCode : byte
    {
        Login = 0x01,
        SelectChannel = 0x02,
        WorldStatus = 0x03,
        SelectCharacter = 0x04,
        CharacterLoad = 0x05,
        CheckName = 0x06,
        CreateCharacter = 0x07,
        DeleteCharacter = 0x08,
        [IgnorePacketPrint]
        Pong = 0x09,
        ClientCrashReport = 0x0A,
        [IgnorePacketPrint]
        ClientHash = 0x0E,

        ChangeField = 0x11,
        ChangeChannel = 0x12,
        FieldConnectCashShop = 0x13,
        [IgnorePacketPrint]
        PlayerMovement = 0x14,
        FieldPlayerSitMapChair = 0x15,
        AttackMelee = 0x16,
        AttackRanged = 0x17,
        AttackMagic = 0x18,
        TakeDamage = 0x1A,
        PlayerChat = 0x1B,
        FaceExpression = 0x1C,
        NpcSelect = 0x1F,

        NpcChat = 0x20,
        NpcShop = 0x21,
        NpcStorage = 0x22,
        InventoryChangeSlot = 0x23,
        UseItem = 0x24,
        UseSummonBag = 0x25,
        UseCashItem = 0x27,
        UseReturnScroll = 0x28,
        InventoryUseScrollOnItem = 0x29,
        DistributeAp = 0x2A,
        HealOverTime = 0x2B,
        DistributeSp = 0x2C,
        UseSkill = 0x2D,
        SkillStop = 0x2E,

        MesoDrop = 0x30,
        RemoteModifyFame = 0x31,
        PlayerInformation = 0x33,
        PetEnterField = 0x34,
        UseInnerPortal = 0x36,
        TeleportRockUse = 0x37,
        ReportPlayer = 0x38,
        AdminCommandMessage = 0x3A,
        MultiChat = 0x3B,
        CommandWhisperFind = 0x3C,
        CuiMessenger = 0x3D,
        PlayerInteraction = 0x3E, // Miniroom
        PartyCreate = 0x3F,
        PartyMessage = 040,
        AdminCommand = 0x41,
        AdminCommandLog = 0x42,
        Buddy = 0x43,
        MysticDoor = 0x45,
        PetMove = 0x48,
        PetChat = 0x49,
        PetAction = 0x4A,
        PetLoot = 0x4B,
        SummonMove = 0x4E,
        SummonAttack = 0x4F,
        SummonDamage = 0x50,

        [IgnorePacketPrint]
        MobMovement = 0x56,
        MobDistanceFromPlayer = 0x57,
        MobPickupDrop = 0x58,
        NpcMovement = 0x5B,
        DropPickup = 0x5F,
        AdminEventStart = 0x65,
        CoconutEvent = 0x68,
        AdminEventReset = 0x6A,
        BoatStatusRequest = 0x70,
        
        Unknown = 0xFF
    }
}
