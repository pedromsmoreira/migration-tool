namespace MigrationToolV2
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Threading.Tasks.Dataflow;
    using Cassandra;
    using Cassandra.Mapping;
    using Infrastructure;
    using Model;
    using Reader;

    internal class Program
    {
        private static void Main(string[] args)
        {
            MainAsync(args).Wait();
        }

        private static async Task MainAsync(string[] args)
        {
            var db = new DatabaseConnection();
            
            var queue = new BufferBlock<IList<RockSong>>(new DataflowBlockOptions { BoundedCapacity = 5 });

            var consumerOptions = new ExecutionDataflowBlockOptions { BoundedCapacity = 1 };

            var consumer1 = new ActionBlock<IList<RockSong>>((songList) =>
            {
                var options = new CqlQueryOptions().SetConsistencyLevel(ConsistencyLevel.LocalOne);
                foreach (var rockSong in songList)
                {
                    db.Mapper.Insert<RockSong>(rockSong, options);}

                Console.WriteLine($"Consumer 1 | pagedList count: {songList.Count}");
            }, consumerOptions);

            var consumer2 = new ActionBlock<IList<RockSong>>((songList) =>
            {
                var options = new CqlQueryOptions().SetConsistencyLevel(ConsistencyLevel.LocalOne);
                foreach (var rockSong in songList)
                {
                    db.Mapper.Insert<RockSong>(rockSong, options);
                }

                Console.WriteLine($"Consumer 2 | pagedList count: {songList.Count}");
            }, consumerOptions);

            var consumer3 = new ActionBlock<IList<RockSong>>((songList) =>
            {
                var options = new CqlQueryOptions().SetConsistencyLevel(ConsistencyLevel.LocalOne);
                foreach (var rockSong in songList)
                {
                    db.Mapper.Insert<RockSong>(rockSong, options);
                }

                Console.WriteLine($"Consumer 3 | pagedList count: {songList.Count}");
            }, consumerOptions);

            var linkoptions = new DataflowLinkOptions { PropagateCompletion = true };

            queue.LinkTo(consumer1, linkoptions);
            queue.LinkTo(consumer2, linkoptions);
            queue.LinkTo(consumer3, linkoptions);

            Console.WriteLine("Start Producing");
            var start = DateTime.UtcNow;

            await ProduceAsync(queue).ConfigureAwait(false);

            await Task.WhenAll(consumer1.Completion, consumer2.Completion, consumer3.Completion);

            Console.WriteLine($"Duration: {DateTime.UtcNow - start}");

            Console.ReadLine();
        }

        private static async Task ProduceAsync(ITargetBlock<IList<RockSong>> queue)
        {
            var songs = CsvReader.ReadCsv("data\\classic-rock-song-list.csv");
            var totalSongs = songs.Count();
            var pageSize = 100;

            var pages = totalSongs / pageSize;
            for (int i = 0; i <= pages; i++)
            {
                Console.WriteLine($"Produce Page: {i}");
                await queue.SendAsync<IList<RockSong>>(songs.Skip(i * pageSize).Take(pageSize).ToList()).ConfigureAwait(false);
            }

            queue.Complete();
        }
    }
}