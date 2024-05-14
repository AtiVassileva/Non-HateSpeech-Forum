using System;
using System.Data;
using Microsoft.ML;
using Microsoft.ML.Data;

namespace ProfanityDetectionTrainer
{
    class Program
    {
        static void Main(string[] args)
        {
            // Create MLContext
            MLContext mlContext = new MLContext();

            // Load data
            IDataView data = mlContext.Data.
                LoadFromTextFile<ProfanityData>
                ("C:\\Users\\Петър Тодоров\\Desktop\\project first\\Non-HateSpeech-Forum\\NonHateSpeechForum\\ProfanityDetectionTrainer\\UncensoredWords.tsv",
                separatorChar: '\t',
                hasHeader: true);

            // Define the pipeline
            var pipeline = mlContext.Transforms.Text.FeaturizeText("Features", nameof(ProfanityData.Text))
                .Append(mlContext.BinaryClassification.Trainers.SdcaLogisticRegression());

            // Train the model
            var model = pipeline.Fit(data);

            // Save the model
            mlContext.Model.Save(model, data.Schema, "C:\\Users\\Петър Тодоров\\Desktop\\project first\\Non-HateSpeech-Forum\\NonHateSpeechForum\\profanity_detection_model.zip");

            Console.WriteLine("Model training completed. Model saved to profanity_detection_model.zip.");

            EvaluateModel(mlContext, data, model);
        }
        static void EvaluateModel(MLContext mlContext, IDataView data, ITransformer model)
        {
            var predictions = model.Transform(data);

            try
            {
                var metrics = mlContext.BinaryClassification.Evaluate(predictions);
                Console.WriteLine("Evaluation Metrics:");
                Console.WriteLine($"  Accuracy: {metrics.Accuracy}");
                Console.WriteLine($"  Precision: {metrics.PositivePrecision}");
                Console.WriteLine($"  Recall: {metrics.PositiveRecall}");
                Console.WriteLine($"  AUC: {metrics.AreaUnderRocCurve}");
            }
            catch (ArgumentOutOfRangeException ex)
            {
                Console.WriteLine("Evaluation Metrics:");
                Console.WriteLine("AUC cannot be computed because there are no positive instances in the data.");
                // Optionally, handle the exception further if needed
            }
        }
    }

}