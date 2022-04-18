namespace VFRZInstancing.Instancing
{
    /// <summary>
    /// Instances for Drawing
    /// </summary>
    public static class InstancesAtlasCoordinate
    {
        public const uint AtlasIndexMask   = 0b_11111111_11000000_00000000_00000000; // atlas index, 0-1023
        public const uint AtlasXOffsetMask = 0b_00000000_00111111_00000000_00000000; // tile x position in atlas, 0-63 (2048px / 32px)
        public const uint AtlasYOffsetMask = 0b_00000000_00000000_11111110_00000000; // tile y position in atlas, 0-127 (2048px / 16px)
        public const uint TileWidthMask    = 0b_00000000_00000000_00000001_11100000; // additional tile width, 0-15 (tile is always at least 1 tile wide)
        public const uint TileHeightMask   = 0b_00000000_00000000_00000000_00011111; // additional tile height, 0-31 (tile is always at least 1 tile high)

        public static void SetAtlasIndex(ref int value, in int atlasIndex) => value |= (atlasIndex << 22);

        public static void SetTilePositionInAtlas(ref int value, in int atlasXOffset, in int atlasYOffset)
        {
            SetTilePositionXInAtlas(ref value, atlasXOffset);
            SetTilePositionYInAtlas(ref value, atlasYOffset);
        }

        public static void SetTilePositionXInAtlas(ref int value, in int atlasXOffset) => value |= (atlasXOffset << 16);

        public static void SetTilePositionYInAtlas(ref int value, in int atlasYOffset) => value |= (atlasYOffset << 9);

        public static void SetTileSizeInAtlas(ref int value, in int tileWidth, in int tileHeight)
        {
            SetTileWidthInAtlas(ref value, tileWidth);
            SetTileHeightInAtlas(ref value, tileHeight);
        }

        public static void SetTileWidthInAtlas(ref int value, in int tileWidth) => value |= ((tileWidth - 1) << 5);

        public static void SetTileHeightInAtlas(ref int value, in int tileHeight) => value |= (tileHeight - 1);
    }
}
