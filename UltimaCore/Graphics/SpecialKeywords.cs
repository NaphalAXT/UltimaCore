﻿using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;

namespace UltimaCore.Graphics
{
    public static class SpecialKeywords
    {
        private static UOFile _file;

        private static List<KeywordEntry> _keywords = new List<KeywordEntry>();

        public static IReadOnlyList<KeywordEntry> Keywords => _keywords;

        public static void Load()
        {
            if (_keywords.Count > 0)
                return;

            string path = Path.Combine(FileManager.UoFolderPath, "speech.mul");
            if (!File.Exists(path))
                throw new FileNotFoundException();

            _file = new UOFileMul(path);

            while (_file.Position < _file.Length)
            {
                ushort id = (ushort)((_file.ReadByte() << 8) | _file.ReadByte());
                ushort length = (ushort)((_file.ReadByte() << 8) | _file.ReadByte());

                if (length > 128)
                        length = 128;

                _keywords.Add(new KeywordEntry()
                {
                    Code = id,
                    Text = Encoding.UTF8.GetString(_file.ReadArray(length))
                });
            }          
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct KeywordEntry
    {
        public ushort Code;
        //[MarshalAs(UnmanagedType.LPWStr)]
        public string Text;
    }
}
