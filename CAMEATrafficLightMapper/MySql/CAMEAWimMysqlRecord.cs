namespace CAMEATrafficLightMapper.MySql;

public sealed class CAMEAWimMysqlRecord
{
    public ulong Id { get; init; }

    public DateTime DateTime { get; init; }
    public DateTime DateTimeLocal { get; init; }
    public ushort Ms { get; init; }

    public string Wim { get; init; } = null!;

    public bool OppositeDirection { get; init; }

    public sbyte WimDetectorId { get; init; }

    public string? DetectionId { get; init; }

    public string? DetectionImageId { get; init; }
    public string? DetectionImageIdBack { get; init; }

    public int TimeDistanceFrontFront { get; init; }
    public int TimeDistanceBackFront { get; init; }

    public short VehicleClass { get; init; }
    public short VehicleClassOrig { get; init; }

    public short Velocity { get; init; }
    public short Length { get; init; }

    public string? Lp { get; init; }
    public string? LpBack { get; init; }

    public DateTime InsertTimestamp { get; init; }

    public int? TotalWeight { get; init; }

    public sbyte AxlesCount { get; init; }

    public short? FrontOverhang { get; init; }

    public short? Axles12Base { get; init; }
    public short? Axles23Base { get; init; }
    public short? Axles34Base { get; init; }
    public short? Axles45Base { get; init; }
    public short? Axles56Base { get; init; }
    public short? Axles67Base { get; init; }
    public short? Axles78Base { get; init; }
    public short? Axles89Base { get; init; }
    public short? Axles910Base { get; init; }
    public short? Axles1011Base { get; init; }
    public short? Axles1112Base { get; init; }
    public short? Axles1213Base { get; init; }
    public short? Axles1314Base { get; init; }
    public short? Axles1415Base { get; init; }
    public short? Axles1516Base { get; init; }
    public short? Axles1617Base { get; init; }
    public short? Axles1718Base { get; init; }

    public short? Axle1LeftsideLoad { get; init; }
    public short? Axle1RightsideLoad { get; init; }
    public short? Axle2LeftsideLoad { get; init; }
    public short? Axle2RightsideLoad { get; init; }
    public short? Axle3LeftsideLoad { get; init; }
    public short? Axle3RightsideLoad { get; init; }
    public short? Axle4LeftsideLoad { get; init; }
    public short? Axle4RightsideLoad { get; init; }
    public short? Axle5LeftsideLoad { get; init; }
    public short? Axle5RightsideLoad { get; init; }
    public short? Axle6LeftsideLoad { get; init; }
    public short? Axle6RightsideLoad { get; init; }
    public short? Axle7LeftsideLoad { get; init; }
    public short? Axle7RightsideLoad { get; init; }
    public short? Axle8LeftsideLoad { get; init; }
    public short? Axle8RightsideLoad { get; init; }
    public short? Axle9LeftsideLoad { get; init; }
    public short? Axle9RightsideLoad { get; init; }
    public short? Axle10LeftsideLoad { get; init; }
    public short? Axle10RightsideLoad { get; init; }
    public short? Axle11LeftsideLoad { get; init; }
    public short? Axle11RightsideLoad { get; init; }
    public short? Axle12LeftsideLoad { get; init; }
    public short? Axle12RightsideLoad { get; init; }
    public short? Axle13LeftsideLoad { get; init; }
    public short? Axle13RightsideLoad { get; init; }
    public short? Axle14LeftsideLoad { get; init; }
    public short? Axle14RightsideLoad { get; init; }
    public short? Axle15LeftsideLoad { get; init; }
    public short? Axle15RightsideLoad { get; init; }
    public short? Axle16LeftsideLoad { get; init; }
    public short? Axle16RightsideLoad { get; init; }
    public short? Axle17LeftsideLoad { get; init; }
    public short? Axle17RightsideLoad { get; init; }
    public short? Axle18LeftsideLoad { get; init; }
    public short? Axle18RightsideLoad { get; init; }

    public string Overweighting { get; init; } = null!;

    public DateTime MatchDateTime { get; init; }

    public sbyte LoopClass { get; init; }

    public short? LPos { get; init; }
    public short? RPos { get; init; }

    public string? DualTire { get; init; }

    public sbyte? Validity { get; init; }

    public string? ValidityFlags { get; init; }
    public sbyte? VelocityValidity { get; init; }
    public sbyte? SDimensionsValidity { get; init; }
    public string? ErrorFlags { get; init; }

    public short? TempInternal { get; init; }
    public short? TempAsphalt { get; init; }

    public int? Acceleration { get; init; }

    public ushort? AxAccu1 { get; init; }
    public ushort? AxAccu2 { get; init; }
    public ushort? AxAccu3 { get; init; }
    public ushort? AxAccu4 { get; init; }
    public ushort? AxAccu5 { get; init; }
    public ushort? AxAccu6 { get; init; }
    public ushort? AxAccu7 { get; init; }
    public ushort? AxAccu8 { get; init; }
    public ushort? AxAccu9 { get; init; }
    public ushort? AxAccu10 { get; init; }
    public ushort? AxAccu11 { get; init; }
    public ushort? AxAccu12 { get; init; }
    public ushort? AxAccu13 { get; init; }
    public ushort? AxAccu14 { get; init; }
    public ushort? AxAccu15 { get; init; }
    public ushort? AxAccu16 { get; init; }
    public ushort? AxAccu17 { get; init; }
    public ushort? AxAccu18 { get; init; }

    public int? SumAccu { get; init; }

    public string? OverweightDesc { get; init; }

    public byte? PassageType { get; init; }

    public string? UnicamWimClass { get; init; }

    public short? VWidth { get; init; }
    public short? Width { get; init; }
    public short? Possition { get; init; }
    public short? VelocityLoop { get; init; }

    public string? VCode { get; init; }
    public byte? VcRaised { get; init; }
    public string? ECode { get; init; }
    public byte? EcRaised { get; init; }

    public string? CarLayout { get; init; }

    public sbyte? StopGo { get; init; }

    public ushort? ChassisHeight { get; init; }

    public short? SHeight { get; init; }
    public short? SWidth { get; init; }
    public int? SLength { get; init; }
    public sbyte? SSeparated { get; init; }

    public sbyte? Overheight { get; init; }

    public string? Oversize { get; init; }
    public string? OversizeDesc { get; init; }

    public string? Overvelocity { get; init; }
    public string? OvervelocityDesc { get; init; }

    public sbyte? TrailerPresence { get; init; }
    public sbyte? FoundInWhitelist { get; init; }

    public string? WheelPressureTest { get; init; }

    public DateTime? VerificationDate { get; init; }

    public int? LoopFrqMax { get; init; }
}
