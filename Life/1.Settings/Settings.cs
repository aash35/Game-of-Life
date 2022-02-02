using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml.Xsl;

namespace Life
{
    public class Settings
    {

        //The arguments and there parameters split into a list
        public List<string> argumentsList { get; private set; } = new List<string>();
        //the different settings of the program
        public string inputFile { get; private set; }
        public int generations { get; private set; }
        public int rows { get; private set; }
        public int columns { get; private set; }
        public float randomFactor { get; private set; }
        public float refreshRate { get; private set; }
        public bool periodic { get; private set; }
        public bool stepMode { get; private set; }
        public string neighbourhood { get; private set; }
        public int neighbourhoodOrder { get; private set; }
        public bool neighbourhoodCentre { get; private set; }
        public int neighbourhoodNumber { get; private set; }
        public List<string> survivalString { get; private set; } = new List<string>();
        public List<string> birthString { get; private set; } = new List<string>();
        public List<int> survival { get; private set; } = new List<int>();
        public List<int> birth { get; private set; } = new List<int>();
        public int memory { get; private set; }
        public string outputFile { get; private set; }
        public bool output { get; private set; }
        public bool ghostMode { get; private set; }

        /// <summary>
        /// A constructor that sets the settings for the universe then prints the error messages (if there are any)
        /// </summary>
        /// <param name="args"> a string array that has all the arguments that control the universe settings</param>
        public Settings(string[] args)
        {
            SetDefaults();

            //splits all the args into the argumentsList variable 
            ArgumentList(args);

            string commandLineSuccess = "Success: Command line arguments processed.";
            string commandLineError = "";

            for (int i = 0; i < argumentsList.Count; i++)
            {
                args = argumentsList[i].Split(" ");
                if (argumentsList[i].Contains("--"))
                {
                    if (args[0] == "--dimensions")
                    {
                        Dimensions(args, ref commandLineError);
                    }

                    else if (args[0] == "--periodic")
                    {
                        Periodic(args, ref commandLineError);
                    }

                    else if (args[0] == "--random")
                    {
                        Random(args, ref commandLineError);

                    }

                    else if (args[0] == "--seed")
                    {
                        Seed(args, ref commandLineError);
                    }

                    else if (args[0] == "--generations")
                    {
                        Generations(args, ref commandLineError);
                    }

                    else if (args[0] == "--max-update")
                    {
                        MaxUpdate(args, ref commandLineError);
                    }

                    else if (args[0] == "--step")
                    {
                        Step(args, ref commandLineError);
                    }

                    else if (args[0] == "--neighbour")
                    {
                        Neighbour(args, ref commandLineError);
                    }
                    else if (args[0] == "--survival")
                    {
                        SurvivalAndBirth(args, ref commandLineError, survival, survivalString, "Survival");
                    }
                    else if (args[0] == "--birth")
                    {
                        SurvivalAndBirth(args, ref commandLineError, birth, birthString, "Birth");
                    }
                    else if (args[0] == "--memory")
                    {
                        Memory(args, ref commandLineError);
                    }
                    else if (args[0] == "--output")
                    {
                        Output(args, ref commandLineError);
                    }
                    else if (args[0] == "--ghost")
                    {
                        Ghost(args, ref commandLineError);
                    }

                    else
                    {
                        commandLineError += "<>Parameter Error: incorrect option was used";
                    }
                }
                else
                {
                    commandLineError += $"<>Parameter Error: '{args[i]}' is not a valid parameter, use '--' followed by a"
                        + " setting type to modify Life";
                }
            }
            if (commandLineError.Length > 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                string[] Errors = commandLineError.Split("<>");
                for (int i = 1; i < Errors.Length; i++)
                {
                    Console.WriteLine(Errors[i]);
                }
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(commandLineSuccess);
                Console.ResetColor();

            }

        }

        /// <summary>
        /// Sets the default values of the settings and is called when the object is constructed
        /// </summary>
        public void SetDefaults()
        {
            inputFile = "N/A";
            generations = 50;
            rows = 16;
            columns = 16;
            randomFactor = 0.5F;
            refreshRate = 5F;
            periodic = false;
            stepMode = false;
            neighbourhood = "Moore";
            neighbourhoodOrder = 1;
            neighbourhoodCentre = false;

            SurvivalDefaults();
            BirthDefaults();

            memory = 16;
            outputFile = "N/A";
            output = false;
            ghostMode = false;

            NeighbourhoodCalculator(neighbourhood, neighbourhoodOrder, (neighbourhoodCentre == false ? "false" 
                : "true"));

        }
        /// <summary>
        /// Sets the defaults for the survivalString and survival variables
        /// </summary>
        public void SurvivalDefaults()
        {
            survival.Clear();
            survivalString.Clear();
            survivalString.Add("2...3");
            survival.Add(2);
            survival.Add(3);


        }
        /// <summary>
        /// Sets the defaults for the birthString and birth variables
        /// </summary>
        public void BirthDefaults()
        {
            birth.Clear();
            birthString.Clear();
            birthString.Add("3");
            birth.Add(3);

        }

        /// <summary>
        /// Splits the string args array into a list of arguments and its parameters
        /// </summary>
        /// <param name="args">all the arguments that are entered when the program started</param>
        public void ArgumentList(string[] args)
        {
            string argumentsString = "";
            foreach (string element in args)
            {
                argumentsString += " " + element;
            }
            argumentsList = argumentsString.Split("--").ToList();
            argumentsList.RemoveAt(0);
            for (int i = 0; i < argumentsList.Count(); i++)
            {
                argumentsList[i] = "--" + argumentsList[i];
                argumentsList[i] = argumentsList[i].Trim();
            }
            List<string> argumentsListHolder = new List<string>();

            //removes the --birth and --survival args and adds then at the end (used to calculate the 
            //neighbourhoodNumber variable) 
            for (int i = 0; i < argumentsList.Count(); i++)
            {
                if (argumentsList[i].Contains("--birth") || argumentsList[i].Contains("--survival"))
                {
                    argumentsListHolder.Add(argumentsList[i]);
                    argumentsList.RemoveAt(i);
                    i--;
                }
            }
            argumentsList.AddRange(argumentsListHolder);

        }

        /// <summary>
        /// This sets the dimensions varaible if the paramaeters are correct
        /// </summary>
        /// <param name="args">takes the dimension arg with its parameters so it can be queried when required</param>
        /// <param name="commandLineError">a string that has all the different error messages concatenated onto 
        /// it</param>
        public void Dimensions(string[] args, ref string commandLineError)
        {
            try
            {
                if ((args.Length > (0 + 2)) && (!args[0 + 1].Contains("--")
                    && !args[0 + 2].Contains("--")))
                {
                    if (args.Length > (0 + 3) && !args[0 + 3].Contains("--"))
                    {
                        throw new Exception("too many parameters");
                    }
                    try
                    {
                        int rowsValidator = Int32.Parse(args[0 + 1]);
                        int columnsValidator = Int32.Parse(args[0 + 2]);

                        if ((rowsValidator >= 4 && rowsValidator <= 48) && (columnsValidator >= 4
                            && columnsValidator <= 48))
                        {
                            rows = rowsValidator;
                            columns = columnsValidator;
                        }
                        else
                        {
                            throw new Exception("both dimensions must be between 4 and 48");
                        }
                    }
                    catch (Exception e)
                    {
                        if (e.Message.Contains("both"))
                        {
                            throw e;
                        }
                        else
                        {
                            throw new Exception("the dimensions were not numbers");
                        }
                    }
                }
                else
                {
                    throw new Exception("dimensions requires 2 extra parameters");
                }
            }
            catch (Exception e)
            {
                commandLineError += ("<>Dimensions Error: " + e.Message);
            }

        }
        /// <summary>
        /// This sets the periodic varaible if the paramaeters are correct
        /// </summary>
        /// <param name="args">takes the periodic arg with its parameters so it can be queried when required</param>
        /// <param name="commandLineError">a string that has all the different error messages concatenated onto 
        /// it</param>
        public void Periodic(string[] args, ref string commandLineError)
        {
            try
            {
                if (args.Length > (0 + 1) && !args[0 + 1].Contains("--"))
                {
                    throw new Exception("too many parameters");
                }
                periodic = true;
            }
            catch (Exception e)
            {
                commandLineError += ("<>Periodic Error: " + e.Message);
            }

        }
        /// <summary>
        /// This sets the Random varaible if the paramaeters are correct
        /// </summary>
        /// <param name="args">takes the Random arg with its parameters so it can be queried when required</param>
        /// <param name="commandLineError">a string that has all the different error messages concatenated onto 
        /// it</param>
        public void Random(string[] args, ref string commandLineError)
        {
            try
            {
                if ((args.Length > (0 + 1)) && !args[0 + 1].Contains("--"))
                {
                    if (args.Length > (0 + 2) && !args[0 + 2].Contains("--"))
                    {
                        throw new Exception("too many parameters");
                    }

                    float randomValidator;
                    bool checker = float.TryParse(args[0 + 1], out randomValidator);
                    if (checker == true)
                    {
                        if (randomValidator >= 0 && randomValidator <= 1)
                        {
                            randomFactor = randomValidator;
                        }
                        else
                        {
                            throw new Exception("the Random parameter must be between 0 and 1");
                        }
                    }
                    else
                    {
                        throw new Exception("the Random parameter was not a number");
                    }
                }
                else
                {
                    throw new Exception("random requires 1 extra parameter");
                }
            }
            catch (Exception e)
            {
                commandLineError += ("<>Randomisation Number Error: " + e.Message);
            }

        }
        /// <summary>
        /// This sets the Seed varaible if the paramaeters are correct
        /// </summary>
        /// <param name="args">takes the Seed arg with its parameters so it can be queried when required</param>
        /// <param name="commandLineError">a string that has all the different error messages concatenated onto 
        /// it</param>
        public void Seed(string[] args, ref string commandLineError)
        {
            try
            {
                if ((args.Length > (0 + 1)) && !args[0 + 1].Contains("--"))
                {
                    if (args.Length > (0 + 2) && !args[0 + 2].Contains("--"))
                    {
                        throw new Exception("too many parameters");
                    }
                    if ((File.Exists(args[0 + 1]) && Path.GetExtension(args[0 + 1]) == ".seed"))
                    {
                        inputFile = args[0 + 1];
                    }
                    else
                    {
                        throw new Exception("must be a valid file path to a .seed document");
                    }
                }
                else
                {
                    throw new Exception("seed requires 1 extra parameter");
                }
            }
            catch (Exception e)
            {
                commandLineError += ("<>Seed Error: " + e.Message);
            }
        }
        /// <summary>
        /// This sets the Generations varaible if the paramaeters are correct
        /// </summary>
        /// <param name="args">takes the Generations arg with its parameters so it can be queried when required</param>
        /// <param name="commandLineError">a string that has all the different error messages concatenated onto 
        /// it</param>
        public void Generations(string[] args, ref string commandLineError)
        {
            try
            {
                if ((args.Length > (0 + 1)) && !args[0 + 1].Contains("--"))
                {
                    if (args.Length > (0 + 2) && !args[0 + 2].Contains("--"))
                    {
                        throw new Exception("too many parameters");
                    }
                    int generationsValidator;
                    bool checker = Int32.TryParse(args[0 + 1], out generationsValidator);
                    if (checker == true)
                    {
                        if (generationsValidator >= 0)
                        {
                            generations = generationsValidator;
                        }
                        else
                        {
                            throw new Exception("the Generations parameter must be a positive integer");
                        }
                    }
                    else
                    {
                        throw new Exception("the Generations parameter was not a number");
                    }
                }
                else
                {
                    throw new Exception("dimensions requires 1 extra parameter");
                }
            }
            catch (Exception e)
            {
                commandLineError += ("<>Generations Error: " + e.Message);
            }

        }
        /// <summary>
        /// This sets the MaxUpdate varaible if the paramaeters are correct
        /// </summary>
        /// <param name="args">takes the MaxUpdate arg with its parameters so it can be queried when required</param>
        /// <param name="commandLineError">a string that has all the different error messages concatenated onto 
        /// it</param>
        public void MaxUpdate(string[] args, ref string commandLineError)
        {
            try
            {
                if ((args.Length > (0 + 1)) && !args[0 + 1].Contains("--"))
                {
                    if (args.Length > (0 + 2) && !args[0 + 2].Contains("--"))
                    {
                        throw new Exception("too many parameters");
                    }
                    float updateValidator;
                    bool checker = float.TryParse(args[0 + 1], out updateValidator);
                    if (checker == true)
                    {
                        if (updateValidator >= 1 && updateValidator <= 30)
                        {
                            refreshRate = updateValidator;
                        }
                        else
                        {
                            throw new Exception("the Refresh Rate parameter must be between 1 and 30");
                        }
                    }
                    else
                    {
                        throw new Exception("the Refresh Rate parameter was not a number");
                    }
                }
                else
                {
                    throw new Exception("dimensions requires 1 extra parameter");
                }
            }
            catch (Exception e)
            {
                commandLineError += ("<>Refresh Rate Error: " + e.Message);
            }

        }
        /// <summary>
        /// This sets the Step varaible if the paramaeters are correct
        /// </summary>
        /// <param name="args">takes the Step arg with its parameters so it can be queried when required</param>
        /// <param name="commandLineError">a string that has all the different error messages concatenated onto 
        /// it</param>
        public void Step(string[] args, ref string commandLineError)
        {
            try
            {
                if (args.Length > (0 + 1) && !args[0 + 1].Contains("--"))
                {
                    throw new Exception("too many parameters");
                }
                stepMode = true;
            }
            catch (Exception e)
            {
                commandLineError += ("<>Step Error: " + e.Message);
            }

        }
        /// <summary>
        /// This sets the Neighbour varaible if the paramaeters are correct
        /// </summary>
        /// <param name="args">takes the Neighbour arg with its parameters so it can be queried when required</param>
        /// <param name="commandLineError">a string that has all the different error messages concatenated onto 
        /// it</param>
        public void Neighbour(string[] args, ref string commandLineError)
        {
            try
            {
                if (args.Length == 4)
                {
                    try
                    {
                        bool validationPass = true;
                        string localError = "<Validation Error>";
                        string typeValidator = args[1].ToLower();
                        int orderValidator = Int32.Parse(args[2]);
                        string centreValidator = args[3].ToLower();

                        if (orderValidator < 1 || orderValidator > 10)
                        {
                            validationPass = false;
                            localError += "Order must be between 1 and 10. ";
                        }

                        if (typeValidator == "moore" || typeValidator == "vonneumann") { }
                        else
                        {
                            validationPass = false;
                            localError += "The type must be either moore or vonneumann. ";
                        }
                        if (centreValidator == "true" || centreValidator == "false") { }
                        else
                        {
                            validationPass = false;
                            localError += "The option must be true or false ";
                        }

                        if (validationPass == true)
                        {
                            neighbourhood = (typeValidator == "moore" ? "Moore" : "VonNeumann");
                            neighbourhoodOrder = orderValidator;
                            neighbourhoodCentre = (centreValidator == "true" ? true : false);
                            NeighbourhoodCalculator((typeValidator == "moore" ? "Moore" : "VonNeumann")
                                , orderValidator, centreValidator);
                        }
                        else
                        {
                            throw new Exception(localError);
                        }

                    }
                    catch (Exception e)
                    {
                        if (e.Message.Contains("<Validation Error>"))
                        {
                            string newError = e.Message;
                            newError = newError.Substring(18, newError.Length - 18);

                            throw new Exception(newError);
                        }
                        else
                        {
                            throw new Exception("the order (second parameter) was not a number");
                        }
                    }
                }
                else
                {
                    if (args.Length > 4)
                    {
                        throw new Exception("neighbour requires 3 extra parameters (too many extra parameters)");

                    }
                    else if (args.Length < 4)
                    {
                        throw new Exception("neighbour requires 3 extra parameters (string then int then boolean)");
                    }
                }
            }
            catch (Exception e)
            {
                commandLineError += ("<>Neighbourhood Error: " + e.Message);
            }


        }
        /// <summary>
        /// This sets the survivalString and survival varaible or the birthString and birth varaible if the 
        /// paramaeters are correct
        /// </summary>
        /// <param name="args">takes the survival or birth arg with its parameters so it can be queried when
        /// required</param>
        /// <param name="commandLineError">a string that has all the different error messages concatenated onto 
        /// it</param>
        /// <param name="numberList">takes the list of numbers that need to be validated and adds then to the birth
        /// or survival variables</param>
        /// <param name="stringList">takes the list of strings that need to be validated and adds then to the 
        /// birthString or survivalString variables</param>
        /// <param name="name">take the name of the options to be modified ("survival" or "birth")</param>
        public void SurvivalAndBirth(string[] args, ref string commandLineError, List<int> numberList, List<string> stringList, string name)
        {
            try
            {
                stringList.Clear();
                numberList.Clear();
                for (int i = 1; i < args.Length; i++)
                {
                    if (args[i].Contains("..."))
                    {
                        string[] deconstructor = args[i].Split("...");
                        try
                        {
                            int firstNumberValidator = Int32.Parse(deconstructor[0]);
                            int secondNumberValidator = Int32.Parse(deconstructor[1]);
                            if (firstNumberValidator > secondNumberValidator)
                            {
                                throw new Exception($"parameter number {i} must have the first number smaller " +
                                    $"then the second");
                            }
                            if ((firstNumberValidator <= neighbourhoodNumber && firstNumberValidator >= 0)
                                && (secondNumberValidator <= neighbourhoodNumber && secondNumberValidator >= 0))
                            {
                                stringList.Add(args[i]);
                                for (int j = firstNumberValidator; j < secondNumberValidator + 1; j++)
                                {
                                    numberList.Add(j);
                                }
                            }
                            else
                            {
                                throw new Exception($"both parameters in option {i} must be less than or equal to " +
                                    $"the number of neighbouring cells ({neighbourhoodNumber}) and a" +
                                    $" non-negative number");
                            }

                        }
                        catch (Exception e)
                        {
                            if (e.Message.Contains("both"))
                            {
                                throw e;
                            }
                            else if (e.Message.Contains("smaller then"))
                            {
                                throw e;
                            }
                            else
                            {
                                throw new Exception($"parameter number {i} was not numbers");
                            }
                        }

                    }
                    else
                    {
                        int numberValidator;
                        bool checker = Int32.TryParse(args[i], out numberValidator);
                        if (checker == true)
                        {
                            if (numberValidator <= neighbourhoodNumber && numberValidator >= 0)
                            {
                                stringList.Add(args[i]);
                                numberList.Add(numberValidator);
                            }
                            else
                            {
                                throw new Exception($"parameter number {i} must be less than or equal to the number "
                                    + $"of neighbouring cells ({neighbourhoodNumber}) and a non-negative number");
                            }
                        }
                        else
                        {
                            throw new Exception($"parameter number {i} was not a number");
                        }

                    }

                }
            }
            catch (Exception e)
            {
                if (name == "Birth")
                {
                    BirthDefaults();
                }
                else if (name == "Survival")
                {
                    SurvivalDefaults();
                }

                commandLineError += ($"<>{name} Error: " + e.Message + $"\n{name} resetting to defaults.");
            }

        }
        /// <summary>
        /// This sets the Memory varaible if the paramaeters are correct
        /// </summary>
        /// <param name="args">takes the Memory arg with its parameters so it can be queried when required</param>
        /// <param name="commandLineError">a string that has all the different error messages concatenated onto 
        /// it</param>
        public void Memory(string[] args, ref string commandLineError)
        {
            try
            {
                if ((args.Length > (0 + 1)) && !args[0 + 1].Contains("--"))
                {
                    if (args.Length > (0 + 2) && !args[0 + 2].Contains("--"))
                    {
                        throw new Exception("too many parameters");
                    }
                    int memoryValidator;
                    bool checker = Int32.TryParse(args[0 + 1], out memoryValidator);
                    if (checker == true)
                    {
                        if (memoryValidator >= 4 && memoryValidator <= 512)
                        {
                            memory = memoryValidator;
                        }
                        else
                        {
                            throw new Exception("the Memory must be a number between 4 and 512");
                        }
                    }
                    else
                    {
                        throw new Exception("the Generational Memory parameter was not a number");
                    }
                }
                else
                {
                    throw new Exception("memory requires 1 extra parameter");
                }
            }
            catch (Exception e)
            {
                commandLineError += ("<>Generational Memory Error: " + e.Message);
            }

        }
        /// <summary>
        /// This sets the Output varaible if the paramaeters are correct
        /// </summary>
        /// <param name="args">takes the Output arg with its parameters so it can be queried when required</param>
        /// <param name="commandLineError">a string that has all the different error messages concatenated onto 
        /// it</param>
        public void Output(string[] args, ref string commandLineError)
        {
            try
            {
                string directoryName = Path.GetDirectoryName(args[0 + 1]);
                if ((args.Length > (0 + 1)) && !args[0 + 1].Contains("--"))
                {
                    if (args.Length > (0 + 2) && !args[0 + 2].Contains("--"))
                    {
                        throw new Exception("too many parameters");
                    }
                    if (Directory.Exists(directoryName))
                    {
                        if (Path.GetExtension(args[0 + 1]) == ".seed")
                        {
                            outputFile = args[0 + 1];
                            output = true;
                        }
                        else
                        {
                            throw new Exception("the output must have the .seed extention");
                        }
                    }
                    else
                    {
                        throw new Exception("not a valid directory");
                    }
                }
                else
                {
                    throw new Exception("output requires 1 extra parameter");
                }
            }
            catch (Exception e)
            {
                commandLineError += ("<>Output File Error: " + e.Message);
            }
        }
        /// <summary>
        /// This sets the Ghost varaible if the paramaeters are correct
        /// </summary>
        /// <param name="args">takes the Ghost arg with its parameters so it can be queried when required</param>
        /// <param name="commandLineError">a string that has all the different error messages concatenated onto 
        /// it</param>
        public void Ghost(string[] args, ref string commandLineError)
        {
            try
            {
                if (args.Length > (0 + 1) && !args[0 + 1].Contains("--"))
                {
                    throw new Exception("too many parameters");
                }
                ghostMode = true;
            }
            catch (Exception e)
            {
                commandLineError += ("<>Step Error: " + e.Message);
            }

        }
        /// <summary>
        /// Calculates the number of neighbours each indivdual cell can have so Survival and Birth can validation 
        /// correctly
        /// </summary>
        /// <param name="type">takes the neighbourhood type "moore" or "vonneumann" and calculates base 
        /// off the name</param>
        /// <param name="order">takes the neighbourhood order and is used for the calculations<param>
        /// <param name="centre">takes the neighbourhood centre aand is used for the calculations</param>
        public void NeighbourhoodCalculator(string type, int order, string centre)
        {
            int neighbourhoodCalculation = 0;
            int multiplier = 0;
            for (int i = 0; i < order; i++)
            {
                multiplier += i + 1;

            }
            if (type == "Moore")
            {
                neighbourhoodCalculation += 8 * multiplier;
                if (centre == "true")
                {
                    neighbourhoodCalculation++;
                }

            }
            else if (type == "VonNeumann")
            {
                neighbourhoodCalculation += 4 * multiplier;
                if (centre == "true")
                {
                    neighbourhoodCalculation++;
                }
            }
            neighbourhoodNumber = neighbourhoodCalculation;
        }

        /// <summary>
        /// override the to string method to display all the settings of the universe
        /// </summary>
        /// <returns>return a string that contains all the settings of the universe</returns>
        public override string ToString()
        {
            char[] name = neighbourhood.ToCharArray();
            name[0] = char.ToUpper(name[0]);
            string upperNeighbourhood = new string(name);

            string survivalDisplayer = string.Join(" ", survivalString.ToArray());
            string birthDisplayer = string.Join(" ", birthString.ToArray());

            string returnMessage = "The program will run on the following settings:\n\n"
                + ($"{"Input File: ",20}{inputFile}") + "\n"
                + ($"{"Output File: ",20}{outputFile}") + "\n"
                + ($"{"Generations: ",20}{generations}") + "\n"
                + ($"{"Memory: ",20}{memory}") + "\n"
                + ($"{"Refresh Rate: ",20}{refreshRate} updates/s") + "\n"
                + ($"{"Rules: ",20}S( {survivalDisplayer} ) B( {birthDisplayer} )") + "\n"
                + ($"{"Neighbourhood: ",20}{upperNeighbourhood} ({neighbourhoodOrder}) " +
                $"{(neighbourhoodCentre == false ? "" : "(centre-counted)")}") + "\n"
                + ($"{"Periodic: ",20}{(periodic == false ? "No" : "Yes")}") + "\n"
                + ($"{"Rows: ",20}{rows}") + "\n"
                + ($"{"Columns: ",20}{columns}") + "\n"
                + ($"{"Random Factor: ",20}{randomFactor.ToString("P")}") + "\n"
                + ($"{"Step Mode: ",20}{(stepMode == false ? "No" : "Yes")}") + "\n"
                + ($"{"Ghost Mode: ",20}{(ghostMode == false ? "No" : "Yes")}") + "\n";

            return returnMessage;
        }
    }
}
