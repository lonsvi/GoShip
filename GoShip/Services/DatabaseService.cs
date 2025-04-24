using System.Data.SQLite;
using System.Collections.Generic;
using GoShip.Models;
using System;

namespace GoShip.Services
{
    public class DatabaseService
    {
        private readonly string connectionString = "Data Source=goship.db;Version=3;";

        public DatabaseService()
        {
            InitializeDatabase();
        }

        private void InitializeDatabase()
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string sql = @"
                    CREATE TABLE IF NOT EXISTS Users (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Login TEXT NOT NULL UNIQUE,
                        Password TEXT NOT NULL,
                        Role TEXT NOT NULL
                    );
                    CREATE TABLE IF NOT EXISTS Products (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Name TEXT NOT NULL,
                        Price DECIMAL NOT NULL,
                        ImageUrl TEXT,
                        Calories INT,
                        Proteins DECIMAL,
                        Fats DECIMAL
                    );
                    CREATE TABLE IF NOT EXISTS Orders (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        UserId INTEGER,
                        ProductId INTEGER,
                        Address TEXT NOT NULL,
                        OrderDate TEXT NOT NULL,
                        FOREIGN KEY(UserId) REFERENCES Users(Id),
                        FOREIGN KEY(ProductId) REFERENCES Products(Id)
                    );
                ";
                var command = new SQLiteCommand(sql, connection);
                command.ExecuteNonQuery();

                // Добавление тестовых продуктов
                string insertProducts = @"
                    INSERT OR IGNORE INTO Products (Name, Price, ImageUrl, Calories, Proteins, Fats) 
                    VALUES ('Пицца Маргарита', 500, 'pizza.jpg', 800, 20, 30),
                           ('Бургер', 300, 'burger.jpg', 600, 25, 35),
                           ('Салат Цезарь', 400, 'salad.jpg', 400, 15, 20)";
                command = new SQLiteCommand(insertProducts, connection);
                command.ExecuteNonQuery();
            }
        }

        public bool RegisterUser(string login, string password, string role)
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string checkSql = "SELECT COUNT(*) FROM Users WHERE Login = @login";
                var checkCommand = new SQLiteCommand(checkSql, connection);
                checkCommand.Parameters.AddWithValue("@login", login);
                long count = (long)checkCommand.ExecuteScalar();

                if (count > 0) return false; // Логин уже занят

                string sql = "INSERT INTO Users (Login, Password, Role) VALUES (@login, @password, @role)";
                var command = new SQLiteCommand(sql, connection);
                command.Parameters.AddWithValue("@login", login);
                command.Parameters.AddWithValue("@password", password); // В реальном проекте хешируйте пароль
                command.Parameters.AddWithValue("@role", role);
                command.ExecuteNonQuery();
                return true;
            }
        }

        public (int userId, string role)? Authenticate(string login, string password)
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string sql = "SELECT Id, Role FROM Users WHERE Login = @login AND Password = @password";
                var command = new SQLiteCommand(sql, connection);
                command.Parameters.AddWithValue("@login", login);
                command.Parameters.AddWithValue("@password", password);
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        int userId = reader.GetInt32(0);
                        string role = reader.GetString(1);
                        return (userId, role);
                    }
                }
                return null;
            }
        }

        public List<Product> GetProducts()
        {
            var products = new List<Product>();
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string sql = "SELECT * FROM Products";
                var command = new SQLiteCommand(sql, connection);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        products.Add(new Product
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            Price = reader.GetDecimal(2),
                            ImageUrl = reader.GetString(3),
                            Calories = reader.GetInt32(4),
                            Proteins = reader.GetDecimal(5),
                            Fats = reader.GetDecimal(6)
                        });
                    }
                }
            }
            return products;
        }

        public void PlaceOrder(int userId, int productId, string address)
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string sql = "INSERT INTO Orders (UserId, ProductId, Address, OrderDate) VALUES (@userId, @productId, @address, @orderDate)";
                var command = new SQLiteCommand(sql, connection);
                command.Parameters.AddWithValue("@userId", userId);
                command.Parameters.AddWithValue("@productId", productId);
                command.Parameters.AddWithValue("@address", address);
                command.Parameters.AddWithValue("@orderDate", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                command.ExecuteNonQuery();
            }
        }
    }
}