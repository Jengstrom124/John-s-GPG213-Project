using Unity.Netcode;
using UnityEngine;

/// <summary>
/// Basic "Singleton" implementation
/// </summary>
/// <typeparam name="T">T is defined when you inherit your Manager. Put the class name in the triangle brackets</typeparam>
public class ManagerBase<T> : NetworkBehaviour where T : ManagerBase<T>
{
    // There's only ever ONE of these. Hence the name singleton
    public static T Instance;

    public virtual void Awake()
    {
        Instance = (T)this;
    }
}
