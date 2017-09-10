using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;

namespace Ember.IO
{
    /// <summary>
    /// Used to write default property files
    /// </summary>
    public static class DefaultPropertiesCreator
    {
        /// <summary>
        /// Default Properties
        /// </summary>
        public static Dictionary<string, List<PropertyItem>> DefaultProperties { get; set; } = new Dictionary<string, List<PropertyItem>>()
        {
            { "General", new List<PropertyItem>()
            {
                new PropertyItem("Antialiasing", "Smooths out sharp edges", "Antialiasing", false, typeof(bool)),
                new PropertyItem("Hide Eye Candy", "Hides small details in the game", "HideEyeCandy", true, typeof(bool)),
                new PropertyItem("Show Godrays", "", "ShowGodray", false, typeof(bool))
            }},
            { "Performance", new List<PropertyItem>()
            {
                new PropertyItem("Show Shadows", "", "ShadowsEnabled", false, typeof(bool)),
                new PropertyItem("Character Inking", "Draws a thin black outline around characters", "CharacterInking", false, typeof(bool)),
                new PropertyItem("HUD Animations", "Some HUD elements will have animations", "EnableHUDAnimations", false, typeof(bool)),
                new PropertyItem("Grass Swaying", "Bushes will sway when you step into them", "EnableGrassSwaying", false, typeof(bool)),
                new PropertyItem("Enable FXAA", "Special method of Antialisaing", "EnableFXAA", false, typeof(bool))
            }}
        };

        /// <summary>
        /// Writes <see cref="DefaultProperties"/> into the specified path
        /// </summary>
        /// <param name="path">The path to wrie to</param>
        public static void WriteDefaultProperties(string path)
        {
            File.WriteAllText(path, JsonConvert.SerializeObject(DefaultProperties, Formatting.Indented));
        }
    }
}
