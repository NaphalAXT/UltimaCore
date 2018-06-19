﻿using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;

namespace UltimaCore.Graphics
{
    public static class Gumps
    {
        private static UOFile _file;

        public static void Load()
        {
            string path = Path.Combine(FileManager.UoFolderPath, "gumpartLegacyMUL.uop");
            if (File.Exists(path))
            {
                _file = new UOFileUop(path, ".tga", 0x10000, true);
            }
            else
            {
                path = Path.Combine(FileManager.UoFolderPath, "Gumpart.mul");
                string pathidx = Path.Combine(FileManager.UoFolderPath, "Gumpidx.mul");

                if (File.Exists(path) && File.Exists(pathidx))
                {
                    _file = new UOFileMul(path, pathidx, 0x10000, 12);
                }
            }

            string pathdef = Path.Combine(FileManager.UoFolderPath, "gump.def");
            if (!File.Exists(pathdef))
                return;

            using (StreamReader reader = new StreamReader(File.OpenRead(pathdef)))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    line = line.Trim();
                    if (line.Length <= 0 || line[0] == '#')
                        continue;
                    string[] defs = line.Replace('\t', ' ').Split(' ');
                    if (defs.Length != 3)
                        continue;

                    int ingump = int.Parse(defs[0]);
                    int outgump = int.Parse(defs[1].Replace("{", string.Empty).Replace("}", string.Empty));
                    int outhue = int.Parse(defs[2]);

                    _file.Entries[ingump] = _file.Entries[outgump];
                }
            }
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct GumpBlock
        {
            public ushort Value;
            public ushort Run;
        }

        public unsafe static ushort[] GetGump(int index, out int width, out int height)
        {
            (int length, int extra, bool patcher) = _file.SeekByEntryIndex(index);

            if (extra == -1)
            {
                width = 0; height = 0;
                return null;
            }

            width = (extra >> 16) & 0xFFFF;
            height = extra & 0xFFFF;

            if (width <= 0 || height <= 0)
                return null;

            ushort[] pixels = new ushort[width * height];
            int* lookuplist = (int*)(_file.StartAddress + _file.Position);

            for (int y = 0; y < height; y++)
            {
                int gsize = 0;
                if (y < height - 1)
                    gsize = lookuplist[y + 1] - lookuplist[y];
                else
                    gsize = (length / 4) - lookuplist[y];


                GumpBlock* gmul = (GumpBlock*)(_file.StartAddress + _file.Position + lookuplist[y] * 4);
                int pos = y * width;

                for (int i = 0; i < gsize; i++)
                {
                    ushort val = gmul[i].Value;
                    ushort a = (ushort)((val > 0 ? 0x8000 : 0) | val);
                    int count = gmul[i].Run;
                    for (int j = 0; j < count; j++)
                        pixels[pos++] = a;
                }
            }
         
            return pixels;
        }
    }
}
