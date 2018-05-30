﻿namespace RazzleServer.Common.Packet
{
    public enum ServerOperationCode : ushort
    {
        CheckPasswordResult = 0x00,
        SendLink = 0x01,
        CheckUserLimitResult = 0x03,
        SetGender = 0x04,
        PinCodeOperation = 0x06,
        PinAssigned = 0x07,
        AllCharList = 0x08,
        WorldInformation = 0x0A,
        SelectWorldResult = 0x0B,
        SelectCharacterResult = 0x0C,
        CheckCharacterNameResult = 0x0D,
        CreateNewCharacterResult = 0x0E,
        DeleteCharacterResult = 0x0F,

        ChangeChannel = 0x10,
        Ping = 0x11,
        ReloginResponse = 0x16,
        InventoryOperation = 0x1A,
        StatChanged = 0x1C,
        BuffGive = 0x1D,
        BuffCancel = 0x1E,

        SkillsUpdate = 0x21,
        FameResponse = 0x23,
        ShowStatusInfo = 0x24,
        Notes = 0x26,
        TeleportRockLocations = 0x27,
        MountUpdate = 0x2D,
        QuestClear = 0x2E,
        TitleBoxSend = 0x2F,

        UseSkillBook = 0x30,
        ItemSortComplete = 0x31,
        ItemSortComplete2 = 0x32,
        ReportPlayer = 0x33,
        ReportPlayerMessage = 0x34,
        BbsOperation = 0x38,
        CharacterInformation = 0x3A,
        PartyOperation = 0x3B,
        Buddylist = 0x3C,
        GuildOperation = 0x3E,

        PortalSpawn = 0x40,
        BroadcastMsg = 0x41,

        SetAvatarMegaphone = 0x54,
        GmPolice = 0x59,
        SilverBoxOpen = 0x5A,
        SkillMacro = 0x5B,
        SetField = 0x5C,
        CashShopOpen = 0x5E,
        //BOSS_ENV = 0x54
        //MULTICHAT = 0x56

        ResetScreen = 0x60,
        BlockPortal = 0x62,
        ShowApple = 0x63,
        GroupMessage = 0x64,
        Whisper = 0x65,
        Clock = 0x6E,
        UserEnterField = 0x78,
        UserLeaveField = 0x79,
        UserChat = 0x7A,
        Chalkboard = 0x7B,
        SummonSpawn = 0x73,
        SummonRemove = 0x74,
        SummonMove = 0x75,
        SummonAttack = 0x76,
        SummonDamage = 0x78,
        ShowScrollEffect = 0x7B,

        Move = 0x8D,
        CloseRangeAttack = 0x8E,
        RangedAttack = 0x8F,

        MagicAttack = 0x90,
        Hit = 0x94,
        Emotion = 0x95,
        ItemEffect = 0x96,
        ShowChair = 0x97,
        AvatarModified = 0x98,
        ForeignEffectShow = 0x99,
        ForeignBuffCancel = 0x9B,
        PartyUpdateHp = 0x9C,

        Sit = 0xA0,
        Message = 0xA1,
        LuckSackPass = 0xA4,
        LuckSackFail = 0xA5,
        QuestResult = 0xA6,
        Hint = 0xA9,
        Cooldown = 0xAD,
        MobEnterField = 0xAF,

        MobLeaveField = 0xB0,
        MobChangeController = 0xB1,
        MobMove = 0xB2,
        MobMoveResponse = 0xB3,
        MobApplyStatus = 0xB5,
        MobRemoveStatus = 0xB6,
        MobDamageMob = 0xB8,
        MobDamaged = 0xB9,
        AriantThing = 0xBC,
        MobHpIndicator = 0xBD,
        ShowMagnet = 0xBE,
        CatchMonster = 0xBF,

        NpcEnterField = 0xC2,
        NpcLeaveField = 0xC3,
        NpcChangeController = 0xC4,
        NpcMove = 0xC5,
        HiredMerchantEnterField = 0xCA,
        HiredMerchanLeaveField = 0xCB,
        HiredMerchantUpdate = 0xCC,
        DropEnterField = 0xCD,
        DropLeaveField = 0xCE,

        MistSpawn = 0xD2,
        MistRemove = 0xD3,
        DoorSpawn = 0xD4,
        DoorRemove = 0xD5,
        ReactorChangeState = 0xD6,
        ReactorEnterField = 0xD8,
        ReactorLeaveField = 0xD9,

        MonsterCarnivalStart = 0xE2,
        MonsterCarnivalObtainCp = 0xE3,
        MonsterCarnivalPartyCp = 0xE4,
        MonsterCarnivalSummon = 0xE5,
        MonsterCarnivalDied = 0xE7,
        AriantPqStart = 0xEA,
        ZakumShrine = 0xEC,
        NpcTalk = 0xED,
        OpenNpcShop = 0xEE,
        ConfirmShopTransaction = 0xEF,

        Storage = 0xF0,
        Messenger = 0xF4,
        PlayerInteraction = 0xF5,
        Duey = 0xFD,
        CashShopUpdate = 0xFF,

        CashShopOperation = 0x100,
        Keymap = 0x107,
        Kite = 0x10C,
        TvEnterField = 0x10D,
        TvLeaveField = 0x10E,
        TvEnable = 0x10F,

        MtsOperation2 = 0x113,
        MtsOperation = 0x114,

        // TODO
        ChangeSkillRecordResult = 36,
        MemoResult = 41,
        MapTransferResult = 42,
        SueCharacterResult = 55,
        TransferFieldReqInogred = 131,
        AdminResult = 144,
        AnnounceBox = 165,
        RemoteEffect = 198,
        SetTemporaryStat = 199,
        ResetTemporaryStat = 200,
        Effect = 206,
        Teleport = 207,
        BalloonMsg = 214,
        MobStatSet = 242,
        MobStatReset = 243,
        ScriptMessage = 304,
        AdminShopMessage = 307,
        AdminShop = 308,
        TemporaryStatSet = 333,
        TemporaryStatReset = 333
    }
}
