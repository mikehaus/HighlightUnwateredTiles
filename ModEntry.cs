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
        private bool _isLaunched;
        private Dictionary<int, List<Vector2>> _cropLocations = new Dictionary<int, List<Vector2>>();

        private Texture2D _highlightTexture = new Texture2D(Game1.graphics.GraphicsDevice, 1, 1);
        // private Stage CurrentStage = Stage.none;

        public override void Entry(IModHelper helper)
        {
            helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
            helper.Events.GameLoop.DayStarted += this.OnDayStarted;
            helper.Events.Player.Warped += this.HandleWarped;
            helper.Events.GameLoop.UpdateTicked += this.HandleUpdateTicked;
            helper.Events.Display.RenderingHud += this.OnRenderingHud;
        }

        private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
        {
            // Handle Initial Lauched items here 
            this.Monitor.Log("Loaded highlight unwatered crops");
            this._isLaunched = true;
        }

        private void OnDayStarted(object? sender, DayStartedEventArgs e)
        {
            if (!this._isLaunched)
                return;

            this.Monitor.Log("READING CROP INFO");
        }

        private void HandleWarped(object? sender, WarpedEventArgs e)
        {
            if (!Context.IsWorldReady) return;

            this.Monitor.Log($"Entered new map location {e.NewLocation.Name}");

            // TODO: enhance this with specific location logic
            this.UpdateWaterableCropLocations();
        }

        // TODO: Refactor this to incorporate new utilities
        private void UpdateWaterableCropLocations()
        {
            if (!Context.IsWorldReady)
                return;

            var crops = Game1.cropData;
            _cropLocations.Clear();

            foreach (var crop in crops)
            {
                this.Monitor.Log($"Crop ID: {crop.Key}, Data: {crop.Value}", LogLevel.Info);
            }

            GameLocation farm = Game1.getLocationFromName("Farm");
            foreach (var tile in farm.terrainFeatures.Pairs)
            {
                if (tile.Value is HoeDirt dirt)
                {
                    if (dirt.crop != null && dirt.needsWatering())
                    {
                        Vector2 pos = tile.Key;
                        Crop crop = dirt.crop;

                        // this.Monitor.Log($"Crop: {crop.rowInSpriteSheet.Name} id: {crop.rowInSpriteSheet.Value}");
                        int cropId = crop.rowInSpriteSheet.Value;
                        if (!_cropLocations.ContainsKey(cropId))
                        {
                            _cropLocations[cropId] = new List<Vector2>();
                        }

                        _cropLocations[cropId].Add(pos);
                    }
                }
            }

            this.Monitor.Log($"Found {_cropLocations.Count} Crops needing watering");
            foreach (var cropLocation in _cropLocations)
            {
                foreach (var vec in cropLocation.Value)
                {
                    this.Monitor.Log($"Crop ID: {cropLocation.Key}, xCoord: {vec.X}, yCoord: {vec.Y}");
                }
            }
        }

        private void OnRenderingHud(object? sender, RenderingHudEventArgs e)
        {
            if (!Context.IsWorldReady) return;

            this.Monitor.Log("Rendering hud");
            foreach (var cropLocation in _cropLocations)
            {
                foreach (var coordinates in cropLocation.Value)
                {
                    this.Monitor.Log("Hightlighting Tile:");
                    this.HighlightTile(e, coordinates);
                }
            }
        }

        private void HighlightTile(RenderingHudEventArgs e, Vector2 coordinates)
        {
            Rectangle tileBounds = new Rectangle(
                (int)(coordinates.X * Game1.tileSize - Game1.viewport.X),
                (int)(coordinates.Y * Game1.tileSize - Game1.viewport.Y),
                Game1.tileSize,
                Game1.tileSize
            );

            e.SpriteBatch.Draw(
                _highlightTexture,
                tileBounds,
                Color.Aqua * 0.5f);
        }
        
        private void HandleUpdateTicked(object? sender, UpdateTickedEventArgs e)
        {
            if (!Context.IsWorldReady) return;

            var visibleTiles = TileHelper.GetTilesInViewport();

            foreach (var coordinate in visibleTiles)
            {
               // TODO: check each tile at coordinate and update the waterable tiles dictionary 
            }
        }
    }
}