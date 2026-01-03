using UnityEngine;

namespace Hero
{
    public class ClassData
    {
        public string Name;
        public string ColorData;


        public Color GetColor()
        {
            switch (ColorData)
            {
                case "red": return Color.red;
                case "blue": return Color.blue;
                case "yellow": return Color.yellow;
            }
            return Color.gray8;
        }
    }
}