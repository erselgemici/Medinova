using Medinova.DTOs.MLDtos;
using Medinova.Models;
using Microsoft.ML;
using Microsoft.ML.Transforms.TimeSeries;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Medinova.Services
{
    public class AppointmentForecaster
    {
        private readonly MedinovaContext _context;

        public AppointmentForecaster()
        {
            _context = new MedinovaContext();
        }

        public List<DailyAppointmentDataDto> PredictNext3Months()
        {
            var rawData = _context.Appointments
                .Where(x => x.AppointmentDate != null)
                .Select(x => x.AppointmentDate.Value) // Sadece tarihleri çek
                .ToList();

            if (!rawData.Any()) throw new Exception("Veritabanında hiç randevu verisi yok. Lütfen SQL Scriptini çalıştırın.");

            // Saatleri sıfırlayarak (00:00:00) gün bazlı grupla
            var groupedData = rawData
                .GroupBy(d => d.Date)
                .Select(g => new { Date = g.Key, Count = g.Count() })
                .OrderBy(x => x.Date)
                .ToList();

            DateTime startDate = groupedData.First().Date;
            DateTime endDate = groupedData.Last().Date;

            var mlDataList = new List<DailyAppointmentDataDto>();

            // Boşlukları (Randevu olmayan günleri) 0 ile doldur
            for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
            {
                var existing = groupedData.FirstOrDefault(x => x.Date == date);
                mlDataList.Add(new DailyAppointmentDataDto
                {
                    Date = date,
                    Count = existing != null ? (float)existing.Count : 0f
                });
            }

            int dataCount = mlDataList.Count;
            int windowSize = 7;
            int seriesLength = 30;

            if (dataCount < 40) // Veri çok azsa parametreleri küçült
            {
                windowSize = 2;
                seriesLength = 5;
            }

            // MODEL KURULUMU
            var mlContext = new MLContext();
            var dataView = mlContext.Data.LoadFromEnumerable(mlDataList);

            var pipeline = mlContext.Forecasting.ForecastBySsa(
                outputColumnName: nameof(AppointmentPredictionDto.ForecastedCounts),
                inputColumnName: nameof(DailyAppointmentDataDto.Count),
                windowSize: windowSize,
                seriesLength: seriesLength,
                trainSize: dataCount,
                horizon: 90,
                confidenceLevel: 0.95f,
                confidenceLowerBoundColumn: nameof(AppointmentPredictionDto.LowerBoundCounts),
                confidenceUpperBoundColumn: nameof(AppointmentPredictionDto.UpperBoundCounts)
            );

            // EĞİTİM & TAHMİN
            var model = pipeline.Fit(dataView);
            var forecastingEngine = model.CreateTimeSeriesEngine<DailyAppointmentDataDto, AppointmentPredictionDto>(mlContext);
            var forecast = forecastingEngine.Predict();

            // SONUÇLARI DÖK
            var resultList = new List<DailyAppointmentDataDto>();
            var lastDate = mlDataList.Last().Date;

            for (int i = 0; i < 90; i++)
            {
                float prediction = Math.Max(0, forecast.ForecastedCounts[i]);
                resultList.Add(new DailyAppointmentDataDto
                {
                    Date = lastDate.AddDays(i + 1),
                    Count = (float)Math.Round(prediction)
                });
            }

            return resultList;
        }
    }
}
