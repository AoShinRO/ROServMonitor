using Microsoft.Win32;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace AoShinhoServ_Monitor.Forms
{
    /// <summary>
    /// Lógica interna para OptionsWnd.xaml
    /// </summary>
    public partial class OptionsWnd : Window
    {
        public OptionsWnd()
        {
            InitializeComponent();
        }

        #region Btn_Related

        public string OpenPathDialogBox(ROServers.Type type)
        {
            
            if (type == ROServers.Type.ROBrowser)
            {
                using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
                {
                    var folderResult = dialog.ShowDialog();

                    if (folderResult == System.Windows.Forms.DialogResult.OK)
                    {
                        return dialog.SelectedPath;
                    }
                }
                return Configuration.RobPath;
            }

            OpenFileDialog box;
            switch (type)
            {
                case ROServers.Type.Login:
                    box = new OpenFileDialog { Filter = @"login-server (*.exe)|*.exe|All files (*.*)|*.*" };
                    break;

                case ROServers.Type.Char:
                    box = new OpenFileDialog { Filter = @"char-server (*.exe)|*.exe|All files (*.*)|*.*" };
                    break;

                case ROServers.Type.Web:
                    box = new OpenFileDialog { Filter = @"web-server (*.exe)|*.exe|All files (*.*)|*.*" };
                    break;

                default:
                    box = new OpenFileDialog { Filter = @"map-server (*.exe)|*.exe|All files (*.*)|*.*" };
                    break;
            }
            bool? result = box.ShowDialog();

            if (result.HasValue && result.Value)
                return box.FileName;

            switch (type)
            {
                case ROServers.Type.Login: return Configuration.LoginPath;
                case ROServers.Type.Char: return Configuration.CharPath;
                case ROServers.Type.Web: return Configuration.WebPath;
                default: return Configuration.MapPath;
            }
        }

        private void Okaylbl_MouseDown(object sender, MouseButtonEventArgs e)
        {
        }

        private void Cancellbl_MouseDown(object sender, MouseButtonEventArgs e)
        {
        }
        private void WhiteMode_Checked(object sender, RoutedEventArgs e)
        {           
        }

        private void WhiteMode_Unchecked(object sender, RoutedEventArgs e)
        {
        }

        private void MapExePath_Click(object sender, RoutedEventArgs e) => SaveServerFilePath(MapPath, ROServers.Type.Map);

        private void WebExePath_Click(object sender, RoutedEventArgs e) => SaveServerFilePath(WebPath, ROServers.Type.Web);

        private void LoginExePath_Click(object sender, RoutedEventArgs e) => SaveServerFilePath(LoginPath, ROServers.Type.Login);

        private void CharExePath_Click(object sender, RoutedEventArgs e) => SaveServerFilePath(CharPath, ROServers.Type.Char);

        private void ROBExePath_Click(object sender, RoutedEventArgs e) => SaveServerFilePath(ROBPath, ROServers.Type.ROBrowser);

        private void SaveServerFilePath(TextBox Box,ROServers.Type Types)
        {
            Box.Text = OpenPathDialogBox(Types);
            switch (Types)
            {
                case ROServers.Type.Login:
                    Configuration.LoginPath = Box.Text;
                    break;

                case ROServers.Type.Char:
                    Configuration.CharPath = Box.Text;
                    break;

                case ROServers.Type.Web:
                    Configuration.WebPath = Box.Text;
                    break;
                case ROServers.Type.ROBrowser:
                    Configuration.RobPath = Box.Text;
                    break;
                default:
                    Configuration.MapPath = Box.Text;
                    break;
                    
            }
        }


        #endregion Btn_Related

        private void DevMode_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void DevMode_Unchecked(object sender, RoutedEventArgs e)
        {

        }

        private void CmakeMode_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void CmakeMode_Unchecked(object sender, RoutedEventArgs e)
        {

        }

        private void PreRenewalMode_Unchecked(object sender, RoutedEventArgs e)
        {

        }

        private void PreRenewalMode_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void ROBrowser_Checked(object sender, RoutedEventArgs e)
        {
        }

        private void ROBrowser_Unchecked(object sender, RoutedEventArgs e)
        {
        }

        private void WSproxy_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !e.Text.All(char.IsDigit);
        }

        private void FontSizeUp_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(FontSizeBox.Text, out int size))
            {
                size++;
                FontSizeBox.Text = size.ToString();
            }
            else
            {
                FontSizeBox.Text = "12";
            }
        }

        private void FontSizeDown_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(FontSizeBox.Text, out int size))
            {
                if (size > 1)
                    size--;

                FontSizeBox.Text = size.ToString();
            }
            else
            {
                FontSizeBox.Text = "12";
            }
        }

        private void FontSelector_Loaded(object sender, RoutedEventArgs e)
        {
            ILogging.OptWin.FontSelector.ItemsSource = Fonts.SystemFontFamilies
                .Select(f => f.Source)
                .OrderBy(name => name)
                .ToList();

        }
    }
}