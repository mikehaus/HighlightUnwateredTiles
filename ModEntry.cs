using System;
using Microsoft.Xna.Framework;
using StardewValley.Menus;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.GameData.Crops;
using StardewValley.SDKs;

namespace HighlightUnwateredTiles
{
    public class ModEntry : Mod
    {
        private bool isLaunched;
        private List<string> cropsToHighlight = new List<string>();
        // private Stage CurrentStage = Stage.none;
        
        public override void Entry(IModHelper helper)
        {
            helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
            helper.Events.GameLoop.DayStarted += this.OnDayStarted;
            helper.Events.GameLoop.UpdateTicked += this.CheckForCropsToWater;
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
            var crops = Game1.cropData;
            
            foreach (var crop in crops)
            {
                this.Monitor.Log($"Crop ID: {crop.Key}, Data: {crop.Value}", LogLevel.Info);
                if (crop.Value.NeedsWatering)
                    cropsToHighlight.Add(crop.Key);
            }

            foreach (var cropId in cropsToHighlight)
            {
                this.Monitor.Log($"CROP NEEDS WATERING: {cropId}", LogLevel.Info);
            }
        }

        // private void HandleReadCrops(object? sender, EventArgs? e)
        private void HandleReadCrops()
        {
            if (!Context.IsWorldReady)
                return;

            var crops = Game1.cropData;

            foreach (var crop in crops)
            {
                this.Monitor.Log($"Crop ID: {crop.Key}, Data: {crop.Value}", LogLevel.Info);
            }
        }

        private void CheckForCropsToWater(object? sender, UpdateTickedEventArgs e)
        {
            
        }
        
    }
}