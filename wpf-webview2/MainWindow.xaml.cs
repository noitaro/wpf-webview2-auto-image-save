using Microsoft.Web.WebView2.Core;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;

namespace wpf_webview2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            InitializeAsync();
        }

        async void InitializeAsync()
        {
            await webView.EnsureCoreWebView2Async(null);
            webView.CoreWebView2.SourceChanged += CoreWebView2_SourceChanged;
            webView.CoreWebView2.WebResourceRequested += CoreWebView2_WebResourceRequested;
            webView.CoreWebView2.WebResourceResponseReceived += CoreWebView2_WebResourceResponseReceived;
        }

        private void CoreWebView2_SourceChanged(object sender, CoreWebView2SourceChangedEventArgs e)
        {
            Debug.WriteLine($"SourceChanged: {webView.CoreWebView2.Source}");
            textBox.Text = webView.CoreWebView2.Source;
        }

        private void CoreWebView2_WebResourceRequested(object sender, CoreWebView2WebResourceRequestedEventArgs e)
        {
            Debug.WriteLine($"WebResourceRequested: {e.Request.Uri}");
        }

        private async void CoreWebView2_WebResourceResponseReceived(object sender, CoreWebView2WebResourceResponseReceivedEventArgs e)
        {
            // URL の末尾が jpg か png のみ、画像を保存する。
            if (e.Request.Uri.EndsWith("jpg") || e.Request.Uri.EndsWith("png"))
            {
                Debug.WriteLine($"WebResourceResponseReceived: {e.Request.Uri}");

                // フォルダが無ければ作成する。
                if (!File.Exists("img")) Directory.CreateDirectory("img");

                var uri = new Uri(e.Request.Uri);

                // 非同期でレスポンス画像を取得する。
                using (var stream = await e.Response.GetContentAsync())
                {
                    using (var fileStream = new FileStream($"img/{uri.Segments.Last()}", FileMode.Create, FileAccess.Write))
                    {
                        // ストリームをファイルに保存する。
                        stream.CopyTo(fileStream);
                    };
                };
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // IDを自動入力する.
            webView.CoreWebView2.ExecuteScriptAsync($"document.querySelector('#LoginComponent > form > div.input-field-group > div:nth-child(1) > input[type=text]').value = '★★★ ID ★★★'");
            // パスワードを自動入力する.
            webView.CoreWebView2.ExecuteScriptAsync($"document.querySelector('#LoginComponent > form > div.input-field-group > div:nth-child(2) > input[type=password]').value = '★★★ パスワード ★★★'");
            // ログインをする.
            webView.CoreWebView2.ExecuteScriptAsync($"document.querySelector('#LoginComponent > form > button').click()");
        }
    }
}
