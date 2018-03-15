using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace TerViewer
{
    class Program
    {
        static void Main(string[] args)
        {
            //Sample
            //            singleton Substance(loosensoil)
            //{
            //                ter2_id = 6;
            //                terrainMaterialName = "LoosenSoil";
            //                tunnelFloorMaterialName = "TunnelFloorMaterial";
            //                tunnelCeilingMaterialName = "TunnelCeilingMaterial";
            //                tunnelWallMaterialName = "TunnelWallsMaterial";
            //                quantity_k = 1;
            //                maxHeightDiffBeforeFall = 4;
            //                canBeDigged = 0;
            //                canBeShaped = 1;
            //                diggedObjectID = 334;
            //                droppedObjectID = 334;
            //                footstepsType = "soil";
            //                WalkSpeedMultiplier = 0.97;
            //                HorseSpeedMultiplier = 0.97;
            //                WheelSpeedMultiplier = 0.85;
            //                SledgeSpeedMultuplier = 0.2;
            //            };
            var substances = new Dictionary<int, string>();
            var substancesFile = @"D:\Steam\steamapps\common\Life is Feudal Your Own\scripts\cm_substances.cs";
            var regex = new Regex(@"ter2_id.+?([0-9]*?);.*?terrainMaterialName.*?\""([A-Za-z]*?)\"";", RegexOptions.Singleline);
            var text = File.ReadAllText(substancesFile);
            foreach (Match match in regex.Matches(text))
            {
                var substId = int.Parse(match.Groups[1].Value);
                var substName = match.Groups[2].Value;
                substances.Add(substId, substName);
            }
            foreach (var substance in substances)
            {
                Console.WriteLine($"Id: {substance.Key}, Name: {substance.Value}");
            }
        }
    }
}