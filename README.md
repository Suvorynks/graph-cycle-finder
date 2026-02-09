# Graph Theory Tool: Cycle & Route Finder

This advanced C# console application is designed to analyze directed graphs represented by adjacency matrices. It efficiently calculates the total number of cycles and specific routes of a given length $k$ starting from a selected vertex.

### Mathematical Context
The program utilizes the fundamental property of **Adjacency Matrices** ($A$):
* The number of different paths of length $k$ between vertices $i$ and $j$ is equal to the element at row $i$ and column $j$ of the matrix $A^k$.
* If $i = j$, the value $A^k[i, i]$ represents the number of cycles of length $k$ starting and ending at vertex $i$.



This project was developed as part of the discrete mathematics curriculum at **Lviv Polytechnic National University**.

### Key Features
* **Matrix Exponentiation:** Implements an efficient matrix multiplication algorithm to calculate $A^k$.
* **Path Visualization:** Uses a **Recursive Depth-First Search (DFS)** to find and print every unique route of length $k$.
* **Dual Input Modes:** * **Manual:** Interactive console input with strict 0/1 validation.
    * **File I/O:** Ability to load large matrices from text files (supports "Copy as path" formatting).
* **Graph Analysis:** Automatically checks for self-loops and notifies the user if the graph is "simple" or "complex."
* **Enhanced UX:** Features a custom ASCII-art welcome animation and colored console feedback for better readability.
* **Localization Ready:** Full `UTF-8` support for Ukrainian and English character sets.

### Technical Stack
* **Language:** C# (.NET Core)
* **Algorithms:** Matrix Multiplication, Binary Exponentiation, Recursive Backtracking (DFS).
* **Namespace focus:** `System.IO`, `System.Text`, `System.Threading`.

### How to Use
1. Launch the application to see the welcome animation.
2. Select your input method (Manual or File).
3. Specify the vertex and the desired path length $k$.
4. Review the total count and the detailed list of all found routes.
