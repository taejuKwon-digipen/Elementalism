using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;

public class OrderManager : MonoBehaviour 
{
    [SerializeField] Renderer[] BackRenderer;
    [SerializeField] Renderer[] MiddleRenderer;
    [SerializeField] string SortingLayerName;

    int OriginOrder;

    void Start()
    {
        SetOrder(0);
    }

    public void SetOriginOrder(int originOder)
    {
        this.OriginOrder = originOder;
        SetOrder(originOder);
    }
    
    public void SetMostFrontOrder (bool ismostfront)
    {
        SetOrder (ismostfront ? 100 : OriginOrder);
    }   

    public void SetOrder(int order)//We will use this script when we get card sprite
    {
        int MulOrder = order * 10; 
        
        foreach(var renderer in BackRenderer)
        {
            renderer.sortingLayerName = SortingLayerName; 
            renderer.sortingOrder = MulOrder;
        }
        foreach(var renderer in MiddleRenderer)
        {
            renderer.sortingLayerName = SortingLayerName;
            renderer.sortingOrder = MulOrder + 1;

        }
    }
 
}
