using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.Text.RegularExpressions;

namespace DataTime
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        public class Auth
        {
            public static string Id = null;
            public static string Name = null;
            public static string Price = null;
            public static string Date = null;
        }
        MySqlConnection conn;

        private MySqlDataAdapter MyDA = new MySqlDataAdapter();
        private BindingSource bSource = new BindingSource();
        private DataSet ds = new DataSet();
        private DataTable table = new DataTable();

        public void SetMyCustomFormat()
        {

            dateTimePicker1.Format = DateTimePickerFormat.Custom;
            dateTimePicker1.CustomFormat = "YYYY-MM-MM";
        }
        public string data1(DateTimePicker a)
        {
            var DataTime1 = dateTimePicker1.Value.ToString("yyyy-MM-dd HH:mm:ss");
            return DataTime1.ToString();
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            string connStr = "server=chuc.caseum.ru;port=33333;user=st_1_20_4;database=is_1_20_st4_KURS;password=32006333;";
            conn = new MySqlConnection(connStr);
            GetListUsers();
            //Видимость полей в гриде
            dataGridView1.Columns[0].Visible = true;
            dataGridView1.Columns[1].Visible = true;
            dataGridView1.Columns[2].Visible = true;
            dataGridView1.Columns[3].Visible = true;


            //Ширина полей
            dataGridView1.Columns[0].FillWeight = 15;
            dataGridView1.Columns[1].FillWeight = 40;
            dataGridView1.Columns[2].FillWeight = 15;
            dataGridView1.Columns[3].FillWeight = 15;

            //Режим для полей "Только для чтения"
            dataGridView1.Columns[0].ReadOnly = true;
            dataGridView1.Columns[1].ReadOnly = true;
            dataGridView1.Columns[2].ReadOnly = true;
            dataGridView1.Columns[3].ReadOnly = true;

            //Растягивание полей грида
            dataGridView1.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dataGridView1.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dataGridView1.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dataGridView1.Columns[3].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            //Убираем заголовки строк
            dataGridView1.RowHeadersVisible = false;
            //Показываем заголовки столбцов
            dataGridView1.ColumnHeadersVisible = true;
            ChangeColorDGV();
        }
        private void ChangeColorDGV()
        {

            //Отражаем количество записей в ДатаГриде
            int count_rows = dataGridView1.RowCount - 1;


            var DataTime2 = DateTime.Now.AddDays(-3);
            var DataTime3 = DateTime.Now;

            //Проходимся по ДатаГриду и красим строки в нужные нам цвета, в зависимости от статуса студента
            for (int i = 0; i < count_rows; i++)
            {

                //статус конкретного студента в Базе данных, на основании индекса строки
                DateTime id_selected_status = Convert.ToDateTime(dataGridView1.Rows[i].Cells[3].Value);
                //Логический блок для определения цветности

                if (DataTime3 > id_selected_status)
                {

                    dataGridView1.Rows[i].DefaultCellStyle.BackColor = Color.Red;
                }
                if (DataTime2 > id_selected_status)
                {
                    //Красим в желтый
                    dataGridView1.Rows[i].DefaultCellStyle.BackColor = Color.Yellow;
                }

            }
        }

        public void reload_list()
        {
            table.Clear();
            GetListUsers();
            ChangeColorDGV();
        }
        public void DeleteUser()
        {
            // string st1 = dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString();
            //Формируем строку запроса на добавление строк
            // string sql_delete_user = "DELETE FROM Prodykti WHERE id=" + st1;
            //Посылаем запрос на обновление данных
            //MySqlCommand delete_user = new MySqlCommand(sql_delete_user, conn);
            try
            {
                conn.Open();
                // delete_user.ExecuteNonQuery();
                MessageBox.Show("Удаление прошло успешно", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка удаления строки \n" + ex, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }
            finally
            {
                conn.Close();
                //Вызов метода обновления ДатаГрида
                reload_list();
            }
        }
        public void GetListUsers()
        {
            string CommandStr = "SELECT * FROM Prodykti";
            conn.Open();
            MyDA.SelectCommand = new MySqlCommand(CommandStr, conn);
            MyDA.Fill(table);
            //Указываем, что источником данных в bindingsource является заполненная выше таблица
            bSource.DataSource = table;
            //Указываем, что источником данных ДатаГрида является bindingsource 
            dataGridView1.DataSource = bSource;
            //Закрываем соединение
            conn.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {

            if (textBox1.Text == "")
            {
                MessageBox.Show("Введите мне имя", "Ошибка");
                return;
            }
            if (textBox2.Text == "")
            {
                MessageBox.Show("Какова моя цена?", "Ошибка");
                return;
            }

            if (dateTimePicker1.Text == "")
            {
                MessageBox.Show("Дата изготовки", "Ошибка");
                return;
            }

            string gg = data1(dateTimePicker1);
            MySqlCommand command = new MySqlCommand($"INSERT INTO Prodykti (Name,Price,Date) VALUES(@Name, @Price,'{gg}');", conn);
            conn.Open();

            command.Parameters.Add("@Name", MySqlDbType.VarChar, 25).Value = textBox1.Text;
            command.Parameters.Add("@Price", MySqlDbType.Float, 25).Value = textBox2.Text;

            try
            {
                if (command.ExecuteNonQuery() == 1)
                {
                    MessageBox.Show("Вы успешно добавили");
                    GetListUsers();
                    table.Clear();
                    ChangeColorDGV();
                }
                else
                {
                    MessageBox.Show("Произошла ошибка");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка" + ex);
            }
            finally
            {
                conn.Close();
            }


        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            ChangeColorDGV();
            DeleteUser();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            reload_list();
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            dataGridView1.CurrentCell = null;
            string selected_id = textBox5.Text;
            int count_rows = dataGridView1.RowCount - 1;
            Regex regex = new Regex($@"{selected_id}(\w*)");
            for (int i = 0; i < count_rows; i++)
            {
                DataGridViewRow row = dataGridView1.Rows[i];
                string a = Convert.ToString(row.Cells[1].Value);
                MatchCollection matches = regex.Matches(a);

                if (matches.Count > 0)
                // if (regex.IsMatch(row))
                {
                    dataGridView1.Rows[i].Visible = true;

                }
                else
                {
                    dataGridView1.Rows[i].Visible = false;
                }
                ChangeColorDGV();

            }

        }
    }
    }

            

        
    
    
    

