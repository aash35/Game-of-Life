using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Life
{
    public class Version1
    {
        public TextReader reader { get; set; }
        public List<int[]> aliveCells { get; set; } = new List<int[]>();
        public string line { get; set; }

        /// <summary>
        /// the constructor that sets the text reader to be the same object as the one first used to determine if the
        /// file was 1.0 or 2.0. This then runs the CalculateCells method determines which cell are alive or dead
        /// </summary>
        /// <param name="reader">the reader object first used to read the file</param>
        /// <param name="line">the first line the show which version the file is</param>
        /// <param name="universe">the 2d array that is used to set the cells that are alive or dead</param>
        public Version1(TextReader reader, string line, int[,] universe)
        {
            this.reader = reader;
            this.line = line;
            CalculateCells(universe);
        }
        /// <summary>
        /// reads each line of the file and adds the cells that are alive to an array which is used to set the intial
        /// conditions of the universe
        /// </summary>
        /// <param name="universe">the 2d array that is used to set the cells that are alive or dead</param>
        public virtual void  CalculateCells(int[,] universe)
        {
            while ((line = reader.ReadLine()) != null)
            {
                string[] aliveCellArray = line.Split(" ");
                if (aliveCellArray.Length > 2)
                {
                    throw new Exception("file format is wrong (too many points)");
                }
                else
                {
                    int rowValidator;
                    int columnValidator;
                    bool checkerRow = Int32.TryParse(aliveCellArray[0], out rowValidator);
                    bool checkerColumn = Int32.TryParse(aliveCellArray[1], out columnValidator);
                    if (checkerRow == true && checkerColumn == true)
                    {
                        int[] aliveRowAndColumn = new int[] { rowValidator, columnValidator };
                        aliveCells.Add(aliveRowAndColumn);
                    }
                    else
                    {
                        throw new Exception("file format is wrong (points not numbers)");
                    }

                }
            }
            SetUniverse(universe);
        }
        /// <summary>
        /// sets the intial state of the universe by selecting the row and column's that need to be set alive (1)
        /// </summary>
        /// <param name="universe">the 2d array that is used to set the cells that are alive or dead</param>
        public virtual void SetUniverse(int[,] universe)
        {
            string dimensionsError = "";
            string rowSizeError = "";
            bool columnSizeError = false;
            int numberColumnSizeError = 0;

            for (int i = 0; i < aliveCells.Count(); i++)
            {
                if (universe.GetLength(0) > aliveCells[i][0] && universe.GetLength(1)
                            > aliveCells[i][1])
                {
                    universe[aliveCells[i][0], aliveCells[i][1]] = 1;
                }
                else if (universe.GetLength(0) < (aliveCells[i][0] + 1))
                {
                    rowSizeError = "there must be at least " + (aliveCells[i][0] + 1)
                        + " rows to fit file settings";
                }
                else if (universe.GetLength(1) < (aliveCells[i][1] + 1))
                {
                    if (aliveCells[i][1] > numberColumnSizeError)
                    {
                        numberColumnSizeError = (aliveCells[i][1] + 1);
                        columnSizeError = true;
                    }
                }
            }
            if (rowSizeError.Length > 0 || columnSizeError == true)
            {
                dimensionsError = ((rowSizeError.Length > 0 ? rowSizeError + " " : "")
                    + (rowSizeError.Length > 0 && columnSizeError == true ? "and " : "")
                    + (columnSizeError == true
                    ? ("there must be at least " + numberColumnSizeError.ToString()
                    + " columns to fit file settings") : ""));
                throw new Exception(dimensionsError);
            }
        }


    }
}
