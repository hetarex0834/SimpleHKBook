using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
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
using System.Windows.Shapes;
using System.Xaml.Schema;
using System.Xml.Linq;

namespace SimpleHKBook
{
    /// <summary>
    /// 修正・削除用モーダルウインドウ
    /// ModalWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class ModalWindow : Window
    {
        private readonly string id; // レコードID
        private readonly string expense = "expense"; // 支出
        private readonly string income = "income"; // 収入

        /// <summary>
        /// コントロールにDataGridの内容を設定
        /// </summary>
        /// <param name="dgd">DataGrid</param>
        public ModalWindow(DataGrid dgd)
        {
            InitializeComponent();

            // レコードID
            id = ((TextBlock)dgd.Columns[0].GetCellContent(dgd.SelectedItem)).Text;
            // DatePicker
            dp.Text = ((TextBlock)dgd.Columns[1].GetCellContent(dgd.SelectedItem)).Text;
            // RadioButton
            switch (((TextBlock)dgd.Columns[2].GetCellContent(dgd.SelectedItem)).Text)
            {
                case "支出":
                    rbtExpense.IsChecked = true;
                    break;
                case "収入":
                    rbtIncome.IsChecked = true;
                    break;
            }
            // ComboBox
            for (var i = 0; i < cmbCategory.Items.Count; i++)
            {
                if ($"{cmbCategory.Items[i]}" == ((TextBlock)dgd.Columns[3].GetCellContent(dgd.SelectedItem)).Text)
                {
                    cmbCategory.SelectedIndex = i;
                    break;
                }
            }
            // TextBox
            txtAmount.Text = ((TextBlock)dgd.Columns[4].GetCellContent(dgd.SelectedItem)).Text;
            txtMemo.Text = ((TextBlock)dgd.Columns[5].GetCellContent(dgd.SelectedItem)).Text;
        }
        
        /// <summary>
        /// ComboBoxにアイテムを設定
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

            cmbCategory.Items.Clear();
            foreach (var item in items[group]) cmbCategory.Items.Add(item);
            cmbCategory.SelectedIndex = 0;
        }

        /// <summary>
        /// RadioButtonの値に応じてComboBoxの区分を変更
        /// </summary>
        /// <param name="sender">RadioButton</param>
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
        /// txtAmountへの貼り付けを制限
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TxtAmount_PreviewExecuted(object sender, ExecutedRoutedEventArgs e) => e.Handled = e.Command == ApplicationCommands.Paste;

        /// <summary>
        /// 修正内容を保存
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

            // XMLファイル
            var xml = XElement.Load("HKBookData.xml");
            // 修正対象
            var elem = xml.Descendants("HKBookData")
                             .Where(x => x.Element("ID")!.Value == id)
                             .Select(x => x).Single();
            // 修正内容をXMLファイルへ保存
            elem.Element("日付")!.Value = $"{dp.SelectedDate:yyyy/MM/dd}";
            elem.Element("区分")!.Value = (rbtExpense.IsChecked == true) ? "支出" : "収入";
            elem.Element("カテゴリ")!.Value = $"{cmbCategory.SelectedItem}";
            elem.Element("金額")!.Value = $"{int.Parse(txtAmount.Text)}";
            elem.Element("メモ")!.Value = txtMemo.Text;
            xml.Save("HKBookData.xml");

            MessageBox.Show("保存しました。", "保存完了", MessageBoxButton.OK, MessageBoxImage.Information);
            Close();
        }

        /// <summary>
        /// レコードを削除
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            // XMLファイル
            var xml = XElement.Load("HKBookData.xml");
            // 削除対象
            var elem = xml.Descendants("HKBookData")
                             .Where(x => x.Element("ID")!.Value == id)
                             .Select(x => x).Single();
            // 対象をXMLファイルから削除
            elem.Remove();
            xml.Save("HKBookData.xml");

            MessageBox.Show("削除しました。", "削除完了", MessageBoxButton.OK, MessageBoxImage.Information);
            Close();
        }
    }
}
