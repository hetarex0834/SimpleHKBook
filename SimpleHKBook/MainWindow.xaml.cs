﻿using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Serialization;

namespace SimpleHKBook
{
    /// <summary>
    /// シンプル家計簿
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DataTable dt = new("HKBookData");
        private static readonly string expense = "expense"; // 支出
        private static readonly string income = "income"; // 収入
        private int year = DateTime.Now.Year;
        private int month = DateTime.Now.Month;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            SetCmbCategoryItem(expense);
            InitDataTable();

            try
            {
                LoadXmlToDataTable();
            }
            catch
            {
                SaveDataTableToXml();
            }
            finally
            {
                ExtractRecordsByDate();
            }
        }

        /// <summary>
        /// コンボボックスにアイテムを設定
        /// </summary>
        /// <param name="group">区分</param>
        private void SetCmbCategoryItem(in string group)
        {
            var items = new Dictionary<string, string[]>
            {
                // 支出
                {
                    expense,
                    new string[]
                    {
                        "食費",
                        "日用品費",
                        "住居費",
                        "水道光熱費",
                        "通信費",
                        "保険料",
                        "車両費",
                        "学費",
                        "税金",
                        "交通費",
                        "医療費",
                        "被服費",
                        "美容費",
                        "交際費",
                        "娯楽費",
                        "小遣い",
                        "その他",
                    }
                },
                // 収入
                {
                    income,
                    new string[]
                    {
                        "給料",
                        "その他",
                    }
                },
            };

            cmbCategory.DataContext = items[group];
            cmbCategory.SelectedIndex = 0;
        }

        /// <summary>
        /// DataTableを初期化
        /// </summary>
        private void InitDataTable()
        {
            var clms = new Dictionary<string, Type>
            {
                { "ID", typeof(int) },
                { "日付", typeof(string) },
                { "区分", typeof(string) },
                { "カテゴリ", typeof(string) },
                { "金額", typeof(int) },
                { "メモ", typeof(string) },
            };

            dt.Clear();
            dt.Rows.Clear();
            dt.Columns.Clear();
            foreach (var c in clms) dt.Columns.Add(c.Key, c.Value);
        }

        /// <summary>
        /// DataTableの内容をXML形式でファイル保存
        /// </summary>
        private void SaveDataTableToXml()
        {
            using var fs = new FileStream("HKBookData.xml", FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
            var serializer = new XmlSerializer(dt.GetType());
            serializer.Serialize(fs, dt);
        }

        /// <summary>
        /// XMLファイルの内容をDataTableに読み込む
        /// </summary>
        private void LoadXmlToDataTable()
        {
            using var fs = new FileStream("HKBookData.xml", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            var serializer = new XmlSerializer(dt.GetType());
            dt = serializer.Deserialize(fs) as DataTable ?? new("HKBookData");
        }

        /// <summary>
        /// txtTotalに集計結果を設定
        /// </summary>
        private void SetTotalValue()
        {
            int total = 0;
            foreach (DataRow r in dt.Rows)
            {
                switch (r["区分"])
                {
                    case "支出":
                        total -= (int)r["金額"];
                        break;
                    case "収入":
                        total += (int)r["金額"];
                        break;
                }
            }
            txtTotal.Text = $"{total:#,0}";
        }

        /// <summary>
        /// 1ヶ月分のレコードを抽出
        /// </summary>
        private void ExtractRecordsByDate()
        {
            lblSelectedDate.DataContext = (month < 10) ? $"{year}年0{month}月" : $"{year}年{month}月";

            var dt = this.dt.Clone();
            var rows = this.dt.AsEnumerable()
                            .Where(x => int.Parse($"{x["日付"]}".Split('/')[0]) == year)
                            .Where(x => int.Parse($"{x["日付"]}".Split('/')[1]) == month);

            foreach (var row in rows) dt.ImportRow(row);
            dgd.DataContext = dt;
            SetTotalValue();
        }

        /// <summary>
        /// ラジオボタンの値に応じてコンボボックスの区分を変更
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            var rbt = sender as RadioButton;
            if (cmbCategory != null)
            {
                if (rbt == rbtExpense) SetCmbCategoryItem(expense); // 支出
                else if (rbt == rbtIncome) SetCmbCategoryItem(income); // 収入
            }
        }

        /// <summary>
        /// 数字以外のtxtAmountへの入力を制限
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TxtAmount_PreviewTextInput(object sender, TextCompositionEventArgs e) => e.Handled = new Regex("[^0-9]").IsMatch(e.Text);

        /// <summary>
        /// txtAmountへの貼り付けを禁止
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TxtAmount_PreviewExecuted(object sender, ExecutedRoutedEventArgs e) => e.Handled = e.Command == ApplicationCommands.Paste;

        /// <summary>
        /// 入力結果を保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (txtAmount.Text == "")
            {
                MessageBox.Show("金額を入力してください。", "金額未入力", MessageBoxButton.OK, MessageBoxImage.Stop);
                return;
            }

            try
            {
                int id = (dt.Rows.Count > 0) ? Convert.ToInt32(dt.Compute("MAX(ID)", null)) + 1 : 1;
                var row = dt.NewRow();
                row["ID"] = id;
                row["日付"] = $"{dp.SelectedDate:yyyy/MM/dd}";
                row["区分"] = (rbtExpense.IsChecked == true) ? "支出" : "収入";
                row["カテゴリ"] = $"{cmbCategory.SelectedItem}";
                row["金額"] = int.Parse(txtAmount.Text);
                row["メモ"] = txtMemo.Text;
                dt.Rows.Add(row);
                SaveDataTableToXml();

                MessageBox.Show("保存しました。", "保存完了", MessageBoxButton.OK, MessageBoxImage.Information);
                txtAmount.Text = "";
                txtMemo.Text = "";
            }
            catch
            {
                if (MessageBox.Show("保存ファイルとテーブルの整合性が確認できませんでした。\nファイルを初期化しますか？",
                                               "保存ファイル不整合", MessageBoxButton.OKCancel, MessageBoxImage.Error) == MessageBoxResult.OK)
                {
                    InitDataTable();
                    SaveDataTableToXml();
                    MessageBox.Show("ファイルを初期化しました。", "初期化完了", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            finally
            {
                ExtractRecordsByDate();
            }
        }

        /// <summary>
        /// 修正・削除用ウインドウを表示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Dgd_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (dgd.SelectedItem != null)
            {
                var window = new ModalWindow(dgd)
                {
                    Owner = GetWindow(this),
                };
                window.ShowDialog();
            }
            LoadXmlToDataTable();
            ExtractRecordsByDate();
        }

        /// <summary>
        /// 1ヶ月前のレコードを抽出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnLeft_Click(object sender, RoutedEventArgs e)
        {
            if (month == 1)
            {
                month = 12;
                year--;
            }
            else month--;
            ExtractRecordsByDate();
        }

        /// <summary>
        /// 1ヶ月後のレコードを抽出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnRight_Click(object sender, RoutedEventArgs e)
        {
            if (month == 12)
            {
                month = 1;
                year++;
            }
            else month++;
            ExtractRecordsByDate();
        }
    }
}
