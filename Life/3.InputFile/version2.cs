using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Life
{
    class Version2 : Version1
    {
        public List<int[]> deadCells { get; set; } = new List<int[]>();
        /// <summary>
        /// the constructor that inherits the function of the version one constructor
        /// </summary>
        /// <param name="reader">the reader object first used to read the file</param>
        /// <param name="line">the first line the show which version the file is</param>
        /// <param name="universe">the 2d array that is used to set the cells that are alive or dead</param>
        public Version2(TextReader reader, string line, int[,] universe) : base(reader, line, universe)
        {

        }
        /// <summary>
        /// method that overrides the version1 CalculateCells method.  versio2 files are structured differenly and
        /// therefore must be read in a different matter. each line is read and depending of the property it possesses
        /// a different method is called to determine the alive or dead cells. After each line is read the universe is
        /// set
        /// </summary>
        /// <param name="universe">the 2d array that is used to set the cells that are alive or dead</param>
        public override void CalculateCells(int[,] universe)
        {
            while ((line = reader.ReadLine()) != null)
            {
                if (line.Contains("cell"))
                {
                    Cell();
                    SetUniverse(universe);
                }
                else if (line.Contains("rectangle"))
                {
                    Rectangle();
                    SetUniverse(universe);
                }
                else if (line.Contains("ellipse"))
                {
                    Ellipse();
                    SetUniverse(universe);
                }
            }
        }
        /// <summary>
        /// works the same as the SetUniverse method from version1 however it can also set cells to be alive(1)
        /// or dead(0) based on what the file reads
        /// </summary>
        /// <param name="universe">the 2d array that is used to set the cells that are alive or dead</param>
        /// <param name="cells">either the dead or alive array depending on what is passed to the method</param>
        /// <param name="set">the int value of either 0 or 1 depending on if the cell is to be alive or dead</param>
        public void SetState(int[,] universe, List<int[]> cells, int set)
        {
            string dimensionsError = "";
            string rowSizeError = "";
            bool columnSizeError = false;
            int numberColumnSizeError = 0;

            for (int i = 0; i < cells.Count(); i++)
            {
                if (universe.GetLength(0) > cells[i][0] && universe.GetLength(1)
                            > cells[i][1])
                {
                    universe[cells[i][0], cells[i][1]] = set;
                }
                else if (universe.GetLength(0) < (cells[i][0] + 1))
                {
                    rowSizeError = "there must be at least " + (cells[i][0] + 1)
                        + " rows to fit file settings";
                }
                else if (universe.GetLength(1) < (cells[i][1] + 1))
                {
                    if (cells[i][1] > numberColumnSizeError)
                    {
                        numberColumnSizeError = (cells[i][1] + 1);
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
        /// <summary>
        /// overrides from the version1 method however calls the SetState method based on if the array that was added
        /// to in the reading of the file was the alive or dead array
        /// </summary>
        /// <param name="universe">the 2d array that is used to set the cells that are alive or dead</param>
        public override void SetUniverse(int[,] universe)
        {
            if (aliveCells.Count() > 0)
            {
                SetState(universe, aliveCells, 1);
            }
            if (deadCells.Count() > 0)
            {
                SetState(universe, deadCells, 0);
            }
            aliveCells.Clear();
            deadCells.Clear();
        }
        /// <summary>
        /// method that is called if the line read is states its a cell. simple row and column coordinates that
        /// determine which cell is alive or dead
        /// </summary>
        public void Cell()
        {
            List<string> Cells = new List<string>();

            Cells = line.Split(" ").ToList();
            string state = Cells[0];
            Cells.RemoveAt(0);
            Cells.RemoveAt(0);
            Cells[0] = Cells[0].TrimEnd(',');
            if (Cells.Count() > 2)
            {
                throw new Exception("file format is wrong (too many points)");
            }
            else
            {
                int rowValidator;
                int columnValidator;
                bool checkerRow = Int32.TryParse(Cells[0], out rowValidator);
                bool checkerColumn = Int32.TryParse(Cells[1], out columnValidator);
                if (checkerRow == true && checkerColumn == true)
                {
                    int[] RowAndColumn = new int[] { rowValidator, columnValidator };
                    if (state.Contains("(o)"))
                    {
                        aliveCells.Add(RowAndColumn);
                    }
                    else if (state.Contains("(x)"))
                    {
                        deadCells.Add(RowAndColumn);

                    }
                }
                else
                {
                    throw new Exception("file format is wrong (points not numbers)");
                }

            }

        }
        /// <summary>
        /// method that is called if the line read is states its a rectangle. states four points the determine the
        /// two corners of the rectangle. loops through the cells between these point and sets then to alive or dead
        /// depending of the line
        /// </summary>
        public void Rectangle()
        {
            List<string> Cells = new List<string>();

            Cells = line.Split(" ").ToList();
            string state = Cells[0];
            Cells.RemoveAt(0);
            Cells.RemoveAt(0);
            Cells.RemoveAt(0);
            for (int i = 0; i < Cells.Count(); i++)
            {
                Cells[i] = Cells[i].TrimEnd(',');
            }
            if (Cells.Count() > 4)
            {
                throw new Exception("file format is wrong (too many points)");
            }
            else
            {
                int bottomLeftValidator;
                int bottomRightValidator;
                int topLeftValidator;
                int topRightValidator;
                bool checkerBottomLeft = Int32.TryParse(Cells[0], out bottomLeftValidator);
                bool checkerBottomRight = Int32.TryParse(Cells[1], out bottomRightValidator);
                bool checkerTopLeft = Int32.TryParse(Cells[2], out topLeftValidator);
                bool checkerTopRight = Int32.TryParse(Cells[3], out topRightValidator);
                if (checkerBottomLeft == true && checkerBottomRight == true && checkerTopLeft == true
                    && checkerTopRight == true)
                {
                    for (int i = bottomLeftValidator; i < topRightValidator + 1; i++)
                    {
                        for (int j = bottomRightValidator; j < topLeftValidator + 1; j++)
                        {
                            int[] RowAndColumn = new int[] { i, j };
                            if (state.Contains("(o)"))
                            {
                                aliveCells.Add(RowAndColumn);
                            }
                            else if (state.Contains("(x)"))
                            {
                                deadCells.Add(RowAndColumn);
                            }
                        }
                    }
                }
                else
                {
                    throw new Exception("file format is wrong (points not numbers)");
                }

            }
        }
        /// <summary>
        /// method that is called if the line read is states its a ellipse. states four points the determine the
        /// two corners of the ellipse. it loops through the cells like the retangle however determines if the cells
        /// are within the circle based on some math and then sets the cells alive or dead depending of the line
        /// </summary>
        public void Ellipse()
        {
            List<string> Cells = new List<string>();
            List<string> cellsHolder = new List<string>();

            Cells = line.Split(" ").ToList();
            string state = Cells[0];
            cellsHolder.Add(Cells[Cells.Count()-4]);
            cellsHolder.Add(Cells[Cells.Count() - 3]);
            cellsHolder.Add(Cells[Cells.Count() - 2]);
            cellsHolder.Add(Cells[Cells.Count() - 1]);
            Cells.Clear();
            Cells.AddRange(cellsHolder);
            for (int i = 0; i < Cells.Count(); i++)
            {
                Cells[i] = Cells[i].TrimEnd(',');
            }
            if (Cells.Count() > 4)
            {
                throw new Exception("file format is wrong (too many points)");
            }
            else
            {
                float bottomRow;
                float bottomColumn;
                float topRow;
                float topColumn;
                float[] centrePoint = new float[2];
                bool checkerBottomLeft = float.TryParse(Cells[0], out bottomRow);
                bool checkerBottomRight = float.TryParse(Cells[1], out bottomColumn);
                bool checkerTopLeft = float.TryParse(Cells[2], out topRow);
                bool checkerTopRight = float.TryParse(Cells[3], out topColumn);
                if (checkerBottomLeft == true && checkerBottomRight == true && checkerTopLeft == true
                    && checkerTopRight == true)
                {
                    centrePoint[0] = (topRow + bottomRow) / 2;
                    centrePoint[1] = (topColumn + bottomColumn) / 2;
                    float cellStater = 0;

                    for (float i = bottomRow; i < topRow + 1; i++)
                    {
                        for (float j = bottomColumn; j < topColumn + 1; j++)
                        {
                            cellStater = (4 * ((float)Math.Pow((i - centrePoint[0]), 2)) / (float)Math.Pow((topRow-bottomRow + 1), 2))
                            + (4 * ((float)Math.Pow((j - centrePoint[1]), 2)) / (float)Math.Pow((topColumn-bottomColumn+1), 2));
                            if (cellStater <= 1)
                            {
                                int[] RowAndColumn = new int[] { (int)i, (int)j };

                                if (state.Contains("(o)"))
                                {
                                    aliveCells.Add(RowAndColumn);
                                }
                                else if (state.Contains("(x)"))
                                {
                                    deadCells.Add(RowAndColumn);
                                }

                            }
                        }
                    }
                }
                else
                {
                    throw new Exception("file format is wrong (points not numbers)");
                }

            }

        }

    }
}
