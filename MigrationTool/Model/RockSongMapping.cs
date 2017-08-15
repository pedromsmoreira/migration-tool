namespace MigrationTool.Model
{
    using Cassandra.Mapping;

    public class RockSongMapping : Mappings
    {
        public RockSongMapping()
        {
            this.For<RockSong>()
                .TableName("rock_songs_by_id")
                .PartitionKey("song", "artist")
                .ClusteringKey("release_year")
                .Column(s => s.Song, cn => cn.WithName("song"))
                .Column(s => s.Artist, cn => cn.WithName("artist"))
                .Column(s => s.ReleaseYear, cn => cn.WithName("release_year"))
                .Column(s => s.Combined, cn => cn.WithName("combined"));
        }
    }
}