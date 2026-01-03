using System.IO;
using UnityEngine;

namespace Level
{
    public static class LevelLoader
    {
        public static void LoadLevel(int level = 1)
        {
            var levelPath = Path.Combine(
                Application.streamingAssetsPath,
                "Levels",
                $"level{level}.txt"
            );
            Debug.Log($"levelPath ; {levelPath}");
            if (!File.Exists(levelPath))
            {
                Debug.LogError($"File not found: {levelPath}");
                return;
            }

            using var reader = new StreamReader(levelPath);
            var y = 0;

            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();

                if (line == null)
                {
                    break;
                }

                // Пропускаем комментарии
                if (line.StartsWith("//") || string.IsNullOrWhiteSpace(line))
                    continue;

                // Парсим строку уровня
                ParseLevelRow(line, y);
                y++;
            }
        }

        private static void ParseLevelRow(string line, int row = 1)
        {
            var cells = line.Split(',');
            for (var x = 0; x < cells.Length; x++)
            {
                var cell = cells[x].Trim();

                if (!string.IsNullOrEmpty(cell) && cell != "0")
                {
                    LevelBlockUtils.CreateBlock(x, row);
                }
            }
        }
    }
}