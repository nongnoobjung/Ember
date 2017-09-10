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
                new PropertyItem("Antialiasing", "Smooths out sharp edges", "Antialiasing", typeof(bool)),
                new PropertyItem("Hide Eye Candy", "Hides small details in the game", "HideEyeCandy", typeof(bool))
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
