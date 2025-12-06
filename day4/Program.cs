using common;

class Program {
  static void Main(string[] args) {
  Map<char> map = new Map<char>(File.ReadAllLines(args[0]), c=>c);

    int initialCount = map.Where(tile => tile.data == '@').Count();
    var canRemove = new List<(int x, int y)>(); 

    do {
      canRemove.Clear();
      foreach((int x, int y, char tile) in map) {
        if (tile == '@' && map.GetNeighbors(x, y).Where(t => t == '@').Count() < 4) {
          canRemove.Add((x, y));
        }
      } 

      canRemove.ForEach(p => map.Set(p.x, p.y, '.'));
    } while(canRemove.Count() > 0);

    Console.WriteLine(initialCount - map.Where(tile => tile.data == '@').Count());
  }
}



