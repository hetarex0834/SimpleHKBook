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
        public MainWindow()
        {
            InitializeComponent();

            SetCmbCategoryItem();
        }

        private void SetCmbCategoryItem()
        {
            var items = new Dictionary<string, string[]>
            {
                // 支出
                {
                    "expense",
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
                    "income",
                    new string[]
                    {
                        "給料",
                        "その他",
                    }
                },
            };

            foreach (var item in items["expense"]) cmbCategory.Items.Add(item);
        }
    }
}
