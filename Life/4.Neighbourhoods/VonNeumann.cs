using System;
using System.Collections.Generic;
using System.Text;

namespace Life
{
    class VonNeumann : Neighbourhoods
    {
        /// <summary>
        /// counts the neighbours of the cell in question using the VonNeumann neighbourhood pattern.
        /// the VonNeumann pattern checks select cells surrounding the cell in question. It does this by determining
        /// the distance from the centre point then finding the range of columns both left and right
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
            int centrePoint = row;
            for (int rowNeighbour = (row - order); rowNeighbour <= (row + order); rowNeighbour++)
            {
                int distance = row - rowNeighbour;
                int range = 0;
                if(distance > 0)
                {
                    range = Math.Abs(distance - order);

                }
                else if (distance <= 0)
                {
                    range = distance + order;
                }
                for (int columnNeighbour = (column - range); columnNeighbour <= (column + range); columnNeighbour++)
                {
                    if (universeSettings.neighbourhoodCentre == true)
                    {
                        FinalCheck(universeSettings, universe, rowNeighbour, columnNeighbour);

                    }
                    else if (universeSettings.neighbourhoodCentre == false)
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
