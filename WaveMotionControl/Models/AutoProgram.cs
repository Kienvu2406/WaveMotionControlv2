namespace WaveMotionControl.Models;

/// <summary>
/// AUTO mới: bản đồ 16x16, nhiều cụm độc lập và hiệu ứng theo lớp.
/// 1 vòng motor = 1 chu kỳ đi + về của con trượt.
/// </summary>
public enum AutoEffectType
{
    WaveFromCenter,
    WaveHeadToTail
}

public enum AutoWaveDirection
{
    LeftToRight,
    RightToLeft,
    TopToBottom,
    BottomToTop
}

public sealed record AutoGridCell(
    int Row,
    int Column,
    AxisAddress? DriverId);

public sealed record AutoWaveLayer(
    int Index,
    double DistanceSquared,
    IReadOnlyList<AxisAddress> Drivers);

public sealed record AutoCluster(
    int Id,
    int TopRow,
    int LeftColumn,
    int Width,
    int Height,
    IReadOnlyList<AutoGridCell> Cells,
    AutoEffectType Effect,
    AutoWaveDirection WaveDirection,
    double LayerOffsetRevolutions,
    double FrequencyHz)
{
    public double CenterRow => TopRow + (Height - 1) / 2.0;
    public double CenterColumn => LeftColumn + (Width - 1) / 2.0;

    public IReadOnlyList<AutoWaveLayer> BuildWaveLayers()
    {
        return Effect switch
        {
            AutoEffectType.WaveHeadToTail => BuildHeadToTailLayers(),
            _ => BuildCenterLayers()
        };
    }

    private IReadOnlyList<AutoWaveLayer> BuildCenterLayers()
    {
        // Layer dạng vòng chữ nhật đồng tâm (Chebyshev rings):
        // 5x5 => 2 2 2 2 2 / 2 1 1 1 2 / 2 1 0 1 2 / ...
        // 4x4 => 1 1 1 1 / 1 0 0 1 / 1 0 0 1 / 1 1 1 1.
        // Dùng tọa độ nhân 2 để xử lý đồng nhất cụm chẵn/lẻ mà không cần số thực.
        return Cells
            .Where(c => c.DriverId is not null)
            .Select(c =>
            {
                var localRow2 = 2 * (c.Row - TopRow) - (Height - 1);
                var localCol2 = 2 * (c.Column - LeftColumn) - (Width - 1);
                var layerIndex = Math.Max(Math.Abs(localRow2), Math.Abs(localCol2)) / 2;
                return new { Cell = c, LayerIndex = layerIndex };
            })
            .GroupBy(x => x.LayerIndex)
            .OrderBy(g => g.Key)
            .Select(g => new AutoWaveLayer(
                g.Key,
                g.Key,
                g.Select(x => x.Cell.DriverId!.Value).Distinct().ToArray()))
            .ToArray();
    }

    private IReadOnlyList<AutoWaveLayer> BuildHeadToTailLayers()
    {
        // Phương án A: cả một hàng hoặc một cột là một layer và chạy đồng thời.
        // Trái -> Phải: mỗi cột là một layer.
        // Phải -> Trái: thứ tự cột đảo lại.
        // Trên -> Dưới: mỗi hàng là một layer.
        // Dưới -> Trên: thứ tự hàng đảo lại.
        return Cells
            .Where(c => c.DriverId is not null)
            .Select(c =>
            {
                var layerIndex = WaveDirection switch
                {
                    AutoWaveDirection.RightToLeft =>
                        (LeftColumn + Width - 1) - c.Column,
                    AutoWaveDirection.TopToBottom =>
                        c.Row - TopRow,
                    AutoWaveDirection.BottomToTop =>
                        (TopRow + Height - 1) - c.Row,
                    _ =>
                        c.Column - LeftColumn
                };

                return new { Cell = c, LayerIndex = layerIndex };
            })
            .GroupBy(x => x.LayerIndex)
            .OrderBy(g => g.Key)
            .Select(g => new AutoWaveLayer(
                g.Key,
                g.Key,
                g.Select(x => x.Cell.DriverId!.Value).Distinct().ToArray()))
            .ToArray();
    }
}

public sealed record AutoProgram(
    int GridRows,
    int GridColumns,
    IReadOnlyList<AutoGridCell> GridCells,
    IReadOnlyList<AutoCluster> Clusters,
    int PulsesPerRevolution,
    double FrequencyHz,
    double LayerOffsetRevolutions,
    double RampUpSeconds,
    double RampDownSeconds)
{
    public const int GridSize = 16;

    public IReadOnlyDictionary<AxisAddress, AutoCluster> DriverClusters()
    {
        var result = new Dictionary<AxisAddress, AutoCluster>();
        foreach (var cluster in Clusters)
        {
            foreach (var cell in cluster.Cells)
            {
                if (cell.DriverId is AxisAddress driver)
                {
                    result[driver] = cluster;
                }
            }
        }

        return result;
    }
}
