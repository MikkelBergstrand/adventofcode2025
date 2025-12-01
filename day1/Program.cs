using System.Text.RegularExpressions;

class Program {

  static int CountZeros(int org, int offset) {
    if (offset > 0) {
      return (offset + org) / 100;
    } else {
      return -(offset + org + (org == 0 ? 0 : -100)) / 100;
    }
  }

  static void Main(string[] args) {
    string[] lines = File.ReadAllLines(args[0]);
    int sum = 50;
    int zeroCount = 0;
    foreach(string line in lines){
      Match match = Regex.Match(line, @"([L|R])([0-9]+)");

      int dir = match.Groups[1].Value == "L" ? -1 : 1;
      int val = int.Parse(match.Groups[2].Value);

      int next = (((sum + dir*val) % 100) + 100) % 100;
      zeroCount += CountZeros(sum, dir*val);

      sum = next;
    }
    Console.WriteLine(zeroCount);
  }
}
