using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public enum Items
{
    [Description("None")][InspectorName("None")] None,

    [Description("Stone")] [InspectorName("Gem/Stone")] Stone,

    [Description("Stick")][InspectorName("Wood/Stick")] Stick,

    [Description("Axe")][InspectorName("Tool/Axe")] Axe
}
