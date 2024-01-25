using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace LibrarySystem
{
    //Ödünç alınan kitapları ve ödünç alan kişilerin bilgilerini tutan class
    public class UserData
    {
        public string UserName { get; set; } 
        public string BookTitle { get; set; }
        public DateTime BorrowDate { get; set; }
        public DateTime ReturnDate { get; set; }

    
    }
}
