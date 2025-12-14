
struct Point {
  public int x;
  public int y;

  public Point(int x, int y) {
    this.x = x;
    this.y = y;
  }

  public static Point operator+(Point a, Point b) {
    return new Point(a.x + b.x, a.y + b.y);
  }

  public static Point operator-(Point a, Point b) {
    return new Point(a.x - b.x, a.y - b.y);
  }

  public static bool operator==(Point a, Point b) {
    return a.x == b.x && a.y == b.y;
  }

  public static bool operator!=(Point a, Point b) { return !(a == b); }
}

class Tile {
  public Point location;
  public Point normal;
  public bool restrictive;


  public Tile(Point location) {
    this.location = location;
  }

  public static long Area(Tile a, Tile b) {
    return ((long)Math.Abs(a.location.x - b.location.x)+1) *
      ((long)Math.Abs(a.location.y - b.location.y)+1);
  }
}

public struct Wall {
  public int coordinate; 
  public int start;
  public int end;
}

public class Program {
  static int mod(int x, int m) {
      return (x%m + m)%m;
  }

  static bool inRange(int x, int low, int high) {
    return x > low && x < high;
  }

  public static void Main(string[] args) {
    Tile[] tiles = File.ReadAllLines(args[0])
      .Select(x => x.Split(',')
      .Select(y => int.Parse(y)).ToArray())
      .Select(x => new Tile(new Point(x[0], x[1]))).ToArray();

    // Locate the left-most vertical wall, 
    // This is so we are able to initially classify what is the interior
    // and the exterior of the enclosed shape.
    int leftMostX = int.MaxValue;
    int leftIndex = -1;

    var horizontalWalls = new List<Wall>();
    var verticalWalls = new List<Wall>();

    for(int i = 0; i <= tiles.Length; i++) {
      int prev = i % tiles.Length;
      int next = (i + 1) % tiles.Length;
      
      if (tiles[prev].location.x == tiles[next].location.x && tiles[prev].location.x < leftMostX) {
        leftMostX = tiles[prev].location.x;
        leftIndex = i;
      }
      
      if(tiles[prev].location.x == tiles[next].location.x) {
        verticalWalls.Add(new Wall{
          coordinate = tiles[prev].location.x,
          start = Math.Min(tiles[prev].location.y, tiles[next].location.y),
          end = Math.Max(tiles[prev].location.y, tiles[next].location.y),
        });
      } else {
        horizontalWalls.Add(new Wall{
          coordinate = tiles[prev].location.y,
          start = Math.Min(tiles[prev].location.x, tiles[next].location.x),
          end = Math.Max(tiles[prev].location.x, tiles[next].location.x),
        });
      }
    }


    // Start at the leftmost red tile that is a part of a vertical segment, so we know
    // the normal vector must point to the right
    int normalIndex = 0; 
    bool prevHoriz = false;

    Point[] normals = new Point[]{
     new Point(1, 0), 
     new Point(0, 1), 
     new Point(-1, 0), 
     new Point(0, -1), 
    };

    for(int i = 0; i < tiles.Count()+1; i++) {
      int prev = (i + leftIndex ) % tiles.Count();
      int next = (i + leftIndex + 1) % tiles.Count();

      Console.WriteLine(tiles[prev].location);
      bool horiz = tiles[next].location.y == tiles[prev].location.y;
      Point normal = normals[normalIndex];
      if (horiz == prevHoriz) {
        tiles[prev].normal = normals[normalIndex];
      } else {
        // Determine next direction  
        int diry = Math.Sign(tiles[next].location.y - tiles[prev].location.y); 
        int dirx = Math.Sign(tiles[next].location.x - tiles[prev].location.x); 

        if((diry != 0 && diry == normal.y) || (dirx != 0 && dirx == normal.x )) {
          normalIndex = mod(normalIndex + 1, normals.Length);
          tiles[prev].restrictive = true;
        } else {
          normalIndex = mod(normalIndex - 1, normals.Length);
        }
        // Corner - combination of the previous and the next normal
        tiles[prev].normal = normal + normals[normalIndex];
      }
      
      prevHoriz = horiz;
    }

    long maxArea = 0;
    for(int i = 0; i < tiles.Length; i++) {
      Tile tile = tiles[i];
      var candidates = tiles[(i+1)..].AsEnumerable();
      if(tile.restrictive) {
        candidates = candidates.Where(t => { 
            var diff = (tile.location - t.location); 
            return  
              (Math.Sign(diff.x) != tile.normal.x && Math.Sign(diff.y) != tile.normal.y);
        });
        candidates = candidates.Where(t => { 
          return (t.restrictive && (t.normal.x == -tile.normal.x && t.normal.y == -tile.normal.y)) || 
                (!t.restrictive && !(t.normal.x == tile.normal.x && t.normal.y == tile.normal.y)); 
        });
      } else if(tile.normal.x == 0 || tile.normal.y == 0) {
        candidates = candidates.Where(t => {
          var diff = (t.location - tile.location); 
          return diff.x == 0 || diff.y == 0 && 
            (tile.normal.y == 0 && tile.normal.x == Math.Sign(diff.x)) ||
            (tile.normal.x == 0 && tile.normal.y == Math.Sign(diff.y));
        }).Where(t => {
          return (t.restrictive && (tile.normal.x != 0 && tile.normal.x == -t.normal.x) && (tile.normal.y != 0 && tile.normal.y == -t.normal.y)) || 
          !t.restrictive;
        });
      } else {
        candidates = candidates.Where(t =>{
            var diff = (tile.location - t.location);
            return
              !(Math.Sign(diff.x) == -tile.normal.x && Math.Sign(diff.y) == -tile.normal.y);
        });
        candidates = candidates.Where(t => { 
          return !(t.normal.x == tile.normal.x && t.normal.y == tile.normal.y);
        });
      }

      foreach(Tile candidate in candidates) {
        long candidateArea = Tile.Area(tile, candidate);
        if (candidateArea > maxArea) {

          // Initially check if any tiles intersect, not counting the edge
          int lbx = Math.Min(tile.location.x, candidate.location.x);
          int ubx = Math.Max(tile.location.x, candidate.location.x);
          int lby = Math.Min(tile.location.y, candidate.location.y);
          int uby = Math.Max(tile.location.y, candidate.location.y);

          var horizIntersects = horizontalWalls.Where(w => {
              return w.coordinate > lby && w.coordinate < uby &&
                Math.Max(lbx, w.start) < Math.Min(ubx, w.end);
              });

          var vertIntersects = verticalWalls.Where(w => {
              return w.coordinate > lbx && w.coordinate < ubx &&
                Math.Max(lby, w.start) < Math.Min(uby, w.end);
              });

          if (!horizIntersects.Any() && !vertIntersects.Any()) {
            maxArea = candidateArea;
          }
        }
      }
    }
    Console.WriteLine(maxArea);
  }
}





