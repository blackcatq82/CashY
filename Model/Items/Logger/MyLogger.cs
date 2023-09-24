using CommunityToolkit.Mvvm.ComponentModel;
using System.Text.Json.Serialization;

namespace CashY.Model.Items.Logger
{
    public partial class MyLogger : ObservableObject
    {
        private int id;
        [JsonPropertyName("id")]
        public int Id
        {
            get => id;
            set => SetProperty(ref id, value);
        }

        private string message;
        [JsonPropertyName("message")]
        public string Message
        {
            get => message;
            set => SetProperty(ref message, value);
        }

        private string level;
        [JsonPropertyName("level")]
        public string Level
        {
            get => level;
            set => SetProperty(ref level, value);
        }

        private string date_create;
        [JsonPropertyName("date_create")]
        public string Date_create
        {
            get => date_create;
            set => SetProperty(ref date_create, value);
        }


        [ObservableProperty]
        private DateTime create_date;

        public Task CopyTo(LoggerRequest request)
        {
            this.Id = request.id;
            this.Message = request.message;
            this.Level = request.level;
            this.Create_date = request.date_create;
            return Task.CompletedTask;
        }
    }
}
