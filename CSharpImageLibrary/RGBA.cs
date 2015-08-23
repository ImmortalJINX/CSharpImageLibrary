﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsefulThings;
using static CSharpImageLibrary.DDSGeneral;

namespace CSharpImageLibrary
{
    /// <summary>
    /// Provides RGBA (DDS) format functionality
    /// </summary>
    public static class RGBA
    {
        /// <summary>
        /// Loads useful information from RGBA DDS image file.
        /// </summary>
        /// <param name="imagePath">Path to RGBA DDS file.</param>
        /// <param name="Width">Detected Width.</param>
        /// <param name="Height">Detected Height.</param>
        /// <returns>Raw pixel data as stream.</returns>
        private static MemoryTributary Load(string imagePath, out double Width, out double Height)
        {
            using (FileStream fs = new FileStream(imagePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                return Load(fs, out Width, out Height);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="Width"></param>
        /// <param name="Height"></param>
        /// <returns></returns>
        private static MemoryTributary Load(Stream stream, out double Width, out double Height)
        {
            DDS_HEADER header = null;
            Format format = ParseDDSFormat(stream, out header);

            Width = header.dwWidth;
            Height = header.dwHeight;

            MemoryTributary imgData = new MemoryTributary();
            imgData.ReadFrom(stream, stream.Length - stream.Position);

            return imgData;
        }
    }
}