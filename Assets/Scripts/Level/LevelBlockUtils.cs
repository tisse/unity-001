using System.Collections.Generic;
using UnityEngine;

namespace Level
{
    public static class LevelBlockUtils
    {
        private static readonly Dictionary<string, BlockType> BlockTypeMap = new()
        {
            { "0", BlockType.Empty },
            { "1", BlockType.Floor },
            { "2", BlockType.Water }
        };

        private static readonly Color WaterColor = new(0.2f, 0.6f, 0.9f, 0.4f);

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

        public static GameObject CreateBlock(int i, int j, int level)
        {
            const int xSize = 10;
            const float ySize = 0.4f;
            const float yMargin = 1f;
            const int zSize = 10;
            var gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            gameObject.name = $"Block_{i}_{j}_{level}";
            gameObject.transform.position = new Vector3(i * xSize, (level * (yMargin + ySize)), j * zSize);
            gameObject.transform.rotation = Quaternion.identity;
            gameObject.transform.localScale = new Vector3(1f * xSize, ySize, 1f * zSize);
            gameObject.tag = "FLOOR";
            return gameObject;
        }

        public static GameObject CreateBlock(int i, int j, int level, string blockTypeKey)
        {
            const int xSize = 10;
            const float ySize = 0.4f;
            const float yMargin = 1f;
            const int zSize = 10;
            var blockType = BlockTypeMap[blockTypeKey];
            GameObject gameObject = null;
            var parameter = new LevelBlockBuildParameter()
            {
                I = i,
                J = j,
                Level = level,
                XSize = xSize,
                YMargin = yMargin,
                YSize = ySize,
                ZSize = zSize
            };
            switch (blockType)
            {
                case BlockType.Empty: 
                    // CreateEmptyBlock(parameter); 
                    break;
                case BlockType.Floor:
                    gameObject = CreateFloorBlock(parameter);
                    break;
                case BlockType.Water:
                    gameObject = CreateWaterBlock(parameter);
                    break;
                default: break;
            }
            return gameObject;
        }

        private static GameObject CreateFloorBlock(LevelBlockBuildParameter parameter)
        {
            var gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            gameObject.name = $"Block_{parameter.I}_{parameter.J}_{parameter.Level}";
            gameObject.transform.position = new Vector3(parameter.I * parameter.XSize, 
                parameter.Level * (parameter.YMargin + parameter.YSize), 
                parameter.J * parameter.ZSize);
            gameObject.transform.rotation = Quaternion.identity;
            gameObject.transform.localScale = new Vector3(1f * parameter.XSize, parameter.YSize, 1f * parameter.ZSize);
            gameObject.tag = "FLOOR";
            return gameObject;
        }

        private static GameObject CreateEmptyBlock(LevelBlockBuildParameter parameter)
        {
            var gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            gameObject.name = $"EmptyBlock_{parameter.I}_{parameter.J}_{parameter.Level}";
            gameObject.transform.position = new Vector3(parameter.I * parameter.XSize,
                parameter.Level * (parameter.YMargin + parameter.YSize),
                parameter.J * parameter.ZSize);
            gameObject.transform.rotation = Quaternion.identity;
            gameObject.transform.localScale = new Vector3(1f * parameter.XSize, parameter.YSize, 1f * parameter.ZSize);
            var renderer = gameObject.GetComponent<Renderer>();
            if (renderer == null)
            {
                renderer = gameObject.AddComponent<MeshRenderer>();
            }

            renderer.material.color = Color.yellowGreen;
            return gameObject;
        }

        private static GameObject CreateWaterBlock(LevelBlockBuildParameter parameter)
        {
            var position = new Vector3(parameter.I * parameter.XSize,
                parameter.Level * (parameter.YMargin + parameter.YSize),
                parameter.J * parameter.ZSize);
            var localScale = new Vector3(1f * parameter.XSize, parameter.YSize, 1f * parameter.ZSize);
            var waterBlock = GameObject.CreatePrimitive(PrimitiveType.Cube);
            waterBlock.transform.localPosition = position;
            waterBlock.transform.localScale = localScale;
            waterBlock.name = $"WaterBlock_{parameter.I}_{parameter.J}_{parameter.Level}";
            var renderer = waterBlock.GetComponent<Renderer>();
            var shader = Shader.Find("Universal Render Pipeline/Lit");
            var waterMaterial = new Material(shader)
            {
                color = WaterColor,
                renderQueue = 3000
            };

            waterMaterial.SetFloat("_Metallic", 0.1f);
            waterMaterial.SetFloat("_Glossiness", 0.9f); // Высокая гладкость для эффекта воды
            waterMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            waterMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            waterMaterial.SetInt("_ZWrite", 0);
            waterMaterial.DisableKeyword("_ALPHATEST_ON");
            waterMaterial.EnableKeyword("_ALPHABLEND_ON");
            waterMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            waterMaterial.SetOverrideTag("RenderType", "Transparent");
            waterMaterial.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
            waterMaterial.SetFloat("_Blend", 0);
            waterMaterial.SetFloat("_Surface", 1);
            
            renderer.material = waterMaterial;
            return waterBlock;
        }

        private static GameObject CreateWaterBlock1(LevelBlockBuildParameter parameter)
        {
            var position = new Vector3(parameter.I * parameter.XSize,
                parameter.Level * (parameter.YMargin + parameter.YSize),
                parameter.J * parameter.ZSize);
            var localScale = new Vector3(1f * parameter.XSize, parameter.YSize, 1f * parameter.ZSize);
            var waterBlock = new GameObject($"WaterBlock_{parameter.I}_{parameter.J}_{parameter.Level}")
            {
                transform =
                {
                    position = position
                }
            };

            var waterVisual = GameObject.CreatePrimitive(PrimitiveType.Cube);
            waterVisual.name = $"WaterVisual_{parameter.I}_{parameter.J}_{parameter.Level}";
            waterVisual.transform.SetParent(waterBlock.transform);
            waterVisual.transform.localPosition = Vector3.zero;
            waterVisual.transform.localScale = localScale;

            var renderer = waterVisual.GetComponent<Renderer>();
            var waterMaterial = new Material(Shader.Find("Standard"))
            {
                color = WaterColor,
                renderQueue = 3000
            };

            waterMaterial.SetFloat("_Metallic", 0.1f);
            waterMaterial.SetFloat("_Glossiness", 0.9f); // Высокая гладкость для эффекта воды
            waterMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            waterMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            waterMaterial.SetInt("_ZWrite", 0);
            waterMaterial.DisableKeyword("_ALPHATEST_ON");
            waterMaterial.EnableKeyword("_ALPHABLEND_ON");
            waterMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");

            renderer.material = waterMaterial;
            return waterBlock;
        }
    }

    internal class LevelBlockBuildParameter
    {
        public int I;
        public int J;
        public int Level;
        public int XSize;
        public float YMargin;
        public float YSize;
        public int ZSize;
    }
}