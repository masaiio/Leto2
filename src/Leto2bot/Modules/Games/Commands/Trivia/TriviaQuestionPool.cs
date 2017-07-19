using Leto2bot.Extensions;
using Leto2bot.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;

namespace Leto2bot.Modules.Games.Trivia
{
    public class TriviaQuestionPool
    {
        public class PokemonNameId
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        private static TriviaQuestionPool _instance;
        public static TriviaQuestionPool Instance { get; } = _instance ?? (_instance = new TriviaQuestionPool());

        private const string questionsFile = "data/trivia_questions.json";
        private const string pokemonMapPath = "data/pokemon/name-id_map4.json";
        private readonly int maxPokemonId;

        private Random rng { get; } = new Leto2Random();
        
        private TriviaQuestion[] pool { get; }
        private ImmutableDictionary<int, string> map { get; }

        static TriviaQuestionPool() { }

        private TriviaQuestionPool()
        {
            pool = JsonConvert.DeserializeObject<TriviaQuestion[]>(File.ReadAllText(questionsFile));
            map = JsonConvert.DeserializeObject<PokemonNameId[]>(File.ReadAllText(pokemonMapPath))
                    .ToDictionary(x => x.Id, x => x.Name)
                    .ToImmutableDictionary();

            maxPokemonId = 721; //xd
        }

        public TriviaQuestion GetRandomQuestion(HashSet<TriviaQuestion> exclude, bool isPokemon)
        {
            if (pool.Length == 0)
                return null;

            if (isPokemon)
            {
                var num = rng.Next(1, maxPokemonId + 1);
                return new TriviaQuestion("Who's That Pokémon?", 
                    map[num].ToTitleCase(),
                    "Pokemon",
                    $@"http://leto2bot.me/images/pokemon/shadows/{num}.png",
                    $@"http://leto2bot.me/images/pokemon/real/{num}.png");
            }
            TriviaQuestion randomQuestion;
            while (exclude.Contains(randomQuestion = pool[rng.Next(0, pool.Length)])) ;

            return randomQuestion;
        }
    }
}
