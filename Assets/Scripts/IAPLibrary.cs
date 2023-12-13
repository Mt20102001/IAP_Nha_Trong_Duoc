using IAP;
using UnityEngine;

[CreateAssetMenu(fileName = "IAPLibrary", menuName = "IAP/IAPs Library")]
public class IAPLibrary : ScriptableObject
{
    [field: SerializeField] public IAPElement[] iAPElements;
}
