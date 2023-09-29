using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public bool isUsable;
    public Element element;

    public Cell(bool _isUsable, Element _element)
    {
       isUsable = _isUsable;
       element = _element;
    }
}
