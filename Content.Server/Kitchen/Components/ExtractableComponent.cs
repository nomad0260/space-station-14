﻿using Content.Server.Kitchen.EntitySystems;
using Content.Shared.Chemistry.Components;
using Content.Server.Andromeda.Botany.Systems;

namespace Content.Server.Kitchen.Components
{
    /// <summary>
    /// Tag component that denotes an entity as Extractable
    /// </summary>
    [RegisterComponent]
    [Access(typeof(ReagentGrinderSystem), typeof(PlantExtractorSystem))]
    public sealed partial class ExtractableComponent : Component
    {
        [DataField("juiceSolution")]
        public Solution? JuiceSolution;

        [DataField("grindableSolutionName")]
        public string? GrindableSolution;
    }
}
