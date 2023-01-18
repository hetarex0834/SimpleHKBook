using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

namespace SimpleHKBook
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // カテゴリ
        private readonly string expense = "expense"; // 支出
        private readonly string income = "income"; // 収入

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            SetCmbCategoryItem(expense);
        }

        /// <summary>
        /// ラジオボタンの値に応じてコンボボックスのカテゴリを変更
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
        /// コンボボックスにアイテムを設定
        /// </summary>
        /// <param name="category">カテゴリ</param>
        private void SetCmbCategoryItem(in string category)
        {
            var items = new Dictionary<string, string[]>
            {
                // 支出
                {
                    expense,
                    new string[]
                    {
                        "食費",
                        "日用雑貨",
                        "家賃・住宅ローン",
                        "水道光熱費",
                        "保険料",
                        "医療費",
                        "通信費",
                        "交通費",
                        "教育費",
                        "衣服費",
                        "交際費",
                        "趣味",
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

            cmbCategory.Items.Clear();
            foreach (var item in items[category]) cmbCategory.Items.Add(item);
            cmbCategory.SelectedIndex = 0;
        }
    }
}
