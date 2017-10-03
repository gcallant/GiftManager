// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DatabaseManager.cs" company="grantcallant.com">
// Author: Grant Callant  
// </copyright>
// <summary>
//   Defines the DatabaseManager type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace GiftManager.Code
{
   using System;
   using System.Collections.Generic;
   using System.Data.SQLite;
   using System.IO;
   using System.Linq;
   using System.Linq.Expressions;
   using System.Reflection;
   using System.Text;

   /// <summary>
   /// A loose "Singletonesque" class that does initial database setup and holds connection info for other 
   /// datastore classes
   /// </summary>
   internal sealed class DatabaseManager
   {
      /// <summary>
      /// Gets the db name.
      /// </summary>
      private string DBName { get; }

      /// <summary>
      /// Initializes a new instance of the <see cref="DatabaseManager"/> class.
      /// </summary>
      /// <param name="dbName">
      /// The db name.
      /// </param>
      internal DatabaseManager(string dbName = "GiftManager.sqlite")
      {
         this.DBName = !string.IsNullOrEmpty(dbName) ? dbName : "GiftManager.sqlite";
         this.CreateDatabase();
         this.CreateTables();
      }

      /// <summary>
      /// Initializes a zero-store file ready for the database.
      /// </summary>
      private void CreateDatabase()
      {
         if(!File.Exists(this.DBName))
         {
            SQLiteConnection.CreateFile(this.DBName);
         }
      }

      /// <summary>
      /// The method that holds the connection details for each subsequent connection.
      /// </summary>
      /// <returns>
      /// The <see cref="SQLiteConnection"/>.
      /// </returns>
      public SQLiteConnection ConnectToDatabase() => new SQLiteConnection("DataSource=" + this.DBName + ";Version=3;");

      /// <summary>
      /// Creates the tables needed for the GiftManager
      /// </summary>
      /// <returns>
      /// The <see cref="int"/>.
      /// </returns>
      private int CreateTables()
      {
         string statement = "CREATE TABLE IF NOT EXISTS Member (" + "MemberId INTEGER NOT NULL PRIMARY KEY UNIQUE, "
                            + "MemberName TEXT NOT NULL, " + "MemberUserName TEXT NOT NULL UNIQUE, "
                            + "PasswordHash TEXT NOT NULL, " + "Salt TEXT NOT NULL, "
                            + "IsAdminUser BOOLEAN NOT NULL); " + "CREATE TABLE IF NOT EXISTS Gift ("
                            + "GiftId INTEGER NOT NULL PRIMARY KEY UNIQUE, " + "GiftName TEXT NOT NULL, "
                            + "GiftPrice INTEGER NOT NULL, " + "GiftDescription TEXT); "
                            + "CREATE TABLE IF NOT EXISTS Assignments ("
                            + "MemberId INTEGER NOT NULL PRIMARY KEY UNIQUE, " + "AssignedMemberId INTEGER NOT NULL, "
                            + "AssignedGiftId INTEGER, " + "FOREIGN KEY(MemberId) REFERENCES Member(MemberId), "
                            + "FOREIGN KEY(AssignedMemberId) REFERENCES Member(MemberId), "
                            + "FOREIGN KEY(AssignedGiftId) REFERENCES Gift(GiftId));";

         using(SQLiteConnection connection = this.ConnectToDatabase())
         {
            connection.Open();
            using(var sqliteCommand = new SQLiteCommand(statement, connection))
            {
               return sqliteCommand.ExecuteNonQuery();
            }
         }
      }
   }

   /// <summary>
   ///    An attribute directive for modeling an OOP system in database
   /// </summary>
   public class DBAttribute : Attribute
   {
      /// <summary>
      ///    For identifying if a member property is the primary key in a table
      /// </summary>
      public bool PrimaryKey { get; set; }
   }

   /// <summary>
   ///    Small, modified, simplified ORM for working with SQLite
   ///    Original Author: Swaraj Ketan Santra
   ///    Email: swaraj.ece.jgec@gmail.com
   ///    Subsequent Author: Grant Callant
   ///    Email: grant@grantcallant.com
   ///    License: MIT
   ///    The MIT License (MIT)
   ///    Copyright (c)
   ///    Permission is hereby granted, free of charge, to any person obtaining a copy of this software and
   ///    associated documentation files (the "Software"), to deal in the Software without restriction, including
   ///    without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
   ///    copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following
   ///    conditions:
   ///    The above copyright notice and this permission notice shall be included in all copies or substantial portions of the
   ///    Software.
   ///    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
   ///    WARRANTIES
   ///    OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
   ///    HOLDERS BE
   ///    LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
   ///    OUT OF OR
   ///    IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
   ///    URL: https://code.msdn.microsoft.com/windowsapps/how-to-use-sqlite-with-96f22fac
   /// </summary>
   /// <typeparam name="T"> Class and type</typeparam>
   public class GenericDataOperations<T>
   where T : class, new()
   {
      /// <summary>
      /// The database manager.
      /// </summary>
      private readonly DatabaseManager databaseManager;

      /// <summary>
      /// Initializes a new instance of the <see cref="GenericDataOperations{T}"/> class.
      /// </summary>
      /// <param name="dbName">
      /// The db name.
      /// </param>
      public GenericDataOperations(string dbName = "")
      {
         this.databaseManager = new DatabaseManager(dbName);
      }

      /// <summary>
      /// Inserts a generic object into the database
      /// </summary>
      /// <param name="record">
      /// The record to be inserted.
      /// </param>
      /// <returns>
      /// The <see cref="int"/>.
      /// </returns>
      public int AddRecord(T record)
      {
         var column = new StringBuilder();
         var data = new StringBuilder();
         var statement = new StringBuilder();

         IList<PropertyInfo> recordInfo = this.GetPropertyInfoList(record);

         foreach(PropertyInfo info in recordInfo)
         {
            ApplyInsertAttributes(record, info, column, data);
         }

         FormatStatement(record, column, data, statement, InsertType);
         return this.ExecuteSQLStatement(statement.ToString());
      }

      /// <summary>
      /// Allows a named table to be dropped
      /// </summary>
      /// <param name="tableName">
      /// The table name.
      /// </param>
      protected void DropTable(string tableName)
      {
         string statement = $"DROP TABLE IF EXISTS {tableName}";
         ExecuteSQLStatement(statement);
      }

      /// <summary>
      /// Formats a statement depending on an incoming function pointer
      /// </summary>
      /// <param name="record">
      /// The record.
      /// </param>
      /// <param name="column">
      /// The column.
      /// </param>
      /// <param name="data">
      /// The data.
      /// </param>
      /// <param name="statement">
      /// The statement.
      /// </param>
      /// <param name="queryType">
      /// The type of query this method should format for
      /// </param>
      private static void FormatStatement(
         T record,
         StringBuilder column,
         StringBuilder data,
         StringBuilder statement,
         Func<T, StringBuilder, StringBuilder, string> queryType)
      {
         if(!string.IsNullOrEmpty(column.ToString()))
         {
            column.Remove(column.Length - 1, 1);
            data.Remove(data.Length - 1, 1);
            statement.Append(queryType(record, column, data));
         }
      }

      /// <summary>
      /// Parametrized statement for inserts
      /// </summary>
      /// <param name="record">
      /// The record.
      /// </param>
      /// <param name="column">
      /// The column.
      /// </param>
      /// <param name="data">
      /// The data.
      /// </param>
      /// <returns>
      /// The <see cref="string"/>.
      /// </returns>
      private static string InsertType(T record, StringBuilder column, StringBuilder data) =>
         $"INSERT INTO [{record.GetType().Name}] ( {column} ) VALUES ( {data} );";

      /// <summary>
      /// Parametrized method for interesting the data from object member attributes
      /// </summary>
      /// <param name="record">
      /// The record.
      /// </param>
      /// <param name="info">
      /// The info.
      /// </param>
      /// <param name="column">
      /// The column.
      /// </param>
      /// <param name="data">
      /// The data.
      /// </param>
      private static void ApplyInsertAttributes(T record, PropertyInfo info, StringBuilder column, StringBuilder data)
      {
         var attribute = (DBAttribute)info.GetCustomAttribute(typeof(DBAttribute));

         if(attribute != null)
         {
            column.Append($"[{info.Name}],");
            data.Append($"{(info.GetValue(record) != null ? $"'{info.GetValue(record)}'" : "NULL")},");
         }
      }

      /// <summary>
      /// Adds multiple records with one query
      /// </summary>
      /// <param name="records">
      /// The records.
      /// </param>
      /// <returns>
      /// The <see cref="int"/>.
      /// </returns>
      public int AddMultipleRecords(IList<T> records)
      {
         var statement = new StringBuilder();
         foreach(T record in records)
         {
            var column = new StringBuilder();
            var data = new StringBuilder();

            IList<PropertyInfo> recordInfo = this.GetPropertyInfoList(record);
            foreach(PropertyInfo info in recordInfo)
            {
               ApplyInsertAttributes(record, info, column, data);
            }
            FormatStatement(record, column, data, statement, InsertType);
         }
         return this.ExecuteSQLStatement(statement.ToString());
      }

      /// <summary>
      /// Updates an existing record
      /// </summary>
      /// <param name="record">
      /// The record.
      /// </param>
      /// <returns>
      /// The <see cref="int"/>.
      /// </returns>
      public int UpdateRecord(T record)
      {
         var column = new StringBuilder();
         var data = new StringBuilder();
         int updated = 0;
         IList<PropertyInfo> recordInfo = this.GetPropertyInfoList(record);

         foreach(PropertyInfo info in recordInfo)
         {
            ApplyUpdateAttributes(record, info, data, column);
         }
         updated = this.Updated(record, column, data, updated, UpdateType);
         return updated;
      }

      /// <summary>
      /// Formats a statement and applies member attributes depending on an incoming function pointer
      /// </summary>
      /// <param name="record"></param>
      /// <param name="column"></param>
      /// <param name="data"></param>
      /// <param name="updated"></param>
      /// <param name="updateType"></param>
      /// <returns></returns>
      private int Updated(
         T record,
         StringBuilder column,
         StringBuilder data,
         int updated,
         Func<T, StringBuilder, StringBuilder, string> updateType)
      {
         if(!string.IsNullOrEmpty(column.ToString()))
         {
            column.Remove(column.Length - 1, 1);
            var statement = new StringBuilder();
            statement.Append(updateType(record, column, data));
            if(updateType.Method.Name.Equals("DeleteType"))
            {
               statement.Replace(",", " AND ");
            }
            updated = this.ExecuteSQLStatement(statement.ToString());
         }
         return updated;
      }

      /// <summary>
      /// Parametrized statement for updates
      /// </summary>
      /// <param name="record">
      /// The record.
      /// </param>
      /// <param name="column">
      /// The column.
      /// </param>
      /// <param name="data">
      /// The data.
      /// </param>
      /// <returns>
      /// The <see cref="string"/>.
      /// </returns>
      private static string UpdateType(T record, StringBuilder column, StringBuilder data) =>
         $"UPDATE [{record.GetType().Name}] SET {column} WHERE {data};";

      /// <summary>
      /// Deletes a record
      /// </summary>
      /// <param name="record">
      /// The record.
      /// </param>
      /// <returns>
      /// The <see cref="int"/>.
      /// </returns>
      public int DeleteRecord(T record)
      {
         var column = new StringBuilder();
         var data = new StringBuilder();
         int removedCount = 0;
         IList<PropertyInfo> recordInfo = this.GetPropertyInfoList(record);

         foreach(PropertyInfo info in recordInfo)
         {
            ApplyUpdateAttributes(record, info, data, column);
         }
         removedCount = this.Updated(record, column, data, removedCount, DeleteType);
         return removedCount;
      }

      /// <summary>
      /// Parametrized statement for delete queries
      /// </summary>
      /// <param name="record">
      /// The record.
      /// </param>
      /// <param name="column">
      /// The column.
      /// </param>
      /// <param name="data">
      /// The data.
      /// </param>
      /// <returns>
      /// The <see cref="string"/>.
      /// </returns>
      private static string DeleteType(T record, StringBuilder column, StringBuilder data) =>
         $"DELETE FROM [{record.GetType().Name}] WHERE {column};";

      /// <summary>
      /// Applies member attributes for update queries
      /// </summary>
      /// <param name="record">
      /// The record.
      /// </param>
      /// <param name="info">
      /// The info.
      /// </param>
      /// <param name="data">
      /// The data.
      /// </param>
      /// <param name="column">
      /// The column.
      /// </param>
      private static void ApplyUpdateAttributes(T record, PropertyInfo info, StringBuilder data, StringBuilder column)
      {
         var attribute = (DBAttribute)info.GetCustomAttribute(typeof(DBAttribute));

         if(attribute != null)
         {
            if(attribute.PrimaryKey)
            {
               data.Append($"[{info.Name}] = '{info.GetValue(record)}'");
            }
            else
            {
               column.Append(
                  $"[{info.Name}] = {(info.GetValue(record) != null ? $"'{info.GetValue(record)}'" : "NULL")},");
            }
         }
      }

      /// <summary>
      /// Retrieves a record using an applied filter
      /// </summary>
      /// <param name="filter">
      /// The filter.
      /// </param>
      /// <returns>
      /// The <see cref="IList"/>.
      /// </returns>
      public IList<T> Find(Filter<T> filter)
      {
         var record = new T();
         return this.ExecuteSelectStatement(filter);
      }

      /// <summary>
      /// Retrieves a record by primary key
      /// </summary>
      /// <param name="id">
      /// The id.
      /// </param>
      /// <returns>
      /// The <see cref="T"/>.
      /// </returns>
      public T GetById(dynamic id)
      {
         var record = new T();
         var data = new StringBuilder();

         IList<PropertyInfo> recordInfo = this.GetPropertyInfoList(record);

         foreach(PropertyInfo info in recordInfo)
         {
            var attribute = (DBAttribute)info.GetCustomAttribute(typeof(DBAttribute));

            if(attribute != null && attribute.PrimaryKey)
            {
               data.Append($"[{info.Name}]='{id}'");
               break;
            }
         }
         if(!string.IsNullOrEmpty(data.ToString()))
         {
            var statement = new StringBuilder();
            statement.Append($"SELECT * FROM[{record.GetType().Name}] WHERE {data}");
            IList<T> recordsList = this.ExecuteSelectStatement(statement.ToString());
            if(recordsList != null && recordsList.Count > 0)
            {
               record = recordsList[0];
            }
         }

         return record;
      }

      /// <summary>
      /// Executes a select statement
      /// </summary>
      /// <param name="statement">
      /// The statement.
      /// </param>
      /// <returns>
      /// The <see cref="IList"/>.
      /// </returns>
      private IList<T> ExecuteSelectStatement(string statement)
      {
         using(SQLiteConnection connection = this.databaseManager.ConnectToDatabase())
         {
            connection.Open();
            var sqLiteCommand = new SQLiteCommand(statement, connection);
            using(SQLiteDataReader reader = sqLiteCommand.ExecuteReader())
            {
               return new EntityMapper().Map<T>(reader);
            }
         }
      }

      /// <summary>
      ///    Get all records 
      /// </summary>
      public IList<T> GetAll()
      {
         var entity = new T();
         return this.ExecuteSelectStatement($"SELECT * FROM [{entity.GetType().Name}]");
      }

      /// <summary>
      /// Overloaded execute select- returns a list from a passed in filter parameter
      /// </summary>
      /// <param name="filter">
      /// The filter.
      /// </param>
      /// <typeparam name="TEntity">
      /// </typeparam>
      /// <returns>
      /// The <see cref="IList"/>.
      /// </returns>
      private IList<TEntity> ExecuteSelectStatement<TEntity>(Filter<TEntity> filter)
      where TEntity : class, new()
      {
         using(SQLiteConnection connection = this.databaseManager.ConnectToDatabase())
         {
            connection.Open();
            var sqLiteCommand = new SQLiteCommand(filter.Query, connection);
            using(SQLiteDataReader reader = sqLiteCommand.ExecuteReader())
            {
               return new EntityMapper().Map<TEntity>(reader);
            }
         }
      }

      /// <summary>
      /// Makes a connection to the database and executes 
      /// </summary>
      /// <param name="statement">
      /// The statement.
      /// </param>
      /// <returns>
      /// The <see cref="int"/>.
      /// </returns>
      private int ExecuteSQLStatement(string statement)
      {
         int recordsChanged = 0;
         using(SQLiteConnection connection = this.databaseManager.ConnectToDatabase())
         {
            connection.Open();
            using(var sqLiteCommand = new SQLiteCommand(statement, connection))
            {
               try
               {
                  recordsChanged = sqLiteCommand.ExecuteNonQuery();
               }
               catch(Exception e)
               {
#if DEBUG
                  Console.WriteLine(e.Message);
#endif
               }
            }
            return recordsChanged;
         }
      }

      /// <summary>
      /// Uses reflection to allow for generic record management, returns a list of property info
      /// </summary>
      /// <param name="record">
      /// The record.
      /// </param>
      /// <returns>
      /// The <see cref="IList"/>.
      /// </returns>
      private IList<PropertyInfo> GetPropertyInfoList(T record)
      {
         return record.GetType().GetProperties().Where(
            dbAttribute =>
               dbAttribute.CustomAttributes.FirstOrDefault(attribute => attribute.AttributeType == typeof(DBAttribute))
               != null).ToList();
      }
   }

   /// <summary>
   ///    A modified utility class for mapping datatypes executed by the GenericDataStore class
   ///    Original Author: Swaraj Ketan Santra
   ///    Email: swaraj.ece.jgec@gmail.com
   ///    Subsequent Author: Grant Callant
   ///    Email: grant@grantcallant.com
   ///    License: MIT
   ///    The MIT License (MIT)
   ///    Copyright (c)
   ///    Permission is hereby granted, free of charge, to any person obtaining a copy of this software and
   ///    associated documentation files (the "Software"), to deal in the Software without restriction, including
   ///    without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
   ///    copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following
   ///    conditions:
   ///    The above copyright notice and this permission notice shall be included in all copies or substantial portions of the
   ///    Software.
   ///    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
   ///    WARRANTIES
   ///    OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
   ///    HOLDERS BE
   ///    LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
   ///    OUT OF OR
   ///    IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
   ///    URL: https://code.msdn.microsoft.com/windowsapps/how-to-use-sqlite-with-96f22fac
   /// </summary>
   internal class EntityMapper
   {
      /// <summary>
      /// Maps updates to the property parameters
      /// </summary>
      /// <param name="reader">
      /// The reader.
      /// </param>
      /// <typeparam name="T">
      /// </typeparam>
      /// <returns>
      /// The <see cref="IList"/>.
      /// </returns>
      public IList<T> Map<T>(SQLiteDataReader reader)
      where T : class, new()
      {
         IList<T> list = new List<T>();
         while(reader.Read())
         {
            var record = new T();
            foreach(PropertyInfo info in record.GetType().GetProperties().Where(
               property => property.CustomAttributes.FirstOrDefault(
                              attribute => attribute.AttributeType == typeof(DBAttribute)) != null).ToList())
            {
               try
               {
                  var attribute = (DBAttribute)info.GetCustomAttribute(typeof(DBAttribute));

                  if(attribute != null)
                  {
                     if(reader[info.Name] != DBNull.Value)
                     {
                        info.SetValue(record, reader[info.Name]);
                     }
                  }
               }
               catch(Exception e)
               {
#if DEBUG
                  Console.WriteLine(e.Message);
                  Console.ReadLine();
#endif
               }
            }
            list.Add(record);
         }
         return list;
      }
   }

   /// <summary>
   ///    A modified utility class for finding data objects based on injected parameters
   ///    Original Author: Swaraj Ketan Santra
   ///    Email: swaraj.ece.jgec@gmail.com
   ///    Subsequent Author: Grant Callant
   ///    Email: grant@grantcallant.com
   ///    License: MIT
   ///    The MIT License (MIT)
   ///    Copyright (c)
   ///    Permission is hereby granted, free of charge, to any person obtaining a copy of this software and
   ///    associated documentation files (the "Software"), to deal in the Software without restriction, including
   ///    without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
   ///    copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following
   ///    conditions:
   ///    The above copyright notice and this permission notice shall be included in all copies or substantial portions of the
   ///    Software.
   ///    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
   ///    WARRANTIES
   ///    OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
   ///    HOLDERS BE
   ///    LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
   ///    OUT OF OR
   ///    IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
   ///    URL: https://code.msdn.microsoft.com/windowsapps/how-to-use-sqlite-with-96f22fac
   /// </summary>
   public class Filter<T>
   where T : class, new()
   {
      /// <summary>
      /// The statement.
      /// </summary>
      private readonly StringBuilder statement;

      /// <summary>
      /// Gets the entity name.
      /// </summary>
      private string EntityName { get; }

      /// <summary>
      /// The parametrized query
      /// </summary>
      internal string Query =>
         $"SELECT * FROM [{this.EntityName}] {(this.statement.ToString() != "" ? "WHERE" : "")} {this.statement};";

      /// <summary>
      /// Initializes a new instance of the <see cref="Filter{T}"/> class.
      /// </summary>
      public Filter()
      {
         this.statement = new StringBuilder();
         this.EntityName = typeof(T).Name;
      }

      /// <summary>
      /// Adds an expression to the filter
      /// </summary>
      /// <param name="expression">
      /// The expression.
      /// </param>
      /// <param name="expressionValue">
      /// The expression value.
      /// </param>
      public void Add(Expression<Func<T, dynamic>> expression, dynamic expressionValue)
      {
         if(!string.IsNullOrEmpty(this.statement.ToString()))
         {
            this.statement.Append(" AND ");
         }
         this.statement.Append(
            $" [{this.NameOf(expression)}] = {(expressionValue != null ? $"'{expressionValue}'" : "NULL")}");
      }

      /// <summary>
      /// Gets the reflected membername of the expression
      /// </summary>
      /// <param name="expression">
      /// The expression.
      /// </param>
      /// <returns>
      /// The <see cref="string"/>.
      /// </returns>
      private string NameOf(Expression<Func<T, object>> expression)
      {
         var memberExpression = (MemberExpression)expression.Body;

         if(memberExpression == null)
         {
            var unaryExpression = (UnaryExpression)expression.Body;
            memberExpression = (MemberExpression)unaryExpression.Operand;
         }
         return memberExpression.Member.Name;
      }
   }
}