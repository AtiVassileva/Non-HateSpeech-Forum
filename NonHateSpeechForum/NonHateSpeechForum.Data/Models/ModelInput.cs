using Microsoft.ML.Data;
public class ModelInput
{
    [ColumnName("Text")]
    public string? Content { get; set; }
}