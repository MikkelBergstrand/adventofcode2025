string[] lines = File.ReadAllLines(args[0]);

void ProblemA(){
  var numbers = new List<List<int>>();
  foreach(string line in lines.SkipLast(1)) {
    numbers.Add(line.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(x => int.Parse(x)).ToList());
  }

  List<char> operators = lines.Last().Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(x => x[0]).ToList();

  long result = 0;

  for(int i = 0; i < numbers[0].Count(); i++) {
    long initial = numbers[0][i];
    for(int j = 1; j < numbers.Count(); j++) {
      if (operators[i] == '*') {
        initial *= numbers[j][i];
      } else {
        initial += numbers[j][i];
      }
    }
    result += initial;
  }
  Console.WriteLine(result);
}

// Part B)

// Use the position of the operator to
// determine dimensions of the columns.
(int index, char op)[] operators = lines.Last().Index().Where(x => x.Item != ' ').ToArray();
long result = 0;

string[] numbers = lines[0..(lines.Length-1)];
for(int i = 0; i < operators.Length; i++) {
  char op = operators[i].op;
  // Determine low and high index of column. 
  // It ends either when the line ends or where the next column starts.
  int low = operators[i].index;
  int high = i == operators.Length - 1 ? lines[0].Length-1 : operators[i+1].index-2;

  long next = 0;
  for(int dpos = high; dpos >= low; dpos--) {
    // Capture all numbers along column index dpos, filter out blank characters,
    // concat to a string and then convert to a number.
    long number = long.Parse(new string(numbers.Select(no => no[dpos]).Where(no => no != ' ').ToArray()));
  
    if (next == 0) {
      next = number; 
    } else if(op == '*') {
      next *= number;
    } else {
      next += number;
    }
  }
  result += next;
}
Console.WriteLine($"b): {result}");
