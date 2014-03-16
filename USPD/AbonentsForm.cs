
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Npgsql;



namespace USPD
{
    public partial class AbonentsForm : Form
    {
        private const int refreshTime = 0;//время между обновлениями мс
        private static String connectionString = "Server=127.0.0.1;Port=5432;Database=Wave;User Id=postgres;Password=root;";
        private BackgroundWorker backgroundGetDataFromTable = new System.ComponentModel.BackgroundWorker();

        private int number_trading_floor = 2;
        private int number_sector = 1;
        private int number_module = 0;

        //private const String functionName = "return_displayed_fields_of_abonents(number_trading_floor,number_sector,number_module);";//имя хранимой продцедуры
        private DataTable newData;//хранит в себе таблицу, являющуюся результатом хранимой продцедуры

        private String getFunctionName()
        {
            return "__displayed_fields_of_abonents("
                + (number_trading_floor != 0 ? number_trading_floor.ToString() : "")
                + (number_sector != 0 ? "," + number_sector.ToString() : "")
                + (number_module != 0 ? "," + number_module.ToString() : "")
                + ");";
        }



        public AbonentsForm()
        {
            InitializeComponent();
            backgroundGetDataFromTable.DoWork += new System.ComponentModel.DoWorkEventHandler(backgroundGetDataFromTable_DoWork);
            backgroundGetDataFromTable.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(backgroundGetDataFromTable_RunWorkerCompleted);
            //backgroundGetDataFromTable.RunWorkerAsync();

            numberTradingFloorLabel.Text = number_trading_floor.ToString();
            numberSectorLabel.Text = number_sector.ToString();

        }

        public void refreshData()
        {
            backgroundGetDataFromTable.RunWorkerAsync();
        }


        /// <summary>
        /// Получение таблицы из базы данных путем выполнения хранимой продцедуры
        /// </summary>
        /// <param name="nameOfFunc">Название хранимой продцедуры</param>
        /// <returns>Таблицу сгенерированную хранимой продцедурой</returns>

        private DataTable getDataFromDB(String nameOfFunc)
        {
            //устанавливаем соеденение с бд
            NpgsqlConnection npgSqlConnection = new NpgsqlConnection(connectionString);

            //формируем запрос
            NpgsqlCommand sqlQuery = new NpgsqlCommand("SELECT * FROM " + nameOfFunc + ";", npgSqlConnection);
            try
            {
                npgSqlConnection.Open();

                //заносим результат в ДатаСет
                DataSet ds = new DataSet();
                NpgsqlDataAdapter da = new NpgsqlDataAdapter(sqlQuery);
                da.Fill(ds);
                //da.Fill(0, 1, newData);

                return ds.Tables[0];//Возвращаем первую(единственную) таблицу из датасета
            }
            catch (Npgsql.NpgsqlException ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
            finally
            {
                npgSqlConnection.Close();
            }
        }

        /// <summary>
        /// Обновление данных на форме
        /// </summary>
        private void refreshDataInDataGrid()
        {

            //делаем одинаковую размерность у датагрида и таблицы из бд если в таблице есть данные
            if (dataGridView1.RowCount != newData.Rows.Count && newData.Rows.Count > 0)
            {
                dataGridView1.RowCount = newData.Rows.Count;
                //задаем всплывающий текст для ячеек
                for (int column = 0; column < dataGridView1.ColumnCount; column++)
                {
                    for (int row = 0; row < newData.Rows.Count; row++)
                    {        
                        dataGridView1.Rows[row].Cells[column].ToolTipText = dataGridView1.Columns[column].ToolTipText;
                    }
                }

            }
            /*
            if (dataGridView1.ColumnCount != newData.Columns.Count)
            {
                dataGridView1.ColumnCount = newData.Columns.Count;
            }*/


            //меняем значения по всем колонкам если они различные
            for (int column = 0; column < dataGridView1.ColumnCount; column++)
            {
                for (int row = 0; row < newData.Rows.Count; row++)
                {
                    //MessageBox.Show("row",row.ToString());
                    if (dataGridView1.Rows[row].Cells[column].Value != newData.Rows[row][column])
                    {
                        dataGridView1.Rows[row].Cells[column].Value = newData.Rows[row][column];
                    }
                }
            }
        }

        /// <summary>
        /// Метод вызываемый по завершению backgroundWorker
        /// </summary>
        private void backgroundGetDataFromTable_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //обновляем данные на форме
            refreshDataInDataGrid();
            //запускаем backgroundWorker еще раз
           // backgroundGetDataFromTable.RunWorkerAsync();
        }

        /// <summary>
        /// Тело backgroundWorker
        /// </summary>
        private void backgroundGetDataFromTable_DoWork(object sender, DoWorkEventArgs e)
        {
            //выполняем хранимую продцедуру
            newData = getDataFromDB(getFunctionName());
        }

        private void label1_MouseClick(object sender, MouseEventArgs e)
        {
            refreshData();
        }



    }
}
