using Display;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;

namespace Life
{
    class Universe
    {
        public int[,] universe { get; private set; }
        public string periodictyType { get; private set; } 
        public List<List<int[]>> memoryGenerations { get; private set; } = new List<List<int[]>>();
        public Grid grid { get; set; }
        public Settings UniverseSettings { get; set; }


        /// <summary>
        /// constructor that creates the universe and grid as well as running the NoInput or Input method to 
        /// set the intial state of the universe
        /// </summary>
        /// <param name="UniverseSettings">the settings of the universe</param>
        public Universe(Settings UniverseSettings)
        {
            periodictyType = "Not Detected";
            universe = new int[UniverseSettings.rows, UniverseSettings.columns];
            grid = new Grid(UniverseSettings.rows, UniverseSettings.columns);
            this.UniverseSettings = UniverseSettings;

            if (UniverseSettings.inputFile == "N/A")
            {
                NoInput();
            }
            else
            {
                Input();
            }
        }
        /// <summary>
        /// Primary purpose is to initalise the window and determine wether stepmode i will be used when 
        /// generating the next generation of the universe. It also sets the grid to complete after each generation
        /// is displayed before displaying if a steady state is detected and finally creating the output 
        /// file if requested 
        /// </summary>
        public void UniverseDisplayer()
        {
            grid.InitializeWindow();

            grid.SetFootnote("Iteration: 0");
            if (UniverseSettings.stepMode != true)
            {
                NoStepMode();
            }
            else if (UniverseSettings.stepMode == true)
            {
                StepMode();
            }
            grid.IsComplete = true;

            grid.Render();


            WaitSpacebar();

            grid.RevertWindow();
            string time = DateTime.Now.ToLongTimeString();
            if (periodictyType == "Not Detected")
            {
                Console.WriteLine($"[{time}] Steady-state not detected...");

            }
            else
            {
                Console.WriteLine(periodictyType);
            }
            if(UniverseSettings.output == true)
            {
                Output();
            }
            Console.WriteLine();
            Console.Write($"[{time}] ");
            Spacebar("close");

        }
        /// <summary>
        /// Writes the final generation to an output file in the seed version2 format
        /// </summary>
        public void Output()
        {
            StreamWriter writer = new StreamWriter(UniverseSettings.outputFile);
            writer.WriteLine("#version=2.0");
            int intLastArray = memoryGenerations.Count - 1;
            for (int i = 0;i< memoryGenerations[intLastArray].Count;i++)
            {
                writer.WriteLine($"(o) cell: {memoryGenerations[intLastArray][i][0]}" +
                    $", {memoryGenerations[intLastArray][i][1]}");

            }
            writer.Close();



        }
        /// <summary>
        /// If the settings doesnt specify a seed file then the cells are set to alive (1) or dead (0) based on the 
        /// random factor in settings
        /// </summary>
        public void NoInput()
        {
            for (int row = 0; row < universe.GetLength(0); row++)
            {
                for (int column = 0; column < universe.GetLength(1); column++)
                {
                    Random random = new Random();
                    float randomNumber = random.Next(0, 100);
                    if (randomNumber <= (UniverseSettings.randomFactor * 100))
                    {

                        universe[row, column] = 1;
                    }
                }
            }

        }

        /// <summary>
        /// If the settings have a seed file as an input this function populates the universe with alive (1) and 
        /// dead (0) cells based on a file
        /// </summary>
        public void Input()
        {

            Console.WriteLine(UniverseSettings.inputFile);
            TextReader reader = new StreamReader(UniverseSettings.inputFile);
            string line = reader.ReadLine();
            if (line.Contains("#version=1.0"))
            {
                try
                {
                    Version1 intitalState = new Version1(reader, line, universe);
                }
                catch (Exception e)
                {
                    Console.WriteLine("File Error: {0}", e.Message);
                }
            }
            else if (line.Contains("#version=2.0"))
            {
                try
                {
                    Version2 intitalState = new Version2(reader, line, universe);
                }
                catch (Exception e)
                {
                    Console.WriteLine("File Error: {0}", e.Message);
                }
            }

        }
        /// <summary>
        /// If the setting have step mode enabled the universe will procede whenever spacebar is pressed. It also
        /// sets the cells for ghost mode (if enable) and adds every generation to a list of generations. This
        /// is used to check for a steady state.
        /// </summary>
        public void StepMode()
        {
            for (int generation = -1; generation < UniverseSettings.generations; generation++)
            {
                if (UniverseSettings.ghostMode == true)
                {
                    GhostRender();

                }
                List<int[]> aliveCells = new List<int[]>();
                for (int row = 0; row < universe.GetLength(0); row++)
                {
                    for (int column = 0; column < universe.GetLength(1); column++)
                    {
                        if (universe[row, column] == 1)
                        {
                            aliveCells.Add(new int[] { row, column });
                            grid.UpdateCell(row, column, CellState.Full);
                        }
                    }
                }

                grid.SetFootnote($"Iteration: {generation + 1}");
                grid.Render();
                //breaks the loop of generations if a steady state is detected
                if (CheckSteadyState(aliveCells, generation) == true)
                {
                    AddToMemeory(aliveCells);
                    break;
                }
                if (!((generation + 1) == UniverseSettings.generations))
                {
                    ResetGrid();
                }
                AddToMemeory(aliveCells);
                NextGeneration();
                WaitSpacebar();
            }

        }
        /// <summary>
        /// If the setting have step mode disable the universe will procede after a certain time, this is based of 
        /// the refreshRate in the settings. It also sets the cells for ghost mode (if enable) and adds every 
        /// generation to a list of generations. This is used to check for a steady state.
        /// </summary>
        public void NoStepMode()
        {

            Stopwatch watch = new Stopwatch();
            for (int generation = -1; generation < UniverseSettings.generations; generation++)
            {
                if (UniverseSettings.ghostMode == true)
                {
                    GhostRender();

                }
                
                List<int[]> aliveCells = new List<int[]>();
                watch.Restart();
                for (int row = 0; row < universe.GetLength(0); row++)
                {
                    for (int column = 0; column < universe.GetLength(1); column++)
                    {
                        if (universe[row, column] == 1)
                        {
                            aliveCells.Add(new int[] { row, column });
                            grid.UpdateCell(row, column, CellState.Full);
                        }
                    }
                }
                grid.SetFootnote($"Iteration: {generation + 1}");
                grid.Render();
                //breaks the loop of generations if a steady state is detected
                if (CheckSteadyState(aliveCells, generation) == true)
                {
                    AddToMemeory(aliveCells);
                    break;
                }
                if (!((generation + 1) == UniverseSettings.generations))
                {
                    ResetGrid();
                }
                AddToMemeory(aliveCells);
                NextGeneration();
                while (watch.ElapsedMilliseconds < (1000 / UniverseSettings.refreshRate)) ;
            }

        }
        /// <summary>
        /// Sets the last 3 generations of cells to a certain cell state depending of how old they are.
        /// the older the generation the lighter the cellState
        /// </summary>
        public void GhostRender()
        {
            List<List<int[]>> ghostGenerations = new List<List<int[]>>();
            //loops through the last three items of the memoryGenerations List and adds then to ghostGenerations
            for (int i = memoryGenerations.Count; i > 0 && i > memoryGenerations.Count - 3; i--)
            { 
                ghostGenerations.Add(memoryGenerations[i - 1]);
            }
            //loops through the ghostGenerations array starting from the oldest towards the newest and sets 
            //there cell state 
            for (int i = ghostGenerations.Count-1; i >= 0; i--)
            {
                for(int j = 0; j < ghostGenerations[i].Count; j++)
                {

                    if (i == 0)
                    {
                        grid.UpdateCell(ghostGenerations[i][j][0], ghostGenerations[i][j][1], CellState.Dark);
                    }
                    else if (i == 1)
                    {
                        grid.UpdateCell(ghostGenerations[i][j][0], ghostGenerations[i][j][1], CellState.Medium);
                    }
                    else if (i == 2)
                    {
                        grid.UpdateCell(ghostGenerations[i][j][0], ghostGenerations[i][j][1], CellState.Light);
                    }
                    
                }

            }
            

        }

        /// <summary>
        /// will generate the next generation of the universe based on a set of rules for the program of life.
        /// it will take the previous universe and periodic variables as input and based on the number of alive
        /// neighbours will return the universe with different alive cells
        /// The Moore and VonNeumann class both calculate this number of alive cells differently 
        /// </summary>
        public void NextGeneration()
        {
            Neighbourhoods neighbourhood;
            bool periodic = UniverseSettings.periodic;
            if (UniverseSettings.neighbourhood == "Moore")
            {
                neighbourhood = new Moore();
            }
            else
            {
                neighbourhood = new VonNeumann();
            }
            int[,] nextGenerationArray = new int[universe.GetLength(0), universe.GetLength(1)]; ;
            for (int row = 0; row < universe.GetLength(0); row++)
            {
                for (int column = 0; column < universe.GetLength(1); column++)
                {
                    neighbourhood.CountNeighbours(row, column, UniverseSettings, universe);

                    if (universe[row, column] == 1)
                    {
                        for (int i = 0; i < UniverseSettings.survival.Count; i++)
                        {
                            if (neighbourhood.aliveNeighbours == UniverseSettings.survival[i])
                            {
                                nextGenerationArray[row, column] = 1;
                            }
                        }
                    }

                    if (universe[row, column] == 0)
                    {
                        for (int j = 0; j < UniverseSettings.birth.Count; j++)
                        {
                            if (neighbourhood.aliveNeighbours == UniverseSettings.birth[j])
                            {
                                nextGenerationArray[row, column] = 1;
                            }
                        }
                    }
                }
            }
            universe = nextGenerationArray;
        }

        /// <summary>
        /// takes the grid (which is used to display the universe) and loops between all rows and columns of the
        /// universe setting and sets each cell location to blank. reseting the grid for the next 
        /// generation to be displayed
        /// </summary>
        public void ResetGrid()
        {
            for (int row = 0; row < universe.GetLength(0); row++)
            {
                for (int column = 0; column < universe.GetLength(1); column++)
                {
                    grid.UpdateCell(row, column, CellState.Blank);
                }
            }
        }
        /// <summary>
        /// used to check the current generation with the last number of generations to see if the state of the
        /// current generation matches the state of any previous generations or if the current generation is all dead.
        /// if so the method returns true which stops the generation of new generations. It also sets the
        /// periodictyType that is used when display if a steady state was met in the UniverseDisplayer() method
        /// </summary>
        /// <param name="aliveCells">all the current alive cells of the universe</param>
        /// <param name="generation">the number of the generation that the program is on</param>
        /// <returns>return true if the current generation ("aliveCells") matches any of the older
        /// generation and false if it does not</returns>
        public bool CheckSteadyState(List<int[]> aliveCells, int generation)
        {
            bool steadyStateReach = false;
            
            if (aliveCells.Count == 0)
            {
                string time = DateTime.Now.ToLongTimeString();
                periodictyType = $"[{time}] Steady-state detected ... periodicity = N/A";
                steadyStateReach = true;
                return steadyStateReach;

            }

            for (int i = 0; i < memoryGenerations.Count; i++)
            {
                if (aliveCells.Count == memoryGenerations[i].Count)
                {
                    int steadyState = 0;
                    for (int j = 0; j < memoryGenerations[i].Count; j++)
                    {
                        int eachCellChecker = 0;
                        if (aliveCells[j][0] == memoryGenerations[i][j][0])
                        {
                            eachCellChecker++;

                        }
                        if (aliveCells[j][1] == memoryGenerations[i][j][1])
                        {
                            eachCellChecker++;
                        }
                        if (eachCellChecker == 2)
                        {
                            steadyState++;
                        }

                    }

                    if (steadyState == aliveCells.Count)
                    {
                        string time = DateTime.Now.ToLongTimeString();
                        periodictyType = $"[{time}] Steady-state detected ... periodicity = {generation + 1}";
                        steadyStateReach = true;
                        return steadyStateReach;
                    }
                }
            }
            return steadyStateReach;
        }

        /// <summary>
        /// Adds the current generation ("aliveCells") to the list of the previous generations ("memoryGenerations").
        /// If the memoryGenerations is the same size of the memory setting then the first item added will be removed
        /// meaning that only the required amount of generations are stored
        /// </summary>
        /// <param name="aliveCells">the current aliveCells of the universe</param>
        public void AddToMemeory(List<int[]> aliveCells)
        {
            if (memoryGenerations.Count == UniverseSettings.memory)
            {
                memoryGenerations.RemoveAt(0);

            }
            memoryGenerations.Add(aliveCells);
        }
        /// <summary>
        /// will wait for the user to press spacebar
        /// </summary>
        /// <param name="action">the action is string that states was the program is performing when the 
        /// spacebar is press</param>
        public void Spacebar(string action)
        {
            Console.WriteLine($"Press spacebar to {action} simulation...");
            Stopwatch watch = new Stopwatch();
            List<int> keyList = new List<int>();
            bool breaker = false;
            while (true)
            {
                watch.Restart();
                while (watch.ElapsedMilliseconds < 50) ;
                if (Console.KeyAvailable)
                {
                    keyList.Add(1);
                    while (Console.KeyAvailable)
                    {
                        while (Console.ReadKey().Key != ConsoleKey.Spacebar) 
                        {
                            Console.WriteLine();
                            Console.WriteLine($"Press spacebar to {action} simulation...");
                        }
                    }
                }
                else
                {
                    keyList.Add(0);
                }
                if (keyList.Count == 6)
                {
                    keyList.RemoveAt(0);
                }
                if (keyList.Count == 5)
                {
                    if (keyList[2] == 1 && keyList[1] == 0 && keyList[3] == 0)
                    {
                        break;
                    }
                }
            }
        }
        /// <summary>
        /// same as the spacebar method expect stops the user from holding down spacebar
        /// </summary>
        public void WaitSpacebar()
        {
            Stopwatch watch = new Stopwatch();
            List<int> keyList = new List<int>();
            while (true)
            {
                watch.Restart();
                while (watch.ElapsedMilliseconds < 20) ;
                if (Console.KeyAvailable)
                {
                    keyList.Add(1);
                    while (Console.KeyAvailable)
                    {
                        while (Console.ReadKey().Key != ConsoleKey.Spacebar) ;
                    }
                }
                else
                {
                    keyList.Add(0);
                }
                if (keyList.Count == 6)
                {
                    keyList.RemoveAt(0);
                }
                if (keyList.Count == 5)
                {
                    if (keyList[2] == 1 && keyList[1] == 0 && keyList[3] == 0)
                    {
                        break;
                    }
                }
            }
        }

    }
}
