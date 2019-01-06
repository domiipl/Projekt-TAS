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
            return mysql.Select($@"select * from `projekt_mysql`.`oceny` where `id_przedmiotu` = {idPrzedmiotu}");
        }
        public DataTable Przedmiot(string idPrzedmiotu)
        {
            Classes.MySQLObject mysql = new Classes.MySQLObject();
            return mysql.Select($@"select * from `projekt_mysql`.`przedmiot` where `id_przedmiotu` = {idPrzedmiotu}");
        }
    }
}