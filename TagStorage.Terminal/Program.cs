namespace TagStorage.Terminal;

public static class Program
{
	private static IEnumerable<string> onChange(string input)
	{
		return Enumerable.Range(1, input.Length).Select(n => $"{n}{input}");
	}

	public static void Main(string[] args)
	{
		InteractiveInput interactiveInput = new InteractiveInput(onChange, "Enter input: ");
		string input = interactiveInput.ReadInput();
		Console.WriteLine($"You entered: {input}");
	}
}