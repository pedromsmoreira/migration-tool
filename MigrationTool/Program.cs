namespace MigrationTool
{
    using System.Collections.Generic;
    using Cassandra;
    using Cassandra.Data.Linq;
    using Cassandra.Mapping;
    using Infrastructure;
    using Model;
    using Reader;

    internal class Program
    {
        private static void Main(string[] args)
        {
            var db = new DatabaseConnection();

            var songs = CsvReader.ReadCsv("data\\classic-rock-song-list.csv");

        }
    }
}