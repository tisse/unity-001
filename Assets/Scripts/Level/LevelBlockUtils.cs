using UnityEngine;

namespace Level
{
    public static class LevelBlockUtils
    {
        public static GameObject CreateBlock(int i, int j)
        {
            const int xSize = 10;
            const int zSize = 10;
            var gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            gameObject.transform.position = new Vector3(i * xSize, 1, j * zSize);
            gameObject.transform.rotation = Quaternion.identity;
            gameObject.transform.localScale = new Vector3(1f * xSize, 0.2f, 1f * zSize);
            return gameObject;
        }

        public static GameObject CreateBlock(int i, int j, int k)
        {
            const int xSize = 10;
            const float ySize = 0.4f;
            const int zSize = 10;
            var gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            gameObject.name = $"Block_{i}_{j}_{k}";
            gameObject.transform.position = new Vector3(i * xSize, k * ySize, j * zSize);
            gameObject.transform.rotation = Quaternion.identity;
            gameObject.transform.localScale = new Vector3(1f * xSize, ySize, 1f * zSize);
            return gameObject;
        }
    }
}