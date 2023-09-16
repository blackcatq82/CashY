namespace CashY.Core
{
    public static class FORMATS
    {

        public const string URL_HOME = "http://192.168.101.48/api/core/init.php/{0}/{1}";
        public const string URL_POST_IMAGES = "http://192.168.101.48/api/index.php";
        public const string URL_POST_ITEMS_IMAGES = "http://192.168.101.48/api/items/index.php";
        public const string FORMAT_API_SETUP = "api/index.html/Get/connection?user_hash={0}";
        public const string CREATE_NEW_ACCOUNT = "api/index.html/Get/Set";
        public const string GET_CARDS_FIRST = "api/index.html/Get/cards";
        public const string GET_CARDS_ByPage = "api/index.html/Get/cards/{0}";

        public const string DOWNLOAD_IMAGE = "http://192.168.101.48/api/images/{0}";
        public const string DOWNLOAD_IMAGE_ITEMS = "http://192.168.101.48/api/items/images/{0}";


        #region PopUpMessage
        public const string Title_INFO = "إعلان تنبية هام جداً";

        public const string CLOSE = "CLOSE";
        public const string OPRN = "OPEN";



        public const string MSG_LOGIN = "Walcome again !";
        public const string MSG_LOGIN_ERROR = "Something wrong happen try again later.";





        public const string MSG_CONNECTED = "انت متصل بالانترنت";
        public const string MSG_DISCONNECTED = "انت غير متصل بالانترنت";
        public const string MSG_RECONNECTED = "تم الاتصال من جديد";
        #endregion
    }
}
