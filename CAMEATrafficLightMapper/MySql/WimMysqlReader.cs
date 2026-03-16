using MySqlConnector;
using System.Runtime.CompilerServices;

namespace CAMEATrafficLightMapper.MySql;

public sealed class WimMysqlReader
{
    private readonly string _connectionString;

    public WimMysqlReader(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task PingAsync(CancellationToken ct = default)
    {
        await using var conn = new MySqlConnection(_connectionString);
        await conn.OpenAsync(ct);
    }

    public async IAsyncEnumerable<CAMEAWimMysqlRecord> ReadAsync(
        long? fromId,
        int batchSize,
        [EnumeratorCancellation] CancellationToken ct)
    {
        await using var conn = new MySqlConnection(_connectionString);
        await conn.OpenAsync(ct);

        ulong lastId = (fromId.HasValue && fromId.Value > 0) ? (ulong)fromId.Value : 0UL;

        while (true)
        {
            await using var cmd = conn.CreateCommand();
            cmd.CommandText = Sql;
            cmd.Parameters.AddWithValue("@fromId", lastId);
            cmd.Parameters.AddWithValue("@batchSize", batchSize);

            int count = 0;

            await using var r = await cmd.ExecuteReaderAsync(ct);
            while (await r.ReadAsync(ct))
            {
                var rec = Map(r);
                lastId = rec.Id;
                count++;
                yield return rec;
            }

            if (count == 0)
                yield break;
        }
    }

    private const string Sql = @"
SELECT
    d.id, d.datetime, d.datetime_local, d.ms, d.wim, d.opposite_direction,
    d.wim_detector_id, d.detection_id, d.detection_image_id, d.detection_image_id_back,
    d.time_distance_front_front, d.time_distance_back_front,
    d.vehicle_class, d.vehicle_class_orig, d.velocity, d.length,
    d.lp, d.lp_back, d.insert_timestamp, d.total_weight, d.axles_count, d.front_overhang,
    d.axles_1_2_base, d.axles_2_3_base, d.axles_3_4_base, d.axles_4_5_base,
    d.axles_5_6_base, d.axles_6_7_base, d.axles_7_8_base, d.axles_8_9_base,
    d.axles_9_10_base, d.axles_10_11_base, d.axles_11_12_base, d.axles_12_13_base,
    d.axles_13_14_base, d.axles_14_15_base, d.axles_15_16_base, d.axles_16_17_base,
    d.axles_17_18_base,
    d.axle_1_leftside_load, d.axle_1_rightside_load,
    d.axle_2_leftside_load, d.axle_2_rightside_load,
    d.axle_3_leftside_load, d.axle_3_rightside_load,
    d.axle_4_leftside_load, d.axle_4_rightside_load,
    d.axle_5_leftside_load, d.axle_5_rightside_load,
    d.axle_6_leftside_load, d.axle_6_rightside_load,
    d.axle_7_leftside_load, d.axle_7_rightside_load,
    d.axle_8_leftside_load, d.axle_8_rightside_load,
    d.axle_9_leftside_load, d.axle_9_rightside_load,
    d.axle_10_leftside_load, d.axle_10_rightside_load,
    d.axle_11_leftside_load, d.axle_11_rightside_load,
    d.axle_12_leftside_load, d.axle_12_rightside_load,
    d.axle_13_leftside_load, d.axle_13_rightside_load,
    d.axle_14_leftside_load, d.axle_14_rightside_load,
    d.axle_15_leftside_load, d.axle_15_rightside_load,
    d.axle_16_leftside_load, d.axle_16_rightside_load,
    d.axle_17_leftside_load, d.axle_17_rightside_load,
    d.axle_18_leftside_load, d.axle_18_rightside_load,
    d.overweighting, d.match_datetime, d.loop_class,
    d.l_pos, d.r_pos, d.dual_tire,
    d.validity, d.validity_flags, d.velocity_validity, d.s_dimensions_validity, d.error_flags,
    d.temp_internal, d.temp_asphalt, d.acceleration,
    d.ax_accu_1, d.ax_accu_2, d.ax_accu_3, d.ax_accu_4, d.ax_accu_5,
    d.ax_accu_6, d.ax_accu_7, d.ax_accu_8, d.ax_accu_9, d.ax_accu_10,
    d.ax_accu_11, d.ax_accu_12, d.ax_accu_13, d.ax_accu_14, d.ax_accu_15,
    d.ax_accu_16, d.ax_accu_17, d.ax_accu_18, d.sum_accu,
    d.overweight_desc, d.passage_type, d.unicam_wim_class,
    d.v_width, d.width, d.possition, d.velocity_loop,
    d.v_code, d.vc_raised, d.e_code, d.ec_raised,
    d.car_layout, d.stop_go, d.chassis_height,
    d.s_height, d.s_width, d.s_length, d.s_separated,
    d.overheight, d.oversize, d.oversize_desc,
    d.overvelocity, d.overvelocity_desc,
    d.trailer_presence, d.found_in_whitelist, d.wheel_pressure_test,
    d.verification_date, d.loop_frq_max
FROM wims_detections d
WHERE d.id > @fromId
ORDER BY d.id
LIMIT @batchSize;
";

    private static CAMEAWimMysqlRecord Map(MySqlDataReader r)
    {
        return new CAMEAWimMysqlRecord
        {
            Id = GetUInt64(r, "id"),
            DateTime = GetDateTime(r, "datetime"),
            DateTimeLocal = GetDateTime(r, "datetime_local"),
            Ms = GetUInt16(r, "ms"),
            Wim = GetString(r, "wim"),
            OppositeDirection = GetBool01(r, "opposite_direction"),
            WimDetectorId = GetSByte(r, "wim_detector_id"),
            DetectionId = GetNullableString(r, "detection_id"),
            DetectionImageId = GetNullableString(r, "detection_image_id"),
            DetectionImageIdBack = GetNullableString(r, "detection_image_id_back"),
            TimeDistanceFrontFront = GetInt32(r, "time_distance_front_front"),
            TimeDistanceBackFront = GetInt32(r, "time_distance_back_front"),
            VehicleClass = GetInt16(r, "vehicle_class"),
            VehicleClassOrig = GetInt16(r, "vehicle_class_orig"),
            Velocity = GetInt16(r, "velocity"),
            Length = GetInt16(r, "length"),
            Lp = GetNullableString(r, "lp"),
            LpBack = GetNullableString(r, "lp_back"),
            InsertTimestamp = GetDateTime(r, "insert_timestamp"),
            TotalWeight = GetNullableInt32(r, "total_weight"),
            AxlesCount = GetSByte(r, "axles_count"),
            FrontOverhang = GetNullableInt16(r, "front_overhang"),
            Axles12Base = GetNullableInt16(r, "axles_1_2_base"),
            Axles23Base = GetNullableInt16(r, "axles_2_3_base"),
            Axles34Base = GetNullableInt16(r, "axles_3_4_base"),
            Axles45Base = GetNullableInt16(r, "axles_4_5_base"),
            Axles56Base = GetNullableInt16(r, "axles_5_6_base"),
            Axles67Base = GetNullableInt16(r, "axles_6_7_base"),
            Axles78Base = GetNullableInt16(r, "axles_7_8_base"),
            Axles89Base = GetNullableInt16(r, "axles_8_9_base"),
            Axles910Base = GetNullableInt16(r, "axles_9_10_base"),
            Axles1011Base = GetNullableInt16(r, "axles_10_11_base"),
            Axles1112Base = GetNullableInt16(r, "axles_11_12_base"),
            Axles1213Base = GetNullableInt16(r, "axles_12_13_base"),
            Axles1314Base = GetNullableInt16(r, "axles_13_14_base"),
            Axles1415Base = GetNullableInt16(r, "axles_14_15_base"),
            Axles1516Base = GetNullableInt16(r, "axles_15_16_base"),
            Axles1617Base = GetNullableInt16(r, "axles_16_17_base"),
            Axles1718Base = GetNullableInt16(r, "axles_17_18_base"),
            Axle1LeftsideLoad = GetNullableInt16(r, "axle_1_leftside_load"),
            Axle1RightsideLoad = GetNullableInt16(r, "axle_1_rightside_load"),
            Axle2LeftsideLoad = GetNullableInt16(r, "axle_2_leftside_load"),
            Axle2RightsideLoad = GetNullableInt16(r, "axle_2_rightside_load"),
            Axle3LeftsideLoad = GetNullableInt16(r, "axle_3_leftside_load"),
            Axle3RightsideLoad = GetNullableInt16(r, "axle_3_rightside_load"),
            Axle4LeftsideLoad = GetNullableInt16(r, "axle_4_leftside_load"),
            Axle4RightsideLoad = GetNullableInt16(r, "axle_4_rightside_load"),
            Axle5LeftsideLoad = GetNullableInt16(r, "axle_5_leftside_load"),
            Axle5RightsideLoad = GetNullableInt16(r, "axle_5_rightside_load"),
            Axle6LeftsideLoad = GetNullableInt16(r, "axle_6_leftside_load"),
            Axle6RightsideLoad = GetNullableInt16(r, "axle_6_rightside_load"),
            Axle7LeftsideLoad = GetNullableInt16(r, "axle_7_leftside_load"),
            Axle7RightsideLoad = GetNullableInt16(r, "axle_7_rightside_load"),
            Axle8LeftsideLoad = GetNullableInt16(r, "axle_8_leftside_load"),
            Axle8RightsideLoad = GetNullableInt16(r, "axle_8_rightside_load"),
            Axle9LeftsideLoad = GetNullableInt16(r, "axle_9_leftside_load"),
            Axle9RightsideLoad = GetNullableInt16(r, "axle_9_rightside_load"),
            Axle10LeftsideLoad = GetNullableInt16(r, "axle_10_leftside_load"),
            Axle10RightsideLoad = GetNullableInt16(r, "axle_10_rightside_load"),
            Axle11LeftsideLoad = GetNullableInt16(r, "axle_11_leftside_load"),
            Axle11RightsideLoad = GetNullableInt16(r, "axle_11_rightside_load"),
            Axle12LeftsideLoad = GetNullableInt16(r, "axle_12_leftside_load"),
            Axle12RightsideLoad = GetNullableInt16(r, "axle_12_rightside_load"),
            Axle13LeftsideLoad = GetNullableInt16(r, "axle_13_leftside_load"),
            Axle13RightsideLoad = GetNullableInt16(r, "axle_13_rightside_load"),
            Axle14LeftsideLoad = GetNullableInt16(r, "axle_14_leftside_load"),
            Axle14RightsideLoad = GetNullableInt16(r, "axle_14_rightside_load"),
            Axle15LeftsideLoad = GetNullableInt16(r, "axle_15_leftside_load"),
            Axle15RightsideLoad = GetNullableInt16(r, "axle_15_rightside_load"),
            Axle16LeftsideLoad = GetNullableInt16(r, "axle_16_leftside_load"),
            Axle16RightsideLoad = GetNullableInt16(r, "axle_16_rightside_load"),
            Axle17LeftsideLoad = GetNullableInt16(r, "axle_17_leftside_load"),
            Axle17RightsideLoad = GetNullableInt16(r, "axle_17_rightside_load"),
            Axle18LeftsideLoad = GetNullableInt16(r, "axle_18_leftside_load"),
            Axle18RightsideLoad = GetNullableInt16(r, "axle_18_rightside_load"),
            Overweighting = GetString(r, "overweighting"),
            MatchDateTime = GetDateTime(r, "match_datetime"),
            LoopClass = GetSByte(r, "loop_class"),
            LPos = GetNullableInt16(r, "l_pos"),
            RPos = GetNullableInt16(r, "r_pos"),
            DualTire = GetNullableString(r, "dual_tire"),
            Validity = GetNullableSByte(r, "validity"),
            ValidityFlags = GetNullableString(r, "validity_flags"),
            VelocityValidity = GetNullableSByte(r, "velocity_validity"),
            SDimensionsValidity = GetNullableSByte(r, "s_dimensions_validity"),
            ErrorFlags = GetNullableString(r, "error_flags"),
            TempInternal = GetNullableInt16(r, "temp_internal"),
            TempAsphalt = GetNullableInt16(r, "temp_asphalt"),
            Acceleration = GetNullableInt32(r, "acceleration"),
            AxAccu1 = GetNullableUInt16(r, "ax_accu_1"),
            AxAccu2 = GetNullableUInt16(r, "ax_accu_2"),
            AxAccu3 = GetNullableUInt16(r, "ax_accu_3"),
            AxAccu4 = GetNullableUInt16(r, "ax_accu_4"),
            AxAccu5 = GetNullableUInt16(r, "ax_accu_5"),
            AxAccu6 = GetNullableUInt16(r, "ax_accu_6"),
            AxAccu7 = GetNullableUInt16(r, "ax_accu_7"),
            AxAccu8 = GetNullableUInt16(r, "ax_accu_8"),
            AxAccu9 = GetNullableUInt16(r, "ax_accu_9"),
            AxAccu10 = GetNullableUInt16(r, "ax_accu_10"),
            AxAccu11 = GetNullableUInt16(r, "ax_accu_11"),
            AxAccu12 = GetNullableUInt16(r, "ax_accu_12"),
            AxAccu13 = GetNullableUInt16(r, "ax_accu_13"),
            AxAccu14 = GetNullableUInt16(r, "ax_accu_14"),
            AxAccu15 = GetNullableUInt16(r, "ax_accu_15"),
            AxAccu16 = GetNullableUInt16(r, "ax_accu_16"),
            AxAccu17 = GetNullableUInt16(r, "ax_accu_17"),
            AxAccu18 = GetNullableUInt16(r, "ax_accu_18"),
            SumAccu = GetNullableInt32(r, "sum_accu"),
            OverweightDesc = GetNullableString(r, "overweight_desc"),
            PassageType = GetNullableByte(r, "passage_type"),
            UnicamWimClass = GetNullableString(r, "unicam_wim_class"),
            VWidth = GetNullableInt16(r, "v_width"),
            Width = GetNullableInt16(r, "width"),
            Possition = GetNullableInt16(r, "possition"),
            VelocityLoop = GetNullableInt16(r, "velocity_loop"),
            VCode = GetNullableString(r, "v_code"),
            VcRaised = GetNullableByte(r, "vc_raised"),
            ECode = GetNullableString(r, "e_code"),
            EcRaised = GetNullableByte(r, "ec_raised"),
            CarLayout = GetNullableString(r, "car_layout"),
            StopGo = GetNullableSByte(r, "stop_go"),
            ChassisHeight = GetNullableUInt16(r, "chassis_height"),
            SHeight = GetNullableInt16(r, "s_height"),
            SWidth = GetNullableInt16(r, "s_width"),
            SLength = GetNullableInt32(r, "s_length"),
            SSeparated = GetNullableSByte(r, "s_separated"),
            Overheight = GetNullableSByte(r, "overheight"),
            Oversize = GetNullableString(r, "oversize"),
            OversizeDesc = GetNullableString(r, "oversize_desc"),
            Overvelocity = GetNullableString(r, "overvelocity"),
            OvervelocityDesc = GetNullableString(r, "overvelocity_desc"),
            TrailerPresence = GetNullableSByte(r, "trailer_presence"),
            FoundInWhitelist = GetNullableSByte(r, "found_in_whitelist"),
            WheelPressureTest = GetNullableString(r, "wheel_pressure_test"),
            VerificationDate = GetNullableDateTime(r, "verification_date"),
            LoopFrqMax = GetNullableInt32(r, "loop_frq_max"),
        };
    }

    private static int Ord(MySqlDataReader r, string name) => r.GetOrdinal(name);
    private static bool IsNull(MySqlDataReader r, string name) => r.IsDBNull(Ord(r, name));
    private static string GetString(MySqlDataReader r, string name) => r.GetString(Ord(r, name));
    private static string? GetNullableString(MySqlDataReader r, string name) =>
        IsNull(r, name) ? null : r.GetString(Ord(r, name));
    private static DateTime GetDateTime(MySqlDataReader r, string name) => r.GetDateTime(Ord(r, name));
    private static DateTime? GetNullableDateTime(MySqlDataReader r, string name) =>
        IsNull(r, name) ? null : r.GetDateTime(Ord(r, name));
    private static ulong GetUInt64(MySqlDataReader r, string name) =>
        Convert.ToUInt64(r.GetValue(Ord(r, name)));
    private static ushort GetUInt16(MySqlDataReader r, string name) =>
        Convert.ToUInt16(r.GetValue(Ord(r, name)));
    private static short GetInt16(MySqlDataReader r, string name) =>
        Convert.ToInt16(r.GetValue(Ord(r, name)));
    private static int GetInt32(MySqlDataReader r, string name) =>
        Convert.ToInt32(r.GetValue(Ord(r, name)));
    private static sbyte GetSByte(MySqlDataReader r, string name) =>
        Convert.ToSByte(r.GetValue(Ord(r, name)));
    private static byte? GetNullableByte(MySqlDataReader r, string name) =>
        IsNull(r, name) ? null : Convert.ToByte(r.GetValue(Ord(r, name)));
    private static short? GetNullableInt16(MySqlDataReader r, string name) =>
        IsNull(r, name) ? null : Convert.ToInt16(r.GetValue(Ord(r, name)));
    private static int? GetNullableInt32(MySqlDataReader r, string name) =>
        IsNull(r, name) ? null : Convert.ToInt32(r.GetValue(Ord(r, name)));
    private static sbyte? GetNullableSByte(MySqlDataReader r, string name) =>
        IsNull(r, name) ? null : Convert.ToSByte(r.GetValue(Ord(r, name)));
    private static ushort? GetNullableUInt16(MySqlDataReader r, string name) =>
        IsNull(r, name) ? null : Convert.ToUInt16(r.GetValue(Ord(r, name)));
    private static bool GetBool01(MySqlDataReader r, string name) =>
        Convert.ToInt32(r.GetValue(Ord(r, name))) != 0;
}
