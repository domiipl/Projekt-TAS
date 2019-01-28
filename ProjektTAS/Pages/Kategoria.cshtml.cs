using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using System.Linq;

namespace ProjektTAS.Pages
{
    public class KategoriaModel : PageModel
    {
        public string idkategoria { get; set; }
        public void OnGet(string id)
        {
            idkategoria = id;
        }

        public object Categories()
        {
            Classes.MySQLObject mySQL = new Classes.MySQLObject();
            mySQL.Select($@"select * from `projekt_mysql`.`kategoria`");
            if (mySQL.Data.Rows.Count > 0) return Newtonsoft.Json.JsonConvert.SerializeObject(Classes.StaticMethods.ParseSelect(mySQL.Data));
            else return null;
        }
        public DataTable Kategorie()
        {
            Classes.MySQLObject mysql = new Classes.MySQLObject();
            return mysql.Select($@"select * from `projekt_mysql`.`kategoria` where `id_parent` is null");
        }
        public DataTable AllCategories()
        {
            Classes.MySQLObject mysql = new Classes.MySQLObject();
            return mysql.Select($@"select * from `projekt_mysql`.`kategoria` where `id` is not null");
        }
        public DataTable Produkty()
        {
            Classes.MySQLObject mysql = new Classes.MySQLObject();
            return mysql.Select($@"select * from `projekt_mysql`.`przedmiot` where `id_przedmiotu` is not null");
        }
        public DataTable CategoryId(string idkategoria)
        {
            Classes.MySQLObject mysql = new Classes.MySQLObject();
            return mysql.Select($@"select `id` from `projekt_mysql`.`kategoria` where `nazwa` = {idkategoria}");
        }
    }

}