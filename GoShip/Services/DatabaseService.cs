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
                    CREATE TABLE IF NOT EXISTS Cart (
                        UserId INTEGER NOT NULL,
                        ProductId INTEGER NOT NULL,
                        Quantity INTEGER NOT NULL,
                        PRIMARY KEY (UserId, ProductId),
                        FOREIGN KEY(UserId) REFERENCES Users(Id),
                        FOREIGN KEY(ProductId) REFERENCES Products(Id)
                    );
                    CREATE TABLE IF NOT EXISTS Orders (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        UserId INTEGER,
                        Address TEXT NOT NULL,
                        OrderDate TEXT NOT NULL,
                        Total DECIMAL NOT NULL,
                        Status TEXT NOT NULL DEFAULT 'Активный',
                        FOREIGN KEY(UserId) REFERENCES Users(Id)
                    );
                    CREATE TABLE IF NOT EXISTS OrderItems (
                        OrderId INTEGER NOT NULL,
                        ProductId INTEGER NOT NULL,
                        ProductName TEXT NOT NULL,
                        ProductPrice DECIMAL NOT NULL,
                        Quantity INTEGER NOT NULL,
                        FOREIGN KEY(OrderId) REFERENCES Orders(Id),
                        FOREIGN KEY(ProductId) REFERENCES Products(Id)
                    );
                    CREATE TABLE IF NOT EXISTS UserDetails (
                        UserId INTEGER PRIMARY KEY,
                        Name TEXT,
                        Email TEXT,
                        Address TEXT,
                        FOREIGN KEY(UserId) REFERENCES Users(Id)
                    );
                ";
                var command = new SQLiteCommand(sql, connection);
                command.ExecuteNonQuery();

                // Проверка и добавление столбца Status, если его нет
                string checkColumnSql = "PRAGMA table_info(Orders);";
                command = new SQLiteCommand(checkColumnSql, connection);
                bool hasStatusColumn = false;
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string columnName = reader.GetString(1);
                        if (columnName == "Status")
                        {
                            hasStatusColumn = true;
                            break;
                        }
                    }
                }

                if (!hasStatusColumn)
                {
                    string addColumnSql = "ALTER TABLE Orders ADD COLUMN Status TEXT NOT NULL DEFAULT 'Активный';";
                    command = new SQLiteCommand(addColumnSql, connection);
                    command.ExecuteNonQuery();
                }

               
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

        public List<CartItem> GetCartItems(int userId)
        {
            var cartItems = new List<CartItem>();
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string sql = @"
                    SELECT c.UserId, c.ProductId, c.Quantity, 
                           p.Id, p.Name, p.Price, p.ImageUrl, p.Calories, p.Proteins, p.Fats, p.Carbohydrates
                    FROM Cart c
                    JOIN Products p ON c.ProductId = p.Id
                    WHERE c.UserId = @userId";
                var command = new SQLiteCommand(sql, connection);
                command.Parameters.AddWithValue("@userId", userId);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        cartItems.Add(new CartItem
                        {
                            UserId = reader.GetInt32(0),
                            ProductId = reader.GetInt32(1),
                            Quantity = reader.GetInt32(2),
                            Product = new Product
                            {
                                Id = reader.GetInt32(3),
                                Name = reader.GetString(4),
                                Price = reader.GetDecimal(5),
                                ImageUrl = reader.GetString(6),
                                Calories = reader.GetInt32(7),
                                Proteins = reader.GetDecimal(8),
                                Fats = reader.GetDecimal(9),
                                Carbohydrates = reader.GetDecimal(10)
                            }
                        });
                    }
                }
            }
            return cartItems;
        }

        public void AddToCart(int userId, int productId, int quantity)
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string sql = @"
                    INSERT OR REPLACE INTO Cart (UserId, ProductId, Quantity)
                    VALUES (@userId, @productId, 
                            COALESCE((SELECT Quantity FROM Cart WHERE UserId = @userId AND ProductId = @productId), 0) + @quantity)";
                var command = new SQLiteCommand(sql, connection);
                command.Parameters.AddWithValue("@userId", userId);
                command.Parameters.AddWithValue("@productId", productId);
                command.Parameters.AddWithValue("@quantity", quantity);
                command.ExecuteNonQuery();
            }
        }

        public void RemoveFromCart(int userId, int productId)
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string sql = "DELETE FROM Cart WHERE UserId = @userId AND ProductId = @productId";
                var command = new SQLiteCommand(sql, connection);
                command.Parameters.AddWithValue("@userId", userId);
                command.Parameters.AddWithValue("@productId", productId);
                command.ExecuteNonQuery();
            }
        }

        public void ClearCart(int userId)
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string sql = "DELETE FROM Cart WHERE UserId = @userId";
                var command = new SQLiteCommand(sql, connection);
                command.Parameters.AddWithValue("@userId", userId);
                command.ExecuteNonQuery();
            }
        }

        public List<Order> GetOrders(int userId)
        {
            var orders = new List<Order>();
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string sql = @"
                    SELECT o.Id, o.UserId, o.Address, o.OrderDate, o.Total, o.Status
                    FROM Orders o
                    WHERE o.UserId = @userId";
                var command = new SQLiteCommand(sql, connection);
                command.Parameters.AddWithValue("@userId", userId);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var order = new Order
                        {
                            Id = reader.GetInt32(0),
                            UserId = reader.GetInt32(1),
                            Address = reader.GetString(2),
                            OrderDate = reader.GetString(3),
                            Total = reader.GetDecimal(4),
                            Status = reader.GetString(5),
                            Items = new List<OrderItem>()
                        };
                        orders.Add(order);
                    }
                }

                foreach (var order in orders)
                {
                    string itemsSql = @"
                        SELECT oi.OrderId, oi.ProductId, oi.ProductName, oi.ProductPrice, oi.Quantity,
                               p.Id, p.Name, p.Price, p.ImageUrl, p.Calories, p.Proteins, p.Fats, p.Carbohydrates
                        FROM OrderItems oi
                        JOIN Products p ON oi.ProductId = p.Id
                        WHERE oi.OrderId = @orderId";
                    command = new SQLiteCommand(itemsSql, connection);
                    command.Parameters.AddWithValue("@orderId", order.Id);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            order.Items.Add(new OrderItem
                            {
                                OrderId = reader.GetInt32(0),
                                ProductId = reader.GetInt32(1),
                                ProductName = reader.GetString(2),
                                ProductPrice = reader.GetDecimal(3),
                                Quantity = reader.GetInt32(4),
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
            }
            return orders;
        }

        public List<Order> GetAllOrders()
        {
            var orders = new List<Order>();
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string sql = @"
                    SELECT o.Id, o.UserId, o.Address, o.OrderDate, o.Total, o.Status
                    FROM Orders o";
                var command = new SQLiteCommand(sql, connection);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var order = new Order
                        {
                            Id = reader.GetInt32(0),
                            UserId = reader.GetInt32(1),
                            Address = reader.GetString(2),
                            OrderDate = reader.GetString(3),
                            Total = reader.GetDecimal(4),
                            Status = reader.GetString(5),
                            Items = new List<OrderItem>()
                        };
                        orders.Add(order);
                    }
                }

                foreach (var order in orders)
                {
                    string itemsSql = @"
                        SELECT oi.OrderId, oi.ProductId, oi.ProductName, oi.ProductPrice, oi.Quantity,
                               p.Id, p.Name, p.Price, p.ImageUrl, p.Calories, p.Proteins, p.Fats, p.Carbohydrates
                        FROM OrderItems oi
                        JOIN Products p ON oi.ProductId = p.Id
                        WHERE oi.OrderId = @orderId";
                    command = new SQLiteCommand(itemsSql, connection);
                    command.Parameters.AddWithValue("@orderId", order.Id);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            order.Items.Add(new OrderItem
                            {
                                OrderId = reader.GetInt32(0),
                                ProductId = reader.GetInt32(1),
                                ProductName = reader.GetString(2),
                                ProductPrice = reader.GetDecimal(3),
                                Quantity = reader.GetInt32(4),
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
            }
            return orders;
        }

        public void UpdateOrderStatus(int orderId, string status)
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string sql = "UPDATE Orders SET Status = @status WHERE Id = @orderId";
                var command = new SQLiteCommand(sql, connection);
                command.Parameters.AddWithValue("@status", status);
                command.Parameters.AddWithValue("@orderId", orderId);
                command.ExecuteNonQuery();
            }
        }

        public int CreateOrder(int userId, string address, decimal total, string orderDate)
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string sql = @"
                    INSERT INTO Orders (UserId, Address, OrderDate, Total, Status) 
                    VALUES (@userId, @address, @orderDate, @total, 'Активный');
                    SELECT last_insert_rowid();";
                var command = new SQLiteCommand(sql, connection);
                command.Parameters.AddWithValue("@userId", userId);
                command.Parameters.AddWithValue("@address", address);
                command.Parameters.AddWithValue("@orderDate", orderDate);
                command.Parameters.AddWithValue("@total", total);
                return Convert.ToInt32(command.ExecuteScalar());
            }
        }

        public void AddOrderItem(int orderId, int productId, string productName, decimal productPrice, int quantity)
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string sql = @"
                    INSERT INTO OrderItems (OrderId, ProductId, ProductName, ProductPrice, Quantity) 
                    VALUES (@orderId, @productId, @productName, @productPrice, @quantity)";
                var command = new SQLiteCommand(sql, connection);
                command.Parameters.AddWithValue("@orderId", orderId);
                command.Parameters.AddWithValue("@productId", productId);
                command.Parameters.AddWithValue("@productName", productName);
                command.Parameters.AddWithValue("@productPrice", productPrice);
                command.Parameters.AddWithValue("@quantity", quantity);
                command.ExecuteNonQuery();
            }
        }

        public void SaveUserDetails(int userId, string name, string email, string address)
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string sql = @"
                    INSERT OR REPLACE INTO UserDetails (UserId, Name, Email, Address) 
                    VALUES (@userId, @name, @email, @address)";
                var command = new SQLiteCommand(sql, connection);
                command.Parameters.AddWithValue("@userId", userId);
                command.Parameters.AddWithValue("@name", name);
                command.Parameters.AddWithValue("@email", email);
                command.Parameters.AddWithValue("@address", address);
                command.ExecuteNonQuery();
            }
        }

        public (string name, string email, string address) GetUserDetails(int userId)
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string sql = "SELECT Name, Email, Address FROM UserDetails WHERE UserId = @userId";
                var command = new SQLiteCommand(sql, connection);
                command.Parameters.AddWithValue("@userId", userId);
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return (
                            reader.IsDBNull(0) ? "" : reader.GetString(0),
                            reader.IsDBNull(1) ? "" : reader.GetString(1),
                            reader.IsDBNull(2) ? "" : reader.GetString(2)
                        );
                    }
                }
            }
            return ("", "", "");
        }
    }
}