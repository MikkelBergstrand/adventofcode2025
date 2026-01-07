using System.Text.RegularExpressions;


// Fraction structure to help with integer fraction arithmetic.
// Fractions will automatically *stabilize* meaning:
// - the denomenator can never be zero, only the numerator
// - imperfect fractions will reduce to perfect fractions, e.g. 8/4 -> 2/1 and 2/4 -> 1/2
struct Fraction {

  // Greatest common divisor formula
  public static int Gcd(int a, int b) {
    if(b > a) {
      int temp = a;
      a = b;
      b = temp;
    }

    while(b != 0) {
      int r = a % b;
      a = b;
      b = r;
    }
    return Math.Abs(a);
  }

  // Least common multiple formula
  public static int LCM(int a, int b) {
    return Math.Abs(a*b) / Gcd(a, b);
  }

  public int n; // numerator
  public int d; // denomenator

  public Fraction(int n) { this.n = n; this.d = 1; }
  public Fraction(int n, int d) {
    if (n < 0 && d < 0) {
      n = -n;
      d = -d;
    }

    if(d == 0) throw new Exception($"{n}/{d} denomenator is zero!");
    this.n = n;
    this.d = d;

    if(this.n == 0) 
      this.d = 1;


    // Reduce fraction to perfect it.
    int reducer = Gcd(this.n, this.d);
    while(reducer > 1) {
      this.n /= reducer;
      this.d /= reducer;
      reducer = Gcd(this.n, this.d);
    }
  }

  public int ToInt() { 
    if(!IsInt()) 
      throw new Exception("Do not call ToInt() on non-integer fraction!");
    return n / d; 
  }

  public bool IsInt() { return n % d == 0; }

  public static Fraction operator+(Fraction a, Fraction b) { 
    int lcm = LCM(a.d, b.d);
    int scaleA = lcm / a.d;
    int scaleB = lcm / b.d;
    return new Fraction(a.n*scaleA + b.n*scaleB, lcm);
  }

  public static Fraction operator-(Fraction a, Fraction b) { 
    int lcm = LCM(a.d, b.d);
    int scaleA = lcm / a.d;
    int scaleB = lcm / b.d;
    return new Fraction(a.n*scaleA - b.n*scaleB, lcm);
  }

  public static Fraction operator*(Fraction a, Fraction b) { 
    return new Fraction(a.n*b.n, a.d*b.d);
  }

  public static Fraction operator/(Fraction a, Fraction b) { 
    if(b.n == 0) throw new Exception("Division by zero!");
    return new Fraction(a.n*b.d, a.d*b.n);
  }

  public override string ToString() {
    if (IsInt()) return (n/d).ToString();
    return $"{n}/{d}";
  }
}

class Problem {
  public int Lights { private set; get; }
  public int LightCount { private set; get; }
  public List<int> Toggles { private set; get; }
  public int[] Requirements { private set; get; }

  public Problem(string s) {
    this.Toggles = new List<int>();

    var match = Regex.Match(s, @"\[(.*)\] (.*) \{(.*)\}");

    this.Lights = match.Groups[1].Value.Reverse().Index()
       .Select((i => i.Item == '#' ? (1 << i.Index) : 0)).Sum();
    this.LightCount = match.Groups[1].Value.Length;

    foreach(var toggleMatch in match.Groups[2].Value.Split(' ').Select(x => x[1..^1])) {
      var number = toggleMatch?.ToString()?.Split(',').Select(x => int.Parse(x))
        .Select(x => 1 << (this.LightCount - 1 - x))
        .Sum();
      Toggles.Add(number!.Value);
    }
    
    this.Requirements = match.Groups[3].Value.Split(',').Select(x => int.Parse(x)).ToArray();

  }

  private string PrintLights() {
    string s = "";
    for(int i = LightCount-1; i >= 0; i--) {
      s += ((1 << i) & this.Lights) != 0 ? '#' : '.';
    }
    return s;
  }

  public override string ToString() {
    return "{" + string.Join(",", this.Requirements) + "}";
  }
}

class Program {
  private static int SolveProblem(Problem problem) {
    var neighbors = new PriorityQueue<int, int>();
    var visited = new HashSet<int>();
    neighbors.Enqueue(0, 0);

    int state, priority;
    while(neighbors.TryDequeue(out state, out priority)) {
      if (state == problem.Lights) {
        return priority;
      }
      visited.Add(state); 

      foreach(int toggle in problem.Toggles) {
        int next = state ^ toggle;
        if(visited.Contains(next)) {
          continue;
        }
        neighbors.Enqueue(next, priority+1);
      }
    }
    throw new Exception("No solution was found");
  }


  // Generate a set of which values the current button
  // turns on. Essentially just reverses the mapping done in problem a)
  private static HashSet<int> ToggleSet(int x, int n) {
    var ret = new HashSet<int>();
    while(x > 0) {
      n--;
      if (x % 2 != 0) ret.Add(n);
      x /= 2;
    }
    return ret;
  }


  
  // Helper method to print matrix system
  private static void PrintSystem<T>(T[,] a, T[] b) {
      int m = a.GetLength(0);
      int n = a.GetLength(1);
      for(int i = 0; i < m; i++) {
        for(int j = 0; j < n; j++) {
          Console.Write(a[i, j] + " ");
        }
        Console.Write("| " + b[i]);
        Console.WriteLine();
      }
      Console.WriteLine();
  }

  // Swap to rows in a system of matrices Ax = b
  private static void SwapRows<T>(T[,] a, T[] b, int rowa, int rowb) {
    int m = a.GetLength(0);
    int n = a.GetLength(1);

    for(int i = 0; i < n; i++) {
      T tmp = a[rowa, i];
      a[rowa, i] = a[rowb, i];
      a[rowb, i] = tmp;
    }

    T temp = b[rowa];
    b[rowa] = b[rowb];
    b[rowb] = temp;
  }

  private static int SolveProblemB(Problem problem) {
      var toggleSets = problem.Toggles.Select(x => ToggleSet(x, problem.Requirements.Length)).ToArray();
      
      int m = problem.Requirements.Length;
      int n = toggleSets.Length;
      Fraction[,] a  = new Fraction[m, n];
      for(int i = 0; i < m; i++) {
        for(int j = 0; j < n; j++) {
          a[i, j] = new Fraction(0);
        }
      }
      Fraction[] b = new Fraction[m];
      for(int i = 0; i< problem.Requirements.Length; i++) 
        b[i] = new Fraction(problem.Requirements[i]);

      int[] b_orig = (int[])problem.Requirements.Clone();
      int[,] a_orig = new int[m, n];

     foreach((int index, var toggleSet) in toggleSets.Index()) {
        foreach(int toggle in toggleSet) {
          a[toggle, index] = new Fraction(1);
          a_orig[toggle, index] = 1;
        }
      }

      // Gaussian elimination
      int h = 0; int k = 0; // h = rowIndex, k = columnIndex
      while(h < m && k < n) {

        int i_max = -1;
        double maxVal = 0;
        for(int i = h; i < m; i++) {
          double candidate = Math.Abs((double)a[i,k].n/a[i,k].d);
          if(candidate > maxVal) {
            i_max = i;
            maxVal = candidate; 
          }
        }

        if (i_max == -1){
          k = k + 1;
        } else {
          // Swap rows 
          SwapRows(a, b, h, i_max);
          // Make pivot element one by dividing the row
          Fraction pivot = a[h, k];
          for(int i = 0; i < n; i++) {
            a[h, i] /= pivot;
          }
          b[h] /= pivot;


          //Eliminate below pivot
          for(int i = h+1; i < m; i++){
            // Go down each row, find required factor
            Fraction f = a[i, k];
            for(int j = 0; j < n; j++ ) {
              a[i, j] -= f*a[h, j];
            }
            b[i] -= f*b[h];
          }
          h++; k++;
        }
      }

      // Now to Reduced Row Echelon Form
      for(int row = m-1; row >= 0; row--) {
        int pivotCol = -1;
        for(int col = 0; col < n; col++) {
          if(a[row, col].n != 0) {
              pivotCol = col;
              break;
          }
        }

        //All-zero row
        if(pivotCol == -1) {
          continue; 
        }
        
        // Eliminate above pivot
        for(int u = 0; u < row; u++){
          Fraction f = a[u, pivotCol] / a[row, pivotCol];
          for(int l = 0; l < n; l++) {
            a[u, l] -= f*a[row, l];
          }
          b[u] -= f*b[row];
        }

      }

      // Find free variables (there is more than 1 non-zero row in a column)
      // Also keep track of non-free variables.
      var freeVars = new List<int>();
      var nonFreeVars = new List<(int row, int col)>();
      for(int j = 0; j < n; j++) {
        int nonzeros = 0;
        int row = 0;
        for(int i = 0; i < m; i++) {
          if(a[i, j].n != 0) { 
            nonzeros++; 
            if(nonzeros == 1) {
              row = i;
            }
          }
        }

        if (nonzeros == 1) {
          nonFreeVars.Add((row, j));
        } if (nonzeros >= 2) {
          freeVars.Add(j);
        }
      }


      // No free variables, that means A is almost an identity matrix 
      // of the form Ax = b. Solution is straight-forward.
      if(freeVars.Count() == 0) {
        int sum = 0; 
        foreach(var x in nonFreeVars) {
          sum += b[x.row].ToInt();
        }
        return sum;
      }


      // Non-straight forward: iterate over all possible Permutations
      // of values for the non-free variables. This is not very efficient,
      // but it works.
      int min = int.MaxValue;
      foreach(int[] perm in Permutations(freeVars.Count(), problem.Requirements.Max())) {
        // Keep track of current variable bindings
        var boundVars = new int[n];
        // Assign the free variables from current permutation
        for(int x = 0; x < perm.Length; x++) {
          boundVars[freeVars[x]] = perm[x];
        }

        bool invalid = false;
        foreach(var x in nonFreeVars) {
          // Evaluate non-free var, based on the free var bindings.
          Fraction val = new Fraction(0);
          for(int i = x.col+1; i < n; i++) {
            val -= a[x.row, i] * new Fraction(boundVars[i]);
          }
          val += b[x.row];

          // If value is non-integer or negative, current 
          // permutation cannot be a solution
          if (!val.IsInt()) {
            invalid = true;
            break;
          }
          int intVal = val.ToInt();
          if(intVal < 0) {
            invalid = true;
            break;
          }
          boundVars[x.col] = intVal;
        }

        if(!invalid) {
          min = Math.Min(min, boundVars.Sum());
        }
      }

      if (min == int.MaxValue) 
        throw new Exception("No solution");

      return min;
  }

  // Generate an array of n permutations of all integers in range [0, maxValue] (inclusive)
  static IEnumerable<int[]> Permutations(int n, int maxValue) {
    int[] values = new int[n];
    
    while(true) {
      yield return values;

      int pos = n -1;
      while(pos >= 0) {
        values[pos]++;
        if(values[pos] <= maxValue) 
          break;

        values[pos] = 0;
        pos--;
      }

      if(pos < 0)
        yield break;
    }
  }

  public static void Main(string[] args) {
    var problems =  File.ReadAllLines(args[0]).Select(line => new Problem(line)).ToArray();
    Console.WriteLine(problems.Select(x => SolveProblemB(x)).Sum());
  }
}
