using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public enum ItemCategories
{
    [Description("")] None,

    [Description("Raw")] Raw,

    [Description("Food/Water")] Food,
    [Description("Other")] Other,
    [Description("Tools")] Tools,
    [Description("Weapons")] Weapons,
    [Description("Equipments")] Equipments,
    [Description("Resources")] Resources,
    [Description("Navigation")] Navigation,
    [Description("Decoration")] Decoration
}

public enum ItemSubCategories
{
    //Food
    [Description("Bottles")][InspectorName("Food/Bottles")] Bottles,
    [Description("Bottles")][InspectorName("Food/Containers")] Containers,
    [Description("Bottles")][InspectorName("Food/Purifiers")] Purifiers,
    [Description("Bottles")][InspectorName("Food/Grills")] Grills,
    [Description("Bottles")][InspectorName("Food/CoockingTables")] CoockingTables,
    [Description("Bottles")][InspectorName("Food/Cropplots")] Cropplots,
    [Description("Bottles")][InspectorName("Food/Scrarecrow")] Scrarecrow,
    [Description("Bottles")][InspectorName("Food/Healing")] Healing,
    [Description("Bottles")][InspectorName("Food/Sprinkler")] Sprinkler,

    //Weapons
    [Description("Bottles")][InspectorName("Food/Weapons")] Weapons,
    [Description("Bottles")][InspectorName("Food/BowAndArrow")] BowAndArrow,
    [Description("Bottles")][InspectorName("Food/NetLauncher")] NetLauncher,
}
