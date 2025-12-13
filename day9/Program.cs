
struct Point {
  public int x;
  public int y;

  public Point(int x, int y) {
    this.x = x;
    this.y = y;
  }
}

class Wall {
  public Point start;
  public Point end;
  public Point normal; // Normal vector, points into the shape (towards green tiles)


  public bool Vertical() {
    return start.x == end.x;
  }
  public bool Horizontal() {
    return start.y == end.y;
  }
}


public class Program {
  public static void main(string[] args) {
    (int x, int y)[] tiles = File.ReadAllLines(args[0]).Select(x => x.Split(',').Select(y => int.Parse(y)).ToArray()).Select(x => (x[0], x[1])).ToArray();

    // Create walls
    int leftMostX = int.MaxValue;
    int leftIndex = -1;
    List<Wall> walls = new List<Wall>();
    for(int i = 0; i < tiles.Length; i++) {
      int next = (i + 1) % tiles.Length;
      var wall = new Wall{ 
        start = new Point { x = tiles[i].x, y = tiles[i].y }, 
        end   = new Point { x = tiles[next].x, y = tiles[next].y } 
      };
      
      if (wall.Vertical() && wall.start.x < leftMostX) {
        leftMostX = wall.start.x;
        leftIndex = i;
      }
    }

    // Compute normals
    Console.WriteLine(walls[leftIndex]);
    walls[leftIndex].normal = new Point(1, 0);

    for(int i = leftIndex; i <  walls.Count(); i++) {
      int next = (i + 1) % walls.Count();
      if (walls[i].Horizontal() == walls[next].Horizontal()) {
        walls[next].normal = walls[i].normal;
      }
    }

    for(int i = leftIndex; i <  walls.Count(); i++) {
      int next = (i + 1) % walls.Count(); 
      // Find a corner: ignore if not a corner.
      if(walls[i].Horizontal() != walls[next].Horizontal()) {
        // Look for diagonal matches 
        Point origin = walls[i].end;
        
      }
      
    }
  }
}





