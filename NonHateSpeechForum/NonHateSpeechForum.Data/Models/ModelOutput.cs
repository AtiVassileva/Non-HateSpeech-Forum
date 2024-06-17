using Microsoft.ML.Data;

public class ModelOutput
{
    [ColumnName("PredictedLabel")]
    public bool IsProfane { get; set; }

    public float Score { get; set; }
}
