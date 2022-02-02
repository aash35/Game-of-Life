# Programming Principles - C#

CAB201 - Programming Principles

This game is a recreation of **John Conway's** Famous Game of Life. The principles remain the same with the state of each cell being determined by neighbouring cells. The settings can be changed (see below) for many different game states. There is also a number of preconfigured 'seeds' were the intial state of the game is varied for special outcomes.

## Build Instructions

1) Open the "Life" folder
2) Open the "Life.sln" file
3) Ensure that at the top underneath the "Debug" drop down menu that it say "Release". 
   If not click the drop down and change from "Debug" to "Release.
4) On the right hand side in the "Solution Explorer" right click "Solution 'Life' (2 of 2 projects)"
5) Click "Build Solution" (or Ctrl+Shift+B)
6) Note the location of the build in the bottom panel (e.g. "C:\CAB201_2020S2_ProjectPartA_n10624937\Life\Life\bin\Release\netcoreapp3.1\life.dll") 

## Usage 

The program of "Life" can be called from the CLI with a number of different parameters
1) Open your chosen CLI (e.g. cmd, powershell, terminal) 
2) Navigate to the location of "life.dll" as shown in the above build section (note without the "life.dll" at the end)
3) type "dotnet life.dll" (will run the file with default settings)
4) to run the program with different settings there are a number of different parameters that can be passed after the "dotnet life.dll"
Examples shown at the bottom.

**These arguments are**
-------------
### Rows and Columns: `--dimensions <rows> <columns>`
	
Rules: Where <row> and <column> are numbers between 4 and 48 (inclusive)  
Effect: Changes the dimensions of the universe  
Default: 16x16  
	
### Periodic Behaviour: `--periodic`
	
Rules: There are no extra values for periodic (the next parameter, if there is one, must start with "--")  
Effect: Allows the universe to act in as if the borders wrapped around to the other side  
Default: Off  

### Random Factor: `--random <probability>`

Rules: Where <probability> is between 0 and 1 (inclusive)  
Effect: When a seed is not given each cell will have this probability of being alive  
Default: 50%  

### Input File: `--seed <filename>`

Rules: Where <filename> must be a valid path to a file with the ".seed" extension  
Effect: Sets the alive or dead cells in a universe based on the seed files row and column location  
Default: N/A  

### Generations: `--generations <number>`

Rules: Where <number> is a positive non-zero integer  
Effect: Changes how many generations the universe will go through before ending  
Default: 50  

### Maximum Update Rate: `--max-update <ups>`

Rules: Where <ups> is between 1 and 30 (inclusive)  
Effect: Changes how quickly each generation is rendered  
Default: 5  

### Step Mode: `--step`

Rules: There are no extra values for step (the next parameter, if there is one, must start with "--")  
Effect: To render each new generation the user must press spacebar  
Default: Off  

### Neighbourhood: `--neighbour <type> <order> <centre-count>`

Rules: Where <type> is either "moore" or "vonneumann" (case insenitive) 
       Where <order> is an integer between 1 and 10 (inclusive)
       Where <centre-count> is true or false  
Effect: Changes how each cells neighbouring alive cells are calculated   
Default: Moore 1 false  

### Survival and Birth:  `--survival <param1> <param2> <param3> ..." and "--birth <param1> <param2> <param3> ...`

Rules: Where each <parameter> is either an single positive integer or two positive integers seperated by "..."  
Effect: Changes how many alive cells are needed for a cell to be "born" or "survive" respectively  
Default: Survival is 2 or 3 and Birth is 3  

### Generational Memory: `--memory <number>`

Rules: Where <number> is a integer between 4 and 512 (inclusive)  
Effect: Changes how many generations of the universe are stored to detect a steady state  
Default: 16  

### Output File:  `--output <filename>`

Rules: Where <filename> is a existing path with the output file itself having the .seed extension  
Effect: Changes where the output file is stored and what it is called.  
Default: "N/A"  

### Ghost Mode: `--ghost`

Rules: There are no extra values for ghost (the next parameter, if there is one, must start with "--")  
Effect: Changes the grid rendering to show the previous three generations in degrading form  
Default: Off  



**Some Examples of arguments in use are**  
"dotnet life.dll --step"
"dotnet life.dll --step --periodic"
"dotnet life.dll --dimensions 8 8"
"dotnet life.dll --dimensions 32 32 --seed path/to/seed --max-update 12"
"dotnet life.dll --neighbour vonneumann 8 true --dimensions 16 4 --step --ghost --random 0.8 --generations 12 
	--max-update 10 --seed Seeds\target_16x16.seed --output ..\Output\output.seed"
"dotnet life.dll --survival 9...8 --birth 8 6 9...12 --neighbour vonneumann 2 true 
	--seed Seeds\target2_16x16.seed --dimensions 48 48 --step"

If any of the arguments are not put in properly the defaults will be used.

## Notes 
1. The program uses spacebar for progressing throughout
2. When step mode is used the program will ignore the max update rate when will progress through 
   the generations with spacebar
3. When input file is used the program will ignore the random setting
4. The program will stop early if a steady state is detected

