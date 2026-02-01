using Microsoft.ML.Data;

namespace Medinova.DTOs.MLDtos
{
    public class AppointmentPredictionDto
    {
        [VectorType(90)] // 90 günlük tahmin için
        public float[] ForecastedCounts { get; set; }

        public float[] LowerBoundCounts { get; set; }
        public float[] UpperBoundCounts { get; set; }
    }
}
