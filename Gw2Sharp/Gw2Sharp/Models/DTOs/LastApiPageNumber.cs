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
