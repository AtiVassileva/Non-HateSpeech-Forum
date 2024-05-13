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
            IDataView data = mlContext.Data.LoadFromTextFile<ProfanityData>("C:\\Users\\Петър Тодоров\\Desktop\\project first\\Non-HateSpeech-Forum\\NonHateSpeechForum\\ProfanityDetectionTrainer\\UncensoredWords.tsv", separatorChar: '\t');

            // Define the pipeline
            var pipeline = mlContext.Transforms.Text.FeaturizeText("Features", nameof(ProfanityData.Text))
                .Append(mlContext.BinaryClassification.Trainers.SdcaLogisticRegression());

            // Train the model
            var model = pipeline.Fit(data);

            // Save the model
            mlContext.Model.Save(model, data.Schema, "profanity_detection_model.zip");

            Console.WriteLine("Model training completed. Model saved to profanity_detection_model.zip.");
        }
    }

}