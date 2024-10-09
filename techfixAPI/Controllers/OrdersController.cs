using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Text.Json.Serialization;
using techfixAPI.Models;

namespace techfixAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        public readonly IConfiguration _configuration;
        public OrdersController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost]

        public async Task<IActionResult> Neworder([FromBody] Order order)
        {
            string connString = _configuration.GetConnectionString("dconn");

            SqlConnection conn = new SqlConnection(connString);

            try
            {
                string sql = @"INSERT INTO orders(orderName,itemid,quantity,totalprice,orderdate)
                            VALUES(@OrderName,@ItemID,@Quantify,@TotalPrice,@Orderdate)";

                SqlCommand cmd = new SqlCommand(sql, conn);

                cmd.Parameters.AddWithValue("@OrderName", order.ordername);
                cmd.Parameters.AddWithValue("@ItemID", order.itemId);
                cmd.Parameters.AddWithValue("@Quantify", order.quantity);
                cmd.Parameters.AddWithValue("@TotalPrice", order.total);
                cmd.Parameters.AddWithValue("@Orderdate", order.orderDate);

                await conn.OpenAsync();

                await cmd.ExecuteNonQueryAsync();

                return Ok(new { message = "Order Added Successfully" });


            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            finally { 
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
                string sql = "SELECT * FROM products WHERE productname LIKE @ProductName OR productcatagory LIKE @ProductCatagory OR productprice LIKE @ProductPrice OR shopName LIKE @ShopName";

                SqlCommand cmd = new SqlCommand(sql, conn);

                cmd.Parameters.AddWithValue("@ProductName", "%" + search + "%");
                cmd.Parameters.AddWithValue("@ProductCatagory", "%" + search + "%");
                cmd.Parameters.AddWithValue("@ProductPrice", "%" + search + "%");
                cmd.Parameters.AddWithValue("@ShopName", "%" + search + "%");

                await conn.OpenAsync();

                SqlDataReader reader = cmd.ExecuteReader();

                List<Product> products = new List<Product>();

                while (reader.Read()) {
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

        [HttpGet]
        public async Task<IActionResult> AllOrders()
        {
            string conString = _configuration.GetConnectionString("dconn");

            SqlConnection conn = new SqlConnection(conString);

            try
            {
                string sql = "SELECT * FROM orders";

                SqlCommand cmd = new SqlCommand(sql, conn);

                await conn.OpenAsync();

                SqlDataReader record = cmd.ExecuteReader();

                List<Order> orders = new List<Order>();

                while (record.Read()) {
                    orders.Add(new Order
                    {
                        orderid = Convert.ToInt32(record["orderid"]),
                        ordername = record["ordername"].ToString(),
                        itemId = Convert.ToInt32(record["itemid"]),
                        quantity = Convert.ToInt32(record["quantity"]),
                        total = Convert.ToDouble(record["totalprice"]),
                        orderDate = record["orderdate"].ToString()
                    });
                
                }

                return Ok(orders);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            finally
            {
                await conn.CloseAsync( );
            }

            return Ok();
        }


        [HttpPut("{id}")]

        public async Task<IActionResult> UpdateOrders(int id,[FromBody] Order order)
        {
            string connString = _configuration.GetConnectionString("dconn");

            SqlConnection conn = new SqlConnection(connString); 

            if(id != order.orderid)
            {
                return BadRequest("Id mismatch!"); 
            }

            try
            {
                string sql = @"UPDATE orders
                                SET ordername = @OrderName,
                                itemid = @ItemId,
                                quantity = @Quantity,
                                totalprice = @TotalPrice,
                                orderdate = @OrderDate
                                WHERE orderid = @OrderId";

                SqlCommand cmd = new SqlCommand(sql,conn);

                cmd.Parameters.AddWithValue("@OrderName", order.ordername);
                cmd.Parameters.AddWithValue("@ItemId", order.itemId);
                cmd.Parameters.AddWithValue("@Quantity", order.quantity);
                cmd.Parameters.AddWithValue("@TotalPrice", order.total);
                cmd.Parameters.AddWithValue("@OrderDate", order.orderDate);
                cmd.Parameters.AddWithValue("@OrderId", order.orderid);

                await conn.OpenAsync( );   

                var affectedRow =  await cmd.ExecuteNonQueryAsync();

                if (affectedRow == 1)
                {
                    return Ok(new { status = "success" });
                }
                else
                {
                    return BadRequest(new { message = "Update Faild" });
                }


            }
            catch (Exception ex) { 
                return BadRequest(ex.Message);
            }
            finally
            {
                await conn.CloseAsync( ); 
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> deleteOrder(int id)
        {
            string connString = _configuration.GetConnectionString("dconn");

            SqlConnection conn = new SqlConnection(connString);

            try
            {
                string sql = "DELETE FROM orders WHERE orderid = @OrderId";

                SqlCommand cmd = new SqlCommand(sql, conn);

                cmd.Parameters.AddWithValue("@OrderId", id);

                await conn.OpenAsync();

                int affectedRow = await cmd.ExecuteNonQueryAsync();

                if (affectedRow == 1)
                {
                    return Ok(new { message = "Order Deleted!" });
                }
                else
                {
                    return BadRequest(new { message = "Order Deleted unscuessfull!" });
                }
            }
            catch (Exception ex) {
                return BadRequest(ex.Message);
            }
            finally
            {
                await conn.CloseAsync( );
            }


        }


    }
}
