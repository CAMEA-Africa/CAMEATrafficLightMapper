namespace CAMEATrafficLightMapper.Models;

public sealed class TrafficRecord
{
    public string? Id { get; set; }

    public DateTime TimestampUtc { get; set; }
    public DateTime TimestampLocal { get; set; }

    public string? LicensePlateFront { get; set; }
    public string? LicensePlateBack { get; set; }

    public int? Weight { get; set; }
    public int? WeighingAccuracy { get; set; }
    public int? AxleCount { get; set; }
    public List<Axle> Axles { get; set; } = new();
    public Speed Speed { get; set; } = new();

    public bool? OppositeDirection { get; set; }
    public bool? StopGo { get; set; }
    public bool? TrailerPresent { get; set; }

    public int? RoadTemperature { get; set; }
    public VehicleDistance VehicleDistance { get; set; } = new();

    public Position Position { get; set; } = new();
    public Vehicle Vehicle { get; set; } = new();

    public DateTime? VerificationDate { get; set; }

    public ProprietaryFields Proprietary { get; set; } = new();
}

public sealed class Speed
{
    public int? Current { get; set; }
    public int? Acceleration { get; set; }
}

public sealed class Axle
{
    public int Number { get; set; }
    public int? SpacingToNextAxle { get; set; }
    public int? LoadLeft { get; set; }
    public int? LoadRight { get; set; }
    public int? Accuracy { get; set; }
}

public sealed class Vehicle
{
    public int? OverhangFront { get; set; }
    public string? AxleLayout { get; set; }
    public ushort? ChassisHeight { get; set; }
}

public sealed class VehicleDistance
{
    public int TimeDistanceFrontFront { get; set; }
    public int TimeDistanceBackFront { get; set; }
}

public sealed class Position
{
    public int? Left { get; set; }
    public int? Right { get; set; }
    public int? Vehicle { get; set; }
}

public sealed class ProprietaryFields
{
    public string? RecordId { get; set; }
    public string? SensorId { get; set; }
    public string? TrafficLaneId { get; set; }

    public string? DetectionId { get; set; }
    public string? DetectionImageIdFront { get; set; }
    public string? DetectionImageIdBack { get; set; }

    public string? VehicleClass { get; set; }
    public string? OriginalVehicleClass { get; set; }

    public int? LoopLength { get; set; }
    public DateTime? InsertTimestamp { get; set; }

    public string? OverloadFlags { get; set; }
    public string? OverloadDescription { get; set; }

    public DateTime? MatchDateTime { get; set; }
    public string? LoopClass { get; set; }

    public string? DualTireBits { get; set; }

    public string? WeighingValidity { get; set; }
    public string? WeighingValidityFlags { get; set; }
    public string? SpeedValidityFlags { get; set; }
    public string? DimensionValidityFlags { get; set; }

    public string? ErrorFlags { get; set; }

    public int? InternalTemperature { get; set; }

    public string? PassageType { get; set; }
    public string? CameaWimClass { get; set; }

    public int? VWidth { get; set; }
    public int? Width { get; set; }

    public int? LoopSpeed { get; set; }

    public string? ViolationCode { get; set; }
    public bool? ViolationRaised { get; set; }

    public string? ErrorCode { get; set; }
    public bool? ErrorRaised { get; set; }

    public int? ScannerHeight { get; set; }
    public int? ScannerWidth { get; set; }
    public int? ScannerLength { get; set; }
    public int? ScannerSeparation { get; set; }

    public string? OverheightFlags { get; set; }
    public string? OversizeFlags { get; set; }
    public string? OversizeDescriptions { get; set; }

    public string? OverspeedFlags { get; set; }
    public string? OverspeedDescription { get; set; }

    public bool? FoundInWhitelist { get; set; }
    public string? WheelPressureTest { get; set; }

    public int? LoopFrequencyMax { get; set; }
}
