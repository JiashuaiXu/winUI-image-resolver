using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using ImageResolver.Models;
using Windows.Storage;

namespace ImageResolver.Services
{
    public class CsvExportService
    {
        public async Task<string> ExportToCsvAsync(List<ImageInfo> imageInfos, string outputPath)
        {
            // Use the output path directly
            var fullPath = outputPath;
            if (!Path.IsPathRooted(fullPath))
            {
                fullPath = Path.GetFullPath(fullPath);
            }

            using (var writer = new StreamWriter(fullPath))
            using (var csv = new CsvWriter(writer, new CsvConfiguration(System.Globalization.CultureInfo.InvariantCulture)))
            {
                // Write header
                csv.WriteField("FileName");
                csv.WriteField("FilePath");
                csv.WriteField("Width");
                csv.WriteField("Height");
                csv.WriteField("Resolution");
                csv.WriteField("FileSize (bytes)");
                csv.WriteField("FileSize (Formatted)");
                csv.WriteField("Camera Make");
                csv.WriteField("Camera Model");
                csv.WriteField("Date Taken");
                csv.WriteField("Orientation");
                csv.WriteField("Latitude");
                csv.WriteField("Longitude");
                csv.WriteField("Software");
                csv.WriteField("Artist");
                csv.WriteField("Copyright");
                csv.WriteField("ISO");
                csv.WriteField("F-Number");
                csv.WriteField("Exposure Time");
                csv.WriteField("Focal Length");
                csv.NextRecord();

                // Write data
                foreach (var info in imageInfos)
                {
                    csv.WriteField(info.FileName);
                    csv.WriteField(info.FilePath);
                    csv.WriteField(info.Width);
                    csv.WriteField(info.Height);
                    csv.WriteField(info.Resolution);
                    csv.WriteField(info.FileSize);
                    csv.WriteField(info.FileSizeFormatted);
                    csv.WriteField(info.CameraMake ?? "");
                    csv.WriteField(info.CameraModel ?? "");
                    csv.WriteField(info.DateTaken?.ToString("yyyy-MM-dd HH:mm:ss") ?? "");
                    csv.WriteField(info.Orientation ?? "");
                    csv.WriteField(info.Latitude?.ToString() ?? "");
                    csv.WriteField(info.Longitude?.ToString() ?? "");
                    csv.WriteField(info.Software ?? "");
                    csv.WriteField(info.Artist ?? "");
                    csv.WriteField(info.Copyright ?? "");
                    csv.WriteField(info.ISO?.ToString() ?? "");
                    csv.WriteField(info.FNumber?.ToString() ?? "");
                    csv.WriteField(info.ExposureTime?.ToString() ?? "");
                    csv.WriteField(info.FocalLength?.ToString() ?? "");
                    csv.NextRecord();
                }
            }

            return fullPath;
        }
    }
}

