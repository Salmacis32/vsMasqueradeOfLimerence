namespace Masquerade.Stats
{
    /// <summary>
    /// Values that exist to represent the variables pertaining to an equipment as an item available in shops and level-ups.
    /// </summary>
    [Flags]
    public enum ShopTags
    {
        None = 0,
        HideFromShop = 1,
        HiddenEquipment = 2,
        SpecialShopsOnly = 4,
        StartsUnlocked = 8,
        StartsSeen = 16,
        DoNotExcludeFromShop = 32,
        DropRateAffectedByLuck = 64,
        Unsealable = 128,
        OnlyAppliesToOwner = 256,
        DuplicatesAllowed = 512,
        StaysSpawnedWhileUnavailable = 1024
    }
}
