namespace MigrationTool.Reader
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Model;

    public static class CsvReader
    {
        public static IEnumerable<RockSong> ReadCsv(string filePath)
        {
            string[] allLines = File.ReadAllLines(filePath);

            var songs = from line in allLines
                        let data = line.Split(',')
                        select new RockSong
                        {
                            Song = data[0],
                            Artist = data[1],
                            Combined = data[2],
                            ReleaseYear = data[3]
                        };

            return songs;
        }
    }
}