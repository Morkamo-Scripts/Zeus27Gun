using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using UnityEngine;

namespace Zeus27Gun.Components
{
    public abstract class Zeus27Component : CustomWeapon
    {
        public override float Weight { get; set; } = 1;
        public override SpawnProperties SpawnProperties { get; set; } = null;
        
        public abstract bool EnableHighlight { get; set; }
        public abstract string HighlightColor { get; set; }
        public abstract float HighlightRange { get; set; }
        public abstract float HighlightIntensity { get; set; }
        
        public abstract bool EnableParticles { get; set; }
        public abstract Vector3 SpawnRange { get; set; }
        public abstract float ParticleSize { get; set; }
        public abstract ushort Intensity { get; set; }
    }
}