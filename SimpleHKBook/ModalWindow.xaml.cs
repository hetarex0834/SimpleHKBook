﻿using System;
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

namespace SimpleHKBook
{
    /// <summary>
    /// 修正・削除用ウインドウ
    /// ModalWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class ModalWindow : Window
    {
        private readonly string expense = "expense"; // 支出
        private readonly string income = "income"; // 収入

        public ModalWindow(DataGrid dgd)
        {
            InitializeComponent();

            switch (((TextBlock)dgd.Columns[1].GetCellContent(dgd.SelectedItem)).Text)
            {
                case "支出":
                    rbtExpense.IsChecked = true;
                    break;
                case "収入":
                    rbtIncome.IsChecked = true;
                    break;
            }

            txtAmount.Text = ((TextBlock)dgd.Columns[3].GetCellContent(dgd.SelectedItem)).Text;
            txtMemo.Text = ((TextBlock)dgd.Columns[4].GetCellContent(dgd.SelectedItem)).Text;
            dp.Text = ((TextBlock)dgd.Columns[0].GetCellContent(dgd.SelectedItem)).Text;
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

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
