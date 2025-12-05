string[] lines = File.ReadAllLines(args[0]);

long res = 0;

foreach(string line in lines) {
  int[] nums = line.Select(x => (int)(x - '0')).ToArray();

  int index = 0;
  long num = 0;
  long power = (long)Math.Pow(10, 11);
  for(int digit = 0; digit < 12; digit++) {
    var (idx, val) = nums[index..(nums.Length-(11-digit))].Index().MaxBy(val => val.Item2);
    index += idx+1;
    num += power*val; 
    power /= 10;
  }
  res += num;
}

Console.WriteLine($"Result: {res}");
