using System.Text.RegularExpressions;

string file = File.ReadAllText(args[0]);
string[] chunks = file.Split("\n\n");
  


var ranges = new List<(long a, long b)>();
var ids = new List<long>();
foreach(Match match in Regex.Matches(chunks[0], "([0-9]+)-([0-9]+)")) {
  ranges.Add((long.Parse(match.Groups[1].Value), long.Parse(match.Groups[2].Value)));
}

foreach(string line in chunks[1].Split('\n', StringSplitOptions.RemoveEmptyEntries)) {
  ids.Add(long.Parse(line)); 
}

ranges = ranges.OrderBy(x => x.a).ToList();

bool IsFresh(long id) {
  foreach((long low, long high) in ranges) {
    if (low <= id && high >= id) {
      return true;
    }
  }
  return false;
}

long sum = 0; // Include extra from initial range
long highest = 0;
foreach((long low, long high) in ranges) {
  if (high > highest) {
    sum += high - Math.Max(low, highest) + (low > highest ? 1 : 0);
    highest = high;
  }
}
Console.WriteLine(sum);

