namespace MigrationToolV2.Infrastructure
{
    using System.Collections.Generic;
    using Cassandra;
    using Cassandra.Data.Linq;
    using Cassandra.Mapping;
    using Model;

    public class DatabaseConnection
    {
        public DatabaseConnection()
        {
            MappingConfiguration.Global.Define<RockSongMapping>();

            var cluster = Cluster.Builder()
                .AddContactPoint("localhost")
                .WithPort(9042)
                .Build();

            this.Session = cluster.Connect();

            this.Session.CreateKeyspaceIfNotExists("rock_songs", new Dictionary<string, string>
            {
                {"class", "SimpleStrategy"},
                {"replication_factor", "1"}
            });

            this.Session.ChangeKeyspace("rock_songs");

            this.Session.GetTable<RockSong>().CreateIfNotExists();

            this.Mapper = new Mapper(this.Session);
        }

        public ISession Session { get; set; }

        public IMapper Mapper { get; set; }
    }
}