using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Service.Control.DataBaseConnection.Models;
using System.Threading.Tasks;

namespace System.Service.Control.DataBaseConnection.Data
{
    public class DatabaseConnectRepository
    {
        private IConfiguration _configuration;

        public DatabaseConnectRepository(IConfiguration configuration) {
            _configuration = configuration;
        }
        public async Task<List<DataBaseConnectionModel>> GetProductsListing()
        {
          
            string connectionString = _configuration.GetSection("connectionStrings:defaultConnection").Value;  // get connection string from appconfig.
            DataBaseConnectionModel dbModel = new DataBaseConnectionModel();
            var dbModelList = new List<DataBaseConnectionModel>();
            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(connectionString))
                {
                    using (SqlCommand sqlCommand = new SqlCommand("GetProducts", sqlConnection))
                    {
                        await sqlConnection.OpenAsync();

                        using (var reader = await sqlCommand.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                dbModelList.Add(MapToDBModel(reader));
                            }
                        }
                        return dbModelList;         
                    }
                }
              
            }
            catch (Exception ex)
            {
                throw ex;
                
            }
           
        }



        private DataBaseConnectionModel MapToDBModel(SqlDataReader sqlDataReader) {

            return new DataBaseConnectionModel()
            {

               ReturnCode = sqlDataReader["ReturnCode"].ToString(),  
            active = sqlDataReader["active"].ToString(),  
            availability = sqlDataReader["availability"].ToString(),  
            bottler_id = sqlDataReader["bottler_id"].ToString(), 
            color = sqlDataReader["color"].ToString(),  
            configuration = sqlDataReader["configuration"].ToString(), 
            contract = sqlDataReader["contract"].ToString(),  
            created_at = sqlDataReader["created_at"].ToString(),  
            customer_id = sqlDataReader["customer_id"].ToString(),  
            description = sqlDataReader["description"].ToString(),  
            epid = sqlDataReader["epid"].ToString(),  
            id = sqlDataReader["id"].ToString(),  
            name = sqlDataReader["name"].ToString(),  
            picture = sqlDataReader["picture"].ToString(),  
            product_id = sqlDataReader["product_id"].ToString(),  
            promotion = sqlDataReader["promotion"].ToString(),  
            retail_price = sqlDataReader["retail_price"].ToString(),  
            sale_price = sqlDataReader["sale_price"].ToString(),  
            size = sqlDataReader["size"].ToString(),  
            skuid = sqlDataReader["skuid"].ToString(), 
            small_image = sqlDataReader["small_image"].ToString(),
            image = "https://bottlerstorage.blob.core.windows.net/uploadedimages/" + sqlDataReader["small_image"].ToString(), 
            store = sqlDataReader["store"].ToString(), 
            store_id = sqlDataReader["store_id"].ToString(),  
            _id = sqlDataReader["_id"].ToString(), 
        };

        }

      
    }
}
