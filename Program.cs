using System.CommandLine;

var seedOption = new Option<int?>(["--seed", "-s"], "Seed for the random string");
var lengthOption = new Option<int>(["--length", "-l"], "Length of the random string")
{
    IsRequired = true,
};
var alphabetOption = new Option<Alphabet>(
    aliases: ["--alphabet", "-a"],
    description: "Character sets to be used in the random string.",
    parseArgument: result => Alphabet.From(result.Tokens.Single().Value)
)
{
    IsRequired = true,
};

var rootCommand = new RootCommand { seedOption, lengthOption, alphabetOption };
rootCommand.SetHandler(
    (seed, length, alphabet) =>
    {
        var random = seed.HasValue ? new Random(seed.Value) : new Random();
        var charSet = alphabet.Chars();

        var randomString = new string(
            Enumerable.Range(0, length).Select(_ => charSet[random.Next(charSet.Length)]).ToArray()
        );

        Console.WriteLine(randomString);
    },
    seedOption,
    lengthOption,
    alphabetOption
);

return await rootCommand.InvokeAsync(args);

record Alphabet(bool UseUpperCase, bool UseLowerCase, bool UseDigits, char[] ExtraCharacters)
{
    public static Alphabet From(string pattern)
    {
        var useUpperCase = false;
        var useLowerCase = false;
        var useDigits = false;
        var extraCharacters = new HashSet<char>();

        foreach (var c in pattern)
        {
            if (char.IsAsciiLetterLower(c))
            {
                useLowerCase = true;
                continue;
            }
            if (char.IsAsciiLetterUpper(c))
            {
                useUpperCase = true;
                continue;
            }
            if (char.IsAsciiDigit(c))
            {
                useDigits = true;
                continue;
            }
            extraCharacters.Add(c);
        }

        return new Alphabet(useUpperCase, useLowerCase, useDigits, [.. extraCharacters]);
    }

    public char[] Chars()
    {
        var upperCase = UseUpperCase ? Enumerable.Range('A', 26).Select(c => (char)c) : [];
        var lowerCase = UseLowerCase ? Enumerable.Range('a', 26).Select(c => (char)c) : [];
        var digits = UseDigits ? Enumerable.Range('0', 10).Select(c => (char)c) : [];

        var allCharacters = upperCase.Concat(lowerCase).Concat(digits).Concat(ExtraCharacters);

        return allCharacters.ToArray();
    }
}
