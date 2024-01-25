using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibrarySystem
{
    //kütüphanede kayıtlı olan kitapların bilgilerini tutan class
   public class Book
    {
        public string Title { get; set; }
        public string Author { get; set; }
        public string ISBN { get; set; }
        public int TotalCopies { get; set; }
        public int BorrowedCopies { get; set; }
        public DateTime BorrowDate { get; set; }
    }
}
