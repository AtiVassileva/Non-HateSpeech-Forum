using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.ML;
using Microsoft.ML.Data;

namespace ProfanityDetectionTrainer
{
    public class ModelInput
    {
        public string Text { get; set; }
    }
    public class ModelOutput
    {
        [ColumnName("PredictedLabel")]
        public bool IsProfane { get; set; }

        public float Score { get; set; }
    }

    class Program
    {
        private static PredictionEngine<ModelInput, ModelOutput> _predictionEngine;

        static void Main(string[] args)
        {
            // Create MLContext
            MLContext mlContext = new MLContext();

            // Load data
            string dataPath = "C:\\Users\\atanasia.vasileva\\OneDrive - Scale Focus AD\\Desktop\\Non-HateSpeech-Forum\\NonHateSpeechForum\\ProfanityDetectionTrainer\\UncensoredWords.tsv";
            IDataView data = mlContext.Data.LoadFromTextFile<ProfanityData>(dataPath, separatorChar: '\t', hasHeader: true);

            var dataEnumerable = mlContext.Data.CreateEnumerable<ProfanityData>(data, reuseRowObject: false).ToList();

            // Check if there are any positive instances in the data
            var positiveCount = dataEnumerable.Count(d => !d.IsProfane);
            var negativeCount = dataEnumerable.Count(d => d.IsProfane);
            if (positiveCount == 0 || negativeCount == 0)
            {
                Console.WriteLine("Training and evaluation cannot be performed because there are no positive or negative instances in the data.");
                return;
            }

            Console.WriteLine($"Positive instances: {positiveCount}, Negative instances: {negativeCount}");

            // Define the pipeline
            var pipeline = mlContext.Transforms.Text.FeaturizeText("Features", nameof(ProfanityData.Text))
                .Append(mlContext.BinaryClassification.Trainers.SdcaLogisticRegression(labelColumnName: nameof(ProfanityData.IsProfane), featureColumnName: "Features"));

            // Split data into training and testing sets
            var trainTestData = mlContext.Data.TrainTestSplit(data, testFraction: 0.2);
            var trainData = trainTestData.TrainSet;
            var testData = trainTestData.TestSet;

            // Train the model
            var model = pipeline.Fit(trainData);

            // Evaluate the model
            EvaluateModel(mlContext, testData, model);

            // Save the model
            string modelPath = "C:\\Users\\atanasia.vasileva\\OneDrive - Scale Focus AD\\Desktop\\Non-HateSpeech-Forum\\NonHateSpeechForum\\profanity_detection_model.zip";
            mlContext.Model.Save(model, trainData.Schema, modelPath);

            Console.WriteLine("Model training completed. Model saved to profanity_detection_model.zip.");

            // Load the model
            ITransformer trainedModel;
            using (var stream = new FileStream(modelPath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                trainedModel = mlContext.Model.Load(stream, out _);
            }

            // Create prediction engine
            _predictionEngine = mlContext.Model.CreatePredictionEngine<ModelInput, ModelOutput>(trainedModel);

            // Test prediction
            TestPrediction("You are a great person!", false);  // Expected: not profane
            TestPrediction("faggot", true);  // Expected: profane
        }

        static void EvaluateModel(MLContext mlContext, IDataView testData, ITransformer model)
        {
            var predictions = model.Transform(testData);

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

        static void TestPrediction(string content, bool expectedIsProfane)
        {
            var input = new ModelInput { Text = content };
            var prediction = _predictionEngine.Predict(input);
            Console.WriteLine($"Text: {content}");
            Console.WriteLine($"Predicted Profanity: {prediction.IsProfane}, Expected: {expectedIsProfane}");
        }
    }
}
