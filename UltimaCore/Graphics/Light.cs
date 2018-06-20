﻿using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace UltimaCore.Graphics
{
    public static class Light // is it useful?
    {
        private static UOFileMul _file;

        public static void Load()
        {
            string path = Path.Combine(FileManager.UoFolderPath, "light.mul");
            string pathidx = Path.Combine(FileManager.UoFolderPath, "lightidx.mul");

            if (!File.Exists(path) || !File.Exists(pathidx))
                throw new FileNotFoundException();

            _file = new UOFileMul(path, pathidx, 100);
        }

        public static unsafe ushort[] GetLight(int idx, out int width, out int height)
        {
            (int length, int extra, bool patched) = _file.SeekByEntryIndex(idx);

            width = (extra & 0xFFFF);
            height = ((extra >> 16) & 0xFFFF);

            ushort[] pixels = new ushort[width * height];

            //ushort* p = (ushort*)(_file.StartAddress + _file.Position);


            for (int i = 0; i < height; i++)
            {
                int pos = i * width;
                for (int j = 0; j < width; j++)
                {
                    ushort val = _file.ReadUShort();
                    val = (ushort)((val << 10) | (val << 5) | val);
                    //p++;
                    pixels[pos + j] = (ushort)((val > 0 ? 0x8000 : 0) | val);
                }
            }

            return pixels;
        }
    }
}
