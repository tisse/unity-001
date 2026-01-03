using Hero;
using UnityEngine;

public class GameObjectUtils
{
    protected GameObjectUtils()
    {
    }


    public static GameObject CreateGameObject(ClassData classData, int counter)
    {
        var gameObject = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        gameObject.transform.position = new Vector3(counter, 3, counter);
        gameObject.transform.rotation = Quaternion.identity;
        gameObject.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);

        CreateLight(gameObject, classData);

        var renderer = gameObject.GetComponent<Renderer>();
        if (renderer == null)
        {
            renderer = gameObject.AddComponent<MeshRenderer>();
        }

        renderer.material.color = classData.GetColor();

        CreateRigidbody(gameObject);

        return gameObject;
    }

    private static void CreateRigidbody(GameObject gameObject)
    {
        var rb = gameObject.AddComponent<Rigidbody>();
        rb.mass = 1f;
        rb.useGravity = true;
    }

    private static void CreateLight(GameObject gameObject, ClassData classData)
    {
        var lightComp = gameObject.AddComponent<Light>();
        lightComp.type = LightType.Point;
        lightComp.range = 1f;
        lightComp.intensity = 2f;
        lightComp.color = classData.GetColor();
    }
}