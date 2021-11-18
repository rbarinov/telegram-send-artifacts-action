using System;

namespace TelegramSendArtifactsAction
{
    public static class IconHelper
    {
        private static readonly string[] _icons =
        {
            "🐒", "🐘", "🐝", "🐞", "🐢", "🐥", "🐧", "🐨", "🐫", "🐬", "🐭", "🐯", "🐰", "🐻", "🐌", "🐍", "🐔", "🐑",
            "🐎", "🐜", "🐛", "🐩", "🐮", "🐽"
        };

        private static readonly Random _random = new();

        public static string GetRandomIcon()
        {
            return _icons[_random.Next(0, _icons.Length)];
        }
    }
}
