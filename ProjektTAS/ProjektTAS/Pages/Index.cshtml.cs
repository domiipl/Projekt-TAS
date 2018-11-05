using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using System.Linq;

namespace ProjektTAS.Pages
{
    public class IndexModel : PageModel
    {
        public void OnGet()
        {

        }

        public object Categories()
        {
            Classes.MySQLObject mySQL = new Classes.MySQLObject();
            mySQL.Select($@"select * from `projekt_mysql`.`kategoria`");
            if (mySQL.Data.Rows.Count > 0) return Newtonsoft.Json.JsonConvert.SerializeObject(Classes.StaticMethods.ParseSelect(mySQL.Data));
            else return null;
        }
    }
}