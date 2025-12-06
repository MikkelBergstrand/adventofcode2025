namespace common {

public class Map<T> : IEnumerable<(int x, int y, T data)> {

  private T[][] mapData;
  private int width;
  private int height;

  public Map(string[] data, Func<char, T> mappingFunction) {
    height = data.Length;
    width = data[0].Length;

    this.mapData = data.Select(line => line.Select(c => mappingFunction(c)).ToArray()).ToArray();
  }

  public IEnumerator<(int x, int y, T data)> GetEnumerator()
  {
    foreach ((int y, T[] arr)  in mapData.Index()) {
      foreach ((int x, T data) in arr.Index()) {
        yield return (x, y, data);
      }
    }
  }

  // Cursed
  System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();

  public override string ToString() {
    string s = ""; 
    foreach (T[] line in mapData) {
      s += (string.Join("", line)) + "\n";
    }
    return s;
  }

  public bool InBounds(int x, int y) {
    return x >= 0 && y >= 0 && x < this.width && y < this.height;
  }

  public T Get(int x, int y) {
    return mapData[y][x]; 
  }

  public void Set(int x, int y, T val) {
    mapData[y][x] = val;
  }

  private static (int x, int y)[] neighbors = {
    (1, 0), (-1, 0), (0, 1), (0, -1),
    (-1, -1), (1, -1), (-1, 1), (1, 1)
  };

  public List<T> GetNeighbors(int x, int y) {

    var ret = new List<T>(); 

    foreach (var neighbor in neighbors) {
      (int x, int y) newPos = (x+neighbor.x, y+neighbor.y);

      if (!this.InBounds(newPos.x, newPos.y))
        continue;
      
      ret.Add(this.Get(newPos.x, newPos.y));
    }
    return ret;
  }
}

}
