using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;
namespace Lab31
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            dataGridView1.CellEndEdit += check;
        }
        void check(object sender, DataGridViewCellEventArgs e)
        {
            
            try
            {
                if (dataGridView1[e.RowIndex, e.ColumnIndex].Value == null)
                    return;
                string text = dataGridView1[e.RowIndex, e.ColumnIndex].Value.ToString();
                if (text.ToArray().All(x => char.IsDigit(x) || x == ','))
                {
                    return;
                }
                MessageBox.Show("Введено неправильное значение");
                dataGridView1[e.RowIndex, e.ColumnIndex].Value = "";
            }
            catch (Exception ex)
            {

            }
        }
        //Функция f(x) = ax
        double function(double x, double a)
        {
            return a * x;
        }
        //Посторить график и найти a
        private void начертитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Очистка графика
            chart1.Series[0].Points.Clear();
            chart1.Series[1].Points.Clear();
            double a = 0;
            double xsred = 0;
            double ysred = 0;
            double xmin = double.MaxValue;
            double xmax = double.MinValue;
            double firstexpression = 0;
            double secondexpression = 0;
            //Нахождение максимальной и минимальной точки X, а также средняя x и y 
            for (int i = 0; i < dataGridView1.RowCount; i++)
            { 
                double x = Convert.ToDouble(dataGridView1.Rows[i].Cells[0].Value);
                double y = Convert.ToDouble(dataGridView1.Rows[i].Cells[1].Value);

                if (xmin > x)
                    xmin = x;
                if (xmax < x)
                    xmax = x;


                xsred += x;
                ysred += y;
                chart1.Series[1].Points.AddXY(x,y);
            }
            //Нахождения точка а
            for (int i = 0; i < dataGridView1.RowCount; i++)
            {

                firstexpression += (Convert.ToDouble(dataGridView1.Rows[i].Cells[0].Value) - xsred) * (Convert.ToDouble(dataGridView1.Rows[i].Cells[0].Value) - ysred);
                secondexpression += Math.Pow((Convert.ToDouble(dataGridView1.Rows[i].Cells[0].Value) - xsred), 2);
            }
            a = firstexpression / secondexpression;
            textBox2.Text = a.ToString();
            //Постройка графика по точке
            try
            {
                for (int i = Convert.ToInt32(xmin); i < Convert.ToInt32(xmax); i++)
                {
                    chart1.Series[0].Points.AddXY(i, function(i, a));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Введены неправильные значения или функция");
            }
        }
        //Очистка графика
        private void очиститьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            chart1.Series[0].Points.Clear();
            chart1.Series[1].Points.Clear();
        }
        //Загрузка таблицы из excel
        private void button2_Click(object sender, EventArgs e)
        {
            //Очистка прошлой таблицы
            dataGridView1.Rows.Clear();
            //Диолог для открытия файла таблицы
            OpenFileDialog fl = new OpenFileDialog();
            var result = fl.ShowDialog();
            string FileName = fl.FileName;
            object rOnly = true;
            object SaveChanges = false;
            object MissingObj = System.Reflection.Missing.Value;

            Excel.Application app = new Excel.Application();
            Excel.Workbooks workbooks = null;
            Excel.Workbook workbook = null;
            Excel.Sheets sheets = null;
            try
            {
                workbooks = app.Workbooks;
                workbook = workbooks.Open(FileName, MissingObj, rOnly, MissingObj, MissingObj,
                                            MissingObj, MissingObj, MissingObj, MissingObj, MissingObj,
                                            MissingObj, MissingObj, MissingObj, MissingObj, MissingObj);

                //Получение всех страниц докуента
                sheets = workbook.Sheets;
                foreach (Excel.Worksheet worksheet in sheets)
                {
                    // Получаем диапазон используемых на странице ячеек
                    Excel.Range UsedRange = worksheet.UsedRange;
                    // Получаем строки в используемом диапазоне
                    Excel.Range urRows = UsedRange.Rows;
                    // Получаем столбцы в используемом диапазоне
                    Excel.Range urColums = UsedRange.Columns;

                    // Количества строк и столбцов
                    int RowsCount = urRows.Count;
                    int ColumnsCount = urColums.Count;
                    for (int i = 1; i <= RowsCount; i++)
                    {
                        dataGridView1.Rows.Add();
                        for (int j = 1; j <= ColumnsCount; j++)
                        {
                            Excel.Range CellRange = UsedRange.Cells[i, j];
                            // Получение текста ячейки
                            string CellText = (CellRange == null || CellRange.Value2 == null) ? null :
                                                (CellRange as Excel.Range).Value2.ToString();

                            if (CellText != null)
                            {
                                dataGridView1.Rows[i-1].Cells[j-1].Value = CellText;
                            }
                        }
                    }
                    // Очистка неуправляемых ресурсов на каждой итерации
                    if (urRows != null) Marshal.ReleaseComObject(urRows);
                if (urColums != null) Marshal.ReleaseComObject(urColums);
                if (UsedRange != null) Marshal.ReleaseComObject(UsedRange);
                if (worksheet != null) Marshal.ReleaseComObject(worksheet);
            }
            }
            catch (Exception ex)
            {
                /* Обработка исключений */
            }
            finally
            {
                //    /* Очистка оставшихся неуправляемых ресурсов */
            if (sheets != null) Marshal.ReleaseComObject(sheets);
            if (workbook != null)
            {
                workbook.Close(SaveChanges);
                Marshal.ReleaseComObject(workbook);
                workbook = null;
            }

            if (workbooks != null)
            {
                workbooks.Close();
                Marshal.ReleaseComObject(workbooks);
                workbooks = null;
            }
        }

    }
        //Рандоманая генерация точек
        private void button3_Click(object sender, EventArgs e)
        {

            dataGridView1.Rows.Clear();
            Random rnd = new Random();
            //Генерируются рандомно только точки y
            try
            {
                for (int i = 0; i < Convert.ToInt32(textBox1.Text); i++)
                {
                    dataGridView1.Rows.Add();
                    dataGridView1.Rows[i].Cells[0].Value = i;
                    dataGridView1.Rows[i].Cells[1].Value = rnd.Next() % Convert.ToInt32(textBox1.Text);
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("Введите число точек");
            }
        }
    }
}
