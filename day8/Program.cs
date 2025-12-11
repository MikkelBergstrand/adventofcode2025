struct Box {
  public long x;
  public long y;
  public long z;

  public static long Dist(Box b1, Box b2) {
    long dx = b1.x - b2.x;
    long dy = b1.y - b2.y;
    long dz = b1.z - b2.z;
    return dx*dx + dy*dy + dz*dz;
  }

  public override string ToString() {
    return $"[{x},{y},{z}]";
  }
};

public class Program {
  public static void Main(string[] args) {
    var boxes = File.ReadAllLines(args[0])
      .Select(x => x.Split(',').Select(y => int.Parse(y)).ToArray())
      .Select(box => new Box{ x = box[0], y = box[1], z = box[2] })
      .ToArray();

    // Store all unique connection pairs, and map them to their Eucledian distance
    // Only store (a, b), not (b, a) as well, and a != b
    var pair_to_dist = new Dictionary<(int a, int b), long>();
    for(int i = 0; i < boxes.Length; i++) {
      for (int j = i+1; j < boxes.Length; j++) {
        pair_to_dist[(i, j)] = Box.Dist(boxes[i], boxes[j]);
      }
    }
    pair_to_dist = pair_to_dist.OrderBy(kvp => kvp.Value).ToDictionary();

    // Each circuit is a hashset of box ids
    var circuits = new List<HashSet<int>>();
    var (id, dist) = pair_to_dist.First();
    pair_to_dist.Remove(id);
    circuits.Add(new HashSet<int>{ id.a, id.b });

    while(circuits[0].Count() != boxes.Length) {
      (id, dist) = pair_to_dist.First();
      pair_to_dist.Remove(id);

      // store the first circuit where a connection was found.
      HashSet<int>? found = null; 

      var toRemove = new List<HashSet<int>>();
      foreach(var circuit in circuits) {
        // Check if a circuit contains any boxes in the current box pair.
        if(circuit.Contains(id.a) || circuit.Contains(id.b)) {
          if(found == null) {
            found = circuit; 
          } else {
            // If we are here, we have already found a circuit with a valid connection.
            // Hence, we have two circuits where connection is possible, meaning
            // they must be merged as one circuit.
            found.UnionWith(circuit);
            toRemove.Add(circuit);
          }
          found.Add(id.a);
          found.Add(id.b);
        }
      }
      toRemove.ForEach(x => circuits.Remove(x));

      // No connection was possible: we make a fresh circuit.
      if (found == null) {
        var newCircuit = new HashSet<int>{ id.a, id.b };
        circuits.Add(newCircuit); 
      }
    }

    Console.WriteLine("Part a) " + 
      circuits.Select(hashset => hashset.Count()).OrderBy(x => x).Reverse().Take(3).Aggregate(1, (acc, x) => acc * x)
    );

    Console.WriteLine($"Part b) = {boxes[id.a].x *boxes[id.b].x}");
  }
}

