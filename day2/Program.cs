using System.Text.RegularExpressions;

bool IsRepeatingNumber(long no) {
  string s = no.ToString();
  string doubled = s + s;
  return doubled.Substring(1, doubled.Length-2).Contains(s);
}

List<long> ProcessRange(long a, long b) {
  List<long> ret = new List<long>();

  for(long curr = a; curr <= b; curr++) {
    if (IsRepeatingNumber(curr)) {
      ret.Add(curr);
    }
  }
  return ret;
}

string line = File.ReadAllLines(args[0])[0];
long sum = 0;
foreach (Match m in Regex.Matches(line, @"([0-9]+)\-([0-9]+)")) {
  long a = long.Parse(m.Groups[1].Value);
  long b = long.Parse(m.Groups[2].Value);

  Console.WriteLine((a, b, string.Join(",", ProcessRange(a, b))));
  sum += ProcessRange(a, b).Sum();
  
}

Console.WriteLine($"Sum: {sum}");

