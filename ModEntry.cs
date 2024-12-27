using System;
using Microsoft.Xna.Framework;
using StardewValley.Menus;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.GameData.Crops;
using StardewValley.SDKs;
using StardewValley.TerrainFeatures;

namespace HighlightUnwateredTiles
{
    public class ModEntry : Mod
    {
        private bool isLaunched;
        private Dictionary<int, List<Vector2>> cropLocations = new Dictionary<int, List<Vector2>>();
        // private Stage CurrentStage = Stage.none;

        public override void Entry(IModHelper helper)
        {
            helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
            helper.Events.GameLoop.DayStarted += this.OnDayStarted;
            helper.Events.Player.Warped += this.HandleWarped;
        }

        private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
        {
            // Handle Initial Lauched items here 
            this.Monitor.Log("Loaded highlight unwatered crops");
            this.isLaunched = true;
        }

        private void OnDayStarted(object? sender, DayStartedEventArgs e)
        {
            if (!this.isLaunched)
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

        private void UpdateWaterableCropLocations()
        {
            if (!Context.IsWorldReady)
                return;

            var crops = Game1.cropData;
            this.cropLocations.Clear();

            foreach (var crop in crops)
            {
                this.Monitor.Log($"Crop ID: {crop.Key}, Data: {crop.Value}", LogLevel.Info);
            }

            GameLocation farm = Game1.getLocationFromName("Farm");
            foreach (var tile in farm.terrainFeatures.Pairs)
            {
                // if (tile.Value is not StardewValley.TerrainFeatures.HoeDirt) continue;
                // HoeDirt is a location where crops are planted
                if (tile.Value is StardewValley.TerrainFeatures.HoeDirt dirt)
                {
                    if (dirt.crop != null && dirt.needsWatering())
                    {
                        Vector2 pos = tile.Key;
                        Crop crop = dirt.crop;

                        this.Monitor.Log($"Crop: {crop.rowInSpriteSheet.Name} id: {crop.rowInSpriteSheet.Value}");
                        int cropId = crop.rowInSpriteSheet.Value;
                        if (!this.cropLocations.ContainsKey(cropId))
                        {
                            this.cropLocations[cropId] = new List<Vector2>();
                        }

                        this.cropLocations[cropId].Add(pos);
                    }
                }
            }

            foreach (var cropLocation in this.cropLocations)
            {
                foreach (var i in cropLocation.Value)
                {
                    this.Monitor.Log($"Crop ID: {cropLocation.Key}, xCoord: {i.X}, yCoord: {i.Y}");
                }
            }
        }
    }
}