namespace Surabhi
{

    using System;
    using System.Globalization;

    public interface IParseLevelsFiles
    {
        void ParseFile(string path, string code);
    }

    public class ParseLevels : IParseLevelsFiles
    {
        public void ParseFile(string file, string code)
        {
            var readStream = System.IO.File.OpenRead(file);

            using (var conn = new MySqlDataConnector())
            {
                using (var reader = new System.IO.StreamReader(readStream))
                {
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();

                        var columns = line.Split(";");

                        var dateTimeString = columns[0];
                        var date = DateTime.ParseExact(dateTimeString, "yyyyMMdd HHmmss", CultureInfo.InvariantCulture)
                        .ToUniversalTime()
                        .ToString("yyyy-MM-dd HH:mm:ss");


                        var level = new Level()
                        {
                            Id = $"{date}_{code}",
                            Timestamp = date,
                            Name = code,
                            High = double.Parse(columns[1]),
                            Open = double.Parse(columns[2]),
                            Low = double.Parse(columns[3]),
                            Close = double.Parse(columns[4]),
                            Volume = double.Parse(columns[5])
                        };

                        conn.PutLevel(level);
                    }

                }

            }
        }
    }

    public class Level
    {
        public string Id { get; set; }
        public string Timestamp { get; set; }
        public string Name { get; set; }
        public double Open { get; set; }
        public double High { get; set; }
        public double Low { get; set; }
        public double Close { get; set; }
        public double Volume { get; set; }
    }
}