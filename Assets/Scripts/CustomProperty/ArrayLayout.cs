using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ArrayLayout
{
    [System.Serializable]
    public class rowData
    {
        public bool[] row;
    }

   public rowData[] rows = new rowData[8];
    //creates a grid with a Y of 8.
}
