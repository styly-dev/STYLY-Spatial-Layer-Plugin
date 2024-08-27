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
    readonly HttpListener httpListener = new HttpListener();
    private FileSystemWatcher fileWatcher;

    public int port = 8181;
    public string path = "/";
    private bool serverRunning = false;
    private readonly string assetBundleDir = "_Output/html";
    private static readonly string VisionOsDirectoryName = "VisionOS";
    private static readonly string ThumbnailDirName = "Thumbnails";
    private static readonly string HtmlFilePath = Path.Combine("_Output", "html", "index.html");

    public void StartServer()
    {
        if (serverRunning) return;

        // サーバー起動時にHTMLファイルを静的に生成
        GenerateStaticHtmlFile();

        // ファイルシステムの監視を開始
        StartFileWatcher();

        httpListener.Prefixes.Add($"http://*:{port}{path}");
        httpListener.Start();
        serverRunning = true;

        Debug.Log("Server started.");

        RunServer().Forget();
    }

    private void GenerateStaticHtmlFile()
    {
        var filenames = GetFilenamesSortedByTime();
        var htmlContent = GenerateHtmlTable(filenames);

        // HTMLファイルを保存
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

    private async UniTaskVoid RunServer()
    {
        while (serverRunning)
        {
            var context = await httpListener.GetContextAsync();

            Debug.Log($"{context.Request.HttpMethod} Request path: {context.Request.RawUrl}");

            if (context.Request.RawUrl == "/index.html" || context.Request.RawUrl == "/")
            {
                await ServeStaticHtml(context);
                continue;
            }
            
            if (context.Request.HttpMethod == "POST")
            {
                var dataText = await new StreamReader(context.Request.InputStream,
                    context.Request.ContentEncoding).ReadToEndAsync();
                Debug.Log(dataText);
            }

            var requestPath = context.Request.RawUrl.Substring(1);

            if (requestPath == "lastfilename")
            {
                await ResponseLastFileName(context);
                continue;
            }

            if (requestPath.StartsWith(ThumbnailDirName))
            {
                await ResponseThumbnailData(context, requestPath);
                continue;
            }
            
            if (requestPath == "latest")
            {
                requestPath = GetLatestFilename();
            }

            requestPath = WWW.UnEscapeURL(requestPath);
            await ResponseFileData(context, requestPath);
        }
    }

    public void StopServer()
    {
        if (!serverRunning) return;

        httpListener.Stop();
        serverRunning = false;

        // ファイルシステムの監視を停止
        if (fileWatcher != null)
        {
            fileWatcher.EnableRaisingEvents = false;
            fileWatcher.Dispose();
        }

        Debug.Log("Server stopped.");
    }

    private async Task ServeStaticHtml(HttpListenerContext context)
    {
        byte[] buffer = await File.ReadAllBytesAsync(HtmlFilePath);
        context.Response.ContentLength64 = buffer.Length;
        context.Response.ContentType = "text/html";
        await context.Response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
        context.Response.OutputStream.Close();
    }

    private async UniTask ResponseFileData(HttpListenerContext context, string requestPath)
    {
        string assetBundlePath = Path.Combine(assetBundleDir, VisionOsDirectoryName, requestPath);

        var response = context.Response;
        
        if (File.Exists(assetBundlePath))
        {
            response.StatusCode = 200;
            var buffer = File.ReadAllBytes(assetBundlePath);
            context.Response.Headers.Add("Content-Disposition", "attachment; filename=" + requestPath);
            response.ContentType = "binary/octet-stream";
            response.ContentLength64 = buffer.Length;
            var output = response.OutputStream;
            await output.WriteAsync(buffer, 0, buffer.Length);
            output.Close();
        }
        else
        {
            response.StatusCode = 404;
        }
        context.Response.Close();
    }

    private async UniTask ResponseThumbnailData(HttpListenerContext context, string requestPath)
    {
        requestPath = WebUtility.UrlDecode(requestPath);
        string thumbnailPath = Path.Combine(assetBundleDir, requestPath);

        var response = context.Response;
    
        try
        {
            if (File.Exists(thumbnailPath))
            {
                response.StatusCode = 200;
                response.ContentType = "image/png";
                using (var fileStream = new FileStream(thumbnailPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, bufferSize: 4096, useAsync: true))
                {
                    response.ContentLength64 = fileStream.Length;
                    await fileStream.CopyToAsync(response.OutputStream);
                    response.OutputStream.Flush();
                }
            }
            else
            {
                response.StatusCode = 404;
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error while serving thumbnail: {ex.Message}");
            response.StatusCode = 500;
        }
        finally
        {
            response.OutputStream.Close();
            context.Response.Close();
        }
    }

    private async UniTask ResponseLastFileName(HttpListenerContext context)
    {
        var filename = GetLatestFilename();
        Debug.Log($"ResponseLastName:{filename}");

        var response = context.Response;
        
        response.StatusCode = 200;
        var buffer = System.Text.Encoding.UTF8.GetBytes("{\"last filename\": \"" + WWW.EscapeURL(filename) + "\"}");
        response.ContentType = "application/json";
        response.ContentLength64 = buffer.Length;
        var output = response.OutputStream;
        await output.WriteAsync(buffer, 0, buffer.Length);
        output.Close();
        context.Response.Close();
    }

    private string GetLatestFilename()
    {
        return Path.GetFileName(GetFilenamesSortedByTime().FirstOrDefault());
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

        var validFiles = files.Where(f => (Path.GetFileName(f) != "VOS" && ! System.Text.RegularExpressions
            .Regex.IsMatch(f, ".manifest")));

        var sortedFiles = validFiles.OrderByDescending(File.GetCreationTime).ToList();

        Debug.Log(string.Join(",",sortedFiles.Select(Path.GetFileName).ToList()));
        
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
                var assetUrl = $"http://{GetHostName()}:{port}/{filename}";
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
#endif