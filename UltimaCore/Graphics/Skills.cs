﻿using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace UltimaCore.Graphics
{
    public static class Skills
    {
        private static UOFileMul _file;

        private static readonly Dictionary<int, SkillEntry> _skills = new Dictionary<int, SkillEntry>();

        public static int SkillsCount => _skills.Count;

        public static void Load()
        {
            if (SkillsCount > 0)
                return;

            string path = Path.Combine(FileManager.UoFolderPath, "Skills.mul");
            string pathidx = Path.Combine(FileManager.UoFolderPath, "Skills.idx");

            if (!File.Exists(path) || !File.Exists(pathidx))
                throw new FileNotFoundException();

            _file = new UOFileMul(path, pathidx, 56, 16);

            int i = 0;
            while (_file.Position < _file.Length)
            {
                GetSkill(i++);
            }
        }

        public static SkillEntry GetSkill(int index)
        {
            if (!_skills.TryGetValue(index, out var value))
            {
                (int length, int extra, bool patched) = _file.SeekByEntryIndex(index);
                if (length == 0)
                    return default;

                value = new SkillEntry()
                {
                    HasButton = _file.ReadBool(),
                    Name = Encoding.UTF8.GetString(_file.ReadArray(length - 1)),
                    Index = index
                };

                _skills[index] = value;
            }
            return value;      
        }        
    }

    public struct SkillEntry
    {
        public int Index;
        public string Name;
        public bool HasButton;
    }

}
