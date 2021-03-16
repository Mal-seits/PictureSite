using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace PictureSite.data
{
    public class DbManager
    {
      
        private string _constr;
        public DbManager(string constr)
        {
            _constr = constr;
        }
        public int AddImage(string name, string password)
        {
            using var connection = new SqlConnection(_constr);
            using var command = connection.CreateCommand();
            command.CommandText = @"INSERT INTO Images(Name, Views, Password)
                                    VALUES(@name, @views, @password) SELECT SCOPE_IDENTITY()";
            command.Parameters.AddWithValue("@name", name);
            command.Parameters.AddWithValue("@views", 0);
            command.Parameters.AddWithValue("@password", password);
            connection.Open();
            return (int)(decimal)command.ExecuteScalar();

        }
        public Image GetImage(int Id)
        {
            using var connection = new SqlConnection(_constr);
            using var command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM Images WHERE id = @id";
            command.Parameters.AddWithValue("@id", Id);
            connection.Open();
            var reader = command.ExecuteReader();
            Image image = new Image();
            while (reader.Read())
            {
                image = new Image
                {
                    ID = (int)reader["Id"],
                    Name = (string)reader["name"],
                    Password = (string)reader["password"],
                    Views = (int)reader["views"]
                };
            }

            return image;
        }
        public bool CorrectPassword(int imageId, string password)
        {

            using var connection = new SqlConnection(_constr);
            using var command = connection.CreateCommand();
            command.CommandText = @"SELECT  * from Images WHERE id = @id AND Password = @password";
            command.Parameters.AddWithValue("@id", imageId);
            command.Parameters.AddWithValue("@password", password);
            connection.Open();
            var reader = command.ExecuteReader();
            if (!reader.Read())
            {
                return false;
            }
            else
            {
                return true;
            }
        }
     public void UpdateViews(int imageID)
        {
            using var connection = new SqlConnection(_constr);
            using var command = connection.CreateCommand();
            command.CommandText = @"UPDATE Images SET Views = Views + 1 WHERE Id = @id";
            command.Parameters.AddWithValue("@id", imageID);
            connection.Open();
            command.ExecuteNonQuery();
        }
    }
}
