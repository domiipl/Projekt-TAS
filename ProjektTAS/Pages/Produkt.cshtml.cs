using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using System.Linq;

namespace ProjektTAS.Pages
{
    public class ProduktModel : PageModel
    {
        public string idPrzedmiotu { get; set; }
        public void OnGet(string id)
        {
            idPrzedmiotu = id;
        }
        public DataTable Kategorie()
        {
            Classes.MySQLObject mysql = new Classes.MySQLObject();
            return mysql.Select($@"select * from `projekt_mysql`.`kategoria` where `id_parent` is null");
        }
        public DataTable Oceny(string idPrzedmiotu)
        {
            Classes.MySQLObject mysql = new Classes.MySQLObject();
            return mysql.Select($@"select t0.`ocena`,t0.`opinia`,t1.`email` from `projekt_mysql`.`oceny` t0 inner join `projekt_mysql`.`uzytkownicy` t1 on t0.`id_uzytkownika` = t1.`id_uzytkownika` where `id_przedmiotu` = {idPrzedmiotu}");
        }
        public DataTable Przedmiot(string idPrzedmiotu)
        {
            Classes.MySQLObject mysql = new Classes.MySQLObject();
            return mysql.Select($@"select t0.`id_przedmiotu`, t0.`nazwa` as `nazwa_produktu`, t0.`cena`, t1.`nazwa` as `nazwa_kategorii` from `projekt_mysql`.`przedmiot` t0 inner join `projekt_mysql`.`kategoria` t1 on t0.`id_kategorii` = t1.`id` where `id_przedmiotu` = {idPrzedmiotu}");
        }
    }
}