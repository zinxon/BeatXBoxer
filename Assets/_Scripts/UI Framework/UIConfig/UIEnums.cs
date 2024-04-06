using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UIFramework
{
    public enum UIBaseType
    {
        Normal,
        Fixed,
        Popup
    }

    public enum UIShowMode
    {
        Normal,
        ReverseChange,
        HideOther
    }

    public enum UITransparency{
        Lucency,
        Translucence,
        Impenetrable,
        Penetrate
    }    
}