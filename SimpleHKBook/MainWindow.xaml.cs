using System;
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
        private readonly string expense = "expense"; // 支出
        private readonly string income = "income"; // 収入

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
                dgd.DataContext = dt;
            }
        }

        /// <summary>
        /// DataTableを初期化
        /// </summary>
        private void InitDataTable()
        {
            var clms = new Dictionary<string, Type>
            {
                { "日付", typeof(string) },
                { "区分", typeof(string) },
                { "カテゴリ", typeof(object) },
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
        /// ラジオボタンの値に応じてコンボボックスの区分を変更
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            var rbt = sender as RadioButton;
            if (cmbCategory is not null)
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
        private void TxtAmount_PreviewTextInput(object sender, TextCompositionEventArgs e) => e.Handled = !new Regex("[0-9]").IsMatch(e.Text);

        /// <summary>
        /// txtAmountへの貼り付けを禁止
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TxtAmount_PreviewExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Command == ApplicationCommands.Paste) e.Handled = true;
        }

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
                var row = dt.NewRow();
                row["日付"] = $"{dp.SelectedDate:yyyy-MM-dd}";
                row["区分"] = (rbtExpense.IsChecked == true) ? "支出" : "収入";
                row["カテゴリ"] = cmbCategory.SelectedItem;
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
                MessageBox.Show("保存ファイルとテーブルの整合性が合いませんでした。", "保存ファイル不整合", MessageBoxButton.OK, MessageBoxImage.Error);
                InitDataTable();
                SaveDataTableToXml();
            }
            finally
            {
                dgd.DataContext = dt;
            }
        }
    }
}
