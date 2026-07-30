using UnityEngine;

public static class UnityUtils
{
    public static GameObject SetChild(Transform Parent, string name, Vector3 localPosition, Quaternion localRotation, Vector3 localScale)
    {
        Transform existingChild = Parent.transform.Find(name);

        if (existingChild != null)
        {
            return existingChild.gameObject;
        }
        else
        {
            GameObject newChild = new GameObject(name);

            newChild.transform.SetParent(Parent);

            newChild.transform.localPosition = localPosition;
            newChild.transform.localRotation = localRotation;
            newChild.transform.localScale = localScale;

            return newChild;
        }
    }
}
