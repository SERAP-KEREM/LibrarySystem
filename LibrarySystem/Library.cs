using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace LibrarySystem
{// kütüphanemizde kullanılan fonksiyonlar
    public class Library
    {
        private List<Book> books;
        private List<UserData> userDatas;

        public Library()
        {
            books = new List<Book>();
            userDatas = new List<UserData>();
            LoadData();
        }

        public void AddBook()//kütüphanemize kitap eklediğimiz fonksiyon 
        {
            Console.Write("Kitap Başlığı: ");
            string title = Console.ReadLine();
            Console.Write("Yazar: ");
            string author = Console.ReadLine();
            Console.Write("ISBN: ");
            string isbn = Console.ReadLine();
            Console.Write("Toplam Kopya Sayısı: ");
            int totalCopies = int.Parse(Console.ReadLine());

            Book newBook = new Book
            {
                Title = title,
                Author = author,
                ISBN = isbn,
                TotalCopies = totalCopies,
                BorrowedCopies = 0
            };
            books.Add(newBook);
            SaveData();//alınan bilgileri text dosyasına kaydeder
        }

        public void ListAllBooks()//tüm kitapları listele
        {
            Console.WriteLine("\nTüm Kitaplar");
            foreach (var book in books)
            {
                Console.WriteLine($"Başlık: {book.Title}, Yazar: {book.Author}, ISBN: {book.ISBN}, Toplam Kopya: {book.TotalCopies}, Ödünç Alınan Kopya: {book.BorrowedCopies}");
            }
        }

        public void SearchBook()//kitap adına veya yazar adına göre kitap arama işlemi yapılır
        {
            Console.Write("Aranacak Kitap Başlığı veya Yazarı: ");
            string searchQuery = Console.ReadLine();

            var results = books.Where(b => b.Title.Contains(searchQuery, StringComparison.OrdinalIgnoreCase) || b.Author.Contains(searchQuery, StringComparison.OrdinalIgnoreCase)).ToList();//kitapları büyük-küçük harf duyarlılığı olmadan arama işlemi yapar (linq)

            if (results.Count > 0)
            {
                Console.WriteLine("\nArama Sonuçları");
                foreach (var result in results)
                {
                    Console.WriteLine($"Başlık: {result.Title}, Yazar: {result.Author}, ISBN: {result.ISBN}, Toplam Kopya: {result.TotalCopies}, Ödünç Alınan Kopya: {result.BorrowedCopies}");
                }
            }
            else
            {
                Console.WriteLine("Arama sonuçları bulunamadı.");
            }
        }

        public void BorrowBook() //kitap ödünç alma işlemi
        {
            Console.Write("İsminiz: ");
            string userName = Console.ReadLine();
            Console.Write("Ödünç Almak İstediğiniz Kitabın Başlığı: ");
            string borrowTitle = Console.ReadLine();

            var book = books.Find(b => b.Title.Equals(borrowTitle, StringComparison.OrdinalIgnoreCase));//alınacak kitap ismini listede arar

            if (book == null)
            {
                Console.WriteLine("Kitap bulunamadı.");
                return;
            }

            var user = userDatas.Find(u => u.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase));//kullanıcının daha önceden alıp iade etmediği kitap var mı diye kontrol edilir

            if (user != null && user.ReturnDate > DateTime.Now)
            {
                Console.WriteLine($"Bu kullanıcıya ait ödünç alınmış kitap bulunmaktadır. Kitabı iade etmeden yeni kitap alamazsınız.");
                return;
            }

            if ( book.TotalCopies>0)//alınabilecek yeterli kitap var mı
            {
                book.BorrowedCopies++;//ödünç alınan kitabı arttır
                book.TotalCopies--;//toplam sayıdan eksilt
                if (user == null)//eğer daha önceden böyle bir kayıt yoksa user dataya kaydeder
                {
                    user = new UserData { UserName = userName };
                    userDatas.Add(user);
                }

                user.BookTitle = borrowTitle;
                user.BorrowDate = DateTime.Now;
                user.ReturnDate = DateTime.Now.AddDays(14);//kitabın geri iade edileceği süreyi hesaplıyoruz

                Console.WriteLine($"'{userName}' isimli kişi, '{book.Title}' isimli kitabı ödünç aldı. İade tarihi: {user.ReturnDate}");

                SaveData();//aldğımız bilgileri  düzenler ve kaydeder 
              //  SaveUserData();////////sil
            }
            else
            {
                Console.WriteLine("Özür dileriz, bu kitap şu anda ödünç alınamaz.");
            }
        }

        public void ReturnBook()// kitap iade eden fonksiyon 
        {
            Console.Write("İsminiz: ");
            string userName = Console.ReadLine();

            var user = userDatas.Find(u => u.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase));//kullanıcının ismini arar

            if (user == null)//eğer userdata da bu kişi yoksa kitap almamış demektir
            {
                Console.WriteLine("Bu kullanıcıya ait ödünç alınmış kitap bulunamadı.");
                return;
            }

            var book = books.Find(b => b.Title.Equals(user.BookTitle, StringComparison.OrdinalIgnoreCase));//aldığı kitap bulunur

            //şu andaki zamanı kullanarak geç iade kontrolü yapılır
            if (DateTime.Now > user.ReturnDate)
            {
                double ceza = Math.Ceiling((DateTime.Now - user.ReturnDate).TotalDays) * 2;//kaç gün geç getirdiyse her gün için 2tl ceza öder
                Console.WriteLine($"'{userName}' isimli kişi, '{book.Title}' isimli kitabı {Math.Ceiling((DateTime.Now - user.ReturnDate).TotalDays)} gün geç iade etti. Ceza: {ceza} TL");
            }
            else
            {
                Console.WriteLine($"'{userName}' isimli kişi, '{book.Title}' isimli kitabı zamanında iade etti.");
            }

            book.BorrowedCopies--;//iade olduğu için ödünç verilen kitap sayısı bir düşer
            book.TotalCopies++;//toplam sayı artar

            userDatas.Remove(user);//kullanıcı kitabı iade ettiği için userdata text dosyasından artık silinir

            SaveData();
          ///////  SaveUserData();////////sil
        }

        public void ShowOverdueBooks()//süresi geçen kitaplar hesaplanır
        {
            var overdueBooks = userDatas.Where(u => u.ReturnDate < DateTime.Now && u.ReturnDate > DateTime.MinValue).ToList();//tüm kitaplar için iade edilmeyen gün sayısı hesaplanır (return date = 14) eğer 14'ten fazla ise geç iade edilmiştir

            if (overdueBooks.Count > 0)//geç iade edilen gün sayısı 0'dan büyükse yazdırır
            {
                Console.WriteLine("\nSüresi Geçmiş Kitaplar");
                foreach (var overdueBook in overdueBooks)
                {
                    var book = books.Find(b => b.Title.Equals(overdueBook.BookTitle, StringComparison.OrdinalIgnoreCase));

                    Console.WriteLine($"Kullanıcı: {overdueBook.UserName}, Kitap: {overdueBook.BookTitle}, İade Tarihi: {overdueBook.ReturnDate}");
                }
            }
            else
            {
                Console.WriteLine("Süresi geçmiş kitap bulunmamaktadır.");
            }
        }

        private void LoadData()//verileri alır
        {
            LoadBooks();
            LoadUsers();
        }

        private void LoadBooks()//LibraryList isimli dosyadakileri okur
        {
            if (File.Exists("LibraryList.txt"))
            {
                using (StreamReader reader = new StreamReader("LibraryList.txt"))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        string[] data = line.Split(',');
                        Book book = new Book
                        {
                            Title = data[0],
                            Author = data[1],
                            ISBN = data[2],
                            TotalCopies = int.Parse(data[3]),
                            BorrowedCopies = int.Parse(data[4])
                        };
                        books.Add(book);
                    }
                }
            }
        }

        private void LoadUsers()//UserData isimli dosyadakileri okur
        {
            if (File.Exists("UserData.txt"))
            {
                using (StreamReader reader = new StreamReader("UserData.txt"))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {

                        string[] parts = line.Split(',');

                        UserData loadedUser = new UserData
                        {
                            UserName = parts[0],
                            BookTitle = parts[1],
                            BorrowDate = DateTime.Parse(parts[2]),
                            ReturnDate = DateTime.Parse(parts[3])
                        };

                        userDatas.Add(loadedUser);
                    }
                }
            }
        }

        public void SaveData()//bilgileri kaydeder
        {
            SaveBooks();
            SaveUserData();
        }

        private void SaveBooks()//LibraryList dosyasında bir değişiklik yaptığımızda değişiklikleri kaydeder
        {
            using (StreamWriter writer = new StreamWriter("LibraryList.txt"))
            {
                foreach (var book in books)
                {
                    writer.WriteLine($"{book.Title},{book.Author},{book.ISBN},{book.TotalCopies},{book.BorrowedCopies}");
                }
            }
        }

        private void SaveUserData()//UserData dosyasında bir değişiklik yaptığımızda değişiklikleri kaydeder
        {
            using (StreamWriter writer = new StreamWriter("UserData.txt"))
            {
                foreach (var user in userDatas)
                {
                    writer.WriteLine($"{user.UserName},{user.BookTitle},{user.BorrowDate},{user.ReturnDate}");
                }
            }
        }
    }
}
