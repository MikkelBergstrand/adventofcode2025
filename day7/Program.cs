using common;


var map = new Map<char>(File.ReadAllLines(args[0]), c => c);

var start_pos = map.Where(x => x.data == 'S').First();
int y = start_pos.y;
var xBeams = new Dictionary<int, long>{ {start_pos.x, 1} };
int splits = 0; 


while(++y < map.Height() && xBeams.Count() > 0)  {
  var newBeams = new Dictionary<int, long>();
  foreach(var (x, l) in xBeams) {
    if(map.Get(x, y) == '^') {
      splits++;
      if(x != 0) 
        newBeams[x-1] = l + newBeams.GetValueOrDefault(x-1, 0);
      if(x != map.Width()-1) 
        newBeams[x+1] = l + newBeams.GetValueOrDefault(x+1, 0);
    } else {
      newBeams[x] =  newBeams.GetValueOrDefault(x, 0) + l;
    }
  }
  xBeams = newBeams;
}

Console.WriteLine($"Splits: {splits}");
Console.WriteLine($"Timelines: {xBeams.Values.Sum()}");
