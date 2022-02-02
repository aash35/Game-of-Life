using System;
using System.Collections.Generic;
using System.Text;

namespace Life
{
    class Moore : Neighbourhoods
    {
        /// <summary>
        /// counts the neighbours of the cell in question using the Moore neighbourhood pattern.
        /// the Moore pattern checks all the surrounding cells
        /// </summary>
        /// <param name="row">the row being queried</param>
        /// <param name="column">the column being queried</param>
        /// <param name="universeSettings">the setting of the universe. used to find the neighbourhood order</param>
        /// <param name="universe">the 2d universe array the has all the alive and dead cells that are used to
        /// render the generations. used to loop through each row and column to determine the state 
        /// of the neighbours</param>
        public override void CountNeighbours(int row, int column, Settings universeSettings, int[,] universe)
        {
            aliveNeighbours = 0;
            int order = universeSettings.neighbourhoodOrder;
            for (int rowNeighbour = (row - order); rowNeighbour <= (row + order); rowNeighbour++)
            {
                for (int columnNeighbour = (column - order); columnNeighbour <= (column + order); columnNeighbour++)
                {
                    if(universeSettings.neighbourhoodCentre == true)
                    {
                        FinalCheck(universeSettings, universe, rowNeighbour, columnNeighbour);

                    }
                    else if(universeSettings.neighbourhoodCentre == false)
                    {
                        if (!(rowNeighbour == row && columnNeighbour == column))
                        {
                            FinalCheck(universeSettings, universe, rowNeighbour, columnNeighbour);
                        }

                    }
                    
                }
            }
        }

    }
}
