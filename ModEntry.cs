using System;
using HighlightUnwateredTiles.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Menus;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.GameData.Crops;
using StardewValley.GameData.HomeRenovations;
using StardewValley.SDKs;
using StardewValley.TerrainFeatures;

namespace HighlightUnwateredTiles
{
    public class ModEntry : Mod
    {
        private List<Vector2> _waterableCropCoordinates = new List<Vector2>();

        public override void Entry(IModHelper helper)
        {
            helper.Events.Display.RenderingHud += this.OnRenderingHud;
        }

        private void OnRenderingHud(object? sender, RenderingHudEventArgs e)
        {
            if (!Context.IsWorldReady) return;
            _waterableCropCoordinates.Clear();

            bool isFarm = Game1.currentLocation is Farm;
            if (!isFarm) return;

            IEnumerable<Vector2> visibleTiles = TileHelper.GetTilesInViewport(2);

            foreach (var coordinates in visibleTiles)
            {
                HoeDirt? dirt = GetHoeDirtAtCoordinates(coordinates);
                if (dirt == null) continue;
                if (dirt.crop != null && dirt.needsWatering())
                {
                    _waterableCropCoordinates.Add(coordinates);
                }
            }

            foreach (var coordinates in _waterableCropCoordinates)
            {
                HighlightTile(e, coordinates, Color.Aqua * 0.5f);
            }
        }

        private void HighlightTile(RenderingHudEventArgs e, Vector2 coordinates, Color? highlightColor)
        {
            Vector2 tileScreenPosition = Game1.GlobalToLocal(Game1.viewport,
                new Vector2(coordinates.X * Game1.tileSize, coordinates.Y * Game1.tileSize));

            Rectangle tileHighlightArea = new Rectangle((int)tileScreenPosition.X, (int)tileScreenPosition.Y, Game1.tileSize,
                Game1.tileSize);

            e.SpriteBatch.Draw(
                Game1.staminaRect,
                tileHighlightArea,
                highlightColor ?? Color.Aqua * 0.5f);
        }

        private HoeDirt? GetHoeDirtAtCoordinates(Vector2 coordinates)
        {
            GameLocation currLocation = Game1.currentLocation;
            bool isTerrain = currLocation.terrainFeatures.TryGetValue(coordinates, out TerrainFeature terrain);

            return (isTerrain && terrain is HoeDirt dirt) ? dirt : null;
        }
    }
}