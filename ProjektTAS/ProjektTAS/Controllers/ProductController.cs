using Microsoft.AspNetCore.Mvc;
using ProjektTAS.Classes;
using System;
using System.Linq;

namespace ProjektTAS.Controllers
{
    /// <summary>
    /// kontroler produktów
    /// POST /rest/v1/product/create 
    /// {
    ///     "CategoryId" : int required,
    ///     "UserId" : int required,
    ///     "Name" : string,
    ///     "Price" : decimal
    /// }
    /// </summary>
    [Produces("application/json")]
    [Route("rest/v1/[controller]/[action]")]
    public class ProductController : Controller
    {
        [HttpPost]
        public object Create([FromBody]Product product)
        {
            if (Request.Headers.TryGetValue("Authorization", out Microsoft.Extensions.Primitives.StringValues value) && value.ToString().Contains("Bearer ") && product.Name != null && product.CategoryId != null && product.Price != null)
            {
                MySQLObject mySQL = new MySQLObject();
                var data = mySQL.Select($@"select t1.`id_uprawnienia` 
                                                                from `projekt_mysql`.`tokeny_logowania` t0 
                                                                inner join `projekt_mysql`.`uzytkownicy` t1 
                                                                on t0.`id_uzytkownika` = t1.`id_uzytkownika`
                                                                where t0.`token` = '{value.ToString().Replace("Bearer ", "")}' and t0.`aktywny` = 1 and NOW() < t0.`data_wygasniecia`");
                if (data.Rows.Count > 0 && new int[] { (int)Privileges.User, (int)Privileges.Administrator, (int)Privileges.Moderator }.Contains(Convert.ToInt32(data.Rows[0]["id_uprawnienia"])))
                {
                    try
                    {
                        mySQL.Insert($@"insert into `projekt_mysql`.`przedmiot`(`id_kategorii`,`id_uzytkownika`, `nazwa`,`cena`) values ({product.CategoryId},{StaticMethods.GetUserId(value.ToString().Replace("Bearer ", ""))},'{product.Name}',{product.Price.ToString().Replace(",", ".")})");
                        data = mySQL.Select($@"select max(`id_przedmiotu`) as `value` from `projekt_mysql`.`przedmiot` where `Id_kategorii` = {product.CategoryId} and `nazwa` = '{product.Name}' and `cena` = {product.Price.ToString().Replace(",", ".")}");
                        if (data.Rows.Count > 0)
                        {
                            product.Id = Convert.ToInt32(data.Rows[0]["value"]);
                            product.UserId = StaticMethods.GetUserId(value.ToString().Replace("Bearer ", ""));
                            return product;
                        }
                        else
                        {
                            return StatusCode(500, "Something went terribly wrong");
                        }
                    }
                    catch (Exception)
                    {
                        return StatusCode(500, "Something went terribly wrong");
                    }
                }
                else
                {
                    return StatusCode(403, "Method requires administrative privileges or your token is invalid");
                }
            }
            else
            {
                return StatusCode(400, "Wrong request");
            }
        }
    }
    public class Product
    {
        public int Id { get; set; }
        public int? CategoryId { get; set; }
        public int? UserId { get; set; }
        public string Name { get; set; }
        public decimal? Price { get; set; }
    }
}
