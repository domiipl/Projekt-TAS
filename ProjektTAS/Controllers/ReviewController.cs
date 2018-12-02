using Microsoft.AspNetCore.Mvc;
using ProjektTAS.Classes;
using System;

namespace ProjektTAS.Controllers
{
    /// <summary>
    /// Kontroler ocen
    /// 
    /// POST rest/v1/review/create
    /// Wymaga headera Authorization
    /// {
    ///     "productid" : string required,
    ///     "review" : string required,
    ///     "rating" : string required
    /// }
    /// 
    /// GET rest/v1/review/product/{product id}
    /// 
    /// GET rest/v1/review/product/{product id}/{ilość wyników}/{przesunięcie wyników o offset}
    /// 
    /// GET rest/v1/review/productaverage/{product id}
    /// 
    /// DELETE rest/v1/review/delete/{id produktu}
    /// Wymaga headera Authorization
    /// 
    /// </summary>
    [Produces("application/json")]
    [Route("rest/v1/[controller]/[action]")]
    public class ReviewController : Controller
    {
        [HttpGet("{productId}")]
        public object Product(string productId) => ProductReviews(productId);

        [HttpGet("{productId}/{limit}/{offset}")]
        public object Product(string productId, int limit = 10, int offset = 0) => ProductReviews(productId, limit, offset);

        private object ProductReviews(string productId, int limit = 0, int offset = 0)
        {
            MySQLObject mySQL = new MySQLObject();
            mySQL.Select($@"select * from `oceny` where `id` = {productId} limit {limit} offset {offset}");
            if (mySQL.Data.Rows.Count > 0)
            {
                return StatusCode(200, StaticMethods.ParseSelect(mySQL.Data));
            }
            else
            {
                return StatusCode(200, @"{""Result"" : ""No reviews or you went too far away""}");
            }
        }

        [HttpGet("{productId}")]
        public object ProductAverage(string productId)
        {
            MySQLObject mySQL = new MySQLObject();
            mySQL.Select($@"select `id_przedmiotu`,avg(`ocena`) from `oceny` where `id` = {productId}");
            if (mySQL.Data.Rows.Count > 0)
            {
                return StatusCode(200, StaticMethods.ParseSelect(mySQL.Data));
            }
            else
            {
                return StatusCode(200, @"{""Result"" : ""No reviews""}");
            }
        }

        [HttpDelete("{productId}")]
        public object Delete(string productId)
        {
            if (Request.Headers.TryGetValue("Authorization", out Microsoft.Extensions.Primitives.StringValues value) && value.ToString().Contains("Bearer "))
            {
                MySQLObject mySQL = new MySQLObject();
                mySQL.Select($@"select case when `id_uzytkownika` = {StaticMethods.GetUserId(value.ToString().Substring(7))} then 'true' else 'false' end as `check` from `oceny` where `id` = {productId}");
                if (mySQL.Data.Rows.Count > 0 && mySQL.Data.Rows[0]["check"].ToString() == "true")
                {
                    mySQL.Delete($@"delete from `oceny` where `id` = {productId}");
                    mySQL.Select($@"select `id` from `oceny` where `id` = {productId}");
                    if (mySQL.Data.Rows.Count > 0)
                    {
                        return StatusCode(500, @"{""Result"" : ""Something went terribly wrong""}");
                    }
                    else
                    {
                        return StatusCode(200, @"{""Result"" : ""Product deleted sucessfully""}");
                    }
                }
                else
                {
                    return StatusCode(403, @"{""Result"": ""Unauthorized""}");
                }
            }
            else
            {
                return StatusCode(403, @"{""Result"":""Unauthorized""}");
            }
        }

        [HttpPost]
        public object Create([FromBody]Review review)
        {
            if (Request.Headers.TryGetValue("Authorization", out Microsoft.Extensions.Primitives.StringValues value) && value.ToString().Contains("Bearer ") && review != null && review.productId != null && review.rating != null && review.review != null && review.rating >= 0 && review.rating <= 10)
            {
                int? userId = StaticMethods.GetUserId(value.ToString().Substring(7));
                if (userId != null)
                {
                    MySQLObject mySQL = new MySQLObject();
                    review.userId = userId;
                    mySQL.Select($@"select `id` from `oceny` where `id_uzytkownika` = {userId} and `id_przedmiotu` = {review.productId}");
                    string reviewId = "default";
                    if (mySQL.Data.Rows.Count > 0)
                    {
                        reviewId = mySQL.Data.Rows[0]["id"].ToString();
                    }
                    mySQL.Replace($@"REPLACE INTO `oceny` values ({reviewId}, {userId}, {review.productId}, '{review.review.Replace("'", "")}',{review.rating})");
                    if (reviewId != "default")
                    {
                        review.id = Convert.ToInt32(reviewId);
                        return StatusCode(200, review);
                    }
                    else
                    {
                        try
                        {
                            mySQL.Select($@"select `id` from `oceny` where `id_uzytkownika` = {userId} and `id_przedmiotu` = {review.productId}");
                            review.id = Convert.ToInt32(mySQL.Data.Rows[0]["id"].ToString());
                            return StatusCode(200, review);
                        }
                        catch
                        {
                            return StatusCode(500, @"{""Result"":""Something went terribly wrong""}");
                        }
                    }
                }
                else
                {
                    return StatusCode(403, @"{""Result"":""Token is invalid""}");
                }
            }
            else
                return StatusCode(400);
        }
    }

    public class Review
    {
        public int? id;
        public int? userId;
        public int? productId;
        public string review;
        public int? rating;
    }
}