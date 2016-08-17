using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
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

namespace Gemini.Demo.Modules.WebPage.Controls
{
    /// <summary>
    /// Interaction logic for InternetExplorer.xaml
    /// </summary>
    public partial class InternetExplorer : UserControl
    {
        private SHDocVw.IWebBrowser2 axBrowser;

        private const string _blankUri = "about:blank";
                
        private double _progress;
        public double ProgressInPercentage
        {
            get { return _progress; }
            set
            {
                _progress = value;
            }
        }

        private static bool EventHandled { get; set; }

        public InternetExplorer()
        {
            InitializeComponent();

            Browser.Navigating += Browser_Navigating;
            Browser.Navigated += Browser_Navigated;
            Browser.LoadCompleted += Browser_LoadCompleted;

            // To fire ProgressChange event, we have to get IWebBrowser2 interface
            // It needs to add Microsoft.Internet.Controls to References
            // http://stackoverflow.com/questions/2063855/wpf-webbrowser-how-i-do-access-progress-and-new-window-events
            axBrowser = typeof(WebBrowser).GetProperty("AxIWebBrowser2", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(Browser, null) as SHDocVw.IWebBrowser2;
            ((SHDocVw.DWebBrowserEvents2_Event)axBrowser).ProgressChange += Browser_ProgressChange;
            ((SHDocVw.DWebBrowserEvents2_Event)axBrowser).TitleChange += Browser_TitleChange;

            
            // Something strange: after once load a URI, a local file can be loaded. Otherwise, download starts.
            Browser.Navigate(_blankUri);
        }

        #region WebBrowser Events

        private void Browser_TitleChange(string Text)
        {
            Title = Text;
        }

        private void Browser_LoadCompleted(object sender, NavigationEventArgs e)
        {
            WebBrowser wb = sender as WebBrowser;

            Source = e.Uri;
            urlTextBox.Text = e.Uri.ToString();            

            Thread.Sleep(1000);

            if (e.Uri.IsFile)
            {
                Title = System.IO.Path.GetFileName(e.Uri.LocalPath);
            }

            IsLoading = false;
        }

        private void Browser_Navigated(object sender, NavigationEventArgs e)
        { }

        private void Browser_Navigating(object sender, NavigatingCancelEventArgs e)
        {
            WebBrowser wb = sender as WebBrowser;
            SuppressScriptErrors(wb, true);

            IsLoading = true;
            CurrentProgress = 0;
        }

        private void Browser_ProgressChange(int Progress, int ProgressMax)
        {
            CurrentProgress = (ProgressMax == 0) ? 0.0 : 100.0 * (double)Progress / (double)ProgressMax;
        }

        // Source: https://social.msdn.microsoft.com/Forums/vstudio/en-US/4f686de1-8884-4a8d-8ec5-ae4eff8ce6db/new-wpf-webbrowser-how-do-i-suppress-script-errors?forum=wpf
        private void SuppressScriptErrors(WebBrowser wb, bool Hide)
        {
            FieldInfo fi = typeof(WebBrowser).GetField("_axIWebBrowser2", BindingFlags.Instance | BindingFlags.NonPublic);
            if (fi != null)
            {
                object browser = fi.GetValue(wb);
                if (browser != null)
                {
                    browser.GetType().InvokeMember("Silent", BindingFlags.SetProperty, null, browser, new object[] { Hide });
                }
            }
        }

        #endregion

        #region Toolbar Behaviors

        private void BrowseBack_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ((Browser != null) && (Browser.CanGoBack));
        }

        private void BrowseBack_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Browser.GoBack();
        }

        private void BrowseForward_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ((Browser != null) && (Browser.CanGoForward));
        }

        private void Browse_CanRefresh(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (Browser != null);
        }

        private void Browse_Refreshed(object sender, ExecutedRoutedEventArgs e)
        {
            Browser.Refresh();
        }

        private void BrowseForward_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Browser.GoForward();
        }

        #endregion

        #region urlTextBox Events

        // Source: https://social.msdn.microsoft.com/Forums/vstudio/en-US/564b5731-af8a-49bf-b297-6d179615819f/how-to-selectall-in-textbox-when-textbox-gets-focus-by-mouse-click?forum=wpf

        private void urlTextBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            TextBox tb = sender as TextBox;
            if (tb != null)
            {
                tb.SelectAll();
            }
        }

        private void urlTextBox_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            if (tb != null)
            {
                tb.SelectAll();
            }
        }

        private void urlTextBox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            TextBox tb = sender as TextBox;
            if (tb != null)
            {
                if (!tb.IsKeyboardFocusWithin)
                {
                    e.Handled = true;
                    tb.Focus();
                }
            }
        }

        private void urlTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Target = urlTextBox.Text;
            }
        }

        #endregion

        #region ProgressBar Events

        // Source: http://stackoverflow.com/questions/4734814/progressbar-foreground-color
        private void ProgressBar_Loaded(object sender, RoutedEventArgs e)
        {
            var progressBar = sender as ProgressBar;
            if (progressBar == null) return;

            var animation = progressBar.Template.FindName("Animation", progressBar) as FrameworkElement;
            if (animation != null)
                animation.Visibility = Visibility.Collapsed;
        }

        #endregion

        #region Target DependencyProperty

        public static readonly DependencyProperty TargetProperty = DependencyProperty.Register(
            "Target",
            typeof(string),
            typeof(InternetExplorer),
            new FrameworkPropertyMetadata(default(string))
            {
                BindsTwoWayByDefault = true,
                DefaultUpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                PropertyChangedCallback = UriPropertyChanged
            });

        public string Target
        {
            get { return (string)GetValue(TargetProperty); }
            set { SetValue(TargetProperty, value); }
        }

        private static void UriPropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs args)
        {
            var ie = (InternetExplorer)o;

            if (ie.IsLoading)
                return;

            if (EventHandled)
            {
                EventHandled = false;
                return;
            }

            var uriStr = args.NewValue as string;
            var result = false;
            if (uriStr == null || string.Equals(uriStr, _blankUri, StringComparison.OrdinalIgnoreCase))
            {
                ie.Browser.Navigate(_blankUri);
            }
            else if (uriStr.StartsWith("content://"))
            {
                LoadStringContent(ie, (string)args.NewValue, out result);
            }
            else if (uriStr.StartsWith("file://"))
            {
                ie.Browser.Navigate(new UriBuilder(uriStr).Uri);
                //LoadFileContent(ie, (string)args.NewValue, out result);
            }
            else if (uriStr.StartsWith("http://") || uriStr.StartsWith("https://"))
            {
                ie.Browser.Navigate(new UriBuilder(uriStr).Uri);
            }
            else
            {
                Uri UriToNavigate = null;

                try
                {
                    UriToNavigate = new UriBuilder(uriStr).Uri;
                    bool isValidUri = UriToNavigate.IsWellFormedOriginalString();

                    if (isValidUri)
                    {
                        Uri uriResult;
                        isValidUri = Uri.TryCreate(UriToNavigate.ToString(), UriKind.Absolute, out uriResult)
                                     && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps)
                                     && uriResult.Host.Replace("www.", "").Split('.').Count() > 1
                                     && uriResult.HostNameType == UriHostNameType.Dns
                                     && uriResult.Host.Length > uriResult.Host.LastIndexOf(".") + 1
                                     && uriResult.ToString().Length <= 255;
                    }

                    if (!isValidUri)
                        UriToNavigate = new Uri(string.Format("http://www.google.com/search?q={0}", uriStr));

                    ie.Browser.Navigate(UriToNavigate);
                    result = true;
                }
                catch
                { }
            }
        }

        private static void LoadFileContent(InternetExplorer ie, string path, out bool result)
        {
            try
            {
                var content = File.ReadAllText(path);
                ie.Browser.NavigateToString(content);
                result = true;
            }
            catch (Exception)
            {
                result = false;
            }
        }

        private static void LoadStringContent(InternetExplorer ie, string target, out bool result)
        {
            ie.Browser.NavigateToString(target.Substring(10));
            result = true;
        }

        #endregion

        #region Title DependencyProperty

        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
            "Title",
            typeof(string),
            typeof(InternetExplorer),
            new FrameworkPropertyMetadata(default(string))
            {
                BindsTwoWayByDefault = true,
                DefaultUpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            });

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        #endregion

        #region Source DependencyProperty

        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register(
            "Source",
            typeof(Uri),
            typeof(InternetExplorer),
            new FrameworkPropertyMetadata(default(string))
            {
                BindsTwoWayByDefault = true,
                DefaultUpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            });

        public Uri Source
        {
            get { return (Uri)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        #endregion

        #region IsLoading DependencyProperty

        public static readonly DependencyProperty IsLoadingProperty = DependencyProperty.Register(
            "IsLoading",
            typeof(bool),
            typeof(InternetExplorer),
            new FrameworkPropertyMetadata(default(bool))
            {
                BindsTwoWayByDefault = true,
                DefaultUpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            });

        public bool IsLoading
        {
            get { return (bool)GetValue(IsLoadingProperty); }
            set { SetValue(IsLoadingProperty, value); }
        }

        #endregion

        #region CurrentProgress DependencyProperty

        public static readonly DependencyProperty ProgressProperty = DependencyProperty.Register(
            "CurrentProgress",
            typeof(double),
            typeof(InternetExplorer),
            new FrameworkPropertyMetadata(default(double))
            {
                BindsTwoWayByDefault = true,
                DefaultUpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            });

        public double CurrentProgress
        {
            get { return (double)GetValue(ProgressProperty); }
            set { SetValue(ProgressProperty, value); }
        }

        #endregion
    }
}
