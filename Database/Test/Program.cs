////======================================================================
////
////        Filename    : Program.cs
////        Description : Database test
////
////        Created by kesalin@gmail.com at 2013-3-24 14:00:16
////        http://blog.csdn.net/kesalin/
//// 
////======================================================================

using System;
using Database.Core;
using Database.Utility;

namespace Test
{
    // SQL syntax
    //
    // Select : http://en.wikipedia.org/wiki/Select_(SQL)
    // Insert : http://en.wikipedia.org/wiki/Insert_(SQL)
    // Delete : http://en.wikipedia.org/wiki/Delete_(SQL)
    // Updated : http://en.wikipedia.org/wiki/Update_(SQL)
    // Truncate : http://en.wikipedia.org/wiki/Truncate_(SQL)

    internal class Program
    {
        private IDatabase _db;
        private const DatabaseType _dbType = DatabaseType.MySQL;

        #region Database related

        // You need to create a MySQL database named "sample" with columns 
        // id(int), Name(varchar(45)), Address(varchar(45)), Age(int) for this test.
        // 
        private void CreateDatabase()
        {
            if (_db == null)
            {
                // Setup you database information here.
                //
                var connStr = DatabaseHelper.CreateConnectionString(_dbType, "localhost", "sample", "root", "123456");
                _db = DatabaseFactory.CreateDatabase(_dbType, connStr);

                if (_db == null)
                    Console.WriteLine(" >> Failed to create database with connection string {0}.", connStr);
                else
                    Console.WriteLine(" >> Created database.");
            }
        }

        private void CloseDatabase()
        {
            if (_db != null)
            {
                _db.Dispose();
                _db = null;
            }    
        }

        public void TestInsert()
        {
            if (_db == null)
                return;

            const string sqlCmd = "insert into customer (id, Name,Address,Age) values (0,'飘飘白云','上海张江高科',28)";

            try
            {
                _db.Open();
                _db.ExcuteSql(sqlCmd);

                Console.WriteLine(" >> Succeed. {0}", sqlCmd);
            }
            catch (Exception ex)
            {
                Console.WriteLine(" >> Failed to {0}. {1}", sqlCmd, ex.Message);
            }
            finally
            {
                _db.Close();
            }
        }

        public void TestFind()
        {
            if (_db == null)
                return;

            const string sqlCmd = "select Name,Address,Age from customer where Name='飘飘白云'";

            try
            {
                _db.Open();
                var dataSet = _db.ExcuteSqlForDataSet(sqlCmd);
                var recordCount = DatabaseHelper.GetRowCount(dataSet);

                Console.WriteLine(" >> Excuted {0}", sqlCmd);
                Console.WriteLine(" >> Found {0} record.", recordCount);

                for (int i = 0; i < recordCount; i++)
                {
                    var name = DatabaseHelper.GetValue(dataSet, i, 0) as string;
                    var address = DatabaseHelper.GetValue(dataSet, i, 1) as string;
                    var age = DatabaseHelper.GetIntValue(dataSet, i, 2);

                    Console.WriteLine("    >> Record {0}, Name:{1}, Address:{2}, Age:{3}", i + 1, name, address, age);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(" >> Failed to {0}. {1}", sqlCmd, ex.Message);
            }
            finally
            {
                _db.Close();
            }
        }

        public void TestUpdate()
        {
            if (_db == null)
                return;

            const string sqlCmd = "update customer set Address='张江高科' where Name='飘飘白云'";

            try
            {
                _db.Open();
                _db.ExcuteSql(sqlCmd);

                Console.WriteLine(" >> Succeed. {0}", sqlCmd);
            }
            catch (Exception ex)
            {
                Console.WriteLine(" >> Failed to {0}. {1}", sqlCmd, ex.Message);
            }
            finally
            {
                _db.Close();
            }
        }

        public void TestDelete()
        {
            if (_db == null)
                return;

            const string sqlCmd = "delete from customer where Name='飘飘白云'";

            try
            {
                _db.Open();
                _db.ExcuteSql(sqlCmd);

                Console.WriteLine(" >> Succeed. {0}", sqlCmd);
            }
            catch (Exception ex)
            {
                Console.WriteLine(" >> Failed to {0}. {1}", sqlCmd, ex.Message);
            }
            finally
            {
                _db.Close();
            }
        }

        #endregion

        static void Main(string[] args)
        {
            var runner = new Program();

            runner.CreateDatabase();

            runner.TestInsert();
            runner.TestFind();

            runner.TestUpdate();
            runner.TestFind();

            runner.TestDelete();
            runner.TestFind();

            runner.CloseDatabase();

            Console.ReadLine();
        }
    }
}
