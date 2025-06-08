using System;
using RPGTurnBased.Core;

namespace RPGTurnBased
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            try
            {
                using var game = new Game1();
                game.Run();
            }
            catch (Exception ex)
            {
                // Log error to console for debugging
                Console.WriteLine($"Error starting game: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");

                // Wait for user input before closing console
                Console.WriteLine("\nPress any key to close...");
                Console.ReadKey();
            }
        }
    }
}