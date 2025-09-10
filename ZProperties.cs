// Properties for HHSAdvSDL

using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Data;
using Microsoft.VisualBasic;
using System.Diagnostics;
using System.Text.Json;

namespace HHSAdvSDL
{
    public class ZProperties
    {
        public class Attributes
        {
            public string FontPath { get; set; } = @"C:\Windows\Fonts\YuGothR.ttc";
            public bool OpeningRoll { get; set; } = true;
            public bool PlaySound { get; set; } = true;
        }

        private Attributes attributes = new Attributes();

        public Attributes Attrs
        {
            get { return attributes; }
        }

        public ZProperties()
        {
        }
        public bool Load(string fileName)
        {
            if (!File.Exists(fileName)) return false;

            string jsonString = File.ReadAllText(fileName);
            if (string.IsNullOrEmpty(jsonString))
            {
                return false;
            }
            var deserializedAttributes = JsonSerializer.Deserialize<Attributes>(jsonString);
            if (deserializedAttributes == null)
            {
                return false;
            }
            attributes = deserializedAttributes;

            return true;
        }
        public bool Save(string fileName)
        {
            File.WriteAllText(fileName, JsonSerializer.Serialize(attributes));
            return true;
        }

    }
}
