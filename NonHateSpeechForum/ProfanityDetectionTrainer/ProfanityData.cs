using Microsoft.ML.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProfanityDetectionTrainer
{
    public class ProfanityData
    {
        [LoadColumn(0)]
        public string? Text { get; set; }

        [LoadColumn(1), ColumnName("Label")]
        public bool IsProfane { get; set; }
    }
}
