﻿using CSharpImageLibrary.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using UsefulThings;
using System.Runtime.InteropServices;
using System.ComponentModel;
using CSharpImageLibrary.Headers;
using CSharpImageLibrary.DDS;
using System.Collections.Concurrent;
using System.Threading.Tasks.Dataflow;

namespace CSharpImageLibrary
{
    /// <summary>
    /// Determines how alpha is handled.
    /// </summary>
    public enum AlphaSettings
    {
        /// <summary>
        /// Keeps any existing alpha.
        /// </summary>
        KeepAlpha,

        /// <summary>
        /// Premultiplies RBG and Alpha channels. Alpha remains.
        /// </summary>
        Premultiply,

        /// <summary>
        /// Removes alpha channel.
        /// </summary>
        RemoveAlphaChannel,
    }

    /// <summary>
    /// Determines how Mipmaps are handled.
    /// </summary>
    public enum MipHandling
    {
        /// <summary>
        /// If mips are present, they are used, otherwise regenerated.
        /// </summary>
        [Description("If mips are present, they are used, otherwise regenerated.")]
        Default,

        /// <summary>
        /// Keeps existing mips if existing. Doesn't generate new ones either way.
        /// </summary>
        [Description("Keeps existing mips if existing. Doesn't generate new ones either way.")]
        KeepExisting,

        /// <summary>
        /// Removes old mips and generates new ones.
        /// </summary>
        [Description("Removes old mips and generates new ones.")]
        GenerateNew,

        /// <summary>
        /// Removes all but the top mip. Used for single mip formats.
        /// </summary>
        [Description("Removes all but the top mip. Used for single mip formats.")]
        KeepTopOnly
    }

    /// <summary>
    /// Provides main image functions
    /// </summary>
    public static class ImageEngine
    {
         /// <summary>
        /// True = Windows WIC Codecs are present (8+)
        /// </summary>
        public static bool WindowsWICCodecsAvailable { get; set; }

        /// <summary>
        /// Enables threading of Loading and Saving operations to improve performance.
        /// </summary>
        public static bool EnableThreading { get; set; } = true;

        /// <summary>
        /// CURRENTLY DISABLED. Didn't work :(
        /// Enables GPU Accelerated encoding and decoding of all formats.
        /// NOTE: WIC formats (jpg, bmp, png etc) probably already use GPU, but are not covered by this flag.
        /// </summary>
        public static bool EnableGPUAcceleration { get; set; } = false;

        /// <summary>
        /// Determines how many threads to use. -1 is infinite.
        /// </summary>
        public static int NumThreads { get; set; } = -1;

        /// <summary>
        /// Constructor. Checks WIC status before any other operation.
        /// </summary>
        static ImageEngine()
        {
            WindowsWICCodecsAvailable = WIC_Codecs.WindowsCodecsPresent();

            // Enable GPU Acceleration by default
            /*if (GPU.IsGPUAvailable)
                EnableGPUAcceleration = false;*/
        }

        internal static List<MipMap> LoadImage(Stream imageStream, AbstractHeader header, int maxDimension, double scale)
        {
            imageStream.Seek(0, SeekOrigin.Begin);
            List<MipMap> MipMaps = null;

            int decodeWidth = header.Width > header.Height ? maxDimension : 0;
            int decodeHeight = header.Width < header.Height ? maxDimension : 0;

            switch (header.Format)
            {
                case ImageEngineFormat.DDS_DXT1:
                case ImageEngineFormat.DDS_DXT2:
                case ImageEngineFormat.DDS_DXT3:
                case ImageEngineFormat.DDS_DXT4:
                case ImageEngineFormat.DDS_DXT5:
                    MipMaps = WIC_Codecs.LoadWithCodecs(imageStream, decodeWidth, decodeHeight, scale, true);
                    if (MipMaps == null)
                    {
                        // Windows codecs unavailable/failed. Load with mine.
                        MipMaps = DDSGeneral.LoadDDS((MemoryStream)imageStream, (DDS_Header)header, maxDimension);
                    }
                    break;
                case ImageEngineFormat.DDS_G8_L8:
                case ImageEngineFormat.DDS_ARGB_4:
                case ImageEngineFormat.DDS_RGB_8:
                case ImageEngineFormat.DDS_V8U8:
                case ImageEngineFormat.DDS_A8L8:
                case ImageEngineFormat.DDS_ARGB_8:
                case ImageEngineFormat.DDS_ARGB_32F:
                case ImageEngineFormat.DDS_ABGR_8:
                case ImageEngineFormat.DDS_G16_R16:
                case ImageEngineFormat.DDS_R5G6B5:
                case ImageEngineFormat.DDS_ATI1:
                case ImageEngineFormat.DDS_ATI2_3Dc:
                case ImageEngineFormat.DDS_CUSTOM:
                    MipMaps = DDSGeneral.LoadDDS((MemoryStream)imageStream, (DDS_Header)header, maxDimension);
                    break;
                case ImageEngineFormat.GIF:
                case ImageEngineFormat.JPG:
                case ImageEngineFormat.PNG:
                case ImageEngineFormat.BMP:
                case ImageEngineFormat.TIF:
                    MipMaps = WIC_Codecs.LoadWithCodecs(imageStream, decodeWidth, decodeHeight, scale, false);
                    break;
                case ImageEngineFormat.TGA:
                    using (var tga = new TargaImage(imageStream, ((TGA_Header)header).header))
                        //MipMaps = new List<MipMap>() { new MipMap(tga.ImageData, tga.Header.Width, tga.Header.Height, 1) }; 
                    break;
                case ImageEngineFormat.DDS_DX10:
                    throw new FormatException("DX10/DXGI not supported properly yet.");
                default:
                    throw new FormatException($"Format unknown: {header.Format}.");
            }

            return MipMaps;
        }

        internal static AbstractHeader LoadHeader(Stream stream)
        {
            stream.Seek(0, SeekOrigin.Begin);

            // Determine type of image
            ImageFormats.SupportedExtensions ext = ImageFormats.DetermineImageType(stream);

            // Parse header
            AbstractHeader header = null;
            switch (ext)
            {
                case ImageFormats.SupportedExtensions.BMP:
                    header = new BMP_Header(stream);
                    break;
                case ImageFormats.SupportedExtensions.DDS:
                    header = new DDS_Header(stream);
                    break;
                case ImageFormats.SupportedExtensions.JPG:
                    header = new JPG_Header(stream);
                    break;
                case ImageFormats.SupportedExtensions.PNG:
                    header = new PNG_Header(stream);
                    break;
                case ImageFormats.SupportedExtensions.TGA:
                    header = new TGA_Header(stream);
                    break;
                case ImageFormats.SupportedExtensions.GIF:
                    header = new GIF_Header(stream);
                    break;
                case ImageFormats.SupportedExtensions.TIF:
                    header = new TIFF_Header(stream);
                    break;
                default:
                    throw new NotSupportedException("Image type unknown.");
            }
            return header;
        }

        internal static void SplitChannels(MipMap mip, string savePath)
        {
            char[] channels = new char[] { 'B', 'G', 'R', 'A' };
            for (int i = 0; i < 4; i++)
            {
                // Extract channel into grayscale image
                /*var grayChannel = BuildGrayscaleFromChannel(mip.Pixels, i);

                // Save channel
                var img = UsefulThings.WPF.Images.CreateWriteableBitmap(grayChannel, mip.Width, mip.Height, PixelFormats.Gray8);
                PngBitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(img));
                byte[] bytes = null;
                using (MemoryStream ms = new MemoryStream(grayChannel.Length))
                {
                    encoder.Save(ms);
                    bytes = ms.ToArray();
                }

                if (bytes == null)
                    throw new InvalidDataException("Failed to save channel. Reason unknown.");

                string tempPath = Path.GetFileNameWithoutExtension(savePath) + "_" + channels[i] + ".png";
                string channelPath = Path.Combine(Path.GetDirectoryName(savePath), UsefulThings.General.FindValidNewFileName(tempPath));
                File.WriteAllBytes(channelPath, bytes);*/
            }
        }

        static byte[] BuildGrayscaleFromChannel(byte[] pixels, int channel)
        {
            byte[] destination = new byte[pixels.Length / 4];
            for (int i = channel, count = 0; i < pixels.Length; i+=4, count++)
                destination[count] = pixels[i];
            

            return destination;
        }


        /// <summary>
        /// Save mipmaps as given format to stream.
        /// </summary>
        /// <param name="MipMaps">List of Mips to save.</param>
        /// <param name="format">Desired format.</param>
        /// <param name="mipChoice">Determines how to handle mipmaps.</param>
        /// <param name="maxDimension">Maximum value for either image dimension.</param>
        /// <param name="alphaSetting">Determines how to handle alpha.</param>
        /// <param name="mipToSave">0 based index on which mipmap to make top of saved image.</param>
        /// <param name="customMasks">Custom user defined masks for colours.</param>
        /// <returns>True on success.</returns>
        internal static byte[] Save(List<MipMap> MipMaps, ImageEngineFormat format, MipHandling mipChoice, AlphaSettings alphaSetting, int maxDimension = 0, int mipToSave = 0, List<uint> customMasks = null)
        {
            List<MipMap> newMips = new List<MipMap>(MipMaps);

            int width = newMips[0].Width;
            int height = newMips[0].Height;

            bool isMippable = ImageFormats.IsFormatMippable(format);
            if ((isMippable && mipChoice == MipHandling.GenerateNew) || (isMippable && newMips.Count == 1 && mipChoice == MipHandling.Default))
                DDSGeneral.BuildMipMaps(newMips);

            // KFreon: Resize if asked
            if (maxDimension != 0 && maxDimension < width && maxDimension < height) 
            {
                if (!UsefulThings.General.IsPowerOfTwo(maxDimension))
                    throw new ArgumentException($"{nameof(maxDimension)} must be a power of 2. Got {nameof(maxDimension)} = {maxDimension}");

                // KFreon: Check if there's a mipmap suitable, removes all larger mipmaps
                var validMipmap = newMips.Where(img => (img.Width == maxDimension && img.Height <= maxDimension) || (img.Height == maxDimension && img.Width <=maxDimension));  // Check if a mip dimension is maxDimension and that the other dimension is equal or smaller
                if (validMipmap?.Count() != 0)
                {
                    int index = newMips.IndexOf(validMipmap.First());
                    newMips.RemoveRange(0, index);
                }
                else
                {
                    // KFreon: Get the amount the image needs to be scaled. Find largest dimension and get it's scale.
                    double scale = maxDimension * 1d / (width > height ? width : height);

                    // KFreon: No mip. Resize.
                    newMips[0] = Resize(newMips[0], scale);
                }
            }

            // KFreon: Ensure we have a power of two for dimensions FOR DDS ONLY
            double fixXScale = 0;
            double fixYScale = 0;
            if (ImageFormats.IsBlockCompressed(format) && (!UsefulThings.General.IsPowerOfTwo(width) || !UsefulThings.General.IsPowerOfTwo(height)))
            {
                double newWidth = 0;
                double newHeight = 0;

                // Takes into account aspect ratio (a little bit)
                double aspect = width / height;
                if (aspect > 1)
                {
                    newWidth = UsefulThings.General.RoundToNearestPowerOfTwo(width);
                    var tempScale = newWidth / width;
                    newHeight = UsefulThings.General.RoundToNearestPowerOfTwo((int)(height * tempScale));
                }
                else
                {
                    newHeight = UsefulThings.General.RoundToNearestPowerOfTwo(height);
                    var tempScale = newHeight / height;
                    newWidth = UsefulThings.General.RoundToNearestPowerOfTwo((int)(width * tempScale));
                }


                // Little extra bit to allow integer cast from Double with the correct answer. Occasionally dimensions * scale would be 511.99999999998 instead of 512, so adding a little allows the integer cast to return correct value.
                fixXScale = 1d * newWidth / width + 0.001;  
                fixYScale = 1d * newHeight / height + 0.001;
                newMips[0] = Resize(newMips[0], fixXScale, fixYScale);
            }

            if (fixXScale != 0 || fixYScale != 0 || mipChoice == MipHandling.KeepTopOnly)
                DestroyMipMaps(newMips, mipToSave);

            if ((fixXScale != 0 || fixXScale != 0) && isMippable && mipChoice != MipHandling.KeepTopOnly)
                DDSGeneral.BuildMipMaps(newMips);


            byte[] destination = null;
            if (format.ToString().Contains("DDS"))
                destination = DDSGeneral.Save(newMips, format, alphaSetting, customMasks);
            else
            {
                // KFreon: Try saving with built in codecs
                var mip = newMips[0];
                destination = WIC_Codecs.SaveWithCodecs(mip.Pixels, format, mip.Width, mip.Height, alphaSetting);
            }

            return destination;
        }      
        
        internal static MipMap Resize(MipMap mipMap, double scale, bool preserveAspect = true)
        {
            if (preserveAspect)
                return Resize(mipMap, scale, scale);  // Could be either scale dimension, doesn't matter.
            else
                return Resize(mipMap, scale, scale);
        }

        internal static MipMap Resize(MipMap mipMap, double xScale, double yScale)
        {
            var baseBMP = UsefulThings.WPF.Images.CreateWriteableBitmap(mipMap.Pixels, mipMap.Width, mipMap.Height);
            baseBMP.Freeze();
            return Resize(baseBMP, xScale, yScale, mipMap.Width, mipMap.Height);

            #region Old code, but want to keep not only for posterity, but I'm not certain the above works in the context below.
            // KFreon: Only do the alpha bit if there is any alpha. Git #444 (https://github.com/ME3Explorer/ME3Explorer/issues/444) exposed the issue where if there isn't alpha, it overruns the buffer.
            /*bool alphaPresent = mipMap.AlphaPresent;

            WriteableBitmap alpha = new WriteableBitmap(origWidth, origHeight, 96, 96, PixelFormats.Bgr32, null);
            if (alphaPresent && !mergeAlpha)
            {
                // Pull out alpha since scaling with alpha doesn't work properly for some reason
                try
                {
                    unsafe
                    {
                        alpha.Lock();
                        int index = 3;
                        byte* alphaPtr = (byte*)alpha.BackBuffer.ToPointer();
                        for (int i = 0; i < origWidth * origHeight * 4; i += 4)
                        {
                            // Set all pixels in alpha to value of alpha from original image - otherwise scaling will interpolate colours
                            alphaPtr[i] = mipMap.Pixels[index];
                            alphaPtr[i + 1] = mipMap.Pixels[index];
                            alphaPtr[i + 2] = mipMap.Pixels[index];
                            alphaPtr[i + 3] = mipMap.Pixels[index];
                            index += 4;
                        }

                        alpha.Unlock();
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.ToString());
                    throw;
                }
            }

            var bmp = UsefulThings.WPF.Images.CreateWriteableBitmap(mipMap.Pixels, mipMap.Width, mipMap.Height);
            FormatConvertedBitmap main = new FormatConvertedBitmap(bmp, PixelFormats.Bgr32, null, 0);

            

            // Scale RGB
            ScaleTransform scaletransform = new ScaleTransform(scale, scale);
            TransformedBitmap scaledMain = new TransformedBitmap(main, scaletransform);


            // Put alpha back in
            FormatConvertedBitmap newConv = new FormatConvertedBitmap(scaledMain, PixelFormats.Bgra32, null, 0);
            WriteableBitmap resized = new WriteableBitmap(newConv);

            if (alphaPresent && !mergeAlpha)
            {
                TransformedBitmap scaledAlpha = new TransformedBitmap(alpha, scaletransform);
                WriteableBitmap newAlpha = new WriteableBitmap(scaledAlpha);

                try
                {
                    unsafe
                    {
                        resized.Lock();
                        newAlpha.Lock();
                        byte* resizedPtr = (byte*)resized.BackBuffer.ToPointer();
                        byte* alphaPtr = (byte*)newAlpha.BackBuffer.ToPointer();
                        for (int i = 3; i < newStride * newHeight; i += 4)
                            resizedPtr[i] = alphaPtr[i];

                        resized.Unlock();
                        newAlpha.Unlock();
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.ToString());
                    throw;
                }
            }
            
            return new MipMap(resized.GetPixelsAsBGRA32(), newWidth, newHeight, alphaPresent);*/
            #endregion Old code
        }

        internal static MipMap Resize(BitmapSource baseBMP, double xScale, double yScale, int width, int height)
        {
            int origWidth = width;
            int origHeight = height;
            int origStride = origWidth * 4;
            int newWidth = (int)(origWidth * xScale);
            int newHeight = (int)(origHeight * yScale);
            int newStride = newWidth * 4;

            float[] newPixels = null;
            var bmp = UsefulThings.WPF.Images.CreateWPFBitmap(baseBMP, newWidth, newHeight);
            bmp.Freeze();
            newPixels = GetPixelsAsFloats(bmp);

            return new MipMap(newPixels, newWidth, newHeight);
        }

        internal static float[] GetPixelsAsFloats(BitmapSource bmp)
        {
            var source = bmp;
            if (bmp.Format != PixelFormats.Rgb128Float)
                source = new FormatConvertedBitmap(bmp, PixelFormats.Rgb128Float, null, 0);

            float[] pixels = new float[source.PixelHeight * source.PixelWidth * 4];
            source.CopyPixels(pixels, source.PixelWidth * 4, 0);

            return pixels;
        }


        /// <summary>
        /// Destroys mipmaps. Expects at least one mipmap in given list.
        /// </summary>
        /// <param name="MipMaps">List of Mipmaps.</param>
        /// <param name="mipToSave">Index of mipmap to save. 1 based, i.e. top is 1.</param>
        /// <returns>Number of mips present.</returns>
        private static int DestroyMipMaps(List<MipMap> MipMaps, int mipToSave)
        {
            MipMaps.RemoveRange(mipToSave + 1, MipMaps.Count - 1);  // +1 because mipToSave is 0 based and we want to keep it
            return 1;
        }


        /// <summary>
        /// Parses a string to an ImageEngineFormat.
        /// </summary>
        /// <param name="format">String representation of ImageEngineFormat.</param>
        /// <returns>ImageEngineFormat of format.</returns>
        public static ImageEngineFormat ParseFromString(string format)
        {
            ImageEngineFormat parsedFormat = ImageEngineFormat.Unknown;

            if (format.Contains("dxt1", StringComparison.OrdinalIgnoreCase))
                parsedFormat = ImageEngineFormat.DDS_DXT1;
            else if (format.Contains("dxt2", StringComparison.OrdinalIgnoreCase))
                parsedFormat = ImageEngineFormat.DDS_DXT2;
            else if (format.Contains("dxt3", StringComparison.OrdinalIgnoreCase))
                parsedFormat = ImageEngineFormat.DDS_DXT3;
            else if (format.Contains("dxt4", StringComparison.OrdinalIgnoreCase))
                parsedFormat = ImageEngineFormat.DDS_DXT4;
            else if (format.Contains("dxt5", StringComparison.OrdinalIgnoreCase))
                parsedFormat = ImageEngineFormat.DDS_DXT5;
            else if (format.Contains("bmp", StringComparison.OrdinalIgnoreCase))
                parsedFormat = ImageEngineFormat.BMP;
            else if (format.Contains("argb", StringComparison.OrdinalIgnoreCase))
                parsedFormat = ImageEngineFormat.DDS_ARGB_8;
            else if (format.Contains("ati1", StringComparison.OrdinalIgnoreCase))
                parsedFormat = ImageEngineFormat.DDS_ATI1;
            else if (format.Contains("ati2", StringComparison.OrdinalIgnoreCase) || format.Contains("3dc", StringComparison.OrdinalIgnoreCase))
                parsedFormat = ImageEngineFormat.DDS_ATI2_3Dc;
            else if (format.Contains("l8", StringComparison.OrdinalIgnoreCase) || format.Contains("g8", StringComparison.OrdinalIgnoreCase))
                parsedFormat = ImageEngineFormat.DDS_G8_L8;
            else if (format.Contains("v8u8", StringComparison.OrdinalIgnoreCase))
                parsedFormat = ImageEngineFormat.DDS_V8U8;
            else if (format.Contains("jpg", StringComparison.OrdinalIgnoreCase) || format.Contains("jpeg", StringComparison.OrdinalIgnoreCase))
                parsedFormat = ImageEngineFormat.JPG;
            else if (format.Contains("png", StringComparison.OrdinalIgnoreCase))
                parsedFormat = ImageEngineFormat.PNG;
            else if (format.Contains("tiff", StringComparison.OrdinalIgnoreCase))
                parsedFormat = ImageEngineFormat.TIF;


            return parsedFormat;
        }


        /// <summary>
        /// Performs a bulk conversion of a bunch of images given conversion parameters.
        /// </summary>
        /// <param name="files">List of supported files to be converted.</param>
        /// <param name="saveFormat">Destination format of all textures.</param>
        /// <param name="saveFolder">Destination folder of all textures. Can be null if <paramref name="useSourceAsDestination"/> is set.</param>
        /// <param name="saveMipType">Determines how to handle mipmaps for converted images.</param>
        /// <param name="useSourceAsDestination">True = Converted images are saved next to the originals.</param>
        /// <param name="removeAlpha">True = Alpha is removed from converted images.</param>
        /// <param name="customMasks">List of user specified DDS colour masks.</param>
        /// <param name="progressReporter">Progress reporting callback.</param>
        /// <returns></returns>
        public static async Task<ConcurrentBag<string>> BulkConvert(IEnumerable<string> files, ImageEngineFormat saveFormat, string saveFolder,
            MipHandling saveMipType = MipHandling.Default, bool useSourceAsDestination = false, bool removeAlpha = false, List<uint> customMasks = null, IProgress<int> progressReporter = null)
        {
            ConcurrentBag<string> Failures = new ConcurrentBag<string>();


            // Test if can parallelise uncompressed saving
            // Below says: Only formats that don't support mips or do but aren't block compressed - can be parallised.
            bool supportsParallel = !ImageFormats.IsFormatMippable(saveFormat);
            supportsParallel |= !supportsParallel && !ImageFormats.IsBlockCompressed(saveFormat);


            if (EnableThreading && supportsParallel)
                Failures = await DoBulkParallel(files, saveFormat, saveFolder, saveMipType, useSourceAsDestination, removeAlpha, customMasks, progressReporter);
            else
                foreach (var file in files)
                {
                    using (ImageEngineImage img = new ImageEngineImage(file))
                    {
                        string filename = Path.GetFileNameWithoutExtension(file) + "." + ImageFormats.GetExtensionOfFormat(saveFormat);
                        string path = Path.Combine(useSourceAsDestination ? Path.GetDirectoryName(file) : saveFolder, filename);

                        path = UsefulThings.General.FindValidNewFileName(path);

                        try
                        {
                            await img.Save(path, saveFormat, saveMipType, removeAlpha: removeAlpha, customMasks: customMasks);
                        }
                        catch (Exception e)
                        {
                            Failures.Add(path + "  Reason: " + e.ToString());
                        }
                    }

                    progressReporter?.Report(1);   // Value not relevent.
                }


            return Failures;
        }

        static async Task<ConcurrentBag<string>> DoBulkParallel(IEnumerable<string> files, ImageEngineFormat saveFormat, string saveFolder,
            MipHandling saveMipType = MipHandling.Default, bool useSourceAsDestination = false, bool removeAlpha = false, List<uint> customMasks = null, IProgress<int> progressReporter = null)
        {
            ConcurrentBag<string> failures = new ConcurrentBag<string>();

            BufferBlock<string> fileNameStore = new BufferBlock<string>();
            int maxParallelism = ImageEngine.NumThreads == 1 ? 1 :
                (ImageEngine.NumThreads == -1 ? Environment.ProcessorCount : ImageEngine.NumThreads);


            // Define block to perform each conversion
            var encoder = new TransformBlock<string, Tuple<byte[], string>>(file =>
            {
                byte[] data = null;

                string filename = Path.GetFileNameWithoutExtension(file) + "." + ImageFormats.GetExtensionOfFormat(saveFormat);
                string path = Path.Combine(useSourceAsDestination ? Path.GetDirectoryName(file) : saveFolder, filename);
                path = UsefulThings.General.FindValidNewFileName(path);

                using (ImageEngineImage img = new ImageEngineImage(file))
                {
                    try
                    {
                        data = img.Save(saveFormat, saveMipType, removeAlpha: removeAlpha, customMasks: customMasks);
                    }
                    catch (Exception e)
                    {
                        failures.Add(path + "  Reason: " + e.ToString());
                    }
                }

                progressReporter.Report(1);  // Value not relevent.
                return new Tuple<byte[], string>(data, path);
            }, new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = maxParallelism, BoundedCapacity = maxParallelism });

            // Define block to write converted data to disk
            var diskWriter = new ActionBlock<Tuple<byte[], string>>(tuple =>
            {
                File.WriteAllBytes(tuple.Item2, tuple.Item1);
            }, new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = 2, BoundedCapacity = maxParallelism });  // Limit to 2 disk write operations at a time, but allow many to be stored in it's buffer.


            // Link blocks together
            fileNameStore.LinkTo(encoder, new DataflowLinkOptions { PropagateCompletion = true });
            encoder.LinkTo(diskWriter, new DataflowLinkOptions { PropagateCompletion = true });

            // Begin production
            new Action(async () =>
            {
                foreach (var file in files)
                    await fileNameStore.SendAsync(file);

                fileNameStore.Complete();
            }).Invoke();

            await diskWriter.Completion;

            return failures;
        }

        /// <summary>
        /// Merges up to 4 channels into a single image.
        /// </summary>
        /// <param name="blue">Blue channel. Can be null.</param>
        /// <param name="green">Green channel. Can be null.</param>
        /// <param name="red">Red channel. Can be null.</param>
        /// <param name="alpha">Alpha channel. Can be null.</param>
        /// <returns>Merged image.</returns>
        public static ImageEngineImage MergeChannels(MergeChannelsImage blue, MergeChannelsImage green, MergeChannelsImage red, MergeChannelsImage alpha)
        {
            // Merge selected channels
            int length = red?.Pixels.Length ?? blue?.Pixels.Length ?? green?.Pixels.Length ?? alpha?.Pixels.Length ?? 0;
            int width = red?.Width ?? blue?.Width ?? green?.Width ?? alpha?.Width ?? 0;
            int height = red?.Height ?? blue?.Height ?? green?.Height ?? alpha?.Height ?? 0;
            
            // Tests
            var testChannel = blue ?? red ?? green ?? alpha;
            if (!testChannel.IsCompatibleWith(blue, red, green, alpha))
                throw new ArgumentException("All channels must have the same dimensions and data length.");

            byte[] merged = new byte[length];

            if (EnableThreading)
            {
                var merger = new Action<int, MergeChannelsImage>((start, channel) =>
                {
                    for (int i = start; i < length; i += 4)
                        merged[i] = channel?.Pixels[i] ?? 0;
                });
                var b = Task.Run(() => merger(0, blue));
                var g = Task.Run(() => merger(1, green));
                var r = Task.Run(() => merger(2, red));
                var a = Task.Run(() => merger(3, alpha));

                Task.WaitAll(b, g, r, a);
            }
            else
            {
                for (int i = 0; i < length; i += 4)
                {
                    merged[i] = blue?.Pixels[i] ?? 0;
                    merged[i + 1] = green?.Pixels[i + 1] ?? 0;
                    merged[i + 2] = red?.Pixels[i + 2] ?? 0;
                    merged[i + 3] = alpha?.Pixels[i + 3] ?? 0;
                }
            }

            //var mip = new MipMap(merged, width, height);
            return new ImageEngineImage(null);
        }
    }
}
