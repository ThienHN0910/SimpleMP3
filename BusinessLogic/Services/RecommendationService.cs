using Microsoft.ML;
using Microsoft.ML.Data;
using Models;
using Repos.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.ML.Trainers;

namespace BusinessLogic.Services
{
    public class RecommendationService
    {
        private readonly ITrainingRepo _trainingRepo;
        private readonly MLContext _mlContext;
        private ITransformer _model;
        private PredictionEngine<PlayHistoryEntry, TrackPrediction> _predictionEngine;

        public RecommendationService(ITrainingRepo trainingRepo)
        {
            _trainingRepo = trainingRepo;
            _mlContext = new MLContext();
        }

        public void Train()
        {
            var trainingData = _trainingRepo.LoadTrainingDataFromEf();
            var dataView = _mlContext.Data.LoadFromEnumerable(trainingData);

            var pipeline = _mlContext.Transforms.Conversion.MapValueToKey("userIdEncoded", nameof(PlayHistoryEntry.UserId))
                .Append(_mlContext.Transforms.Conversion.MapValueToKey("trackIdEncoded", nameof(PlayHistoryEntry.TrackId)))
                .Append(_mlContext.Recommendation().Trainers.MatrixFactorization(
                    labelColumnName: nameof(PlayHistoryEntry.Label),
                    matrixColumnIndexColumnName: "userIdEncoded",
                    matrixRowIndexColumnName: "trackIdEncoded"));

            _model = pipeline.Fit(dataView);
            _predictionEngine = _mlContext.Model.CreatePredictionEngine<PlayHistoryEntry, TrackPrediction>(_model);
        }

        public float PredictScore(uint userId, uint trackId)
        {
            if (_predictionEngine == null)
                throw new InvalidOperationException("Model chưa được train!");

            var prediction = _predictionEngine.Predict(new PlayHistoryEntry
            {
                UserId = userId,
                TrackId = trackId
            });

            return prediction.Score;
        }

        public List<uint> RecommendTopTracks(uint userId, List<uint> candidateTrackIds, int top = 5)
        {
            if (_predictionEngine == null)
                throw new InvalidOperationException("Model chưa được train!");

            return candidateTrackIds
                .Select(trackId => new
                {
                    TrackId = trackId,
                    Score = PredictScore(userId, trackId)
                })
                .OrderByDescending(x => x.Score)
                .Take(top)
                .Select(x => x.TrackId)
                .ToList();
        }

        public void SaveModel(string path)
        {
            if (_model == null) return;

            var trainingData = _mlContext.Data.LoadFromEnumerable(_trainingRepo.LoadTrainingDataFromEf());
            _mlContext.Model.Save(_model, trainingData.Schema, path);
        }

        public void LoadModel(string path)
        {
            if (!File.Exists(path)) throw new FileNotFoundException("Không tìm thấy model!");

            DataViewSchema modelSchema;
            _model = _mlContext.Model.Load(path, out modelSchema);
            _predictionEngine = _mlContext.Model.CreatePredictionEngine<PlayHistoryEntry, TrackPrediction>(_model);
        }
    }
}
