using System;
using System.Collections.Generic;
using System.IO;
using LibrarySystem;

class Program
{
    static void Main()
    {
        Library library = new Library();
      
            while (true)
            {
                Console.WriteLine("\nKütüphane Yönetim Sistemi");
                Console.WriteLine("1. Yeni Kitap Ekle");
                Console.WriteLine("2. Tüm Kitapları Listele");
                Console.WriteLine("3. Kitap Ara");
                Console.WriteLine("4. Kitap Ödünç Al");
                Console.WriteLine("5. Kitap İade Et");
                Console.WriteLine("6. Süresi Geçmiş Kitapları Görüntüle");
                Console.WriteLine("7. Çıkış");

                Console.Write("Yapmak istediğiniz işlemi seçin (1-7): ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                    library.AddBook();
                    break;
                    case "2":
                    library.ListAllBooks();
                        break;
                    case "3":
                        library.SearchBook();
                        break;
                    case "4":
                        library.BorrowBook();
                        break;
                    case "5":
                    library.ReturnBook();
                        break;
                    case "6":
                    library.ShowOverdueBooks();
                        break;
                    case "7":
                    library.SaveData();
                        Console.WriteLine("Programdan çıkılıyor...");
                        return;
                    default:
                        Console.WriteLine("Geçersiz bir seçenek girdiniz. Lütfen tekrar deneyin.");
                        break;
                }
            }
        }
    }
        
    
