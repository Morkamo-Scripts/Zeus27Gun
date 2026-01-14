using Exiled.API.Interfaces;
using Zeus27Gun.Variants;

namespace Zeus27Gun
{
    public class Config : IConfig
    {
        public bool IsEnabled { get; set; } = true;
        public bool Debug { get; set; } = false;

        public Zeus27Shoker Zeus27Shoker { get; set; } = new();
        public Zeus27Tranquilizer Zeus27Tranquilizer { get; set; } = new();
    }
}