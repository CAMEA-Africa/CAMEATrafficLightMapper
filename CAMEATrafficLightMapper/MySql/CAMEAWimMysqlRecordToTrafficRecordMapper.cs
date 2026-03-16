using CAMEATrafficLightMapper.Models;

namespace CAMEATrafficLightMapper.MySql;

public static class CAMEAWimMysqlRecordToTrafficRecordMapper
{
    public static TrafficRecord ToTrafficRecord(CAMEAWimMysqlRecord src)
    {
        if (src is null) throw new ArgumentNullException(nameof(src));

        var rec = new TrafficRecord
        {
            Id = null,
            TimestampUtc = src.DateTime.AddMilliseconds(src.Ms),
            TimestampLocal = src.DateTimeLocal.AddMilliseconds(src.Ms),
            OppositeDirection = src.OppositeDirection,
            LicensePlateFront = src.Lp,
            LicensePlateBack = src.LpBack,
            Weight = src.TotalWeight,
            AxleCount = src.AxlesCount,
            VehicleDistance = new VehicleDistance
            {
                TimeDistanceFrontFront = src.TimeDistanceFrontFront,
                TimeDistanceBackFront = src.TimeDistanceBackFront
            },
            Speed = new Speed
            {
                Current = src.Velocity,
                Acceleration = src.Acceleration
            },
            Position = new Position
            {
                Left = src.LPos,
                Right = src.RPos,
                Vehicle = src.Possition
            },
            Vehicle = new Vehicle
            {
                OverhangFront = src.FrontOverhang,
                AxleLayout = src.CarLayout,
                ChassisHeight = src.ChassisHeight
            },
            RoadTemperature = src.TempAsphalt,
            WeighingAccuracy = src.SumAccu,
            StopGo = ToBoolNullable(src.StopGo),
            TrailerPresent = ToBoolNullable(src.TrailerPresence),
            VerificationDate = src.VerificationDate,
            Proprietary = new ProprietaryFields
            {
                RecordId = src.Id.ToString(),
                SensorId = src.Wim,
                TrafficLaneId = src.WimDetectorId.ToString(),
                DetectionId = src.DetectionId,
                DetectionImageIdFront = src.DetectionImageId,
                DetectionImageIdBack = src.DetectionImageIdBack,
                VehicleClass = src.VehicleClass.ToString(),
                OriginalVehicleClass = src.VehicleClassOrig.ToString(),
                LoopLength = src.Length,
                InsertTimestamp = src.InsertTimestamp,
                OverloadFlags = src.Overweighting,
                MatchDateTime = src.MatchDateTime,
                LoopClass = src.LoopClass.ToString(),
                DualTireBits = src.DualTire,
                WeighingValidity = src.Validity?.ToString(),
                WeighingValidityFlags = src.ValidityFlags,
                SpeedValidityFlags = src.VelocityValidity?.ToString(),
                DimensionValidityFlags = src.SDimensionsValidity?.ToString(),
                ErrorFlags = src.ErrorFlags,
                InternalTemperature = src.TempInternal,
                OverloadDescription = src.OverweightDesc,
                PassageType = src.PassageType?.ToString(),
                CameaWimClass = src.UnicamWimClass,
                VWidth = src.VWidth,
                Width = src.Width,
                LoopSpeed = src.VelocityLoop,
                ViolationCode = src.VCode,
                ViolationRaised = ToBoolNullable(src.VcRaised),
                ErrorCode = src.ECode,
                ErrorRaised = ToBoolNullable(src.EcRaised),
                ScannerHeight = src.SHeight,
                ScannerWidth = src.SWidth,
                ScannerLength = src.SLength,
                ScannerSeparation = src.SSeparated,
                OverheightFlags = src.Overheight?.ToString(),
                OversizeFlags = src.Oversize,
                OversizeDescriptions = src.OversizeDesc,
                OverspeedFlags = src.Overvelocity,
                OverspeedDescription = src.OvervelocityDesc,
                FoundInWhitelist = ToBoolNullable(src.FoundInWhitelist),
                WheelPressureTest = src.WheelPressureTest,
                LoopFrequencyMax = src.LoopFrqMax
            }
        };

        rec.Axles = BuildAxles(src);
        return rec;
    }

    private static List<Axle> BuildAxles(CAMEAWimMysqlRecord src)
    {
        if (src.AxlesCount <= 0) return new List<Axle>();
        var count = Math.Min((int)src.AxlesCount, 18);
        var axles = new List<Axle>(count);
        for (var i = 1; i <= count; i++)
        {
            axles.Add(new Axle
            {
                Number = i,
                LoadLeft = GetAxleLeftLoad(src, i),
                LoadRight = GetAxleRightLoad(src, i),
                Accuracy = GetAxleAccuracy(src, i),
                SpacingToNextAxle = i < count ? GetAxleSpacing(src, i) : null
            });
        }
        return axles;
    }

    private static int? GetAxleSpacing(CAMEAWimMysqlRecord s, int a) => a switch
    {
        1 => s.Axles12Base, 2 => s.Axles23Base, 3 => s.Axles34Base, 4 => s.Axles45Base,
        5 => s.Axles56Base, 6 => s.Axles67Base, 7 => s.Axles78Base, 8 => s.Axles89Base,
        9 => s.Axles910Base, 10 => s.Axles1011Base, 11 => s.Axles1112Base, 12 => s.Axles1213Base,
        13 => s.Axles1314Base, 14 => s.Axles1415Base, 15 => s.Axles1516Base, 16 => s.Axles1617Base,
        17 => s.Axles1718Base, _ => null
    };

    private static int? GetAxleLeftLoad(CAMEAWimMysqlRecord s, int a) => a switch
    {
        1 => s.Axle1LeftsideLoad, 2 => s.Axle2LeftsideLoad, 3 => s.Axle3LeftsideLoad,
        4 => s.Axle4LeftsideLoad, 5 => s.Axle5LeftsideLoad, 6 => s.Axle6LeftsideLoad,
        7 => s.Axle7LeftsideLoad, 8 => s.Axle8LeftsideLoad, 9 => s.Axle9LeftsideLoad,
        10 => s.Axle10LeftsideLoad, 11 => s.Axle11LeftsideLoad, 12 => s.Axle12LeftsideLoad,
        13 => s.Axle13LeftsideLoad, 14 => s.Axle14LeftsideLoad, 15 => s.Axle15LeftsideLoad,
        16 => s.Axle16LeftsideLoad, 17 => s.Axle17LeftsideLoad, 18 => s.Axle18LeftsideLoad,
        _ => null
    };

    private static int? GetAxleRightLoad(CAMEAWimMysqlRecord s, int a) => a switch
    {
        1 => s.Axle1RightsideLoad, 2 => s.Axle2RightsideLoad, 3 => s.Axle3RightsideLoad,
        4 => s.Axle4RightsideLoad, 5 => s.Axle5RightsideLoad, 6 => s.Axle6RightsideLoad,
        7 => s.Axle7RightsideLoad, 8 => s.Axle8RightsideLoad, 9 => s.Axle9RightsideLoad,
        10 => s.Axle10RightsideLoad, 11 => s.Axle11RightsideLoad, 12 => s.Axle12RightsideLoad,
        13 => s.Axle13RightsideLoad, 14 => s.Axle14RightsideLoad, 15 => s.Axle15RightsideLoad,
        16 => s.Axle16RightsideLoad, 17 => s.Axle17RightsideLoad, 18 => s.Axle18RightsideLoad,
        _ => null
    };

    private static int? GetAxleAccuracy(CAMEAWimMysqlRecord s, int a) => a switch
    {
        1 => (int?)s.AxAccu1, 2 => (int?)s.AxAccu2, 3 => (int?)s.AxAccu3, 4 => (int?)s.AxAccu4,
        5 => (int?)s.AxAccu5, 6 => (int?)s.AxAccu6, 7 => (int?)s.AxAccu7, 8 => (int?)s.AxAccu8,
        9 => (int?)s.AxAccu9, 10 => (int?)s.AxAccu10, 11 => (int?)s.AxAccu11, 12 => (int?)s.AxAccu12,
        13 => (int?)s.AxAccu13, 14 => (int?)s.AxAccu14, 15 => (int?)s.AxAccu15, 16 => (int?)s.AxAccu16,
        17 => (int?)s.AxAccu17, 18 => (int?)s.AxAccu18, _ => null
    };

    private static bool? ToBoolNullable(sbyte? v) => v.HasValue ? v.Value != 0 : null;
    private static bool? ToBoolNullable(byte? v) => v.HasValue ? v.Value != 0 : null;
}
