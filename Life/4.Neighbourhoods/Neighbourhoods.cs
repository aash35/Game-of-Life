using System;
using System.Collections.Generic;
using System.Text;

namespace Life
{
    public abstract class Neighbourhoods
    {
        public int aliveNeighbours { get; set; }
        /// <summary>
        /// abstract class the counts the neighbouring cells if they are alive
        /// </summary>
        /// <param name="row">the row of the universe the is being queried</param>
        /// <param name="column">the column of the universe the is being queried</param>
        /// <param name="universeSettings">the settings of the universe</param>
        /// <param name="universe">the 2d array that is used to set the cells that are alive or dead</param>
        public abstract void CountNeighbours(int row, int column, Settings universeSettings, int[,] universe);
        /// <summary>
        /// check the rows and columns of the universe 2d array and if the cells are set to 1 then aliveNeighbours is
        /// incremented. this number is used to determine if a cell is born or survives the next generation
        /// </summary>
        /// <param name="universeSettings">the settings of the universe. used to check if the universe is 
        /// periodic or not</param>
        /// <param name="universe">the 2d universe array the has all the alive and dead cells that are used to
        /// render the generations. used to loop through each row and column to determine the state 
        /// of the neighbours</param>
        /// <param name="rowNeighbour">the neighbouring row that is being queried</param>
        /// <param name="columnNeighbour">the neighbouring column that is being queried</param>
        public void FinalCheck(Settings universeSettings, int[,] universe, int rowNeighbour, int columnNeighbour)
        {
            if (universeSettings.periodic != true)
            {
                if ((rowNeighbour >= 0 && rowNeighbour < universe.GetLength(0))
                    && (columnNeighbour >= 0 && columnNeighbour < universe.GetLength(1)))
                {
                    if (universe[rowNeighbour, columnNeighbour] == 1)
                    {
                        aliveNeighbours++;
                    }
                }
            }
            //if periodic setting is on then the rows and column loop around to the other side of the universe
            else
            {
                int periodicRow = rowNeighbour;
                int periodicColumn = columnNeighbour;
                if (rowNeighbour < 0)
                {
                    periodicRow = rowNeighbour + universe.GetLength(0);
                }
                else if (rowNeighbour > (universe.GetLength(0) - 1))
                {
                    periodicRow = rowNeighbour - universe.GetLength(0);
                }
                if (columnNeighbour < 0)
                {
                    periodicColumn = columnNeighbour + universe.GetLength(1);
                }
                else if (columnNeighbour > (universe.GetLength(1) - 1))
                {
                    periodicColumn = columnNeighbour - universe.GetLength(1);
                }
                if (universe[periodicRow, periodicColumn] == 1)
                {
                    aliveNeighbours++;
                }
            }

        }


    }
    

}
