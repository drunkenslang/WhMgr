﻿namespace WhMgr.Alarms.Alerts
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;

    public class DynamicReplacementEngine
    {
        public static string ReplaceText(string alarmText, IReadOnlyDictionary<string, string> pkmnInfo)
        {
            var placeHolder = alarmText;
            var keys = pkmnInfo.Keys.ToList();

            //Loop through all available keys, replace any place holders with values.
            for (var i = 0; i < keys.Count; i++)
            {
                var key = keys[i];
                var value = pkmnInfo[key];

                placeHolder = placeHolder.Replace($"<{key}>", value);
            }

            //Replace IF statement blocks i.e. <#is_ditto>**Catch Pokemon:** <original_pkmn_name></is_ditto>. If value is true return value inside IF block, otherwise return an empty string.
            for (var i = 0; i < keys.Count; i++)
            {
                var key = keys[i];
                var value = pkmnInfo[key];

                if (bool.TryParse(value, out var result))
                {
                    placeHolder = ReplaceBlock(placeHolder, key, result);
                }
            }
            return placeHolder;
        }

        private static string ReplaceBlock(string text, string property, bool value = false)
        { 
            var expr = @"\<#" + property + @">([^\}]+)\</" + property + @">";
            var regex = new Regex(expr);
            var match = regex.Match(text);
            return string.IsNullOrEmpty(match?.Value) ? 
                text :
                text.Replace(match.Value, value ?
                    match?.Groups[1]?.Value : 
                    string.Empty
            );
        }
    }
}