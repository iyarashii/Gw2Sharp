// Copyright (c) 2022 iyarashii @ https://github.com/iyarashii 
// Licensed under the GNU General Public License v3.0,
// go to https://github.com/iyarashii/Gw2Sharp/blob/master/LICENSE for license details.

using SQLite;

namespace Gw2Sharp.Models.DTOs
{
    public class LastApiPageNumber
    {
        public int ApiPageNumber { get; set; }

        [PrimaryKey]
        public int Id { get; set; }

        public LastApiPageNumber(int apiPageNumber, int id)
        {
            ApiPageNumber = apiPageNumber;
            Id = id;
        }

        public LastApiPageNumber()
        {

        }
    }
}
