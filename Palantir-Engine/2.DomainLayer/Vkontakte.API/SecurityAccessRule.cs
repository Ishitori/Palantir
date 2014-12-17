namespace Ix.Palantir.Vkontakte.API
{
    using System;

    // http://vkontakte.ru/developers.php?oid=-1&p=%D0%9F%D1%80%D0%B0%D0%B2%D0%B0_%D0%BF%D1%80%D0%B8%D0%BB%D0%BE%D0%B6%D0%B5%D0%BD%D0%B8%D0%B9
    [Flags]
    public enum SecurityAccessRule
    {
        None = 0,
        Notifications = 1,
        Frieds = 2,
        Photos = 4,
        AudioFiles = 8,
        VideoFiles = 16,
        Applications = 32,
        Quesions = 64,
        WikiPages = 128,
        LeftHref = 256,
        QuickPost = 512,
        Statuses = 1024,
        Notes = 2048,
        ExtendedMethods = 4096,
        WallMethods = 8192,
        Missed1 = 16384,
        AdCabinet = 32768,
        Missed2 = 65536,
        Documents = 131072,
        Groups = 262144
    }
}