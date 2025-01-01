using StardewValley;
using Microsoft.Xna.Framework;
using xTile.Layers;

namespace HighlightUnwateredTiles.Utilities;

/// <summary>
/// Extension helper utilities for working with Game tiles
/// </summary>
internal static class TileHelper
{
    /// <summary>
    /// Returns the visible area of the screen for the player at a given point in time 
    /// </summary>
    /// <param name="tileBufferCount"></param>
    public static Rectangle GetBufferedViewport(int tileBufferCount = 0)
    {
        return new Rectangle(
            x: (Game1.viewport.X / Game1.tileSize) - tileBufferCount,
            y: (Game1.viewport.Y / Game1.tileSize) - tileBufferCount,
            width: (int)Math.Ceiling(Game1.viewport.Width / (decimal)Game1.tileSize) + (tileBufferCount * 2),
            height: (int)Math.Ceiling(Game1.viewport.Height / (decimal)Game1.tileSize) + (tileBufferCount * 2)
        );
    }

    /// <summary>
    /// Returns all the tiles in a game location
    /// </summary>
    /// <param name="location">A Stardew GameLocation</param>
    public static IEnumerable<Vector2> GetTilesInLocation(this GameLocation location)
    {
        if (location?.Map?.Layers == null) return Enumerable.Empty<Vector2>(); 
        
        Layer layer = location.Map.Layers[0];
        return TileHelper.GetTiles(0, 0, layer.LayerWidth, layer.LayerHeight);
    }

    /// <summary>
    /// Returns all the tiles in an XNA Rectangle area
    /// </summary>
    /// <param name="boundingRect">An XNA Rectangle object representing a given area</param>
    public static IEnumerable<Vector2> GetTilesInArea(this Rectangle boundingRect)
    {
        return TileHelper.GetTiles(boundingRect.X, boundingRect.Y, boundingRect.Width, boundingRect.Height);
    }

    /// <summary>
    /// Returns all the tile coordinates in the players viewport with optional buffer (extra X tiles beyond seen area)
    /// </summary>
    /// <param name="tileBufferCount">An additional count of tiles to be used to prevent pop-in</param>
    public static IEnumerable<Vector2> GetTilesInViewport(int tileBufferCount = 0)
    {
       Rectangle viewport = TileHelper.GetBufferedViewport(tileBufferCount); 
       
       return TileHelper.GetTilesInArea(viewport);
    }

    /// <summary>
    /// Get all tiles in a given x, y coordinate plane for a given width and height
    /// </summary>
    /// <param name="x">The X coordinate (top-left tile)</param>
    /// <param name="y">The Y coordinate (top-left tile)</param>
    /// <param name="width">the grid width</param>
    /// <param name="height">the grid height</param>
    private static IEnumerable<Vector2> GetTiles(int x, int y, int width, int height)
    {
        for (int curX = x, maxX = x + width - 1; curX <= +maxX; curX++)
        {
            for (int curY = y, maxY = y + height - 1; curY <= +maxY; curY++)
                yield return new Vector2(curX, curY);
        }
    }
}