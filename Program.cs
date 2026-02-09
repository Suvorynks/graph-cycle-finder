using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

class Program
{
    // --- АНІМАЦІЯ СМАЙЛИКА ---
    static void ShowIntro()
    {
        Console.Clear();
        Console.CursorVisible = false;
        Console.ForegroundColor = ConsoleColor.Yellow;

        string[] frameOpen =
        {
            @"    .--------.    ",
            @"   /          \   ",
            @"  |   O    O   |  ",
            @"  |     ^      |  ",
            @"  |   \____/   |  ",
            @"   \          /   ",
            @"    '--------'    "
        };

        string[] frameWink =
        {
            @"    .--------.    ",
            @"   /          \   ",
            @"  |   >    O   |  ",
            @"  |     ^      |  ",
            @"  |   \____/   |  ",
            @"   \          /   ",
            @"    '--------'    "
        };

        int leftPadding = 5;

        for (int i = 0; i < 3; i++)
        {
            Console.SetCursorPosition(0, 2);
            foreach (string line in frameOpen)
            {
                Console.WriteLine(new string(' ', leftPadding) + line);
            }
            Console.WriteLine(new string(' ', leftPadding) + "   Привіт! :D");
            Thread.Sleep(600);

            Console.SetCursorPosition(0, 2);
            foreach (string line in frameWink)
            {
                Console.WriteLine(new string(' ', leftPadding) + line);
            }
            Console.WriteLine(new string(' ', leftPadding) + "   Привіт! ;) ");
            Thread.Sleep(250);
        }

        Thread.Sleep(500);
        Console.ResetColor();
        Console.CursorVisible = true;
        Console.Clear();
    }

    // --- ВВЕДЕННЯ ---
    static int ReadInt(string message, int? min = null, int? max = null)
    {
        while (true)
        {
            Console.Write(message);
            string s = Console.ReadLine();

            if (int.TryParse(s, out int value))
            {
                if (min.HasValue && value < min.Value)
                {
                    Console.WriteLine($"Значення повинно бути ≥ {min.Value}");
                    continue;
                }
                if (max.HasValue && value > max.Value)
                {
                    Console.WriteLine($"Значення повинно бути ≤ {max.Value}");
                    continue;
                }
                return value;
            }
            Console.WriteLine("Введіть ціле число!");
        }
    }

    static int[,] ReadMatrixFromFile(string path)
    {
        string[] lines = File.ReadAllLines(path);
        // Видаляємо порожні рядки
        lines = Array.FindAll(lines, line => !string.IsNullOrWhiteSpace(line));

        // Перевірка на порожній файл
        if (lines.Length == 0)
        {
            throw new Exception("Файл порожній! Будь ласка, перевірте файл або оберіть інший.");
        }

        int n = lines.Length;
        int[,] A = new int[n, n];

        for (int i = 0; i < n; i++)
        {
            var parts = lines[i].Trim().Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length != n)
                throw new Exception($"Рядок {i + 1} містить {parts.Length} чисел. Очікувалось {n}.");

            for (int j = 0; j < n; j++)
            {
                int val = int.Parse(parts[j]);
                if (val != 0 && val != 1)
                    throw new Exception($"Помилка: елемент [{i + 1},{j + 1}] = {val}. Дозволено тільки 0 та 1!");
                A[i, j] = val;
            }
        }
        return A;
    }

    static int[,] ReadMatrixInteractive(int n)
    {
        Console.WriteLine($"\nВведіть матрицю суміжності {n}×{n} (тільки 0 і 1):");
        int[,] A = new int[n, n];

        for (int i = 0; i < n; i++)
        {
            while (true)
            {
                Console.Write($"Рядок {i + 1}: ");
                string input = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(input)) continue;

                var parts = input.Trim().Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length != n)
                {
                    Console.WriteLine($"Потрібно {n} чисел!");
                    continue;
                }

                bool ok = true;
                for (int j = 0; j < n; j++)
                {
                    if (!int.TryParse(parts[j], out A[i, j]) || (A[i, j] != 0 && A[i, j] != 1))
                    {
                        Console.WriteLine("Елементи мають бути 0 або 1.");
                        ok = false;
                    }
                }
                if (ok) break;
            }
        }
        return A;
    }

    static void CheckForLoops(int[,] A)
    {
        int n = A.GetLength(0);
        bool hasLoop = false;
        StringBuilder loopInfo = new StringBuilder();

        for (int i = 0; i < n; i++)
        {
            if (A[i, i] == 1)
            {
                hasLoop = true;
                loopInfo.Append($"{i + 1} ");
            }
        }

        if (hasLoop)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\n!!!  УВАГА: Цей граф містить петлі!");
            Console.WriteLine($"   Вершини з петлями: {loopInfo}");
            Console.ResetColor();
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\n✓ Граф простий (без петель).");
            Console.ResetColor();
        }
    }

    // --- ОБЧИСЛЕННЯ ---
    static int[,] Multiply(int[,] A, int[,] B)
    {
        int n = A.GetLength(0);
        int[,] C = new int[n, n];
        for (int i = 0; i < n; i++)
            for (int k = 0; k < n; k++)
                if (A[i, k] != 0)
                    for (int j = 0; j < n; j++)
                        C[i, j] += A[i, k] * B[k, j];
        return C;
    }

    static int[,] MatrixPower(int[,] A, int p)
    {
        int n = A.GetLength(0);
        int[,] result = new int[n, n];
        for (int i = 0; i < n; i++) result[i, i] = 1;

        int[,] baseM = (int[,])A.Clone();
        while (p > 0)
        {
            if ((p & 1) == 1) result = Multiply(result, baseM);
            baseM = Multiply(baseM, baseM);
            p >>= 1;
        }
        return result;
    }

    static void PrintMatrix(int[,] M)
    {
        int n = M.GetLength(0);
        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
                Console.Write(M[i, j] + " ");
            Console.WriteLine();
        }
    }

    // --- ПОШУК ТА ВИВІД МАРШРУТІВ ---
    static void PrintAllCycles(int[,] adj, int startNodeZeroBased, int k)
    {
        Console.WriteLine("\nСписок знайдених маршрутів:");
        List<int> currentPath = new List<int>();
        currentPath.Add(startNodeZeroBased);

        int foundCount = 0;

        FindPathsRecursive(adj, startNodeZeroBased, startNodeZeroBased, k, currentPath, ref foundCount);

        if (foundCount == 0)
        {
            Console.WriteLine("  (Маршрутів не знайдено)");
        }
    }

    static void FindPathsRecursive(int[,] adj, int current, int target, int stepsLeft, List<int> path, ref int count)
    {
        if (stepsLeft == 0)
        {
            if (current == target)
            {
                count++;
                string route = string.Join(" -> ", path.Select(v => v + 1));
                Console.WriteLine($"  {count}. {route}");
            }
            return;
        }

        int n = adj.GetLength(0);
        for (int next = 0; next < n; next++)
        {
            if (adj[current, next] == 1)
            {
                path.Add(next);
                FindPathsRecursive(adj, next, target, stepsLeft - 1, path, ref count);
                path.RemoveAt(path.Count - 1);
            }
        }
    }

    // --- MAIN ---
    static void Main()
    {
        Console.InputEncoding = Encoding.UTF8;
        Console.OutputEncoding = Encoding.UTF8;

        ShowIntro();

        while (true)
        {
            Console.Clear();
            Console.WriteLine("=== Добрий день) \r\n Ця програма створена для: \r\n знаходження кількості циклів у графі, що заданий матрицею суміжності === \r\n (Пошук циклів та маршрутів)");
            Console.WriteLine("Введіть потрібний вам варіант, звідки мені брати матрицю");

            Console.WriteLine("1 — Ввести матрицю вручну");
            Console.WriteLine("2 — Завантажити матрицю з файлу");

            Console.Write("\nВаш вибір: ");
            string choice = Console.ReadLine().Trim();
            int[,] A = null;

            if (choice == "2")
            {
                Console.WriteLine("\n[Порада] Щоб скопіювати шлях: Shift + Правий клік -> Копіювати як шлях");
                // Ось тут ми говоримо користувачу про 0
                Console.WriteLine("(Введіть 0, щоб повернутися назад)");

                while (true)
                {
                    Console.Write("Введіть шлях до файлу: ");
                    string path = Console.ReadLine().Trim().Trim('"');

                    // ПЕРЕВІРКА НА "0"
                    if (path == "0") break;

                    if (!File.Exists(path))
                    {
                        Console.WriteLine("Файл не знайдено! Спробуйте ще раз.");
                        continue;
                    }

                    try
                    {
                        A = ReadMatrixFromFile(path);
                        Console.WriteLine("\nМатрицю завантажено:");
                        PrintMatrix(A);
                        break;
                    }
                    catch (Exception ex)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Помилка файлу: " + ex.Message);
                        Console.ResetColor();
                    }
                }
                if (A == null) continue;
            }
            else
            {
                int n = ReadInt("Введіть кількість вершин n: ", 1);
                A = ReadMatrixInteractive(n);
            }

            CheckForLoops(A);
            int size = A.GetLength(0);
            bool workingWithCurrentGraph = true;

            while (workingWithCurrentGraph)
            {
                Console.WriteLine("\n" + new string('-', 40));

                int vertex = ReadInt($"Введіть номер вершини (1..{size}): ", 1, size);
                int k = ReadInt("Введіть довжину циклу k: ", 1);

                int[,] Ak = MatrixPower(A, k);
                int result = Ak[vertex - 1, vertex - 1];

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"\n>>> Загальна кількість циклів: {result}");
                Console.ResetColor();

                if (result > 0)
                {
                    if (result > 1000)
                        Console.WriteLine("Маршрутів занадто багато для виводу.");
                    else
                        PrintAllCycles(A, vertex - 1, k);
                }

                Console.WriteLine("\nОберіть дію:");
                Console.WriteLine("1 — Продовжити роботу з цим графом");
                Console.WriteLine("2 — Ввести новий граф");
                Console.WriteLine("3 — Вийти з програми");
                Console.Write("Ваш вибір: ");

                while (true)
                {
                    var key = Console.ReadKey(intercept: true);

                    if (key.KeyChar == '1')
                    {
                        Console.WriteLine("1");
                        break;
                    }
                    else if (key.KeyChar == '2')
                    {
                        Console.WriteLine("2");
                        workingWithCurrentGraph = false;
                        break;
                    }
                    else if (key.KeyChar == '3')
                    {
                        Console.WriteLine("3");
                        return;
                    }
                }
            }
        }
    }
}