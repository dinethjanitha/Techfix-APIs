using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using techfixAPI.Models;

namespace techfixAPI.Controllers
{
    [Route("api/product")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public ProductsController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost]
        public async Task<IActionResult> AddProduct([FromBody] Product product)
        {
            string conString = _configuration.GetConnectionString("dconn");

            SqlConnection conn = new SqlConnection(conString);
            try
            {
                string sql = "INSERT INTO products(productname,productcatagory,quantity,productprice,shopName,productDes) " +
                    "VALUES(@ProductName,@ProductCatagory,@Quantity,@Productprice,@ShopName,@ProductDes)";

                SqlCommand cmd = new SqlCommand(sql, conn);

                cmd.Parameters.AddWithValue("@ProductName", product.productName);
                cmd.Parameters.AddWithValue("@ProductCatagory", product.productCategory);
                cmd.Parameters.AddWithValue("@Quantity", product.quantity);
                cmd.Parameters.AddWithValue("@Productprice", product.productprice);
                cmd.Parameters.AddWithValue("@ShopName", product.shopName);
                cmd.Parameters.AddWithValue("@ProductDes", product.productDescription);

                await conn.OpenAsync();

                await cmd.ExecuteNonQueryAsync();

                return Ok(new { message = "Product Add sucessfully!" });

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            finally {
                await conn.CloseAsync();

            }

        }

        [HttpGet]

        public async Task<IActionResult> AllProducts()
        {
            string conString = _configuration.GetConnectionString("dconn");

            SqlConnection conn = new SqlConnection(conString);

            try
            {
                string sql = "SELECT * FROM products";

                SqlCommand cmd = new SqlCommand(sql, conn);

                try
                {
                    await conn.OpenAsync();
                    SqlDataReader reader = cmd.ExecuteReader();

                    List<Product> products = new List<Product>();

                    while (reader.Read())
                    {
                        products.Add(new Product
                        {
                            ProductId = Convert.ToInt32(reader["productid"]),
                            productName = reader["productname"].ToString(),
                            productCategory = reader["productcatagory"].ToString(),
                            quantity = Convert.ToInt32(reader["quantity"]),
                            productprice = reader["productprice"].ToString(),
                            shopName = reader["shopName"].ToString(),
                            productDescription = reader["productDes"].ToString()
                        });


                    }

                    return Ok(products);
                }
                catch (Exception ex) {
                    return BadRequest(ex.Message);
                }

            }
            catch (Exception ex) {


            }

            return Ok();
        }

        [HttpPut("{id}")]

        public async Task<IActionResult> Update(int id, [FromBody] Product product)
        {
            if (id != product.ProductId)
            {
                return BadRequest("Product ID mismatch!");
            }

            string conString = _configuration.GetConnectionString("dconn");
            SqlConnection conn = new SqlConnection(conString);
            try
            {
                string sql = @"UPDATE products SET
                               productname = @ProductName,
                               productcatagory = @ProductCatagory,
                               quantity = @Quantity,
                               productprice = @Productprice,
                               productDes = @ProductDes
                               WHERE productid = @ProductID";

                SqlCommand cmd = new SqlCommand(sql, conn);

                cmd.Parameters.AddWithValue("@ProductName", product.productName);
                cmd.Parameters.AddWithValue("@ProductCatagory", product.productCategory);
                cmd.Parameters.AddWithValue("@Quantity", product.quantity);
                cmd.Parameters.AddWithValue("@Productprice", product.productprice);
                cmd.Parameters.AddWithValue("@ProductDes", product.productDescription);
                cmd.Parameters.AddWithValue("@ProductID", product.ProductId);

                await conn.OpenAsync();

                int affectedRows = await cmd.ExecuteNonQueryAsync();
                if (affectedRows == 1) {
                    return Ok(new { message = "Product Updated" });

                }
                else
                {
                    return BadRequest(new { message = "Prodcut Update Faild" });
                }



            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);

            }
            finally {
                await conn.CloseAsync();
            }



            return Ok(new { message = "OK" });
        }


        [HttpDelete("{id}")]

        public async Task<IActionResult> DeleteProduct(int id)
        {

            string conString = _configuration.GetConnectionString("dconn");

            SqlConnection conn = new SqlConnection(conString);

            try
            {
                string sql = "DELETE FROM products WHERE productid = @ProductId";

                SqlCommand cmd = new SqlCommand(sql, conn);

                cmd.Parameters.AddWithValue("@ProductId", id);

                await conn.OpenAsync();

                int affectedRow = await cmd.ExecuteNonQueryAsync();

                if (affectedRow == 1)
                {
                    return Ok(new { message = "Product Deleted!" });
                }
                else
                {
                    return BadRequest(new { message = "Product Deleted unscuessfull!" });
                }

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            } finally {
                await conn.CloseAsync();
            }
        }

       


        [HttpGet("{search}")]

        public async Task<IActionResult> searchproduct(string search)
        {
            string conString = _configuration.GetConnectionString("dconn");

            SqlConnection conn = new SqlConnection(conString);

            try
            {
                string sql = @"SELECT * FROM products WHERE productname LIKE @ProductName OR productcatagory LIKE 
                            @ProductCatagory OR productprice LIKE @ProductPrice OR shopName LIKE @ShopName";

                SqlCommand cmd = new SqlCommand(sql, conn);

                cmd.Parameters.AddWithValue("@ProductName", "%" + search + "%");
                cmd.Parameters.AddWithValue("@ProductCatagory", "%" + search + "%");
                cmd.Parameters.AddWithValue("@ProductPrice", "%" + search + "%");
                cmd.Parameters.AddWithValue("@ShopName", "%" + search + "%");

                await conn.OpenAsync();

                SqlDataReader reader = cmd.ExecuteReader();

                List<Product> products = new List<Product>();

                while (reader.Read())
                {
                    products.Add(new Product
                    {
                        ProductId = Convert.ToInt32(reader["productid"]),
                        productName = reader["productname"].ToString(),
                        productCategory = reader["productcatagory"].ToString(),
                        quantity = Convert.ToInt32(reader["quantity"]),
                        productprice = reader["productprice"].ToString(),
                        shopName = reader["shopName"].ToString(),
                        productDescription = reader["productDes"].ToString()
                    });
                }

                return Ok(products);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            finally
            {
                await conn.CloseAsync();
            }
        }

        [HttpGet("shop1")]

        public async Task<IActionResult> ShopOne() {
            string conString = _configuration.GetConnectionString("dconn");

            SqlConnection conn = new SqlConnection(conString);

            try
            {
                string sql = "SELECT * FROM products WHERE shopName = 'Dineth'";

                SqlCommand cmd = new SqlCommand(sql, conn);


                await conn.OpenAsync();

                SqlDataReader reader = cmd.ExecuteReader();

                List<Product> products = new List<Product>();

                while (reader.Read())
                {
                    products.Add(new Product
                    {
                        ProductId = Convert.ToInt32(reader["productid"]),
                        productName = reader["productname"].ToString(),
                        productCategory = reader["productcatagory"].ToString(),
                        quantity = Convert.ToInt32(reader["quantity"]),
                        productprice = reader["productprice"].ToString(),
                        shopName = reader["shopName"].ToString(),
                        productDescription = reader["productDes"].ToString()
                    });
                }

                return Ok(products);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            finally
            {
                await conn.CloseAsync();
            }
        }

        [HttpGet("shop2")]

        public async Task<IActionResult> ShopTwo()
        {
            string conString = _configuration.GetConnectionString("dconn");

            SqlConnection conn = new SqlConnection(conString);

            try
            {
                string sql = "SELECT * FROM products WHERE shopName = 'NetTech'";

                SqlCommand cmd = new SqlCommand(sql, conn);


                await conn.OpenAsync();

                SqlDataReader reader = cmd.ExecuteReader();

                List<Product> products = new List<Product>();

                while (reader.Read())
                {
                    products.Add(new Product
                    {
                        ProductId = Convert.ToInt32(reader["productid"]),
                        productName = reader["productname"].ToString(),
                        productCategory = reader["productcatagory"].ToString(),
                        quantity = Convert.ToInt32(reader["quantity"]),
                        productprice = reader["productprice"].ToString(),
                        shopName = reader["shopName"].ToString(),
                        productDescription = reader["productDes"].ToString()
                    });
                }

                return Ok(products);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            finally
            {
                await conn.CloseAsync();
            }
        }





    }

    


}
