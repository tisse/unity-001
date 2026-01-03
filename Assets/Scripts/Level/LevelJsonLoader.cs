using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace Level
{
    public static class LevelJsonLoader
    {
        public static void LoadJsonLevel(int level = 1)
        {
            var jsonFile = Resources.Load<TextAsset>($"data/level{level}");
            if (jsonFile == null)
            {
                Debug.Log($"level{level}: is null");
                return;
            }

            var data = JsonConvert.DeserializeObject<List<LevelData>>(jsonFile.text);
            data.ForEach(item =>
            {
                item.Data.ForEach(itemData =>
                    {
                        var cells = itemData.Data.Split(',');
                        for (var x = 0; x < cells.Length; x++)
                        {
                            var cell = cells[x].Trim();

                            if (!string.IsNullOrEmpty(cell) && cell != "0")
                            {
                                LevelBlockUtils.CreateBlock(x, itemData.Row, item.Floor);
                            }
                        }
                    }
                );
            });
        }

    }
}