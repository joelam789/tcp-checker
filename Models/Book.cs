using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using NPoco;

namespace TcpChecker.Models
{
    [TableName("tbl_book")]
    [PrimaryKey("book_id")] // [PrimaryKey("book_id,book_name")]
    public partial class Book
    {
        [Column("book_id")]
        public int BookId { get; set; }

        [Column("book_name")]
        public string BookName { get; set; }

        [Column("book_price")]
        public decimal BookPrice { get; set; }

        [Column("amount")]
        public int Amount { get; set; }

        //[Ignore]
        //public string Remark { get; set; }
    }
}
