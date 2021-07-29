﻿using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using WClipboard.Windows.Native;

// Source: https://stackoverflow.com/questions/35636953/show-multiple-file-properties-in-c-sharp

namespace WClipboard.Windows.Helpers
{
    public static class ShellIDListHelper
    {
        
        public static MemoryStream Create(IReadOnlyCollection<string> filenames)
        {
            // first convert all files into pidls list
            int pos = 0;
            byte[][] pidls = new byte[filenames.Count][];
            foreach (var filename in filenames)
            {
                // Get pidl based on name
                var pidl = NativeMethods.ILCreateFromPath(filename);
                uint pidlSize = NativeMethods.ILGetSize(pidl);
                // Copy over to our managed array
                pidls[pos] = new byte[pidlSize];
                Marshal.Copy(pidl, pidls[pos++], 0, (int)pidlSize);
                NativeMethods.ILFree(pidl);
            }

            // Determine where in CIDL we will start pumping PIDLs
            int pidlOffset = 4 * (filenames.Count + 2);
            // Start the CIDL stream
            var memStream = new MemoryStream();
            var sw = new BinaryWriter(memStream);
            // Initialize CIDL witha count of files
            sw.Write(filenames.Count);
            // Calcualte and write relative offsets of every pidl starting with root
            sw.Write(pidlOffset);
            pidlOffset += 4; // root is 4 bytes
            foreach (var pidl in pidls)
            {
                sw.Write(pidlOffset);
                pidlOffset += pidl.Length;
            }

            // Write the root pidl (0) followed by all pidls
            sw.Write(0);
            foreach (var pidl in pidls) sw.Write(pidl);
            // stream now contains the CIDL
            return memStream;
        }
    }
}
