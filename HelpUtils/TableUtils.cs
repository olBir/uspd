using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;
using System.Data.Common;
using System.Data;
using System.Windows.Forms;
using System.Configuration;

namespace TableUtils
{
    /// <summary>
    ///  Класс OperationWithDB
    ///  некоторые операции с базами данных
    /// </summary>
    public class OperationWithDB : IDisposable
    {
        private static String connectionString = "Server=127.0.0.1;Port=5432;Database=Wave;User Id=postgres;Password=root;";
        private NpgsqlConnection npgSqlConnection;

        /// <summary>
        /// Формирует строку соеденения к бд из файла настроек 
        /// </summary>
        /*private string getConnectionStringFromSettings()
        {

            return "Server=" + USPD.connectline.Default.server + ";Port=" + USPD.connectline.Default.port + ";Database=" + USPD.connectline.Default.database + ";User Id=" + USPD.connectline.Default.userid + ";Password=" + USPD.connectline.Default.password + ";";
        }*/

        public OperationWithDB()
        {
            //Настраиваем соеденение
            /*{
                String conStr = getConnectionStringFromSettings();
                if (conStr != null)
                {
                    connectionString = conStr;
                }
            }

            npgSqlConnection = new NpgsqlConnection(connectionString);*/

            //npgSqlConnection = new NpgsqlConnection(getConnectionStringFromSettings());

            npgSqlConnection = new NpgsqlConnection(connectionString);
        }


        /// <summary>
        ///  Метод обертка
        ///  для отсылания sql запроса без возвращаемого результата бд
        /// </summary>
        public void doSqlQuery(String query)
        {
            npgSqlConnection.Open();

            NpgsqlCommand sqlQuery = new NpgsqlCommand(query, npgSqlConnection);

            try
            {
                sqlQuery.ExecuteNonQuery();
            }
            catch (Npgsql.NpgsqlException ex)
            {
                MessageBox.Show("Error in doSqlQueryWitouthAnswer", ex.Message);
            }
            finally
            {
                npgSqlConnection.Close();
            }
        }

        /// <summary>
        ///  Метод для очистки таблицы
        /// </summary>
        /// <param name="tableName">Имя таблицы</param>
        public void clearTable(String tableName)
        {
            doSqlQuery("DELETE FROM " + tableName);
        }

        public void Dispose()
        {
            npgSqlConnection.Dispose();
        }


    }


    /// <summary>
    ///  Класс FillTable
    ///  позволяет заполнять таблицы случайными данными
    /// </summary>
    public class FillTable : IDisposable
    {
        private OperationWithDB db = new OperationWithDB();
        private static int numOfRowsInAbonents = 0;

        /// <summary>
        ///  Метод для очистки всех таблиц
        /// </summary>
        private void clearAllTables()
        {
            db.clearTable("abonents");
            db.clearTable("electricitysupplyref");
            db.clearTable("abonentshistory");
        }

        /// <summary>
        ///  Метод для заполнения таблицы electricitysupplyref
        /// </summary>
        /// <param name="numberOfLines">Количество строк в таблице</param>
        private void fillElectricitysupplyref(int numberOfLines)
        {
            for (int i = 0; i < numberOfLines; i++)
            {
                db.doSqlQuery("INSERT INTO electricitysupplyref VALUES (" + i + ", 'description_for_electricitysupply" + i + "'," + i + ");");
            }
        }

        /// <summary>
        ///  Метод для заполнения таблицы abonents
        /// </summary>
        /// <param name="numberOfLines">Количество строк в таблице</param>
        private void fillAbonents()
        {
            Random rand = new Random();
            numOfRowsInAbonents = rand.Next(50);
            for (int i = 0; i < numOfRowsInAbonents; i++)
            {
                db.doSqlQuery("INSERT INTO abonents VALUES (" + i + ", 'description'," + rand.Next(1, 4) + "," + rand.Next(1, 4) + "," + rand.Next(1, 4) + "," + rand.Next(1, 4) + "," + rand.Next(1, 4) + ",'" + DateTime.Now + "','mode'," + rand.Next(1, 4) + "," + rand.Next(1, 4) + ");");
            }
        }

        private void fillAbonentsHistory()
        {
            Random rand = new Random();
            for (int i = 0; i < numOfRowsInAbonents; i++)
            {
                for (int j = 0; j < 10; j++)
                    db.doSqlQuery("INSERT INTO abonentshistory VALUES (" + i + ",'" + DateTime.Now.AddDays(-j) + "'," + rand.Next(1, 500) + ");");
            }
        }


        /// <summary>
        ///  Метод для заполнения всех таблиц
        /// </summary>
        public void deleteAndFillAllTables()
        {
            clearAllTables();
            fillElectricitysupplyref(5);
            fillAbonents();
            fillAbonentsHistory();
        }

        public void changeValuesInDb()
        {
            //если еще нет данных заполняем таблицу
            if (numOfRowsInAbonents == 0)
            {
                deleteAndFillAllTables();
            }

            else
            {
                //меняем значения построчно
                Random rand = new Random();
                for (int i = 0; i < numOfRowsInAbonents; i++)
                {
                    db.doSqlQuery("UPDATE abonents SET levelaccess=" + rand.Next(1, 4) + ",number_trading_floor=" + rand.Next(1, 4) + ",number_sector=" + rand.Next(1, 4) + ",number_module=" + rand.Next(1, 4) + ",id_group_electricity_supply=" + rand.Next(1, 4) + ",amount_electricity_consumed=" + rand.Next(1, 4) + ",power_consumption=" + rand.Next(1, 4) + "where id=" + i.ToString() + ";");
                }
            }
        }

        /* static void Main(string[] args)
         {
             using (FillTable f = new FillTable())
             {
                 f.fillAllTables();
             }
         }
         */



        public void Dispose()
        {
            db.Dispose();
        }
    }
}
