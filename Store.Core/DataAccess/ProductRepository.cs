﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;

namespace Store.Core
{
    public class ProductRepository : IProductRepository
    {
        SqlConnection connection;
        SqlCommand command;
        SqlDataReader reader;
        SqlDataAdapter adapter;
        DataTable table;
        private string _conStrn = @"Data Source = .\SQLEXPRESS;Initial Catalog = dbgrocery_store_products; Integrated Security = True";
        public  List<Product> CreateProducts()
        {
            var DefaultProducts = new List<Product>();
            var sql = "select * from tbproducts;";

            using (connection = new SqlConnection(_conStrn))
            {
                command = new SqlCommand(sql, connection);
                connection.Open();
                reader =  command.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        DefaultProducts.Add(new Product(reader.GetString(0))
                        {
                            Name = reader.GetString(1),
                            Price = reader.GetInt32(2),
                            Quantity = reader.GetInt32(3)
                        }
                        );
                    }
                }

            }
            return DefaultProducts;
        }

        public bool AddToProducts(string id, string prodName, decimal prodPrice , int qty)
        {
            bool result;
            using (connection = new SqlConnection(_conStrn))
            {
                

                var sql = $"insert into tbproducts (id , product_name , price, quantity) values ('{id}' , '{prodName}', {prodPrice}, {qty});";
                command = new SqlCommand(sql, connection);
                connection.Open();

                var response= command.ExecuteNonQuery();
                result = response == 1 ? true : false;

            }

            return result;

        }
    }
}
