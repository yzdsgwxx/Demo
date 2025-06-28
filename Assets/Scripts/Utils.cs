using UnityEngine;

public static class Utils
{
    public static Vector3 GetFootPosition(Collider col)
    {
        return col.bounds.center + Vector3.down * col.bounds.extents.y;
    }

    public static GameObject GetPlayer(GameObject obj)
    {
        return GameObject.FindGameObjectWithTag("Player");
    }

    public static Vector3 RemoveZFromDirection(Vector3 direction)
    {
        direction.y = 0;
        direction = direction.normalized;
        return direction;
    }
}
