using System;
using System.Collections.Generic;
using System.IO;

namespace Ember.IO.Ini
{
    public class IniFile
    {
        public Dictionary<string, Dictionary<string, string>> Entries { get; private set; } = new Dictionary<string, Dictionary<string, string>>();

        public IniFile(Dictionary<string, Dictionary<string, string>> entries)
        {
            this.Entries = entries;
        }

        public IniFile(string fileLocation) : this(File.OpenRead(fileLocation)) { }

        public IniFile(Stream stream)
        {
            using (StreamReader sr = new StreamReader(stream))
            {
                while (!sr.EndOfStream)
                {
                    string[] line = sr.ReadLine().Split(new char[] { '[', ']', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (line.Length != 0 && line[0].Length != 0)
                    {
                        this.Entries.Add(line[0], new Dictionary<string, string>());
                        ReadValues(sr, line[0]);
                    }
                }
            }
        }

        public void Write(string fileLocation)
        {
            Write(File.Create(fileLocation));
        }

        public void Write(Stream stream)
        {
            using (StreamWriter sw = new StreamWriter(stream))
            {
                foreach (KeyValuePair<string, Dictionary<string, string>> entry in this.Entries)
                {
                    sw.WriteLine(string.Format("[{0}]", entry.Key));
                    foreach (KeyValuePair<string, string> value in entry.Value)
                    {
                        sw.WriteLine(string.Format("{0}={1}", value.Key, value.Value));
                    }
                }
            }
        }

        private void ReadValues(StreamReader sr, string entry)
        {
            string[] line = null;

            while (sr.Peek() != '[')
            {
                if (!sr.EndOfStream)
                {
                    if ((line = sr.ReadLine().Split(new char[] { '=' }, StringSplitOptions.RemoveEmptyEntries)).Length != 0)
                    {
                        this.Entries[entry].Add(line[0], line[1]);
                    }
                }
                else
                {
                    break;
                }
            }
        }
    }
}
