using CashY.Core;
namespace CashY.Services
{
    public interface ILoadData
    {
        string filename { get; set; }
        string message { get; set; }
        int countfiles { get; set; }
        int currentIndex {get; set; }
        long datalength { get; set; }
        long currentBytesLength { get ; set; }
        int totatfiles { get; set; }

        Task ReloadingData();
        Task ClearAllFiles();
        Task DelCateImage(int id);
        Task DelItemImage(int id);
        Task DownloadSingle(string item_image, int Id);
        Task DownloadSingleC(string item_image, int Id);

        Task<string> GetPathCates(int id, string imageName);
        Task<string> GetPathItems(int id, string imageName);
    }

    public class LoadData : ILoadData
    {
        private readonly IPopupServices popupServices;
        private readonly IDatabaseServices databaseServices;

        public string folder_items { get => _folder_items; set => _folder_items = value; }
        private string _folder_items;

        public string folder_categorys { get => _folder_categorys; set => _folder_categorys = value; }
        private string _folder_categorys;

        public string filename { get => _filename; set => _filename = value; }
        private string _filename;

        public string message { get => _message; set => _message = value; }
        private string _message;

        private int _countfiles;
        public int countfiles { get => _countfiles; set => _countfiles = value; }

        private int _currentIndex;
        public int currentIndex { get => _currentIndex; set => _currentIndex = value; }


        public long datalength { get => _datalength; set => _datalength = value; }
        private long _datalength = 0;

        public long currentBytesLength { get => _currentBytesLength; set => _currentBytesLength = value; }
        private long _currentBytesLength = 0;

        public int totatfiles { get => _totatfiles; set => _totatfiles = value; }
        private int _totatfiles = 0;
        public LoadData(IPopupServices popupServices, IDatabaseServices databaseServices)
        {
            this.popupServices = popupServices;
            this.databaseServices = databaseServices;

            folder_items     = Path.Combine(FileSystem.Current.AppDataDirectory, "items"    );
            folder_categorys = Path.Combine(FileSystem.Current.AppDataDirectory, "categorys");


            CreateFolders();
        }

        private void CreateFolders()
        {
            if(!Directory.Exists(folder_items))
                Directory.CreateDirectory(folder_items);

            if (!Directory.Exists(folder_categorys))
                Directory.CreateDirectory(folder_categorys);
        }

        public async Task ReloadingData()
        {
            try
            {
                CreateFolders();

                totatfiles = databaseServices.categories.Count + databaseServices.items.Count;
                
                await ReloadCategoryImages();
                await ReloadItemImages();
                message = "Complated to download all files as well!";
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync($"ReloadingData : {ex}");
                await popupServices.ShowPopUpMessage("Error", "Something wrong happend!", "Retry again");
            }
        }

        private async Task ReloadCategoryImages()
        {
            message = "starting to loading category images...";
            countfiles = databaseServices.categories.Count;
            foreach (var item in databaseServices.categories)
            {
                string FileExtension = Path.GetExtension(item.cate_image);
                string imageName = string.Format("{0}{1}", item.Id.ToString(), FileExtension);
                string filepath = Path.Combine(folder_categorys, imageName);

                if (File.Exists(filepath))
                {
                    currentIndex++;
                    continue;
                }

                filename = string.Format("File : {0}", filepath);

                string url = string.Format(FORMATS.DOWNLOAD_IMAGE, item.cate_image);
                await DownloadImage(filepath, url);
                currentIndex++;
            }
        }


        private async Task ReloadItemImages()
        {
            message = "starting to loading items images...";
            countfiles =  databaseServices.items.Count;
            foreach (var item in databaseServices.items)
            {
                string FileExtension = Path.GetExtension(item.Item_image);
                string imageName = string.Format("{0}{1}", item.Id.ToString(), FileExtension);
                string filepath = Path.Combine(folder_items, imageName);

                if (File.Exists(filepath))
                {
                    currentIndex++;
                    continue;
                }

                filename = string.Format("File : {0}", filepath);
                string url = string.Format(FORMATS.DOWNLOAD_IMAGE_ITEMS, item.Item_image);
                await DownloadImage(filepath, url);
                currentIndex++;
            }
        }

        private async Task DownloadImage(string path, string url)
        {
            // Create an ImageSource from the URL
            Uri imageUrl = new Uri(url);
            using(FileStream stream = File.Create(path))
            using (HttpClient httpClient = new())
            {
                // Download the image asynchronously
                byte[] imageBytes = await httpClient.GetByteArrayAsync(imageUrl);
                currentBytesLength += imageBytes.LongLength;
                // save to file
                await stream.WriteAsync(imageBytes, 0, imageBytes.Length);
                await Task.Delay(33);
            }
        }

        public async Task DownloadSingle(string item_image, int Id)
        {
            string FileExtension = Path.GetExtension(item_image);
            string imageName = string.Format("{0}{1}", Id.ToString(), FileExtension);
            string filepath = Path.Combine(folder_items, imageName);

            if (File.Exists(filepath))
            {
                return;
            }

            filename = string.Format("File : {0}", filepath);
            string url = string.Format(FORMATS.DOWNLOAD_IMAGE_ITEMS, item_image);
            await DownloadImage(filepath, url);
        }

        public async Task DownloadSingleC(string cate_image, int Id)
        {
            string FileExtension = Path.GetExtension(cate_image);
            string imageName = string.Format("{0}{1}", Id.ToString(), FileExtension);
            string filepath = Path.Combine(folder_categorys, imageName);

            if (File.Exists(filepath))
            {
                return;
            }

            filename = string.Format("File : {0}", filepath);
            string url = string.Format(FORMATS.DOWNLOAD_IMAGE, cate_image);
            await DownloadImage(filepath, url);
        }
        public Task ClearAllFiles()
        {
            List<string> files = new List<string>();

            if(Directory.Exists(folder_categorys))
                files.AddRange(Directory.GetFiles(folder_categorys));

            if (Directory.Exists(folder_items))
                files.AddRange(Directory.GetFiles(folder_items));

            foreach (string file in files)
            {
                File.Delete(file);
                Console.WriteLine("File delete : {0}", file);
            }

            return Task.CompletedTask;
        }


        public Task DelCateImage(int id)
        {
            string[] files = Directory.GetFiles(folder_categorys);
            foreach (string file in files) 
            {
                string name = string.Format("{0}{1}", id, Path.GetExtension(file));
                if(name ==  Path.GetFileName(file))
                {
                    Console.WriteLine("Founded name file:{0}", name);
                    File.Delete(file);
                    break;
                }
            }
            return Task.CompletedTask;
        }

        public Task DelItemImage(int id)
        {
            string[] files = Directory.GetFiles(folder_items);
            foreach (string file in files)
            {
                string name = string.Format("{0}{1}", id, Path.GetExtension(file));
                if (name ==  Path.GetFileName(file))
                {
                    Console.WriteLine("Founded name file:{0}", name);
                    File.Delete(file);
                    break;
                }
            }
            return Task.CompletedTask;
        }

        public Task<string> GetPathCates(int id, string imageName)
        {
            return Task.FromResult(string.Format("{0}/{1}{2}", folder_categorys, id, Path.GetExtension(imageName)));
        }
        public Task<string> GetPathItems(int id, string imageName)
        {
            return Task.FromResult(string.Format("{0}/{1}{2}", folder_items, id, Path.GetExtension(imageName)));
        }
    }
}
