using System;
using System.Collections.Generic;
using System.Data.SQLite;
using GoShip.Models;

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
                        Fats DECIMAL,
                        Carbohydrates DECIMAL
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

                string insertProducts = @"
                    INSERT OR IGNORE INTO Products (Name, Price, ImageUrl, Calories, Proteins, Fats, Carbohydrates) 
                    VALUES ('Драники', 159, 'https://images.unsplash.com/photo-1598188306268-1a619eb1b560', 800, 20, 30, 40),
                           ('Суп поре', 119, 'https://images.unsplash.com/photo-1604152135912-429d685af212', 600, 25, 35, 45),
                           ('Овощный микс', 210, 'https://images.unsplash.com/photo-1512621776951-a57141f2eefd', 400, 15, 20, 30),
                           ('Сырный супчик', 129, 'https://images.unsplash.com/photo-1604329758250-315b426683ec', 103, 5.7, 5.8, 6),
                           ('Курица с рисом', 179, 'https://images.unsplash.com/photo-1627662054978-18c76e8a408f', 400, 15, 20, 30),
                           ('Рис с мясом', 149, 'https://images.unsplash.com/photo-1603133872878-684f208fb84b', 103, 5.7, 5.8, 6)";
                command = new SQLiteCommand(insertProducts, connection);
                command.ExecuteNonQuery();

                string insertUser = @"
                    INSERT OR IGNORE INTO Users (Login, Password, Role) 
                    VALUES ('client', 'password', 'Client')";
                command = new SQLiteCommand(insertUser, connection);
                command.ExecuteNonQuery();
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
                        return (reader.GetInt32(0), reader.GetString(1));
                    }
                }
            }
            return null;
        }

        public void RegisterUser(string login, string password, string role)
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string sql = @"
                    INSERT INTO Users (Login, Password, Role) 
                    VALUES (@login, @password, @role)";
                var command = new SQLiteCommand(sql, connection);
                command.Parameters.AddWithValue("@login", login);
                command.Parameters.AddWithValue("@password", password);
                command.Parameters.AddWithValue("@role", role);
                try
                {
                    command.ExecuteNonQuery();
                }
                catch (SQLiteException ex) when (ex.Message.Contains("UNIQUE constraint failed"))
                {
                    throw new Exception("Пользователь с таким логином уже существует!");
                }
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
                            Fats = reader.GetDecimal(6),
                            Carbohydrates = reader.GetDecimal(7)
                        });
                    }
                }
            }
            return products;
        }

        public List<Order> GetOrders(int userId)
        {
            var orders = new List<Order>();
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string sql = @"
                    SELECT o.Id, o.UserId, o.ProductId, o.Address, o.OrderDate, 
                           p.Id, p.Name, p.Price, p.ImageUrl, p.Calories, p.Proteins, p.Fats, p.Carbohydrates
                    FROM Orders o
                    JOIN Products p ON o.ProductId = p.Id
                    WHERE o.UserId = @userId";
                var command = new SQLiteCommand(sql, connection);
                command.Parameters.AddWithValue("@userId", userId);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        orders.Add(new Order
                        {
                            Id = reader.GetInt32(0),
                            UserId = reader.GetInt32(1),
                            ProductId = reader.GetInt32(2),
                            Address = reader.GetString(3),
                            OrderDate = reader.GetString(4),
                            Product = new Product
                            {
                                Id = reader.GetInt32(5),
                                Name = reader.GetString(6),
                                Price = reader.GetDecimal(7),
                                ImageUrl = reader.GetString(8),
                                Calories = reader.GetInt32(9),
                                Proteins = reader.GetDecimal(10),
                                Fats = reader.GetDecimal(11),
                                Carbohydrates = reader.GetDecimal(12)
                            }
                        });
                    }
                }
            }
            return orders;
        }

        public void PlaceOrder(int userId, int productId, string address)
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string sql = @"
                    INSERT INTO Orders (UserId, ProductId, Address, OrderDate) 
                    VALUES (@userId, @productId, @address, @orderDate)";
                var command = new SQLiteCommand(sql, connection);
                command.Parameters.AddWithValue("@userId", userId);
                command.Parameters.AddWithValue("@productId", productId);
                command.Parameters.AddWithValue("@address", address);
                command.Parameters.AddWithValue("@orderDate", DateTime.Now.ToString("yyyy-MM-dd"));
                command.ExecuteNonQuery();
            }
        }

        public void RemoveOrder(int orderId)
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string sql = "DELETE FROM Orders WHERE Id = @orderId";
                var command = new SQLiteCommand(sql, connection);
                command.Parameters.AddWithValue("@orderId", orderId);
                command.ExecuteNonQuery();
            }
        }

        public void UpdateOrder(int orderId, string address, string orderDate)
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string sql = @"
                    UPDATE Orders 
                    SET Address = @address, OrderDate = @orderDate 
                    WHERE Id = @orderId";
                var command = new SQLiteCommand(sql, connection);
                command.Parameters.AddWithValue("@orderId", orderId);
                command.Parameters.AddWithValue("@address", address);
                command.Parameters.AddWithValue("@orderDate", orderDate);
                command.ExecuteNonQuery();
            }
        }
    }
}