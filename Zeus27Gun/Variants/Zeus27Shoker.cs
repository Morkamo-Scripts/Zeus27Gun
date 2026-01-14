using System.Collections;
using System.ComponentModel;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.API.Features.Pickups;
using Exiled.Events.EventArgs.Player;
using LabApi.Events.Arguments.ServerEvents;
using MEC;
using PlayerRoles;
using RueI.API;
using RueI.API.Elements;
using UnityEngine;
using Zeus27Gun.Components;
using events = Exiled.Events.Handlers;
using Light = Exiled.API.Features.Toys.Light;

namespace Zeus27Gun.Variants
{
    public class Zeus27Shoker : Zeus27Component
    {
        public override uint Id { get; set; } = 50;
        public override string Name { get; set; } = "Zeus27 - Шокер.";
        public override string Description { get; set; }
        public override ItemType Type { get; set; } = ItemType.GunCOM15;
        public override float Damage { get; set; } = 15;
        
        public override bool EnableHighlight { get; set; } = true;
        public override string HighlightColor { get; set; } = "#eba400";
        public string HighlightSecondColor { get; set; } = "#00eb3b";
        public override float HighlightRange { get; set; } = 0.7f;
        public override float HighlightIntensity { get; set; } = 4f;
    
        public override bool EnableParticles { get; set; } = true;
        public override Vector3 SpawnRange { get; set; } = new(0.7f, 0.7f, 0.7f);
        public override float ParticleSize { get; set; } = 0.1f;
        public override ushort Intensity { get; set; } = 5;

        private bool _isFirstEquip = true;

        protected override void SubscribeEvents()
        {
            events.Player.UnloadingWeapon += OnUnloading;
            events.Player.ReloadingWeapon += OnReloading;
            events.Player.Shot += OnShot;
            events.Player.Shooting += OnShooting;
            events.Player.ChangedItem += OnChangedItem;
            events.Player.DroppedItem += OnDroppedItem;
            LabApi.Events.Handlers.ServerEvents.PickupCreated += OnPickupCreated;
        }

        protected override void UnsubscribeEvents()
        {
            events.Player.UnloadingWeapon -= OnUnloading;
            events.Player.ReloadingWeapon -= OnReloading;
            events.Player.Shot -= OnShot;
            events.Player.Shooting -= OnShooting;
            events.Player.ChangedItem -= OnChangedItem;
            events.Player.DroppedItem -= OnDroppedItem;
            LabApi.Events.Handlers.ServerEvents.PickupCreated -= OnPickupCreated;
        }
        
        private void OnPickupCreated(PickupCreatedEventArgs ev) => HighlightItemDouble(Pickup.Get(ev.Pickup.GameObject));
        private void OnDroppedItem(DroppedItemEventArgs ev) => HighlightItemDouble(ev.Pickup);
        
        private void HighlightItemDouble(Pickup pickup)
        {
            if (Check(pickup))
            {
                if (ColorUtility.TryParseHtmlString(HighlightColor, out var color))
                {
                    var anchor = HighlightManager.MakeLight(pickup.Position, color,
                        LightShadows.None, HighlightRange, HighlightIntensity - 1.5f);

                    Light anchor2 = null;
                    
                    if (ColorUtility.TryParseHtmlString(HighlightSecondColor, out var lightSecondColor))
                    {
                        anchor2 = HighlightManager.MakeLight(pickup.Position, lightSecondColor,
                            LightShadows.None, HighlightRange, HighlightIntensity);
                    }
                    
                    if (EnableParticles)
                    {
                        HighlightManager.ProceduralParticles(anchor.GameObject, color, 0, 0.05f,
                            SpawnRange, ParticleSize, Intensity);
                        
                        if (ColorUtility.TryParseHtmlString(HighlightSecondColor, out var secondColor))
                            HighlightManager.ProceduralParticles(anchor.GameObject, secondColor, 0, 0.05f,
                                SpawnRange, ParticleSize, Intensity);
                    }
                    
                    anchor.Transform.SetParent(pickup.Transform);
                    anchor.Spawn();
                    
                    anchor2?.Transform.SetParent(pickup.Transform);
                    anchor2?.Spawn();
                }
                else
                {
                    var anchor = HighlightManager.MakeLight(pickup.Position, Color.white,
                        LightShadows.None, HighlightRange, HighlightIntensity);
                    
                    if (EnableParticles)
                    {
                        HighlightManager.ProceduralParticles(anchor.GameObject, Color.white, 0, 0.05f,
                            SpawnRange, ParticleSize, Intensity);
                        
                        if (ColorUtility.TryParseHtmlString(HighlightSecondColor, out _))
                            HighlightManager.ProceduralParticles(anchor.GameObject, Color.white, 0, 0.05f,
                                SpawnRange, ParticleSize, Intensity);
                    }
                    
                    anchor.Transform.SetParent(pickup.Transform);
                    anchor.Spawn();
                        
                    Log.Warn("Установлен некорректный цвет подсветки, выбор значения по умолчанию..."); 
                }
            }
        }

        protected override void OnAcquired(Player player, Item item, bool displayMessage)
        {
            if (!_isFirstEquip)
                return;
            
            _isFirstEquip = false;

            if (Item.Get(item.Base) is Firearm firearm)
            {
                firearm.MagazineAmmo = 1;
                firearm.MaxMagazineAmmo = 1;
            }
        }

        protected new void OnReloading(ReloadingWeaponEventArgs ev)
        {
            if (Check(ev.Firearm))
                ev.IsAllowed = false;
        }

        private void OnUnloading(UnloadingWeaponEventArgs ev)
        {
            if (Check(ev.Firearm))
                ev.IsAllowed = false;
        }
        
        private new void OnShot(ShotEventArgs ev)
        {
            if (!Check(ev.Firearm))
                return;
            
            Timing.CallDelayed(30f, () =>
            {
                ev.Firearm.MagazineAmmo = 1;
                ev.Firearm.MaxMagazineAmmo = 1;
            });
            
            if (ev.Target == null || ev.Target.Role.Type == RoleTypeId.Scp173)
                return;
            
            ev.Target.EnableEffect(EffectType.Flashed, 15f);
            ev.Target.EnableEffect(EffectType.Ensnared, 15f);
        }
        
        private void OnChangedItem(ChangedItemEventArgs ev)
        {
            if (Check(ev.Item))
                CoroutineRunner.Run(HintsHandler(ev.Player));
        }

        public IEnumerator HintsHandler(Player player)
        {
            while (!Round.IsEnded && Round.IsStarted && player.IsAlive && Check(player.CurrentItem))
            {
                RueDisplay.Get(player).Show(
                    new Tag(),
                    new BasicElement(110, "<size=25><b><color=#F79100>Вы используете Zeus27 - Шокер!</color></b></size>"), 1.1f);

                foreach (var spec in player.CurrentSpectatingPlayers)
                {
                    RueDisplay.Get(spec).Show(
                        new Tag(),
                        new BasicElement(110, "<size=25><b><color=#F79100>Игрок использует Zeus27 - Шокер!</color></b></size>"), 1.1f);
                    
                    Timing.CallDelayed(1.2f, () => RueDisplay.Get(spec).Update());
                }
                Timing.CallDelayed(1.2f, () => RueDisplay.Get(player).Update());

                yield return new WaitForSeconds(1f);
            }
        }
    }
}