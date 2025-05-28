using System.Text;

namespace TagStorage.Terminal;

public class InteractiveInput<T>(Func<string, IEnumerable<T>> onChange, Func<T, string> printTransform, string prompt = "> ")
{
    private const int max_display_lines = 10;

    private readonly StringBuilder inputBuffer = new StringBuilder();
    private int cursorLeft = 0;

    private IEnumerable<T> changed = [];
    private IEnumerable<string> changedLines = [];
    private int lineStart = 0;

    private void invokeChange()
    {
        lineStart = 0;
        changed = onChange(inputBuffer.ToString());
        changedLines = changed.Select(printTransform);
    }

    public T? ReadInput()
    {
        while (true)
        {
            string readInput = inputBuffer.ToString();

            Console.Clear();
            Console.WriteLine($"{prompt}{readInput}");

            foreach (string line in changedLines.Skip(lineStart).Take(10))
            {
                Console.WriteLine(line);
            }

            Console.SetCursorPosition(prompt.Length + cursorLeft, 0);

            ConsoleKeyInfo key = Console.ReadKey(true);

            if (key.Key == ConsoleKey.Enter)
            {
                Console.Clear();
                return changed.Skip(lineStart).FirstOrDefault();
            }

            if (key.Key == ConsoleKey.Backspace)
            {
                if (cursorLeft > 0)
                {
                    inputBuffer.Remove(--cursorLeft, 1);
                    invokeChange();
                }

                continue;
            }

            if (key.Key == ConsoleKey.Delete)
            {
                if (cursorLeft < inputBuffer.Length)
                {
                    inputBuffer.Remove(cursorLeft, 1);
                    invokeChange();
                }

                continue;
            }

            if (key.Key == ConsoleKey.LeftArrow)
            {
                if (cursorLeft > 0)
                {
                    cursorLeft--;
                }

                continue;
            }

            if (key.Key == ConsoleKey.RightArrow)
            {
                if (cursorLeft < inputBuffer.Length)
                {
                    cursorLeft++;
                }

                continue;
            }

            if (key.Key == ConsoleKey.DownArrow)
            {
                if (lineStart + max_display_lines < changedLines.Count())
                {
                    lineStart++;
                }

                continue;
            }

            if (key.Key == ConsoleKey.UpArrow)
            {
                if (lineStart > 0)
                {
                    lineStart--;
                }

                continue;
            }

            if (char.IsAscii(key.KeyChar))
            {
                inputBuffer.Insert(cursorLeft++, key.KeyChar);
                invokeChange();
            }
        }
    }
}
