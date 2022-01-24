using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

public class StepmaniaParser
{
    public float[][] ExtractBeatmap()
    {
            
        // Get the data from the file
        var stepmania = File.ReadAllText(Application.dataPath + "/excitement.sm");
        var metadata = new Regex("#.*?;", RegexOptions.Singleline).Match(stepmania);

        // Add the song to the pack
        var isSongValid = true; // check for validity in the metadata

        string notes = "";
        
        while (metadata.Success)
        {
            // get the key value pairs
            var datum = metadata.Value;
            var key = datum.Substring(0, datum.IndexOf(":")).Trim('#').Trim(':');
            var value = datum.Substring(datum.IndexOf(":")).Trim(':').Trim(';');
            
            if (key.ToUpper() == "NOTES")
            {
                notes = value;
                break;
            }
            metadata = metadata.NextMatch();
        }
        
        int n = 6;
        string[] lines = notes
            .Split(Environment.NewLine.ToCharArray())
            .Skip(n)
            .ToArray();

        notes = string.Join(Environment.NewLine, lines);
        
        string[] measures = notes
            .Split("\n,")
            .ToArray();

        string[][] smMap = new string[measures.Length][];
        for (int i = 0; i < measures.Length; i++)
        {
            smMap[i] = measures[i].Split("\n")
                .ToArray();
        }

        float[] beat = {0, 0, 0, 0};

        List<float>[] meep =
        {
            new(),
            new(),
            new(),
            new()
        };

        for (int i = 0; i < smMap.Length; i++)
        {
            float progression = 4f / smMap[i].Length;
            for (int j = 0; j < smMap[i].Length; j++)
            {
                for (int k = 0; k < smMap[i][j].Length; k++)
                {
                    if (smMap[i][j][k] == '1')
                    {
                        meep[k].Add(beat[k]);
                        beat[k] += progression;
                    }
                }
            }
        }

        float[][] beatMap = new float[4][];

        for (int i = 0; i < 4; i++)
        {
            beatMap[i] = meep[i].ToArray();
        }
        
        return beatMap;
    }
}