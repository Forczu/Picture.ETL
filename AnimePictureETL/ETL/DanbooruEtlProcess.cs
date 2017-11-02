using AnimePictureETL.ETL.Load;
using AnimePictureETL.ETL.Transform;
using AnimePictureETL.Extract.ETL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using log4net;

namespace AnimePictureETL.ETL
{
    public class DanbooruEtlProcess
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static string EXTRACT_HEADER = "extracts:";
        private static string TRANSFORM_HEADER = "transforms:";
        private static string LOAD_HEADER = "loads:";
        private static string FREQUENCY_HEADER = "frequency:";

        ExtractChangesDanbooru extract = null;
        TransformChangesDanbooru transform = null;
        LoadChangesDanbooru load = null;

        IList<string> observedTags = new List<string>();
        IDictionary<string, string[]> possibleTransformations = new Dictionary<string, string[]>();

        int checkInterval;

        public DanbooruEtlProcess(string configFilePath, string downloadDest)
        {
            ParseConfig(configFilePath);
            extract = new ExtractChangesDanbooru(observedTags, downloadDest);
            transform = new TransformChangesDanbooru(possibleTransformations);
            load = new LoadChangesDanbooru();
        }

        public DanbooruEtlProcess(string configFilePath, string tag, string downloadDest)
        {
            ParseConfig(configFilePath);
            List<string> list = new List<string>();
            list.Add(tag);
            extract = new ExtractChangesDanbooru(list, downloadDest);
            transform = new TransformChangesDanbooru(possibleTransformations);
            load = new LoadChangesDanbooru();
        }

        public void Run()
        {
            log.Info("Starting the ETL process");
            log.Info("Starting the Extract phase for Danbooru");
            var changes = extract.Extract();
            log.Info("Starting the Transform phase for Danbooru");
            var changedObjects = transform.Transform(changes);
            log.Info("Starting the Load phase for Danbooru");
            load.Load(changedObjects);
            log.Info("The ETL process for Danbooru ended successfully.");
        }

        private void ParseConfig(string configFilePath)
        {
            using (StreamReader reader = new StreamReader(configFilePath))
            {
                while(!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    if (line == EXTRACT_HEADER)
                    {
                        ParseExtractConfig(reader);
                        ParseTransformConfig(reader);
                    }
                    if (line == FREQUENCY_HEADER)
                    {
                        line = reader.ReadLine();
                        checkInterval = Convert.ToInt32(line);
                    }
                }
            }
        }

        private void ParseExtractConfig(StreamReader reader)
        {
            string line = reader.ReadLine();
            while (line != TRANSFORM_HEADER)
            {
                if (!string.IsNullOrWhiteSpace(line))
                {
                    observedTags.Add(line);
                }
                line = reader.ReadLine();
            }
        }

        private void ParseTransformConfig(StreamReader reader)
        {
            string line = reader.ReadLine();
            while (line != LOAD_HEADER)
            {
                if (!string.IsNullOrEmpty(line))
                {
                    string[] options = line.Split('=');
                    string[] values = options[0].Split(' ');
                    string[] keys = options[1].Split(',');
                    foreach (var key in keys)
                    {
                        possibleTransformations.Add(key, values);
                    }
                }
                line = reader.ReadLine();
            }
        }
    }
}