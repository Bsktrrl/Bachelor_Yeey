using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public enum Items
{
    [Description("None")][InspectorName("None")] None,

    [Description("Stone")] [InspectorName("Nature/Stone")] Stone,
    [Description("Plank")][InspectorName("Nature/Plank")] Plank,
    [Description("Leaf")][InspectorName("Nature/Leaf")] Leaf,

    [Description("Axe")][InspectorName("Tool/Axe")] Axe,
    [Description("Builder")][InspectorName("Tool/Builder")] Builder
}
