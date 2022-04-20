using UnityEngine;

public class DisableOnPlay : MonoBehaviour
{
    void Awake()
    {
        foreach (Transform child in GetComponentsInChildren<Transform>())
        {
            child.gameObject.SetActive(false);
        }
    }
}
