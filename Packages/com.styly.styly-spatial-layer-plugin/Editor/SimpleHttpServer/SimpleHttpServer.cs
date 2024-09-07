#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEditor;
using UnityEngine;

public class SimpleHttpServer
{
    private readonly HttpListener httpListener = new HttpListener();
    private bool serverRunning = false;

    public int Port { get; set; }
    public string DocumentRoot { get; set; }
    
    // POSTリクエストのテキストデータ処理用のデリゲート
    public Action<string> OnReceiveText { get; set; }

    // POSTリクエストのバイナリデータ処理用のデリゲート
    public Action<byte[]> OnReceiveBinary { get; set; }
    
    public void StartServer()
    {
        if (serverRunning) return;

        httpListener.Prefixes.Add($"http://*:{Port}/");
        httpListener.Start();
        serverRunning = true;

        Debug.Log("Server started.");

        RunServer().Forget();
    }

    public void StopServer()
    {
        if (!serverRunning) return;

        httpListener.Stop();
        serverRunning = false;

        Debug.Log("Server stopped.");
    }

    private async UniTaskVoid RunServer()
    {
        while (serverRunning)
        {
            var context = await httpListener.GetContextAsync();
            Debug.Log($"{context.Request.HttpMethod} Request path: {context.Request.RawUrl}");

            // リクエストの処理をここで行う
            await HandleRequest(context);
        }
    }
    protected async Task HandleRequest(HttpListenerContext context)
    {
        // HTTPメソッドの取得
        string httpMethod = context.Request.HttpMethod;

        if (httpMethod == "GET")
        {
            await HandleGetRequest(context);
        }
        else if (httpMethod == "POST")
        {
            await HandlePostRequest(context);
        }
        else
        {
            // 他のメソッドは405 Method Not Allowedを返す
            context.Response.StatusCode = 405;
            byte[] buffer = Encoding.UTF8.GetBytes("405 Method Not Allowed");
            context.Response.ContentType = "text/plain";
            context.Response.ContentLength64 = buffer.Length;
            await context.Response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
            context.Response.Close();
        }
    }

    // GETリクエストの処理
    private async Task HandleGetRequest(HttpListenerContext context)
    {
        // リクエストパスを取得
        string requestPath = context.Request.RawUrl.TrimStart('/');
    
        // ルートパスへのアクセスの場合に index.html を返す
        if (string.IsNullOrEmpty(requestPath) || requestPath == "/")
        {
            requestPath = "index.html";
        }

        // リクエストされたファイルのフルパスを生成
        string filePath = Path.Combine(DocumentRoot, requestPath);
        Debug.Log($"file path: {filePath}");
    
        if (File.Exists(filePath))
        {
            context.Response.StatusCode = 200;

            // MIMEタイプを決定する
            string mimeType = GetMimeType(filePath);
            context.Response.ContentType = mimeType;

            byte[] buffer = await File.ReadAllBytesAsync(filePath);
            context.Response.ContentLength64 = buffer.Length;
            await context.Response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
        }
        else
        {
            // 404エラーメッセージの設定
            context.Response.StatusCode = 404;
            byte[] buffer = Encoding.UTF8.GetBytes("404 Not Found");
            context.Response.ContentType = "text/plain";
            context.Response.ContentLength64 = buffer.Length;
            await context.Response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
        }

        context.Response.Close();
    }

    private async Task HandlePostRequest(HttpListenerContext context)
    {
        try
        {
            // コンテンツタイプを取得
            string contentType = context.Request.ContentType;

            if (contentType != null && (contentType.StartsWith("text/") || contentType == "application/json"))
            {
                // テキストデータとして処理
                using (var reader = new StreamReader(context.Request.InputStream, context.Request.ContentEncoding))
                {
                    string textData = await reader.ReadToEndAsync();
                    Debug.Log($"Received text data: {textData}");
                    
                    // テキストデータの処理をデリゲートに委任
                    OnReceiveText?.Invoke(textData);
                }

                // 成功レスポンスを返す
                context.Response.StatusCode = 200;
                byte[] buffer = Encoding.UTF8.GetBytes("POST request with text data received and processed successfully.");
                context.Response.ContentType = "text/plain";
                context.Response.ContentLength64 = buffer.Length;
                await context.Response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
            }
            else
            {
                // バイナリデータとして処理
                long contentLength = context.Request.ContentLength64;
                if (contentLength > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await context.Request.InputStream.CopyToAsync(memoryStream);
                        byte[] binaryData = memoryStream.ToArray();

                        Debug.Log($"Received binary data of length: {binaryData.Length}");

                        // バイナリデータの処理をデリゲートに委任
                        OnReceiveBinary?.Invoke(binaryData);
                    }

                    // 成功レスポンスを返す
                    context.Response.StatusCode = 200;
                    byte[] buffer = Encoding.UTF8.GetBytes("POST request with binary data received and processed successfully.");
                    context.Response.ContentType = "text/plain";
                    context.Response.ContentLength64 = buffer.Length;
                    await context.Response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
                }
                else
                {
                    // コンテンツがない場合のエラーレスポンス
                    context.Response.StatusCode = 400; // Bad Request
                    byte[] buffer = Encoding.UTF8.GetBytes("No data received in POST request.");
                    context.Response.ContentType = "text/plain";
                    context.Response.ContentLength64 = buffer.Length;
                    await context.Response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
                }
            }
        }
        catch (Exception ex)
        {
            // エラーレスポンスを返す
            Debug.LogError($"Error processing POST request: {ex.Message}");
            context.Response.StatusCode = 500; // Internal Server Error
            byte[] buffer = Encoding.UTF8.GetBytes("500 Internal Server Error");
            context.Response.ContentType = "text/plain";
            context.Response.ContentLength64 = buffer.Length;
            await context.Response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
        }

        context.Response.Close();
    }
    
    // MIMEタイプを決定するヘルパーメソッド
    private string GetMimeType(string filePath)
    {
        string extension = Path.GetExtension(filePath).ToLowerInvariant();

        switch (extension)
        {
            case ".html":
            case ".htm":
                return "text/html";
            case ".css":
                return "text/css";
            case ".js":
                return "application/javascript";
            case ".json":
                return "application/json";
            case ".png":
                return "image/png";
            case ".jpg":
            case ".jpeg":
                return "image/jpeg";
            case ".gif":
                return "image/gif";
            case ".svg":
                return "image/svg+xml";
            case ".ico":
                return "image/x-icon";
            case ".xml":
                return "application/xml";
            case ".pdf":
                return "application/pdf";
            case ".zip":
                return "application/zip";
            case ".txt":
                return "text/plain";
            // その他の拡張子についても必要に応じて追加
            default:
                return "application/octet-stream"; // デフォルトはバイナリデータ
        }
    }
    public static string GetHostName()
    {
        string hostname = Dns.GetHostName();
        IPAddress[] adrList = Dns.GetHostAddresses(hostname);

        var usableIps = adrList
            .Where(ip => ip.AddressFamily == AddressFamily.InterNetwork)
            .Where(ip => !ip.Equals(IPAddress.Parse("127.0.0.1")));

        var ipAddresses = usableIps as IPAddress[] ?? usableIps.ToArray();
        foreach (var ip in ipAddresses)
        {
            Debug.Log(ip);
        }

        return ipAddresses.FirstOrDefault().ToString();
    }
}

public class UnityHttpServerManager
{
    private readonly SimpleHttpServer server;
    private FileSystemWatcher fileWatcher;
    private readonly string assetBundleDir = "_Output/html";
    private static readonly string VisionOsDirectoryName = "VisionOS";
    private static readonly string ThumbnailDirName = "Thumbnails";
    private static readonly string HtmlFilePath = Path.Combine("_Output", "html", "index.html");

    public int Port => server. Port;

    public string DocumentRoot => server.DocumentRoot;

    public UnityHttpServerManager()
    {
        server = new SimpleHttpServer();
        server.Port = 8181;
        server.DocumentRoot = Path.Combine(Application.dataPath, "..",assetBundleDir);
    }

    public void StartServer()
    {
        // サーバー起動時にHTMLファイルを静的に生成
        GenerateStaticHtmlFile();

        // ファイルシステムの監視を開始
        StartFileWatcher();

        server.StartServer();
    }

    public void StopServer()
    {
        server.StopServer();
        if (fileWatcher != null)
        {
            fileWatcher.EnableRaisingEvents = false;
            fileWatcher.Dispose();
        }
    }

    private void GenerateStaticHtmlFile()
    {
        var filenames = GetFilenamesSortedByTime();
        var htmlContent = GenerateHtmlTable(filenames);

        Directory.CreateDirectory(Path.GetDirectoryName(HtmlFilePath));
        File.WriteAllText(HtmlFilePath, htmlContent);

        Debug.Log($"Static HTML generated at: {HtmlFilePath}");
    }

    private void StartFileWatcher()
    {
        fileWatcher = new FileSystemWatcher
        {
            Path = Path.Combine(assetBundleDir, VisionOsDirectoryName),
            NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite,
            Filter = "*.*"
        };

        fileWatcher.Changed += OnFilesChanged;
        fileWatcher.Created += OnFilesChanged;
        fileWatcher.Deleted += OnFilesChanged;
        fileWatcher.Renamed += OnFilesChanged;
        fileWatcher.EnableRaisingEvents = true;

        Debug.Log("Started watching file changes.");
    }

    private void OnFilesChanged(object sender, FileSystemEventArgs e)
    {
        Debug.Log($"File system changed: {e.ChangeType} - {e.FullPath}");
        GenerateStaticHtmlFile();
    }

    private List<string> GetFilenamesSortedByTime()
    {
        string dirPath = Path.Combine(assetBundleDir, VisionOsDirectoryName);
        if (!Directory.Exists(dirPath))
        {
            Debug.LogError($"Directory not found: {dirPath}");
            Directory.CreateDirectory(dirPath);
            return new List<string>();
        }

        var files = Directory.GetFiles(dirPath);
        if (files.Length == 0)
        {
            return new List<string>();
        }

        var validFiles = files.Where(f => (Path.GetFileName(f) != "VOS" && !System.Text.RegularExpressions.Regex.IsMatch(f, ".manifest")));
        var sortedFiles = validFiles.OrderByDescending(File.GetCreationTime).ToList();

        Debug.Log(string.Join(",", sortedFiles.Select(Path.GetFileName).ToList()));
        return sortedFiles.Select(Path.GetFileName).ToList();
    }

    private string GenerateHtmlTable(List<string> filenames)
    {
        var sb = new StringBuilder();
        sb.AppendLine("<html>");
        sb.AppendLine("<head><title>Contents List</title></head>");
        sb.AppendLine("<body>");
        sb.AppendLine("<h1>Contents List</h1>");
        if (filenames.Count == 0)
        {
            sb.AppendLine("<p>No contents found.</p>");
        }
        else
        {
            sb.AppendLine("<table border='1'>");

            foreach (var filename in filenames)
            {
                var assetUrl = $"http://{SimpleHttpServer.GetHostName()}:{server.Port}/{VisionOsDirectoryName}/{filename}";
                var thumbnailUrl = $"{ThumbnailDirName}/{filename}.png";

                sb.AppendLine("<tr>");
                sb.AppendLine($"<td><img src='{thumbnailUrl}' alt='Thumbnail' width='100'></td>");
                sb.AppendLine($"<td>{filename}</td>");
                sb.AppendLine(
                    $"<td><a href='styly-vos://assetbundle?url={WebUtility.UrlEncode(assetUrl)}&type=Bounded'>Bounded</a></td>");
                sb.AppendLine(
                    $"<td><a href='styly-vos://assetbundle?url={WebUtility.UrlEncode(assetUrl)}&type=Unbounded'>Unbounded</a></td>");
                sb.AppendLine("</tr>");
            }

            sb.AppendLine("</table>");
        }

        sb.AppendLine("</body>");
        sb.AppendLine("</html>");

        return sb.ToString();
    }
}
#endif