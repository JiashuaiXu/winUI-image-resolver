using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;
using ExifLib;
using ImageResolver.Models;

namespace ImageResolver.Services
{
    public class ImageInfoService
    {
        public async Task<List<ImageInfo>> ProcessImagesAsync(string folderPath)
        {
            var imageInfos = new List<ImageInfo>();
            var supportedExtensions = new[] { ".jpg", ".jpeg", ".png", ".bmp", ".tiff", ".tif", ".gif" };

            try
            {
                var folder = await StorageFolder.GetFolderFromPathAsync(folderPath);
                var files = await folder.GetFilesAsync();

                foreach (var file in files.Where(f => supportedExtensions.Contains(f.FileType.ToLower())))
                {
                    try
                    {
                        var imageInfo = await ProcessImageAsync(file);
                        imageInfos.Add(imageInfo);
                    }
                    catch (Exception ex)
                    {
                        // Log error but continue processing other files
                        System.Diagnostics.Debug.WriteLine($"Error processing {file.Name}: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error accessing folder: {ex.Message}");
            }

            return imageInfos;
        }

        private async Task<ImageInfo> ProcessImageAsync(StorageFile file)
        {
            var imageInfo = new ImageInfo
            {
                FileName = file.Name,
                FilePath = file.Path,
                FileSize = (long)(await file.GetBasicPropertiesAsync()).Size
            };

            // Format file size
            imageInfo.FileSizeFormatted = FormatFileSize(imageInfo.FileSize);

            // Get image dimensions using Windows.Graphics.Imaging
            try
            {
                using (var stream = await file.OpenAsync(FileAccessMode.Read))
                {
                    var decoder = await BitmapDecoder.CreateAsync(stream);
                    imageInfo.Width = (int)decoder.PixelWidth;
                    imageInfo.Height = (int)decoder.PixelHeight;
                }
            }
            catch
            {
                // Fallback: try to read from EXIF
            }

            // Read EXIF data
            try
            {
                using (var stream = await file.OpenStreamForReadAsync())
                {
                    using (var reader = new ExifReader(stream))
                    {
                        // Camera information
                        reader.GetTagValue<string>(ExifTags.Make, out var make);
                        reader.GetTagValue<string>(ExifTags.Model, out var model);
                        imageInfo.CameraMake = make;
                        imageInfo.CameraModel = model;

                        // Date taken
                        reader.GetTagValue<DateTime>(ExifTags.DateTimeDigitized, out var dateTaken);
                        if (dateTaken != default)
                        {
                            imageInfo.DateTaken = dateTaken;
                        }
                        else
                        {
                            reader.GetTagValue<DateTime>(ExifTags.DateTime, out var dateTime);
                            if (dateTime != default)
                            {
                                imageInfo.DateTaken = dateTime;
                            }
                        }

                        // Orientation
                        reader.GetTagValue<ushort>(ExifTags.Orientation, out var orientation);
                        imageInfo.Orientation = GetOrientationString(orientation);

                        // GPS coordinates
                        reader.GetTagValue<double[]>(ExifTags.GPSLatitude, out var latArray);
                        reader.GetTagValue<double[]>(ExifTags.GPSLongitude, out var lonArray);
                        reader.GetTagValue<string>(ExifTags.GPSLatitudeRef, out var latRef);
                        reader.GetTagValue<string>(ExifTags.GPSLongitudeRef, out var lonRef);

                        if (latArray != null && latArray.Length == 3)
                        {
                            var lat = latArray[0] + latArray[1] / 60.0 + latArray[2] / 3600.0;
                            imageInfo.Latitude = latRef == "S" ? -lat : lat;
                        }

                        if (lonArray != null && lonArray.Length == 3)
                        {
                            var lon = lonArray[0] + lonArray[1] / 60.0 + lonArray[2] / 3600.0;
                            imageInfo.Longitude = lonRef == "W" ? -lon : lon;
                        }

                        // Software and artist
                        reader.GetTagValue<string>(ExifTags.Software, out var software);
                        reader.GetTagValue<string>(ExifTags.Artist, out var artist);
                        reader.GetTagValue<string>(ExifTags.Copyright, out var copyright);
                        imageInfo.Software = software;
                        imageInfo.Artist = artist;
                        imageInfo.Copyright = copyright;

                        // Camera settings
                        reader.GetTagValue<ushort>(ExifTags.ISOSpeedRatings, out var iso);
                        imageInfo.ISO = iso > 0 ? iso : null;

                        if (reader.GetTagValue<object>(ExifTags.FNumber, out var fNumberObj))
                        {
                            // Handle different possible types for FNumber
                            if (fNumberObj is double fNumberDouble)
                            {
                                imageInfo.FNumber = fNumberDouble;
                            }
                            else if (fNumberObj is float fNumberFloat)
                            {
                                imageInfo.FNumber = fNumberFloat;
                            }
                            else if (fNumberObj is ushort fNumberUShort)
                            {
                                imageInfo.FNumber = fNumberUShort;
                            }
                        }

                        if (reader.GetTagValue<object>(ExifTags.ExposureTime, out var exposureTimeObj))
                        {
                            // Handle different possible types for ExposureTime
                            if (exposureTimeObj is double exposureTimeDouble)
                            {
                                imageInfo.ExposureTime = exposureTimeDouble;
                            }
                            else if (exposureTimeObj is float exposureTimeFloat)
                            {
                                imageInfo.ExposureTime = exposureTimeFloat;
                            }
                            else if (exposureTimeObj is ushort exposureTimeUShort)
                            {
                                imageInfo.ExposureTime = exposureTimeUShort;
                            }
                        }

                        if (reader.GetTagValue<object>(ExifTags.FocalLength, out var focalLengthObj))
                        {
                            // Handle different possible types for FocalLength
                            if (focalLengthObj is double focalLengthDouble)
                            {
                                imageInfo.FocalLength = focalLengthDouble;
                            }
                            else if (focalLengthObj is float focalLengthFloat)
                            {
                                imageInfo.FocalLength = focalLengthFloat;
                            }
                            else if (focalLengthObj is ushort focalLengthUShort)
                            {
                                imageInfo.FocalLength = focalLengthUShort;
                            }
                        }
                    }
                }
            }
            catch
            {
                // EXIF data not available or corrupted
            }

            return imageInfo;
        }

        private string FormatFileSize(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB" };
            double len = bytes;
            int order = 0;
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len = len / 1024;
            }
            return $"{len:0.##} {sizes[order]}";
        }

        private string GetOrientationString(ushort orientation)
        {
            return orientation switch
            {
                1 => "Normal",
                2 => "Mirrored",
                3 => "Rotated 180°",
                4 => "Mirrored and Rotated 180°",
                5 => "Mirrored and Rotated 90° CCW",
                6 => "Rotated 90° CW",
                7 => "Mirrored and Rotated 90° CW",
                8 => "Rotated 90° CCW",
                _ => "Unknown"
            };
        }
    }
}

