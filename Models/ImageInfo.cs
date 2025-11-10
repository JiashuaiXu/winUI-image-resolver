using System;

namespace ImageResolver.Models
{
    public class ImageInfo
    {
        public string FileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public int Width { get; set; }
        public int Height { get; set; }
        public string Resolution => $"{Width}x{Height}";
        public long FileSize { get; set; }
        public string FileSizeFormatted { get; set; } = string.Empty;
        
        // EXIF Information
        public string? CameraMake { get; set; }
        public string? CameraModel { get; set; }
        public DateTime? DateTaken { get; set; }
        public string? Orientation { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string? Software { get; set; }
        public string? Artist { get; set; }
        public string? Copyright { get; set; }
        public int? ISO { get; set; }
        public double? FNumber { get; set; }
        public double? ExposureTime { get; set; }
        public double? FocalLength { get; set; }
    }
}

