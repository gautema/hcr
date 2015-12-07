using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using HttpClientRecorder.SerializedEntities;
using Newtonsoft.Json;

namespace HttpClientRecorder
{
    internal class FileHandler
    {
        private readonly string _fileLocation;

        public FileHandler(string fileLocation)
        {
            _fileLocation = fileLocation;
        }

        internal string GetFileName(string hostname)
        {
            return _fileLocation + hostname.StripSpecialChars() + ".json";
        }

        public List<Interaction> GetInteractions(string hostname)
        {
            var filename = GetFileName(hostname);
            if (!File.Exists(filename)) return null;
            var file = File.ReadAllText(filename);
            return JsonConvert.DeserializeObject<List<Interaction>>(file);
        }

        public void SaveInteraction(string hostname, Interaction interaction)
        {
            List<Interaction> interactions;
            var filename = GetFileName(hostname);
            if (File.Exists(filename))
            {
                var file = File.ReadAllText(hostname);
                interactions = JsonConvert.DeserializeObject<List<Interaction>>(file);
            }
            else
            {
                interactions = new List<Interaction>();
            }
            interactions.Add(interaction);
            var json = JsonConvert.SerializeObject(interactions, Formatting.Indented);
            File.WriteAllText(filename, json);
        }
    }
}