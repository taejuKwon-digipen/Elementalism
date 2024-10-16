using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy : Entity
{
    void OnMouseDown()
    {
        this.Hit(this);
    }
}
