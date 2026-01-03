using System;
using System.Collections.Generic;

namespace Level
{
    public class LevelData
    {
        public int Floor;
        public List<RowData> Data;

        public override string ToString()
        {
            var join = String.Join(", ", Data.ToString());
            return $"Floor: {Floor}, Data: {join}";
        }
    }

    public class RowData
    {
        public int Row;
        public string Data;

        public override string ToString()
        {
            return $"Row: {Row}, Data: {Data}";
        }
    }
}