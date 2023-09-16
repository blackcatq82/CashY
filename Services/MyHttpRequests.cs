using System.Net;
using System.Text;

namespace CashY.Services
{

    public interface IMyHttpRequests
    {
        CancellationToken cancellationToken { get; set; }

        int timeOut { get; set; }

        Task<(bool, string)> GetAsync(string url);

        Task<(bool, string)> PutAsync(string Url, string paramterPut);

        Task<bool> ImagePostAsync(string url, string imagePath, string nameImage);

    }

    public class MyHttpRequests : IMyHttpRequests
    {
        private int _timeOut;
        public int timeOut { get => _timeOut; set => _timeOut = value; }
        private CancellationToken _cancellationToken;
        public CancellationToken cancellationToken { get => _cancellationToken; set => _cancellationToken = value; }

        public async Task<(bool, string)> GetAsync(string url)
        {
            try
            {
                if (timeOut <= 10)
                    timeOut = 15;
                var httpClientHandler = new HttpClientHandler();
                httpClientHandler.CookieContainer = new CookieContainer();

                using var httpClient = new HttpClient(httpClientHandler);

                httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/105.0.0.0 Safari/537.36");
                httpClient.Timeout = TimeSpan.FromSeconds(timeOut);

                // Send the GET request and get the response with cancellation support
                var response = await httpClient.GetAsync(url, cancellationToken);

                // Check if the response is successful
                if (response.IsSuccessStatusCode)
                {
                    // Read the response content as a string
                    string html = await response.Content.ReadAsStringAsync();
                    return (true, html);
                }
                else
                {
                    // Handle non-successful response
                    return (false, string.Empty);
                }
            }
            catch (OperationCanceledException)
            {
                // Handle cancellation
                return (false, "Request was canceled.");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return (false, string.Empty);
            }
        }

        public async Task<(bool, string)> PutAsync(string url, string jsonData)
        {
            try
            {
                if (timeOut <= 10)
                    timeOut = 15;

                var httpClientHandler = new HttpClientHandler();
                httpClientHandler.CookieContainer = new();

                using var httpClient = new HttpClient(httpClientHandler);

                httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/105.0.0.0 Safari/537.36");
                httpClient.Timeout = TimeSpan.FromSeconds(timeOut);

                // Create a StringContent with your JSON data
                var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                // Send the PUT request and get the response with cancellation support
                var response = await httpClient.PutAsync(url, content, cancellationToken);

                // Check if the response is successful
                if (response.IsSuccessStatusCode)
                {
                    // Read the response content as a string
                    string html = await response.Content.ReadAsStringAsync();
                    return (true, html);
                }
                else
                {
                    // Handle non-successful response
                    return (false, string.Empty);
                }
            }
            catch (OperationCanceledException)
            {
                // Handle cancellation
                return (false, "Request was canceled.");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return (false, string.Empty);
            }
        }

        public async Task<bool> ImagePostAsync(string url, string imagePath, string nameImage)
        {
            try
            {
                if (timeOut <= 10)
                    timeOut = 15;

                string imageName = nameImage;

                // Create an instance of HttpClient
                using (HttpClient client = new HttpClient())
                {
                    // Create a new MultipartFormDataContent
                    var content = new MultipartFormDataContent();

                    // Read the image data into a byte array asynchronously
                    byte[] imageBytes = await File.ReadAllBytesAsync(imagePath);

                    // Create a ByteArrayContent from the image data
                    var imageContent = new ByteArrayContent(imageBytes);

                    // Get the file extension from the image path
                    string extension = Path.GetExtension(imagePath);
                    string current_nameImage = imageName;

                    if (!imageName.Contains(extension))
                        current_nameImage = string.Format("{0}{1}", imageName, extension);

                    // Add the image as a stream content with a specific name
                    content.Add(imageContent, "image", current_nameImage);

                    // Optionally, add textual data with UTF-8 encoding
                    var textContent = new StringContent(current_nameImage, Encoding.UTF8, "text/plain");
                    content.Add(textContent, "name" );

                    // Send the POST request with the image
                    HttpResponseMessage response = await client.PostAsync(url, content);

                    // Check if the request was successful
                    if (response.IsSuccessStatusCode)
                    {
                        // Request was successful; you can handle the response here
                        string responseContent = await response.Content.ReadAsStringAsync();
                        Console.WriteLine("Response: " + responseContent);
                        if (responseContent.Contains("successfully"))
                            return true;
                        else
                            return false;
                    }
                    else
                    {
                        // Request failed; handle the error
                        Console.WriteLine("Request failed with status code: " + response.StatusCode);
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions that may occur during the request
                Console.WriteLine("Error: " + ex.Message);
                return false;
            }
        }
    }
}
