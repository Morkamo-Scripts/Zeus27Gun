using System;
using Exiled.API.Features;
using Exiled.CustomItems.API;

namespace Zeus27Gun
{
    public class Plugin : Plugin<Config>
    {
        public override string Name => "Zeus27Gun";
        public override string Prefix => Name;
        public override string Author => "Morkamo";
        public override Version Version => new Version(1, 0, 0);
        public override Version RequiredExiledVersion => new Version(9, 12, 1);

        public static Plugin Instance { get; private set; }
        
        public override void OnEnabled()
        {
            Instance = this;
            Config.Zeus27Shoker.Register();
            Config.Zeus27Tranquilizer.Register();
            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            Instance = null;
            Config.Zeus27Shoker.Unregister();
            Config.Zeus27Tranquilizer.Unregister();
            base.OnDisabled();
        }
    }
}