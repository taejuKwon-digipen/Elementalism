using UnityEngine;

public class CardSpawnPoint : MonoBehaviour
{
    public int Index { get; private set; }
    public bool IsOccupied { get; set; }
    
    private void Awake()
    {
        Index = transform.GetSiblingIndex();
    }
} 