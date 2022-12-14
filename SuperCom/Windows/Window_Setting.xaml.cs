﻿using ICSharpCode.AvalonEdit;
using SuperCom.Config;
using SuperCom.Config.WindowConfig;
using SuperCom.Entity;
using SuperCom.ViewModel;
using SuperControls.Style;
using SuperControls.Style.Windows;
using SuperControls.Style.XAML.CustomWindows;
using SuperUtils.Common;
using SuperUtils.IO;
using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Shapes;
using static SuperCom.Entity.HighLightRule;

namespace SuperCom.Windows
{
    /// <summary>
    /// Interaction logic for Window_Setting.xaml
    /// </summary>
    public partial class Window_Setting : BaseWindow
    {

        private MainWindow Main { get; set; }
        public VieModel_Setting vieModel { get; set; }
        public Window_Setting()
        {
            InitializeComponent();
            Init();
        }

        public void Init()
        {
            vieModel = new VieModel_Setting();
            DataContext = vieModel;
            dataGrid.ItemsSource = vieModel.RuleSets;
            foreach (Window item in App.Current.Windows)
            {
                if (item.Name.Equals("mainWindow"))
                {
                    Main = (MainWindow)item;
                    break;
                }
            }
        }


        private void BaseWindow_ContentRendered(object sender, EventArgs e)
        {
            // 设置语言
            int langIdx = 0;
            if (!string.IsNullOrEmpty(ConfigManager.Settings.CurrentLanguage))
            {
                for (int i = 0; i < langComboBox.Items.Count; i++)
                {
                    ComboBoxItem item = langComboBox.Items[i] as ComboBoxItem;
                    if (item.Tag.ToString().Equals(ConfigManager.Settings.CurrentLanguage))
                    {
                        langIdx = i;
                        break;
                    }
                }
            }
            langComboBox.SelectedIndex = langIdx;
            langComboBox.SelectionChanged += (s, ev) =>
            {
                if (ev.AddedItems?.Count > 0)
                {
                    ComboBoxItem comboBoxItem = ev.AddedItems[0] as ComboBoxItem;
                    string lang = comboBoxItem.Tag.ToString();
                    SuperControls.Style.LangManager.SetLang(lang);
                    SuperCom.Lang.LangManager.SetLang(lang);
                    vieModel.CurrentLanguage = lang;
                }
            };
        }


        public void AdjustWindow()
        {

            if (ConfigManager.Settings.FirstRun)
            {
                this.Width = SystemParameters.WorkArea.Width * 0.7;
                this.Height = SystemParameters.WorkArea.Height * 0.7;
                this.Left = SystemParameters.WorkArea.Width * 0.1;
                this.Top = SystemParameters.WorkArea.Height * 0.1;
                ConfigManager.Settings.FirstRun = false;
            }
            else
            {
                if (ConfigManager.Settings.Height == SystemParameters.WorkArea.Height && ConfigManager.Settings.Width < SystemParameters.WorkArea.Width)
                {
                    baseWindowState = 0;
                    this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                    this.CanResize = true;
                }
                else
                {
                    this.Left = ConfigManager.Settings.X;
                    this.Top = ConfigManager.Settings.Y;
                    this.Width = ConfigManager.Settings.Width;
                    this.Height = ConfigManager.Settings.Height;
                }


                baseWindowState = (BaseWindowState)ConfigManager.Settings.WindowState;
                if (baseWindowState == BaseWindowState.FullScreen)
                {
                    this.WindowState = System.Windows.WindowState.Maximized;
                }
                else if (baseWindowState == BaseWindowState.None)
                {
                    baseWindowState = 0;
                    this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                }
                if (this.Width == SystemParameters.WorkArea.Width
                    && this.Height == SystemParameters.WorkArea.Height) baseWindowState = BaseWindowState.Maximized;
            }
        }

        private void SaveConfigValue()
        {
            ConfigManager.Settings.X = this.Left;
            ConfigManager.Settings.Y = this.Top;
            ConfigManager.Settings.Width = this.Width;
            ConfigManager.Settings.Height = this.Height;
            ConfigManager.Settings.WindowState = (long)baseWindowState;
            ConfigManager.Settings.AutoBackup = vieModel.AutoBackup;
            ConfigManager.Settings.AutoBackupPeriodIndex = vieModel.AutoBackupPeriodIndex;
            ConfigManager.Settings.Save();
        }



        private void AddNewBaudRate(object sender, MouseButtonEventArgs e)
        {
            InputWindow inputWindow = new InputWindow(this);
            if ((bool)inputWindow.ShowDialog())
            {
                string text = inputWindow.Text;
                bool success = int.TryParse(text, out int baudRate);
                if (success && baudRate > 0 && !vieModel.BaudRates.Contains(baudRate.ToString()))
                {
                    vieModel.BaudRates.Add(baudRate.ToString());
                    ConfigManager.Main.CustomBaudRates = JsonUtils.TrySerializeObject(vieModel.BaudRates);
                    ConfigManager.Main.Save();
                    Main.vieModel.LoadBaudRates();
                }
            }
        }

        private bool IsPortRunning()
        {
            if (Main != null && Main.vieModel.SideComPorts?.Count > 0)
            {
                foreach (var item in Main.vieModel.SideComPorts)
                {
                    if (item.Connected) return true;
                }
            }
            return false;
        }

        private void DeleteBaudRate(object sender, RoutedEventArgs e)
        {
            if (IsPortRunning())
            {
                MessageCard.Error("请关闭所有串口后再试");
                return;
            }
            if (vieModel.BaudRates?.Count <= 1)
            {
                MessageCard.Error("至少保留一个");
                return;
            }
            Button button = sender as Button;
            if (button != null && button.Tag != null)
            {
                string value = button.Tag.ToString();
                int idx = -1;
                for (int i = 0; i < vieModel.BaudRates.Count; i++)
                {
                    if (value.Equals(vieModel.BaudRates[i].ToString()))
                    {
                        idx = i;
                        break;
                    }
                }
                if (idx >= 0 && idx < vieModel.BaudRates.Count)
                {
                    vieModel.BaudRates.RemoveAt(idx);
                    ConfigManager.Main.CustomBaudRates = JsonUtils.TrySerializeObject(vieModel.BaudRates);
                    ConfigManager.Main.Save();
                    Main.vieModel.LoadBaudRates();
                }
            }
        }

        private void SaveSettings(object sender, RoutedEventArgs e)
        {
            ConfigManager.CommonSettings.FixedOnSearch = vieModel.FixedOnSearch;
            ConfigManager.CommonSettings.FixedOnSendCommand = vieModel.FixedOnSendCommand;
            ConfigManager.CommonSettings.ScrollOnSearchClosed = vieModel.ScrollOnSearchClosed;
            ConfigManager.CommonSettings.LogNameFormat = vieModel.LogNameFormat;
            ConfigManager.CommonSettings.Save();


            ConfigManager.Settings.CurrentLanguage = vieModel.CurrentLanguage;
            ConfigManager.Settings.Save();

            vieModel.SaveAllRule();
            MessageCard.Success("保存成功");
            ApplyRule();
            Main?.ReadXshdList();
        }

        private void ApplyRule()
        {
            //string xshdPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AvalonEdit", "Higlighting", "Default.xshd");
            HighLightRule rule = vieModel.HighLightRules.Where(arg => arg.RuleID == vieModel.CurrentRuleID).FirstOrDefault();
            if (rule == null) return;
            string xshdPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                "AvalonEdit", "Higlighting", rule.FileName);
            if (!File.Exists(xshdPath)) return;

            using (Stream s = File.OpenRead(xshdPath))
            {
                using (System.Xml.XmlTextReader reader = new System.Xml.XmlTextReader(s))
                {
                    previewTextEditor.SyntaxHighlighting = ICSharpCode.AvalonEdit.Highlighting.Xshd.HighlightingLoader.Load
                        (reader, ICSharpCode.AvalonEdit.Highlighting.HighlightingManager.Instance);
                }
            }
        }

        private void RestoreSettings(object sender, RoutedEventArgs e)
        {
            if (IsPortRunning())
            {
                MessageCard.Error("请关闭所有串口后再试");
                return;
            }

            if (new MsgBox(this, "将删除所有自定义串口设置，是否继续？").ShowDialog() == false)
            {
                return;
            }

            vieModel.BaudRates = new System.Collections.ObjectModel.ObservableCollection<string>();
            foreach (var item in PortSetting.DEFAULT_BAUDRATES)
            {
                vieModel.BaudRates.Add(item.ToString());
            }
            ConfigManager.Main.CustomBaudRates = JsonUtils.TrySerializeObject(vieModel.BaudRates);
            ConfigManager.Main.Save();
            Main.vieModel.LoadBaudRates();

            vieModel.LogNameFormat = CommonSettings.DEFAULT_LOGNAMEFORMAT;
            vieModel.FixedOnSearch = true;
            vieModel.FixedOnSendCommand = false;
            vieModel.ScrollOnSearchClosed = true;
            MessageCard.Success("已恢复默认值");
        }


        private void searchBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            List<char> list = System.IO.Path.GetInvalidFileNameChars().ToList();
            foreach (var item in e.Text.ToCharArray())
            {
                if (list.Contains(item))
                {
                    MessageCard.Error("非法文件名：\\ / : * ? \" < > | ");
                    e.Handled = true;
                    break;
                }
            }
        }

        private void NewRule(object sender, RoutedEventArgs e)
        {
            vieModel.NewRule();
        }

        public void ShowRuleSetByRule(HighLightRule rule)
        {
            if (rule != null)
            {
                vieModel.RuleSets = new System.Collections.ObjectModel.ObservableCollection<RuleSet>();
                List<RuleSet> ruleSets = new List<RuleSet>();
                if (!string.IsNullOrEmpty(rule.RuleSetString))
                    ruleSets = JsonUtils.TryDeserializeObject<List<RuleSet>>(rule.RuleSetString);
                if (ruleSets.Count > 0)
                {
                    foreach (var item in ruleSets)
                    {
                        vieModel.RuleSets.Add(item);
                    }

                }
                vieModel.CurrentRuleID = rule.RuleID;
                vieModel.ShowCurrentRule = true;
                dataGrid.ItemsSource = null;
                dataGrid.ItemsSource = vieModel.RuleSets;
            }
        }

        private void RuleListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems == null || e.AddedItems.Count == 0)
                return;
            HighLightRule rule = e.AddedItems[0] as HighLightRule;
            ShowRuleSetByRule(rule);
            vieModel.CurrentRuleID = rule.RuleID;
            previewTextEditor.Text = rule.PreviewText;
            ApplyRule();
        }

        private void RenameRule(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = sender as MenuItem;
            ContextMenu contextMenu = menuItem.Parent as ContextMenu;
            Grid grid = contextMenu.PlacementTarget as Grid;
            TextBox textBox = grid.Children.OfType<TextBox>().FirstOrDefault();
            if (textBox != null)
            {
                textBox.Visibility = Visibility.Visible;
                textBox.SelectAll();
                textBox.Focus();
            }


        }

        private void DeleteRule(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            if (button != null && button.Tag != null)
            {
                long.TryParse(button.Tag.ToString(), out long id);
                if (!vieModel.DeleteRule(id))
                    MessageCard.Error("删除失败");
                else
                {
                    Main?.ReadXshdList();
                }

            }
        }

        private void RenameTextBoxLostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            textBox.Visibility = Visibility.Hidden;
            string newName = textBox.Text;
            if (textBox.Tag != null)
            {
                string ruleID = textBox.Tag.ToString();
                if (!string.IsNullOrEmpty(ruleID))
                {
                    HighLightRule rule = vieModel.HighLightRules.Where(arg => arg.RuleID.ToString().Equals(ruleID)).FirstOrDefault();
                    string oldName = rule.RuleName;
                    if (string.IsNullOrEmpty(newName))
                    {
                        textBox.Text = oldName;
                        return;
                    }

                    if (rule != null && !oldName.Equals(newName))
                    {
                        string oldFileName = rule.GetFullFileName();
                        rule.RuleName = newName;
                        vieModel.RenameRule(rule);
                        TextBlock textBlock = (textBox.Parent as Grid).Children.OfType<TextBlock>().FirstOrDefault();
                        textBlock.Text = newName;

                        // 重命名文件
                        string newFileName = rule.GetFullFileName();
                        if (File.Exists(oldFileName) && !File.Exists(newFileName))
                            FileHelper.TryMoveFile(oldFileName, newFileName);
                        Main?.ReadXshdList();
                    }
                }
            }
        }

        private void TextBox_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Escape)
            {
                RenameTextBoxLostFocus(sender, e);
            }
        }

        private void AddNewRuleItem(object sender, RoutedEventArgs e)
        {
            vieModel.NewRuleSet();
        }

        private void DeleteRuleSet(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            if (button != null && button.Tag != null)
            {
                string ruleSetID = button.Tag.ToString();
                RuleSet ruleSet = vieModel.RuleSets.Where(arg => arg.RuleSetID.ToString().Equals(ruleSetID)).FirstOrDefault();
                if (ruleSet != null)
                {
                    vieModel.RuleSets.Remove(ruleSet);
                    HighLightRule rule = vieModel.HighLightRules.Where(arg => arg.RuleID == vieModel.CurrentRuleID).FirstOrDefault();
                    if (rule != null)
                    {
                        if (!string.IsNullOrEmpty(rule.RuleSetString))
                        {
                            rule.RuleSetList = JsonUtils.TryDeserializeObject<List<RuleSet>>(rule.RuleSetString);
                            rule.RuleSetList.RemoveAll(arg => arg.RuleSetID.ToString().Equals(ruleSetID));
                            rule.RuleSetString = JsonUtils.TrySerializeObject(rule.RuleSetList);
                            vieModel.UpdateRule(rule);
                        }
                    }
                }
            }
        }

        private void SaveRuleSet(object sender, RoutedEventArgs e)
        {
            string ruleSets = JsonUtils.TrySerializeObject(vieModel.RuleSets.ToList());
            HighLightRule rule = vieModel.HighLightRules.Where(arg => arg.RuleID == vieModel.CurrentRuleID).FirstOrDefault();
            if (rule != null && !string.IsNullOrEmpty(ruleSets))
            {
                if (!ruleSets.Equals(rule.RuleSetString))
                {
                    rule.RuleSetString = ruleSets;
                    vieModel.UpdateRule(rule);
                }
            }
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems == null || e.AddedItems.Count == 0) return;
            SaveRuleSet(null, null);
        }


        private long CurrentRuleSetID;
        private Border CurrentBorder;

        private void ShowColorPicker(object sender, MouseButtonEventArgs e)
        {
            Border border = sender as Border;
            long.TryParse(border.Tag.ToString(), out CurrentRuleSetID);
            CurrentBorder = border;
            SolidColorBrush solidColorBrush = border.Background as SolidColorBrush;
            Color color = solidColorBrush.Color;
            colorPickerPopup.IsOpen = true;
            colorPicker.SetCurrentColor(color);
        }

        private void CancelColorPicker(object sender, RoutedEventArgs e)
        {
            colorPickerPopup.IsOpen = false;
        }

        private void ConfirmColorPicker(object sender, RoutedEventArgs e)
        {
            colorPickerPopup.IsOpen = false;
            Color color = colorPicker.SelectedColor;
            vieModel.SetRuleSetColor(color, CurrentRuleSetID);
            SaveRuleSet(null, null);
            CurrentBorder.Background = new SolidColorBrush(color);
        }

        private void previewTextEditor_TextChanged(object sender, EventArgs e)
        {
            HighLightRule highLightRule = vieModel.HighLightRules.Where(arg => arg.RuleID == vieModel.CurrentRuleID).FirstOrDefault();
            if (highLightRule != null)
            {
                highLightRule.PreviewText = (sender as TextEditor).Text;
            }
        }

        private void BaseWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            SaveConfigValue();
        }

        private void BaseWindow_Loaded(object sender, RoutedEventArgs e)
        {
            AdjustWindow();
        }
    }
}
